using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WickedSuffering
{
    class TargetEngine
    {
        public float[,] heightdata;
        Model targetmodel;
        Camera c;
        ContentManager content;
        public List<target> targets;
        int terrainHeight;
        int terrainWidth;
        public List<BoundingBox> boundingBox;

        public TargetEngine(Camera c, ContentManager content)
        {
            this.c = c;
            this.content = content;
            targets = new List<target>();
           

        }

        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices, note works with rotating and moving targets ya sanad , aka world transform
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            
            return new BoundingBox(min, max);
        }
        private void generatePositions(){
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                int x = random.Next((-terrainWidth / 2) + 20, (terrainWidth / 2) -20);
                int z = random.Next((-terrainHeight / 2) + 20, (terrainHeight / 2) -20);
                Vector3 pos = new Vector3(x, heightdata[(terrainHeight / 2) + x, (terrainWidth / 2) - z], z);
                
                targets.Add(new target(this.c,this.content,"bot" + i, pos));

                //adding collisionbox to 
                boundingBox.Add(UpdateBoundingBox(targetmodel,Matrix.CreateTranslation(pos)));


            }
        }

        public void loadContent(int width, int height, float[,] heightdata)
        {
            targetmodel = content.Load<Model>("Models/nanosuit/nanosuit"); 
            this.terrainWidth = width;
            this.terrainHeight = height;
            this.heightdata = heightdata;
           
            boundingBox = new List<BoundingBox>();
             generatePositions();

            //effect = content.Load<Effect>("Heightmap/effects");
            //AK47.Meshes[0].MeshParts[0].Effect = effect;

        }



        public void updateTarget(GameTime gametime)
        {

            for (int i = 0; i < targets.Count; i++)
            {

                targets[i].Update(gametime);
            }
        }

        public void DrawTarget(GameTime gametime)
        {
            for (int i = 0; i < targets.Count; i++)
            {

                targets[i].DrawTarget(gametime, targetmodel);

            }

        }






    }
}
