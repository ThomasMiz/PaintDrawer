using PaintDrawer.Letters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.Actions
{
    interface IAction
    {
        void Act();
    }

    static class Actions
    {
        static IAction[] actions;

        public static void Init(CharFont font)
        {
            actions = new IAction[]{
                new SimpleWrite(font, "Esta compu ha sido hackeada por la FBI!"),
                new SimpleWrite(font, "Che, por favor NO SIGAN TIC!!!\n\n- Los de 6to :)\n(na, joda!)"),
                new SimpleWrite(font, "Estoy controlando esta compu remotamente desde...\n\nLA VIRGOCUEVA :O", 76),
                new SimpleWrite(font, "Somos Annonymous, hackeamos compus de ort. que desperdicio..."),
                new WriteRemoveWrite(font, "Awante kristina fer...", 76, "naaa jodaaaaaa!\nMacri gato!!!", 76),
                new SimpleWrite(font, "Vieron el ultimo ep. de Rick and Morty? aaajajajaja MAINKRA xddddddddddd"),
                new SimpleWrite(font, "Wooo! Soy el fantasma del presupueeestooo!\nah re que nunca hubo presupuesto..."),
                new SimpleWrite(font, "Un programa que escribe solo en paint? SEEEEEE LO KIERO AHORA MISMO :VVV"),
                new SimpleWrite(font, "No se me ocurre que mas poner que escriba este programa"),
                new SimpleWrite(font, "Buenos dias a todos los alumnos de ort! Ojala que reprueben mate"),
                new SimpleWrite(font, "Necesitas clases de matematica? Llama a X:\nX=50x+33sin(50y)-30"),
                new DrawUndistortedChar(font, new Vec2(50, 170), (char)2),
                //new SimpleWrite(font, ""),
            };
        }

        static int last = -1;

        public static IAction RandomAction
        {
            get
            {
                //return actions[actions.Length - 1];

                if (actions.Length == 1)
                    return actions[0];

                int index;
                do
                {
                    index = Stuff.r.Next(actions.Length);
                } while (index == last);

                last = index;
                return actions[index];
            }
        }
    }
}
