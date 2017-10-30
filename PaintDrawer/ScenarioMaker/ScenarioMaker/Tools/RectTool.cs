using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScenarioMaker.Tools
{
    class RectTool : Tool
    {
        VertexBuffer buffer;
        Vector2 from;
        bool firstChosen = false;

        public RectTool(Scenes.Editor e)
            : base(e)
        {
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), 5, BufferUsage.WriteOnly);
            editor.MouseText = "rectangle";
        }

        public override void Update()
        {
            if (firstChosen)
            {
                if ((Game1.game.ks.IsKeyDown(Keys.LeftControl) || Game1.game.ks.IsKeyDown(Keys.RightControl)) && Game1.game.KeyJustClicked(Keys.Z))
                {
                    firstChosen = false;
                }
            }
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (firstChosen)
            {
                Vector2 to = mousePos;
                editor.AddShape(new Rect(from.X, from.Y, to.X - from.X, to.Y - from.Y).ToPoly());
                editor.SetTool(null);
            }
            else
            {
                firstChosen = true;
                from = mousePos;
            }
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            if (firstChosen)
            {
                Vector2 tr = new Vector2(editor.MousePos.X, from.Y), bl = new Vector2(from.X, editor.MousePos.Y);
                VertexPositionColor[] data = new VertexPositionColor[5];
                data[0] = new VertexPositionColor(new Vector3(from, 0), Game1.BlueColor);
                data[1] = new VertexPositionColor(new Vector3(tr, 0), Game1.BlueColor);
                data[2] = new VertexPositionColor(new Vector3(tr.X, bl.Y, 0), Game1.BlueColor);
                data[3] = new VertexPositionColor(new Vector3(bl, 0), Game1.BlueColor);
                data[4] = data[0];

                buffer.SetData(data);

                foreach (EffectPass pass in editor.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.SetVertexBuffer(buffer);
                    device.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
                }

                batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, editor.cameraView);
                Color color = Game1.BlueColor;
                color.A = 96;
                Vector2 t = new Vector2(tr.X, bl.Y);
                float s = editor.oneOverScale * 5;
                Game1.game.DrawThickLine(from, tr, s, color);
                Game1.game.DrawThickLine(tr, t, s, color);
                Game1.game.DrawThickLine(t, bl, s, color);
                Game1.game.DrawThickLine(bl, from, s, color);
                
                batch.End();
            }
        }

        public override void OnExit()
        {
            buffer.Dispose();
        }
    }
}
