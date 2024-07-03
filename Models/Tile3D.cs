using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Tiles3D.Models
{
	public class Tile3D
	{
		// SpecialDraw allows us to draw over the tile! 
		// We can hide the tile underneath by covering it with the 3D model.
		//
		// If you want to draw the 3D model behind the player, just use 
		// PostDraw in the same way.
		public void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			//Tile t = Main.tile[i, j];

			//// This is to ensure it only runs on the first frame.
			//// Otherwise this runs for each frame!
			//if (t.frameX % 32 == 0 && t.frameY % 32 == 0)
			//{
			//	// Let's rotate it 1 degree each draw.
			//	PylonModel.RotationZ += 1;

			//	if (PylonModel.RotationZ == 360)
			//		PylonModel.RotationZ = 0;

			//	// Draw the PylonModel using World Coordinates!
			//	PylonModel.Draw(new Vector2(i, j).ToWorldCoordinates(16, 0));
			//}

			//base.SpecialDraw(i, j, spriteBatch);
		}
	}
}
