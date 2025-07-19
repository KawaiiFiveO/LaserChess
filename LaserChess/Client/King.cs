using System;
using LaserChess.Logic;

namespace LaserChess.Client
{
	// Token: 0x02000015 RID: 21
	public class King : Piece
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00002549 File Offset: 0x00000749
		public override int Frames
		{
			get
			{
				return 1;
			}
		}

        // Token: 0x06000088 RID: 136 RVA: 0x0000254C File Offset: 0x0000074C
        public King(Player player_1, LaserChess.Logic.Piece piece_1)
            : base(player_1, piece_1)
		{
		}
	}
}
