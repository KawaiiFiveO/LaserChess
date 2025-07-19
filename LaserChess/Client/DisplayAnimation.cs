using System;

namespace LaserChess.Client
{
	// Token: 0x0200001D RID: 29
	public class DisplayAnimation : Animation
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x00004D68 File Offset: 0x00002F68
		public DisplayAnimation(SpriteList spriteList_1, int int_3, int int_4, int int_5, float float_2)
		{
			if (spriteList_1 != null)
			{
				this.spriteList_0 = spriteList_1;
				this.int_0 = int_3;
				this.int_1 = int_4;
				this.int_2 = int_5;
				this.float_0 = 0f;
				if ((double)float_2 <= 0.0)
				{
					this.float_0 = 1f;
				}
				else
				{
					this.float_1 = 1f / float_2;
					this.enum5_0 = DisplayAnimation.Enum5.const_1;
				}
				this.spriteList_0.Display = this.method_0();
				return;
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004DE8 File Offset: 0x00002FE8
		private int method_0()
		{
			int num = this.int_0 + (int)((float)(this.int_1 - this.int_0) * this.float_0);
			if (num >= 0)
			{
				num %= this.int_2;
			}
			else
			{
				num = this.int_2 - -num % this.int_2;
			}
			return num;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00002672 File Offset: 0x00000872
		public override bool IsIdle()
		{
			return this.enum5_0 == DisplayAnimation.Enum5.const_0;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004E34 File Offset: 0x00003034
		public override void Update(float float_2)
		{
			if (this.enum5_0 == DisplayAnimation.Enum5.const_1)
			{
				this.float_0 += this.float_1 * float_2;
				if (this.float_0 >= 1f)
				{
					this.float_0 = 1f;
					this.enum5_0 = DisplayAnimation.Enum5.const_0;
				}
				this.spriteList_0.Display = this.method_0();
			}
		}

		// Token: 0x04000091 RID: 145
		private DisplayAnimation.Enum5 enum5_0;

		// Token: 0x04000092 RID: 146
		private SpriteList spriteList_0;

		// Token: 0x04000093 RID: 147
		private int int_0;

		// Token: 0x04000094 RID: 148
		private int int_1;

		// Token: 0x04000095 RID: 149
		private int int_2;

		// Token: 0x04000096 RID: 150
		private float float_0;

		// Token: 0x04000097 RID: 151
		private float float_1;

		// Token: 0x0200001E RID: 30
		private enum Enum5
		{
			// Token: 0x04000099 RID: 153
			const_0,
			// Token: 0x0400009A RID: 154
			const_1
		}
	}
}
