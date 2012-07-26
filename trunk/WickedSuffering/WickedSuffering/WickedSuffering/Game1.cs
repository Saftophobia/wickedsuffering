using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WickedSuffering
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        heightmap Heightmap;

        playercam playercam;

        Texture2D crossHair;

        Camera c;

        Skydome sky;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        TargetEngine TargetEngine;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            c = new Camera(new Vector3(0, 100, 0), Vector3.Zero, this.GraphicsDevice);
            c.View = Matrix.CreateLookAt(new Vector3(250.0f,250.0f,250.0f),Vector3.Zero,Vector3.Up);
            Heightmap = new heightmap(this.GraphicsDevice,this.Content, c);
            playercam = new playercam(this.GraphicsDevice,c,this.Content);
            TargetEngine = new TargetEngine(c, this.Content);
            sky = new Skydome(this.GraphicsDevice, this.Content, c.View, c.Projection, c.Position);
            crossHair = Content.Load<Texture2D>("crosshair");
            
            base.Initialize();
        }

        

       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Heightmap.loadContent();
            TargetEngine.loadContent(Heightmap.terrainWidth, Heightmap.terrainHeight, Heightmap.heightData);
            playercam.loadcontent(Heightmap.heightData,Heightmap.terrainWidth,Heightmap.terrainHeight,TargetEngine.targets,TargetEngine.boundingBox);
            sky.LoadContent();
            
            
            // TODO: use this.Content to load your game content here
        }
        

        

        
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            TargetEngine.updateTarget(gameTime);
            playercam.update(gameTime);
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

           

            base.Update(gameTime);
        }

      
        protected override void Draw(GameTime gameTime)
        {
            var time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
            sky.GeneratePerlinNoise(time);

            

            GraphicsDevice.Clear(Color.CornflowerBlue);

            sky.DrawSkyDome(c);

            Heightmap.Draw(gameTime);


           
            TargetEngine.DrawTarget(gameTime);
            playercam.DrawAK47(gameTime);


            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            string pos = "Current Position: " + c.Position.ToString();
            
            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), pos, new Vector2(3, 3), Color.Black);
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), pos, new Vector2(2, 2), Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), fps, new Vector2(3, 63), Color.Black);
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), fps, new Vector2(2, 62), Color.White);
            spriteBatch.Draw(crossHair, new Vector2((GraphicsDevice.Viewport.Width / 2) - (crossHair.Width / 2), (GraphicsDevice.Viewport.Height / 2) - (crossHair.Height / 2)), Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
