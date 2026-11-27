using System;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	public class Teleporter : Piece
	{
		public override int Frames
		{
			get
			{
				return 1;
			}
		}

		public Teleporter(Player player, LaserChess.Logic.Piece pieceLogic)
			: base(player, pieceLogic)
		{
		}

		public override void Init(GraphicsClass graphicsClass, string color, string kind)
		{
			if (graphicsClass == null)
			{
				return;
			}
			base.Add(new SkinSprite(graphicsClass.Skin, color + "Bubble"));
			base.Init(graphicsClass, color, kind);
		}
	}
}
