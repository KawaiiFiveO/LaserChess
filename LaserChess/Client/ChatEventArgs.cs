using System;

namespace LaserChess.Client
{
	// Token: 0x02000024 RID: 36
	public class ChatEventArgs : EventArgs
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00002917 File Offset: 0x00000B17
		public int PlayerNumber
		{
			get
			{
				return this.int_0;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000CF RID: 207 RVA: 0x0000291F File Offset: 0x00000B1F
		public string ChatText
		{
			get
			{
				return this.string_0;
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00002927 File Offset: 0x00000B27
		public ChatEventArgs(int int_1, string string_1)
		{
			this.int_0 = int_1;
			this.string_0 = string_1;
		}

		// Token: 0x040000C6 RID: 198
		private int int_0;

		// Token: 0x040000C7 RID: 199
		private string string_0;
	}
}
