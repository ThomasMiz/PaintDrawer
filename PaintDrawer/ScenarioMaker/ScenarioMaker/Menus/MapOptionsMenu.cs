using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Menus
{
    class MapOptionsMenu : Menu
    {
        Vector2 sep1, sep2;

        JustText sizeXText, sizeYText;
        NumberTextBox sizeX, sizeY;
        bool isTyping = false;

        JustText gridSizeText, backgroundTypeText;
        Button gridPlus, gridMinus;
        Slider gridSize, backgroundType;

        Button save, exit;

        public MapOptionsMenu(Scenes.Editor e)
            : base(e)
        {
            Vector2 zero = Vector2.Zero;
            sizeX = new NumberTextBox(zero, zero, Game1.BlueColor, defIn, e.BoundsWidth, 10, 5000, this);
            sizeY = new NumberTextBox(zero, zero, Game1.BlueColor, defIn, e.BoundsHeight, 10, 5000, this);
            sizeXText = new JustText(zero, zero, "Bounds.X:", 10);
            sizeYText = new JustText(zero, zero, "Bounds.Y:", 10);

            gridSize = new Slider(zero, zero, Game1.BlueColor, Game1.RedColor, 10, 1, 100, editor.GridSize, true);
            gridSizeText = new JustText(zero, zero, "GridSize:", 10);
            gridPlus = new Button(zero, zero, Game1.GreenColor, "Grid++", 10);
            gridMinus = new Button(zero, zero, Game1.RedColor, "Grid--", 10);

            save = new Button(zero, zero, Game1.BlueColor, "Save as", defIn);
            exit = new Button(zero, zero, Game1.RedColor, "Exit to menu", defIn);

            backgroundType = new Slider(zero, zero, Game1.BlueColor, Game1.RedColor, defIn, 1, 3, editor.BackgroundType, true);
            backgroundTypeText = new JustText(zero, zero, "Background:", defIn);
        }

        public override void Update()
        {
            if (!isTyping)
            {
                if (Game1.game.KeyJustClicked(Microsoft.Xna.Framework.Input.Keys.Escape))
                    editor.SetMenu(null);
                else if (gridSize.isSliding)
                {
                    gridSize.UpdateOpen(Game1.game.ms.X);
                    editor.GridSize = (int)gridSize.Value;
                }
                else if (backgroundType.isSliding)
                {
                    backgroundType.UpdateOpen(Game1.game.ms.X);
                    editor.BackgroundType = (byte)backgroundType.Value;
                }
            }
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            sizeX.Draw(batch, mousePos);
            sizeXText.Draw(batch);
            sizeY.Draw(batch, mousePos);
            sizeYText.Draw(batch);
            gridSize.Draw(batch, mousePos);
            gridSizeText.Draw(batch);
            gridPlus.Draw(batch, mousePos);
            gridMinus.Draw(batch, mousePos);
            save.Draw(batch, mousePos);
            exit.Draw(batch, mousePos);
            backgroundTypeText.Draw(batch);
            backgroundType.Draw(batch, mousePos);

            batch.Draw(Game1.white, sep1, null, Color.Black, 0f, new Vector2(0.5f, 0), new Vector2(5, 150), SpriteEffects.None, 0f);
            batch.Draw(Game1.white, sep2, null, Color.Black, 0f, new Vector2(0.5f, 0), new Vector2(5, 150), SpriteEffects.None, 0f);
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (isTyping)
            {
                if (sizeX.isTyping)
                {
                    if (!sizeX.Contains(mousePos))
                        sizeX.OnCharEnter(null, new CharacterEventArgs((char)13, 0));
                }
                else if (sizeY.isTyping)
                {
                    if (!sizeY.Contains(mousePos))
                        sizeY.OnCharEnter(null, new CharacterEventArgs((char)13, 0));
                }
            }

            if (!isTyping)
            {
                if (sizeX.OnClick(mousePos))
                {
                    isTyping = true;
                }
                else if (sizeY.OnClick(mousePos))
                {
                    isTyping = true;
                }
                else if (gridSize.Contains(mousePos))
                {
                    gridSize.isSliding = true;
                }
                else if (gridPlus.Contains(mousePos))
                {
                    gridSize.Value++;
                    editor.GridSize = (int)gridSize.Value;
                }
                else if (gridMinus.Contains(mousePos))
                {
                    gridSize.Value--;
                    editor.GridSize = (int)gridSize.Value;
                }
                else if (backgroundType.Contains(mousePos))
                {
                    backgroundType.isSliding = true;
                }
                else if (save.Contains(mousePos))
                {
                    #region SaveMap
                    if (editor.PlayerCount == 0)
                    {
                        MessageBox.Show("Please enter the starting position of at least one player before saving.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (editor.ScenarioName.Length == 0)
                    {
                        MessageBox.Show("Please enter a valid scenario name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        #region SaveScenario
                        try
                        {
                            SaveFileDialog dial = new SaveFileDialog();
                            dial.AddExtension = true;
                            dial.CheckPathExists = true;
                            dial.DefaultExt = Game1.FileExtension;
                            dial.ShowHelp = false;
                            dial.Filter = Game1.FileFilter;
                            DialogResult res = dial.ShowDialog();
                            if (res == DialogResult.OK || res == DialogResult.Yes)
                            {
                                Stream s = dial.OpenFile();
                                if (s != null)
                                {
                                    int excluded = 0;
                                    for (int i = 0; i < editor.shapes.Count; i++)
                                        if (!editor.shapes[i].IsOk) excluded++;
                                    if (excluded != 0)
                                    {
                                        System.Windows.Forms.DialogResult res1 = System.Windows.Forms.MessageBox.Show(excluded + " Polygons will be excluded from your file because they're invalid. They will remain in the editor but won't be saved in the file so once the editor is closed they will be lost. Proceed saving without these polygons?", "Warning", System.Windows.Forms.MessageBoxButtons.OKCancel);
                                        if (res1 == System.Windows.Forms.DialogResult.Cancel)
                                            return;
                                    }

                                    byte[] data = editor.GetSaveFile();
                                    if (data.Length != 0)
                                    {
                                        s.Write(data, 0, data.Length);
                                        s.Flush();
                                        s.Close();
                                        editor.FileLocation = dial.FileName;
                                    }
                                    s.Dispose();
                                }
                            }
                            dial.Dispose();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Error saving to file:\n" + e.Message);
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (exit.Contains(mousePos))
                {
                    #region Exit
                    DialogResult res = MessageBox.Show("Are you sure you want to exit? Unsaved data will be lost.", "Warning", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                        Game1.game.SetScene(new Scenes.MainMenu());
                    #endregion
                }
            }
        }

        public override void OnResize()
        {
            float size = Game1.HalfWidth / 3f, x = size / 2f;
            float dat = size * (2f / 3f), dat2 = size * (4f / 3f);
            sizeXText.Resize(new Vector2(x, 37.5f), new Vector2(size, 75));
            sizeYText.Resize(new Vector2(x, 112.5f), new Vector2(size, 75));
            x += size;
            sep1.X = x + size / 2;
            sep2.X = sep1.X + size + size;
            sizeX.OnResize(new Vector2(x, 37.5f), new Vector2(size, 75));
            sizeY.OnResize(new Vector2(x, 112.5f), new Vector2(size, 75));
            x += size;

            gridSizeText.Resize(new Vector2(x+dat/2f-size/2f, 25), new Vector2(dat, 50));
            gridPlus.Resize(new Vector2(x + size, 75), new Vector2(size, 50));
            gridMinus.Resize(new Vector2(x, 75), new Vector2(size, 50));
            backgroundTypeText.Resize(new Vector2(x+dat/2f-size/2f , 125), new Vector2(dat, 50));
            x += size;
            gridSize.Resize(new Vector2(x+dat/2f-size/2f, 25f), dat2);
            backgroundType.Resize(new Vector2(x+dat/2f-size/2f, 125f), dat2);
            x += size;
            save.Resize(new Vector2(x + size/2f, 37.5f), new Vector2(size * 2, 75));
            exit.Resize(new Vector2(x + size / 2f, 112.5f), new Vector2(size * 2, 75));
        }

        public override void NumberTextBoxChanged(NumberTextBox box)
        {
            if (box == sizeX)
            {
                editor.SetBoundsSize(sizeX.Value, editor.BoundsHeight);
            }
            else if (box == sizeY)
            {
                editor.SetBoundsSize(editor.BoundsWidth, sizeY.Value);
            }
        }

        public override void NumberTextBoxDone(NumberTextBox box)
        {
            NumberTextBoxChanged(box);
            isTyping = false;
        }
    }
}