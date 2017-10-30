using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Menus
{
    class ToolsMenu : Menu
    {
        Button rect, poly, line, vsel, ssel;

        public ToolsMenu(Scenes.Editor e) : base(e)
        {
            rect = new Button(Vector2.Zero, Vector2.Zero, Game1.BlueColor, "Rectangle", defIn);
            poly = new Button(Vector2.Zero, Vector2.Zero, Game1.BlueColor, "Polygon", defIn);
            line = new Button(Vector2.Zero, Vector2.Zero, Game1.BlueColor, "LineGroup", defIn);
            vsel = new Button(Vector2.Zero, Vector2.Zero, Game1.BlueColor, "Vertex\nSelect", defIn);
            ssel = new Button(Vector2.Zero, Vector2.Zero, Game1.BlueColor, "Shape\nSelect", defIn);
        }

        public override void Update()
        {
            
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (rect.Contains(mousePos))
            {
                editor.SetTool(new Tools.RectTool(editor));
            }
            else if (poly.Contains(mousePos))
            {
                editor.SetTool(new Tools.PolyTool(editor));
            }
            else if (line.Contains(mousePos))
            {
                editor.SetTool(new Tools.LineTool(editor));
            }
            else if (vsel.Contains(mousePos))
            {
                editor.SetTool(new Tools.VertexSelectTool(editor));
            }
            else if (ssel.Contains(mousePos))
            {
                editor.SetTool(new Tools.ShapeSelectTool(editor));
            }
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            rect.Draw(batch, mousePos);
            poly.Draw(batch, mousePos);
            line.Draw(batch, mousePos);
            vsel.Draw(batch, mousePos);
            ssel.Draw(batch, mousePos);
        }

        public override void OnResize()
        {
            float size = Game1.ScreenWidth / 5, x = size/2;
            rect.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            poly.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            line.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            vsel.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            ssel.Resize(new Vector2(x, 75), new Vector2(size, 150));
        }
    }
}
