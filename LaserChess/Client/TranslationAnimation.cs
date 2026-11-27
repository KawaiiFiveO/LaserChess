using System;
using System.Drawing;

namespace LaserChess.Client
{
	public class TranslationAnimation : Animation
	{
		public TranslationAnimation(SpriteList sprites, PointF from, PointF to, float duration)
		{
			if (sprites == null)
			{
				throw new ArgumentNullException("spriteList");
			}
			this.sprites = sprites;
			this.from = from;
			this.to = to;
			this.progress = 0f;
			if ((double)duration > 0.0)
			{
				this.rate = 1f / duration;
				this.state = TranslationAnimation.State.Running;
			}
			else
			{
				this.progress = 1f;
			}
			this.sprites.Translation = this.CurrentPosition();
		}

		private PointF CurrentPosition()
		{
			return new PointF
			{
				X = this.from.X + (this.to.X - this.from.X) * this.progress,
				Y = this.from.Y + (this.to.Y - this.from.Y) * this.progress
			};
		}

		public override bool IsIdle()
		{
			return this.state == TranslationAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			if (this.state == TranslationAnimation.State.Running)
			{
				this.progress += this.rate * elapsed;
				if (this.progress >= 1f)
				{
					this.progress = 1f;
					this.state = TranslationAnimation.State.Idle;
				}
				this.sprites.Translation = this.CurrentPosition();
			}
		}

		private TranslationAnimation.State state;

		private SpriteList sprites;

		private PointF from;

		private PointF to;

		private float progress;

		private float rate;

		private enum State
		{
			Idle,
			Running
		}
	}
}
