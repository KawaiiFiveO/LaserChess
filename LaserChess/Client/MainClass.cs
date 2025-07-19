using System;
using System.Windows.Forms;
using TG.Client;
using TG.DxGame;

namespace LaserChess.Client
{
	// Token: 0x02000029 RID: 41
	public sealed class MainClass
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00002050 File Offset: 0x00000250
		private MainClass()
		{
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00007888 File Offset: 0x00005A88
		[STAThread]
		private static void Main()
		{
			GraphicsClass graphicsClass = null;
			ResourcePak resourcePak = null;
			GameClass gameClass = null;
			Control control = null;
			try
			{
				resourcePak = new ResourcePak("Skin.pak");
				control = new Control();
				gameClass = new GameClass(control);
				graphicsClass = new GraphicsClass(gameClass, resourcePak);
				if (graphicsClass.CreateGraphicsSample())
				{
					graphicsClass.Run();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"Exception occurred in ",
					ex.Source,
					Environment.NewLine,
					ex.Message,
					Environment.NewLine,
					ex.StackTrace
				}), "TrayGames Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			finally
			{
				if (gameClass != null)
				{
					gameClass.Dispose();
				}
				if (graphicsClass != null)
				{
					graphicsClass.Dispose();
				}
				if (resourcePak != null)
				{
					resourcePak.Dispose();
				}
				if (control != null)
				{
					control.Dispose();
				}
			}
		}
	}
}
