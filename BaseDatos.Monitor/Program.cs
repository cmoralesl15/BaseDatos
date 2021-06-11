using System;
using System.Configuration;
using System.IO;

namespace BaseDatos.Monitor
{
    class Program
    {
        static void Main()
        {
            using (var watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["folder"]))
            {
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += OnChanged;
                watcher.Filter = ConfigurationManager.AppSettings["file"].ToString();
                watcher.EnableRaisingEvents = true;
                Console.WriteLine("Monitor de transacciones");
                Console.ReadLine();
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            try
            {
                string[] lines = System.IO.File.ReadAllLines($@"{ConfigurationManager.AppSettings["folder"]}\{ConfigurationManager.AppSettings["file"]}");
                Console.WriteLine(lines[lines.Length - 1]);
            }
            catch (Exception)
            {
            }
        }
    }
}
