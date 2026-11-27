using System;
using System.Drawing;
using Microsoft.DirectX;
using TG.Skin;

namespace LaserChess.Client
{
	public class SkinSprite
	{
		public SkinSprite(SkinningClass skin, string name)
		{
			if (skin == null)
			{
				throw new ArgumentNullException("skin");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			SkinItemClass item = skin.GetItem("default", name);
			SkinSprite.spriteCount++;
			this.skinItem = item.CreateAnother("Sprite" + SkinSprite.spriteCount, item.Frame);
		}

		public void RemoveFromFrame()
		{
			this.skinItem.RemoveMeFromFrame(this.skinItem.Frame);
		}

		public float Width
		{
			get
			{
				return this.skinItem.attr.Width;
			}
		}

		public float Height
		{
			get
			{
				return this.skinItem.attr.Height;
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
				this.UpdateTransform();
			}
		}

		public float Rotation
		{
			get
			{
				return this.rotation;
			}
			set
			{
				this.rotation = value;
				this.UpdateTransform();
			}
		}

		public PointF Center
		{
			get
			{
				return this.center;
			}
			set
			{
				this.center = value;
				this.UpdateTransform();
			}
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
				this.UpdateTransform();
			}
		}

		public PointF Scaling
		{
			get
			{
				return this.scaling;
			}
			set
			{
				this.scaling = value;
				this.UpdateTransform();
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
				this.UpdateTransform();
			}
		}

		private void UpdateTransform()
		{
			Matrix identity = Matrix.Identity;
			identity.Multiply(Matrix.Translation(-this.skinItem.attr.Width * this.center.X, -this.skinItem.attr.Height * this.center.Y, 0f));
			identity.Multiply(Matrix.Scaling(this.scaling.X, this.scaling.Y, 1f));
			identity.Multiply(Matrix.RotationZ(6.2831855f * this.rotation / 360f));
			identity.Multiply(Matrix.Translation(this.translation.X, this.translation.Y, 0f));
			this.skinItem.TransformationMatrix = identity;
			this.skinItem.Modulation = Color.FromArgb((int)(255f * this.alpha), 255, 255, 255);
			this.skinItem.Display = this.display;
		}

		private SkinItemClass skinItem;

		private float alpha;

		private float rotation;

		private PointF center = new PointF(0.5f, 0.5f);

		private PointF scaling = new PointF(1f, 1f);

		private PointF translation;

		private int display;

		private static int spriteCount;
	}
}
