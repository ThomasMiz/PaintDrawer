using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScenarioMaker.Tools;

namespace ScenarioMaker.Menus
{
    class ShapeSelectMenu : Menu
    {
        Button trans, rot, scal, dupe, sel, delt;
        ShapeSelectTool tool;

        public ShapeSelectMenu(Scenes.Editor editor, ShapeSelectTool tool)
            : base(editor)
        {
            this.tool = tool;
            Vector2 zero = Vector2.Zero;
            trans = new Button(zero, zero, Game1.BlueColor, "Translate", defIn);
            rot = new Button(zero, zero, Game1.BlueColor, "Rotate", defIn);
            scal = new Button(zero, zero, Game1.BlueColor, "Scale", defIn);
            sel = new Button(zero, zero, Game1.BlueColor, "Select", defIn);
            dupe = new Button(zero, zero, Game1.BlueColor, "Duplicate", defIn);
            delt = new Button(zero, zero, Game1.BlueColor, "Delete", defIn);
        }

        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            trans.Draw(batch, mousePos);
            scal.Draw(batch, mousePos);
            rot.Draw(batch, mousePos);
            sel.Draw(batch, mousePos);
            dupe.Draw(batch, mousePos);
            delt.Draw(batch, mousePos);
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (trans.Contains(mousePos))
            {
                tool.OnOperationChosen(VertexSelectTool.Op_Trans);
            }
            else if (scal.Contains(mousePos))
            {
                tool.OnOperationChosen(VertexSelectTool.Op_Scal);
            }
            else if (rot.Contains(mousePos))
            {
                tool.OnOperationChosen(VertexSelectTool.Op_Rot);
            }
            else if (sel.Contains(mousePos))
            {
                tool.OnOperationChosen(VertexSelectTool.Op_Sel);
            }
            else if (dupe.Contains(mousePos))
            {
                tool.OnOperationChosen(ShapeSelectTool.Op_Dupe);
            }
            else if (delt.Contains(mousePos))
            {
                tool.OnOperationChosen(ShapeSelectTool.Op_Delete);
            }
        }

        public override void OnResize()
        {
            float size = Game1.ScreenWidth / 6f, x = size / 2f;
            trans.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            scal.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            rot.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            sel.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            dupe.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            delt.Resize(new Vector2(x, 75), new Vector2(size, 150));
            
        }
    }
}
