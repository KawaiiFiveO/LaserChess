using System;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	public class PopupYesNo : Popup
	{
		public override void Init(GraphicsClass graphicsClass, string name, SoundSystem soundSystem)
		{
			base.Init(graphicsClass, name, soundSystem);
			this.yesButton = new Button(graphicsClass, "PopupButtonYes", soundSystem);
			this.noButton = new Button(graphicsClass, "PopupButtonNo", soundSystem);
		}

		public override void Show()
		{
			base.Show();
			this.yesButton.Show();
			this.noButton.Show();
		}

		public override void Hide()
		{
			this.yesButton.Hide();
			this.noButton.Hide();
			base.Hide();
		}

		public SkinButtonClass ButtonYes
		{
			get
			{
				return this.yesButton.SkinItem;
			}
		}

		public SkinButtonClass ButtonNo
		{
			get
			{
				return this.noButton.SkinItem;
			}
		}

		private Button yesButton;

		private Button noButton;
	}
}
