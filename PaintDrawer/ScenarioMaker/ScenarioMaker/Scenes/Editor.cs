using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScenarioMaker.Menus;
using ScenarioMaker.Tools;
using System.Threading;
using System.IO;

namespace ScenarioMaker.Scenes
{
    class Editor : Scene
    {
        public const float VertexSaveScale = 100f;
        public static float ClampDistance = 20, MinScale = 0.1f, MaxScale = 100f;
        public static Color vertexColor = Color.Red;
        static Texture2D LEL;
        public static void Load(ContentManager con)
        {

        }

        public String FileLocation;
        Vector2 menuSize, brTextPos;
        Matrix menuOffset;

        public VerticalToolMenu verticalMenu;
        Vector2 verticalMenuSize;
        Matrix verticalMenuOffset;
        Color verticalColor;

        ToolsMenu toolsMenu;
        Tool currentTool;

        Menu menu;

        Rect bounds;
        public List<Shape> shapes;
        public int BoundsWidth { get { return (int)bounds.Width; } }
        public int BoundsHeight { get { return (int)bounds.Height; } }

        public Matrix view, cameraView;
        public BasicEffect effect;
        VertexBuffer gridBuffer;

        Vector2 cameraCenter;
        public float oneOverScale;
        float camScale;
        public float CameraScale
        {
            get { return camScale; }
            set
            {
                camScale = value;
                if (CameraScale < MinScale) CameraScale = MinScale;
                else if (CameraScale > MaxScale) CameraScale = MaxScale;
                RecalcCameraMatrix();
            }
        }

        int gridSizeSeparator = 10;
        public int GridSize
        {
            get { return gridSizeSeparator; }
            set
            {
                gridSizeSeparator = value;
                MakeGridVertex();
            }
        }
        int gridPrimitiveCount;

        public Vector2 MousePos, OldMousePos;
        public Color mouseColor;
        public String MouseText
        {
            get { return mtext; }
            set
            {
                mtext = value;
            }
        }
        public String ExtraText
        {
            get { return etext; }
            set
            {
                etext = value;
                extraTextOrigin = new Vector2(0, Game1.font.MeasureString(etext).Y);
            }
        }
        Vector2 mtextOrigin, extraTextOrigin, extraTextPos;
        String mtext, etext;

        public Vector2 powerupPos;

        public String ScenarioName;

        public Vector2[] playersPos;
        public bool[] playersEnabled;
        public int PlayerCount
        {
            get
            {
                int c = 0;
                for (int i = 0; i < playersEnabled.Length; i++)
                    if (playersEnabled[i]) c++;
                return c;
            }
        }

        public byte BackgroundType;

        public bool MouseOnEditor() { return Game1.game.ms.X < verticalMenuOffset.Translation.X && Game1.game.ms.Y < menuOffset.Translation.Y; }
        Thread AutoSaveThread;

        public Editor()
        {
            Init();
            bounds = new Rect(0, 0, 0, 0);
            SetBoundsSize(500, 500);
            powerupPos = new Vector2(bounds.Width / 2f, bounds.Height / 2f);
            FileLocation = "";
            BackgroundType = 1;
            ScenarioName = Stuff.RandomName();

            AutoSaveThread = new Thread(SaveThread);
            AutoSaveThread.Start();
        }

