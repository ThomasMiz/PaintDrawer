using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker
{
    public abstract class Scene
    {
        public abstract void Update();
        public abstract void Draw(SpriteBatch batch, GraphicsDevice device);
        public abstract void OnResize();
        public virtual void OnExit() { }
    }
}
