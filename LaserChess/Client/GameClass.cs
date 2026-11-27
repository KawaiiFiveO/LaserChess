using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using LaserChess.Logic;
using Microsoft.DirectX.Direct3D;
using TG.Client;
using TG.DxGame;
using TG.Game;
using TG.Skin;

namespace LaserChess.Client
{
	public class GameClass : IDisposable, IGameClass
	{
		public GameClass(Control control)
		{
			this.gameLogic = new Logic.Game();
			this.game = new Game(this.gameLogic, control);
			this.commandPanel = new CommandPanel();
			this.game.MoveRequestEvent += this.OnMoveRequest;
		}

		private void OnMoveRequest(object sender, MoveRequestEventArgs e)
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Move" },
				{ "SubType", "Request" },
				{ "Piece", e.PieceNumber },
				{ "Command", e.MoveCommand },
				{ "Direction", e.Direction }
			});
		}

		private void OnGameEvent(object sender, GameEventArgs e)
		{
			Hashtable value = e.Value;
			if (value.Contains("MsgType"))
			{
				string text = (string)value["MsgType"];
				string text2;
				if ((text2 = text) != null)
				{
					if (text2 == "Ready")
					{
						this.HandleReady(value);
						return;
					}
					if (text2 == "Chat")
					{
						this.HandleChat(value);
						return;
					}
					if (text2 == "Draw")
					{
						this.HandleDraw(value);
						return;
					}
					if (text2 == "Game")
					{
						this.HandleNewGameMessage(value);
						return;
					}
					if (text2 == "Move")
					{
						this.HandleMove(value);
						return;
					}
					if (!(text2 == "GameOver"))
					{
						return;
					}
					this.HandleGameOver(value);
				}
			}
		}

		private void HandleReady(Hashtable message)
		{
			this.game.LocalPlayerNumber = (int)message["YourPlayerNum"];
			for (int i = 0; i < 2; i++)
			{
				this.playerNames[i] = (string)message[i];
				Avatar avatar = this.avatars[0];
				if (i == this.game.LocalPlayerNumber)
				{
					avatar = this.avatars[1];
				}
				avatar.Name = this.playerNames[i];
			}
		}

		private void StartNewGame(Hashtable message)
		{
			this.game.SoundSystem.GameInProgress = true;
			this.game.GameLogic.Reset();
			this.game.LocalPlayerNumber = this.game.LocalPlayerNumber;
			this.popups.Current = null;
			this.resignButton.State = SkinTextButtonClass.States.Enabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Enabled;
		}

		private void HandleGameOver(Hashtable message)
		{
			if (message.Contains("Result"))
			{
				this.game.SoundSystem.GameInProgress = false;
				this.resignButton.State = SkinTextButtonClass.States.Disabled;
				this.offerDrawButton.State = SkinTextButtonClass.States.Disabled;
				string text = (string)message["Result"];
				string text2;
				if ((text2 = text) != null)
				{
					if (text2 == "Win")
					{
						this.popups.Current = this.winPopup;
						this.game.SoundSystem.PlaySound(SoundSystem.Sound.GameWin);
						this.newGameButton.State = SkinTextButtonClass.States.Enabled;
						return;
					}
					if (text2 == "Lose")
					{
						this.popups.Current = this.losePopup;
						this.game.SoundSystem.PlaySound(SoundSystem.Sound.GameLose);
						this.newGameButton.State = SkinTextButtonClass.States.Enabled;
						return;
					}
					if (text2 == "Draw")
					{
						this.popups.Current = this.drawPopup;
						this.game.SoundSystem.PlaySound(SoundSystem.Sound.GameDraw);
						this.newGameButton.State = SkinTextButtonClass.States.Enabled;
						return;
					}
					if (!(text2 == "Quit"))
					{
						return;
					}
					this.popups.Current = this.quitPopup;
					this.newGameButton.State = SkinTextButtonClass.States.Disabled;
				}
			}
		}

		private void HandleChat(Hashtable message)
		{
			if (message.Contains("Player"))
			{
				int num = (int)message["Player"];
				if (num < 0)
				{
					return;
				}
				if (num >= 2)
				{
					return;
				}
				if (message.Contains("Text"))
				{
					string text = (string)message["Text"];
					Avatar avatar = this.avatars[0];
					if (num == this.game.LocalPlayerNumber)
					{
						avatar = this.avatars[1];
					}
					avatar.ChatText = text;
					avatar.StartChatTimer();
					this.game.SoundSystem.PlaySound(SoundSystem.Sound.IncomingChatMsg);
				}
			}
		}

		private void HandleDraw(Hashtable message)
		{
			if (!message.Contains("SubType"))
			{
				return;
			}
			string text = (string)message["SubType"];
			string text2;
			if ((text2 = text) != null)
			{
				if (text2 == "Request")
				{
					this.ShowDrawOffer(message);
					return;
				}
				if (!(text2 == "Decline"))
				{
					return;
				}
				this.HandleDrawDeclined(message);
			}
		}

		private void ShowDrawOffer(Hashtable message)
		{
			this.previousPopup = this.popups.Current;
			this.popups.Current = this.acceptDrawPopup;
			this.acceptDrawPopup.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.AcceptDrawClicked);
			this.acceptDrawPopup.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.DeclineDrawClicked);
			this.DisableGameButtons();
		}

		private bool AcceptDrawClicked(SkinButtonClass button)
		{
			this.popups.Current = this.waitPopup;
			this.SendDrawAccept();
			return true;
		}

		private bool DeclineDrawClicked(SkinButtonClass button)
		{
			this.popups.Current = this.previousPopup;
			this.SendDrawDecline();
			this.newGameButton.State = SkinTextButtonClass.States.Disabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Enabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Enabled;
			return true;
		}

		private void SendDrawAccept()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Draw" },
				{ "SubType", "Accept" }
			});
		}

		private void SendDrawDecline()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Draw" },
				{ "SubType", "Decline" }
			});
		}

		private void HandleDrawDeclined(Hashtable message)
		{
			MessageBox.Show("Your opponent has declined your draw request.", this.GameName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			this.popups.Current = this.previousPopup;
			this.newGameButton.State = SkinTextButtonClass.States.Disabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Enabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Enabled;
		}

		private void HandleNewGameMessage(Hashtable message)
		{
			if (!message.Contains("SubType"))
			{
				return;
			}
			string text = (string)message["SubType"];
			string text2;
			if ((text2 = text) != null)
			{
				if (text2 == "Request")
				{
					this.ShowNewGameRequest(message);
					return;
				}
				if (text2 == "Decline")
				{
					this.HandleNewGameDeclined(message);
					return;
				}
				if (!(text2 == "Reset"))
				{
					return;
				}
				this.StartNewGame(message);
			}
		}

		private void ShowNewGameRequest(Hashtable message)
		{
			this.previousPopup = this.popups.Current;
			this.popups.Current = this.playAgainPopup;
			this.playAgainPopup.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.AcceptNewGameClicked);
			this.playAgainPopup.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.DeclineNewGameClicked);
			this.DisableGameButtons();
		}

		private bool AcceptNewGameClicked(SkinButtonClass button)
		{
			this.popups.Current = this.waitPopup;
			this.SendNewGameAccept();
			return true;
		}

		private bool DeclineNewGameClicked(SkinButtonClass button)
		{
			this.popups.Current = this.previousPopup;
			this.SendNewGameDecline();
			this.newGameButton.State = SkinTextButtonClass.States.Enabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Disabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Disabled;
			return true;
		}

		private void SendNewGameAccept()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Game" },
				{ "SubType", "Accept" }
			});
		}

		private void SendNewGameDecline()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Game" },
				{ "SubType", "Decline" }
			});
		}

		private void HandleNewGameDeclined(Hashtable message)
		{
			MessageBox.Show("Your opponent has declined your new game request.", this.GameName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			this.popups.Current = this.previousPopup;
			this.newGameButton.State = SkinTextButtonClass.States.Enabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Disabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Disabled;
		}

		private void HandleMove(Hashtable message)
		{
			if (!message.Contains("SubType"))
			{
				return;
			}
			string text = (string)message["SubType"];
			string text2;
			if ((text2 = text) != null)
			{
				if (text2 == "Update")
				{
					this.HandleMoveUpdate(message);
					return;
				}
				if (!(text2 == "Decline"))
				{
					return;
				}
				GameClass.HandleMoveDeclined(message);
			}
		}

		private void HandleMoveUpdate(Hashtable message)
		{
			int num = (int)message["Player"];
			int num2 = (int)message["Piece"];
			MoveCommand moveCommand = (MoveCommand)message["Command"];
			Direction direction = (Direction)message["Direction"];
			if (!this.gameLogic.ProcessMoveCommand(num, num2, moveCommand, direction))
			{
				throw new InvalidOperationException("LaserChess.Client.ProcessMoveAcceptMessage(): Game out of sync.");
			}
			this.sendReadyWhenIdle = true;
		}

		private static void HandleMoveDeclined(Hashtable message)
		{
		}

		private void OnDataEvent(object sender, EventArgs e)
		{
			this.dataAvailable.Set();
		}

		private void OnAdminEvent(object sender, AdminEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			switch (e.Type)
			{
			case AdminEventType.Connected:
				this.HandleAdminConnected(e);
				return;
			case AdminEventType.Disconnected:
			{
				string text = "name=discback, layer=9, type=generic, top=0, left=0, width=(frame).width, height=(frame).height";
				SkinGenericClass skinGenericClass = (SkinGenericClass)this.graphics.Skin.ReadAndCreateSkinItem(this.graphics, this.graphics.Skin.GetFrame("default"), text);
				SkinGenericClass skinGenericClass2 = skinGenericClass;
				skinGenericClass2.PaintCallback = (PaintEventHandler)Delegate.Combine(skinGenericClass2.PaintCallback, new PaintEventHandler(this.PaintDisconnectedBackground));
				skinGenericClass.Modulation = Color.FromArgb(128, 0, 0, 0);
				skinGenericClass.Enabled = true;
				skinGenericClass.ExclusivelyHandleMouseInput = true;
				skinGenericClass.MouseDown += this.DisconnectedMouseDown;
				string text2 = "name=disctext, layer=9, type=textbox, hcenter=(frame).hcenter, vcenter=(frame).vcenter, width=(frame).width, height=40, backcolor=1;0;0;0, forecolor=255;0;0, fontname=ariel, size=39, italic=no, bold=no, align=center";
				SkinTextboxClass skinTextboxClass = (SkinTextboxClass)this.graphics.Skin.ReadAndCreateSkinItem(this.graphics, this.graphics.Skin.GetFrame("default"), text2);
				skinTextboxClass.Text = "Disconnected.";
				skinTextboxClass.DropShadow = SkinTextboxClass.DropShadowStyle.Blur;
				this.minimizeButton.SkinItem.Enabled = false;
				this.helpButton.SkinItem.Enabled = false;
				this.newGameButton.Enabled = false;
				this.resignButton.Enabled = false;
				this.offerDrawButton.Enabled = false;
				this.replayButton.Enabled = false;
				this.moreHelpButton.Enabled = false;
				this.continueButton.Enabled = false;
				return;
			}
			case AdminEventType.Shutdown:
				this.graphics.Close();
				break;
			case AdminEventType.Minimize:
			case AdminEventType.Restore:
				break;
			default:
				return;
			}
		}

		private void PaintDisconnectedBackground(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.Black);
		}

		private void DisconnectedMouseDown(object sender, MouseEventArgs e)
		{
			this.LeaveGame();
		}

		private void HandleAdminConnected(AdminEventArgs e)
		{
			if (e.Data["PlayerNum"] == null && e.Data["PlayerData"] == null)
			{
				return;
			}
			if (e.Data["PlayerNum"] != null && e.Data["PlayerData"] != null)
			{
				this.playerData = (Hashtable)e.Data["PlayerData"];
				foreach (DictionaryEntry entry in this.playerData)
				{
					Hashtable player = (Hashtable)entry.Value;
					if (player.Contains("AvatarHash"))
					{
						string avatarHash = (string)player["AvatarHash"];
						if (!string.IsNullOrEmpty(avatarHash))
						{
							this.tgGame.RequestAsset(avatarHash);
						}
					}
				}
				return;
			}
			throw new InvalidOperationException("LaserChess.Client.ProcessAdminConnected: we got some of the player info - but not all of it");
		}

		private void OnAssetEvent(object sender, AssetEventArgs e)
		{
			if (!File.Exists(e.Filename))
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				Hashtable hashtable = (Hashtable)this.playerData[i];
				if (hashtable != null && hashtable.Contains("AvatarHash"))
				{
					string text = (string)hashtable["AvatarHash"];
					if (text == e.Hash)
					{
						try
						{
							Avatar avatar = this.avatars[0];
							if (i == this.game.LocalPlayerNumber)
							{
								avatar = this.avatars[1];
							}
							avatar.LoadFile(e.Filename);
						}
						catch (ArgumentException)
						{
						}
					}
				}
			}
		}

		private bool CloseClicked(SkinButtonClass button)
		{
			this.LeaveGame();
			return true;
		}

		private bool MinimizeClicked(SkinButtonClass button)
		{
			this.graphics.WindowState = FormWindowState.Minimized;
			return true;
		}

		private bool HelpClicked(SkinButtonClass button)
		{
			if (this.helpButton.SkinItem.Up)
			{
				this.HideHelp();
			}
			else
			{
				this.ShowHelp();
			}
			return true;
		}

		private bool NewGameClicked(SkinTextButtonClass button)
		{
			this.previousPopup = this.popups.Current;
			this.popups.Current = this.confirmPopup;
			this.confirmPopup.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.ConfirmNewGameClicked);
			this.confirmPopup.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.CancelNewGameClicked);
			this.DisableGameButtons();
			return true;
		}

		private bool ConfirmNewGameClicked(SkinButtonClass button)
		{
			this.popups.Current = this.waitPopup;
			this.SendNewGameRequest();
			return true;
		}

		private bool CancelNewGameClicked(SkinButtonClass button)
		{
			this.popups.Current = this.previousPopup;
			this.newGameButton.State = SkinTextButtonClass.States.Enabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Disabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Disabled;
			return true;
		}

		private void SendNewGameRequest()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Game" },
				{ "SubType", "Request" }
			});
		}

		private bool ResignClicked(SkinTextButtonClass button)
		{
			this.previousPopup = this.popups.Current;
			this.popups.Current = this.confirmPopup;
			this.confirmPopup.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.ConfirmResignClicked);
			this.confirmPopup.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.CancelResignClicked);
			this.DisableGameButtons();
			return true;
		}

		private bool ConfirmResignClicked(SkinButtonClass button)
		{
			this.popups.Current = this.waitPopup;
			this.SendResign();
			return true;
		}

		private bool CancelResignClicked(SkinButtonClass button)
		{
			this.popups.Current = this.previousPopup;
			this.newGameButton.State = SkinTextButtonClass.States.Disabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Enabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Enabled;
			return true;
		}

		private void SendResign()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Resign" },
				{ "SubType", "Request" }
			});
		}

		private bool OfferDrawClicked(SkinTextButtonClass button)
		{
			this.previousPopup = this.popups.Current;
			this.popups.Current = this.confirmPopup;
			this.confirmPopup.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.ConfirmOfferDrawClicked);
			this.confirmPopup.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.CancelOfferDrawClicked);
			this.DisableGameButtons();
			return true;
		}

		private bool ConfirmOfferDrawClicked(SkinButtonClass button)
		{
			this.popups.Current = this.waitPopup;
			this.SendDrawRequest();
			return true;
		}

		private bool CancelOfferDrawClicked(SkinButtonClass button)
		{
			this.popups.Current = this.previousPopup;
			this.newGameButton.State = SkinTextButtonClass.States.Disabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Enabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Enabled;
			return true;
		}

		private void SendDrawRequest()
		{
			this.SendToServer(new Hashtable
			{
				{ "MsgType", "Draw" },
				{ "SubType", "Request" }
			});
		}

		private bool ReplayClicked(SkinTextButtonClass button)
		{
			return true;
		}

		private bool MoreHelpClicked(SkinTextButtonClass button)
		{
			Process.Start("http://www.traygames.com/Help/Docs.aspx?doc=laserchess");
			return true;
		}

		private bool ContinueClicked(SkinTextButtonClass button)
		{
			this.HideHelp();
			this.helpButton.SkinItem.Reset();
			return true;
		}

		private void HideHelp()
		{
			this.helpScreen.Display = -1;
			this.moreHelpButton.Hide();
			this.continueButton.Hide();
			this.moreHelpButton.Animate(0f);
			this.continueButton.Animate(0f);
		}

		private void ShowHelp()
		{
			this.helpScreen.Display = 0;
			this.moreHelpButton.Reset();
			this.continueButton.Reset();
			this.moreHelpButton.Animate(0f);
			this.continueButton.Animate(0f);
		}

		private void LeaveGame()
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("MsgType", "UserLeavingGame");
			hashtable.Add("Player", this.game.LocalPlayerNumber);
			this.tgGame.SendToServer(hashtable);
			this.tgGame.Dispose();
			this.graphics.Close();
		}

		private void CreateButtons()
		{
			this.closeButton = new Button(this.graphics, "ButtonClose", this.game.SoundSystem);
			this.closeButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.CloseClicked);
			this.minimizeButton = new Button(this.graphics, "ButtonMinimize", this.game.SoundSystem);
			this.minimizeButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.MinimizeClicked);
			this.helpButton = new Button(this.graphics, "ButtonHelp", this.game.SoundSystem);
			this.helpButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.HelpClicked);
			this.helpButton.SkinItem.Toggle = true;
			this.newGameButton = (SkinTextButtonClass)this.graphics.Skin.GetItem("default", "ButtonNewGame");
			this.newGameButton.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.NewGameClicked);
			this.newGameButton.Text = "New Game";
			this.newGameButton.State = SkinTextButtonClass.States.Disabled;
			this.newGameButton.ButtonUp += this.OnButtonUp;
			this.newGameButton.ButtonDown += this.OnButtonDown;
			this.resignButton = (SkinTextButtonClass)this.graphics.Skin.GetItem("default", "ButtonResign");
			this.resignButton.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.ResignClicked);
			this.resignButton.Text = "Resign";
			this.resignButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.ButtonUp += this.OnButtonUp;
			this.resignButton.ButtonDown += this.OnButtonDown;
			this.offerDrawButton = (SkinTextButtonClass)this.graphics.Skin.GetItem("default", "ButtonOfferDraw");
			this.offerDrawButton.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.OfferDrawClicked);
			this.offerDrawButton.Text = "Offer Draw";
			this.offerDrawButton.State = SkinTextButtonClass.States.Disabled;
			this.offerDrawButton.ButtonUp += this.OnButtonUp;
			this.offerDrawButton.ButtonDown += this.OnButtonDown;
			this.replayButton = (SkinTextButtonClass)this.graphics.Skin.GetItem("default", "ButtonReplay");
			this.replayButton.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.ReplayClicked);
			this.replayButton.Text = "Replay";
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.replayButton.ButtonUp += this.OnButtonUp;
			this.replayButton.ButtonDown += this.OnButtonDown;
			this.replayButton.Hide();
			this.moreHelpButton = (SkinTextButtonClass)this.graphics.Skin.GetItem("default", "ButtonMoreHelp");
			this.moreHelpButton.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.MoreHelpClicked);
			this.moreHelpButton.Text = "More";
			this.moreHelpButton.ButtonUp += this.OnButtonUp;
			this.moreHelpButton.ButtonDown += this.OnButtonDown;
			this.continueButton = (SkinTextButtonClass)this.graphics.Skin.GetItem("default", "ButtonContinue");
			this.continueButton.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.ContinueClicked);
			this.continueButton.Text = "Continue";
			this.continueButton.ButtonUp += this.OnButtonUp;
			this.continueButton.ButtonDown += this.OnButtonDown;
			this.helpScreen = this.graphics.Skin.GetItem("default", "HelpScreen");
			this.HideHelp();
		}

		private void OnButtonDown(object sender, EventArgs e)
		{
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.ButtonDown);
		}

		private void OnButtonUp(object sender, EventArgs e)
		{
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.ButtonUp);
		}

		private void SendToServer(Hashtable message)
		{
			if (!this.tgGame.SendToServer(message))
			{
				throw new InvalidOperationException("Failed to send message to server.");
			}
		}

		private void DisableGameButtons()
		{
			this.newGameButton.State = SkinTextButtonClass.States.Disabled;
			this.replayButton.State = SkinTextButtonClass.States.Disabled;
			this.resignButton.State = SkinTextButtonClass.States.Disabled;
			this.offerDrawButton.State = SkinTextButtonClass.States.Disabled;
			this.commandPanel.Hide();
		}

		void IGameClass.Animate(float elapsed)
		{
			this.UpdateFrame();
		}

		protected void UpdateFrame()
		{
			if (!this.timerStarted)
			{
				TGGameClass.QueryPerformanceCounter(ref this.lastCounter);
				this.timerStarted = true;
			}
			long num = 0L;
			long num2 = 0L;
			TGGameClass.QueryPerformanceCounter(ref num);
			TGGameClass.QueryPerformanceFrequency(ref num2);
			long num3 = num - this.lastCounter;
			this.lastCounter = num;
			float num4 = (float)num3 / (float)num2;
			if (this.popups.Current == null)
			{
				if (this.game.AnimationList.IsIdle())
				{
					this.avatars[0].Active = this.game.GameLogic.CurrentPlayerNumber != this.game.LocalPlayerNumber;
					this.avatars[1].Active = this.game.GameLogic.CurrentPlayerNumber == this.game.LocalPlayerNumber;
				}
			}
			else
			{
				this.avatars[0].Active = false;
				this.avatars[1].Active = false;
			}
			this.avatars[0].Animate(num4);
			this.avatars[1].Animate(num4);
			this.game.Animate(num4);
			if (this.sendReadyWhenIdle && this.game.AnimationList.IsIdle())
			{
				this.SendToServer(new Hashtable { { "MsgType", "Ready" } });
				this.sendReadyWhenIdle = false;
			}
			this.game.SoundSystem.FlushQueues();
			if (this.dataAvailable.WaitOne(0, false))
			{
				this.tgGame.RetrieveMessages();
			}
		}

		public void PreSkinPaint()
		{
		}

		public void Init(GraphicsClass graphicsClass)
		{
			this.graphics = graphicsClass;
			this.tgGame.GameEvent += this.OnGameEvent;
			this.tgGame.AdminEvent += this.OnAdminEvent;
			this.tgGame.AssetEvent += this.OnAssetEvent;
			this.tgGame.DataEvent += this.OnDataEvent;
			this.tgGame.Start(8000, true);
			this.CreateButtons();
			this.game.Init(this.graphics);
			this.commandPanel.Init(this.graphics, this.game);
			this.avatars[0] = new Avatar();
			this.avatars[1] = new Avatar();
			this.avatars[0].Init(graphicsClass, "Remote");
			this.avatars[1].Init(graphicsClass, "Local");
			this.winPopup.Init(graphicsClass, "Win", this.game.SoundSystem);
			this.losePopup.Init(graphicsClass, "Lose", this.game.SoundSystem);
			this.drawPopup.Init(graphicsClass, "Draw", this.game.SoundSystem);
			this.quitPopup.Init(graphicsClass, "Quit", this.game.SoundSystem);
			this.waitPopup.Init(graphicsClass, "Wait", this.game.SoundSystem);
			this.confirmPopup.Init(graphicsClass, "Sure", this.game.SoundSystem);
			this.acceptDrawPopup.Init(graphicsClass, "Accept", this.game.SoundSystem);
			this.playAgainPopup.Init(graphicsClass, "Again", this.game.SoundSystem);
			this.popups.Add(this.winPopup);
			this.popups.Add(this.losePopup);
			this.popups.Add(this.drawPopup);
			this.popups.Add(this.quitPopup);
			this.popups.Add(this.waitPopup);
			this.popups.Add(this.acceptDrawPopup);
			this.popups.Add(this.playAgainPopup);
			this.popups.Current = this.waitPopup;
			try
			{
				Assembly assembly = Assembly.GetAssembly(typeof(GameClass));
				Icon icon = new Icon(assembly.GetManifestResourceStream("LaserChess.Client.LaserChess.ico"));
				this.graphics.Icon = icon;
			}
			catch (NullReferenceException ex)
			{
				Reporter.SoftError(string.Concat(new string[]
				{
					"Init(): Failed to load application icon.",
					Environment.NewLine,
					"Exception = ",
					ex.GetType().ToString(),
					Environment.NewLine,
					"Full Text = ",
					ex.Message,
					Environment.NewLine,
					ex.StackTrace
				}));
			}
		}

		public void OnEnter(EventArgs e)
		{
		}

		public void OnKeyDown(KeyEventArgs e)
		{
		}

		public void OnKeyPress(KeyPressEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			Avatar avatar = this.avatars[1];
			string text = avatar.ChatText;
			if (!avatar.BeingEdited)
			{
				avatar.BeingEdited = true;
				avatar.ChatText = "";
				text = "";
			}
			if (e.KeyChar == '\u001b')
			{
				avatar.ChatText = "";
				return;
			}
			if (e.KeyChar != '\r')
			{
				if (e.KeyChar != '\b')
				{
					if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar) || char.IsSymbol(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
					{
						Avatar avatar2 = avatar;
						avatar2.ChatText += e.KeyChar;
						if (avatar.ChatBoxFull)
						{
							avatar.ChatText = text;
						}
					}
				}
				else if (avatar.ChatText.Length > 0)
				{
					avatar.ChatText = text.Substring(0, text.Length - 1);
					return;
				}
			}
			else if (text.Length > 0)
			{
				this.SendToServer(new Hashtable
				{
					{ "MsgType", "Chat" },
					{
						"Player",
						this.game.LocalPlayerNumber
					},
					{ "Text", text }
				});
				avatar.BeingEdited = false;
				this.game.SoundSystem.PlaySound(SoundSystem.Sound.OutgoingChatMsg);
				return;
			}
		}

		public void OnKeyUp(KeyEventArgs e)
		{
			if (e != null)
			{
			}
		}

		public void OnMouseDown(MouseEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			int num = this.graphics.Skin.IsPointInsideGrip("default", e.X, e.Y);
			this.commandPanel.PieceLogic = null;
			if (num == 4)
			{
				this.dragging = true;
				Point point = this.graphics.PointToScreen(new Point(e.X, e.Y));
				this.dragOffset.X = this.graphics.Left - point.X;
				this.dragOffset.Y = this.graphics.Top - point.Y;
				return;
			}
			if (this.popups.Current != null)
			{
				return;
			}
			if (this.gameLogic.CurrentPlayerNumber != this.game.LocalPlayerNumber)
			{
				return;
			}
			Tile tileAtPoint = this.game.Board.GetTileAtPoint(e.X, e.Y);
			if (tileAtPoint == null)
			{
				return;
			}
			Logic.Piece piece = tileAtPoint.Piece;
			if (piece == null)
			{
				return;
			}
			if (piece.Player.PlayerNumber != this.game.LocalPlayerNumber)
			{
				return;
			}
			this.commandPanel.PieceLogic = piece;
		}

		public void OnMouseMove(MouseEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			if (this.dragging)
			{
				Point point = this.graphics.PointToScreen(new Point(e.X, e.Y));
				this.graphics.Left = point.X + this.dragOffset.X;
				this.graphics.Top = point.Y + this.dragOffset.Y;
			}
		}

		public void OnMouseUp(MouseEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			if (this.dragging)
			{
				this.dragging = false;
			}
		}

		public void Render(Device device)
		{
			this.avatars[0].Render(device);
			this.avatars[1].Render(device);
		}

		public bool DisplayFPS
		{
			get
			{
				return false;
			}
		}

		public string GameName
		{
			get
			{
				return GameClass.gameName;
			}
		}

		public Size GameSize
		{
			get
			{
				return GameClass.gameSize;
			}
		}

		public int MaxActiveFPS
		{
			get
			{
				return 60;
			}
		}

		public int MaxInactiveFPS
		{
			get
			{
				return 5;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						if (this.game != null)
						{
							this.game.Dispose();
							this.game = null;
						}
						if (this.graphics != null)
						{
							this.graphics.Dispose();
							this.graphics = null;
						}
						if (this.tgGame != null)
						{
							this.tgGame.Dispose();
							this.tgGame = null;
						}
						if (this.dataAvailable != null)
						{
							this.dataAvailable.Close();
							this.dataAvailable = null;
						}
						for (int i = 0; i < 2; i++)
						{
							if (this.avatars[i] != null)
							{
								this.avatars[i].Dispose();
								this.avatars[i] = null;
							}
						}
					}
					this.disposed = true;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private Avatar[] avatars = new Avatar[2];

		private Hashtable playerData;

		private bool disposed;

		private Game game;

		private bool sendReadyWhenIdle;

		private CommandPanel commandPanel;

		private static string gameName = "LaserChess";

		private Logic.Game gameLogic;

		private static Size gameSize = new Size(800, 600);

		private GraphicsClass graphics;

		private TGGameClass tgGame = new TGGameClass();

		private string[] playerNames = new string[2];

		private bool dragging;

		private Point dragOffset;

		private AutoResetEvent dataAvailable = new AutoResetEvent(false);

		private Button closeButton;

		private Button minimizeButton;

		private Button helpButton;

		private SkinTextButtonClass newGameButton;

		private SkinTextButtonClass resignButton;

		private SkinTextButtonClass offerDrawButton;

		private SkinTextButtonClass replayButton;

		private SkinItemClass helpScreen;

		private SkinTextButtonClass moreHelpButton;

		private SkinTextButtonClass continueButton;

		private Popup winPopup = new Popup();

		private Popup losePopup = new Popup();

		private Popup drawPopup = new Popup();

		private Popup quitPopup = new Popup();

		private Popup waitPopup = new Popup();

		private PopupYesNo confirmPopup = new PopupYesNo();

		private PopupYesNo acceptDrawPopup = new PopupYesNo();

		private PopupYesNo playAgainPopup = new PopupYesNo();

		private Popup previousPopup;

		private PopupCollection popups = new PopupCollection();

		private bool timerStarted;

		private long lastCounter;
	}
}
