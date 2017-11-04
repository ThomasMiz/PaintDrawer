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

    /// <summary>
    /// A Static class containing methods for some default random IActions
    /// </summary>
    static class Actions
    {
        static IAction[] actions;

        /// <summary>
        /// This method should be called when the application starts
        /// </summary>
        /// <param name="font">The CharFont to use to create the IActions</param>
        public static void Init(CharFont font)
        {
            actions = new IAction[]{
                new SimpleWrite(font, "Esta compu ha sido hackeada por la FBI!"),
                new SimpleWrite(font, "Somos Annonymous, hackeamos compus de ort. yey :D"),
                new SimpleWrite(font, "Un programa que escribe solo en paint? Ah ni tikero..."),
                new SimpleWrite(font, "No se me ocurre que mas poner que escriba este programa"),
                new SimpleWrite(font, "Buenos dias a todos los alumnos de ort! Suerte aprobando ranzo."),
                new SimpleWrite(font, "Necesitas clases de matematica? Llama a X:\nX=50x+33sin(50y)-30"),
                new DrawUndistortedChar(font, SimpleWrite.DefaultAt, (char)1),
                new DrawUndistortedChar(font, SimpleWrite.DefaultAt, (char)3),
                new DrawUndistortedChar(font, SimpleWrite.DefaultAt, (char)4),
                new SimpleWrite(font, "Mandenme un mail y escribo lo que me digan!!!"),
                new SimpleWrite(font, "Daaleee, nadie me va a mandar nada? No sean asi...")
                //new SimpleWrite(font, ""),
            };
        }

        static int last = -1;

        /// <summary>
        /// Gets a random action.
        /// </summary>
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
