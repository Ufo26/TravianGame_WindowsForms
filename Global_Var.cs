
using System.Drawing;

namespace Global_Var
{
    static public class Global
    {
        /// <summary> Разрешение экрана по горизонтали. </summary>
        static public int Screen_Width;
        /// <summary> Разрешение экрана по вертикали. </summary>
        static public int Screen_Height;
        /// <summary> Разрешение рабочей области без ПУСКА по горизонтали </summary>
        static public int ScreenArea_Width;
        /// <summary> Разрешение рабочей области без ПУСКА по вертикали </summary>
        static public int ScreenArea_Height;
        /// <summary> Размер главного окна по горизонтали. </summary>
        static public int MainWindowWidth;
        /// <summary> Размер главного окна по вертикали. </summary>
        static public int MainWindowHeight;
        /// <summary> Высота полосы (шапки) окна, где [свернуть, развернуть, закрыть] справа вверху. </summary>
        //static public int KrestBar_Height = (int)(Screen.PrimaryScreen.WorkingArea.Size.Height - Form.ClientSize.Height);
        /// <summary> <b> _1920: </b> константное значение. Ширина экрана разработки приложения. </summary>
        public const int _1920 = 1920;
        /// <summary> <b> _1080: </b> константное значение. Высота экрана разработки приложения. </summary>
        public const int _1080 = 1080;
        /// <summary> параметр для таймера. число означает из какой функции пришли и что делать </summary>
        static public int timer1_param;
        /// <summary> Размер карты. </summary>
        static public Size SizeMap = new Size(10, 10);
    }
}
