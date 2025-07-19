using System;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	// Token: 0x02000013 RID: 19
	public class Player
	{
        // Token: 0x06000078 RID: 120 RVA: 0x000045A8 File Offset: 0x000027A8
        public Player(Game game_1, LaserChess.Logic.Player player_0)
        {
			if (game_1 == null)
			{
				throw new ArgumentNullException("game");
			}
			if (player_0 == null)
			{
				throw new ArgumentNullException("playerLogic");
			}
			this.game_0 = game_1;
			for (int i = 0; i < 18; i++)
			{
                LaserChess.Logic.Piece piece = player_0.GetPiece(i);
                Type type = piece.GetType();
				if (type != typeof(King))
				{
					if (type == typeof(Teleporter))
					{
						this.piece_0[i] = new Teleporter(this, piece);
					}
					else
					{
						this.piece_0[i] = new Piece(this, piece);
					}
				}
				else
				{
					this.piece_0[i] = new King(this, piece);
				}
				this.game_0.RegisterPiece(this.piece_0[i], player_0.GetPiece(i));
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004674 File Offset: 0x00002874
		public void Init(GraphicsClass graphicsClass_0, string string_0)
		{
			this.piece_0[0].Init(graphicsClass_0, string_0, "Triangle");
			this.piece_0[1].Init(graphicsClass_0, string_0, "Triangle");
			this.piece_0[2].Init(graphicsClass_0, string_0, "Diagonal");
			this.piece_0[3].Init(graphicsClass_0, string_0, "Teleporter");
			this.piece_0[4].Init(graphicsClass_0, string_0, "King");
			this.piece_0[5].Init(graphicsClass_0, string_0, "Laser");
			this.piece_0[6].Init(graphicsClass_0, string_0, "Diagonal");
			this.piece_0[7].Init(graphicsClass_0, string_0, "Triangle");
			this.piece_0[8].Init(graphicsClass_0, string_0, "Triangle");
			this.piece_0[9].Init(graphicsClass_0, string_0, "Triangle");
			this.piece_0[10].Init(graphicsClass_0, string_0, "Block");
			this.piece_0[11].Init(graphicsClass_0, string_0, "Block");
			this.piece_0[12].Init(graphicsClass_0, string_0, "Straight");
			this.piece_0[13].Init(graphicsClass_0, string_0, "Straight");
			this.piece_0[14].Init(graphicsClass_0, string_0, "Splitter");
			this.piece_0[15].Init(graphicsClass_0, string_0, "Block");
			this.piece_0[16].Init(graphicsClass_0, string_0, "Block");
			this.piece_0[17].Init(graphicsClass_0, string_0, "Triangle");
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000024C0 File Offset: 0x000006C0
		public Piece FindPiece(LaserChess.Logic.Piece piece_1)
		{
			return this.game_0.FindPiece(piece_1);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000047F4 File Offset: 0x000029F4
		public void ShowAllPieces()
		{
			for (int i = 0; i < 18; i++)
			{
				this.piece_0[i].Show();
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000481C File Offset: 0x00002A1C
		public void HideAllPieces()
		{
			for (int i = 0; i < 18; i++)
			{
				this.piece_0[i].Hide();
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000024CE File Offset: 0x000006CE
		public Game Game
		{
			get
			{
				return this.game_0;
			}
		}

		// Token: 0x0400006F RID: 111
		private const int int_0 = 18;

		// Token: 0x04000070 RID: 112
		private Piece[] piece_0 = new Piece[18];

		// Token: 0x04000071 RID: 113
		private Game game_0;
	}
}
