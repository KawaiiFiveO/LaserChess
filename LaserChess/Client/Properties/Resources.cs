using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace LaserChess.Client.Properties
{
	// Token: 0x0200002A RID: 42
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	internal class Resources
	{
		// Token: 0x0600012C RID: 300 RVA: 0x00002050 File Offset: 0x00000250
		internal Resources()
		{
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00007970 File Offset: 0x00005B70
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("LaserChess.Client.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00002C22 File Offset: 0x00000E22
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00002C29 File Offset: 0x00000E29
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x040000F2 RID: 242
		private static ResourceManager resourceMan;

		// Token: 0x040000F3 RID: 243
		private static CultureInfo resourceCulture;
	}
}
