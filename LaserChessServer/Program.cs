using System;
using System.Collections;
using System.Text;

namespace LaserChessServer
{
    internal static class Program
    {
        private const string Usage =
@"LaserChessServer - local stand-in for the TrayGames LaserChess server.

Usage: LaserChessServer.exe [--bot] [--name1 NAME] [--name2 NAME] [--verbose]

  --bot          Start LaserChessBot.exe as player 2 when player 1 joins.
  --name1 NAME   Name shown for player 1 (moves first). Default: Player 1
  --name2 NAME   Name shown for player 2. Default: Player 2
  --verbose      Log every message sent and received.

Start this first or after, then run LaserChess.exe twice with no arguments
(once with --bot). The first window to connect is player 1.";

        private static int Main(string[] args)
        {
            string name0 = "Player 1";
            string name1 = "Player 2";
            bool launchBot = false;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg == "--name1" && i + 1 < args.Length)
                {
                    name0 = args[++i];
                }
                else if (arg == "--name2" && i + 1 < args.Length)
                {
                    name1 = args[++i];
                }
                else if (arg == "--verbose" || arg == "-v")
                {
                    Log.VerboseEnabled = true;
                }
                else if (arg == "--bot")
                {
                    launchBot = true;
                }
                else
                {
                    Console.WriteLine(Usage);
                    return arg == "--help" || arg == "-h" || arg == "/?" ? 0 : 2;
                }
            }

            using (HarnessServer server = new HarnessServer(name0, name1, launchBot))
            {
                Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    server.Stop();
                };
                try
                {
                    return server.Run();
                }
                catch (Exception ex)
                {
                    Log.Error("Fatal: {0}", ex);
                    return 1;
                }
            }
        }
    }

    internal static class Log
    {
        private static readonly object Sync = new object();

        public static bool VerboseEnabled { get; set; }

        public static void Info(string format, params object[] args)
        {
            Write(ConsoleColor.Gray, format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            Write(ConsoleColor.Yellow, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Write(ConsoleColor.Red, format, args);
        }

        public static void Verbose(string format, params object[] args)
        {
            if (VerboseEnabled)
            {
                Write(ConsoleColor.DarkGray, format, args);
            }
        }

        /// <summary>Single-line dump of a message Hashtable.</summary>
        public static string Describe(Hashtable table)
        {
            if (table == null)
            {
                return "(null)";
            }
            StringBuilder text = new StringBuilder("{");
            bool first = true;
            foreach (DictionaryEntry entry in table)
            {
                if (!first)
                {
                    text.Append(", ");
                }
                first = false;
                text.Append(entry.Key).Append('=');
                byte[] bytes = entry.Value as byte[];
                Hashtable nested = entry.Value as Hashtable;
                if (bytes != null)
                {
                    text.Append("byte[").Append(bytes.Length).Append(']');
                }
                else if (nested != null)
                {
                    text.Append(Describe(nested));
                }
                else
                {
                    text.Append(entry.Value);
                }
            }
            return text.Append('}').ToString();
        }

        private static void Write(ConsoleColor color, string format, object[] args)
        {
            string line = DateTime.Now.ToString("HH:mm:ss") + "  " + string.Format(format, args);
            lock (Sync)
            {
                ConsoleColor previous = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = previous;
            }
        }
    }
}
