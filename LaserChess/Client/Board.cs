using System;
using System.Drawing;
using LaserChess.Logic;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x0200000B RID: 11
	public class Board
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002231 File Offset: 0x00000431
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002238 File Offset: 0x00000438
		public static bool Flip
		{
			get
			{
				return Board.bool_0;
			}
			set
			{
				Board.bool_0 = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002240 File Offset: 0x00000440
		public PointF TileSize
		{
			get
			{
				return this.pointF_0;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003638 File Offset: 0x00001838
		public static Direction NormalizeDirection(Direction direction_0)
		{
			if (!Board.Flip)
			{
				return direction_0;
			}
			switch (direction_0)
			{
			case Direction.Up:
				return Direction.Down;
			case Direction.Down:
				return Direction.Up;
			case Direction.Left:
				return Direction.Right;
			case Direction.Right:
				return Direction.Left;
			default:
				throw new InvalidFacingException();
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002248 File Offset: 0x00000448
		public Board(Logic.Board board_1)
		{
			this.board_0 = board_1;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003674 File Offset: 0x00001874
		public void Init(GraphicsClass graphicsClass_0)
		{
			if (graphicsClass_0 == null)
			{
				return;
			}
			this.skinItemClass_0 = graphicsClass_0.Skin.GetItem("default", "BaseGameBoard");
			this.pointF_0.X = this.skinItemClass_0.attr.Width / (float)this.board_0.Width;
			this.pointF_0.Y = this.skinItemClass_0.attr.Height / (float)this.board_0.Height;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000036F0 File Offset: 0x000018F0
		public Tile GetTileAtPoint(int int_0, int int_1)
		{
			Tile tile = null;
			float num = (float)int_0 - this.skinItemClass_0.attr.Left;
			float num2 = this.skinItemClass_0.attr.Bot - (float)int_1;
			if (Board.Flip)
			{
				num = this.skinItemClass_0.attr.Right - (float)int_0;
				num2 = (float)int_1 - this.skinItemClass_0.attr.Top;
			}
			if (num >= 0f && num < this.skinItemClass_0.attr.Width && num2 >= 0f && num2 < this.skinItemClass_0.attr.Height)
			{
				int num3 = (int)(num / this.pointF_0.X);
				int num4 = (int)(num2 / this.pointF_0.Y);
				tile = this.board_0.GetTile(num3, num4);
			}
			return tile;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000037BC File Offset: 0x000019BC
		public PointF GetTileCenter(Tile tile_0)
		{
			PointF pointF = new PointF(0f, 0f);
			if (tile_0 != null)
			{
				pointF.X = this.skinItemClass_0.attr.Left + (float)tile_0.PosX * this.pointF_0.X + this.pointF_0.X / 2f + 1f;
				pointF.Y = this.skinItemClass_0.attr.Bot - (float)tile_0.PosY * this.pointF_0.Y - this.pointF_0.Y / 2f - 1f;
				if (Board.Flip)
				{
					pointF.X = this.skinItemClass_0.attr.Right - (float)tile_0.PosX * this.pointF_0.X - this.pointF_0.X / 2f - 1f;
					pointF.Y = this.skinItemClass_0.attr.Top + (float)tile_0.PosY * this.pointF_0.Y + this.pointF_0.Y / 2f + 1f;
				}
			}
			return pointF;
		}

		// Token: 0x04000023 RID: 35
		private Logic.Board board_0;

		// Token: 0x04000024 RID: 36
		private SkinItemClass skinItemClass_0;

		// Token: 0x04000025 RID: 37
		private PointF pointF_0;

		// Token: 0x04000026 RID: 38
		private static bool bool_0;
	}
}
