using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TG.Client;
using TG.DxGame;
using TG.Skin;
using TG.Sound;

namespace LaserChess.Client
{
	// Token: 0x0200000D RID: 13
	public class SoundSystem : IDisposable
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00003A40 File Offset: 0x00001C40
		public SoundSystem(Control control_1)
		{
			this.control_0 = control_1;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003B74 File Offset: 0x00001D74
		public void Init(GraphicsClass graphicsClass_1)
		{
			this.graphicsClass_0 = graphicsClass_1;
			this.button_0 = new Button(graphicsClass_1, "ButtonSoundOn", this);
			this.button_0.Show();
			this.button_0.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_3);
			this.button_1 = new Button(graphicsClass_1, "ButtonSoundOff", this);
			this.button_1.Hide();
			this.button_1.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_2);
			this.button_2 = new Button(graphicsClass_1, "ButtonMusicOn", this);
			this.button_2.Show();
			this.button_2.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_4);
			this.button_3 = new Button(graphicsClass_1, "ButtonMusicOff", this);
			this.button_3.Hide();
			this.button_3.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_5);
			try
			{
				this.method_0();
				this.PlaySound(SoundSystem.Sound.Throb);
			}
			catch (Exception ex)
			{
				this.method_6();
				Reporter.SoftError(string.Concat(new string[]
				{
					"Init(): Failed to initialize sound library.",
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

		// Token: 0x0600004B RID: 75 RVA: 0x0000229E File Offset: 0x0000049E
		public void PlaySound(SoundSystem.Sound sound_0)
		{
			this.queue_0.Enqueue(sound_0);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000022AC File Offset: 0x000004AC
		public void StopSound(SoundSystem.Sound sound_0)
		{
			this.queue_1.Enqueue(sound_0);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003CF8 File Offset: 0x00001EF8
		public void FlushQueues()
		{
			while (this.queue_1.Count > 0)
			{
				SoundSystem.Sound sound = this.queue_1.Dequeue();
				if (this.oggPlayManager_0 != null)
				{
					this.oggPlayManager_0.StopOggFile((int)sound);
					Thread.Sleep(0);
				}
			}
			while (this.queue_0.Count > 0)
			{
				SoundSystem.Sound sound2 = this.queue_0.Dequeue();
				if (((sound2 <= SoundSystem.Sound.Throb && this.bool_2) || (sound2 > SoundSystem.Sound.Throb && this.bool_1)) && this.oggPlayManager_0 != null)
				{
					this.oggPlayManager_0.PlayOggFile(this.string_2[(int)sound2], (int)sound2);
					Thread.Sleep(0);
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000022F8 File Offset: 0x000004F8
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000022BA File Offset: 0x000004BA
		public bool GameInProgress
		{
			get
			{
				return this.bool_3;
			}
			set
			{
				if (value != this.bool_3)
				{
					this.bool_3 = value;
					this.StopSound(SoundSystem.Sound.First);
					this.StopSound(SoundSystem.Sound.Theme);
					this.StopSound(SoundSystem.Sound.Throb);
					if (this.bool_3)
					{
						this.PlaySound(SoundSystem.Sound.First);
						return;
					}
					this.PlaySound(SoundSystem.Sound.Throb);
				}
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003D94 File Offset: 0x00001F94
		private void method_0()
		{
			for (int i = 0; i < this.string_1.Length; i++)
			{
				if (!File.Exists(Application.StartupPath + "\\" + this.string_1[i]))
				{
					throw new DllNotFoundException("Unable to load DLL '" + this.string_1[i] + "': The specified module could not be found.");
				}
			}
			if (!Directory.Exists(Application.StartupPath + "\\sounds\\"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\sounds\\");
			}
			for (int j = 0; j < this.string_2.Length; j++)
			{
				string text = Application.StartupPath + "\\sounds\\" + this.string_2[j];
				if (File.Exists(text))
				{
					DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
					DateTime lastWriteTimeUtc2 = File.GetLastWriteTimeUtc(Application.StartupPath + "\\skin.pak");
					if (DateTime.Compare(lastWriteTimeUtc2, lastWriteTimeUtc) > 0)
					{
						File.Delete(text);
					}
				}
				if (!File.Exists(text))
				{
					FileStream fileStream = new FileStream(text, FileMode.Create);
					byte[] array = new byte[1024];
					Stream resourceStream = this.graphicsClass_0.Skin.GetResourceStream("sound/" + this.string_2[j]);
					if (resourceStream != null)
					{
						for (int k = resourceStream.Read(array, 0, array.GetLength(0)); k > 0; k = resourceStream.Read(array, 0, array.GetLength(0)))
						{
							fileStream.Write(array, 0, k);
						}
						resourceStream.Close();
					}
					fileStream.Close();
				}
				this.string_2[j] = text;
			}
			if ((this.oggPlayManager_0 = new OggPlayManager(this.control_0, false, OggPlayManager.SampleSize.SixteenBits)) != null)
			{
				this.oggPlayManager_0.PlayOggFileResult += this.method_7;
				this.oggPlayManager_0.StopOggFileNow += this.method_7;
			}
			this.bool_1 = true;
			this.bool_2 = true;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003F78 File Offset: 0x00002178
		private void method_1()
		{
			try
			{
				this.bool_1 = false;
				this.bool_2 = false;
				if (this.oggPlayManager_0 != null)
				{
					for (SoundSystem.Sound sound = SoundSystem.Sound.First; sound < SoundSystem.Sound.Last; sound++)
					{
						this.StopSound(sound);
					}
					this.FlushQueues();
					this.oggPlayManager_0.WaitForAllOggFiles(new TimeSpan(0, 0, 2));
					this.oggPlayManager_0.Dispose();
					this.oggPlayManager_0 = null;
				}
			}
			catch (NullReferenceException ex)
			{
				Reporter.SoftError(string.Concat(new string[]
				{
					"CloseSoundLibrary(): Failed to properly close sound library.",
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

		// Token: 0x06000052 RID: 82 RVA: 0x00002300 File Offset: 0x00000500
		private bool method_2(SkinButtonClass skinButtonClass_0)
		{
			this.bool_1 = true;
			this.button_0.Show();
			skinButtonClass_0.Hide();
			return true;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004054 File Offset: 0x00002254
		private bool method_3(SkinButtonClass skinButtonClass_0)
		{
			this.bool_1 = false;
			for (SoundSystem.Sound sound = SoundSystem.Sound.First; sound < SoundSystem.Sound.Last; sound++)
			{
				if (sound > SoundSystem.Sound.Throb)
				{
					this.StopSound(sound);
				}
			}
			this.button_1.Show();
			skinButtonClass_0.Hide();
			return true;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000231B File Offset: 0x0000051B
		private bool method_4(SkinButtonClass skinButtonClass_0)
		{
			this.StopSound(SoundSystem.Sound.First);
			this.StopSound(SoundSystem.Sound.Theme);
			this.StopSound(SoundSystem.Sound.Throb);
			this.bool_2 = false;
			this.button_3.Show();
			skinButtonClass_0.Hide();
			return true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004094 File Offset: 0x00002294
		private bool method_5(SkinButtonClass skinButtonClass_0)
		{
			this.StopSound(SoundSystem.Sound.First);
			this.StopSound(SoundSystem.Sound.Theme);
			this.StopSound(SoundSystem.Sound.Throb);
			this.bool_2 = true;
			this.button_2.Show();
			skinButtonClass_0.Hide();
			if (!this.bool_3)
			{
				this.PlaySound(SoundSystem.Sound.Throb);
			}
			else
			{
				this.PlaySound(SoundSystem.Sound.Theme);
			}
			return true;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000234B File Offset: 0x0000054B
		private void method_6()
		{
			this.bool_1 = false;
			this.bool_2 = false;
			this.button_2.Hide();
			this.button_0.Hide();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000040E8 File Offset: 0x000022E8
		private void method_7(object sender, OggPlayEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			SoundSystem.Sound playId = (SoundSystem.Sound)e.PlayId;
			if (e.Success)
			{
				if (e.StopRequest)
				{
					return;
				}
				if (this.bool_2)
				{
					switch (playId)
					{
					case SoundSystem.Sound.First:
						this.PlaySound(SoundSystem.Sound.Theme);
						return;
					case SoundSystem.Sound.Theme:
						this.PlaySound(SoundSystem.Sound.Theme);
						return;
					case SoundSystem.Sound.Throb:
						this.PlaySound(SoundSystem.Sound.Throb);
						break;
					default:
						return;
					}
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004150 File Offset: 0x00002350
		protected virtual void Dispose(bool bool_4)
		{
			lock (this)
			{
				if (!this.bool_0)
				{
					if (bool_4 && this.oggPlayManager_0 != null)
					{
						this.method_1();
					}
					this.bool_0 = true;
				}
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002371 File Offset: 0x00000571
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0400002B RID: 43
		private const string string_0 = "\\sounds\\";

		// Token: 0x0400002C RID: 44
		private bool bool_0;

		// Token: 0x0400002D RID: 45
		private bool bool_1 = true;

		// Token: 0x0400002E RID: 46
		private bool bool_2 = true;

		// Token: 0x0400002F RID: 47
		private Control control_0;

		// Token: 0x04000030 RID: 48
		private OggPlayManager oggPlayManager_0;

		// Token: 0x04000031 RID: 49
		private string[] string_1 = new string[] { "TgPlayOgg.dll", "TgPlayOgg_vorbisfile.dll" };

		// Token: 0x04000032 RID: 50
		private string[] string_2 = new string[]
		{
			"Intro.ogg", "Theme.ogg", "Throb.ogg", "GameWin.ogg", "GameLose.ogg", "GameDraw.ogg", "Minimize.ogg", "Restore.ogg", "ButtonDown.ogg", "ButtonUp.ogg",
			"KeyPress.ogg", "MovePiece.ogg", "TurnPiece.ogg", "CapturePiece.ogg", "TeleportOut.ogg", "TeleportIn.ogg", "FireLaser.ogg", "IncomingChatMsg.ogg", "OutgoingChatMsg.ogg", "PieceHit1.ogg",
			"PieceHit2.ogg", "PieceHit3.ogg", "PieceHit4.ogg"
		};

		// Token: 0x04000033 RID: 51
		private GraphicsClass graphicsClass_0;

		// Token: 0x04000034 RID: 52
		private Button button_0;

		// Token: 0x04000035 RID: 53
		private Button button_1;

		// Token: 0x04000036 RID: 54
		private Button button_2;

		// Token: 0x04000037 RID: 55
		private Button button_3;

		// Token: 0x04000038 RID: 56
		private bool bool_3;

		// Token: 0x04000039 RID: 57
		private Queue<SoundSystem.Sound> queue_0 = new Queue<SoundSystem.Sound>();

		// Token: 0x0400003A RID: 58
		private Queue<SoundSystem.Sound> queue_1 = new Queue<SoundSystem.Sound>();

		// Token: 0x0200000E RID: 14
		public enum Sound
		{
			// Token: 0x0400003C RID: 60
			First,
			// Token: 0x0400003D RID: 61
			Intro = 0,
			// Token: 0x0400003E RID: 62
			Theme,
			// Token: 0x0400003F RID: 63
			Throb,
			// Token: 0x04000040 RID: 64
			GameWin,
			// Token: 0x04000041 RID: 65
			GameLose,
			// Token: 0x04000042 RID: 66
			GameDraw,
			// Token: 0x04000043 RID: 67
			Minimize,
			// Token: 0x04000044 RID: 68
			Restore,
			// Token: 0x04000045 RID: 69
			ButtonDown,
			// Token: 0x04000046 RID: 70
			ButtonUp,
			// Token: 0x04000047 RID: 71
			KeyPress,
			// Token: 0x04000048 RID: 72
			MovePiece,
			// Token: 0x04000049 RID: 73
			TurnPiece,
			// Token: 0x0400004A RID: 74
			CapturePiece,
			// Token: 0x0400004B RID: 75
			TeleportOut,
			// Token: 0x0400004C RID: 76
			TeleportIn,
			// Token: 0x0400004D RID: 77
			FireLaser,
			// Token: 0x0400004E RID: 78
			IncomingChatMsg,
			// Token: 0x0400004F RID: 79
			OutgoingChatMsg,
			// Token: 0x04000050 RID: 80
			PieceHit1,
			// Token: 0x04000051 RID: 81
			PieceHit2,
			// Token: 0x04000052 RID: 82
			PieceHit3,
			// Token: 0x04000053 RID: 83
			PieceHit4,
			// Token: 0x04000054 RID: 84
			Last
		}
	}
}
