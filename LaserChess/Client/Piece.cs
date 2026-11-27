using System;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	public class Piece : SpriteList
	{
		public LaserChess.Logic.Piece PieceLogic
		{
			get
			{
				return this.pieceLogic;
			}
		}

		public Piece(Player player, LaserChess.Logic.Piece pieceLogic)
		{
			this.player = player;
			this.pieceLogic = pieceLogic;
		}

		private static float NormalizeAngle(float angle)
		{
			if (angle < 0f)
			{
				angle = 360f - -angle % 360f;
			}
			else
			{
				angle %= 360f;
			}
			return angle;
		}

		public virtual int Frames
		{
			get
			{
				return 24;
			}
		}

		public int AngleToDisplay(float angle)
		{
			return (int)((Piece.NormalizeAngle(angle) + 0.5f) * 24f / 360f) % this.Frames;
		}

		public static float FacingToAngle(Direction facing)
		{
			switch (Board.NormalizeDirection(facing))
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

		public void Show()
		{
			Tile tile = this.pieceLogic.Tile;
			base.Translation = this.player.Game.Board.GetTileCenter(tile);
			base.Alpha = 1f;
			base.Display = this.AngleToDisplay(Piece.FacingToAngle(this.pieceLogic.FrontFacing));
		}

		public void Hide()
		{
			base.Display = -1;
		}

		public virtual void Init(GraphicsClass graphicsClass, string color, string kind)
		{
			if (graphicsClass == null)
			{
				return;
			}
			base.Add(new SkinSprite(graphicsClass.Skin, color + kind));
			base.Add(new SkinSprite(graphicsClass.Skin, "Shadow" + kind));
			Tile tile = this.pieceLogic.Tile;
			base.Translation = this.player.Game.Board.GetTileCenter(tile);
			base.Alpha = 1f;
			base.Display = this.AngleToDisplay(Piece.FacingToAngle(this.pieceLogic.FrontFacing));
			base.Display = -1;
		}

		private LaserChess.Logic.Piece pieceLogic;

		private Player player;
	}
}
