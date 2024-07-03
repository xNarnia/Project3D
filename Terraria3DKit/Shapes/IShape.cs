using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria3DKit.Shapes
{
	/// <summary>
	/// The base interface for all shapes. 
	/// </summary>
	public interface IShape
	{
		Vector2 Position { get; set; }
		float RotationX { get; set; }
		float RotationY { get; set; }
		float RotationZ { get; set; }
		bool LightingEnabled { get; set; }

		void Draw(float x, float y);
		void Draw(Vector2 position);
		void ResetCameraOrientation();
	}
}
