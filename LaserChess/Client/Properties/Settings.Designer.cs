using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace LaserChess.Client.Properties
{
	// Token: 0x0200002B RID: 43
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
	[CompilerGenerated]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00002C31 File Offset: 0x00000E31
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x040000F4 RID: 244
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
