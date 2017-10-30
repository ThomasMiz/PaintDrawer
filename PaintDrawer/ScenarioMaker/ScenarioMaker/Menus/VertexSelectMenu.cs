using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScenarioMaker.Tools;

namespace ScenarioMaker.Menus
{
    class VertexSelectMenu : Menu
    {
        Button trans, rot, scal, sel, toshape;
        VertexSelectTool tool;

        public VertexSelectMenu(Scenes.Editor editor, VertexSelectTool tool)
            : base(editor)
        {
            this.tool = tool;
            Vector2 zero = Vector2.Zero;
            trans = new Button(zero, zero, Game1.BlueColor, "Translate", defIn);
            rot = new Button(zero, zero, Game1.BlueColor, "Rotate", defIn);
            scal = new Button(zero, zero, Game1.BlueColor, "Scale", defIn);
            sel = new Button(zero, zero, Game1.BlueColor, "Select", defIn);
            toshape = new Button(zero, zero, Game1.BlueColor, "To Shape\n Select", defIn);
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
            toshape.Draw(batch, mousePos);
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
            else if (toshape.Contains(mousePos))
            {
                tool.OnOperationChosen(VertexSelectTool.Op_ToShape);
            }
        }

        public override void OnResize()
        {
            float size = Game1.ScreenWidth / 5f, x = size / 2f;
            trans.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            scal.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            rot.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            sel.Resize(new Vector2(x, 75), new Vector2(size, 150));
            x += size;
            toshape.Resize(new Vector2(x, 75), new Vector2(size, 150));
        }
    }
}
