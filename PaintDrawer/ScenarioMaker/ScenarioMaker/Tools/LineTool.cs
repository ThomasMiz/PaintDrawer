using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScenarioMaker.Tools
{
    class LineTool : Tool
    {
        static int size = 30;
        
        VertexBuffer buffer;
        Vector2[] vertices;
        VertexPositionColor[] data;
        int primitiveCount = 0, dataIndex = 0;

        public LineTool(Scenes.Editor e)
            : base(e)
        {
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), size, BufferUsage.WriteOnly);
            vertices = new Vector2[size];
            data = new VertexPositionColor[size];
            editor.MouseText = "linegroup";
        }

        public override void Update()
        {
            editor.MouseText = "linegroup";
            if ((Game1.game.ks.IsKeyDown(Keys.LeftControl) || Game1.game.ks.IsKeyDown(Keys.RightControl)) && Game1.game.KeyJustClicked(Keys.Z))
            {
                if (primitiveCount != 0)
                {
                    primitiveCount--;
                    dataIndex--;
                }
            }
            else if (primitiveCount > 1 && Game1.game.KeyJustClicked(Keys.Enter))
                CloseLine();
        }

        private void CloseLine()
        {
            Vector2[] v = new Vector2[dataIndex];
            for (int i = 0; i < v.Length; i++)
                v[i] = vertices[i];
            editor.AddShape(new Shape(v));
            editor.SetTool(null);
        }

        public override void OnClick(Vector2 mousePos)
        {
            vertices[dataIndex] = mousePos;
            data[dataIndex] = new VertexPositionColor(new Vector3(mousePos, 0), Game1.BlueColor);
            primitiveCount++;
            dataIndex++;

            if (dataIndex == vertices.Length)
            {
                Vector2[] oldVert = vertices;
                VertexPositionColor[] oldDat = data;
                vertices = new Vector2[oldVert.Length + size];
                data = new VertexPositionColor[oldDat.Length + size];
                for (int i = 0; i < oldVert.Length; i++)
                {
                    data[i] = oldDat[i];
                    vertices[i] = oldVert[i];
                }
                buffer.Dispose();
                buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            }
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            data[dataIndex] = new VertexPositionColor(new Vector3(editor.MousePos, 0), Game1.BlueColor);
            buffer.SetData(data);
            device.SetVertexBuffer(buffer);
            if (primitiveCount != 0)
            {
                foreach (EffectPass pass in editor.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.LineStrip, 0, primitiveCount);
                }

                batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, editor.cameraView);
                Color color = Game1.BlueColor;
                color.A = 96;
                float s = editor.oneOverScale * 5;
                int max = dataIndex - 1;
                for (int i = 0; i < max; )
                    Game1.game.DrawThickLine(vertices[i++], vertices[i], s, color);
                Game1.game.DrawThickLine(vertices[max], editor.MousePos, s, color);

                batch.End();
            }
        }

        public override void OnExit()
        {
            buffer.Dispose();
        }
    }
}
