using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace WickedSuffering
{
    class playercam
    {
        Model BulletModel;
        List<Bullet> bulletList;
        
        SoundEffect soundeffect;
        Model AK47;
        float modelRotation = 0.0f;
        Vector3 modelPosition = new Vector3(0,100,0);

        public float[,] heightdata;

        int terrainwidth;

        int terrainheight;

        GraphicsDevice device;

        Camera Camera;

        MouseState OrigMouseState;

        ContentManager content;

        float movspeed = 30.0f;

        float rotspeed = 0.3f;

        float HorizonRot = MathHelper.PiOver2;

        float VerticalRot = -MathHelper.Pi / 10.0f;
        List<target> targets;

        bool crouch = false;

        bool jump = false;

        bool jumping = false;

        bool up = true;

        float down = 0.1f;
        //Effect effect;

        List<BoundingBox> boundingBox;


        public playercam(GraphicsDevice device, Camera c, ContentManager content)
        {
            this.Camera = c;
            this.device = device;
            bulletList = new List<Bullet>();
            this.content = content;

        }


        public void loadcontent(float[,] heightdata, int terrainlength, int terrainheight, List<target> targets,List<BoundingBox> BoundingBox)
        {
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            this.heightdata = heightdata;
            this.terrainheight = terrainheight;
            this.terrainwidth = terrainlength;
            OrigMouseState = Mouse.GetState();
            AK47 = content.Load<Model>("Models/AK/AK");
            this.targets = targets;
            BulletModel = content.Load<Model>("Models/bullet/Bullet");
            soundeffect = content.Load<SoundEffect>("Models/AK/GunAK47SingleShot");
            this.boundingBox = BoundingBox;
            //effect = content.Load<Effect>("Heightmap/effects");
            //AK47.Meshes[0].MeshParts[0].Effect = effect;

        }

        public void update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            MouseState mousestate = Mouse.GetState();

            if (mousestate.LeftButton ==  ButtonState.Pressed)
            {
                Shoot();
                VerticalRot += 0.5f * timeDifference;
            }


            for (int i = 0; i < bulletList.Count();i++ )
            {
                if (!bulletList[i].dead)
                {
                    bulletList[i].Update(gameTime);
                }
                else
                {
                    bulletList.Remove(bulletList[i]);
                }

            }
                
                  
                
            

          



            //-----------








            if (mousestate != OrigMouseState)
            {
                float xDifference = mousestate.X - OrigMouseState.X;
                float yDifference = mousestate.Y - OrigMouseState.Y;

                HorizonRot -= rotspeed * xDifference * timeDifference;
                VerticalRot -= rotspeed * yDifference * timeDifference;

                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);

                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -2);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 2);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(2, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-2, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 2, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -2, 0);
            
            AddToCameraPosition(moveVector * timeDifference);
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(VerticalRot) * Matrix.CreateRotationY(HorizonRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Camera.target = Camera.Position + cameraRotatedTarget;
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);


            Camera.View = Matrix.CreateLookAt(Camera.Position, Camera.target, cameraRotatedUpVector);
        }

        //checking collision on every target
        public bool checkCollision(BoundingSphere sphere)
        {
            foreach (BoundingBox bsphere in boundingBox)
            {

                if (bsphere.Contains(sphere) == ContainmentType.Intersects)
                {
                    return true;
                }
            }
            return false;
        }



        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(VerticalRot) * Matrix.CreateRotationY(HorizonRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            Vector3 pos = Camera.Position;
            pos += movspeed * rotatedVector;
            
            if (pos.X > -349 && pos.Z > -349 && pos.X < 349 && pos.Z < 349);
            {
                //for collision check
                if(!checkCollision(new BoundingSphere(pos,2f)))
                    Camera.Position = pos;
            }

             KeyboardState keyState = Keyboard.GetState();
            
             if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.LeftShift))
             {
                 if (!jump)
                 {
                     crouch = true;
                     if (Camera.Position.Y > heightdata[(terrainheight / 2) + (int)Camera.Position.X, (terrainwidth / 2) - (int)Camera.Position.Z] + 12)
                     {
                         Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y - 1, Camera.Position.Z);
                     }
                     else
                     {
                         Camera.Position = new Vector3(Camera.Position.X, heightdata[(terrainheight / 2) + (int)Camera.Position.X, (terrainwidth / 2) - (int)Camera.Position.Z] + 10, Camera.Position.Z);
                     }
                 }
             }
             else
             {
                 if (Camera.Position.Y < heightdata[(terrainheight / 2) + (int)Camera.Position.X, (terrainwidth / 2) - (int)Camera.Position.Z] + 18)
                 {
                     Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y + 1, Camera.Position.Z);
                 }
                 else
                 {
                     crouch = false;

                 }
             }

             if (keyState.IsKeyDown(Keys.Space) && !crouch)
             { jumping = true; }

                 if (!crouch && jumping)
                 {
                     jump = true;
                     if ((Camera.Position.Y < heightdata[(terrainheight / 2) + (int)Camera.Position.X, (terrainwidth / 2) - (int)Camera.Position.Z] + 40) && up)
                     {
                         Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y + 1, Camera.Position.Z);
                     }
                     else
                     {
                         if (Camera.Position.Y > heightdata[(terrainheight / 2) + (int)Camera.Position.X, (terrainwidth / 2) - (int)Camera.Position.Z] + 20)
                         {
                             up = false;
                             Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y - down, Camera.Position.Z);
                             down += 0.1f;
                         }

                         else
                         {
                             jump = false;
                             jumping = false;
                             up = true;
                             down = 0.2f;
                         }
                     }
                 }
             

             if (!crouch && !jump)
             {
                 // Y coordinates is set to 20 above the heightdata altitude, remove this line to wander in space again.
                 Camera.Position = new Vector3(Camera.Position.X, heightdata[(terrainheight / 2) + (int)Camera.Position.X, (terrainwidth / 2) - (int)Camera.Position.Z] + 20, Camera.Position.Z);
             }

            UpdateViewMatrix();
        }

        public void DrawAK47(GameTime gametime)
        {

            // draw model infront of screen .. with no shaders needed
            Matrix[] transforms = new Matrix[AK47.Bones.Count];
            AK47.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix cameraRotation = Matrix.CreateRotationX(VerticalRot) * Matrix.CreateRotationY(HorizonRot);

            foreach (ModelMesh mesh in AK47.Meshes)
            {

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    

                    effect.World = Matrix.CreateScale(0.2f, 0.2f, 0.2f) * transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationX(VerticalRot) * Matrix.CreateRotationY(HorizonRot)
                        * Matrix.CreateTranslation(Camera.Position);
                    
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                }

                mesh.Draw();

                

            }


            for (int i = 0; i < bulletList.Count();i++ )
            {
                if (!bulletList[i].dead)
                {
                    bulletList[i].Draw(gametime);
                }

            }
            

            //-----------


        }

        public void Shoot()
        {
            SoundEffectInstance soundEffectInstance = soundeffect.CreateInstance();
            soundEffectInstance.IsLooped = false;
            soundEffectInstance.Play();
            /*
            Vector3 pointS = c.Position;
            Vector3 dirV = c.Position - c.target;

            for (int i = 0; i < targets.Count(); i++)
            {

                Vector3 pointQ = new Vector3(targets[i].position.X,targets[i].position.Y + 10,targets[i].position.Z);
                
                float F = ((pointQ - pointS).Length()) * ((pointQ - pointS).Length());
                float R = (Vector3.Dot((pointQ - pointS),dirV)) * (Vector3.Dot((pointQ - pointS),dirV));
                float M = dirV.Length() * dirV.Length();

                float Distance = (F - (R / M)) * (F - (R / M));

                if (Distance < 10)
                {
                    targets[i].alive = false;
                }
            }
             */
            Bullet bullet = new Bullet(BulletModel,this.Camera);
            bulletList.Add(bullet);
        }
    }
}
