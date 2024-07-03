using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Tiles3D.AssimpLib;
using static Terraria.GameContent.TextureAssets;

namespace Terraria3DKit.Shapes
{
	/// <summary>
	/// An enum containing all possible projection types.
	/// </summary>
	public enum ProjectionType
	{
		Orthographic
		//Perspective
	}

	public class ShapeModel : IShape
	{
		/// <summary>
		/// The position of this shape, usually from its origin.
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// The rotation on the X-axis in degrees.
		/// </summary>
		public float RotationX { get; set; }

		/// <summary>
		/// The rotation on the Y-axis in degrees.
		/// </summary>
		public float RotationY { get; set; }

		/// <summary>
		/// The rotation on the Z-axis in degrees.
		/// </summary>
		public float RotationZ { get; set; }

		/// <summary>
		/// <para>A flag that designates whether to use Terraria's lighting or ignore it entirely.</para>
		/// Setting this to disabled will leave the model unaffected by darkness.
		/// </summary>
		public bool LightingEnabled { get; set; }

		/// <summary>
		/// <para>Enables coordinate and scaling fixes for models placed in the ModifyInterfaceLayer method.</para>
		/// </summary>
		public bool ModifyInterfaceLayerFix { get; set; }

		/// <summary>
		/// <para>The number of frames to skip before drawing again.</para>
		/// Increase this improves performance. 
		/// </summary>
		//public int FrameskipCount { get; set; }

		/// <summary>
		/// Determines whether the model is visible. This will suspend drawing of the model.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// <para>The projection (or view-mode) type this shape will use.</para>
		/// Defaults to perspective.
		/// </summary>
		public ProjectionType Projection { get; set; }

		// Assimp Model Variables
		private AssimpModel model { get; set; }
		private IndexBuffer oldBuffer { get; set; }
		private RenderTargetBinding[] oldGraphicsBinding { get; set; }
		private VertexBufferBinding[] oldVertexBinding { get; set; }
		private GraphicsDevice device { get; set; }

		public RenderTarget2D renderTarget { get; set; }
		// End

		private BasicEffect _effect { get; set; }

		private int _polyCount { get; set; }
		//private int _framesSkipped { get; set; }
		private GameTime _previousTime { get; set; }

		private static RasterizerState rasterizerState;
		private static DepthStencilState depthStencilState;
		private static BlendState blendState;

		private static RasterizerState previousRS { get; set; }
		private static DepthStencilState previousDS { get; set; }
		private static BlendState previousBS { get; set; }

