using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace GameLibrary
{
    public class TestGame : Game
    {
        GraphicsDeviceManager graphics;
        private VertexPositionColor[] _vertexPositionColors;
        private short[] _vertexIndices;
        private Model _sphere;
        private BasicEffect _basicEffect;
        private Matrix _checkerboardWorld = Matrix.Identity;
        private Vector3 _cameraTarget;
        private Vector3 _cameraPosition;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.DragComplete | GestureType.Pinch | GestureType.PinchComplete;
            TouchPanel.EnableMouseGestures = true;
            TouchPanel.EnableMouseTouchPoint = true;

            _cameraPosition = new Vector3(0, -50, 50);
            _cameraTarget = new Vector3(0, 0, 0);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            short squares = 10;
            float size = 10f;
            float z = 0f;
            float offset = squares * size / 2f;
            bool empty;
            var vertices = new List<VertexPositionColor>();
            var indices = new List<short>();
            for (short y = 0; y <= squares; y++)
            {
                empty = (y & 1) == 1;
                for (short x = 0; x <= squares; x++)
                {
                    vertices.Add(new VertexPositionColor(new Vector3(x * size - offset, y * size - offset, z), Color.MediumPurple));

                    if (!empty && x < squares && y < squares)
                    {
                        indices.Add((short)(x + y * (squares + 1)));
                        indices.Add((short)(x + (y + 1) * (squares + 1)));
                        indices.Add((short)(x + (y + 1) * (squares + 1) + 1));
                        indices.Add((short)(x + y * (squares + 1)));
                        indices.Add((short)(x + (y + 1) * (squares + 1) + 1));
                        indices.Add((short)(x + y * (squares + 1) + 1));
                    }

                    empty = !empty;
                }
            }

            _vertexPositionColors = vertices.ToArray();
            _vertexIndices = indices.ToArray();

            _sphere = Content.Load<Model>("sphere");

            _basicEffect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true
            };
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
#endif

            var keystate = Keyboard.GetState();
            if (keystate.IsKeyDown(Keys.W))
                _checkerboardWorld *= Matrix.CreateTranslation(0, 0, 0.25f);
            else if (keystate.IsKeyDown(Keys.S))
                _checkerboardWorld *= Matrix.CreateTranslation(0, 0, -0.25f);
            if (keystate.IsKeyDown(Keys.A))
                _checkerboardWorld *= Matrix.CreateTranslation(-0.25f, 0, 0);
            else if (keystate.IsKeyDown(Keys.D))
                _checkerboardWorld *= Matrix.CreateTranslation(0.25f, 0, 0);
            else if (keystate.IsKeyDown(Keys.Enter))
                _checkerboardWorld = Matrix.Identity;

            var mousestate = Mouse.GetState();

            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.Tap)
                {
                }
                else if (gesture.GestureType == GestureType.FreeDrag)
                {
                    var translateXY = new Vector3(-gesture.Delta.X, gesture.Delta.Y, 0) / 10;
                    _cameraTarget += translateXY;
                    _cameraPosition += translateXY;
                }
                else if (gesture.GestureType == GestureType.Pinch)
                {
                    var center = gesture.Position + (gesture.Position - gesture.Position2) / 2;

                    var oldSize = ((gesture.Position - gesture.Delta) - (gesture.Position2 - gesture.Delta2)).Length();
                    var newSize = (gesture.Position - gesture.Position2).Length();

                    var scale = oldSize / newSize;

                    if (scale > 0)
                    {
                        // scale at the origin
                        var cameraPosition = _cameraPosition - _cameraTarget;
                        cameraPosition *= scale;
                        _cameraPosition = cameraPosition + _cameraTarget;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            var view = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, Vector3.Up);
            _basicEffect.View = view;

            var projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60f),
                (float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                1f,
                10_000f);
            _basicEffect.Projection = projection;

            _basicEffect.World = _checkerboardWorld;

            _basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertexPositionColors, 0, _vertexPositionColors.Length, _vertexIndices, 0, _vertexIndices.Length / 3);

            var random = new Random(11);
            for (int i = 0; i < 100; i++)
            {
                var world = _checkerboardWorld * Matrix.CreateTranslation(random.Next(-50_000, 50_000) / 1_000f, random.Next(-50_000, 50_000) / 1_000f, random.Next(0, 20));
                DrawModel(_sphere, world, _basicEffect.View, _basicEffect.Projection);
            }

            base.Draw(gameTime);
        }

        protected virtual void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(1, 1, 1);
                    effect.GraphicsDevice.BlendState = BlendState.Opaque;
                }

                mesh.Draw();
            }
        }
    }
}