        public Editor(byte[] file, String location)
        {
            this.FileLocation = location;
            Init();

            int index = 2; //the first two bytes are file version. For us, that means always 0 and 1. (0, 0) doesnt include name
            byte v1 = file[0], v2 = file[1];

            if (v2 == 0)
                ScenarioName = Stuff.RandomName();
            else
            {
                char[] chars = new char[file[index++]];
                for (int i = 0; i < chars.Length; i++)
                    chars[i] = (char)file[index++];
                ScenarioName = new String(chars);
            }

            BackgroundType = file[index++];

            bounds = new Rect(0, 0, 0, 0);
            SetBoundsSize(ReadFloat(file, ref index), ReadFloat(file, ref index));


            powerupPos = new Vector2(ReadFloat(file, ref index), ReadFloat(file, ref index));

            byte playerAmount = file[index++];
            for (int i = 0; i < playerAmount; i++)
            {
                playersEnabled[i] = true;
                playersPos[i] = new Vector2(ReadFloat(file, ref index), ReadFloat(file, ref index));
            }

            while (index < file.Length)
            {
                byte type = file[index++];
                if (type == Shape.TypeLinegroup)
                {
                    int len = ReadInt(file, ref index);
                    Vector2[] vert = new Vector2[len];
                    for (int i = 0; i < vert.Length; i++)
                        vert[i] = new Vector2(ReadFloat(file, ref index), ReadFloat(file, ref index));
                    AddShape(new Shape(vert));
                }
                else if (type == Shape.TypePoly)
                {
                    int len = ReadInt(file, ref index);
                    Vector2[] vert = new Vector2[len];
                    for (int i = 0; i < vert.Length; i++)
                        vert[i] = new Vector2(ReadFloat(file, ref index), ReadFloat(file, ref index));
                    AddShape(new Polygon(vert));
                }
                else if (type == Shape.TypeRect)
                {
                    AddShape(new Rect(ReadFloat(file, ref index), ReadFloat(file, ref index), ReadFloat(file, ref index), ReadFloat(file, ref index)));
                }
            }



            AutoSaveThread = new Thread(SaveThread);
            AutoSaveThread.Start();
        }

        private void Init()
        {
            FileStream strm = FindImage();
            if (strm == null)
            {
                Game1.game.Exit();
                return;
            }
            LEL = Texture2D.FromStream(Game1.game.GraphicsDevice, strm);
            Stuff.editor = this;
            shapes = new List<Shape>(200);
            menuSize.Y = 150;
            verticalMenuSize.X = 100;

            toolsMenu = new ToolsMenu(this);
            menu = toolsMenu;

            verticalMenu = new VerticalToolMenu(this);
            verticalColor = Color.Multiply(Game1.PrimaryColor, 0.75f);
            verticalColor.A = 255;

            effect = new BasicEffect(Game1.game.GraphicsDevice);
            effect.VertexColorEnabled = true;

            mouseColor = Color.Green;
            MouseText = "";
            mtextOrigin.Y = 100;
            mtextOrigin.X = -50;

            playersPos = new Vector2[8];
            playersEnabled = new bool[8];
        }

