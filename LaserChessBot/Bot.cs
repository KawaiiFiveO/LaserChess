using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using LaserChess.Logic;
using TG.Game;

namespace LaserChessBot
{
    /// <summary>
    /// Headless opponent. Connects to LaserChessServer through the same
    /// TGGameClass the game window uses and plays a greedy one-move lookahead.
    /// </summary>
    internal sealed class Bot : IDisposable
    {
        private static readonly TimeSpan ThinkTime = TimeSpan.FromMilliseconds(700);
        private static readonly MoveCommand[] Commands = { MoveCommand.MovePiece, MoveCommand.TurnPiece, MoveCommand.FireLaser };
        private static readonly Direction[] Directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        private readonly TGGameClass tgGame = new TGGameClass();
        private readonly AutoResetEvent dataAvailable = new AutoResetEvent(false);
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private readonly Random random = new Random();
        private readonly List<Move> history = new List<Move>();
        private Game logic;
        private int seat = -1;
        private bool inGame;
        private bool awaitingResponse;
        private bool greeted;
        private DateTime nextMoveAt;

        public int ExitCode { get; private set; }

        public void Start()
        {
            tgGame.GameEvent += OnGameEvent;
            tgGame.AdminEvent += OnAdminEvent;
            tgGame.DataEvent += delegate { dataAvailable.Set(); };
            tgGame.Start(8000, true);
            timer.Interval = 15;
            timer.Tick += OnTick;
            timer.Start();
            Say("Waiting for LaserChessServer...");
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (dataAvailable.WaitOne(0, false))
            {
                tgGame.RetrieveMessages();
            }
            if (!inGame || awaitingResponse || logic == null || logic.GameOver)
            {
                return;
            }
            if (logic.CurrentPlayerNumber != seat || DateTime.UtcNow < nextMoveAt)
            {
                return;
            }

            Move move = ChooseMove();
            if (move == null)
            {
                Say("No legal move; resigning.");
                Send("MsgType", "Resign", "SubType", "Request");
                inGame = false;
                return;
            }
            awaitingResponse = true;
            Send("MsgType", "Move", "SubType", "Request",
                 "Piece", move.Piece, "Command", move.Command, "Direction", move.Direction);
        }

        private Move ChooseMove()
        {
            List<Move> candidates = new List<Move>();
            foreach (Piece piece in logic.GetPlayer(seat).PieceCollection)
            {
                if (piece.IsDestroyed || !piece.IsReady)
                {
                    continue;
                }
                foreach (MoveCommand command in Commands)
                {
                    foreach (Direction direction in Directions)
                    {
                        if (piece.MoveIsValid(command, direction))
                        {
                            candidates.Add(new Move(seat, piece.PieceNumber, command, direction));
                        }
                    }
                }
            }

            // LaserChess.Logic has no way to copy a game, so each candidate is
            // tried on a fresh board with the game so far replayed onto it.
            Move best = null;
            int bestScore = int.MinValue;
            foreach (Move candidate in candidates)
            {
                Game trial = Replay();
                if (!trial.ProcessMoveCommand(seat, candidate.Piece, candidate.Command, candidate.Direction))
                {
                    continue;
                }
                int score = Score(trial) + random.Next(4);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }
            return best;
        }

        private int Score(Game trial)
        {
            Player me = trial.GetPlayer(seat);
            Player them = trial.GetPlayer(1 - seat);
            int score = 0;
            if (them.King.IsDestroyed)
            {
                score += 100000;
            }
            if (me.King.IsDestroyed)
            {
                score -= 100000;
            }
            foreach (Piece piece in them.PieceCollection)
            {
                if (piece.IsDestroyed)
                {
                    score += 10;
                }
            }
            foreach (Piece piece in me.PieceCollection)
            {
                if (piece.IsDestroyed)
                {
                    score -= 12;
                }
            }
            return score;
        }

        private Game Replay()
        {
            Game game = new Game();
            foreach (Move move in history)
            {
                game.ProcessMoveCommand(move.Player, move.Piece, move.Command, move.Direction);
            }
            return game;
        }

