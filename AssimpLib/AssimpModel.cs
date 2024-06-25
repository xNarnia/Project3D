using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Assimp;

namespace Tiles3D.AssimpLib
{
	// Credits to Dradonhunter for this skeleton
	public class AssimpModel
	{
		private List<Texture2D> textureList;
		private List<Texture2D> materialList;
		private List<VertexBuffer> vertexBuffers;
		private List<IndexBuffer> indexBuffers;
		private List<int> vertexCount;
		private List<int> faceCount;

		private BasicEffect effect;

		private Matrix viewMatrix;
		private Matrix projectionMatrix;

		private RenderTarget2D renderTarget;

		public AssimpModel(string fileName, PostProcessSteps steps)
		{
			AssimpContext context = new AssimpContext();
			Scene scene = context.ImportFile(fileName, steps);
			LoadTexture(scene);
			LoadVertexBuffers(scene);
			LoadMaterials(scene);

			projectionMatrix = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0.001f, 100f);
			viewMatrix = Matrix.CreateLookAt(new Vector3(3.0f, 0.0f, 10.0f), Vector3.Zero, new Vector3(0.0f, 1.0f, 0.0f));

			Main.QueueMainThreadAction(() =>
			{
				renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

				//effect = shadetest3d.Instance.Assets.Request<Effect>("Assets/3DModel4", AssetRequestMode.ImmediateLoad).Value;

				effect = new BasicEffect(Main.graphics.GraphicsDevice);



				//effect.Parameters["WorldMatrix"].SetValue(Main.GameViewMatrix.TransformationMatrix); 
				//effect.Parameters["ViewMatrix"].SetValue(viewMatrix);
				//effect.Parameters["ProjectionMatrix"].SetValue(projectionMatrix);

			});
		}

		private void LoadTexture(Scene scene)
		{
			textureList = new List<Texture2D>(scene.TextureCount);

			for (int i = 0; i < scene.TextureCount; i++)
			{
				EmbeddedTexture texture = scene.Textures[i];
				Texture2D texture2D = new Texture2D(Main.instance.GraphicsDevice, texture.Width, texture.Height);
				Color[] textureColor = new Color[texture.NonCompressedDataSize];
				for (int j = 0; j < texture.NonCompressedDataSize; j++)
				{
					textureColor[j].R = texture.NonCompressedData[j].R;
					textureColor[j].G = texture.NonCompressedData[j].G;
					textureColor[j].B = texture.NonCompressedData[j].B;
					textureColor[j].A = texture.NonCompressedData[j].A;
				}
				texture2D.SetData(textureColor);
				textureList.Add(texture2D);
			}
		}

		private void LoadMaterials(Scene scene)
		{
			materialList = new List<Texture2D>(scene.MaterialCount);
			for (int i = 0; i < scene.MaterialCount; i++)
			{
				var i1 = i;
				Main.QueueMainThreadAction(() =>
				{
					Material material = scene.Materials[i1];

					if (material.GetMaterialTextureCount(TextureType.Diffuse) >= 1)
					{

						if (material.GetMaterialTexture(TextureType.Diffuse, 0, out var slot))
						{
							int idx = slot.FilePath.LastIndexOf("\\");
							string fileName = slot.FilePath.Substring(idx + 1);

							using var file = File.OpenRead(Path.Combine(ModLoader.ModPath, "Tiles3D", "Model", fileName));

							materialList.Add(Texture2D.FromStream(Main.graphics.GraphicsDevice, file));
						}
					}
				});
			}
		}

