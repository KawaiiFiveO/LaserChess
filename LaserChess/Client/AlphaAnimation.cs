using System;

namespace LaserChess.Client
{
	public class AlphaAnimation : Animation
	{
		public AlphaAnimation(SpriteList sprites, float from, float to, float duration)
		{
			if (sprites == null)
			{
				return;
			}
			this.sprites = sprites;
			this.startAlpha = from;
			this.endAlpha = to;
			this.progress = 0f;
			if ((double)duration > 0.0)
			{
				this.rate = 1f / duration;
				this.state = AlphaAnimation.State.Running;
			}
			else
			{
				this.progress = 1f;
			}
			this.sprites.Alpha = this.CurrentAlpha();
		}

		private float CurrentAlpha()
		{
			return this.startAlpha + (this.endAlpha - this.startAlpha) * this.progress;
		}

		public override bool IsIdle()
		{
			return this.state == AlphaAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			if (this.state == AlphaAnimation.State.Running)
			{
				this.progress += this.rate * elapsed;
				if (this.progress >= 1f)
				{
					this.progress = 1f;
					this.state = AlphaAnimation.State.Idle;
				}
				this.sprites.Alpha = this.CurrentAlpha();
			}
		}

		private AlphaAnimation.State state;

		private SpriteList sprites;

		private float startAlpha;

		private float endAlpha;

		private float progress;

		private float rate;

		private enum State
		{
			Idle,
			Running
		}
	}
}