        private void OnAdminEvent(object sender, AdminEventArgs e)
        {
            switch (e.Type)
            {
                case AdminEventType.Connected:
                    Say("Connected. Waiting for an opponent...");
                    break;
                case AdminEventType.Disconnected:
                case AdminEventType.Shutdown:
                    Say("Server closed the connection.");
                    Stop();
                    break;
            }
        }

        private void OnGameEvent(object sender, GameEventArgs e)
        {
            Hashtable message = e.Value;
            string subType = message["SubType"] as string;
            switch (message["MsgType"] as string)
            {
                case "Ready":
                    seat = (int)message["YourPlayerNum"];
                    Say("Playing as {0} against {1}.", message[seat], message[1 - seat]);
                    break;

                case "Game":
                    if (subType == "Reset")
                    {
                        logic = new Game();
                        history.Clear();
                        inGame = true;
                        awaitingResponse = false;
                        nextMoveAt = DateTime.UtcNow + ThinkTime;
                        Say("New game. {0}", seat == 0 ? "Bot moves first." : "You move first.");
                    }
                    else if (subType == "Request")
                    {
                        Say("Rematch requested; accepting.");
                        Send("MsgType", "Game", "SubType", "Accept");
                    }
                    break;

                case "Move":
                    if (subType == "Update")
                    {
                        Move move = new Move((int)message["Player"], (int)message["Piece"],
                                             (MoveCommand)message["Command"], (Direction)message["Direction"]);
                        if (!logic.ProcessMoveCommand(move.Player, move.Piece, move.Command, move.Direction))
                        {
                            Say("Out of sync with the server; leaving.");
                            Stop();
                            return;
                        }
                        history.Add(move);
                        if (move.Player == seat)
                        {
                            awaitingResponse = false;
                        }
                        nextMoveAt = DateTime.UtcNow + ThinkTime;
                        Send("MsgType", "Ready");
                    }
                    else if (subType == "Decline")
                    {
                        awaitingResponse = false;
                        nextMoveAt = DateTime.UtcNow + ThinkTime;
                    }
                    break;

                case "Chat":
                    Say("Chat: {0}", message["Text"]);
                    if (!greeted)
                    {
                        greeted = true;
                        Send("MsgType", "Chat", "Player", seat, "Text", "I'm a bot. Good luck!");
                    }
                    break;

                case "Draw":
                    if (subType == "Request")
                    {
                        Say("Draw offered; declining.");
                        Send("MsgType", "Draw", "SubType", "Decline");
                    }
                    break;

                case "GameOver":
                    inGame = false;
                    string result = message["Result"] as string;
                    if (result == "Quit")
                    {
                        Say("Opponent left.");
                        Stop();
                        return;
                    }
                    Say("Game over: bot {0}.", result == "Win" ? "wins" : result == "Lose" ? "loses" : "draws");
                    break;
            }
        }

        private void Send(params object[] keysAndValues)
        {
            Hashtable message = new Hashtable();
            for (int i = 0; i < keysAndValues.Length; i += 2)
            {
                message.Add(keysAndValues[i], keysAndValues[i + 1]);
            }
            if (!tgGame.SendToServer(message))
            {
                Say("Send failed: {0}", tgGame.TGGameSendErr);
            }
        }

        private void Stop()
        {
            timer.Stop();
            Application.Exit();
        }

        private static void Say(string format, params object[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "  " + string.Format(format, args));
        }

        public void Dispose()
        {
            timer.Dispose();
            tgGame.Dispose();
            dataAvailable.Close();
        }

        private sealed class Move
        {
            public Move(int player, int piece, MoveCommand command, Direction direction)
            {
                Player = player;
                Piece = piece;
                Command = command;
                Direction = direction;
            }

            public int Player { get; private set; }
            public int Piece { get; private set; }
            public MoveCommand Command { get; private set; }
            public Direction Direction { get; private set; }
        }
    }
}
