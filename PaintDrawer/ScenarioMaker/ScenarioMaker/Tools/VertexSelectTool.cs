using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScenarioMaker.Menus;

namespace ScenarioMaker.Tools
{
    class VertexSelectTool : Tool
    {
        public const byte Op_Trans = 0, Op_Scal = 1, Op_Rot = 2, Op_Sel = 3, Op_ToShape = 4;

        bool somethingChosen = false;
        byte operation;
        bool originSet, operating = false;
        float dat, dat2lol;
        Vector2 from, to, opOrigin;
        List<Line> selectedFroms, selectedTos;
        List<Shape> shapes;
        Vector2[] copyFroms, copyTos;
        SimpleTextMenu simpleMenu;
        
        bool done = false, firstChosen = false;

        public VertexSelectTool(Scenes.Editor e)
            : base(e)
        {
            selectedFroms = new List<Line>(50);
            selectedTos = new List<Line>(50);
            shapes = new List<Shape>(25);
            editor.MouseText = "vertexSelect";
        }

        public override void Update()
        {
            if (operating)
            {
                #region Operating
                if (operation == Op_Trans)
                {
                    #region Translate
                    Vector2 delta = editor.MousePos - editor.OldMousePos;
                    if (Game1.game.ms.LeftButton == ButtonState.Pressed && editor.MouseOnEditor())
                    {
                        for (int i = 0; i < selectedFroms.Count; i++)
                        {
                            Vector2 f = selectedFroms[i].from + delta;
                            selectedFroms[i].from = f;
                        }

                        for (int i = 0; i < selectedTos.Count; i++)
                        {
                            Vector2 t = selectedTos[i].to + delta;
                            selectedTos[i].to = t;
                        }

                    }
                    if (Game1.game.KeyJustClicked(Keys.Enter))
                        FinishOp();
                    RecalcShapeVertex();
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
                            float mult = Vector2.Distance(opOrigin, editor.MousePos) / 250f - dat + 1;
                            for (int i = 0; i < selectedFroms.Count; i++)
                            {
                                Vector2 f = copyFroms[i] - opOrigin;
                                f *= mult;
                                f += opOrigin;
                                selectedFroms[i].from = f;
                            }

                            for (int i = 0; i < selectedTos.Count; i++)
                            {
                                Vector2 t = copyTos[i] - opOrigin;
                                t *= mult;
                                t += opOrigin;
                                selectedTos[i].to = t;
                            }
                            RecalcShapeVertex();
                            #endregion
                        }
                        else if (operation == Op_Rot)
                        {
                            #region Rotate
                            float current = (float)Math.Atan2(editor.MousePos.Y - opOrigin.Y, editor.MousePos.X - opOrigin.X) - dat2lol;
                            if (Game1.game.ks.IsKeyDown(Keys.LeftShift) || Game1.game.ks.IsKeyDown(Keys.RightShift))
                                current -= ((current + MathHelper.PiOver4) % Stuff.PiOver8);
                            float deltaRot = current - dat;
                            for (int i = 0; i < selectedFroms.Count; i++)
                            {
                                Vector2 f = selectedFroms[i].from;
                                float rot = (float)Math.Atan2(f.Y - opOrigin.Y, f.X - opOrigin.X), dist = Vector2.Distance(f, opOrigin);
                                rot += deltaRot;
                                f.X = (float)Math.Cos(rot) * dist + opOrigin.X;
                                f.Y = (float)Math.Sin(rot) * dist + opOrigin.Y;
                                selectedFroms[i].from = f;
                            }

                            for (int i = 0; i < selectedTos.Count; i++)
                            {
                                Vector2 f = selectedTos[i].to;
                                float rot = (float)Math.Atan2(f.Y - opOrigin.Y, f.X - opOrigin.X), dist = Vector2.Distance(f, opOrigin);
                                rot += deltaRot;
                                f.X = (float)Math.Cos(rot) * dist + opOrigin.X;
                                f.Y = (float)Math.Sin(rot) * dist + opOrigin.Y;
                                selectedTos[i].to = f;
                            }
                            dat = current;
                            RecalcShapeVertex();
                            #endregion
                        }
                    }
                }
                #endregion
            }
        }

        void FinishOp()
        {
            operating = true;
            originSet = false;
            editor.SetMenu(new VertexSelectMenu(editor, this));
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, editor.cameraView);
            Color color = Game1.BlueColor;
            color.A = 64;
            if (somethingChosen)
            {
                for (int i = 0; i < selectedFroms.Count; i++)
                    Game1.game.DrawCircleAt(selectedFroms[i].from, color, editor.oneOverScale);

                for (int i = 0; i < selectedTos.Count; i++)
                    Game1.game.DrawCircleAt(selectedTos[i].to, color, editor.oneOverScale);
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
                //batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, editor.cameraView);
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
                #region Operating
                if (originSet)
                {
                    if (operation == Op_Scal)
                    {
                        for (int i = 0; i < copyTos.Length; i++)
                            copyTos[i] = selectedTos[i].to;
                        for (int i = 0; i < copyFroms.Length; i++)
                            copyFroms[i] = selectedFroms[i].from;
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
                #endregion
            }
            else if (done)
            {

            }
            else if (firstChosen)
            {
                somethingChosen = true;
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
                    {
                        bool addshape = false;
                        for (int c = 0; c < s.lines.Length; c++)
                        {
                            if (Contains(s.lines[c].from) && !selectedFroms.Contains(s.lines[c]))
                            {
                                selectedFroms.Add(s.lines[c]);
                                addshape = true;
                            }

                            if (Contains(s.lines[c].to) && !selectedTos.Contains(s.lines[c]))
                            {
                                selectedTos.Add(s.lines[c]);
                                addshape = true;
                            }
                        }
                        if (addshape && !shapes.Contains(s))
                            shapes.Add(s);
                    }
                }

                editor.SetMenu(new VertexSelectMenu(editor, this));

                done = true;
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
            if (b == Op_ToShape)
            {
                List<Shape> shapes = new List<Shape>(30);
                for (int i = 0; i < selectedTos.Count; i++)
                    if (!shapes.Contains(selectedTos[i].belongTo))
                        shapes.Add(selectedTos[i].belongTo);
                for (int i = 0; i < selectedFroms.Count; i++)
                    if (!shapes.Contains(selectedFroms[i].belongTo))
                        shapes.Add(selectedFroms[i].belongTo);
                    editor.SetTool(new ShapeSelectTool(editor, shapes));
                return;
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
                copyTos = new Vector2[selectedTos.Count];
                copyFroms = new Vector2[selectedFroms.Count];
                for (int i = 0; i < copyTos.Length; i++)
                    copyTos[i] = selectedTos[i].to;
                for (int i = 0; i < copyFroms.Length; i++)
                    copyFroms[i] = selectedFroms[i].from;
            }
            editor.SetMenu(simpleMenu);
        }

        private void RecalcShapeVertex()
        {
            for (int i = 0; i < shapes.Count; i++)
                shapes[i].RecalcVertex();
        }
    }
}
