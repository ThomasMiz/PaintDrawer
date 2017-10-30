using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Menus
{
    class MapNameMenu : Menu
    {
        JustText text;
        TextBox box;

        public MapNameMenu(Scenes.Editor e)
            : base(e)
        {
            text = new JustText(Vector2.Zero, Vector2.Zero, "Map Name: (max. 32 characters)", 5);
            box = new TextBox(Vector2.Zero, Vector2.Zero, Game1.BlueColor, 5, editor.ScenarioName, 32, this);
        }

        public override void Update()
        {
            if (!box.isTyping && Game1.game.ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                editor.SetMenu(null);
        }

        public override void OnClick(Vector2 mousePos)
        {
            box.OnClick(mousePos);
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            text.Draw(batch);
            box.Draw(batch, mousePos);
        }

        public override void TextBoxDone(TextBox box)
        {
            editor.ScenarioName = box.Value;
        }

        public override void OnResize()
        {
            Vector2 size = new Vector2(Game1.ScreenWidth, 75);
            Vector2 center = new Vector2(Game1.HalfWidth, 38.5f);
            text.Resize(center, size);
            center.Y = 110f;
            box.OnResize(center, size);

        }
    }
}
