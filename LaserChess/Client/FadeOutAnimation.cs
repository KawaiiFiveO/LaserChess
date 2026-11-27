using System;

namespace LaserChess.Client
{
	public class FadeOutAnimation : AlphaAnimation
	{
		public FadeOutAnimation(SpriteList sprites, float duration)
			: base(sprites, 1f, 0f, duration)
		{
		}
	}
}
