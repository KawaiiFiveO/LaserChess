using System;

namespace LaserChess.Client
{
	// Token: 0x02000003 RID: 3
	public class AlphaAnimation : Animation
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002C58 File Offset: 0x00000E58
		public AlphaAnimation(SpriteList spriteList_1, float float_4, float float_5, float float_6)
		{
			if (spriteList_1 == null)
			{
				return;
			}
			this.spriteList_0 = spriteList_1;
			this.float_0 = float_4;
			this.float_1 = float_5;
			this.float_2 = 0f;
			if ((double)float_6 > 0.0)
			{
				this.float_3 = 1f / float_6;
				this.enum0_0 = AlphaAnimation.Enum0.const_1;
			}
			else
			{
				this.float_2 = 1f;
			}
			this.spriteList_0.Alpha = this.method_0();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002058 File Offset: 0x00000258
		private float method_0()
		{
			return this.float_0 + (this.float_1 - this.float_0) * this.float_2;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002075 File Offset: 0x00000275
		public override bool IsIdle()
		{
			return this.enum0_0 == AlphaAnimation.Enum0.const_0;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002CD0 File Offset: 0x00000ED0
		public override void Update(float float_4)
		{
			if (this.enum0_0 == AlphaAnimation.Enum0.const_1)
			{
				this.float_2 += this.float_3 * float_4;
				if (this.float_2 >= 1f)
				{
					this.float_2 = 1f;
					this.enum0_0 = AlphaAnimation.Enum0.const_0;
				}
				this.spriteList_0.Alpha = this.method_0();
			}
		}

		// Token: 0x04000001 RID: 1
		private AlphaAnimation.Enum0 enum0_0;

		// Token: 0x04000002 RID: 2
		private SpriteList spriteList_0;

		// Token: 0x04000003 RID: 3
		private float float_0;

		// Token: 0x04000004 RID: 4
		private float float_1;

		// Token: 0x04000005 RID: 5
		private float float_2;

		// Token: 0x04000006 RID: 6
		private float float_3;

		// Token: 0x02000004 RID: 4
		private enum Enum0
		{
			// Token: 0x04000008 RID: 8
			const_0,
			// Token: 0x04000009 RID: 9
			const_1
		}
	}
}
