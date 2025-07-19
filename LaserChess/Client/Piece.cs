using System;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	// Token: 0x02000014 RID: 20
	public class Piece : SpriteList
	{
        // Token: 0x17000021 RID: 33
        // (get) Token: 0x0600007E RID: 126 RVA: 0x000024D6 File Offset: 0x000006D6
        public LaserChess.Logic.Piece PieceLogic
        {
			get
			{
				return this.piece_0;
			}
		}

        // Token: 0x0600007F RID: 127 RVA: 0x000024DE File Offset: 0x000006DE
        public Piece(Player player_1, LaserChess.Logic.Piece piece_1)
        {
			this.player_0 = player_1;
			this.piece_0 = piece_1;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000024F4 File Offset: 0x000006F4
		private static float smethod_0(float float_1)
		{
			if (float_1 < 0f)
			{
				float_1 = 360f - -float_1 % 360f;
			}
			else
			{
				float_1 %= 360f;
			}
			return float_1;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000251A File Offset: 0x0000071A
		public virtual int Frames
		{
			get
			{
				return 24;
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000251E File Offset: 0x0000071E
		public int AngleToDisplay(float float_1)
		{
			return (int)((Piece.smethod_0(float_1) + 0.5f) * 24f / 360f) % this.Frames;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004844 File Offset: 0x00002A44
		public static float FacingToAngle(Direction direction_0)
		{
			switch (Board.NormalizeDirection(direction_0))
			{
			case Direction.Up:
				return 0f;
			case Direction.Down:
				return 180f;
			case Direction.Left:
				return 270f;
			case Direction.Right:
				return 90f;
			default:
				throw new InvalidFacingException();
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000488C File Offset: 0x00002A8C
		public void Show()
		{
			Tile tile = this.piece_0.Tile;
			base.Translation = this.player_0.Game.Board.GetTileCenter(tile);
			base.Alpha = 1f;
			base.Display = this.AngleToDisplay(Piece.FacingToAngle(this.piece_0.FrontFacing));
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002540 File Offset: 0x00000740
		public void Hide()
		{
			base.Display = -1;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000048E8 File Offset: 0x00002AE8
		public virtual void Init(GraphicsClass graphicsClass_0, string string_0, string string_1)
		{
			if (graphicsClass_0 == null)
			{
				return;
			}
			base.Add(new SkinSprite(graphicsClass_0.Skin, string_0 + string_1));
			base.Add(new SkinSprite(graphicsClass_0.Skin, "Shadow" + string_1));
			Tile tile = this.piece_0.Tile;
			base.Translation = this.player_0.Game.Board.GetTileCenter(tile);
			base.Alpha = 1f;
			base.Display = this.AngleToDisplay(Piece.FacingToAngle(this.piece_0.FrontFacing));
			base.Display = -1;
		}

        // Token: 0x04000072 RID: 114
        private LaserChess.Logic.Piece piece_0;

        // Token: 0x04000073 RID: 115
        private Player player_0;
	}
}
