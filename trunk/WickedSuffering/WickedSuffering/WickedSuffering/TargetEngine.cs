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

        public TargetEngine(Camera c, ContentManager content)
        {
            this.c = c;
            this.content = content;
            targets = new List<target>();
           

        }

        private void generatePositions(){
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                int x = random.Next((-terrainWidth / 2) + 20, (terrainWidth / 2) -20);
                int z = random.Next((-terrainHeight / 2) + 20, (terrainHeight / 2) -20);
                Vector3 pos = new Vector3(x, heightdata[(terrainHeight / 2) + x, (terrainWidth / 2) - z], z);
                
                targets.Add(new target(this.c,this.content,"bot" + i, pos));
            }
        }

        public void loadContent(int width, int height, float[,] heightdata)
        {
            targetmodel = content.Load<Model>("Models/nanosuit/nanosuit"); 
            this.terrainWidth = width;
            this.terrainHeight = height;
            this.heightdata = heightdata;
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
