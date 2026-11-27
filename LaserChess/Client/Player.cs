using System;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	public class Player
	{
		public Player(Game game, LaserChess.Logic.Player playerLogic)
		{
			if (game == null)
			{
				throw new ArgumentNullException("game");
			}
			if (playerLogic == null)
			{
				throw new ArgumentNullException("playerLogic");
			}
			this.game = game;
			for (int i = 0; i < 18; i++)
			{
				LaserChess.Logic.Piece piece = playerLogic.GetPiece(i);
				Type type = piece.GetType();
				if (type != typeof(LaserChess.Logic.King))
				{
					if (type == typeof(LaserChess.Logic.Teleporter))
					{
						this.pieces[i] = new Teleporter(this, piece);
					}
					else
					{
						this.pieces[i] = new Piece(this, piece);
					}
				}
				else
				{
					this.pieces[i] = new King(this, piece);
				}
				this.game.RegisterPiece(this.pieces[i], playerLogic.GetPiece(i));
			}
		}

		public void Init(GraphicsClass graphicsClass, string color)
		{
			this.pieces[0].Init(graphicsClass, color, "Triangle");
			this.pieces[1].Init(graphicsClass, color, "Triangle");
			this.pieces[2].Init(graphicsClass, color, "Diagonal");
			this.pieces[3].Init(graphicsClass, color, "Teleporter");
			this.pieces[4].Init(graphicsClass, color, "King");
			this.pieces[5].Init(graphicsClass, color, "Laser");
			this.pieces[6].Init(graphicsClass, color, "Diagonal");
			this.pieces[7].Init(graphicsClass, color, "Triangle");
			this.pieces[8].Init(graphicsClass, color, "Triangle");
			this.pieces[9].Init(graphicsClass, color, "Triangle");
			this.pieces[10].Init(graphicsClass, color, "Block");
			this.pieces[11].Init(graphicsClass, color, "Block");
			this.pieces[12].Init(graphicsClass, color, "Straight");
			this.pieces[13].Init(graphicsClass, color, "Straight");
			this.pieces[14].Init(graphicsClass, color, "Splitter");
			this.pieces[15].Init(graphicsClass, color, "Block");
			this.pieces[16].Init(graphicsClass, color, "Block");
			this.pieces[17].Init(graphicsClass, color, "Triangle");
		}

		public Piece FindPiece(LaserChess.Logic.Piece pieceLogic)
		{
			return this.game.FindPiece(pieceLogic);
		}

		public void ShowAllPieces()
		{
			for (int i = 0; i < 18; i++)
			{
				this.pieces[i].Show();
			}
		}

		public void HideAllPieces()
		{
			for (int i = 0; i < 18; i++)
			{
				this.pieces[i].Hide();
			}
		}

		public Game Game
		{
			get
			{
				return this.game;
			}
		}

		private const int PieceCount = 18;

		private Piece[] pieces = new Piece[18];

		private Game game;
	}
}
