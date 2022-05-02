using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ModelViewer {
    // This project was made from the following Gamefromscratch's tutorial:
    // https://www.youtube.com/watch?v=OWrBLS7HO0A
    public class ModelViewer : Game {
        private GraphicsDeviceManager _graphics;

        private Vector3 camTarget;
        private Vector3 camPosition;
        private Matrix projectionMatrix;
        private Matrix viewMatrix;
        private Matrix worldMatrix; // May not be needed.

        private BasicEffect basicEffect;

        // Primitive test geometry
        VertexPositionColor[] triangleVertices;
        VertexBuffer vertexBuffer;

        // Orbit
        bool isOrbit = false;
        bool isSpacePressed = false;

        // Back face culling
        RasterizerState rasterizerState = new() {
            CullMode = CullMode.CullCounterClockwiseFace };
        bool isKeyFPressed;

        public ModelViewer() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            base.Initialize();

            // Setup Camera
            camTarget = Vector3.Zero;
            camPosition = new Vector3(0, 0, -1000);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(70),
                GraphicsDevice.DisplayMode.AspectRatio,
                10, 10000);

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            // BasicEffect
            basicEffect = new(GraphicsDevice) {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,

                Projection = projectionMatrix,
                World = worldMatrix
        };

            // Create triangle
            triangleVertices = new VertexPositionColor[3] {
                new(new Vector3(200, 50, -50), Color.Red),
                new(new Vector3(-50, 200, 50), Color.Green),
                new(new Vector3(50, -50, 200), Color.Blue)
            };

            vertexBuffer = new(
                GraphicsDevice,
                typeof(VertexPositionColor),
                3,
                BufferUsage.WriteOnly);

            vertexBuffer.SetData<VertexPositionColor>(triangleVertices);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.RasterizerState = rasterizerState;
        }

        protected override void LoadContent() {
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.D)) {
                camPosition.X -= 10;
                camTarget.X -= 10;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.A)) {
                camPosition.X += 10;
                camTarget.X += 10;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.W)) {
                camPosition.Y += 10;
                camTarget.Y += 10;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.S)) {
                camPosition.Y -= 10;
                camTarget.Y -= 10;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                camPosition.Z -= 25;

            if(Keyboard.GetState().IsKeyDown(Keys.Tab))
                camPosition.Z += 25;

            if(Keyboard.GetState().IsKeyDown(Keys.Space)) {
                if(!isSpacePressed)
                    isOrbit = !isOrbit;
                
                isSpacePressed = true;
            } else
                isSpacePressed = false;

            if(isOrbit)
                camPosition = Vector3.Transform(
                    camPosition,
                    Matrix.CreateRotationY(MathHelper.ToRadians(1)));

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            if(Keyboard.GetState().IsKeyDown(Keys.F)) {
                if(!isKeyFPressed) {
                    RasterizerState newRasterizerState = new();

                    if(GraphicsDevice.RasterizerState.CullMode == CullMode.CullCounterClockwiseFace)
                        newRasterizerState.CullMode = CullMode.None;
                    else newRasterizerState.CullMode = GraphicsDevice.RasterizerState.CullMode + 1;

                    GraphicsDevice.RasterizerState = newRasterizerState;
                }

                isKeyFPressed = true;
            } else
                isKeyFPressed = false;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            basicEffect.View = viewMatrix;
            GraphicsDevice.Clear(Color.DimGray);

            foreach(EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
            }

            base.Draw(gameTime);
        }
    }
}
