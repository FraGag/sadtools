//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                NativeMethods.WM_TASKBARBUTTONCREATED = NativeMethods.RegisterWindowMessage("TaskbarButtonCreated");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            finally
            {
                Properties.Settings.Default.Save();
            }
        }
    }
}
