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
                CreateSimpleWrite(font, "Esta compu ha sido hackeada por la FBI!"),
                CreateSimpleWrite(font, "Somos Annonymous, hackeamos compus de ort. yey :D"),
                CreateSimpleWrite(font, "Un programa que escribe solo en paint? Ah ni tikero..."),
                CreateSimpleWrite(font, "No se me ocurre que mas poner que escriba este programa"),
                CreateSimpleWrite(font, "Buenos dias a todos los alumnos de ort! Suerte aprobando ranzo."),
                CreateSimpleWrite(font, "Necesitas clases de matematica? Llama a X:\nX=50x+33sin(50y)-30"),
                new DrawUndistortedChar(font, (char)1),
                new DrawUndistortedChar(font, (char)3),
                new DrawUndistortedChar(font, (char)4),
                CreateSimpleWrite(font, "Mandenme un mail y escribo lo que me digan!!!"),
                CreateSimpleWrite(font, "Daaleee, nadie me va a mandar nada? No sean asi...")
                //CreateSimpleWrite(font, ""),
            };
        }

        /// <summary>
        /// Creates a SimpleWrite action with the specified CharFont and text. The size is
        /// calculated based on needed drawing area but wont exceed SimpleWrite.DefaultSize
        /// </summary>
        public static SimpleWrite CreateSimpleWrite(CharFont font, String text)
        {
            // suck it, optimization. Let's just fucking try lowering the size slightly until it's good enough.
            float size = SimpleWrite.DefaultSize;
            while (!SimpleWrite.IsSizeOk(Program.font, text, SimpleWrite.DefaultAt, size))
                size -= 2;
            return new SimpleWrite(Program.font, text, size);
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
