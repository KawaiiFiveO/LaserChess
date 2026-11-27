using System;
using LaserChess.Logic;

namespace LaserChess.Client
{
	public class MoveRequestEventArgs : EventArgs
	{
		public MoveCommand MoveCommand
		{
			get
			{
				return this.moveCommand;
			}
		}

		public int PlayerNumber
		{
			get
			{
				return this.playerNumber;
			}
		}

		public int PieceNumber
		{
			get
			{
				return this.pieceNumber;
			}
		}

		public Direction Direction
		{
			get
			{
				return this.direction;
			}
		}

		public MoveRequestEventArgs(int playerNumber, int pieceNumber, MoveCommand moveCommand, Direction direction)
		{
			this.playerNumber = playerNumber;
			this.pieceNumber = pieceNumber;
			this.moveCommand = moveCommand;
			this.direction = direction;
		}

		private MoveCommand moveCommand;

		private int playerNumber;

		private int pieceNumber;

		private Direction direction;
	}
}
