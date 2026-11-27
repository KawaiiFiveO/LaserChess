using System;
using System.Collections.ObjectModel;

namespace LaserChess.Client
{
	public class AnimationList : Collection<Animation>
	{
		public void Update(float elapsed)
		{
			foreach (Animation animation in this)
			{
				animation.Update(elapsed);
			}
		}

		public bool IsIdle()
		{
			foreach (Animation animation in this)
			{
				if (!animation.IsIdle())
				{
					return false;
				}
			}
			return true;
		}
	}
}
