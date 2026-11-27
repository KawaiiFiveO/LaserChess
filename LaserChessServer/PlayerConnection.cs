using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using TG.Base;
using TG.Base.SharedMemory;

namespace LaserChessServer
{
    /// <summary>
    /// One running LaserChess.exe. Owns the shared-memory sender that writes
    /// into the game's own receive queue.
    /// </summary>
    internal sealed class PlayerConnection : IDisposable
    {
        // The game's receive queue is only 8000 bytes and is drained once per
        // rendered frame, so a full queue is normal for a moment.
        private const int SendAttempts = 40;
        private const int SendRetryDelayMs = 50;

        private SharedMemClass sender;
        private readonly int processId;

        private PlayerConnection(string address, SharedMemClass sender)
        {
            Address = address;
            this.sender = sender;
            // In TestHarness mode the client names its queue after its own PID.
            if (!int.TryParse(address, out processId))
            {
                processId = -1;
            }
        }

        /// <summary>The client's queue name ("ClientAppAddress").</summary>
        public string Address { get; private set; }

        /// <summary>0 or 1 while seated in a game, -1 otherwise.</summary>
        public int PlayerNumber { get; set; }

        public string Name { get; set; }

        /// <summary>True between a Move/Update and the client's "Ready" reply.</summary>
        public bool AwaitingReady { get; set; }

        public static PlayerConnection Open(string address)
        {
            MemClass.ExpectedErrorTypes error = MemClass.ExpectedErrorTypes.None;
            SharedMemClass sender = SharedMemClass.MakeSender(address, ref error);
            if (sender == null)
            {
                Log.Warn("Could not open queue of client {0}: {1}", address, error);
                return null;
            }
            PlayerConnection connection = new PlayerConnection(address, sender);
            connection.PlayerNumber = -1;
            return connection;
        }

        /// <summary>
        /// False once the game process has exited or closed its queue.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                if (sender == null)
                {
                    return false;
                }
                if (processId > 0)
                {
                    try
                    {
                        using (Process process = Process.GetProcessById(processId))
                        {
                            if (process.HasExited)
                            {
                                return false;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        return false; // No such process.
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                }
                // Signalled when the client disposes its listener cleanly.
                return sender.IsConnectedToValidActiveListener;
            }
        }

        /// <summary>
        /// Sends a game message: the Hashtable that GameClass receives in its
        /// GameEvent handler.
        /// </summary>
        public bool SendGame(Hashtable payload)
        {
            Log.Verbose("-> P{0} {1}", PlayerNumber, Log.Describe(payload));
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, payload);
                data = stream.ToArray();
            }
            TGMsgClass message = new TGMsgClass(TGMsgClass.TGClientMsgs.GameMsg);
            message.Add("SubType", TGMsgClass.GameMsgSubtype.Game);
            message.Add("Data", data);
            return SendRaw(message);
        }

        /// <summary>
        /// Sends an admin message. PlayerNum and PlayerData are only used by
        /// AdminConnected.
        /// </summary>
        public bool SendAdmin(TGMsgClass.GameMsgSubtype subtype, int playerNumber, Hashtable playerData)
        {
            Log.Verbose("-> {0} Admin {1}", Address, subtype);
            TGMsgClass message = new TGMsgClass(TGMsgClass.TGClientMsgs.GameMsg);
            message.Add("SubType", subtype);
            if (playerData != null)
            {
                message.Add("PlayerNum", playerNumber);
                message.Add("PlayerData", playerData);
            }
            return SendRaw(message);
        }

        private bool SendRaw(TGMsgClass message)
        {
            if (sender == null)
            {
                return false;
            }
            try
            {
                for (int attempt = 0; attempt < SendAttempts; attempt++)
                {
                    if (sender.Send(message.MessageTable))
                    {
                        return true;
                    }
                    if (sender.LastError != MemClass.ExpectedErrorTypes.InsufficientFreeMemoryForOperation)
                    {
                        break;
                    }
                    Thread.Sleep(SendRetryDelayMs);
                }
                Log.Warn("Send to client {0} failed: {1}", Address, sender.LastError);
            }
            catch (Exception ex)
            {
                Log.Warn("Send to client {0} threw: {1}", Address, ex.Message);
            }
            return false;
        }

        public void Dispose()
        {
            if (sender != null)
            {
                try
                {
                    sender.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Verbose("Disposing sender for {0} threw: {1}", Address, ex.Message);
                }
                sender = null;
            }
        }
    }
}
