using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Scenes
{
    class MainMenu : Scene
    {
        public static void Load(ContentManager con)
        {

        }

        Button newButton, loadButton;
        JustText text;

        public MainMenu()
        {
            newButton = new Button(Vector2.Zero, Vector2.Zero, Game1.PrimaryColor, "New", 25);
            loadButton = new Button(Vector2.Zero, Vector2.Zero, Game1.PrimaryColor, "Load", 25);
            text = new JustText(Vector2.Zero, Vector2.Zero, "ScenarioMaker v1.0", 25);
        }

        public override void Update()
        {
            if (Game1.game.MouseLeftClicked())
            {
                Vector2 mousePos = new Vector2(Game1.game.ms.X, Game1.game.ms.Y);
                if (newButton.Contains(mousePos))
                {
                    Game1.game.SetScene(new Editor());
                }
                else if (loadButton.Contains(mousePos))
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Multiselect = false;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.DefaultExt = Game1.FileExtension;
                    dialog.Filter = Game1.FileLoadFilter;
                    
                    DialogResult res = dialog.ShowDialog();
                    if (res == DialogResult.Yes || res == DialogResult.OK)
                    {
                        FileStream s = new FileStream(dialog.FileName, FileMode.Open);
                        byte[] b = new byte[s.Length-5];
                        if (s.ReadByte() == 222 && s.ReadByte() == 111 && s.ReadByte() == 41 && s.ReadByte() == 231 && s.ReadByte() == 60)
                        {
                            int i = 0;
                            for (i = 0; i < b.Length; i++)
                                b[i] = (byte)s.ReadByte();
                            Editor e;
                            try
                            {
                                String name = dialog.FileName;
                                if (name.EndsWith(".map_autosave"))
                                {
                                    name = name.Substring(0, name.Length - 13) + ".map";
                                }
                                e = new Editor(b, name);
                            }
                            catch
                            {
                                MessageBox.Show("File cannot be read.");
                                s.Close();
                                s.Dispose();
                                return;
                            }
                            Game1.game.SetScene(e);
                            s.Close();
                            s.Dispose();
                        }
                        else
                            MessageBox.Show("Invalid file.");
                    }
                    
                }
            }
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Game1.BackColor);
            Vector2 mousePos = new Vector2(Game1.game.ms.X, Game1.game.ms.Y);

            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null);

            newButton.Draw(batch, mousePos);
            loadButton.Draw(batch, mousePos);
            text.Draw(batch);

            batch.End();
        }

        public override void OnResize()
        {
            float halfHalf = Game1.HalfWidth / 2f;
            float by = Game1.HalfHeight * 1.2f;
            newButton.Resize(new Vector2(halfHalf, by), new Vector2(Game1.HalfWidth, Game1.HalfHeight));
            loadButton.Resize(new Vector2(halfHalf + Game1.HalfWidth, by), new Vector2(Game1.HalfWidth, Game1.HalfHeight));

            float textSizeY = by - Game1.HalfHeight / 2f;
            text.Resize(new Vector2(Game1.HalfWidth, textSizeY / 2f + 20), new Vector2(Game1.ScreenWidth, textSizeY));
        }
    }
}
