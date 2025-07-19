using System;
using System.Collections.ObjectModel;
using LaserChess.Client;

namespace ns0
{
	// Token: 0x02000026 RID: 38
	internal class Class0 : Collection<Popup>
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00002B51 File Offset: 0x00000D51
		// (set) Token: 0x0600011D RID: 285 RVA: 0x000077E0 File Offset: 0x000059E0
		public Popup Popup_0
		{
			get
			{
				return this.popup_0;
			}
			set
			{
				if (this.popup_0 == value)
				{
					return;
				}
				if (this.popup_0 != null && value != this.popup_0)
				{
					this.popup_0.Hide();
				}
				this.popup_0 = value;
				if (this.popup_0 != null)
				{
					this.popup_0.Show();
				}
			}
		}

		// Token: 0x040000ED RID: 237
		private Popup popup_0;
	}
}
