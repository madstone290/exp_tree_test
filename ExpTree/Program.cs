﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpTree
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { name = "ajck"});
            ExpressionPlayground.Run();
            Application.Run(new Form1());
        }
    }
}
