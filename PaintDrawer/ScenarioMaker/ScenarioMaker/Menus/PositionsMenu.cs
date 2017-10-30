using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScenarioMaker.Menus
{
    class PositionsMenu : Menu
    {
        PlayerButtonCombo[] buttons;
        Vector2[] separators;
        Vector2 separatorSize;

        Button powerpos;

        public PositionsMenu(Scenes.Editor e) : base(e)
        {
            buttons = new PlayerButtonCombo[8];
            for (int i = 0; i < buttons.Length; i++)
                buttons[i] = new PlayerButtonCombo(i, this);
            buttons[0].set.Enabled = true;
            separators = new Vector2[4];

            powerpos = new Button(Vector2.Zero, Vector2.Zero, Game1.BlueColor, "PowerUp\nPosition", 10);
        }

        public override void Update()
        {
            if (Game1.game.KeyJustClicked(Microsoft.Xna.Framework.Input.Keys.Escape))
                editor.SetMenu(null);
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].Draw(batch, mousePos);
            for (int i = 0; i < separators.Length; i++)
                batch.Draw(Game1.white, separators[i], null, Color.Black, 0f, Vector2.Zero, separatorSize, SpriteEffects.None, 0f);

            powerpos.Draw(batch, mousePos);
        }

        public override void OnClick(Vector2 mousePos)
        {
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].OnClick(mousePos);
            if (powerpos.Contains(mousePos))
                editor.SetTool(new Tools.SetPowerupPosTool(editor, this));
        }

        public override void OnResize()
        {
            float buttonAreaWid = Game1.ScreenWidth * 0.8f;
            float buttonAreaX = Game1.ScreenWidth - buttonAreaWid;
            Vector2 buttonSize = new Vector2(buttonAreaWid / 4f, 75);
            separatorSize = new Vector2(3, 150);
            float halfSS = separatorSize.X * 0.5f;

            separators[0] = new Vector2(buttonAreaX - halfSS, 0);
            buttons[0].Resize(new Vector2(buttonAreaX, 0), buttonSize);
            buttons[1].Resize(new Vector2(buttonAreaX, 75), buttonSize);
            buttonAreaX += buttonSize.X;
            separators[1] = new Vector2(buttonAreaX - halfSS, 0);
            buttons[2].Resize(new Vector2(buttonAreaX, 0), buttonSize);
            buttons[3].Resize(new Vector2(buttonAreaX, 75), buttonSize);
            buttonAreaX += buttonSize.X;
            separators[2] = new Vector2(buttonAreaX - halfSS, 0);
            buttons[4].Resize(new Vector2(buttonAreaX, 0), buttonSize);
            buttons[5].Resize(new Vector2(buttonAreaX, 75), buttonSize);
            buttonAreaX += buttonSize.X;
            separators[3] = new Vector2(buttonAreaX - halfSS, 0);
            buttons[6].Resize(new Vector2(buttonAreaX, 0), buttonSize);
            buttons[7].Resize(new Vector2(buttonAreaX, 75), buttonSize);
            buttonAreaX += buttonSize.X;

            float restX = Game1.ScreenWidth - buttonAreaWid;
            powerpos.Resize(new Vector2(restX * 0.5f, 75), new Vector2(restX, 150));
        }

        public void OnSetPress(int index)
        {
            editor.SetTool(new Tools.SetPlayerPosTool(editor, this, index));
        }
        public void OnCrossPress(int index)
        {
            buttons[index].cross.Enabled = false;
            editor.playersEnabled[index] = false;
        }
        public void OnPlayerPosSet(Vector2 pos, int index)
        {
            editor.playersEnabled[index] = true;
            editor.playersPos[index] = pos;
            buttons[index].cross.Enabled = true;
            editor.SetMenu(this);
        }
        public void OnPowerupPosSet(Vector2 pos)
        {
            editor.powerupPos = pos;
            editor.SetMenu(this);
        }
    }

    struct PlayerButtonCombo
    {
        public Button set, cross;
        public int index;
        PositionsMenu belong;
        public PlayerButtonCombo(int index, PositionsMenu belong)
        {
            this.belong = belong;
            this.index = index;
            set = new Button(new Vector2(), new Vector2(), Game1.BlueColor, "Player " + (index+1), 6);
            cross = new Button(new Vector2(), new Vector2(), Game1.RedColor, "X", 6);
            cross.Enabled = Stuff.editor.playersEnabled[index];
        }

        public void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            set.Draw(batch, mousePos);
            cross.Draw(batch, mousePos);
        }

        public void OnClick(Vector2 mousePos)
        {
            if (set.Contains(mousePos))
            {
                belong.OnSetPress(index);
            }
            else if (cross.Contains(mousePos))
            {
                belong.OnCrossPress(index);
            }
        }

        public void Resize(Vector2 topLeft, Vector2 size)
        {
            Vector2 setSize = new Vector2(size.X * 0.8f, size.Y);
            set.Resize(topLeft + setSize * 0.5f, setSize);

            Vector2 crossSize = new Vector2(size.X * 0.2f, size.Y);
            cross.Resize(new Vector2(topLeft.X + setSize.X + crossSize.X * 0.5f, topLeft.Y + size.Y * 0.5f), crossSize);
        }
    }
}
