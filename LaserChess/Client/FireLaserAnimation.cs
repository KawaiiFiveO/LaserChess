using System;
using System.Collections.ObjectModel;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x0200001F RID: 31
	public class FireLaserAnimation : Animation
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00004E90 File Offset: 0x00003090
		public FireLaserAnimation(Game game_1, SkinningClass skinningClass_1)
		{
			this.skinningClass_0 = skinningClass_1;
			this.game_0 = game_1;
			foreach (Piece piece in game_1.PieceList)
			{
				piece.PieceLogic.FireLaserEvent += this.method_0;
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004F44 File Offset: 0x00003144
		private void method_0(object sender, FireLaserEventArgs e)
		{
			this.spriteList_0.Clear();
			this.spriteList_1.Clear();
			this.spriteList_2.Clear();
			this.collection_0.Clear();
			this.animationList_0.Clear();
			string text = ((e.Piece1.Player.PlayerNumber == 0) ? "White" : "Black");
			SkinSprite skinSprite = new SkinSprite(this.skinningClass_0, text + "LaserGunGlow");
			skinSprite.Rotation = Piece.FacingToAngle(e.Piece1.FrontFacing) - 90f;
			skinSprite.Translation = this.game_0.Board.GetTileCenter(e.Tile1);
			skinSprite.Display = 0;
			this.spriteList_0.Add(skinSprite);
			this.method_1(text);
			this.method_2(text);
			this.method_3();
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005020 File Offset: 0x00003220
		private void method_1(string string_0)
		{
			foreach (LaserBeam laserBeam in this.game_0.GameLogic.LaserBeamList)
			{
				SkinSprite skinSprite = new SkinSprite(this.skinningClass_0, string_0 + "LaserBeamGlow");
				skinSprite.Scaling = new PointF(this.game_0.Board.TileSize.X / skinSprite.Width, 1f);
				skinSprite.Center = new PointF(0f, 0.5f);
				skinSprite.Rotation = Piece.FacingToAngle(laserBeam.Direction) - 90f;
				skinSprite.Translation = this.game_0.Board.GetTileCenter(laserBeam.Tile);
				skinSprite.Display = 0;
				this.spriteList_1.Add(skinSprite);
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005118 File Offset: 0x00003318
		private void method_2(string string_0)
		{
			foreach (Piece piece in this.game_0.PieceList)
			{
				if (piece.PieceLogic.LaserHit)
				{
					this.collection_0.Add(piece);
					Tile tile = piece.PieceLogic.Tile;
					SkinSprite skinSprite = new SkinSprite(this.skinningClass_0, string_0 + "LaserTargetGlow");
					skinSprite.Translation = this.game_0.Board.GetTileCenter(tile);
					skinSprite.Display = 0;
					this.spriteList_2.Add(skinSprite);
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000267D File Offset: 0x0000087D
		public override bool IsIdle()
		{
			return this.enum6_0 == FireLaserAnimation.Enum6.const_0;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000051CC File Offset: 0x000033CC
		public override void Update(float float_3)
		{
			this.animationList_0.Update(float_3);
			switch (this.enum6_0)
			{
			case FireLaserAnimation.Enum6.const_1:
				this.method_4(float_3);
				return;
			case FireLaserAnimation.Enum6.const_2:
				this.method_6(float_3);
				return;
			case FireLaserAnimation.Enum6.const_3:
				this.method_8(float_3);
				return;
			default:
				this.method_9();
				return;
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005220 File Offset: 0x00003420
		private void method_3()
		{
			this.animationList_0.Clear();
			this.animationList_0.Add(new FadeInAnimation(this.spriteList_0, 0.8f));
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.FireLaser);
			this.enum6_0 = FireLaserAnimation.Enum6.const_1;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00002688 File Offset: 0x00000888
		private void method_4(float float_3)
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_5();
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000526C File Offset: 0x0000346C
		private void method_5()
		{
			this.animationList_0.Clear();
			this.animationList_0.Add(new FadeInAnimation(this.spriteList_1, 0.25f));
			this.animationList_0.Add(new FadeInAnimation(this.spriteList_2, 0.25f));
			foreach (Piece piece in this.collection_0)
			{
				this.animationList_0.Add(new FadeOutAnimation(piece, 0.25f));
				switch (this.random_0.Next(4))
				{
				case 0:
					this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit1);
					break;
				case 1:
					this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit2);
					break;
				case 2:
					this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit3);
					break;
				case 3:
					this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit4);
					break;
				}
			}
			this.enum6_0 = FireLaserAnimation.Enum6.const_2;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000269D File Offset: 0x0000089D
		private void method_6(float float_3)
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_7();
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000538C File Offset: 0x0000358C
		private void method_7()
		{
			this.animationList_0.Clear();
			this.animationList_0.Add(new FadeOutAnimation(this.spriteList_0, 1f));
			this.animationList_0.Add(new FadeOutAnimation(this.spriteList_1, 1f));
			this.animationList_0.Add(new FadeOutAnimation(this.spriteList_2, 1f));
			this.enum6_0 = FireLaserAnimation.Enum6.const_3;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000026B2 File Offset: 0x000008B2
		private void method_8(float float_3)
		{
			if (this.animationList_0.IsIdle())
			{
				this.method_9();
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000026C7 File Offset: 0x000008C7
		private void method_9()
		{
			this.animationList_0.Clear();
			this.enum6_0 = FireLaserAnimation.Enum6.const_0;
		}

		// Token: 0x0400009B RID: 155
		private const float float_0 = 0.8f;

		// Token: 0x0400009C RID: 156
		private const float float_1 = 0.25f;

		// Token: 0x0400009D RID: 157
		private const float float_2 = 1f;

		// Token: 0x0400009E RID: 158
		private SkinningClass skinningClass_0;

		// Token: 0x0400009F RID: 159
		private Game game_0;

		// Token: 0x040000A0 RID: 160
		private FireLaserAnimation.Enum6 enum6_0;

		// Token: 0x040000A1 RID: 161
		private SpriteList spriteList_0 = new SpriteList();

		// Token: 0x040000A2 RID: 162
		private SpriteList spriteList_1 = new SpriteList();

		// Token: 0x040000A3 RID: 163
		private SpriteList spriteList_2 = new SpriteList();

		// Token: 0x040000A4 RID: 164
		private Collection<Piece> collection_0 = new Collection<Piece>();

		// Token: 0x040000A5 RID: 165
		private AnimationList animationList_0 = new AnimationList();

		// Token: 0x040000A6 RID: 166
		private Random random_0 = new Random();

		// Token: 0x02000020 RID: 32
		private enum Enum6
		{
			// Token: 0x040000A8 RID: 168
			const_0,
			// Token: 0x040000A9 RID: 169
			const_1,
			// Token: 0x040000AA RID: 170
			const_2,
			// Token: 0x040000AB RID: 171
			const_3
		}
	}
}