		private void LoadVertexBuffers(Scene scene)
		{
			vertexBuffers = new List<VertexBuffer>();
			indexBuffers = new List<IndexBuffer>();
			vertexCount = new List<int>();
			faceCount = new List<int>();


			Main.QueueMainThreadAction(() =>
			{
				for (int i = 0; i < scene.MeshCount; i++)
				{
					VertexBuffer vertexBuffer = new VertexBuffer(Main.instance.GraphicsDevice,
						typeof(VertexPositionColorTexture), scene.Meshes[i].VertexCount, BufferUsage.None);

					List<VertexPositionColorTexture> color = new List<VertexPositionColorTexture>();
					//for (int j = scene.Meshes[i].Vertices.Count - 1; j >= 0; j--)
					for (int j = 0; j < scene.Meshes[i].Vertices.Count; j++)
					{
						var vert = new VertexPositionColorTexture();
						vert.Position = new Vector3(scene.Meshes[i].Vertices[j].X,
							scene.Meshes[i].Vertices[j].Y,
							scene.Meshes[i].Vertices[j].Z);

						if (scene.Meshes[i].HasTextureCoords(0))
						{
							vert.TextureCoordinate = new Vector2(
								scene.Meshes[i].TextureCoordinateChannels[0][j].X,
								scene.Meshes[i].TextureCoordinateChannels[0][j].Y);
							vert.Color = Color.White;
						}
						color.Add(vert);
					}

					//List<int> indices = new List<int>();
					//int f = 0;
					//Face mFace;
					//for (int j = 0; j < scene.Meshes[i].FaceCount * 3; j += 3)
					//{
					//    mFace = scene.Meshes[i].Faces[f];
					//    f++;
					//    indices.Add(mFace.Indices[2]);
					//    indices.Add(mFace.Indices[1]);
					//    indices.Add(mFace.Indices[0]);

					//}

					IndexBuffer indexBuffer = new IndexBuffer(Main.instance.GraphicsDevice,
						typeof(int), scene.Meshes[i].GetIndices().Length, BufferUsage.None);
					indexBuffer.SetData(scene.Meshes[i].GetIndices());

					indexBuffers.Add(indexBuffer);
					vertexBuffer.SetData(color.ToArray());
					vertexBuffers.Add(vertexBuffer);
					vertexCount.Add(scene.Meshes[i].VertexCount);
					faceCount.Add(scene.Meshes[i].GetShortIndices().Length);
				}
			});
		}

		private bool save;

		private int saveTimer = 1;

		private float angle = 0.0f;
		private float deltaTime = 0.0f;
		private float lastTime;

		public void Update(GameTime gameTime)
		{
			float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
			deltaTime = now - lastTime;
			lastTime = now;

			angle += 0.1f * deltaTime;
			if (angle > 360.0f)
			{
				angle -= 360.0f;
			}
		}

		public void Draw()
		{
			GraphicsDevice device = Main.instance.GraphicsDevice;

			var oldBuffer = device.Indices;
			var oldGraphicsBinding = device.GetRenderTargets();
			var oldVertexBinding = device.GetVertexBuffers();

			var world = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0f));
			var view = Main.GameViewMatrix.TransformationMatrix;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.graphics.GraphicsDevice.Viewport.Width,
				Main.graphics.GraphicsDevice.Viewport.Height, 0, -1f, 1f);

			var transformation = world * view * projection;

			//effect.Parameters["WorldMatrix"].SetValue(Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0f)));
			//effect.Parameters["ViewMatrix"].SetValue(Main.GameViewMatrix.TransformationMatrix);
			//effect.Parameters["ProjectionMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, -1f, 1f));
			//effect.Parameters["transformationMatrix"].SetValue(transformation);

			// effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
			effect.View = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(180), 0, 0) * Matrix.CreateTranslation(new Vector3(1, 0, -5));
			effect.World = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(angle), 0, 0);
			effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);

			effect.VertexColorEnabled = true;
			effect.TextureEnabled = true;
			effect.LightingEnabled = false;
			effect.Alpha = 1;



			device.SetRenderTarget(renderTarget);


			var depth = new DepthStencilState()
			{
				DepthBufferEnable = true,
				DepthBufferFunction = CompareFunction.LessEqual
			};


			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, depth, RasterizerState.CullNone);

			device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
			for (int i = 0; i < vertexBuffers.Count; i++)
			{



				device.RasterizerState = RasterizerState.CullNone;
				effect.Texture = materialList[i];

				foreach (var currentTechniquePass in effect.CurrentTechnique.Passes)
				{
					currentTechniquePass.Apply();
					// device.Indices = null;
					device.SetVertexBuffer(vertexBuffers[i]);

					// device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, vertexCount[i], 0, faceCount[i]);
					device.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, vertexBuffers[i].VertexCount / 3);
				}
			}
			Main.spriteBatch.End();
			saveTimer--;

			if (saveTimer == 0)
			{
				renderTarget.SaveAsPng(File.OpenWrite(Path.Combine(ModLoader.ModPath, "Tiles3D", "Model", "antiGravity.png")), Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height);
				saveTimer = 300;
			}

			device.Indices = oldBuffer;
			device.SetRenderTargets(oldGraphicsBinding);
			device.SetVertexBuffers(oldVertexBinding);

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
			Main.spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
			Main.spriteBatch.End();
		}

		public void SetProperty<T>(object source, string propertyName, object value)
		{
			PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public);
			propertyInfo.SetMethod.Invoke(source, new[] { value });
		}
	}
}
