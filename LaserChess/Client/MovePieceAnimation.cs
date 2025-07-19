using System;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000017 RID: 23
	public class MovePieceAnimation : Animation
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00004984 File Offset: 0x00002B84
		public MovePieceAnimation(Game game_1, SkinningClass skinningClass_0)
		{
			this.game_0 = game_1;
			foreach (Piece piece in game_1.PieceList)
			{
				piece.PieceLogic.MovePieceEvent += this.method_0;
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00002581 File Offset: 0x00000781
		private void method_0(object sender, MovePieceEventArgs e)
		{
			this.tile_0 = e.Tile1;
			this.tile_1 = e.Tile2;
			this.piece_0 = this.game_0.FindPiece(e.Piece);
			this.method_1();
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000025B8 File Offset: 0x000007B8
		public override bool IsIdle()
		{
			return this.enum2_0 == MovePieceAnimation.Enum2.const_0;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000049FC File Offset: 0x00002BFC
		public override void Update(float float_1)
		{
			this.animationList_0.Update(float_1);
			MovePieceAnimation.Enum2 @enum = this.enum2_0;
			if (@enum == MovePieceAnimation.Enum2.const_1)
			{
				this.method_2(float_1);
				return;
			}
			this.method_4();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004A30 File Offset: 0x00002C30
		private void method_1()
		{
			this.animationList_0.Clear();
			PointF tileCenter = this.game_0.Board.GetTileCenter(this.tile_0);
			PointF tileCenter2 = this.game_0.Board.GetTileCenter(this.tile_1);
			this.animationList_0.Add(new TranslationAnimation(this.piece_0, tileCenter, tileCenter2, 0.25f));
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.MovePiece);
			this.enum2_0 = MovePieceAnimation.Enum2.const_1;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000025C3 File Offset: 0x000007C3
		private void method_2(float float_1)
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_3();
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000025D8 File Offset: 0x000007D8
		private void method_3()
		{
			this.method_4();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000025E0 File Offset: 0x000007E0
		private void method_4()
		{
			this.animationList_0.Clear();
			this.enum2_0 = MovePieceAnimation.Enum2.const_0;
		}

		// Token: 0x04000074 RID: 116
		private const float float_0 = 0.25f;

		// Token: 0x04000075 RID: 117
		private Game game_0;

		// Token: 0x04000076 RID: 118
		private Tile tile_0;

		// Token: 0x04000077 RID: 119
		private Tile tile_1;

		// Token: 0x04000078 RID: 120
		private Piece piece_0;

		// Token: 0x04000079 RID: 121
		private MovePieceAnimation.Enum2 enum2_0;

		// Token: 0x0400007A RID: 122
		private AnimationList animationList_0 = new AnimationList();

		// Token: 0x02000018 RID: 24
		private enum Enum2
		{
			// Token: 0x0400007C RID: 124
			const_0,
			// Token: 0x0400007D RID: 125
			const_1
		}
	}
}
