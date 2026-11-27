using System;
using System.Drawing;
using LaserChess.Logic;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	public class Board
	{
		public static bool Flip
		{
			get
			{
				return Board.flip;
			}
			set
			{
				Board.flip = value;
			}
		}

		public PointF TileSize
		{
			get
			{
				return this.tileSize;
			}
		}

		public static Direction NormalizeDirection(Direction direction)
		{
			if (!Board.Flip)
			{
				return direction;
			}
			switch (direction)
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

		public Board(Logic.Board boardLogic)
		{
			this.boardLogic = boardLogic;
		}

		public void Init(GraphicsClass graphicsClass)
		{
			if (graphicsClass == null)
			{
				return;
			}
			this.boardItem = graphicsClass.Skin.GetItem("default", "BaseGameBoard");
			this.tileSize.X = this.boardItem.attr.Width / (float)this.boardLogic.Width;
			this.tileSize.Y = this.boardItem.attr.Height / (float)this.boardLogic.Height;
		}

		public Tile GetTileAtPoint(int x, int y)
		{
			Tile tile = null;
			float num = (float)x - this.boardItem.attr.Left;
			float num2 = this.boardItem.attr.Bot - (float)y;
			if (Board.Flip)
			{
				num = this.boardItem.attr.Right - (float)x;
				num2 = (float)y - this.boardItem.attr.Top;
			}
			if (num >= 0f && num < this.boardItem.attr.Width && num2 >= 0f && num2 < this.boardItem.attr.Height)
			{
				int num3 = (int)(num / this.tileSize.X);
				int num4 = (int)(num2 / this.tileSize.Y);
				tile = this.boardLogic.GetTile(num3, num4);
			}
			return tile;
		}

		public PointF GetTileCenter(Tile tile)
		{
			PointF pointF = new PointF(0f, 0f);
			if (tile != null)
			{
				pointF.X = this.boardItem.attr.Left + (float)tile.PosX * this.tileSize.X + this.tileSize.X / 2f + 1f;
				pointF.Y = this.boardItem.attr.Bot - (float)tile.PosY * this.tileSize.Y - this.tileSize.Y / 2f - 1f;
				if (Board.Flip)
				{
					pointF.X = this.boardItem.attr.Right - (float)tile.PosX * this.tileSize.X - this.tileSize.X / 2f - 1f;
					pointF.Y = this.boardItem.attr.Top + (float)tile.PosY * this.tileSize.Y + this.tileSize.Y / 2f + 1f;
				}
			}
			return pointF;
		}

		private Logic.Board boardLogic;

		private SkinItemClass boardItem;

		private PointF tileSize;

		private static bool flip;
	}
}
