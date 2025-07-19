using System;
using System.Drawing;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x0200000C RID: 12
	public class Button
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000038F8 File Offset: 0x00001AF8
		public Button(GraphicsClass graphicsClass_0, string string_0, SoundSystem soundSystem_1)
		{
			if (graphicsClass_0 != null)
			{
				this.soundSystem_0 = soundSystem_1;
				this.skinButtonClass_0 = (SkinButtonClass)graphicsClass_0.Skin.GetItem("default", string_0);
				this.pointF_0 = new PointF(0f, 0f);
				this.pointF_1 = new PointF(this.skinButtonClass_0.attr.Left, this.skinButtonClass_0.attr.Top);
				this.skinButtonClass_0.ButtonUp += this.skinButtonClass_0_ButtonUp;
				this.skinButtonClass_0.ButtonDown += this.skinButtonClass_0_ButtonDown;
				return;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002257 File Offset: 0x00000457
		public SkinButtonClass SkinItem
		{
			get
			{
				return this.skinButtonClass_0;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000043 RID: 67 RVA: 0x0000225F File Offset: 0x0000045F
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000039A4 File Offset: 0x00001BA4
		public PointF Origin
		{
			get
			{
				return this.pointF_0;
			}
			set
			{
				this.pointF_0 = value;
				this.skinButtonClass_0.attr.ResetAttrAxis(ItemAttributeClass.AType.Left, this.pointF_0.X + this.pointF_1.X, ItemAttributeClass.AType.Width, this.skinButtonClass_0.attr.Width);
				this.skinButtonClass_0.attr.ResetAttrAxis(ItemAttributeClass.AType.Top, this.pointF_0.Y + this.pointF_1.Y, ItemAttributeClass.AType.Height, this.skinButtonClass_0.attr.Height);
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002267 File Offset: 0x00000467
		public void Hide()
		{
			this.skinButtonClass_0.Hide();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002274 File Offset: 0x00000474
		public void Show()
		{
			this.skinButtonClass_0.Show();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002281 File Offset: 0x00000481
		private void skinButtonClass_0_ButtonDown(object sender, EventArgs e)
		{
			this.soundSystem_0.PlaySound(SoundSystem.Sound.ButtonDown);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000228F File Offset: 0x0000048F
		private void skinButtonClass_0_ButtonUp(object sender, EventArgs e)
		{
			this.soundSystem_0.PlaySound(SoundSystem.Sound.ButtonUp);
		}

		// Token: 0x04000027 RID: 39
		private SkinButtonClass skinButtonClass_0;

		// Token: 0x04000028 RID: 40
		private PointF pointF_0;

		// Token: 0x04000029 RID: 41
		private PointF pointF_1;

		// Token: 0x0400002A RID: 42
		private SoundSystem soundSystem_0;
	}
}
