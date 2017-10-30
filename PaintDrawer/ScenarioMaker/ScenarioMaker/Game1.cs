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

namespace ScenarioMaker
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const String FileExtension = ".map";
        public const String FileFilter = "Map files (*.map)|*.map|All files (*.*)|*.*";
        public const String FileLoadFilter = "Map files (*.map)|*.map|Map Autosave files (*.map_autosave)|*.map_autosave|All files (*.*)|*.*";
        public static Color BackColor = new Color(48, 48, 64), PrimaryColor = new Color(120, 120, 120), BlueColor = new Color(128, 128, 192), RedColor = new Color(192, 128, 128), GreenColor = new Color(128, 192, 128);
        public static Texture2D white, cross, circle;
        public static Vector2 halfCross, halfCircle;
        public static SpriteFont font;
        public static int ScreenWidth, ScreenHeight, HalfWidth, HalfHeight;
        public static Game1 game;
        public static float time, deltaTime;

        GraphicsDeviceManager graphics;
        public SpriteBatch batch;

        public MouseState ms, oldms;
        public KeyboardState ks, oldks;

        Scene scene;

        public Game1()
        {
            game = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            EventInput.Initialize(Window);
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 3 / 4;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 3 / 4;
            graphics.ApplyChanges();
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            IsFixedTimeStep = false;

            //white = Content.Load<Texture2D>("pixel");
            white = new Texture2D(GraphicsDevice, 1, 1);
            white.SetData<Color>(new Color[] { Color.White });
            font = Content.Load<SpriteFont>("font");
            cross = Content.Load<Texture2D>("cross");
            halfCross = new Vector2(cross.Width / 2, cross.Height / 2);
            circle = Content.Load<Texture2D>("circle");
            halfCircle = new Vector2(circle.Width / 2, circle.Height / 2);
            Stuff.Init();

            Scenes.MainMenu.Load(Content);
            Scenes.Editor.Load(Content);

            scene = new Scenes.MainMenu();
            //scene = new Scenes.Editor();

            Window_ClientSizeChanged(null, null);
        }

        protected override void UnloadContent()
        {
            white.Dispose();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            scene.OnExit();
            Stuff.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += deltaTime;
            oldks = ks;
            oldms = ms;
            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            scene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            scene.Draw(batch, GraphicsDevice);   

            base.Draw(gameTime);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if(Window.ClientBounds.Width != 0 && Window.ClientBounds.Height != 0)
            if (Window.ClientBounds.Width != ScreenWidth || Window.ClientBounds.Height != ScreenHeight)
            {
                ScreenWidth = Window.ClientBounds.Width;
                if (ScreenWidth == 0) ScreenWidth = 1;
                ScreenHeight = Window.ClientBounds.Height;
                if (ScreenHeight == 0) ScreenHeight = 1;
                HalfWidth = ScreenWidth / 2;
                HalfHeight = ScreenHeight / 2;
                graphics.PreferredBackBufferWidth = ScreenWidth;
                graphics.PreferredBackBufferHeight = ScreenHeight;
                //graphics.ApplyChanges();
                if (scene != null)
                    scene.OnResize();
            }
        }

        public void DrawXAt(Vector2 at, Color color)
        {
            batch.Draw(cross, at, null, color, 0f, halfCross, 1f, SpriteEffects.None, 0f);
        }

        public void DrawCircleAt(Vector2 at, Color color)
        {
            batch.Draw(circle, at, null, color, 0f, halfCircle, 1f, SpriteEffects.None, 0f);
        }

        public void DrawXAt(Vector2 at, Color color, float scale)
        {
            batch.Draw(cross, at, null, color, 0f, halfCross, scale, SpriteEffects.None, 0f);
        }

        public void DrawCircleAt(Vector2 at, Color color, float scale)
        {
            batch.Draw(circle, at, null, color, 0f, halfCircle, scale, SpriteEffects.None, 0f);
        }

        public void DrawThickLine(Vector2 from, Vector2 to, float thickness, Color color)
        {
            Vector2 pos = (from + to) / 2f, size = new Vector2(Vector2.Distance(from, to) + thickness, thickness);
            float rot = (float)Math.Atan2(from.Y - to.Y, from.X - to.X);
            batch.Draw(white, pos, null, color, rot, new Vector2(0.5f, 0.5f), size, SpriteEffects.None, 0f);
        }

        public void SetScene(Scene scene)
        {
            this.scene.OnExit();
            this.scene = scene;
            scene.OnResize();
        }

        public bool MouseLeftClicked()
        {
            return ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released;
        }

        public bool KeyJustClicked(Keys key)
        {
            return ks.IsKeyDown(key) && oldks.IsKeyUp(key);
        }
    }
}
