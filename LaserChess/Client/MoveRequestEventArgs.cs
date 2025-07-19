using System;
using LaserChess.Logic;

namespace LaserChess.Client
{
	// Token: 0x02000012 RID: 18
	public class MoveRequestEventArgs : EventArgs
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000073 RID: 115 RVA: 0x0000247B File Offset: 0x0000067B
		public MoveCommand MoveCommand
		{
			get
			{
				return this.moveCommand_0;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00002483 File Offset: 0x00000683
		public int PlayerNumber
		{
			get
			{
				return this.int_0;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000248B File Offset: 0x0000068B
		public int PieceNumber
		{
			get
			{
				return this.int_1;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00002493 File Offset: 0x00000693
		public Direction Direction
		{
			get
			{
				return this.direction_0;
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000249B File Offset: 0x0000069B
		public MoveRequestEventArgs(int int_2, int int_3, MoveCommand moveCommand_1, Direction direction_1)
		{
			this.int_0 = int_2;
			this.int_1 = int_3;
			this.moveCommand_0 = moveCommand_1;
			this.direction_0 = direction_1;
		}

		// Token: 0x0400006B RID: 107
		private MoveCommand moveCommand_0;

		// Token: 0x0400006C RID: 108
		private int int_0;

		// Token: 0x0400006D RID: 109
		private int int_1;

		// Token: 0x0400006E RID: 110
		private Direction direction_0;
	}
}
