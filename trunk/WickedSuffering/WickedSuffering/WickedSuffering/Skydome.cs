using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WickedSuffering
{
    class Skydome
    {
        Model skyDome;
        GraphicsDevice device;
        Effect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        RenderTarget2D cloudsRenderTarget;
        Texture2D cloudStaticMap;
        VertexPositionTexture[] fullScreenVertices;
        VertexDeclaration fullScreenVertexDeclaration;
        Texture2D cloudMap;
        Vector3 cameraPosition;
        ContentManager Content;

        public Skydome(GraphicsDevice d, ContentManager cont,Matrix view, Matrix proj, Vector3 cam)
        {
            device = d;
            viewMatrix = view;
            projectionMatrix = proj;
            cameraPosition = cam;
            Content = cont;
        }

        public void LoadContent()
        {
            fullScreenVertices = SetUpFullscreenVertices();

            effect = Content.Load<Effect>("SkyDome/Effects");
            skyDome = Content.Load<Model>("SkyDome/dome");
            cloudMap = Content.Load<Texture2D>("SkyDome/cloudMap");
            skyDome.Meshes[0].MeshParts[0].Effect = effect;

            PresentationParameters presentationParameters = device.PresentationParameters;
            cloudsRenderTarget = new RenderTarget2D(device, presentationParameters.BackBufferWidth,
                presentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            cloudStaticMap = CreateStaticMap(32);

        }


        public void DrawSkyDome(Camera camera)
        {
            var depthStencilState = new DepthStencilState();
            depthStencilState.DepthBufferWriteEnable = false;
            device.DepthStencilState = depthStencilState;

            Matrix[] modelTransforms = new Matrix[skyDome.Bones.Count];
            skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            var wMatrix = Matrix.CreateTranslation(0, -0.3f, 0) *
                Matrix.CreateScale(100) * Matrix.CreateTranslation(camera.Position);
            foreach (ModelMesh mesh in skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    var worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["SkyDome"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(camera.Projection);
                    currentEffect.Parameters["xTexture"].SetValue(cloudMap);
                    currentEffect.Parameters["xEnableLighting"].SetValue(false);
                }
                mesh.Draw();
            }
            depthStencilState = new DepthStencilState();
            depthStencilState.DepthBufferWriteEnable = true;
            device.DepthStencilState = depthStencilState;
        }



        private Texture2D CreateStaticMap(int resolution)
        {
            var rand = new Random();
            Color[] noisyColors = new Color[(resolution * resolution)];

            for (var x = 0; x < resolution; x++)
            {
                for (var y = 0; y < resolution; y++)
                {
                    noisyColors[x + y * resolution] = new Color(new Vector3((float)rand.Next(1000) / 1000.0f, 0, 0));
                }
            }
            var noiseImage = new Texture2D(device, 32, 32, false, SurfaceFormat.Color);
            noiseImage.SetData(noisyColors);
            return noiseImage;
        }

        private VertexPositionTexture[] SetUpFullscreenVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];
            vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0f), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0f), new Vector2(1, 1));
            vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0f), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0f), new Vector2(1, 0));

            return vertices;
        }




        public void GeneratePerlinNoise(float time)
        {

            device.SetRenderTarget(cloudsRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            effect.CurrentTechnique = effect.Techniques["PerlinNoise"];
            effect.Parameters["xTexture"].SetValue(cloudStaticMap);
            effect.Parameters["xOvercast"].SetValue(1.1f);
            effect.Parameters["xTime"].SetValue(time / 1000.0f);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, fullScreenVertices, 0, 2);


            }

            device.SetRenderTarget(null);
            cloudMap = cloudsRenderTarget;
        }



    }
}
