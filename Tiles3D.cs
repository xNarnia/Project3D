using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Tiles3D
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class Tiles3D : Mod
	{
		public static Tiles3D Instance;
		public override void Load()
		{
			Instance = this;
			base.Load();
		}


	}
}
