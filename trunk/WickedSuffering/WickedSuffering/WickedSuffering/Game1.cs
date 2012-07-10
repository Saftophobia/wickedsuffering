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

        Camera c;

        Skydome sky;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Targets targets;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            c = new Camera(new Vector3(100.0f, 100.0f, 100.0f), Vector3.Zero, this.GraphicsDevice);
            c.View = Matrix.CreateLookAt(new Vector3(250.0f,250.0f,250.0f),Vector3.Zero,Vector3.Up);
            Heightmap = new heightmap(this.GraphicsDevice,this.Content, c);
            playercam = new playercam(this.GraphicsDevice,c,this.Content);
            targets = new Targets(c, this.Content);
            sky = new Skydome(this.GraphicsDevice, this.Content, c.View, c.Projection, c.Position);
            
            base.Initialize();
        }

        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Heightmap.loadContent();
            playercam.loadcontent(Heightmap.heightData,Heightmap.terrainWidth,Heightmap.terrainHeight);
            sky.LoadContent();
            targets.loadContent(Heightmap.terrainWidth, Heightmap.terrainHeight);
            
            // TODO: use this.Content to load your game content here
        }
        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
            sky.GeneratePerlinNoise(time);

            

            GraphicsDevice.Clear(Color.CornflowerBlue);

            sky.DrawSkyDome(c);

            Heightmap.Draw(gameTime);


            playercam.DrawAK47(gameTime);
            targets.DrawTarget(gameTime);

            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            string pos = "Current Position: " + c.Position.ToString();

            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), pos, new Vector2(3, 3), Color.Black);
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), pos, new Vector2(2, 2), Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), fps, new Vector2(3, 33), Color.Black);
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), fps, new Vector2(2, 32), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