        private FileStream FindImage()
        {
            String[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".png") || files[i].EndsWith(".jpg") || files[i].EndsWith(".jpeg"))
                    return new FileStream(files[i], FileMode.Open);
            }
            return null;
        }

        public override void Update()
        {
            oneOverScale = 1f / CameraScale;
            #region MouseControl
            OldMousePos = MousePos;
            MousePos.X = Game1.game.ms.X - Game1.HalfWidth;
            MousePos.Y = Game1.game.ms.Y - Game1.HalfHeight;
            MousePos *= oneOverScale;
            MousePos += cameraCenter;
            bool clampM = true;
            Vector2 cTo = Vector2.Zero;
            if (verticalMenu.ClampVertex)
            {
                #region VertexClamp
                float dist = ClampDistance * oneOverScale;
                dist *= dist;
                float toDist = ClampDistance;
                for (int i = 0; i < shapes.Count; i++)
                    for (int c = 0; c < shapes[i].lines.Length; c++)
                    {
                        Line l = shapes[i].lines[c];
                        float d = Vector2.DistanceSquared(l.from, MousePos);
                        if (d < toDist)
                        {
                            cTo = l.from;
                            toDist = d;
                            clampM = false;
                        }

                        d = Vector2.DistanceSquared(l.to, MousePos);
                        if (d < toDist)
                        {
                            cTo = l.to;
                            toDist = d;
                            clampM = false;
                        }
                    }

                for (int c = 0; c < bounds.lines.Length; c++)
                {
                    Line l = bounds.lines[c];
                    float d = Vector2.DistanceSquared(l.from, MousePos);
                    if (d < toDist)
                    {
                        cTo = l.from;
                        toDist = d;
                        clampM = false;
                    }

                    d = Vector2.DistanceSquared(l.to, MousePos);
                    if (d < toDist)
                    {
                        cTo = l.to;
                        toDist = d;
                        clampM = false;
                    }
                }

                #endregion
            }
            if (clampM)
            {
                if (verticalMenu.ClampMouse)
                {
                    int hg = gridSizeSeparator / 2;
                    int val = (int)MousePos.X;
                    val += MousePos.X < 0 ? -hg : hg;
                    MousePos.X = (val) / gridSizeSeparator * gridSizeSeparator;

                    val = (int)MousePos.Y;
                    val += MousePos.Y < 0 ? -hg : hg;
                    MousePos.Y = (val) / gridSizeSeparator * gridSizeSeparator;
                }
            }
            else
                MousePos = cTo;
            #endregion

            menu.Update();

            #region ClickRegister
            if (Game1.game.MouseLeftClicked())
            {
                Vector2 mp = new Vector2(Game1.game.ms.X, Game1.game.ms.Y);
                if (mp.Y > menuOffset.Translation.Y)
                {
                    mp.Y -= menuOffset.Translation.Y;
                    menu.OnClick(mp);
                }
                else if (mp.X > verticalMenuOffset.Translation.X)
                {
                    mp.X -= verticalMenuOffset.Translation.X;
                    verticalMenu.OnClick(mp);
                }
                else if (currentTool != null)
                {
                    currentTool.OnClick(MousePos);
                }
            }
            #endregion

            if (currentTool != null)
            {
                if (Game1.game.KeyJustClicked(Keys.Escape))
                    SetTool(null);
                else
                    currentTool.Update();
            }

            #region CamControls
            if (Game1.game.ms.MiddleButton == ButtonState.Pressed)
            {
                int delt = Game1.game.ms.X - Game1.game.oldms.X;
                cameraCenter.X -= delt * (1f / CameraScale);

                delt = Game1.game.ms.Y - Game1.game.oldms.Y;
                cameraCenter.Y -= delt * (1f / CameraScale);
                RecalcCameraMatrix();

                int mx = Game1.game.ms.X;
                if (mx <= 0) mx += Game1.ScreenWidth;
                else if (mx >= Game1.ScreenWidth-1) mx -= Game1.ScreenWidth;
                int my = Game1.game.ms.Y;
                if (my <= 0) my += Game1.ScreenHeight;
                else if (my >= Game1.ScreenHeight-1) my -= Game1.ScreenHeight;

                if (mx != Game1.game.ms.X || my != Game1.game.ms.Y)
                {
                    Mouse.SetPosition(mx, my);
                    Game1.game.ms = Mouse.GetState();
                }
            }

            if (Game1.game.KeyJustClicked(Keys.Home))
                CenterCamera();

            if (Game1.game.ms.ScrollWheelValue != Game1.game.oldms.ScrollWheelValue)
            {
                CameraScale += (Game1.game.ms.ScrollWheelValue - Game1.game.oldms.ScrollWheelValue) / 600f;
            }
            #endregion
        }

        void CenterCamera()
        {
            cameraCenter.X = bounds.Width / 2;
            cameraCenter.Y = bounds.Height / 2;
            CameraScale = Math.Min(Game1.ScreenWidth / bounds.Width, Game1.ScreenHeight / bounds.Height);
            CameraScale *= 0.9f;
            RecalcCameraMatrix();
        }

        void RecalcCameraMatrix()
        {
            cameraView = Matrix.CreateTranslation(-cameraCenter.X, -cameraCenter.Y, 0);
            cameraView *= Matrix.CreateScale(CameraScale);
            cameraView *= Matrix.CreateTranslation(Game1.HalfWidth, Game1.HalfHeight, 0);
            //effect.View = view * cameraView * Matrix.CreateTranslation(0, cameraView.Translation.Y * -2, 0);
            view = Matrix.CreateScale(1, -1, 1) * Matrix.CreateTranslation(-cameraCenter.X, cameraCenter.Y, 0) * Matrix.CreateScale(CameraScale, CameraScale, 1);
            effect.View = view;
            oneOverScale = 1f / CameraScale;
            Stuff.SetView(view);
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Game1.BackColor);

            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, cameraView);
            batch.Draw(LEL, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 0.338541f, SpriteEffects.None, 0f);
            batch.End();

            effect.CurrentTechnique.Passes[0].Apply();

            if (verticalMenu.ShowGrid)
            {
                device.SetVertexBuffer(gridBuffer);
                device.DrawPrimitives(PrimitiveType.LineList, 0, gridPrimitiveCount);

            }

            bounds.Draw(device);
            for (int i = 0; i < shapes.Count; i++)
                shapes[i].Draw(device);

            if (currentTool != null)
                currentTool.Draw(batch, device);

            for (int i = 0; i < playersEnabled.Length; i++)
                if (playersEnabled[i])
                    Stuff.DrawPlayerIndicator(playersPos[i], Game1.GreenColor, i);
            Stuff.DrawPowerupIndicator(powerupPos, Game1.GreenColor);

            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, cameraView);
            if (verticalMenu.ShowGuides)
            {
                Game1.game.DrawXAt(Vector2.Zero, Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(bounds.Width, bounds.Height), Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(0, bounds.Height), Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(bounds.Width, 0), Color.Red, oneOverScale);
                float hw = bounds.Width / 2, hh = bounds.Height / 2;
                Game1.game.DrawXAt(new Vector2(hw, 0), Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(0, hh), Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(bounds.Width, hh), Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(hw, bounds.Height), Color.Red, oneOverScale);
                Game1.game.DrawXAt(new Vector2(hw, hh), Color.Red, oneOverScale);
            }
            Game1.game.DrawXAt(MousePos, mouseColor, oneOverScale);
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, verticalMenuOffset);
            batch.Draw(Game1.white, Vector2.Zero, null, verticalColor, 0f, Vector2.Zero, verticalMenuSize, SpriteEffects.None, 0f);
            verticalMenu.Draw(batch, new Vector2(Game1.game.ms.X - verticalMenuOffset.Translation.X, Game1.game.ms.Y));
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, menuOffset);
            batch.Draw(Game1.white, Vector2.Zero, null, Game1.PrimaryColor, 0f, Vector2.Zero, menuSize, SpriteEffects.None, 0f);
            menu.Draw(batch, new Vector2(Game1.game.ms.X, Game1.game.ms.Y - menuOffset.Translation.Y));
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            batch.DrawString(Game1.font, "Z=" + CameraScale, Vector2.Zero, Color.Black, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);

            String m = MousePos.ToString();
            batch.DrawString(Game1.font, m, brTextPos, Color.Black, 0f, Game1.font.MeasureString(m), 0.25f, SpriteEffects.None, 0f);
            batch.DrawString(Game1.font, MouseText, new Vector2(Game1.game.ms.X, Game1.game.ms.Y), Color.Black, 0f, mtextOrigin, 0.15f, SpriteEffects.None, 0f);

            batch.DrawString(Game1.font, etext, extraTextPos, Color.Black, 0f, extraTextOrigin, 0.2f, SpriteEffects.None, 0f);
            batch.End();
        }

        public override void OnResize()
        {
            menuSize.X = Game1.ScreenWidth;
            verticalMenuSize.Y = Game1.ScreenHeight;
            verticalMenuOffset = Matrix.CreateTranslation(Game1.ScreenWidth - verticalMenuSize.X, 0, 0);
            verticalMenu.OnResize();
            menuOffset = Matrix.CreateTranslation(0, Game1.ScreenHeight - menuSize.Y, 0);
            menu.OnResize();

            float hw = Game1.HalfWidth, hh = Game1.HalfHeight;
            view = Matrix.CreateLookAt(new Vector3(hw, hh, 1), new Vector3(hw, hh, 0), new Vector3(0, 1, 0)) * Matrix.CreateScale(1, -1, 1);
            effect.World = Matrix.CreateTranslation(0, 0, 0);
            //effect.View = view;
            Matrix proj = Matrix.CreateOrthographic(Game1.ScreenWidth, Game1.ScreenHeight, 0, 999);
            effect.Projection = proj;
            Stuff.SetProjection(proj);
            CenterCamera();
            brTextPos = new Vector2(Game1.ScreenWidth - verticalMenuSize.X, Game1.ScreenHeight - menuSize.Y);
            extraTextPos = new Vector2(10, Game1.ScreenHeight - menuSize.Y);
        }

        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
        }
        public void RemoveShape(Shape shape)
        {
            shapes.Remove(shape);
        }

        public void SetMenu(Menu menu)
        {
            if (menu == null) menu = toolsMenu;
            this.menu = menu;
            menu.OnResize();
        }

        public void SetBoundsSize(float width, float height)
        {
            bounds.SetSize(width, height);
            MakeGridVertex();
        }

        public void SetTool(Tool newTool)
        {
            if (currentTool != null)
                currentTool.OnExit();
            if (newTool == null)
            {
                SetMenu(null);
                mouseColor = Color.Green;
                MouseText = "";
            }
            currentTool = newTool;
        }

        public void MakeGridVertex()
        {
            float width = bounds.Width, height = bounds.Height;

            if (gridBuffer != null)
                gridBuffer.Dispose();
            int offset = 200;
            VertexPositionColor[] data = new VertexPositionColor[2 * (int)((width + offset) / gridSizeSeparator + 1) + 2 * (int)((height + offset) / gridSizeSeparator + 1)];
            Color col = Color.Multiply(Game1.BackColor, 1.33333333f);
            offset = ((offset/2) % gridSizeSeparator);
            int i = 0;
            float xtra = width + 100;
            float vertexMax = height + 100;
            for (float x = -100; x < xtra; x += gridSizeSeparator)
            {
                data[i++] = new VertexPositionColor(new Vector3(x+offset, -100, 0), col);
                data[i++] = new VertexPositionColor(new Vector3(x+offset, vertexMax, 0), col);
            }

            xtra = height + 100;
            vertexMax = width + 100;
            for (float y = -100; y < xtra; y += gridSizeSeparator)
            {
                data[i++] = new VertexPositionColor(new Vector3(-100, y+offset, 0), col);
                data[i++] = new VertexPositionColor(new Vector3(vertexMax, y+offset, 0), col);
            }

            gridBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), data.Length, BufferUsage.WriteOnly);
            gridBuffer.SetData(data);
            gridPrimitiveCount = data.Length / 2;
        }

        public byte[] GetSaveFile()
        {
            List<byte> data = new List<byte>(shapes.Count * 10);
            data.Add((byte)222);
            data.Add((byte)111);
            data.Add((byte)41);
            data.Add((byte)231);
            data.Add((byte)60); //preamble, if a file starts like this, it is a scenario file

            data.Add((byte)0); //next comes the version. this can be used in the future in case 
            data.Add((byte)1); //changed to the format are made, so the editor can recognize and understand.
            //Currently, having a (0, 0) means it has: bounds size, powerup, up to 8 players, shapes.
            //In version (0, 1), a name was added before the bounds size.

            data.Add((byte)ScenarioName.Length); //add the name length
            for (int i = 0; i < ScenarioName.Length; i++)
                data.Add((byte)ScenarioName[i]); //and each name char asa byte

            data.Add(BackgroundType);

            FileAddFloat(data, bounds.Width);//the next 4 bytes are the BoundsWidth
            FileAddFloat(data, bounds.Height);//the next 4 bytes are the BoundsHeight

            FileAddFloat(data, powerupPos.X); //next come the X and Y positions of
            FileAddFloat(data, powerupPos.Y); //the powerup in the map.

            List<Vector2> pps = new List<Vector2>(8);
            for (int i = 0; i < playersEnabled.Length; i++)
                if (playersEnabled[i])
                    pps.Add(playersPos[i]);
            data.Add((byte)pps.Count); //next, a byte with the amount of players the map can hold.
            for (int i = 0; i < pps.Count; i++)
            {
                FileAddFloat(data, pps[i].X); //for each player position, store the X and Y.
                FileAddFloat(data, pps[i].Y);
            }

            //after that, it cycles throu: {shapeType, data, data, data..., shapeType, etc}
            //shapeType is linegroup, add length in 4 bytes and add {from.x, from.y} for each vertex. Then add {last.to.x, last.to.y}
            for (int i = 0; i < shapes.Count; i++)
            {
                Shape s = shapes[i];
                if (s.IsOk)
                {
                    data.Add(s.Type);
                    if (s.Type == Shape.TypeLinegroup)
                    {
                        #region LineGroup
                        FileAddInt(data, s.lines.Length + 1); //save the amount of vertices
                        int c = 0;
                        for (; c < s.lines.Length; c++)
                        {
                            FileAddFloat(data, s.lines[c].from.X);
                            FileAddFloat(data, s.lines[c].from.Y);
                        }
                        FileAddFloat(data, s.lines[c - 1].to.X);
                        FileAddFloat(data, s.lines[c - 1].to.Y);
                        #endregion
                    }
                    else if (s.Type == Shape.TypePoly)
                    {
                        #region Polygon
                        FileAddInt(data, s.lines.Length); //save the amount of vertices
                        int c = 0;
                        for (; c < s.lines.Length; c++)
                        {
                            FileAddFloat(data, s.lines[c].from.X);
                            FileAddFloat(data, s.lines[c].from.Y);
                        }
                        #endregion
                    }
                    else if (s.Type == Shape.TypeRect)
                    {
                        #region Rectangle
                        Rect r = (Rect)s;
                        FileAddFloat(data, r.lines[0].from.X);
                        FileAddFloat(data, r.lines[0].from.Y);
                        FileAddFloat(data, r.Width);
                        FileAddFloat(data, r.Height);
                        #endregion
                    }
                }
            }

            return data.ToArray();
        }
        
        void FileAddFour(List<byte> list, byte[] more)
        {
            for (int i = 0; i < 4; i++)
                list.Add(more[i]);
        }
        void FileAddFloat(List<byte> list, float add)
        {
            FileAddFour(list, BitConverter.GetBytes(add / VertexSaveScale));
        }
        void FileAddInt(List<byte> list, int add)
        {
            FileAddFour(list, BitConverter.GetBytes(add));
        }
        float ReadFloat(byte[] arr, ref int index)
        {
            int i = index;
            index += 4;
            return BitConverter.ToSingle(arr, i) * VertexSaveScale;
        }
        int ReadInt(byte[] arr, ref int index)
        {
            int i = index;
            index += 4;
            return BitConverter.ToInt32(arr, i);
        }

        public override void OnExit()
        {
            LEL.Dispose();
            AutoSaveThread.Abort();
        }

        void SaveThread()
        {
            while (true)
            {
                for (int i = 3; i > 0; i--)
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder(30);
                    builder.Append("Autosaving in ");
                    builder.Append(i);
                    builder.Append(" minute");
                    if (i != 1)
                        builder.Append('s');
                    ExtraText = builder.ToString();
                    Thread.Sleep(60000); //sleep for 1 minute
                }
                if (FileLocation.Length != 0 && PlayerCount != 0)
                {
                    try
                    {
                        ExtraText = "Saving...";
                        byte[] f = GetSaveFile();
                        System.Text.StringBuilder b = new System.Text.StringBuilder(FileLocation.Length + 10);
                        b.Append(FileLocation);
                        b.Append("_autosave");
                        System.IO.File.WriteAllBytes(b.ToString(), f);
                        ExtraText = "Saved!";
                    }
                    catch
                    {
                        ExtraText = "Error autosaving.";
                    }
                }
                else
                {
                    ExtraText = "Map not ready for saving!";
                }
                Thread.Sleep(5000);
            }
        }
    }
}
