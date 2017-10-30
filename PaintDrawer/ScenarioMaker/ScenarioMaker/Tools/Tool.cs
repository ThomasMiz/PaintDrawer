using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Tools
{
    abstract class Tool
    {
        protected Scenes.Editor editor;
        public Tool(Scenes.Editor e)
        {
            editor = e;
        }

        public abstract void Update();
        public abstract void OnClick(Vector2 mousePos);
        public abstract void Draw(SpriteBatch batch, GraphicsDevice device);
        public abstract void OnExit();
    }
}
