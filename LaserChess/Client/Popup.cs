using System;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000027 RID: 39
	public class Popup
	{
		// Token: 0x06000120 RID: 288 RVA: 0x00007830 File Offset: 0x00005A30
		public virtual void Init(GraphicsClass graphicsClass_0, string string_0, SoundSystem soundSystem_0)
		{
			if (graphicsClass_0 == null)
			{
				throw new ArgumentNullException("gc");
			}
			this.skinItemClass_0 = graphicsClass_0.Skin.GetItem("default", "PopupBackground");
			this.skinItemClass_1 = graphicsClass_0.Skin.GetItem("default", "PopupMessage" + string_0);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00002B61 File Offset: 0x00000D61
		public virtual void Show()
		{
			this.skinItemClass_0.Display = 0;
			this.skinItemClass_1.Display = 0;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00002B7B File Offset: 0x00000D7B
		public virtual void Hide()
		{
			this.skinItemClass_0.Display = -1;
			this.skinItemClass_1.Display = -1;
		}

		// Token: 0x040000EE RID: 238
		private SkinItemClass skinItemClass_0;

		// Token: 0x040000EF RID: 239
		private SkinItemClass skinItemClass_1;
	}
}
