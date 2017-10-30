using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScenarioMaker.Menus;

namespace ScenarioMaker.Tools
{
    class ShapeSelectTool : Tool
    {
        public const byte Op_Trans = VertexSelectTool.Op_Trans, Op_Scal = VertexSelectTool.Op_Scal, Op_Rot = VertexSelectTool.Op_Rot, Op_Sel = VertexSelectTool.Op_Sel, Op_Dupe = 4, Op_Delete = 5;

        bool somethingChosen = false;
        byte operation;
        bool originSet, operating = false;
        float dat, dat2lol;
        Vector2 from, to, opOrigin;
        List<Shape> selected;
        SimpleTextMenu simpleMenu;
        
        bool done = false, firstChosen = false;

        public ShapeSelectTool(Scenes.Editor e) : base(e)
        {
            selected = new List<Shape>(30);
            editor.MouseText = "shapeSelect";
        }

        public ShapeSelectTool(Scenes.Editor e, List<Shape> shapes) : base(e)
        {
            selected = shapes;
            editor.MouseText = "shapeSelect";
            done = true;
            firstChosen = true;
            somethingChosen = true;
            editor.SetMenu(new ShapeSelectMenu(editor, this));
        }

        public override void Update()
        {
            if (operating)
            {
                if (operation == Op_Trans)
                {
                    #region Translate
                    Vector2 delta = editor.MousePos - editor.OldMousePos;
                    if (Game1.game.ms.LeftButton == ButtonState.Pressed && editor.MouseOnEditor())
                    {
                        for (int i = 0; i < selected.Count; i++)
                        {
                            Line[] l = selected[i].lines;
                            for(int c=0; c<l.Length; c++)
                            {
                                l[c].from += delta;
                                l[c].to += delta;
                            }
                        }

                    }
                    RecalcShapeVertexes();
                    if (Game1.game.KeyJustClicked(Keys.Enter))
                        FinishOp();
                    #endregion
                }
                else if (originSet)
                {
                    if (Game1.game.KeyJustClicked(Keys.Enter))
                        FinishOp();
                    else if (Game1.game.ms.LeftButton == ButtonState.Pressed && !Single.IsNaN(dat) && editor.MouseOnEditor())
                    {
                        if (operation == Op_Scal)
                        {
                            #region Scale
                            float multnot = Vector2.Distance(opOrigin, editor.MousePos) / 250f;
                            float mult = multnot - dat + 1;
                            for (int i = 0; i < selected.Count; i++)
                            {
                                Line[] l = selected[i].lines;
                                for (int c = 0; c < l.Length; c++)
                                {
                                    l[c].from = (l[c].from - opOrigin) * mult + opOrigin;
                                    l[c].to = (l[c].to - opOrigin) * mult + opOrigin;
                                }
                            }
                            dat = multnot;
                            RecalcShapeVertexes();
                            #endregion
                        }
                        else if (operation == Op_Rot)
                        {
                            #region Rotate
                            float current = (float)Math.Atan2(editor.MousePos.Y - opOrigin.Y, editor.MousePos.X - opOrigin.X) - dat2lol;
                            if (Game1.game.ks.IsKeyDown(Keys.LeftShift) || Game1.game.ks.IsKeyDown(Keys.RightShift))
                                current -= ((current + MathHelper.PiOver4) % Stuff.PiOver8);
                            float deltaRot = current - dat;
                            for (int i = 0; i < selected.Count; i++)
                            {
                                Line[] l = selected[i].lines;
                                for (int c = 0; c < l.Length; c++)
                                {
                                    Vector2 f = l[c].from;
                                    float rot = (float)Math.Atan2(f.Y - opOrigin.Y, f.X - opOrigin.X), dist = Vector2.Distance(f, opOrigin);
                                    rot += deltaRot;
                                    f.X = (float)Math.Cos(rot) * dist + opOrigin.X;
                                    f.Y = (float)Math.Sin(rot) * dist + opOrigin.Y;
                                    l[c].from = f;

                                    f = l[c].to;
                                    rot = (float)Math.Atan2(f.Y - opOrigin.Y, f.X - opOrigin.X);
                                    dist = Vector2.Distance(f, opOrigin);
                                    rot += deltaRot;
                                    f.X = (float)Math.Cos(rot) * dist + opOrigin.X;
                                    f.Y = (float)Math.Sin(rot) * dist + opOrigin.Y;
                                    l[c].to = f;
                                }
                            }
                            dat = current;
                            RecalcShapeVertexes();
                            #endregion
                        }
                    }
                }
            }
        }

