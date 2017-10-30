using System;
using System.Collections.Generic;

namespace ScenarioMaker.Menus
{
    abstract class Menu
    {
        public const int defIn = 12;
        protected Scenes.Editor editor;

        public Menu(Scenes.Editor editor)
        {
            this.editor = editor;
        }

        public abstract void Update();
        public abstract void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, Microsoft.Xna.Framework.Vector2 mousePos);
        public abstract void OnResize();
        public abstract void OnClick(Microsoft.Xna.Framework.Vector2 mousePos);

        public virtual void NumberTextBoxChanged(NumberTextBox box) { }
        public virtual void NumberTextBoxDone(NumberTextBox box) { }
        public virtual void TextBoxDone(TextBox box) { }
    }
}
