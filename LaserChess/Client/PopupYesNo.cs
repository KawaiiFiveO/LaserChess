using System;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000028 RID: 40
	public class PopupYesNo : Popup
	{
		// Token: 0x06000124 RID: 292 RVA: 0x00002B95 File Offset: 0x00000D95
		public override void Init(GraphicsClass graphicsClass_0, string string_0, SoundSystem soundSystem_0)
		{
			base.Init(graphicsClass_0, string_0, soundSystem_0);
			this.button_0 = new Button(graphicsClass_0, "PopupButtonYes", soundSystem_0);
			this.button_1 = new Button(graphicsClass_0, "PopupButtonNo", soundSystem_0);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00002BC4 File Offset: 0x00000DC4
		public override void Show()
		{
			base.Show();
			this.button_0.Show();
			this.button_1.Show();
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00002BE2 File Offset: 0x00000DE2
		public override void Hide()
		{
			this.button_0.Hide();
			this.button_1.Hide();
			base.Hide();
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000127 RID: 295 RVA: 0x00002C00 File Offset: 0x00000E00
		public SkinButtonClass ButtonYes
		{
			get
			{
				return this.button_0.SkinItem;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00002C0D File Offset: 0x00000E0D
		public SkinButtonClass ButtonNo
		{
			get
			{
				return this.button_1.SkinItem;
			}
		}

		// Token: 0x040000F0 RID: 240
		private Button button_0;

		// Token: 0x040000F1 RID: 241
		private Button button_1;
	}
}
