using System;
using System.Collections.ObjectModel;

namespace LaserChess.Client
{
	// Token: 0x02000009 RID: 9
	public class AnimationList : Collection<Animation>
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00003018 File Offset: 0x00001218
		public void Update(float float_0)
		{
			foreach (Animation animation in this)
			{
				animation.Update(float_0);
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003064 File Offset: 0x00001264
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
