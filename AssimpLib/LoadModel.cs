using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Assimp;

namespace Tiles3D.AssimpLib
{
	// Credits to Dradonhunter for this skeleton
	public class LoadModel
	{
		private AssimpModel model;

		public void Load()
		{
			model = new AssimpModel(Path.Combine(ModLoader.ModPath, "Tiles3D", "Model", "eoc.fbx"),
				PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
			/*
            List<ModelBone> bones = new List<ModelBone>(scene.Meshes[0].BoneCount);
            for (int i = 0; i < bones.Count; i++)
            {
                ModelBone bone = CallConstructorObject<ModelBone>();
                SetProperty<ModelBone>(bone, "Index", i);
                bone.Transform = ConvertAssimpMatrix(scene.Meshes[0].Bones[i].OffsetMatrix);
                SetProperty<ModelBone>(bone, "Name", (scene.Meshes[0].Bones[i].Name));
                bones.Add(bone);
            }

            List<ModelMeshPart> meshes = new List<ModelMeshPart>(scene.Meshes[0].FaceCount);
            for (int i = 0; i < meshes.Count; i++)
            {

                ModelMeshPart part = CallConstructorObject<ModelMeshPart>();

                VertexPositionColor[] color = new VertexPositionColor[scene.Meshes[i].Vertices.Count];
                for (int j = 0; j < color.Length; j++)
                {
                    color[j] = new VertexPositionColor();
                    color[j].Position = new Vector3(scene.Meshes[i].Vertices[j].X, scene.Meshes[i].Vertices[j].Y,
                        scene.Meshes[i].Vertices[j].Z);
                    color[j].Color = new Color(scene.Meshes[i].VertexColorChannels[0][j].R,
                        scene.Meshes[i].VertexColorChannels[0][j].G, scene.Meshes[i].VertexColorChannels[0][j].B,
                        scene.Meshes[i].VertexColorChannels[0][j].A);
                }

                IndexBuffer indexBuffer = new IndexBuffer(Main.instance.GraphicsDevice, typeof(int),
                    scene.Meshes[i].GetIndices().Length, BufferUsage.None);
                indexBuffer.SetData(scene.Meshes[i].GetIndices());
                SetProperty<ModelMeshPart>(part, "IndexBuffer", indexBuffer);

                VertexBuffer vertexBuffer = new VertexBuffer(Main.instance.GraphicsDevice, typeof(VertexPositionColor),
                    color.Length, BufferUsage.None);
                vertexBuffer.SetData(color);

                SetProperty<ModelMeshPart>(part, "NumVertices", scene.Meshes[i].Vertices.Count);
                SetProperty<ModelMeshPart>(part, "StartIndex", scene.Meshes[i].MaterialIndex);

                if (scene.Meshes[i].PrimitiveType == PrimitiveType.Triangle)
                {
                    SetProperty<ModelMeshPart>(part, "PrimitiveCount", scene.Meshes[i].VertexCount / 3);
                }
                else
                {
                    throw new Exception(
                        "Provided model does not contain triangle, please provide a valid triangle model");
                }

                Effect effect = new BasicEffect(Main.instance.GraphicsDevice);
                SetProperty<ModelMeshPart>(part, "StartIndex", scene.Meshes[i].MaterialIndex);
            }

            List<Texture2D> textureList = new List<Texture2D>(scene.TextureCount);

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
            }*/
		}
	}
}
