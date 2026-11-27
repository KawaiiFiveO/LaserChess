using System;
using LaserChess.Logic;

namespace LaserChess.Client
{
	public class King : Piece
	{
		public override int Frames
		{
			get
			{
				return 1;
			}
		}

		public King(Player player, LaserChess.Logic.Piece pieceLogic)
			: base(player, pieceLogic)
		{
		}
	}
}
