using System;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	public class Avatar : IDisposable
	{
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		public void Animate(float elapsed)
		{
			if (this.chatTimeRemaining > 0f)
			{
				this.chatTimeRemaining -= elapsed;
				if (this.chatTimeRemaining <= 0f)
				{
					this.chatTimeRemaining = 0f;
					this.ChatText = "";
				}
			}
			this.pulsePhase += elapsed * 2f * 3.1415927f / 2f;
			if (this.pulsePhase > 6.2831855f)
			{
				this.pulsePhase -= 6.2831855f;
			}
			float num = ((float)Math.Sin((double)this.pulsePhase) + 1f) / 2f;
			float num2 = 0f;
			float num3 = 155f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 155f;
			float num7 = 0f;
			if (this.active)
			{
				num2 = 0f;
				num3 = 155f;
				num4 = 0f;
				num5 = 255f;
				num6 = 255f;
				num7 = 0f;
			}
			float num8 = num * num5 + (1f - num) * num2;
			float num9 = num * num6 + (1f - num) * num3;
			float num10 = num * num7 + (1f - num) * num4;
			this.frame.Modulation = Color.FromArgb(255, (int)num8, (int)num9, (int)num10);
		}

		public void StartChatTimer()
		{
			this.chatTimeRemaining = 10f;
		}

		public void StopChatTimer()
		{
			this.chatTimeRemaining = 0f;
		}

		public string Name
		{
			get
			{
				return this.nameTextBox.Text;
			}
			set
			{
				this.nameTextBox.Text = value;
			}
		}

		public bool BeingEdited
		{
			get
			{
				return this.beingEdited;
			}
			set
			{
				this.beingEdited = value;
				if (!this.beingEdited)
				{
					this.chatBubble.Modulation = Color.FromArgb(255, 255, 255, 255);
					this.StartChatTimer();
					return;
				}
				this.chatBubble.Modulation = Color.FromArgb(160, 255, 255, 96);
				this.StopChatTimer();
			}
		}

		public bool ChatBoxFull
		{
			get
			{
				return this.chatTextBox.NumOccupiedSlots > this.chatTextBox.NumVisibleSlots;
			}
		}

		public string ChatText
		{
			get
			{
				return this.chatTextBox.Text;
			}
			set
			{
				this.chatTextBox.Text = value;
				if (this.chatTextBox.Text.Length == 0)
				{
					this.chatBubble.Display = -1;
					this.chatTextBox.Display = -1;
					return;
				}
				this.chatBubble.Display = 0;
				this.chatTextBox.Display = 0;
			}
		}

		public void Init(GraphicsClass graphicsClass, string prefix)
		{
			if (graphicsClass == null)
			{
				return;
			}
			this.graphics = graphicsClass;
			this.sprite = new Sprite(graphicsClass.GetDevice());
			this.LoadBitmap(new Bitmap(this.graphics.Pak.GetStream("AvatarGeneric.png")));
			this.chatBubble = graphicsClass.Skin.GetItem("default", prefix + "ChatBubble");
			this.chatTextBox = (SkinTextboxClass)graphicsClass.Skin.GetItem("default", prefix + "ChatTextBox");
			this.nameTextBox = (SkinTextboxClass)graphicsClass.Skin.GetItem("default", prefix + "AvatarName");
			this.frame = graphicsClass.Skin.GetItem("default", prefix + "AvatarFrame");
			this.avatarItem = graphicsClass.Skin.GetItem("default", prefix + "Avatar");
			this.position = new Point((int)this.avatarItem.attr.X, (int)this.avatarItem.attr.Y);
		}

		public void LoadFile(string path)
		{
			if (!File.Exists(path))
			{
				return;
			}
			this.LoadBitmap(new Bitmap(path));
		}

		private void LoadBitmap(Bitmap bitmap)
		{
			Surface surface = new Surface(this.graphics.GetDevice(), bitmap, Pool.SystemMemory);
			this.texture = new Texture(this.graphics.GetDevice(), 96, 96, 1, Usage.None, Format.A8R8G8B8, Pool.Default);
			this.graphics.GetDevice().UpdateSurface(surface, this.texture.GetSurfaceLevel(0));
			surface.Dispose();
		}

		public void Render(Device device)
		{
			try
			{
				this.sprite.Begin(SpriteFlags.None);
				this.sprite.Transform = Matrix.Multiply(Matrix.Scaling(1f, 1f, 1f), Matrix.Translation((float)this.position.X, (float)this.position.Y, 0f));
				if (this.texture != null)
				{
					this.sprite.Draw(this.texture, Vector3.Empty, Vector3.Empty, Color.White.ToArgb());
				}
			}
			catch (NullReferenceException ex)
			{
				throw new DeviceLostException("Sprite.Draw threw a NullReferenceException", ex);
			}
			finally
			{
				this.sprite.End();
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
						if (this.texture != null)
						{
							this.texture.Dispose();
							this.texture = null;
						}
						if (this.sprite != null)
						{
							this.sprite.Dispose();
							this.sprite = null;
						}
						if (this.frame != null)
						{
							this.frame.Dispose();
							this.frame = null;
						}
						if (this.avatarItem != null)
						{
							this.avatarItem.Dispose();
							this.avatarItem = null;
						}
						if (this.chatBubble != null)
						{
							this.chatBubble.Dispose();
							this.chatBubble = null;
						}
						if (this.chatTextBox != null)
						{
							this.chatTextBox.Dispose();
							this.chatTextBox = null;
						}
						if (this.nameTextBox != null)
						{
							this.nameTextBox.Dispose();
							this.nameTextBox = null;
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

		private GraphicsClass graphics;

		private Texture texture;

		private Sprite sprite;

		private Point position;

		private SkinItemClass frame;

		private SkinItemClass avatarItem;

		private SkinItemClass chatBubble;

		private SkinTextboxClass chatTextBox;

		private SkinTextboxClass nameTextBox;

		private float chatTimeRemaining;

		private float pulsePhase;

		private bool active;

		private bool disposed;

		private bool beingEdited;
	}
}
