using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace LaserChess.Client
{
	// Token: 0x02000005 RID: 5
	public class SpriteList : Collection<SkinSprite>
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002D2C File Offset: 0x00000F2C
		public new void Clear()
		{
			foreach (SkinSprite skinSprite in this)
			{
				skinSprite.RemoveFromFrame();
			}
			base.Clear();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002080 File Offset: 0x00000280
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002D7C File Offset: 0x00000F7C
		public PointF Translation
		{
			get
			{
				return this.pointF_0;
			}
			set
			{
				this.pointF_0 = value;
				foreach (SkinSprite skinSprite in this)
				{
					skinSprite.Translation = value;
				}
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002088 File Offset: 0x00000288
		// (set) Token: 0x0600000C RID: 12 RVA: 0x00002DCC File Offset: 0x00000FCC
		public float Alpha
		{
			get
			{
				return this.float_0;
			}
			set
			{
				this.float_0 = value;
				foreach (SkinSprite skinSprite in this)
				{
					skinSprite.Alpha = value;
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002090 File Offset: 0x00000290
		// (set) Token: 0x0600000E RID: 14 RVA: 0x00002E1C File Offset: 0x0000101C
		public int Display
		{
			get
			{
				return this.int_0;
			}
			set
			{
				this.int_0 = value;
				foreach (SkinSprite skinSprite in this)
				{
					skinSprite.Display = value;
				}
			}
		}

		// Token: 0x0400000A RID: 10
		private PointF pointF_0;

		// Token: 0x0400000B RID: 11
		private float float_0;

		// Token: 0x0400000C RID: 12
		private int int_0;
	}
}
