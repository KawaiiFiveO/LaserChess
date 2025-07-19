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
using ns0;
using TG.Client;
using TG.DxGame;
using TG.Game;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000025 RID: 37
	public class GameClass : IDisposable, IGameClass
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x00005AB8 File Offset: 0x00003CB8
		public GameClass(Control control_0)
		{
			this.game_1 = new Logic.Game();
			this.game_0 = new Game(this.game_1, control_0);
			this.commandPanel_0 = new CommandPanel();
			this.game_0.MoveRequestEvent += this.method_0;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00005B9C File Offset: 0x00003D9C
		private void method_0(object sender, MoveRequestEventArgs e)
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Move" },
				{ "SubType", "Request" },
				{ "Piece", e.PieceNumber },
				{ "Command", e.MoveCommand },
				{ "Direction", e.Direction }
			});
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00005C18 File Offset: 0x00003E18
		private void tggameClass_0_GameEvent(object sender, GameEventArgs e)
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
						this.method_1(value);
						return;
					}
					if (text2 == "Chat")
					{
						this.method_4(value);
						return;
					}
					if (text2 == "Draw")
					{
						this.method_5(value);
						return;
					}
					if (text2 == "Game")
					{
						this.method_12(value);
						return;
					}
					if (text2 == "Move")
					{
						this.method_19(value);
						return;
					}
					if (!(text2 == "GameOver"))
					{
						return;
					}
					this.method_3(value);
				}
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00005CD0 File Offset: 0x00003ED0
		private void method_1(Hashtable hashtable_1)
		{
			this.game_0.LocalPlayerNumber = (int)hashtable_1["YourPlayerNum"];
			for (int i = 0; i < 2; i++)
			{
				this.string_1[i] = (string)hashtable_1[i];
				Avatar avatar = this.avatar_0[0];
				if (i == this.game_0.LocalPlayerNumber)
				{
					avatar = this.avatar_0[1];
				}
				avatar.Name = this.string_1[i];
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00005D4C File Offset: 0x00003F4C
		private void method_2(Hashtable hashtable_1)
		{
			this.game_0.SoundSystem.GameInProgress = true;
			this.game_0.GameLogic.Reset();
			this.game_0.LocalPlayerNumber = this.game_0.LocalPlayerNumber;
			this.class0_0.Popup_0 = null;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Enabled;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00005DB4 File Offset: 0x00003FB4
		private void method_3(Hashtable hashtable_1)
		{
			if (hashtable_1.Contains("Result"))
			{
				this.game_0.SoundSystem.GameInProgress = false;
				this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Disabled;
				this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Disabled;
				string text = (string)hashtable_1["Result"];
				string text2;
				if ((text2 = text) != null)
				{
					if (text2 == "Win")
					{
						this.class0_0.Popup_0 = this.popup_0;
						this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.GameWin);
						this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Enabled;
						return;
					}
					if (text2 == "Lose")
					{
						this.class0_0.Popup_0 = this.popup_1;
						this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.GameLose);
						this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Enabled;
						return;
					}
					if (text2 == "Draw")
					{
						this.class0_0.Popup_0 = this.popup_2;
						this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.GameDraw);
						this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Enabled;
						return;
					}
					if (!(text2 == "Quit"))
					{
						return;
					}
					this.class0_0.Popup_0 = this.popup_3;
					this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
				}
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005EF8 File Offset: 0x000040F8
		private void method_4(Hashtable hashtable_1)
		{
			if (hashtable_1.Contains("Player"))
			{
				int num = (int)hashtable_1["Player"];
				if (num < 0)
				{
					return;
				}
				if (num >= 2)
				{
					return;
				}
				if (hashtable_1.Contains("Text"))
				{
					string text = (string)hashtable_1["Text"];
					Avatar avatar = this.avatar_0[0];
					if (num == this.game_0.LocalPlayerNumber)
					{
						avatar = this.avatar_0[1];
					}
					avatar.ChatText = text;
					avatar.StartChatTimer();
					this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.IncomingChatMsg);
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005F90 File Offset: 0x00004190
		private void method_5(Hashtable hashtable_1)
		{
			if (!hashtable_1.Contains("SubType"))
			{
				return;
			}
			string text = (string)hashtable_1["SubType"];
			string text2;
			if ((text2 = text) != null)
			{
				if (text2 == "Request")
				{
					this.method_6(hashtable_1);
					return;
				}
				if (!(text2 == "Decline"))
				{
					return;
				}
				this.method_11(hashtable_1);
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00005FF0 File Offset: 0x000041F0
		private void method_6(Hashtable hashtable_1)
		{
			this.popup_5 = this.class0_0.Popup_0;
			this.class0_0.Popup_0 = this.popupYesNo_1;
			this.popupYesNo_1.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_7);
			this.popupYesNo_1.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_8);
			this.method_47();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000293D File Offset: 0x00000B3D
		private bool method_7(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_4;
			this.method_9();
			return true;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00006060 File Offset: 0x00004260
		private bool method_8(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_5;
			this.method_10();
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Enabled;
			return true;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000060B8 File Offset: 0x000042B8
		private void method_9()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Draw" },
				{ "SubType", "Accept" }
			});
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000060F4 File Offset: 0x000042F4
		private void method_10()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Draw" },
				{ "SubType", "Decline" }
			});
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006130 File Offset: 0x00004330
		private void method_11(Hashtable hashtable_1)
		{
			MessageBox.Show("Your opponent has declined your draw request.", this.GameName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			this.class0_0.Popup_0 = this.popup_5;
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Enabled;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006194 File Offset: 0x00004394
		private void method_12(Hashtable hashtable_1)
		{
			if (!hashtable_1.Contains("SubType"))
			{
				return;
			}
			string text = (string)hashtable_1["SubType"];
			string text2;
			if ((text2 = text) != null)
			{
				if (text2 == "Request")
				{
					this.method_13(hashtable_1);
					return;
				}
				if (text2 == "Decline")
				{
					this.method_18(hashtable_1);
					return;
				}
				if (!(text2 == "Reset"))
				{
					return;
				}
				this.method_2(hashtable_1);
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006208 File Offset: 0x00004408
		private void method_13(Hashtable hashtable_1)
		{
			this.popup_5 = this.class0_0.Popup_0;
			this.class0_0.Popup_0 = this.popupYesNo_2;
			this.popupYesNo_2.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_14);
			this.popupYesNo_2.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_15);
			this.method_47();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00002957 File Offset: 0x00000B57
		private bool method_14(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_4;
			this.method_16();
			return true;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006278 File Offset: 0x00004478
		private bool method_15(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_5;
			this.method_17();
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Disabled;
			return true;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000062D0 File Offset: 0x000044D0
		private void method_16()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Game" },
				{ "SubType", "Accept" }
			});
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000630C File Offset: 0x0000450C
		private void method_17()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Game" },
				{ "SubType", "Decline" }
			});
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006348 File Offset: 0x00004548
		private void method_18(Hashtable hashtable_1)
		{
			MessageBox.Show("Your opponent has declined your new game request.", this.GameName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			this.class0_0.Popup_0 = this.popup_5;
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Disabled;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000063AC File Offset: 0x000045AC
		private void method_19(Hashtable hashtable_1)
		{
			if (!hashtable_1.Contains("SubType"))
			{
				return;
			}
			string text = (string)hashtable_1["SubType"];
			string text2;
			if ((text2 = text) != null)
			{
				if (text2 == "Update")
				{
					this.method_20(hashtable_1);
					return;
				}
				if (!(text2 == "Decline"))
				{
					return;
				}
				GameClass.smethod_0(hashtable_1);
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006408 File Offset: 0x00004608
		private void method_20(Hashtable hashtable_1)
		{
			int num = (int)hashtable_1["Player"];
			int num2 = (int)hashtable_1["Piece"];
			MoveCommand moveCommand = (MoveCommand)hashtable_1["Command"];
			Direction direction = (Direction)hashtable_1["Direction"];
			if (!this.game_1.ProcessMoveCommand(num, num2, moveCommand, direction))
			{
				throw new InvalidOperationException("LaserChess.Client.ProcessMoveAcceptMessage(): Game out of sync.");
			}
			this.bool_1 = true;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00002971 File Offset: 0x00000B71
		private static void smethod_0(Hashtable hashtable_1)
		{
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00002973 File Offset: 0x00000B73
		private void tggameClass_0_DataEvent(object sender, EventArgs e)
		{
			this.autoResetEvent_0.Set();
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000647C File Offset: 0x0000467C
		private void tggameClass_0_AdminEvent(object sender, AdminEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			switch (e.Type)
			{
			case AdminEventType.Connected:
				this.method_23(e);
				return;
			case AdminEventType.Disconnected:
			{
				string text = "name=discback, layer=9, type=generic, top=0, left=0, width=(frame).width, height=(frame).height";
				SkinGenericClass skinGenericClass = (SkinGenericClass)this.graphicsClass_0.Skin.ReadAndCreateSkinItem(this.graphicsClass_0, this.graphicsClass_0.Skin.GetFrame("default"), text);
				SkinGenericClass skinGenericClass2 = skinGenericClass;
				skinGenericClass2.PaintCallback = (PaintEventHandler)Delegate.Combine(skinGenericClass2.PaintCallback, new PaintEventHandler(this.method_21));
				skinGenericClass.Modulation = Color.FromArgb(128, 0, 0, 0);
				skinGenericClass.Enabled = true;
				skinGenericClass.ExclusivelyHandleMouseInput = true;
				skinGenericClass.MouseDown += this.method_22;
				string text2 = "name=disctext, layer=9, type=textbox, hcenter=(frame).hcenter, vcenter=(frame).vcenter, width=(frame).width, height=40, backcolor=1;0;0;0, forecolor=255;0;0, fontname=ariel, size=39, italic=no, bold=no, align=center";
				SkinTextboxClass skinTextboxClass = (SkinTextboxClass)this.graphicsClass_0.Skin.ReadAndCreateSkinItem(this.graphicsClass_0, this.graphicsClass_0.Skin.GetFrame("default"), text2);
				skinTextboxClass.Text = "Disconnected.";
				skinTextboxClass.DropShadow = SkinTextboxClass.DropShadowStyle.Blur;
				this.button_1.SkinItem.Enabled = false;
				this.button_2.SkinItem.Enabled = false;
				this.skinTextButtonClass_0.Enabled = false;
				this.skinTextButtonClass_1.Enabled = false;
				this.skinTextButtonClass_2.Enabled = false;
				this.skinTextButtonClass_3.Enabled = false;
				this.skinTextButtonClass_4.Enabled = false;
				this.skinTextButtonClass_5.Enabled = false;
				return;
			}
			case AdminEventType.Shutdown:
				this.graphicsClass_0.Close();
				break;
			case AdminEventType.Minimize:
			case AdminEventType.Restore:
				break;
			default:
				return;
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00002981 File Offset: 0x00000B81
		private void method_21(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.Black);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00002993 File Offset: 0x00000B93
		private void method_22(object sender, MouseEventArgs e)
		{
			this.method_44();
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00006608 File Offset: 0x00004808
		private void method_23(AdminEventArgs adminEventArgs_0)
		{
			if (adminEventArgs_0.Data["PlayerNum"] == null && adminEventArgs_0.Data["PlayerData"] == null)
			{
				return;
			}
			if (adminEventArgs_0.Data["PlayerNum"] != null && adminEventArgs_0.Data["PlayerData"] != null)
			{
                this.hashtable_0 = (Hashtable)adminEventArgs_0.Data["PlayerData"];

                // Replace the 'using' block with a standard 'foreach' loop.
                // This is the correct way to iterate over a Hashtable.
                foreach (DictionaryEntry entry in this.hashtable_0)
                {
                    // The 'entry' variable now holds the key/value pair directly.
                    // We are interested in the value.
                    Hashtable hashtable = (Hashtable)entry.Value;
                    if (hashtable.Contains("AvatarHash"))
                    {
                        string text = (string)hashtable["AvatarHash"];
                        if (!string.IsNullOrEmpty(text))
                        {
                            this.tggameClass_0.RequestAsset(text);
                        }
                    }
                }
                return;
            }
			throw new InvalidOperationException("LaserChess.Client.ProcessAdminConnected: we got some of the player info - but not all of it");
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006714 File Offset: 0x00004914
		private void tggameClass_0_AssetEvent(object sender, AssetEventArgs e)
		{
			if (!File.Exists(e.Filename))
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				Hashtable hashtable = (Hashtable)this.hashtable_0[i];
				if (hashtable != null && hashtable.Contains("AvatarHash"))
				{
					string text = (string)hashtable["AvatarHash"];
					if (text == e.Hash)
					{
						try
						{
							Avatar avatar = this.avatar_0[0];
							if (i == this.game_0.LocalPlayerNumber)
							{
								avatar = this.avatar_0[1];
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

		// Token: 0x060000EF RID: 239 RVA: 0x0000299B File Offset: 0x00000B9B
		private bool method_24(SkinButtonClass skinButtonClass_0)
		{
			this.method_44();
			return true;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000029A4 File Offset: 0x00000BA4
		private bool method_25(SkinButtonClass skinButtonClass_0)
		{
			this.graphicsClass_0.WindowState = FormWindowState.Minimized;
			return true;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000029B3 File Offset: 0x00000BB3
		private bool method_26(SkinButtonClass skinButtonClass_0)
		{
			if (this.button_2.SkinItem.Up)
			{
				this.method_42();
			}
			else
			{
				this.method_43();
			}
			return true;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000067CC File Offset: 0x000049CC
		private bool method_27(SkinTextButtonClass skinTextButtonClass_6)
		{
			this.popup_5 = this.class0_0.Popup_0;
			this.class0_0.Popup_0 = this.popupYesNo_0;
			this.popupYesNo_0.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_28);
			this.popupYesNo_0.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_29);
			this.method_47();
			return true;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000029D6 File Offset: 0x00000BD6
		private bool method_28(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_4;
			this.method_30();
			return true;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000683C File Offset: 0x00004A3C
		private bool method_29(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_5;
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Disabled;
			return true;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000688C File Offset: 0x00004A8C
		private void method_30()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Game" },
				{ "SubType", "Request" }
			});
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000068C8 File Offset: 0x00004AC8
		private bool method_31(SkinTextButtonClass skinTextButtonClass_6)
		{
			this.popup_5 = this.class0_0.Popup_0;
			this.class0_0.Popup_0 = this.popupYesNo_0;
			this.popupYesNo_0.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_32);
			this.popupYesNo_0.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_33);
			this.method_47();
			return true;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000029F0 File Offset: 0x00000BF0
		private bool method_32(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_4;
			this.method_34();
			return true;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00006938 File Offset: 0x00004B38
		private bool method_33(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_5;
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Enabled;
			return true;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00006988 File Offset: 0x00004B88
		private void method_34()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Resign" },
				{ "SubType", "Request" }
			});
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000069C4 File Offset: 0x00004BC4
		private bool method_35(SkinTextButtonClass skinTextButtonClass_6)
		{
			this.popup_5 = this.class0_0.Popup_0;
			this.class0_0.Popup_0 = this.popupYesNo_0;
			this.popupYesNo_0.ButtonYes.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_36);
			this.popupYesNo_0.ButtonNo.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_37);
			this.method_47();
			return true;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00002A0A File Offset: 0x00000C0A
		private bool method_36(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_4;
			this.method_38();
			return true;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00006938 File Offset: 0x00004B38
		private bool method_37(SkinButtonClass skinButtonClass_0)
		{
			this.class0_0.Popup_0 = this.popup_5;
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Enabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Enabled;
			return true;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00006A34 File Offset: 0x00004C34
		private void method_38()
		{
			this.method_46(new Hashtable
			{
				{ "MsgType", "Draw" },
				{ "SubType", "Request" }
			});
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00002549 File Offset: 0x00000749
		private bool method_39(SkinTextButtonClass skinTextButtonClass_6)
		{
			return true;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00002A24 File Offset: 0x00000C24
		private bool method_40(SkinTextButtonClass skinTextButtonClass_6)
		{
			Process.Start("http://www.traygames.com/Help/Docs.aspx?doc=laserchess");
			return true;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00002A32 File Offset: 0x00000C32
		private bool method_41(SkinTextButtonClass skinTextButtonClass_6)
		{
			this.method_42();
			this.button_2.SkinItem.Reset();
			return true;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006A70 File Offset: 0x00004C70
		private void method_42()
		{
			this.skinItemClass_0.Display = -1;
			this.skinTextButtonClass_4.Hide();
			this.skinTextButtonClass_5.Hide();
			this.skinTextButtonClass_4.Animate(0f);
			this.skinTextButtonClass_5.Animate(0f);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006AC0 File Offset: 0x00004CC0
		private void method_43()
		{
			this.skinItemClass_0.Display = 0;
			this.skinTextButtonClass_4.Reset();
			this.skinTextButtonClass_5.Reset();
			this.skinTextButtonClass_4.Animate(0f);
			this.skinTextButtonClass_5.Animate(0f);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00006B10 File Offset: 0x00004D10
		private void method_44()
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("MsgType", "UserLeavingGame");
			hashtable.Add("Player", this.game_0.LocalPlayerNumber);
			this.tggameClass_0.SendToServer(hashtable);
			this.tggameClass_0.Dispose();
			this.graphicsClass_0.Close();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006B74 File Offset: 0x00004D74
		private void method_45()
		{
			this.button_0 = new Button(this.graphicsClass_0, "ButtonClose", this.game_0.SoundSystem);
			this.button_0.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_24);
			this.button_1 = new Button(this.graphicsClass_0, "ButtonMinimize", this.game_0.SoundSystem);
			this.button_1.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_25);
			this.button_2 = new Button(this.graphicsClass_0, "ButtonHelp", this.game_0.SoundSystem);
			this.button_2.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_26);
			this.button_2.SkinItem.Toggle = true;
			this.skinTextButtonClass_0 = (SkinTextButtonClass)this.graphicsClass_0.Skin.GetItem("default", "ButtonNewGame");
			this.skinTextButtonClass_0.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.method_27);
			this.skinTextButtonClass_0.Text = "New Game";
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_0.ButtonUp += this.skinTextButtonClass_5_ButtonUp;
			this.skinTextButtonClass_0.ButtonDown += this.skinTextButtonClass_5_ButtonDown;
			this.skinTextButtonClass_1 = (SkinTextButtonClass)this.graphicsClass_0.Skin.GetItem("default", "ButtonResign");
			this.skinTextButtonClass_1.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.method_31);
			this.skinTextButtonClass_1.Text = "Resign";
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.ButtonUp += this.skinTextButtonClass_5_ButtonUp;
			this.skinTextButtonClass_1.ButtonDown += this.skinTextButtonClass_5_ButtonDown;
			this.skinTextButtonClass_2 = (SkinTextButtonClass)this.graphicsClass_0.Skin.GetItem("default", "ButtonOfferDraw");
			this.skinTextButtonClass_2.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.method_35);
			this.skinTextButtonClass_2.Text = "Offer Draw";
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_2.ButtonUp += this.skinTextButtonClass_5_ButtonUp;
			this.skinTextButtonClass_2.ButtonDown += this.skinTextButtonClass_5_ButtonDown;
			this.skinTextButtonClass_3 = (SkinTextButtonClass)this.graphicsClass_0.Skin.GetItem("default", "ButtonReplay");
			this.skinTextButtonClass_3.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.method_39);
			this.skinTextButtonClass_3.Text = "Replay";
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_3.ButtonUp += this.skinTextButtonClass_5_ButtonUp;
			this.skinTextButtonClass_3.ButtonDown += this.skinTextButtonClass_5_ButtonDown;
			this.skinTextButtonClass_3.Hide();
			this.skinTextButtonClass_4 = (SkinTextButtonClass)this.graphicsClass_0.Skin.GetItem("default", "ButtonMoreHelp");
			this.skinTextButtonClass_4.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.method_40);
			this.skinTextButtonClass_4.Text = "More";
			this.skinTextButtonClass_4.ButtonUp += this.skinTextButtonClass_5_ButtonUp;
			this.skinTextButtonClass_4.ButtonDown += this.skinTextButtonClass_5_ButtonDown;
			this.skinTextButtonClass_5 = (SkinTextButtonClass)this.graphicsClass_0.Skin.GetItem("default", "ButtonContinue");
			this.skinTextButtonClass_5.ClickRoutine = new SkinTextButtonClass.ClickRoutineCallback(this.method_41);
			this.skinTextButtonClass_5.Text = "Continue";
			this.skinTextButtonClass_5.ButtonUp += this.skinTextButtonClass_5_ButtonUp;
			this.skinTextButtonClass_5.ButtonDown += this.skinTextButtonClass_5_ButtonDown;
			this.skinItemClass_0 = this.graphicsClass_0.Skin.GetItem("default", "HelpScreen");
			this.method_42();
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00002A4B File Offset: 0x00000C4B
		private void skinTextButtonClass_5_ButtonDown(object sender, EventArgs e)
		{
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.ButtonDown);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00002A5E File Offset: 0x00000C5E
		private void skinTextButtonClass_5_ButtonUp(object sender, EventArgs e)
		{
			this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.ButtonUp);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00002A72 File Offset: 0x00000C72
		private void method_46(Hashtable hashtable_1)
		{
			if (!this.tggameClass_0.SendToServer(hashtable_1))
			{
				throw new InvalidOperationException("Failed to send message to server.");
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00002A8D File Offset: 0x00000C8D
		private void method_47()
		{
			this.skinTextButtonClass_0.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_3.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_1.State = SkinTextButtonClass.States.Disabled;
			this.skinTextButtonClass_2.State = SkinTextButtonClass.States.Disabled;
			this.commandPanel_0.Hide();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00002ACA File Offset: 0x00000CCA
		void IGameClass.Animate(float float_0)
		{
			this.method_48();
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006F88 File Offset: 0x00005188
		protected void method_48()
		{
			if (!this.bool_3)
			{
				TGGameClass.QueryPerformanceCounter(ref this.long_0);
				this.bool_3 = true;
			}
			long num = 0L;
			long num2 = 0L;
			TGGameClass.QueryPerformanceCounter(ref num);
			TGGameClass.QueryPerformanceFrequency(ref num2);
			long num3 = num - this.long_0;
			this.long_0 = num;
			float num4 = (float)num3 / (float)num2;
			if (this.class0_0.Popup_0 == null)
			{
				if (this.game_0.AnimationList.IsIdle())
				{
					this.avatar_0[0].Active = this.game_0.GameLogic.CurrentPlayerNumber != this.game_0.LocalPlayerNumber;
					this.avatar_0[1].Active = this.game_0.GameLogic.CurrentPlayerNumber == this.game_0.LocalPlayerNumber;
				}
			}
			else
			{
				this.avatar_0[0].Active = false;
				this.avatar_0[1].Active = false;
			}
			this.avatar_0[0].Animate(num4);
			this.avatar_0[1].Animate(num4);
			this.game_0.Animate(num4);
			if (this.bool_1 && this.game_0.AnimationList.IsIdle())
			{
				this.method_46(new Hashtable { { "MsgType", "Ready" } });
				this.bool_1 = false;
			}
			this.game_0.SoundSystem.FlushQueues();
			if (this.autoResetEvent_0.WaitOne(0, false))
			{
				this.tggameClass_0.RetrieveMessages();
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00002971 File Offset: 0x00000B71
		public void PreSkinPaint()
		{
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00007114 File Offset: 0x00005314
		public void Init(GraphicsClass graphicsClass_1)
		{
			this.graphicsClass_0 = graphicsClass_1;
			this.tggameClass_0.GameEvent += this.tggameClass_0_GameEvent;
			this.tggameClass_0.AdminEvent += this.tggameClass_0_AdminEvent;
			this.tggameClass_0.AssetEvent += this.tggameClass_0_AssetEvent;
			this.tggameClass_0.DataEvent += this.tggameClass_0_DataEvent;
			this.tggameClass_0.Start(8000, true);
			this.method_45();
			this.game_0.Init(this.graphicsClass_0);
			this.commandPanel_0.Init(this.graphicsClass_0, this.game_0);
			this.avatar_0[0] = new Avatar();
			this.avatar_0[1] = new Avatar();
			this.avatar_0[0].Init(graphicsClass_1, "Remote");
			this.avatar_0[1].Init(graphicsClass_1, "Local");
			this.popup_0.Init(graphicsClass_1, "Win", this.game_0.SoundSystem);
			this.popup_1.Init(graphicsClass_1, "Lose", this.game_0.SoundSystem);
			this.popup_2.Init(graphicsClass_1, "Draw", this.game_0.SoundSystem);
			this.popup_3.Init(graphicsClass_1, "Quit", this.game_0.SoundSystem);
			this.popup_4.Init(graphicsClass_1, "Wait", this.game_0.SoundSystem);
			this.popupYesNo_0.Init(graphicsClass_1, "Sure", this.game_0.SoundSystem);
			this.popupYesNo_1.Init(graphicsClass_1, "Accept", this.game_0.SoundSystem);
			this.popupYesNo_2.Init(graphicsClass_1, "Again", this.game_0.SoundSystem);
			this.class0_0.Add(this.popup_0);
			this.class0_0.Add(this.popup_1);
			this.class0_0.Add(this.popup_2);
			this.class0_0.Add(this.popup_3);
			this.class0_0.Add(this.popup_4);
			this.class0_0.Add(this.popupYesNo_1);
			this.class0_0.Add(this.popupYesNo_2);
			this.class0_0.Popup_0 = this.popup_4;
			try
			{
				Assembly assembly = Assembly.GetAssembly(typeof(GameClass));
				Icon icon = new Icon(assembly.GetManifestResourceStream("LaserChess.Client.LaserChess.ico"));
				this.graphicsClass_0.Icon = icon;
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

		// Token: 0x0600010D RID: 269 RVA: 0x00002971 File Offset: 0x00000B71
		public void OnEnter(EventArgs eventArgs_0)
		{
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00002971 File Offset: 0x00000B71
		public void OnKeyDown(KeyEventArgs keyEventArgs_0)
		{
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00007410 File Offset: 0x00005610
		public void OnKeyPress(KeyPressEventArgs keyPressEventArgs_0)
		{
			if (keyPressEventArgs_0 == null)
			{
				return;
			}
			Avatar avatar = this.avatar_0[1];
			string text = avatar.ChatText;
			if (!avatar.BeingEdited)
			{
				avatar.BeingEdited = true;
				avatar.ChatText = "";
				text = "";
			}
			if (keyPressEventArgs_0.KeyChar == '\u001b')
			{
				avatar.ChatText = "";
				return;
			}
			if (keyPressEventArgs_0.KeyChar != '\r')
			{
				if (keyPressEventArgs_0.KeyChar != '\b')
				{
					if (char.IsLetterOrDigit(keyPressEventArgs_0.KeyChar) || char.IsPunctuation(keyPressEventArgs_0.KeyChar) || char.IsSymbol(keyPressEventArgs_0.KeyChar) || char.IsWhiteSpace(keyPressEventArgs_0.KeyChar))
					{
						Avatar avatar2 = avatar;
						avatar2.ChatText += keyPressEventArgs_0.KeyChar;
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
				this.method_46(new Hashtable
				{
					{ "MsgType", "Chat" },
					{
						"Player",
						this.game_0.LocalPlayerNumber
					},
					{ "Text", text }
				});
				avatar.BeingEdited = false;
				this.game_0.SoundSystem.PlaySound(SoundSystem.Sound.OutgoingChatMsg);
				return;
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00002AD2 File Offset: 0x00000CD2
		public void OnKeyUp(KeyEventArgs keyEventArgs_0)
		{
			if (keyEventArgs_0 != null)
			{
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00007574 File Offset: 0x00005774
		public void OnMouseDown(MouseEventArgs mouseEventArgs_0)
		{
			if (mouseEventArgs_0 == null)
			{
				return;
			}
			int num = this.graphicsClass_0.Skin.IsPointInsideGrip("default", mouseEventArgs_0.X, mouseEventArgs_0.Y);
			this.commandPanel_0.PieceLogic = null;
			if (num == 4)
			{
				this.bool_2 = true;
				Point point = this.graphicsClass_0.PointToScreen(new Point(mouseEventArgs_0.X, mouseEventArgs_0.Y));
				this.point_0.X = this.graphicsClass_0.Left - point.X;
				this.point_0.Y = this.graphicsClass_0.Top - point.Y;
				return;
			}
			if (this.class0_0.Popup_0 != null)
			{
				return;
			}
			if (this.game_1.CurrentPlayerNumber != this.game_0.LocalPlayerNumber)
			{
				return;
			}
			Tile tileAtPoint = this.game_0.Board.GetTileAtPoint(mouseEventArgs_0.X, mouseEventArgs_0.Y);
			if (tileAtPoint == null)
			{
				return;
			}
			Logic.Piece piece = tileAtPoint.Piece;
			if (piece == null)
			{
				return;
			}
			if (piece.Player.PlayerNumber != this.game_0.LocalPlayerNumber)
			{
				return;
			}
			this.commandPanel_0.PieceLogic = piece;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00007690 File Offset: 0x00005890
		public void OnMouseMove(MouseEventArgs mouseEventArgs_0)
		{
			if (mouseEventArgs_0 == null)
			{
				return;
			}
			if (this.bool_2)
			{
				Point point = this.graphicsClass_0.PointToScreen(new Point(mouseEventArgs_0.X, mouseEventArgs_0.Y));
				this.graphicsClass_0.Left = point.X + this.point_0.X;
				this.graphicsClass_0.Top = point.Y + this.point_0.Y;
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00002AD7 File Offset: 0x00000CD7
		public void OnMouseUp(MouseEventArgs mouseEventArgs_0)
		{
			if (mouseEventArgs_0 == null)
			{
				return;
			}
			if (this.bool_2)
			{
				this.bool_2 = false;
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00002AEC File Offset: 0x00000CEC
		public void Render(Device device_0)
		{
			this.avatar_0[0].Render(device_0);
			this.avatar_0[1].Render(device_0);
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00002B0A File Offset: 0x00000D0A
		public bool DisplayFPS
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00002B0D File Offset: 0x00000D0D
		public string GameName
		{
			get
			{
				return GameClass.string_0;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00002B14 File Offset: 0x00000D14
		public Size GameSize
		{
			get
			{
				return GameClass.size_0;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00002B1B File Offset: 0x00000D1B
		public int MaxActiveFPS
		{
			get
			{
				return 60;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00002B1F File Offset: 0x00000D1F
		public int MaxInactiveFPS
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007704 File Offset: 0x00005904
		protected virtual void Dispose(bool bool_4)
		{
			lock (this)
			{
				if (!this.bool_0)
				{
					if (bool_4)
					{
						if (this.game_0 != null)
						{
							this.game_0.Dispose();
							this.game_0 = null;
						}
						if (this.graphicsClass_0 != null)
						{
							this.graphicsClass_0.Dispose();
							this.graphicsClass_0 = null;
						}
						if (this.tggameClass_0 != null)
						{
							this.tggameClass_0.Dispose();
							this.tggameClass_0 = null;
						}
						if (this.autoResetEvent_0 != null)
						{
							this.autoResetEvent_0.Close();
							this.autoResetEvent_0 = null;
						}
						for (int i = 0; i < 2; i++)
						{
							if (this.avatar_0[i] != null)
							{
								this.avatar_0[i].Dispose();
								this.avatar_0[i] = null;
							}
						}
					}
					this.bool_0 = true;
				}
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00002B22 File Offset: 0x00000D22
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x040000C8 RID: 200
		private Avatar[] avatar_0 = new Avatar[2];

		// Token: 0x040000C9 RID: 201
		private Hashtable hashtable_0;

		// Token: 0x040000CA RID: 202
		private bool bool_0;

		// Token: 0x040000CB RID: 203
		private Game game_0;

		// Token: 0x040000CC RID: 204
		private bool bool_1;

		// Token: 0x040000CD RID: 205
		private CommandPanel commandPanel_0;

		// Token: 0x040000CE RID: 206
		private static string string_0 = "LaserChess";

		// Token: 0x040000CF RID: 207
		private Logic.Game game_1;

		// Token: 0x040000D0 RID: 208
		private static Size size_0 = new Size(800, 600);

		// Token: 0x040000D1 RID: 209
		private GraphicsClass graphicsClass_0;

		// Token: 0x040000D2 RID: 210
		private TGGameClass tggameClass_0 = new TGGameClass();

		// Token: 0x040000D3 RID: 211
		private string[] string_1 = new string[2];

		// Token: 0x040000D4 RID: 212
		private bool bool_2;

		// Token: 0x040000D5 RID: 213
		private Point point_0;

		// Token: 0x040000D6 RID: 214
		private AutoResetEvent autoResetEvent_0 = new AutoResetEvent(false);

		// Token: 0x040000D7 RID: 215
		private Button button_0;

		// Token: 0x040000D8 RID: 216
		private Button button_1;

		// Token: 0x040000D9 RID: 217
		private Button button_2;

		// Token: 0x040000DA RID: 218
		private SkinTextButtonClass skinTextButtonClass_0;

		// Token: 0x040000DB RID: 219
		private SkinTextButtonClass skinTextButtonClass_1;

		// Token: 0x040000DC RID: 220
		private SkinTextButtonClass skinTextButtonClass_2;

		// Token: 0x040000DD RID: 221
		private SkinTextButtonClass skinTextButtonClass_3;

		// Token: 0x040000DE RID: 222
		private SkinItemClass skinItemClass_0;

		// Token: 0x040000DF RID: 223
		private SkinTextButtonClass skinTextButtonClass_4;

		// Token: 0x040000E0 RID: 224
		private SkinTextButtonClass skinTextButtonClass_5;

		// Token: 0x040000E1 RID: 225
		private Popup popup_0 = new Popup();

		// Token: 0x040000E2 RID: 226
		private Popup popup_1 = new Popup();

		// Token: 0x040000E3 RID: 227
		private Popup popup_2 = new Popup();

		// Token: 0x040000E4 RID: 228
		private Popup popup_3 = new Popup();

		// Token: 0x040000E5 RID: 229
		private Popup popup_4 = new Popup();

		// Token: 0x040000E6 RID: 230
		private PopupYesNo popupYesNo_0 = new PopupYesNo();

		// Token: 0x040000E7 RID: 231
		private PopupYesNo popupYesNo_1 = new PopupYesNo();

		// Token: 0x040000E8 RID: 232
		private PopupYesNo popupYesNo_2 = new PopupYesNo();

		// Token: 0x040000E9 RID: 233
		private Popup popup_5;

		// Token: 0x040000EA RID: 234
		private Class0 class0_0 = new Class0();

		// Token: 0x040000EB RID: 235
		private bool bool_3;

		// Token: 0x040000EC RID: 236
		private long long_0;
	}
}
