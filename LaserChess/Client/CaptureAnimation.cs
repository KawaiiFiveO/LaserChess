using System;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x0200000F RID: 15
	public class CaptureAnimation : Animation
	{
		// Token: 0x0600005A RID: 90 RVA: 0x000041A0 File Offset: 0x000023A0
		public CaptureAnimation(Game game_1, SkinningClass skinningClass_0)
		{
			this.game_0 = game_1;
			foreach (Piece piece in game_1.PieceList)
			{
				piece.PieceLogic.CaptureEvent += this.method_0;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004218 File Offset: 0x00002418
		private void method_0(object sender, CaptureEventArgs e)
		{
			this.tile_0 = e.Tile1;
			this.tile_1 = e.Tile2;
			this.piece_0 = this.game_0.FindPiece(e.Piece1);
			this.piece_1 = this.game_0.FindPiece(e.Piece2);
			this.method_1();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002380 File Offset: 0x00000580
		public override bool IsIdle()
		{
			return this.enum1_0 == CaptureAnimation.Enum1.const_0;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004274 File Offset: 0x00002474
		public override void Update(float float_1)
		{
			this.animationList_0.Update(float_1);
			CaptureAnimation.Enum1 @enum = this.enum1_0;
			if (@enum != CaptureAnimation.Enum1.const_1)
			{
				this.method_4();
				return;
			}
			this.method_2(float_1);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000042A8 File Offset: 0x000024A8
		private void method_1()
		{
			this.animationList_0.Clear();
			PointF tileCenter = this.game_0.Board.GetTileCenter(this.tile_0);
			PointF tileCenter2 = this.game_0.Board.GetTileCenter(this.tile_1);
			this.animationList_0.Add(new TranslationAnimation(this.piece_0, tileCenter, tileCenter2, 0.25f));
			this.animationList_0.Add(new FadeOutAnimation(this.piece_1, 0.25f));
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.CapturePiece);
			this.enum1_0 = CaptureAnimation.Enum1.const_1;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000238B File Offset: 0x0000058B
		private void method_2(float float_1)
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_3();
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000023A0 File Offset: 0x000005A0
		private void method_3()
		{
			this.method_4();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000023A8 File Offset: 0x000005A8
		private void method_4()
		{
			this.animationList_0.Clear();
			this.enum1_0 = CaptureAnimation.Enum1.const_0;
		}

		// Token: 0x04000055 RID: 85
		private const float float_0 = 0.25f;

		// Token: 0x04000056 RID: 86
		private Game game_0;

		// Token: 0x04000057 RID: 87
		private LaserChess.Logic.Tile tile_0;

		// Token: 0x04000058 RID: 88
		private LaserChess.Logic.Tile tile_1;

		// Token: 0x04000059 RID: 89
		private Piece piece_0;

		// Token: 0x0400005A RID: 90
		private Piece piece_1;

		// Token: 0x0400005B RID: 91
		private CaptureAnimation.Enum1 enum1_0;

		// Token: 0x0400005C RID: 92
		private AnimationList animationList_0 = new AnimationList();

		// Token: 0x02000010 RID: 16
		private enum Enum1
		{
			// Token: 0x0400005E RID: 94
			const_0,
			// Token: 0x0400005F RID: 95
			const_1
		}
	}
}
