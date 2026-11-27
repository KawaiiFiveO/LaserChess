using System;

namespace LaserChess.Client
{
	public class DisplayAnimation : Animation
	{
		public DisplayAnimation(SpriteList sprites, int fromFrame, int toFrame, int frameCount, float duration)
		{
			if (sprites != null)
			{
				this.sprites = sprites;
				this.fromFrame = fromFrame;
				this.toFrame = toFrame;
				this.frameCount = frameCount;
				this.progress = 0f;
				if ((double)duration <= 0.0)
				{
					this.progress = 1f;
				}
				else
				{
					this.rate = 1f / duration;
					this.state = DisplayAnimation.State.Running;
				}
				this.sprites.Display = this.CurrentFrame();
				return;
			}
		}

		private int CurrentFrame()
		{
			int num = this.fromFrame + (int)((float)(this.toFrame - this.fromFrame) * this.progress);
			if (num >= 0)
			{
				num %= this.frameCount;
			}
			else
			{
				num = this.frameCount - -num % this.frameCount;
			}
			return num;
		}

		public override bool IsIdle()
		{
			return this.state == DisplayAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			if (this.state == DisplayAnimation.State.Running)
			{
				this.progress += this.rate * elapsed;
				if (this.progress >= 1f)
				{
					this.progress = 1f;
					this.state = DisplayAnimation.State.Idle;
				}
				this.sprites.Display = this.CurrentFrame();
			}
		}

		private DisplayAnimation.State state;

		private SpriteList sprites;

		private int fromFrame;

		private int toFrame;

		private int frameCount;

		private float progress;

		private float rate;

		private enum State
		{
			Idle,
			Running
		}
	}
}