        void FinishOp()
        {
            operating = true;
            originSet = false;
            editor.SetMenu(new ShapeSelectMenu(editor, this));
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, editor.cameraView);
            Color color = Game1.BlueColor;
            color.A = 64;
            if (somethingChosen)
            {
                float littleScale = editor.oneOverScale / 2.5f, thickness = editor.oneOverScale * 3;
                for (int i = 0; i < selected.Count; i++)
                    for (int c = 0; c < selected[i].lines.Length; c++)
                    {
                        Line l = selected[i].lines[c];
                        color.A = 196;
                        Game1.game.DrawThickLine(l.from, l.to, thickness, color);
                        color.A = 64;
                        Game1.game.DrawXAt(l.from, color, littleScale);
                        Game1.game.DrawXAt(l.to, color, littleScale);
                    }
            }
            if (done)
            {
                if (operating)
                {
                    if (originSet)
                        Game1.game.DrawXAt(opOrigin, Color.Magenta, editor.oneOverScale);
                }
                else
                    batch.Draw(Game1.white, from, null, color, 0f, Vector2.Zero, to - from, SpriteEffects.None, 0f);
            }
            else if (firstChosen)
            {
                Vector2 size = new Vector2(editor.MousePos.X - from.X, editor.MousePos.Y - from.Y);
                Vector2 pos = from;
                if (size.X < 0)
                {
                    size.X = -size.X;
                    pos.X = editor.MousePos.X;
                }
                if (size.Y < 0)
                {
                    size.Y = -size.Y;
                    pos.Y = editor.MousePos.Y;
                }

                batch.Draw(Game1.white, pos, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);
            }
            batch.End();
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (operating)
            {
                if (originSet)
                {
                    if (operation == Op_Scal)
                    {
                        dat = Vector2.Distance(opOrigin, mousePos) / 250f;
                    }
                    else if (operation == Op_Rot)
                    {
                        dat2lol = (float)Math.Atan2(editor.MousePos.Y - opOrigin.Y, editor.MousePos.X - opOrigin.X);
                        dat = 0;
                    }
                    Update();
                }
                else
                {
                    originSet = true;
                    opOrigin = mousePos;
                    simpleMenu.SetText("Drag to operate. Enter to finish.");
                    dat = Single.NaN;
                    if (operation != Op_Trans)
                        editor.verticalMenu.ClampMouse = false;
                }
            }
            else if (done)
            {

            }
            else if (firstChosen)
            {
                to = mousePos;
                if(to.X < from.X)
                {
                    float tmp = to.X;
                    to.X = from.X;
                    from.X = tmp;
                }
                if (to.Y < from.Y)
                {
                    float tmp = to.Y;
                    to.Y = from.Y;
                    from.Y = tmp;
                }

                for (int i = 0; i < editor.shapes.Count; i++)
                {
                    Shape s = editor.shapes[i];
                    if (s.Type != Shape.TypeRect)
                        for (int c = 0; c < s.lines.Length; c++)
                            if ((Contains(s.lines[c].from) || Contains(s.lines[c].to)) && !selected.Contains(s))
                                selected.Add(s);
                }

                editor.SetMenu(new ShapeSelectMenu(editor, this));

                done = true;
                somethingChosen = true;
            }
            else
            {
                firstChosen = true;
                from = mousePos;
            }
        }

        bool Contains(Vector2 point)
        {
            return point.X >= from.X && point.X <= to.X && point.Y > from.Y && point.Y < to.Y;
        }

        public override void OnExit()
        {
            
        }

        public void OnOperationChosen(byte b)
        {
            if (b == Op_Delete)
            {
                for (int i = 0; i < selected.Count; i++)
                    editor.RemoveShape(selected[i]);
                editor.SetTool(null);
                return;
            }
            if (b == Op_Dupe)
            {
                Vector2 delta = new Vector2(-editor.GridSize);
                for (int i = 0; i < selected.Count; i++)
                {
                    Shape n = selected[i].Duplicate();
                    for(int c=0; c<n.lines.Length; c++)
                    {
                        n.lines[c].from += delta;
                        n.lines[c].to += delta;
                    }
                    selected[i] = n;
                    editor.AddShape(n);
                }
                b = Op_Trans;
            }
            operating = b != Op_Sel;
            if (!operating)
            {
                done = false;
                firstChosen = false;
                return;
            }
            operation = b;
            simpleMenu = new SimpleTextMenu(editor);
            originSet = b == Op_Trans;
            if (originSet)
                simpleMenu.SetText("Drag and drop. Press ENTER once done.");
            else
            {
                simpleMenu.SetText("Select Origin");
                /*copyTos = new Vector2[selectedTos.Count];
                copyFroms = new Vector2[selectedFroms.Count];
                for (int i = 0; i < copyTos.Length; i++)
                    copyTos[i] = selectedTos[i].to;
                for (int i = 0; i < copyFroms.Length; i++)
                    copyFroms[i] = selectedFroms[i].from;*/
            }
            editor.SetMenu(simpleMenu);
        }

        private void RecalcShapeVertexes()
        {
            for (int i = 0; i < selected.Count; i++)
                selected[i].RecalcVertex();
        }
    }
}
