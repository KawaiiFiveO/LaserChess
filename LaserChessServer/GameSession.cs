using System;
using System.Collections;
using LaserChess.Logic;

namespace LaserChessServer
{
    internal enum SessionState
    {
        /// <summary>Fewer than two players seated.</summary>
        Waiting,
        /// <summary>A game is in progress.</summary>
        Playing,
        /// <summary>A king fell; waiting for both clients to finish animating.</summary>
        Finishing,
        /// <summary>Game over; New Game requests are accepted.</summary>
        Over
    }

    /// <summary>
    /// Referee for one two-player table. Owns the authoritative copy of the
    /// rules engine (LaserChess.Logic, the same DLL the client uses) and speaks
    /// the message protocol found in LaserChess.Client.GameClass.
    /// </summary>
    internal sealed class GameSession
    {
        // Upper bound on waiting for "Ready" before announcing the result, in
        // case a client is minimized or stalls mid-animation.
        private static readonly TimeSpan FinishTimeout = TimeSpan.FromSeconds(15);

        // Upper bound on holding a move until both clients have animated the
        // previous one.
        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(10);

        private readonly PlayerConnection[] seats = new PlayerConnection[2];
        private readonly string[] names;
        private Game logic;
        private int drawOfferedBy = -1;
        private int rematchRequestedBy = -1;
        private DateTime finishingSince;
        private DateTime lastUpdateAt;
        private string[] pendingResults;
        private PlayerConnection queuedMoveFrom;
        private Hashtable queuedMove;

        public GameSession(string name0, string name1)
        {
            names = new string[] { name0, name1 };
            State = SessionState.Waiting;
        }

        public SessionState State { get; private set; }

        public bool IsFull
        {
            get { return seats[0] != null && seats[1] != null; }
        }

