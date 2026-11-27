using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace LaserChess.Client
{
	public class SpriteList : Collection<SkinSprite>
	{
		public new void Clear()
		{
			foreach (SkinSprite skinSprite in this)
			{
				skinSprite.RemoveFromFrame();
			}
			base.Clear();
		}

		public PointF Translation
		{
			get
			{
				return this.translation;
			}
			set
			{
				this.translation = value;
				foreach (SkinSprite skinSprite in this)
				{
					skinSprite.Translation = value;
				}
			}
		}

		public float Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				this.alpha = value;
				foreach (SkinSprite skinSprite in this)
				{
					skinSprite.Alpha = value;
				}
			}
		}

		public int Display
		{
			get
			{
				return this.display;
			}
			set
			{
				this.display = value;
				foreach (SkinSprite skinSprite in this)
				{
					skinSprite.Display = value;
				}
			}
		}

		private PointF translation;

		private float alpha;

		private int display;
	}
}
