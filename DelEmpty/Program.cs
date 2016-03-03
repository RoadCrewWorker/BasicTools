using System;
using System.IO;

namespace DelEmpty
{
    class Program
    {
        static void Process(string folder, StreamWriter w)
        {
            Console.WriteLine("now: " + folder);

            foreach (string f in Directory.GetFiles(folder))
            {
                if (File.Exists(f) && (new FileInfo(f)).Length == 0)
                {
                    File.Delete(f);
                    Console.WriteLine("F: " + f);
                    w.WriteLine(f);
                }
            }

            foreach (string dir in Directory.GetDirectories(folder))
            {
                try
                {
                    Process(dir, w);
                }
                catch (Exception e)
                {
                    Console.WriteLine("E: " + dir + " = " + e.ToString());
                }

                if (Directory.GetDirectories(dir).Length == 0 && Directory.GetFiles(dir).Length == 0)
                {
                    Directory.Delete(dir, false);
                    Console.WriteLine("D: " + dir);
                    w.WriteLine(dir);
                }
            }
        }
        static void Main(string[] args)
        {
            StreamWriter w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/del.log", true);
            try
            {
                Process(args[0], w);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Done.");
            Console.ReadKey();
            w.Close();
        }
    }
}
