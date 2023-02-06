using System;
using System.Threading;
using System.Windows.Forms;

namespace WindowsInterface
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Exception);
            Application.Run(new Form1());
        }

        /// <summary> 
        ///     Глобальный <b>try/catch</b>. Мотод обрабатывает все оставшиеся ошибки, которое не прописаны в коде блоком <b>try/catch/finally</b>. <br/>
        ///     Например если поток удалил строку в <b>Event_Stack</b>, а <b>GUI</b> пытается дорисовать интерфейс удалённой строки.
        /// </summary>
        private static void Exception(object sender, ThreadExceptionEventArgs e) {
            MessageBox.Show("Global error!\nНепредвиденная ошибка. Может возникнуть например:\n" +
                "- если <b>GUI</b> обращается к удалённой строке стека событий потоком;\n" +
                "Во всех этих случаях в коде try/catch-ами не облепишься." +
                $"\n\nЗаголовок:\n {e.Exception.Message}" +
                $"\n\n::Подробный текст ошибки::\nStackTrace:\n{e.Exception.StackTrace}\n" +
                $"TargetSite.Name:\n{e.Exception.TargetSite.Name}");
        }
    }
}
