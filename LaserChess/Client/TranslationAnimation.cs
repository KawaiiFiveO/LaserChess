using System;
using System.Drawing;

namespace LaserChess.Client
{
	// Token: 0x02000019 RID: 25
	public class TranslationAnimation : Animation
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00004AAC File Offset: 0x00002CAC
		public TranslationAnimation(SpriteList spriteList_1, PointF pointF_2, PointF pointF_3, float float_2)
		{
			if (spriteList_1 == null)
			{
				throw new ArgumentNullException("spriteList");
			}
			this.spriteList_0 = spriteList_1;
			this.pointF_0 = pointF_2;
			this.pointF_1 = pointF_3;
			this.float_0 = 0f;
			if ((double)float_2 > 0.0)
			{
				this.float_1 = 1f / float_2;
				this.enum3_0 = TranslationAnimation.Enum3.const_1;
			}
			else
			{
				this.float_0 = 1f;
			}
			this.spriteList_0.Translation = this.method_0();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004B30 File Offset: 0x00002D30
		private PointF method_0()
		{
			return new PointF
			{
				X = this.pointF_0.X + (this.pointF_1.X - this.pointF_0.X) * this.float_0,
				Y = this.pointF_0.Y + (this.pointF_1.Y - this.pointF_0.Y) * this.float_0
			};
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000025F4 File Offset: 0x000007F4
		public override bool IsIdle()
		{
			return this.enum3_0 == TranslationAnimation.Enum3.const_0;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004BA8 File Offset: 0x00002DA8
		public override void Update(float float_2)
		{
			if (this.enum3_0 == TranslationAnimation.Enum3.const_1)
			{
				this.float_0 += this.float_1 * float_2;
				if (this.float_0 >= 1f)
				{
					this.float_0 = 1f;
					this.enum3_0 = TranslationAnimation.Enum3.const_0;
				}
				this.spriteList_0.Translation = this.method_0();
			}
		}

		// Token: 0x0400007E RID: 126
		private TranslationAnimation.Enum3 enum3_0;

		// Token: 0x0400007F RID: 127
		private SpriteList spriteList_0;

		// Token: 0x04000080 RID: 128
		private PointF pointF_0;

		// Token: 0x04000081 RID: 129
		private PointF pointF_1;

		// Token: 0x04000082 RID: 130
		private float float_0;

		// Token: 0x04000083 RID: 131
		private float float_1;

		// Token: 0x0200001A RID: 26
		private enum Enum3
		{
			// Token: 0x04000085 RID: 133
			const_0,
			// Token: 0x04000086 RID: 134
			const_1
		}
	}
}
