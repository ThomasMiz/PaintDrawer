using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker.Menus
{
    class VerticalToolMenu : Menu
    {
        ToggleButton showGrid, clampMouse, guidesButton, clampVertex;
        Button mapSett, mapPos, mapName;
        public bool ShowGrid { get { return showGrid.Value; } set { showGrid.Value = value; } }
        public bool ClampMouse { get { return clampMouse.Value; } set { clampMouse.Value = value; } }
        public bool ShowGuides { get { return guidesButton.Value; } set { guidesButton.Value = value; } }
        public bool ClampVertex { get { return clampVertex.Value; } set { clampVertex.Value = value; } }

        public VerticalToolMenu(Scenes.Editor e) : base(e)
        {
            showGrid = new ToggleButton(new Vector2(50, 50), new Vector2(100, 100), "Grid", 10);
            clampMouse = new ToggleButton(new Vector2(50, 150), new Vector2(100, 100), "Clamp\nGrid", 10);
            guidesButton = new ToggleButton(new Vector2(50, 250), new Vector2(100, 100), " Show\nGuides", 10);
            clampVertex = new ToggleButton(new Vector2(50, 350), new Vector2(100, 100), "Clamp\nVertex", 10);
            mapSett = new Button(new Vector2(50, 450), new Vector2(100, 100), Game1.BlueColor, "  Map\nSettings", 10);
            mapPos = new Button(new Vector2(50, 550), new Vector2(100, 100), Game1.BlueColor, "   Map\nPositions", 10);
            mapName = new Button(new Vector2(50, 650), new Vector2(100, 100), Game1.BlueColor, "Map\nName", 10);
        }

        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch batch, Vector2 mousePos)
        {
            showGrid.Draw(batch, mousePos);
            clampMouse.Draw(batch, mousePos);
            guidesButton.Draw(batch, mousePos);
            clampVertex.Draw(batch, mousePos);
            mapSett.Draw(batch, mousePos);
            mapPos.Draw(batch, mousePos);
            mapName.Draw(batch, mousePos);
        }

        public override void OnClick(Vector2 mousePos)
        {
            if (showGrid.Contains(mousePos))
            {
                showGrid.OnClick();
            }
            else if (clampMouse.Contains(mousePos))
            {
                clampMouse.OnClick();
            }
            else if (guidesButton.Contains(mousePos))
            {
                guidesButton.OnClick();
            }
            else if (clampVertex.Contains(mousePos))
            {
                clampVertex.OnClick();
            }
            else if (mapSett.Contains(mousePos))
            {
                editor.SetTool(null);
                editor.SetMenu(new MapOptionsMenu(editor));
            }
            else if (mapPos.Contains(mousePos))
            {
                editor.SetTool(null);
                editor.SetMenu(new PositionsMenu(editor));
            }
            else if (mapName.Contains(mousePos))
            {
                editor.SetTool(null);
                editor.SetMenu(new MapNameMenu(editor));
            }
        }

        public override void OnResize()
        {
            float sizey = Game1.ScreenHeight - 150;
            sizey /= 7;
            if (sizey > 100)
                sizey = 100;
            Vector2 size = new Vector2(100, sizey);

            float y = sizey / 2;
            showGrid.Resize(new Vector2(50, y), size);
            y += sizey;
            clampMouse.Resize(new Vector2(50, y), size);
            y += sizey;
            guidesButton.Resize(new Vector2(50, y), size);
            y += sizey;
            clampVertex.Resize(new Vector2(50, y), size);
            y += sizey;
            mapSett.Resize(new Vector2(50, y), size);
            y += sizey;
            mapPos.Resize(new Vector2(50, y), size);
            y += sizey;
            mapName.Resize(new Vector2(50, y), size);
        }
    }
}
