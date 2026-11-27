using System;

namespace LaserChess.Client
{
	public abstract class Animation
	{
		public abstract void Update(float elapsed);

		public abstract bool IsIdle();
	}
}