        /// <summary>
        /// Seats a newly connected client. Returns false if the table is full.
        /// </summary>
        public bool Seat(PlayerConnection connection)
        {
            for (int i = 0; i < seats.Length; i++)
            {
                if (seats[i] == null)
                {
                    seats[i] = connection;
                    connection.PlayerNumber = i;
                    connection.Name = names[i];
                    connection.SendAdmin(TG.Base.TGMsgClass.GameMsgSubtype.AdminConnected, i, BuildPlayerData());
                    Log.Info("{0} joined as player {1} (client {2}).", names[i], i + 1, connection.Address);
                    if (IsFull)
                    {
                        StartMatch();
                    }
                    else
                    {
                        Log.Info("Waiting for a second LaserChess.exe to start...");
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a client from the table. If a match had started, the
        /// opponent is told the player quit and is returned so the caller can
        /// release it too; the client offers no way to continue after "Quit".
        /// </summary>
        public PlayerConnection Remove(PlayerConnection connection)
        {
            int seat = connection.PlayerNumber;
            if (seat < 0 || seat >= seats.Length || seats[seat] != connection)
            {
                return null;
            }
            bool matchStarted = State != SessionState.Waiting;
            seats[seat] = null;
            connection.PlayerNumber = -1;

            PlayerConnection opponent = seats[1 - seat];
            if (opponent != null && matchStarted)
            {
                Send(opponent, "GameOver", "Result", "Quit");
                seats[1 - seat] = null;
                opponent.PlayerNumber = -1;
            }
            else
            {
                opponent = null;
            }

            ResetTable();
            return opponent;
        }

        public void Handle(PlayerConnection from, Hashtable message)
        {
            string msgType = message["MsgType"] as string;
            string subType = message["SubType"] as string;
            switch (msgType)
            {
                case "Move":
                    if (subType == "Request")
                    {
                        HandleMoveRequest(from, message);
                    }
                    break;
                case "Ready":
                    from.AwaitingReady = false;
                    TryProcessQueuedMove();
                    TryFinish();
                    break;
                case "Chat":
                    HandleChat(from, message);
                    break;
                case "Draw":
                    HandleDraw(from, subType);
                    break;
                case "Resign":
                    if (subType == "Request" && State == SessionState.Playing)
                    {
                        Log.Info("{0} resigned.", from.Name);
                        int loser = from.PlayerNumber;
                        EndGame(loser == 0 ? "Lose" : "Win", loser == 1 ? "Lose" : "Win");
                    }
                    break;
                case "Game":
                    HandleRematch(from, subType);
                    break;
                default:
                    Log.Warn("Unhandled message from {0}: {1}", from.Name, Log.Describe(message));
                    break;
            }
        }

        /// <summary>Called periodically by the server loop.</summary>
        public void Tick()
        {
            TryProcessQueuedMove();
            TryFinish();
        }

        private void StartMatch()
        {
            for (int i = 0; i < seats.Length; i++)
            {
                Hashtable ready = new Hashtable();
                ready.Add("MsgType", "Ready");
                ready.Add("YourPlayerNum", i);
                ready.Add(0, names[0]);
                ready.Add(1, names[1]);
                seats[i].SendGame(ready);
            }
            NewGame();
        }

        private void NewGame()
        {
            logic = new Game();
            drawOfferedBy = -1;
            rematchRequestedBy = -1;
            pendingResults = null;
            queuedMove = null;
            queuedMoveFrom = null;
            foreach (PlayerConnection seat in seats)
            {
                seat.AwaitingReady = false;
            }
            State = SessionState.Playing;
            Broadcast("Game", "SubType", "Reset");
            Log.Info("New game: {0} vs {1}. {0} moves first.", names[0], names[1]);
        }

        private void HandleMoveRequest(PlayerConnection from, Hashtable message)
        {
            if (State != SessionState.Playing || queuedMove != null)
            {
                Send(from, "Move", "SubType", "Decline");
                return;
            }
            if (seats[0].AwaitingReady || seats[1].AwaitingReady)
            {
                // A client is still animating the previous move. The client
                // applies updates immediately, so sending the next one now
                // would cut that animation short.
                queuedMoveFrom = from;
                queuedMove = message;
                return;
            }
            ProcessMove(from, message);
        }

        private void TryProcessQueuedMove()
        {
            if (queuedMove == null)
            {
                return;
            }
            bool allReady = !seats[0].AwaitingReady && !seats[1].AwaitingReady;
            if (allReady || DateTime.UtcNow - lastUpdateAt > ReadyTimeout)
            {
                PlayerConnection from = queuedMoveFrom;
                Hashtable message = queuedMove;
                queuedMoveFrom = null;
                queuedMove = null;
                if (State == SessionState.Playing)
                {
                    ProcessMove(from, message);
                }
            }
        }

        private void ProcessMove(PlayerConnection from, Hashtable message)
        {
            int piece;
            MoveCommand command;
            Direction direction;
            try
            {
                piece = Convert.ToInt32(message["Piece"]);
                command = (MoveCommand)message["Command"];
                direction = (Direction)message["Direction"];
            }
            catch (Exception ex)
            {
                Log.Warn("Malformed move from {0}: {1}", from.Name, ex.Message);
                Send(from, "Move", "SubType", "Decline");
                return;
            }

            if (!logic.ProcessMoveCommand(from.PlayerNumber, piece, command, direction))
            {
                Log.Warn("Rejected illegal move from {0}: piece {1} {2} {3}.", from.Name, piece, command, direction);
                Send(from, "Move", "SubType", "Decline");
                return;
            }

            Log.Verbose("{0}: piece {1} {2} {3}", from.Name, piece, command, direction);
            Hashtable update = new Hashtable();
            update.Add("MsgType", "Move");
            update.Add("SubType", "Update");
            update.Add("Player", from.PlayerNumber);
            update.Add("Piece", piece);
            update.Add("Command", command);
            update.Add("Direction", direction);
            lastUpdateAt = DateTime.UtcNow;
            foreach (PlayerConnection seat in seats)
            {
                seat.AwaitingReady = true;
                seat.SendGame(update);
            }

            if (logic.GameOver)
            {
                Player winner = logic.Winner;
                if (winner == null)
                {
                    pendingResults = new string[] { "Draw", "Draw" };
                    Log.Info("Both kings destroyed.");
                }
                else
                {
                    int w = winner.PlayerNumber;
                    pendingResults = new string[] { w == 0 ? "Win" : "Lose", w == 1 ? "Win" : "Lose" };
                    Log.Info("{0}'s king was destroyed.", names[1 - w]);
                }
                // Let the final laser and capture animations finish before
                // the result popup appears.
                State = SessionState.Finishing;
                finishingSince = DateTime.UtcNow;
            }
        }

        private void TryFinish()
        {
            if (State != SessionState.Finishing)
            {
                return;
            }
            bool allReady = !seats[0].AwaitingReady && !seats[1].AwaitingReady;
            if (allReady || DateTime.UtcNow - finishingSince > FinishTimeout)
            {
                EndGame(pendingResults[0], pendingResults[1]);
            }
        }

        private void HandleChat(PlayerConnection from, Hashtable message)
        {
            string text = message["Text"] as string;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            Log.Info("[chat] {0}: {1}", from.Name, text);
            PlayerConnection opponent = Opponent(from);
            if (opponent == null)
            {
                return;
            }
            Hashtable chat = new Hashtable();
            chat.Add("MsgType", "Chat");
            chat.Add("Player", from.PlayerNumber);
            chat.Add("Text", text);
            opponent.SendGame(chat);
        }

        private void HandleDraw(PlayerConnection from, string subType)
        {
            if (State != SessionState.Playing)
            {
                return;
            }
            int me = from.PlayerNumber;
            int them = 1 - me;
            switch (subType)
            {
                case "Request":
                    if (drawOfferedBy == them)
                    {
                        // Both offered at once: treat as agreed.
                        Log.Info("Both players offered a draw.");
                        EndGame("Draw", "Draw");
                    }
                    else if (drawOfferedBy < 0)
                    {
                        drawOfferedBy = me;
                        Log.Info("{0} offered a draw.", from.Name);
                        Send(seats[them], "Draw", "SubType", "Request");
                    }
                    break;
                case "Accept":
                    if (drawOfferedBy == them)
                    {
                        Log.Info("{0} accepted the draw.", from.Name);
                        EndGame("Draw", "Draw");
                    }
                    break;
                case "Decline":
                    if (drawOfferedBy == them)
                    {
                        Log.Info("{0} declined the draw.", from.Name);
                        drawOfferedBy = -1;
                        Send(seats[them], "Draw", "SubType", "Decline");
                    }
                    break;
            }
        }

        private void HandleRematch(PlayerConnection from, string subType)
        {
            if (State != SessionState.Over)
            {
                return;
            }
            int me = from.PlayerNumber;
            int them = 1 - me;
            switch (subType)
            {
                case "Request":
                    if (rematchRequestedBy == them)
                    {
                        NewGame();
                    }
                    else if (rematchRequestedBy < 0)
                    {
                        rematchRequestedBy = me;
                        Log.Info("{0} asked for a new game.", from.Name);
                        Send(seats[them], "Game", "SubType", "Request");
                    }
                    break;
                case "Accept":
                    if (rematchRequestedBy == them)
                    {
                        NewGame();
                    }
                    break;
                case "Decline":
                    if (rematchRequestedBy == them)
                    {
                        Log.Info("{0} declined a new game.", from.Name);
                        rematchRequestedBy = -1;
                        Send(seats[them], "Game", "SubType", "Decline");
                    }
                    break;
            }
        }

        private void EndGame(string result0, string result1)
        {
            State = SessionState.Over;
            drawOfferedBy = -1;
            rematchRequestedBy = -1;
            pendingResults = null;
            queuedMove = null;
            queuedMoveFrom = null;
            Send(seats[0], "GameOver", "Result", result0);
            Send(seats[1], "GameOver", "Result", result1);
            Log.Info("Game over: {0} {1}, {2} {3}.", names[0], result0, names[1], result1);
        }

        private void ResetTable()
        {
            logic = null;
            drawOfferedBy = -1;
            rematchRequestedBy = -1;
            pendingResults = null;
            queuedMove = null;
            queuedMoveFrom = null;
            State = SessionState.Waiting;
        }

        private Hashtable BuildPlayerData()
        {
            // GameClass reads entries 0 and 1 as Hashtables and only looks for
            // an optional "AvatarHash", which we leave out so no avatar
            // download is requested.
            Hashtable data = new Hashtable();
            for (int i = 0; i < names.Length; i++)
            {
                Hashtable player = new Hashtable();
                player.Add("Name", names[i]);
                data.Add(i, player);
            }
            return data;
        }

        private PlayerConnection Opponent(PlayerConnection connection)
        {
            return seats[1 - connection.PlayerNumber];
        }

        private void Broadcast(string msgType, string key, object value)
        {
            foreach (PlayerConnection seat in seats)
            {
                Send(seat, msgType, key, value);
            }
        }

        private static void Send(PlayerConnection to, string msgType, string key, object value)
        {
            if (to == null)
            {
                return;
            }
            Hashtable message = new Hashtable();
            message.Add("MsgType", msgType);
            message.Add(key, value);
            to.SendGame(message);
        }
    }
}
