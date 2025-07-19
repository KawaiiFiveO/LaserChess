using System;
using System.Drawing;
using Microsoft.DirectX;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000006 RID: 6
	public class SkinSprite
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002E6C File Offset: 0x0000106C
		public SkinSprite(SkinningClass skinningClass_0, string string_0)
		{
			if (skinningClass_0 == null)
			{
				throw new ArgumentNullException("skin");
			}
			if (string_0 == null)
			{
				throw new ArgumentNullException("name");
			}
			SkinItemClass item = skinningClass_0.GetItem("default", string_0);
			SkinSprite.int_1++;
			this.skinItemClass_0 = item.CreateAnother("Sprite" + SkinSprite.int_1, item.Frame);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000020A0 File Offset: 0x000002A0
		public void RemoveFromFrame()
		{
			this.skinItemClass_0.RemoveMeFromFrame(this.skinItemClass_0.Frame);
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020B8 File Offset: 0x000002B8
		public float Width
		{
			get
			{
				return this.skinItemClass_0.attr.Width;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000020CA File Offset: 0x000002CA
		public float Height
		{
			get
			{
				return this.skinItemClass_0.attr.Height;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000020EB File Offset: 0x000002EB
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000020DC File Offset: 0x000002DC
		public float Alpha
		{
			get
			{
				return this.float_0;
			}
			set
			{
				this.float_0 = value;
				this.method_0();
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002102 File Offset: 0x00000302
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000020F3 File Offset: 0x000002F3
		public float Rotation
		{
			get
			{
				return this.float_1;
			}
			set
			{
				this.float_1 = value;
				this.method_0();
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002119 File Offset: 0x00000319
		// (set) Token: 0x06000018 RID: 24 RVA: 0x0000210A File Offset: 0x0000030A
		public PointF Center
		{
			get
			{
				return this.pointF_0;
			}
			set
			{
				this.pointF_0 = value;
				this.method_0();
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002130 File Offset: 0x00000330
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002121 File Offset: 0x00000321
		public PointF Translation
		{
			get
			{
				return this.pointF_2;
			}
			set
			{
				this.pointF_2 = value;
				this.method_0();
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002147 File Offset: 0x00000347
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002138 File Offset: 0x00000338
		public PointF Scaling
		{
			get
			{
				return this.pointF_1;
			}
			set
			{
				this.pointF_1 = value;
				this.method_0();
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000215E File Offset: 0x0000035E
		// (set) Token: 0x0600001E RID: 30 RVA: 0x0000214F File Offset: 0x0000034F
		public int Display
		{
			get
			{
				return this.int_0;
			}
			set
			{
				this.int_0 = value;
				this.method_0();
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002F04 File Offset: 0x00001104
		private void method_0()
		{
			Matrix identity = Matrix.Identity;
			identity.Multiply(Matrix.Translation(-this.skinItemClass_0.attr.Width * this.pointF_0.X, -this.skinItemClass_0.attr.Height * this.pointF_0.Y, 0f));
			identity.Multiply(Matrix.Scaling(this.pointF_1.X, this.pointF_1.Y, 1f));
			identity.Multiply(Matrix.RotationZ(6.2831855f * this.float_1 / 360f));
			identity.Multiply(Matrix.Translation(this.pointF_2.X, this.pointF_2.Y, 0f));
			this.skinItemClass_0.TransformationMatrix = identity;
			this.skinItemClass_0.Modulation = Color.FromArgb((int)(255f * this.float_0), 255, 255, 255);
			this.skinItemClass_0.Display = this.int_0;
		}

		// Token: 0x0400000D RID: 13
		private SkinItemClass skinItemClass_0;

		// Token: 0x0400000E RID: 14
		private float float_0;

		// Token: 0x0400000F RID: 15
		private float float_1;

		// Token: 0x04000010 RID: 16
		private PointF pointF_0 = new PointF(0.5f, 0.5f);

		// Token: 0x04000011 RID: 17
		private PointF pointF_1 = new PointF(1f, 1f);

		// Token: 0x04000012 RID: 18
		private PointF pointF_2;

		// Token: 0x04000013 RID: 19
		private int int_0;

		// Token: 0x04000014 RID: 20
		private static int int_1;
	}
}
