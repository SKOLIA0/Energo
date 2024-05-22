using DotNetEnv;
using System;
using System.Windows.Forms;

namespace Energo
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Загрузка переменных окружения из .env файла
            Env.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
