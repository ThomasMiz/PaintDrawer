using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker
{
    class Stuff
    {
        public static Random r = new Random(DateTime.Now.Millisecond);
        public static float Random() { return (float)r.NextDouble(); }
        public static float Random(float max) { return (float)r.NextDouble() * max; }
        public static float Random(float min, float max) { return min + (float)r.NextDouble() * (max - min); }
        public static bool RandomBool() { return r.NextDouble() > 0.5; }
        public static String RandomName()
        {
            String[] f = new String[] { "Mlg ", "The ", "Dank ", "Chill ", "Massive " };
            String[] s = new String[] { "SwagLand", "Clusterfuck", "Memes", "Voyage", "Timelink" };
            return f[r.Next(f.Length)] + s[r.Next(s.Length)];
        }

        public const float PiOver8 = MathHelper.PiOver4 / 2f;

        private static VertexBuffer circleBuffer, guyBuffer;
        private static Effect effect;
        private static int circlePrimitiveCount;
        public static Scenes.Editor editor;

        private static String powerupText;
        private static float powerupScale, powerupRadius = 30;
        private static Vector2 powerupOrigin;

        public static void Init()
        {
            effect = Game1.game.Content.Load<Effect>("colorfx");
            int vertexCount = 128;
            circlePrimitiveCount = vertexCount;
            VertexPositionColor[] data = new VertexPositionColor[vertexCount + 1];
            float diff = MathHelper.TwoPi / vertexCount;
            for (int i = 0; i < vertexCount; i++)
            {
                float rot = diff * i;
                data[i] = new VertexPositionColor(new Vector3((float)Math.Cos(rot), (float)Math.Sin(rot), 0), Color.White);
            }
            data[vertexCount] = data[0];
            circleBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), data.Length, BufferUsage.WriteOnly);
            circleBuffer.SetData(data);
            
            data[0] = new VertexPositionColor(new Vector3(.5f, .6f, 0), Game1.GreenColor);
            data[1] = new VertexPositionColor(new Vector3(.39f, .94f, 0), Game1.GreenColor);
            data[2] = new VertexPositionColor(new Vector3(.43f, .62f, 0), Game1.GreenColor);
            data[3] = new VertexPositionColor(new Vector3(.39f, .41f, 0), Game1.GreenColor);
            data[4] = new VertexPositionColor(new Vector3(.10f, .36f, 0), Game1.GreenColor);
            data[5] = new VertexPositionColor(new Vector3(.46f, .32f, 0), Game1.GreenColor);
            data[6] = new VertexPositionColor(new Vector3(.46f, .28f, 0), Game1.GreenColor);
            data[7] = new VertexPositionColor(new Vector3(.38f, .2f, 0), Game1.GreenColor);
            data[8] = new VertexPositionColor(new Vector3(.38f, .13f, 0), Game1.GreenColor);
            data[9] = new VertexPositionColor(new Vector3(.46f, .05f, 0), Game1.GreenColor);
            data[10] = new VertexPositionColor(new Vector3(.53f, .05f, 0), Game1.GreenColor);
            data[11] = new VertexPositionColor(new Vector3(.61f, .13f, 0), Game1.GreenColor);
            data[12] = new VertexPositionColor(new Vector3(.61f, .2f, 0), Game1.GreenColor);
            data[13] = new VertexPositionColor(new Vector3(.53f, .28f, 0), Game1.GreenColor);
            data[14] = new VertexPositionColor(new Vector3(.53f, .32f, 0), Game1.GreenColor);
            data[15] = new VertexPositionColor(new Vector3(.89f, .36f, 0), Game1.GreenColor);
            data[16] = new VertexPositionColor(new Vector3(.6f, .41f, 0), Game1.GreenColor);
            data[17] = new VertexPositionColor(new Vector3(.56f, .62f, 0), Game1.GreenColor);
            data[18] = new VertexPositionColor(new Vector3(.6f, .94f, 0), Game1.GreenColor);
            data[19] = new VertexPositionColor(new Vector3(.5f, .6f, 0), Game1.GreenColor);
            guyBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), 20, BufferUsage.WriteOnly);
            guyBuffer.SetData(data, 0, 20);

            powerupText = "PowerUp";
            powerupOrigin = Game1.font.MeasureString(powerupText) * 0.5f;
            powerupScale = powerupRadius / powerupOrigin.X * 0.85f;
        }

        public static void DrawCircle(Vector2 center, float radius, Color color)
        {
            Game1.game.GraphicsDevice.SetVertexBuffer(circleBuffer);
            effect.Parameters[3].SetValue(color.ToVector4());
            effect.Parameters[0].SetValue(Matrix.CreateScale(radius) * Matrix.CreateTranslation(center.X, center.Y, 0));
            effect.CurrentTechnique.Passes[0].Apply();
            Game1.game.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, circlePrimitiveCount);
        }
        public static void DrawGuy(Vector2 center, float size, Color color)
        {
            Game1.game.GraphicsDevice.SetVertexBuffer(guyBuffer);
            effect.Parameters[3].SetValue(color.ToVector4());
            effect.Parameters[0].SetValue(Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) * Matrix.CreateScale(size) * Matrix.CreateTranslation(center.X, center.Y, 0));
            effect.CurrentTechnique.Passes[0].Apply();
            Game1.game.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 19);
        }

        public static void DrawPlayerIndicator(Vector2 center, Color color, int number)
        {
            effect.CurrentTechnique.Passes[0].Apply();
            DrawCircle(center, 25f, color);
            DrawGuy(center, 50f, color);
            Game1.game.batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, editor.cameraView);
            int n = number+1;
            String s = n.ToString();
            Game1.game.batch.DrawString(Game1.font, s, center, color, 0f, Game1.font.MeasureString(s) * 0.5f, 0.25f, SpriteEffects.None, 0f);
            Game1.game.batch.End();
        }

        public static void DrawPowerupIndicator(Vector2 center, Color color)
        {
            DrawCircle(center, powerupRadius, color);
            Game1.game.batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, editor.cameraView);
            Game1.game.batch.DrawString(Game1.font, powerupText, center, color, 0f, powerupOrigin, powerupScale, SpriteEffects.None, 0f);
            Game1.game.batch.End();
        }

        public static void Dispose()
        {
            circleBuffer.Dispose();
            guyBuffer.Dispose();
        }

        public static void SetProjection(Matrix proj) { effect.Parameters[2].SetValue(proj); }
        public static void SetView(Matrix view) { effect.Parameters[1].SetValue(view); }
    }

    class Button
    {
        static Vector2 origin = new Vector2(0.5f, 0.5f);

        Vector2 pos, size, textOrigin, topLeft;
        protected Color dark, light;
        float textSize, pixelsIn;
        String text;
        bool enabled = true;
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        public Button(Vector2 center, Vector2 size, Color darkColor, String text)
        {
            pixelsIn = 0;
            dark = darkColor;
            light = Color.Multiply(dark, 1.25f);
            this.text = text;
            Vector2 ts = Game1.font.MeasureString(text);
            textOrigin = ts / 2.0f;
        }

        public Button(Vector2 center, Vector2 size, Color darkColor, String text, float pixelsInward)
        {
            pixelsIn = pixelsInward + pixelsInward;
            dark = darkColor;
            light = Color.Multiply(dark, 1.25f);
            this.text = text;
            Vector2 ts = Game1.font.MeasureString(text);
            textOrigin = ts / 2.0f;
        }

        public void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            batch.Draw(Game1.white, pos, null, enabled ? (Contains(mousePos) ? light : dark) : new Color(192,192,192), 0f, origin, size, SpriteEffects.None, 0f);
            batch.DrawString(Game1.font, text, pos, enabled ? Color.Black : Color.DimGray, 0f, textOrigin, textSize, SpriteEffects.None, 0);
        }

        public bool Contains(Vector2 mousePos)
        {
            Vector2 m = mousePos - topLeft;
            return enabled && (m.X > 0 && m.X < size.X && m.Y > 0 && m.Y < size.Y);
        }

        public void Resize(Vector2 center, Vector2 newsize)
        {
            pos = center;
            size = newsize;
            size.X -= pixelsIn;
            size.Y -= pixelsIn;
            Vector2 m = Game1.font.MeasureString(text);
            Vector2 textIn = size * 0.1f;
            textSize = Math.Min((size.X-textIn.X) / m.X, (size.Y-textIn.Y) / m.Y);
            if (textSize > 1.5f) textSize = 1.5f;
            topLeft = pos - (size / 2f);
        }
    }

    class ToggleButton : Button
    {
        bool value;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                dark = value ? green : red;
                light = Color.Multiply(dark, 1.25f);
            }
        }

        static Color green = new Color(128, 192, 128), red = new Color(192, 128, 128);

        public ToggleButton(Vector2 center, Vector2 size, String text)
            : base(center, size, green, text) { value = true; }

        public ToggleButton(Vector2 center, Vector2 size, String text, float pixelsInward)
            : base(center, size, green, text, pixelsInward) { value = true; }

        public void OnClick()
        {
            value = !value;
            dark = value ? green : red;
            light = Color.Multiply(dark, 1.25f);
        }
    }

    class Slider
    {
        static Vector2 origin = new Vector2(0.5f, 0.5f);
        static float valSize = 25, halfValSize = valSize / 2f;
        Vector2 pos, size, valPos, topLeft;
        Color dark, light, sliderDark, sliderLight;
        float inX, textSize, valMaxX, valMinX;
        float minValue, maxValue, valueDelta, val;
        public bool isSliding = false, ClampInt;
        String text;
        Vector2 textOrigin;
        public float Value
        {
            get { return val; }
            set
            {
                if (ClampInt)
                    val = (int)(value+0.5f);
                else
                    val = ((int)(value * 1000)) / 1000f;
                if (val > maxValue)
                    val = maxValue;
                else if (val < minValue)
                    val = minValue;
                text = val.ToString();
                valPos.X = (val - minValue) / valueDelta * (valMaxX - valMinX) + valMinX;
                textOrigin = Game1.font.MeasureString(text) / 2f;
            }
        }

        public Slider(Vector2 center, Vector2 size, Color darkColor, Color sliderColor, float inX, float minValue, float maxValue, float initialValue, bool clampInt)
        {
            this.ClampInt = clampInt;
            this.inX = inX + inX;
            dark = Color.Multiply(darkColor, 1.25f);
            light = Color.Multiply(dark, 1.25f);
            sliderDark = sliderColor;
            sliderLight = Color.Multiply(sliderDark, 1.25f);
            textSize = valSize / Game1.font.MeasureString("0").Y;
            this.minValue = minValue;
            this.maxValue = maxValue;
            valueDelta = maxValue - minValue;
            val = initialValue;
        }

        public void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            batch.Draw(Game1.white, pos, null, Contains(mousePos) ? light : dark, 0f, origin, size, SpriteEffects.None, 0f);
            batch.Draw(Game1.white, valPos, null, isSliding ? sliderDark : sliderLight, 0f, origin, valSize, SpriteEffects.None, 0f);
            batch.DrawString(Game1.font, text, pos, Color.Black, 0f, textOrigin, textSize, SpriteEffects.None, 0f);
        }

        public void UpdateOpen(float MouseX)
        {
            valPos.X = MouseX;
            if (valPos.X < valMinX)
                valPos.X = valMinX;
            else if (valPos.X > valMaxX)
                valPos.X = valMaxX;
            Value = minValue + valueDelta * ((valPos.X - valMinX) / (valMaxX - valMinX));
            isSliding = Game1.game.ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        }

        public bool Contains(Vector2 mousePos)
        {
            Vector2 m = mousePos - topLeft;
            return (m.X > 0 && m.X < size.X && m.Y > 0 && m.Y < size.Y);
        }

        public void Resize(Vector2 center, float sizeX)
        {
            pos = center;
            size = new Vector2(sizeX - inX, valSize);
            float thingy = size.X / 2 - halfValSize;
            valPos.Y = pos.Y;
            valMinX = pos.X - thingy;
            valMaxX = pos.X + thingy;
            topLeft = center - size / 2;
            Value = val;
        }
    }

    class NumberTextBox
    {
        static float thingDelay = 250;
        static Vector2 origin = new Vector2(0.5f, 0.5f);
        StringBuilder text;
        Vector2 pos, size, underPos, underSize;
        Vector2 topLeft;
        float textScale, sizeIn, nextThing;
        bool showThing;
        public bool isTyping;
        public int Value { get { return Int32.Parse(text.ToString()); } }
        Color dark, light, med;
        int min, max;
        Menus.Menu menu;

        public NumberTextBox(Vector2 center, Vector2 size, Color dark, float sizeIn, int value, int min, int max, Menus.Menu menu)
        {
            this.menu = menu;
            this.sizeIn = sizeIn + sizeIn;
            text = new StringBuilder(max.ToString().Length + 1);
            text.Append(value.ToString());
            pos = center;
            this.dark = dark;
            this.light = Color.Multiply(dark, 1.4f);
            med = new Color(dark.R, dark.G, light.B);
            this.min = min;
            this.max = max;
        }

        public bool OnClick(Vector2 mousePos)
        {
            if (!isTyping && Contains(mousePos))
            {
                EventInput.CharEntered += OnCharEnter;
                nextThing = Game1.time;
                showThing = true;
                isTyping = true;
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            if (isTyping)
            {
                batch.Draw(Game1.white, pos, null, med, 0f, origin, size, SpriteEffects.None, 0f);
                if (Game1.time > nextThing)
                {
                    nextThing += thingDelay;
                    showThing = !showThing;
                }
                if(showThing)
                {
                    Vector2 s = Game1.font.MeasureString(text) * textScale;
                    s.Y = 0;
                    batch.DrawString(Game1.font, "_", topLeft + s, Color.Black, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                batch.Draw(Game1.white, pos, null, Contains(mousePos) ? light : dark, 0f, origin, size, SpriteEffects.None, 0f);
            }
            batch.Draw(Game1.white, underPos, null, Color.DarkSlateGray, 0f, origin, underSize, SpriteEffects.None, 0f);
            batch.DrawString(Game1.font, text, topLeft, Color.Black, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
        }

        public bool Contains(Vector2 mousePos)
        {
            Vector2 m = mousePos - topLeft;
            return (m.X > 0 && m.X < size.X && m.Y > 0 && m.Y < size.Y);
        }

        public void OnCharEnter(object sender, CharacterEventArgs e)
        {
            if (e.Character == 13)
            {
                isTyping = false;
                EventInput.CharEntered -= OnCharEnter;
                int val;
                if (text.Length == 0)
                    val = 0;
                else
                    val = Value;
                if (val < min)
                    val = min;
                else if (val > max)
                    val = max;
                text.Clear();
                text.Append(val.ToString());
                menu.NumberTextBoxDone(this);
            }
            else if (e.Character >= '0' && e.Character <= '9')
            {
                if (text.Length < text.Capacity)
                {
                    if (text.Length == 1 && text[0] == '0')
                        text.Clear();
                    text.Append(e.Character);
                    int val;
                    if (text.Length == 0)
                        val = 0;
                    else
                        val = Value;
                    if (val > max)
                        val = max;
                    text.Clear();
                    text.Append(val.ToString());
                    menu.NumberTextBoxChanged(this);
                }
            }
            else if (e.Character == 8)
            {//delete char
                text.Length--;
                if (text.Length == 0)
                    text.Append("0");
                menu.NumberTextBoxChanged(this);
            }
        }

        public void OnResize(Vector2 center, Vector2 size)
        {
            pos = center;
            size.X -= sizeIn;
            size.Y -= sizeIn;
            this.size = size;
            topLeft = pos - size / 2f;
            underPos.X = pos.X;
            underSize.Y = 3f;
            underPos.Y = topLeft.Y + size.Y - underSize.Y / 1.5f;
            underSize.X = size.X * 0.9f;

            Vector2 s = Game1.font.MeasureString(max.ToString());
            textScale = Math.Min(size.X / s.X, size.Y / s.Y);
        }
    }

    class JustText
    {
        String text;
        float sizeIn, scale;
        Vector2 pos, origin, measure;

        public JustText(Vector2 center, Vector2 size, String text, float sizeIn)
        {
            this.text = text;
            measure = Game1.font.MeasureString(text);
            origin = measure / 2f;
            pos = center;
            this.sizeIn = sizeIn + sizeIn;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(Game1.font, text, pos, Color.Black, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public void Resize(Vector2 center, Vector2 size)
        {
            pos = center;
            size.X -= sizeIn;
            size.Y -= sizeIn;
            scale = Math.Min(size.X / measure.X, size.Y / measure.Y);
            
        }
    }

    class TextBox
    {
        static float thingDelay = 250;
        static Vector2 origin = new Vector2(0.5f, 0.5f);
        StringBuilder text;
        Vector2 pos, size, underPos, underSize;
        Vector2 topLeft;
        float textScale, sizeIn, nextThing;
        bool showThing;
        public bool isTyping;
        public String Value { get { return text.ToString(); } }
        Color dark, light, med;
        int maxl;
        Menus.Menu menu;

        public TextBox(Vector2 center, Vector2 size, Color dark, float sizeIn, String value, int maxLength, Menus.Menu menu)
        {
            this.menu = menu;
            this.sizeIn = sizeIn + sizeIn;
            maxl = maxLength;
            text = new StringBuilder(maxl);
            text.Append(value);
            pos = center;
            this.dark = dark;
            this.light = Color.Multiply(dark, 1.4f);
            med = new Color(dark.R, dark.G, light.B);
        }

        public bool OnClick(Vector2 mousePos)
        {
            if (isTyping)
            {
                if (!Contains(mousePos))
                {
                    OnCharEnter(null, new CharacterEventArgs((char)13, 0));
                }
            }
            else
            {
                if (Contains(mousePos))
                {
                    EventInput.CharEntered += OnCharEnter;
                    nextThing = Game1.time;
                    showThing = true;
                    isTyping = true;
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            if (isTyping)
            {
                batch.Draw(Game1.white, pos, null, med, 0f, origin, size, SpriteEffects.None, 0f);
                if (Game1.time > nextThing)
                {
                    nextThing += thingDelay;
                    showThing = !showThing;
                }
                if (showThing)
                {
                    Vector2 s = Game1.font.MeasureString(text) * textScale;
                    s.Y = 0;
                    batch.DrawString(Game1.font, "_", topLeft + s, Color.Black, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                batch.Draw(Game1.white, pos, null, Contains(mousePos) ? light : dark, 0f, origin, size, SpriteEffects.None, 0f);
            }
            batch.Draw(Game1.white, underPos, null, Color.DarkSlateGray, 0f, origin, underSize, SpriteEffects.None, 0f);
            batch.DrawString(Game1.font, text, topLeft, Color.Black, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
        }

        bool Contains(Vector2 mousePos)
        {
            Vector2 m = mousePos - topLeft;
            return (m.X > 0 && m.X < size.X && m.Y > 0 && m.Y < size.Y);
        }

        void OnCharEnter(object sender, CharacterEventArgs e)
        {
            if (e.Character == 13)
            {
                isTyping = false;
                EventInput.CharEntered -= OnCharEnter;
                menu.TextBoxDone(this);
            }
            else if (e.Character > 31 && e.Character < 255)
            {
                if (text.Length < maxl)
                    text.Append(e.Character);
            }
            else if (e.Character == 8)
            {//delete char
                if (text.Length != 0)
                    text.Length--;
            }
        }

        public void OnResize(Vector2 center, Vector2 size)
        {
            pos = center;
            size.X -= sizeIn;
            size.Y -= sizeIn;
            this.size = size;
            topLeft = pos - size / 2f;
            underPos.X = pos.X;
            underSize.Y = 3f;
            underPos.Y = topLeft.Y + size.Y - underSize.Y / 1.5f;
            underSize.X = size.X * 0.9f;

            Vector2 s = Game1.font.MeasureString("M");
            textScale = size.Y / s.Y;
        }
    }
}