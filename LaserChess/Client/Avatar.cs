using System;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x0200000A RID: 10
	public class Avatar : IDisposable
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002196 File Offset: 0x00000396
		// (set) Token: 0x06000027 RID: 39 RVA: 0x0000219E File Offset: 0x0000039E
		public bool Active
		{
			get
			{
				return this.bool_0;
			}
			set
			{
				this.bool_0 = value;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000030B8 File Offset: 0x000012B8
		public void Animate(float float_2)
		{
			if (this.float_0 > 0f)
			{
				this.float_0 -= float_2;
				if (this.float_0 <= 0f)
				{
					this.float_0 = 0f;
					this.ChatText = "";
				}
			}
			this.float_1 += float_2 * 2f * 3.1415927f / 2f;
			if (this.float_1 > 6.2831855f)
			{
				this.float_1 -= 6.2831855f;
			}
			float num = ((float)Math.Sin((double)this.float_1) + 1f) / 2f;
			float num2 = 0f;
			float num3 = 155f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 155f;
			float num7 = 0f;
			if (this.bool_0)
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
			this.skinItemClass_0.Modulation = Color.FromArgb(255, (int)num8, (int)num9, (int)num10);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000021A7 File Offset: 0x000003A7
		public void StartChatTimer()
		{
			this.float_0 = 10f;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000021B4 File Offset: 0x000003B4
		public void StopChatTimer()
		{
			this.float_0 = 0f;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000021C1 File Offset: 0x000003C1
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000021CE File Offset: 0x000003CE
		public string Name
		{
			get
			{
				return this.skinTextboxClass_1.Text;
			}
			set
			{
				this.skinTextboxClass_1.Text = value;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000021DC File Offset: 0x000003DC
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00003204 File Offset: 0x00001404
		public bool BeingEdited
		{
			get
			{
				return this.bool_2;
			}
			set
			{
				this.bool_2 = value;
				if (!this.bool_2)
				{
					this.skinItemClass_2.Modulation = Color.FromArgb(255, 255, 255, 255);
					this.StartChatTimer();
					return;
				}
				this.skinItemClass_2.Modulation = Color.FromArgb(160, 255, 255, 96);
				this.StopChatTimer();
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021E4 File Offset: 0x000003E4
		public bool ChatBoxFull
		{
			get
			{
				return this.skinTextboxClass_0.NumOccupiedSlots > this.skinTextboxClass_0.NumVisibleSlots;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000021FE File Offset: 0x000003FE
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00003274 File Offset: 0x00001474
		public string ChatText
		{
			get
			{
				return this.skinTextboxClass_0.Text;
			}
			set
			{
				this.skinTextboxClass_0.Text = value;
				if (this.skinTextboxClass_0.Text.Length == 0)
				{
					this.skinItemClass_2.Display = -1;
					this.skinTextboxClass_0.Display = -1;
					return;
				}
				this.skinItemClass_2.Display = 0;
				this.skinTextboxClass_0.Display = 0;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000032D0 File Offset: 0x000014D0
		public void Init(GraphicsClass graphicsClass_1, string string_0)
		{
			if (graphicsClass_1 == null)
			{
				return;
			}
			this.graphicsClass_0 = graphicsClass_1;
			this.sprite_0 = new Sprite(graphicsClass_1.GetDevice());
			this.method_0(new Bitmap(this.graphicsClass_0.Pak.GetStream("AvatarGeneric.png")));
			this.skinItemClass_2 = graphicsClass_1.Skin.GetItem("default", string_0 + "ChatBubble");
			this.skinTextboxClass_0 = (SkinTextboxClass)graphicsClass_1.Skin.GetItem("default", string_0 + "ChatTextBox");
			this.skinTextboxClass_1 = (SkinTextboxClass)graphicsClass_1.Skin.GetItem("default", string_0 + "AvatarName");
			this.skinItemClass_0 = graphicsClass_1.Skin.GetItem("default", string_0 + "AvatarFrame");
			this.skinItemClass_1 = graphicsClass_1.Skin.GetItem("default", string_0 + "Avatar");
			this.point_0 = new Point((int)this.skinItemClass_1.attr.X, (int)this.skinItemClass_1.attr.Y);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000220B File Offset: 0x0000040B
		public void LoadFile(string string_0)
		{
			if (!File.Exists(string_0))
			{
				return;
			}
			this.method_0(new Bitmap(string_0));
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000033F8 File Offset: 0x000015F8
		private void method_0(Bitmap bitmap_0)
		{
			Surface surface = new Surface(this.graphicsClass_0.GetDevice(), bitmap_0, Pool.SystemMemory);
			this.texture_0 = new Texture(this.graphicsClass_0.GetDevice(), 96, 96, 1, Usage.None, Format.A8R8G8B8, Pool.Default);
			this.graphicsClass_0.GetDevice().UpdateSurface(surface, this.texture_0.GetSurfaceLevel(0));
			surface.Dispose();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000345C File Offset: 0x0000165C
		public void Render(Device device_0)
		{
			try
			{
				this.sprite_0.Begin(SpriteFlags.None);
				this.sprite_0.Transform = Matrix.Multiply(Matrix.Scaling(1f, 1f, 1f), Matrix.Translation((float)this.point_0.X, (float)this.point_0.Y, 0f));
				if (this.texture_0 != null)
				{
					this.sprite_0.Draw(this.texture_0, Vector3.Empty, Vector3.Empty, Color.White.ToArgb());
				}
			}
			catch (NullReferenceException ex)
			{
				throw new DeviceLostException("Sprite.Draw threw a NullReferenceException", ex);
			}
			finally
			{
				this.sprite_0.End();
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000352C File Offset: 0x0000172C
		protected virtual void Dispose(bool bool_3)
		{
			lock (this)
			{
				if (!this.bool_1)
				{
					if (bool_3)
					{
						if (this.texture_0 != null)
						{
							this.texture_0.Dispose();
							this.texture_0 = null;
						}
						if (this.sprite_0 != null)
						{
							this.sprite_0.Dispose();
							this.sprite_0 = null;
						}
						if (this.skinItemClass_0 != null)
						{
							this.skinItemClass_0.Dispose();
							this.skinItemClass_0 = null;
						}
						if (this.skinItemClass_1 != null)
						{
							this.skinItemClass_1.Dispose();
							this.skinItemClass_1 = null;
						}
						if (this.skinItemClass_2 != null)
						{
							this.skinItemClass_2.Dispose();
							this.skinItemClass_2 = null;
						}
						if (this.skinTextboxClass_0 != null)
						{
							this.skinTextboxClass_0.Dispose();
							this.skinTextboxClass_0 = null;
						}
						if (this.skinTextboxClass_1 != null)
						{
							this.skinTextboxClass_1.Dispose();
							this.skinTextboxClass_1 = null;
						}
					}
					this.bool_1 = true;
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002222 File Offset: 0x00000422
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000015 RID: 21
		private GraphicsClass graphicsClass_0;

		// Token: 0x04000016 RID: 22
		private Texture texture_0;

		// Token: 0x04000017 RID: 23
		private Sprite sprite_0;

		// Token: 0x04000018 RID: 24
		private Point point_0;

		// Token: 0x04000019 RID: 25
		private SkinItemClass skinItemClass_0;

		// Token: 0x0400001A RID: 26
		private SkinItemClass skinItemClass_1;

		// Token: 0x0400001B RID: 27
		private SkinItemClass skinItemClass_2;

		// Token: 0x0400001C RID: 28
		private SkinTextboxClass skinTextboxClass_0;

		// Token: 0x0400001D RID: 29
		private SkinTextboxClass skinTextboxClass_1;

		// Token: 0x0400001E RID: 30
		private float float_0;

		// Token: 0x0400001F RID: 31
		private float float_1;

		// Token: 0x04000020 RID: 32
		private bool bool_0;

		// Token: 0x04000021 RID: 33
		private bool bool_1;

		// Token: 0x04000022 RID: 34
		private bool bool_2;
	}
}
