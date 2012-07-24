using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WickedSuffering
{
    class heightmap
    {
        Texture2D sandTexture;

        VertexBuffer myVertexBuffer;

        IndexBuffer myIndexBuffer;

        GraphicsDevice device;

        public float[,] heightData;

        VertexPositionNormalTexture[] vertices;
        
        int[] indices;
        
        Texture2D heightMap;

        Effect effect;

        Camera c;

        ContentManager content;

        public int terrainWidth;

        public int terrainHeight;


        public heightmap(GraphicsDevice device, ContentManager content, Camera c)
        {
            this.content = content;
            this.device = device;
            this.c = c;

        }

        public void loadContent()
        {
            heightMap = content.Load<Texture2D>("HeightMap/heightmap");
            effect = content.Load<Effect>("HeightMap/effects");
            sandTexture = content.Load<Texture2D>("HeightMap/sand-texture");
            LoadHeightData(heightMap);
            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
            CopyToBuffers();

        }

        

        public void Draw(GameTime time)
        {
            DrawEnvironment(time);   
        }   

        protected void DrawEnvironment(GameTime gameTime)
        {

            //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;
            Matrix worldMatrix = Matrix.CreateTranslation(-terrainWidth / 2.0f, 0, terrainHeight / 2.0f);// *Matrix.CreateRotationY(angle);

            //Sets the effects to be used from the fx file such as coloring the terrain and adding lighting.
            effect.CurrentTechnique = effect.Techniques["Textured"];
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();
            effect.Parameters["xTexture"].SetValue(sandTexture);
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.1f);
            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xView"].SetValue(c.View);
            effect.Parameters["xProjection"].SetValue(c.Projection);
            effect.Parameters["xWorld"].SetValue(worldMatrix);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.Indices = myIndexBuffer;
                device.SetVertexBuffer(myVertexBuffer);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }
        }
        
        private void SetUpVertices()
        {
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    if (heightData[x, y] < minHeight)
                        minHeight = heightData[x, y];
                    if (heightData[x, y] > maxHeight)
                        maxHeight = heightData[x, y];
                }
            }

            vertices = new VertexPositionNormalTexture[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);

                    vertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / 30.0f;
                    vertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / 30.0f;   
                }
            }

        }
       
        private void SetUpIndices()
        {
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = (int)(x + y * terrainWidth);
                    int lowerRight = (int)((x + 1) + y * terrainWidth);
                    int topLeft = (int)(x + (y + 1) * terrainWidth);
                    int topRight = (int)((x + 1) + (y + 1) * terrainWidth);

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }
       
        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = (int)heightMap.Width;
            terrainHeight = (int)heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);

            heightData = new float[terrainWidth, terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R / 5.0f;
        }

        private void CopyToBuffers()
        {
            myVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            myVertexBuffer.SetData(vertices);
            myIndexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            myIndexBuffer.SetData(indices);
        }

        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }
       
        private void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;

            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }


    }
}
