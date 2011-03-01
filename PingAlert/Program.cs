using System;

using System.Collections.Generic;
using System.Windows.Forms;

namespace PingAlert
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(String[] args)
        {
            Application.Run(new Form1(args));
        }
    }
}