using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Tiles3D.Tiles;

namespace Tiles3D.Items
{
	/// <summary>
	/// This is a standard Mod Item.
	/// 
	/// This is used to place the ModTile that will render our 3D Dirt Cube.
	/// </summary>
	public class CthuluItem3D : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CthuluTile3D>(), 0);
			Item.width = 26;
			Item.height = 22;
			Item.value = 50;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock, 1)
				.Register();
		}
	}
}