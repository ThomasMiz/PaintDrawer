using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Menus
{
    class SimpleTextMenu : Menu
    {
        JustText text;

        public SimpleTextMenu(Scenes.Editor e)
            : base(e)
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            text.Draw(batch);
        }

        public override void OnClick(Vector2 mousePos)
        {
            
        }

        public override void OnResize()
        {
            text.Resize(new Vector2(Game1.HalfWidth, 75), new Vector2(Game1.ScreenWidth, 150));
        }

        public void SetText(String t)
        {
            text = new JustText(new Vector2(Game1.HalfWidth, 75), new Vector2(Game1.ScreenWidth, 150), t, 10);
        }
    }
}
