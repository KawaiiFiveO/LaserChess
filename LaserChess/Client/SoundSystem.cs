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
	public class SoundSystem : IDisposable
	{
		public SoundSystem(Control control)
		{
			this.control = control;
		}

		public void Init(GraphicsClass graphicsClass)
		{
			this.graphics = graphicsClass;
			this.soundOnButton = new Button(graphicsClass, "ButtonSoundOn", this);
			this.soundOnButton.Show();
			this.soundOnButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.DisableSoundClicked);
			this.soundOffButton = new Button(graphicsClass, "ButtonSoundOff", this);
			this.soundOffButton.Hide();
			this.soundOffButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.EnableSoundClicked);
			this.musicOnButton = new Button(graphicsClass, "ButtonMusicOn", this);
			this.musicOnButton.Show();
			this.musicOnButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.DisableMusicClicked);
			this.musicOffButton = new Button(graphicsClass, "ButtonMusicOff", this);
			this.musicOffButton.Hide();
			this.musicOffButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.EnableMusicClicked);
			try
			{
				this.OpenSoundLibrary();
				this.PlaySound(SoundSystem.Sound.Throb);
			}
			catch (Exception ex)
			{
				this.DisableSoundControls();
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

		public void PlaySound(SoundSystem.Sound sound)
		{
			this.playQueue.Enqueue(sound);
		}

		public void StopSound(SoundSystem.Sound sound)
		{
			this.stopQueue.Enqueue(sound);
		}

		public void FlushQueues()
		{
			while (this.stopQueue.Count > 0)
			{
				SoundSystem.Sound sound = this.stopQueue.Dequeue();
				if (this.oggPlayer != null)
				{
					this.oggPlayer.StopOggFile((int)sound);
					Thread.Sleep(0);
				}
			}
			while (this.playQueue.Count > 0)
			{
				SoundSystem.Sound sound2 = this.playQueue.Dequeue();
				if (((sound2 <= SoundSystem.Sound.Throb && this.musicEnabled) || (sound2 > SoundSystem.Sound.Throb && this.soundEnabled)) && this.oggPlayer != null)
				{
					this.oggPlayer.PlayOggFile(this.soundFiles[(int)sound2], (int)sound2);
					Thread.Sleep(0);
				}
			}
		}

		public bool GameInProgress
		{
			get
			{
				return this.gameInProgress;
			}
			set
			{
				if (value != this.gameInProgress)
				{
					this.gameInProgress = value;
					this.StopSound(SoundSystem.Sound.First);
					this.StopSound(SoundSystem.Sound.Theme);
					this.StopSound(SoundSystem.Sound.Throb);
					if (this.gameInProgress)
					{
						this.PlaySound(SoundSystem.Sound.First);
						return;
					}
					this.PlaySound(SoundSystem.Sound.Throb);
				}
			}
		}

		private void OpenSoundLibrary()
		{
			for (int i = 0; i < this.requiredDlls.Length; i++)
			{
				if (!File.Exists(Application.StartupPath + "\\" + this.requiredDlls[i]))
				{
					throw new DllNotFoundException("Unable to load DLL '" + this.requiredDlls[i] + "': The specified module could not be found.");
				}
			}
			if (!Directory.Exists(Application.StartupPath + "\\sounds\\"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\sounds\\");
			}
			for (int j = 0; j < this.soundFiles.Length; j++)
			{
				string text = Application.StartupPath + "\\sounds\\" + this.soundFiles[j];
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
					Stream resourceStream = this.graphics.Skin.GetResourceStream("sound/" + this.soundFiles[j]);
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
				this.soundFiles[j] = text;
			}
			if ((this.oggPlayer = new OggPlayManager(this.control, false, OggPlayManager.SampleSize.SixteenBits)) != null)
			{
				this.oggPlayer.PlayOggFileResult += this.OnOggPlayEvent;
				this.oggPlayer.StopOggFileNow += this.OnOggPlayEvent;
			}
			this.soundEnabled = true;
			this.musicEnabled = true;
		}

		private void CloseSoundLibrary()
		{
			try
			{
				this.soundEnabled = false;
				this.musicEnabled = false;
				if (this.oggPlayer != null)
				{
					for (SoundSystem.Sound sound = SoundSystem.Sound.First; sound < SoundSystem.Sound.Last; sound++)
					{
						this.StopSound(sound);
					}
					this.FlushQueues();
					this.oggPlayer.WaitForAllOggFiles(new TimeSpan(0, 0, 2));
					this.oggPlayer.Dispose();
					this.oggPlayer = null;
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

		private bool EnableSoundClicked(SkinButtonClass button)
		{
			this.soundEnabled = true;
			this.soundOnButton.Show();
			button.Hide();
			return true;
		}

		private bool DisableSoundClicked(SkinButtonClass button)
		{
			this.soundEnabled = false;
			for (SoundSystem.Sound sound = SoundSystem.Sound.First; sound < SoundSystem.Sound.Last; sound++)
			{
				if (sound > SoundSystem.Sound.Throb)
				{
					this.StopSound(sound);
				}
			}
			this.soundOffButton.Show();
			button.Hide();
			return true;
		}

		private bool DisableMusicClicked(SkinButtonClass button)
		{
			this.StopSound(SoundSystem.Sound.First);
			this.StopSound(SoundSystem.Sound.Theme);
			this.StopSound(SoundSystem.Sound.Throb);
			this.musicEnabled = false;
			this.musicOffButton.Show();
			button.Hide();
			return true;
		}

		private bool EnableMusicClicked(SkinButtonClass button)
		{
			this.StopSound(SoundSystem.Sound.First);
			this.StopSound(SoundSystem.Sound.Theme);
			this.StopSound(SoundSystem.Sound.Throb);
			this.musicEnabled = true;
			this.musicOnButton.Show();
			button.Hide();
			if (!this.gameInProgress)
			{
				this.PlaySound(SoundSystem.Sound.Throb);
			}
			else
			{
				this.PlaySound(SoundSystem.Sound.Theme);
			}
			return true;
		}

		private void DisableSoundControls()
		{
			this.soundEnabled = false;
			this.musicEnabled = false;
			this.musicOnButton.Hide();
			this.soundOnButton.Hide();
		}

		private void OnOggPlayEvent(object sender, OggPlayEventArgs e)
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
				if (this.musicEnabled)
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

		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.disposed)
				{
					if (disposing && this.oggPlayer != null)
					{
						this.CloseSoundLibrary();
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

		private const string SoundsFolder = "\\sounds\\";

		private bool disposed;

		private bool soundEnabled = true;

		private bool musicEnabled = true;

		private Control control;

		private OggPlayManager oggPlayer;

		private string[] requiredDlls = new string[] { "TgPlayOgg.dll", "TgPlayOgg_vorbisfile.dll" };

		private string[] soundFiles = new string[]
		{
			"Intro.ogg", "Theme.ogg", "Throb.ogg", "GameWin.ogg", "GameLose.ogg", "GameDraw.ogg", "Minimize.ogg", "Restore.ogg", "ButtonDown.ogg", "ButtonUp.ogg",
			"KeyPress.ogg", "MovePiece.ogg", "TurnPiece.ogg", "CapturePiece.ogg", "TeleportOut.ogg", "TeleportIn.ogg", "FireLaser.ogg", "IncomingChatMsg.ogg", "OutgoingChatMsg.ogg", "PieceHit1.ogg",
			"PieceHit2.ogg", "PieceHit3.ogg", "PieceHit4.ogg"
		};

		private GraphicsClass graphics;

		private Button soundOnButton;

		private Button soundOffButton;

		private Button musicOnButton;

		private Button musicOffButton;

		private bool gameInProgress;

		private Queue<SoundSystem.Sound> playQueue = new Queue<SoundSystem.Sound>();

		private Queue<SoundSystem.Sound> stopQueue = new Queue<SoundSystem.Sound>();

		public enum Sound
		{
			First,
			Intro = 0,
			Theme,
			Throb,
			GameWin,
			GameLose,
			GameDraw,
			Minimize,
			Restore,
			ButtonDown,
			ButtonUp,
			KeyPress,
			MovePiece,
			TurnPiece,
			CapturePiece,
			TeleportOut,
			TeleportIn,
			FireLaser,
			IncomingChatMsg,
			OutgoingChatMsg,
			PieceHit1,
			PieceHit2,
			PieceHit3,
			PieceHit4,
			Last
		}
	}
}
