using System;
using System.Collections.ObjectModel;

namespace LaserChess.Client
{
	internal class PopupCollection : Collection<Popup>
	{
		public Popup Current
		{
			get
			{
				return this.current;
			}
			set
			{
				if (this.current == value)
				{
					return;
				}
				if (this.current != null && value != this.current)
				{
					this.current.Hide();
				}
				this.current = value;
				if (this.current != null)
				{
					this.current.Show();
				}
			}
		}

		private Popup current;
	}
}
