using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	public class Game : IDisposable
	{
		public SoundSystem SoundSystem
		{
			get
			{
				return this.soundSystem;
			}
		}

		public int LocalPlayerNumber
		{
			get
			{
				return this.localPlayerNumber;
			}
			set
			{
				this.localPlayerNumber = value;
				Board.Flip = this.localPlayerNumber == 1;
				for (int i = 0; i < 2; i++)
				{
					this.players[i].ShowAllPieces();
				}
			}
		}

		public event EventHandler<MoveRequestEventArgs> MoveRequestEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.moveRequestHandler = (EventHandler<MoveRequestEventArgs>)Delegate.Combine(this.moveRequestHandler, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.moveRequestHandler = (EventHandler<MoveRequestEventArgs>)Delegate.Remove(this.moveRequestHandler, value);
			}
		}

		public Logic.Game GameLogic
		{
			get
			{
				return this.gameLogic;
			}
		}

		public Collection<Piece> PieceList
		{
			get
			{
				return this.pieceList;
			}
		}

		public Board Board
		{
			get
			{
				return this.board;
			}
		}

		public Game(Logic.Game gameLogic, Control control)
		{
			this.soundSystem = new SoundSystem(control);
			this.gameLogic = gameLogic;
			this.board = new Board(this.gameLogic.Board);
			for (int i = 0; i < 2; i++)
			{
				this.players[i] = new Player(this, this.gameLogic.GetPlayer(i));
			}
		}

		public void Init(GraphicsClass graphicsClass)
		{
			if (graphicsClass == null)
			{
				return;
			}
			this.soundSystem.Init(graphicsClass);
			this.board.Init(graphicsClass);
			this.players[0].Init(graphicsClass, "White");
			this.players[1].Init(graphicsClass, "Black");
			this.animationList.Add(new MovePieceAnimation(this, graphicsClass.Skin));
			this.animationList.Add(new TurnPieceAnimation(this, graphicsClass.Skin));
			this.animationList.Add(new FireLaserAnimation(this, graphicsClass.Skin));
			this.animationList.Add(new CaptureAnimation(this, graphicsClass.Skin));
			this.animationList.Add(new TeleportAnimation(this, graphicsClass.Skin));
		}

		public void RegisterPiece(Piece piece, LaserChess.Logic.Piece pieceLogic)
		{
			this.pieceList.Add(piece);
			this.piecesByLogic[pieceLogic] = piece;
		}

		public Piece FindPiece(LaserChess.Logic.Piece pieceLogic)
		{
			return (Piece)this.piecesByLogic[pieceLogic];
		}

		public void Animate(float elapsed)
		{
			try
			{
				this.animationList.Update(elapsed);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"Exception occurred in ",
					ex.Source,
					Environment.NewLine,
					ex.Message,
					Environment.NewLine,
					ex.StackTrace
				}), "TrayGames Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		public AnimationList AnimationList
		{
			get
			{
				return this.animationList;
			}
		}

		public void RaiseMoveRequestEvent(int playerNumber, int pieceNumber, MoveCommand moveCommand, Direction direction)
		{
			if (this.moveRequestHandler != null)
			{
				this.moveRequestHandler(this, new MoveRequestEventArgs(playerNumber, pieceNumber, moveCommand, direction));
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.disposed)
				{
					if (disposing && this.soundSystem != null)
					{
						this.soundSystem.Dispose();
						this.soundSystem = null;
					}
					this.disposed = true;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private const int PlayerCount = 2;

		private bool disposed;

		private SoundSystem soundSystem;

		private int localPlayerNumber;

		private AnimationList animationList = new AnimationList();

		private EventHandler<MoveRequestEventArgs> moveRequestHandler;

		private Logic.Game gameLogic;

		private Board board;

		private Player[] players = new Player[2];

		private Hashtable piecesByLogic = new Hashtable();

		private Collection<Piece> pieceList = new Collection<Piece>();
	}
}
