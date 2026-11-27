using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using TG.Base;
using TG.Base.SharedMemory;

namespace LaserChessServer
{
    /// <summary>
    /// Listens on the shared-memory queue that LaserChess.exe connects to when
    /// started without command-line arguments ("TestHarness" mode, see
    /// TG.Game.TGGameClass in TGGameLib), and routes messages to the session.
    /// </summary>
    internal sealed class HarnessServer : IDisposable
    {
        public const string QueueName = "TestHarnessSvrQ";
        private const int QueueSize = 65536;
        private static readonly TimeSpan LivenessInterval = TimeSpan.FromSeconds(1);

        private readonly Dictionary<string, PlayerConnection> connections = new Dictionary<string, PlayerConnection>();
        private readonly AutoResetEvent dataReady = new AutoResetEvent(false);
        private readonly GameSession session;
        private readonly List<string> ignoredAddresses = new List<string>();
        private readonly bool launchBot;
        private SharedMemClass listener;
        private volatile bool stopRequested;

        public HarnessServer(string name0, string name1, bool launchBot)
        {
            session = new GameSession(name0, name1);
            this.launchBot = launchBot;
        }

        public void Stop()
        {
            stopRequested = true;
            dataReady.Set();
        }

        public int Run()
        {
            if (SharedMemClass.IsListenerExist(QueueName))
            {
                Log.Error("Another server is already listening on {0}.", QueueName);
                return 1;
            }

            MemClass.ExpectedErrorTypes error = MemClass.ExpectedErrorTypes.None;
            listener = SharedMemClass.MakeListener(QueueSize, QueueName, ref error);
            if (listener == null)
            {
                Log.Error("Could not create queue {0}: {1}", QueueName, error);
                return 1;
            }
            // Raised on TGLib's listener thread; all real work stays on this one.
            listener.DataReceived += delegate { dataReady.Set(); };

            Log.Info("Listening on {0}. Start LaserChess.exe twice, with no arguments.", QueueName);
            Log.Info("Press Ctrl+C to stop.");

            DateTime nextLivenessCheck = DateTime.UtcNow;
            while (!stopRequested)
            {
                dataReady.WaitOne(250, false);
                Drain();
                if (DateTime.UtcNow >= nextLivenessCheck)
                {
                    CheckLiveness();
                    nextLivenessCheck = DateTime.UtcNow + LivenessInterval;
                }
                session.Tick();
            }

            Log.Info("Shutting down.");
            foreach (PlayerConnection connection in connections.Values)
            {
                connection.SendAdmin(TGMsgClass.GameMsgSubtype.AdminDisconnected, -1, null);
            }
            return 0;
        }

        private void Drain()
        {
            while (true)
            {
                Hashtable raw = null;
                bool more;
                try
                {
                    more = listener.ReceiveQueueData(ref raw);
                }
                catch (Exception ex)
                {
                    Log.Warn("Failed to read from {0}: {1}", QueueName, ex.Message);
                    return;
                }
                if (raw != null)
                {
                    try
                    {
                        Dispatch(raw);
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("Error handling message: {0}", ex);
                    }
                }
                if (!more)
                {
                    return;
                }
            }
        }

        private void Dispatch(Hashtable raw)
        {
            TGMsgClass message = new TGMsgClass(raw);
            if (!raw.Contains("MsgType") || message.Type != TGMsgClass.TGClientMsgs.GameMsg)
            {
                Log.Verbose("Ignoring non-game message: {0}", Log.Describe(raw));
                return;
            }

            string address = raw["ClientAppAddress"] as string;
            if (string.IsNullOrEmpty(address))
            {
                Log.Warn("Game message without ClientAppAddress: {0}", Log.Describe(raw));
                return;
            }

            // A GameMsg with neither SubType nor Data is the client's hello,
            // sent once its own receive queue exists.
            if (!raw.Contains("Data"))
            {
                if (!raw.Contains("SubType"))
                {
                    OnHello(address);
                }
                return;
            }

            PlayerConnection from;
            if (!connections.TryGetValue(address, out from))
            {
                if (!ignoredAddresses.Contains(address))
                {
                    ignoredAddresses.Add(address);
                    Log.Warn("Ignoring messages from client {0}, which is not at the table. Close that window.", address);
                }
                return;
            }

            Hashtable payload = Deserialize((byte[])raw["Data"]);
            Log.Verbose("<- P{0} {1}", from.PlayerNumber, Log.Describe(payload));

            if ((payload["MsgType"] as string) == "UserLeavingGame")
            {
                Drop(from, "closed the game");
                return;
            }
            session.Handle(from, payload);
        }

        private void OnHello(string address)
        {
            if (connections.ContainsKey(address))
            {
                Log.Verbose("Duplicate hello from {0}.", address);
                return;
            }

            PlayerConnection connection = PlayerConnection.Open(address);
            if (connection == null)
            {
                return;
            }

            if (session.IsFull || session.State != SessionState.Waiting)
            {
                Log.Warn("Table is full; turning away client {0}.", address);
                connection.SendAdmin(TGMsgClass.GameMsgSubtype.AdminDisconnected, -1, null);
                connection.Dispose();
                return;
            }

            connections.Add(address, connection);
            session.Seat(connection);
            if (launchBot && !session.IsFull)
            {
                StartBot();
            }
        }

        private static void StartBot()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LaserChessBot.exe");
            if (!File.Exists(path))
            {
                Log.Error("--bot: {0} not found.", path);
                return;
            }
            try
            {
                ProcessStartInfo start = new ProcessStartInfo(path);
                start.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                start.UseShellExecute = true;
                Process.Start(start);
                Log.Info("Started LaserChessBot.exe.");
            }
            catch (Exception ex)
            {
                Log.Error("--bot: could not start {0}: {1}", path, ex.Message);
            }
        }

        private void CheckLiveness()
        {
            List<PlayerConnection> dead = new List<PlayerConnection>();
            foreach (PlayerConnection connection in connections.Values)
            {
                if (!connection.IsAlive)
                {
                    dead.Add(connection);
                }
            }
            foreach (PlayerConnection connection in dead)
            {
                Drop(connection, "disconnected");
            }
        }

        private void Drop(PlayerConnection connection, string reason)
        {
            string name = connection.Name ?? connection.Address;
            connections.Remove(connection.Address);
            PlayerConnection opponent = session.Remove(connection);
            connection.Dispose();
            Log.Info("{0} {1}.", name, reason);

            if (opponent != null)
            {
                // After "Quit" the client cannot start another game, so free
                // the seat for a fresh pair of windows.
                connections.Remove(opponent.Address);
                ignoredAddresses.Add(opponent.Address);
                opponent.Dispose();
                Log.Info("{0} was told the opponent quit. Close both windows and start two new ones to play again.", opponent.Name);
            }
        }

        private static Hashtable Deserialize(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return (Hashtable)new BinaryFormatter().Deserialize(stream);
            }
        }

        public void Dispose()
        {
            foreach (PlayerConnection connection in connections.Values)
            {
                connection.Dispose();
            }
            connections.Clear();
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
            dataReady.Close();
        }
    }
}
