using System;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	// Token: 0x02000016 RID: 22
	public class Teleporter : Piece
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00002549 File Offset: 0x00000749
		public override int Frames
		{
			get
			{
				return 1;
			}
		}

        // Token: 0x0600008A RID: 138 RVA: 0x0000254C File Offset: 0x0000074C
        public Teleporter(Player player_1, LaserChess.Logic.Piece piece_1)
            : base(player_1, piece_1)
		{
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00002556 File Offset: 0x00000756
		public override void Init(GraphicsClass graphicsClass_0, string string_0, string string_1)
		{
			if (graphicsClass_0 == null)
			{
				return;
			}
			base.Add(new SkinSprite(graphicsClass_0.Skin, string_0 + "Bubble"));
			base.Init(graphicsClass_0, string_0, string_1);
		}
	}
}
