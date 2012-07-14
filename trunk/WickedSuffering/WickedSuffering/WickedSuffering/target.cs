using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WickedSuffering
{
    class target
    {
        float rotationvariable = 0;

        string ID;

        Camera c;

        ContentManager content;


        Vector3 position;

        bool alive;

        public target(Camera c,ContentManager content, string ID,Vector3 position)
        {
            this.c = c;
            this.content = content;
            this.ID = ID;
            this.position = position;
            alive = true;

        }

        public void Update(GameTime gametime)
        {
            if(alive)
                rotationvariable += 0.05f;
        }








        public void DrawTarget(GameTime gametime, Model target)
        {
            


                // draw model infront of screen .. with no shaders needed
                Matrix[] transforms = new Matrix[target.Bones.Count];
                target.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in target.Meshes)
                {

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.LightingEnabled = true;




                        effect.World = Matrix.CreateScale(0.8f, 0.8f, 0.8f) * transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotationvariable) * Matrix.CreateTranslation(position);
                        effect.View = c.View;
                        effect.Projection = c.Projection;
                    }

                    mesh.Draw();


                

            }

        }





    }
}
