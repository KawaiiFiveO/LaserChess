using System;

namespace LaserChess.Client
{
	public class FadeInAnimation : AlphaAnimation
	{
		public FadeInAnimation(SpriteList sprites, float duration)
			: base(sprites, 0f, 1f, duration)
		{
		}
	}
}