		/// <summary>
		/// <para>The base model for all shapes.</para>
		/// When inherited from, it provides drawing functionality, vertice handling, and graphic rendering management.
		/// </summary>
		public ShapeModel()
		{
			model = new AssimpModel(Path.Combine(ModLoader.ModPath, "Tiles3D", "Model", "eoc.fbx"),
				PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

			device = Main.instance.GraphicsDevice;

			oldBuffer = device.Indices;
			oldGraphicsBinding = device.GetRenderTargets();
			oldVertexBinding = device.GetVertexBuffers();


			Main.QueueMainThreadAction(() =>
			{
				renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

				//effect = shadetest3d.Instance.Assets.Request<Effect>("Assets/3DModel4", AssetRequestMode.ImmediateLoad).Value;




				//effect.Parameters["WorldMatrix"].SetValue(Main.GameViewMatrix.TransformationMatrix); 
				//effect.Parameters["ViewMatrix"].SetValue(viewMatrix);
				//effect.Parameters["ProjectionMatrix"].SetValue(projectionMatrix);

			});

			Main.NewText("Loaded Cthulu");

			Position = Vector2.Zero;
			RotationX = 0f;
			RotationY = 0f;
			RotationZ = 0f;
			LightingEnabled = true;
			Projection = ProjectionType.Orthographic;
			ModifyInterfaceLayerFix = false;

			if (rasterizerState == null)
			{
				rasterizerState = new RasterizerState();
				rasterizerState.CullMode = CullMode.None;
			}

			if (depthStencilState == null)
				depthStencilState = DepthStencilState.Default;

			if(blendState == null)
				blendState = BlendState.AlphaBlend;

			previousRS = previousRS ?? Main.graphics.GraphicsDevice.RasterizerState;
			previousDS = previousDS ?? Main.graphics.GraphicsDevice.DepthStencilState;
			previousBS = previousBS ?? Main.graphics.GraphicsDevice.BlendState;

			//FrameskipCount = 5;
			Visible = true;

			//_framesSkipped = 0;
		}

		/// <summary>
		/// Resets the rotation of this model to 0 on the X, Y, and Z axis.
		/// </summary>
		public void ResetCameraOrientation()
		{
			RotationX = 0f;
			RotationY = 0f;
			RotationZ = 0f;
		}

		/// <summary>
		/// Rotates the camera around the origin by the given rotation vector in degrees.
		/// </summary>
		public void RotateCamera(Vector3 rotationVectorDegrees)
		{
			RotationX += rotationVectorDegrees.X;
			RotationY += rotationVectorDegrees.Y;
			RotationZ += rotationVectorDegrees.Z;
		}

		/// <summary>
		/// <para>Draws the shape at the desired coordinates.</para>
		/// Coordinates are aligned to world coordinates.
		/// </summary>
		/// <param name="spriteBatch">Spritebatch that will be used to draw this shape.</param>
		/// <param name="x">The X world coordinate to draw this shape.</param>
		/// <param name="y">The Y world coordinate to draw this shape.</param>
		public void Draw(float x = 0, float y = 0)
			=> Draw(new Vector2(x, y));

		/// <summary>
		/// <para>Draws the shape at the desired coordinates. Coordinates are aligned to world coordinates.</para>
		/// <para>If you place this draw in a Tile draw such as SpecialDraw, post-draw, or another Tile draw, it will not draw at 60 fps.</para>
		/// <para>To draw your 3D model at 60 FPS, place it in the main Mod Draw function of your mod and set (ModifyInterfaceLayerFix = true).</para>
		/// </summary>
		/// <param name="spriteBatch">Spritebatch that will be used to draw this shape.</param>
		/// <param name="pos">The world coordinate to draw this shape.</param>
		public void Draw(Vector2 pos)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			//if (_framesSkipped >= FrameskipCount)
			//{
			//	_framesSkipped = 0;
			//	return;
			//}
			//else
			//{
			//	_framesSkipped++;
			//}

			if (!Visible)
				return;

			_effect = new BasicEffect(spriteBatch.GraphicsDevice);

			GraphicsDevice device = Main.instance.GraphicsDevice;

			// The effect world matrix will handle all visual tranformations to our model
			// This is easier than transforming all the vertices of our model one-by-one
			_effect.World = Matrix.Identity;
			_effect.World *= Matrix.CreateScale(80);
			_effect.World *= Matrix.CreateTranslation(0, 0, 0);
			_effect.World *=
					// Handles Rotation
					Matrix.CreateFromYawPitchRoll(
						(float)RotationX.ToRadians(),
						(float)RotationY.ToRadians(),
						(float)RotationZ.ToRadians());

			//if (ModifyInterfaceLayerFix)
			if (false)
			{
				// Handles Game Zoom set by the Zoom slider
				_effect.World *=
					Matrix.CreateScale(
						Main.GameViewMatrix.Zoom.X,
						Main.GameViewMatrix.Zoom.X,
						Main.GameViewMatrix.Zoom.Y);
			}

			var posWorldtoScreenCoords = pos - Main.screenPosition;

			// This is not respecting coordinates when put in Tile Draw
			// When this is ran through ModifyLayerInterface, it works appropriately
			// I have no clue what is going on. This is a major impedement.
			if (false)
			{
				float aspectRatio =
					Main.graphics.GraphicsDevice.Viewport.Width / (float)Main.graphics.GraphicsDevice.Viewport.Height;
				float fieldOfView = MathHelper.PiOver4;
				float nearClipPlane = 1;
				float farClipPlane = 10000;

				_effect.View = Matrix.CreateLookAt(
					new Vector3(0, 100, 0), // Camera Position
					Vector3.Zero, // Look-at Vector
					Vector3.UnitZ); // Up Axis is Z

				var x = (((float)Main.screenWidth / 2f)) + posWorldtoScreenCoords.X;
				var y = (((float)Main.screenHeight / 2f)) + posWorldtoScreenCoords.Y;
				var yOffset = -862f;

				if (ModifyInterfaceLayerFix)
				{
					x *= Main.GameZoomTarget;
					y *= Main.GameZoomTarget;
				}
				else
				{
					x += Main.GameZoomTarget;
					y += Main.GameZoomTarget;
				}

				_effect.World = _effect.World
					* Matrix.CreateTranslation(x, yOffset, y);

				_effect.Projection = Matrix.CreatePerspectiveFieldOfView(
					fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
			}
			else if (Projection == ProjectionType.Orthographic)
			{
				// We control the objects rotation by controlling the world
				// This way, we don't need to adjust every individual vertice

				//_effect.View = Matrix.CreateLookAt(
				//	new Vector3(0, 1, 0), // Camera Position
				//	Vector3.Zero, // Look-at Vector
				//	Vector3.UnitZ); // Up Axis is Z

				var x = ((float)posWorldtoScreenCoords.X / Main.GameViewMatrix.Zoom.X);
				var y = ((float)posWorldtoScreenCoords.Y / Main.GameViewMatrix.Zoom.Y);

				x += -Main.graphics.GraphicsDevice.Viewport.Width / Main.GameViewMatrix.Zoom.X / 2;
				y += -Main.graphics.GraphicsDevice.Viewport.Height / Main.GameViewMatrix.Zoom.Y / 2;

				_effect.View = Matrix.CreateTranslation(x, y, -1000);

				Main.NewText($"X: {x} - Y: {y}");

				if (ModifyInterfaceLayerFix)
				{
					x *= Main.GameZoomTarget;
					y *= Main.GameZoomTarget;
				}

				// Moves the object to the desired position
				// Multiplying the Matrices "adds" them together
				//
				// We translate the Y 3000 pixels towards us 
				// so we can see 3D models up to 3000 pixels large
				// We don't see the Y axis in Orthographic mode
				// so it doesn't matter how large we make it!
				//_effect.World = Matrix.CreateTranslation(0, 0, 0);

				// Produces the "flat" style
			}
			spriteBatch.End();

			_effect.Projection =
				Matrix.CreateOrthographic(
					Main.graphics.GraphicsDevice.Viewport.Width / Main.GameViewMatrix.Zoom.X,
					Main.graphics.GraphicsDevice.Viewport.Height / Main.GameViewMatrix.Zoom.X, -2000, 2000);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, _effect);
			{
				Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
				Main.graphics.GraphicsDevice.DepthStencilState = depthStencilState;
				Main.graphics.GraphicsDevice.BlendState = blendState;


				for (int i = 0; i < model.vertexBuffers.Count; i++)
				{
					_effect.Texture = model.materialList[i];

					// Lighting
					var x = (int)(pos.X) / 16;
					var y = (int)(pos.Y) / 16;
					Vector3 color = Lighting.GetColor(x, y).ToVector3();

					//_effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);

					_effect.DirectionalLight0.DiffuseColor = color;
					_effect.DirectionalLight1.DiffuseColor = color;
					_effect.DirectionalLight2.DiffuseColor = color;
					_effect.AmbientLightColor = color;
					_effect.DiffuseColor = color;
					_effect.EmissiveColor = color;

					_effect.VertexColorEnabled = true;
					_effect.TextureEnabled = true;
					_effect.LightingEnabled = false;
					_effect.Alpha = 1;

					foreach (var pass in _effect.CurrentTechnique.Passes)
					{
						pass.Apply();

						device.SetVertexBuffer(model.vertexBuffers[i]);

						spriteBatch.GraphicsDevice.DrawPrimitives(
									Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 
									0,
									model.vertexBuffers[i].VertexCount / 3);
					}
				}

				spriteBatch.End();
			}

			Main.graphics.GraphicsDevice.RasterizerState = previousRS;
			Main.graphics.GraphicsDevice.DepthStencilState = previousDS;
			Main.graphics.GraphicsDevice.BlendState = previousBS;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		}
	}
}
