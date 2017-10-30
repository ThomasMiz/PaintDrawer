using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Tools
{
    class SetPowerupPosTool : Tool
    {
        Menus.PositionsMenu menu;

        public SetPowerupPosTool(Scenes.Editor e, Menus.PositionsMenu menu)
            : base(e)
        {
            this.menu = menu;
            editor.MouseText = "powerup position";
        }

        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            Stuff.DrawPowerupIndicator(editor.MousePos, Game1.BlueColor);
        }

        public override void OnClick(Vector2 mousePos)
        {
            editor.SetTool(null);
            menu.OnPowerupPosSet(mousePos);
        }

        public override void OnExit()
        {
            
        }
    }
}
