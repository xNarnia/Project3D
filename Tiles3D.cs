using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria3DKit.Shapes;
using Tiles3D.Tiles;

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

		public override void PostSetupContent()
		{
			base.PostSetupContent();

			// This pylon will be rendered at an inconsistent FPS using SpecialDraw 
			// (slows down when zoomed in)
			var cthulu = new ShapeModel();
			CthuluTile3D.CthuluModel = cthulu;
		}

	}
}
