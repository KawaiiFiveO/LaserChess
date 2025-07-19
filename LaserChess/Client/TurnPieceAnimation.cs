using System;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x0200001B RID: 27
	public class TurnPieceAnimation : Animation
	{
		// Token: 0x06000098 RID: 152 RVA: 0x00004C04 File Offset: 0x00002E04
		public TurnPieceAnimation(Game game_1, SkinningClass skinningClass_0)
		{
			this.game_0 = game_1;
			foreach (Piece piece in game_1.PieceList)
			{
				piece.PieceLogic.TurnPieceEvent += this.method_0;
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000025FF File Offset: 0x000007FF
		private void method_0(object sender, TurnPieceEventArgs e)
		{
			this.direction_0 = e.Facing1;
			this.direction_1 = e.Facing2;
			this.piece_0 = this.game_0.FindPiece(e.Piece);
			this.method_1();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00002636 File Offset: 0x00000836
		public override bool IsIdle()
		{
			return this.enum4_0 == TurnPieceAnimation.Enum4.const_0;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004C7C File Offset: 0x00002E7C
		public override void Update(float float_1)
		{
			this.animationList_0.Update(float_1);
			TurnPieceAnimation.Enum4 @enum = this.enum4_0;
			if (@enum == TurnPieceAnimation.Enum4.const_1)
			{
				this.method_2(float_1);
				return;
			}
			this.method_4();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004CB0 File Offset: 0x00002EB0
		private void method_1()
		{
			this.animationList_0.Clear();
			float num = Piece.FacingToAngle(this.direction_0);
			float num2 = Piece.FacingToAngle(this.direction_1);
			float num3 = num2 - num;
			if (num3 < -180f)
			{
				num3 += 360f;
			}
			if (num3 > 180f)
			{
				num3 -= 360f;
			}
			num2 = num + num3;
			int frames = this.piece_0.Frames;
			int num4 = (int)(num * (float)frames / 360f);
			int num5 = (int)(num2 * (float)frames / 360f);
			this.animationList_0.Add(new DisplayAnimation(this.piece_0, num4, num5, frames, 0.25f));
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.TurnPiece);
			this.enum4_0 = TurnPieceAnimation.Enum4.const_1;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00002641 File Offset: 0x00000841
		private void method_2(float float_1)
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_3();
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00002656 File Offset: 0x00000856
		private void method_3()
		{
			this.method_4();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000265E File Offset: 0x0000085E
		private void method_4()
		{
			this.animationList_0.Clear();
			this.enum4_0 = TurnPieceAnimation.Enum4.const_0;
		}

		// Token: 0x04000087 RID: 135
		private const float float_0 = 0.25f;

		// Token: 0x04000088 RID: 136
		private Game game_0;

		// Token: 0x04000089 RID: 137
		private Direction direction_0;

		// Token: 0x0400008A RID: 138
		private Direction direction_1;

		// Token: 0x0400008B RID: 139
		private Piece piece_0;

		// Token: 0x0400008C RID: 140
		private TurnPieceAnimation.Enum4 enum4_0;

		// Token: 0x0400008D RID: 141
		private AnimationList animationList_0 = new AnimationList();

		// Token: 0x0200001C RID: 28
		private enum Enum4
		{
			// Token: 0x0400008F RID: 143
			const_0,
			// Token: 0x04000090 RID: 144
			const_1
		}
	}
}
