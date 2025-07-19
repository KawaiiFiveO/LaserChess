using System;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000021 RID: 33
	public class TeleportAnimation : Animation
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x000053FC File Offset: 0x000035FC
		public TeleportAnimation(Game game_1, SkinningClass skinningClass_0)
		{
			this.game_0 = game_1;
			foreach (Piece piece in game_1.PieceList)
			{
				piece.PieceLogic.TeleportEvent += this.method_0;
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005474 File Offset: 0x00003674
		private void method_0(object sender, TeleportEventArgs e)
		{
			this.tile_0 = e.Tile1;
			this.tile_1 = e.Tile2;
			this.tile_2 = e.Tile3;
			this.piece_0 = null;
			this.piece_1 = null;
			this.piece_2 = null;
			if (e.Piece1 != null)
			{
				this.piece_0 = this.game_0.FindPiece(e.Piece1);
			}
			if (e.Piece2 != null)
			{
				this.piece_1 = this.game_0.FindPiece(e.Piece2);
			}
			if (e.Piece3 != null)
			{
				this.piece_2 = this.game_0.FindPiece(e.Piece3);
			}
			this.method_1();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000026DB File Offset: 0x000008DB
		public override bool IsIdle()
		{
			return this.enum7_0 == TeleportAnimation.Enum7.const_0;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005520 File Offset: 0x00003720
		public override void Update(float float_3)
		{
			this.animationList_0.Update(float_3);
			switch (this.enum7_0)
			{
			case TeleportAnimation.Enum7.const_1:
				this.method_2();
				return;
			case TeleportAnimation.Enum7.const_2:
				this.method_5();
				return;
			case TeleportAnimation.Enum7.const_3:
				this.method_8();
				return;
			default:
				this.method_10();
				return;
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005570 File Offset: 0x00003770
		private void method_1()
		{
			this.animationList_0.Clear();
			PointF tileCenter = this.game_0.Board.GetTileCenter(this.tile_0);
			PointF tileCenter2 = this.game_0.Board.GetTileCenter(this.tile_1);
			this.animationList_0.Add(new TranslationAnimation(this.piece_0, tileCenter, tileCenter2, 0.25f));
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.MovePiece);
			this.enum7_0 = TeleportAnimation.Enum7.const_1;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000026E6 File Offset: 0x000008E6
		private void method_2()
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_3();
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000026FB File Offset: 0x000008FB
		private void method_3()
		{
			this.method_4();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000055EC File Offset: 0x000037EC
		private void method_4()
		{
			this.animationList_0.Clear();
			this.animationList_0.Add(new FadeOutAnimation(this.piece_1, 1.2f));
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.TeleportOut);
			this.enum7_0 = TeleportAnimation.Enum7.const_2;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00002703 File Offset: 0x00000903
		private void method_5()
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_6();
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00002718 File Offset: 0x00000918
		private void method_6()
		{
			if (this.piece_1 != this.piece_2)
			{
				this.method_7();
				return;
			}
			this.method_10();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005638 File Offset: 0x00003838
		private void method_7()
		{
			this.animationList_0.Clear();
			PointF tileCenter = this.game_0.Board.GetTileCenter(this.tile_2);
			this.piece_1.Translation = tileCenter;
			this.animationList_0.Add(new FadeInAnimation(this.piece_1, 0.6f));
			if (this.piece_2 != null)
			{
				if (this.piece_2 != this.piece_1)
				{
					this.animationList_0.Add(new FadeOutAnimation(this.piece_2, 1.2f));
				}
			}
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.TeleportIn);
			this.enum7_0 = TeleportAnimation.Enum7.const_3;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00002735 File Offset: 0x00000935
		private void method_8()
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_9();
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000274A File Offset: 0x0000094A
		private void method_9()
		{
			this.method_10();
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00002752 File Offset: 0x00000952
		private void method_10()
		{
			this.animationList_0.Clear();
			this.enum7_0 = TeleportAnimation.Enum7.const_0;
		}

		// Token: 0x040000AC RID: 172
		private const float float_0 = 0.25f;

		// Token: 0x040000AD RID: 173
		private const float float_1 = 1.2f;

		// Token: 0x040000AE RID: 174
		private const float float_2 = 0.6f;

		// Token: 0x040000AF RID: 175
		private Game game_0;

		// Token: 0x040000B0 RID: 176
		private Tile tile_0;

		// Token: 0x040000B1 RID: 177
		private Tile tile_1;

		// Token: 0x040000B2 RID: 178
		private Tile tile_2;

		// Token: 0x040000B3 RID: 179
		private Piece piece_0;

		// Token: 0x040000B4 RID: 180
		private Piece piece_1;

		// Token: 0x040000B5 RID: 181
		private Piece piece_2;

		// Token: 0x040000B6 RID: 182
		private TeleportAnimation.Enum7 enum7_0;

		// Token: 0x040000B7 RID: 183
		private AnimationList animationList_0 = new AnimationList();

		// Token: 0x02000022 RID: 34
		private enum Enum7
		{
			// Token: 0x040000B9 RID: 185
			const_0,
			// Token: 0x040000BA RID: 186
			const_1,
			// Token: 0x040000BB RID: 187
			const_2,
			// Token: 0x040000BC RID: 188
			const_3
		}
	}
}
