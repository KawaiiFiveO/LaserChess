using System;
using System.Windows.Forms;

namespace LaserChessBot
{
    internal static class Program
    {
        [STAThread]
        private static int Main()
        {
            // TGGameClass only enters TestHarness mode when the process has
            // no command-line arguments, so the bot takes none.
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                Console.WriteLine("LaserChessBot takes no arguments. Start LaserChessServer, then run LaserChessBot.exe.");
                return 2;
            }

            using (Bot bot = new Bot())
            {
                bot.Start();
                Application.Run();
                return bot.ExitCode;
            }
        }
    }
}
