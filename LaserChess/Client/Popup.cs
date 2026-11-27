using System;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	public class Popup
	{
		public virtual void Init(GraphicsClass graphicsClass, string name, SoundSystem soundSystem)
		{
			if (graphicsClass == null)
			{
				throw new ArgumentNullException("gc");
			}
			this.background = graphicsClass.Skin.GetItem("default", "PopupBackground");
			this.message = graphicsClass.Skin.GetItem("default", "PopupMessage" + name);
		}

		public virtual void Show()
		{
			this.background.Display = 0;
			this.message.Display = 0;
		}

		public virtual void Hide()
		{
			this.background.Display = -1;
			this.message.Display = -1;
		}

		private SkinItemClass background;

		private SkinItemClass message;
	}
}
