using System;
using System.Drawing;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	public class Button
	{
		public Button(GraphicsClass graphicsClass, string name, SoundSystem soundSystem)
		{
			if (graphicsClass != null)
			{
				this.soundSystem = soundSystem;
				this.skinItem = (SkinButtonClass)graphicsClass.Skin.GetItem("default", name);
				this.origin = new PointF(0f, 0f);
				this.basePosition = new PointF(this.skinItem.attr.Left, this.skinItem.attr.Top);
				this.skinItem.ButtonUp += this.OnButtonUp;
				this.skinItem.ButtonDown += this.OnButtonDown;
				return;
			}
		}

		public SkinButtonClass SkinItem
		{
			get
			{
				return this.skinItem;
			}
		}

		public PointF Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.origin = value;
				this.skinItem.attr.ResetAttrAxis(ItemAttributeClass.AType.Left, this.origin.X + this.basePosition.X, ItemAttributeClass.AType.Width, this.skinItem.attr.Width);
				this.skinItem.attr.ResetAttrAxis(ItemAttributeClass.AType.Top, this.origin.Y + this.basePosition.Y, ItemAttributeClass.AType.Height, this.skinItem.attr.Height);
			}
		}

		public void Hide()
		{
			this.skinItem.Hide();
		}

		public void Show()
		{
			this.skinItem.Show();
		}

		private void OnButtonDown(object sender, EventArgs e)
		{
			this.soundSystem.PlaySound(SoundSystem.Sound.ButtonDown);
		}

		private void OnButtonUp(object sender, EventArgs e)
		{
			this.soundSystem.PlaySound(SoundSystem.Sound.ButtonUp);
		}

		private SkinButtonClass skinItem;

		private PointF origin;

		private PointF basePosition;

		private SoundSystem soundSystem;
	}
}
