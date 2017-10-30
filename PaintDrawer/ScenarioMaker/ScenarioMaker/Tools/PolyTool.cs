using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScenarioMaker.Tools
{
    class PolyTool : Tool
    {
        static int size = 30;
        
        VertexBuffer buffer;
        Vector2[] vertices;
        List<LinearMath.Line> lines;
        VertexPositionColor[] data;
        int primitiveCount = 0, dataIndex = 0;
        bool validLine = true;

        public PolyTool(Scenes.Editor e)
            : base(e)
        {
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), size, BufferUsage.WriteOnly);
            vertices = new Vector2[size];
            data = new VertexPositionColor[size];
            editor.MouseText = "polygon";
            lines = new List<LinearMath.Line>(size);
        }

        public override void Update()
        {
            editor.MouseText = "polygon";
            if ((Game1.game.ks.IsKeyDown(Keys.LeftControl) || Game1.game.ks.IsKeyDown(Keys.RightControl)) && Game1.game.KeyJustClicked(Keys.Z))
            {
                if(primitiveCount != 0)
                {
                    primitiveCount--;
                    dataIndex--;
                    if (lines.Count > 0)
                        lines.RemoveAt(lines.Count - 1);
                }
            }
            else if (primitiveCount > 2)
            {
                float dat = (editor.oneOverScale * Scenes.Editor.ClampDistance);
                if (Vector2.DistanceSquared(vertices[0], editor.MousePos) < dat * dat)
                {
                    editor.MousePos = vertices[0];
                    editor.MouseText = "close poly [enter]";
                }
                if (Game1.game.KeyJustClicked(Keys.Enter))
                    ClosePoly();
            }

            if (dataIndex != 0)
            {
                LinearMath.Raycast cast = new LinearMath.Raycast(lines, vertices[dataIndex - 1], editor.MousePos);
                if (editor.MouseText.StartsWith("cl"))
                    validLine = true;
                else
                    validLine = cast.HasNoContacts;
            }
        }

        private void ClosePoly()
        {
            Vector2[] v = new Vector2[dataIndex];
            for (int i = 0; i < v.Length; i++)
                v[i] = vertices[i];
            editor.AddShape(new Polygon(v));
            editor.SetTool(null);
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (validLine)
            {
                vertices[dataIndex] = mousePos;
                if (primitiveCount > 2 && editor.MouseText.StartsWith("cl")) //mousetext is "close poly"
                {
                    ClosePoly();
                    return;
                }
                data[dataIndex] = new VertexPositionColor(new Vector3(mousePos, 0), Game1.BlueColor);
                primitiveCount++;
                dataIndex++;
                if (dataIndex > 2)
                {
                    lines.Add(new LinearMath.Line(vertices[dataIndex - 3], vertices[dataIndex - 2]));
                }

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

                if (!validLine)
                {
                    color.R = Game1.RedColor.R;
                    color.G = Game1.RedColor.G;
                    color.B = Game1.RedColor.B;
                }

                Game1.game.DrawThickLine(vertices[max], editor.MousePos, s, color);
                color.A = 64;
                Game1.game.DrawThickLine(editor.MousePos, vertices[0], s / 2f, color);

                batch.End();
            }
        }

        public override void OnExit()
        {
            buffer.Dispose();
        }
    }
}
