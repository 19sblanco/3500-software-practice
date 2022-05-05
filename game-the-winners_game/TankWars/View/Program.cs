using GC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
/// <summary>
/// Authors: Steven Blanco and Israel Prescott
/// CS 3500
/// PS9
/// </summary>
namespace View
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            View theView = new View();
            Application.Run(theView);
        }
    }
}
