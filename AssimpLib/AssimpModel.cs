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
		public List<Texture2D> textureList;
		public List<Texture2D> materialList;
		public List<VertexBuffer> vertexBuffers;
		public List<IndexBuffer> indexBuffers;
		public List<int> vertexCount;
		public List<int> faceCount;

		public BasicEffect effect;

		public Matrix viewMatrix;
		public Matrix projectionMatrix;


		public AssimpModel(string fileName, PostProcessSteps steps)
		{
			AssimpContext context = new AssimpContext();
			Scene scene = context.ImportFile(fileName, steps);
			LoadTexture(scene);
			LoadVertexBuffers(scene);
			LoadMaterials(scene);
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
	}
}
