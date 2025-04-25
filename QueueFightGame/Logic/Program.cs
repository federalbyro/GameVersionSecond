using System;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace QueueFightGame.UI // Assuming UI code is in this namespace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Полные имена или псевдоним
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new MainMenuForm());
        }
    }
}