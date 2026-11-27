using System;

namespace LaserChess.Client
{
	public class ChatEventArgs : EventArgs
	{
		public int PlayerNumber
		{
			get
			{
				return this.playerNumber;
			}
		}

		public string ChatText
		{
			get
			{
				return this.chatText;
			}
		}

		public ChatEventArgs(int playerNumber, string chatText)
		{
			this.playerNumber = playerNumber;
			this.chatText = chatText;
		}

		private int playerNumber;

		private string chatText;
	}
}
