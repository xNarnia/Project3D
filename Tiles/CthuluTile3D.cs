using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria3DKit.Shapes;

namespace Tiles3D.Tiles
{
	public class CthuluTile3D : ModTile
	{
		public static ShapeModel CthuluModel { get; set; }
		public const int FrameWidth = 18 * 1;
		public const int FrameHeight = 18 * 1;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

			// Place anywhere
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorLeft = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorTop = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}
		//public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		//{
		//	// Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
		//	// Therefore we register the top-left of the tile as a "special point"
		//	// This allows us to draw things in SpecialDraw
		//	if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
		//		Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
		//}

		// SpecialDraw allows us to draw over the tile! 
		// We can hide the tile underneath by covering it with the 3D model.
		//
		// If you want to draw the 3D model behind the player, just use 
		// PostDraw in the same way.
		//public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile t = Main.tile[i, j];
			//Main.NewText("Drawing");
			CthuluModel.RotationZ = 20;

			// This is to ensure it only runs on the first tile.
			// Otherwise this runs for each frame!
			if (t.TileFrameX % 32 == 0 && t.TileFrameY % 32 == 0)
			{
				// Let's rotate it 1 degree each draw.
				CthuluModel.RotationX += 1;

				if (CthuluModel.RotationX == 360)
					CthuluModel.RotationX = 0;

				// Draw the PylonModel using World Coordinates!
				CthuluModel.Draw(new Vector2(i, j).ToWorldCoordinates(16, 0));
			}
		}
	}
}
