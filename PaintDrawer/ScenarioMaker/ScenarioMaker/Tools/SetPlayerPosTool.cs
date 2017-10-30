using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Tools
{
    class SetPlayerPosTool : Tool
    {
        Menus.PositionsMenu menu;
        int index;

        public SetPlayerPosTool(Scenes.Editor e, Menus.PositionsMenu menu, int index)
            : base(e)
        {
            this.index = index;
            this.menu = menu;
            editor.MouseText = "player " + (index+1) + " position";
        }

        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch batch, GraphicsDevice device)
        {
            Stuff.DrawPlayerIndicator(editor.MousePos, Game1.BlueColor, index);
        }

        public override void OnClick(Vector2 mousePos)
        {
            editor.SetTool(null);
            menu.OnPlayerPosSet(mousePos, index);
            
        }

        public override void OnExit()
        {
            
        }
    }
}
