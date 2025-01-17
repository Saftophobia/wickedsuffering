﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WickedSuffering
{
    class Bullet
    {
        public bool dead;
        Vector3 position;
        Vector3 speed;
        Model model;
        private int lifeTime;
        private int elapsedTime;
        Camera c;
        Vector3 targetvector;
        Vector3 positionvector;


        public Bullet(Model model,Camera C)
        {
            this.model = model;
            this.c = C;
            dead = false;
            lifeTime = 10000;
            elapsedTime = 0;
            position = c.Position;
            positionvector = c.Position;
            targetvector = c.target;
        }



        public void Update(GameTime gameTime)
        {
            
            this.elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            this.dead = (this.elapsedTime >= this.lifeTime);
            position.X += (targetvector.X - positionvector.X) * 0.5f;
            position.Y += (targetvector.Y - positionvector.Y) * 0.5f;
            position.Z += (targetvector.Z - positionvector.Z) * 0.5f;
            
            
            
        }

        public void Draw(GameTime gametime)
        {



            // draw model infront of screen .. with no shaders needed
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.LightingEnabled = true;
                    effect.World = Matrix.CreateScale(0.08f, 0.08f, 0.08f) * transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(position);
                    effect.View = c.View;
                    effect.Projection = c.Projection;
                }

                mesh.Draw();




            }

        }


       
    }
}
