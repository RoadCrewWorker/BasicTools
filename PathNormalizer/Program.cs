using System;
using System.IO;

namespace PathNormalizer
{
    class Program
    {
        static void TryMove(FileInfo fi, FileInfo fi_norm, StreamWriter w)
        {
            if (!fi_norm.Directory.Exists)
            {
                Directory.CreateDirectory(fi_norm.DirectoryName);
                Console.WriteLine(fi_norm.DirectoryName + "\t" + fi_norm + "\tcreated_dir");
                w.WriteLine(fi_norm.DirectoryName + "\t" + fi_norm + "\tcreated_dir");
            }
            try
            {
                File.Move(fi.FullName, fi_norm.FullName);
                Console.WriteLine(fi + "\t" + fi_norm + "\tsuccess");
                w.WriteLine(fi + "\t" + fi_norm + "\tsuccess");
            }
            catch (Exception e)
            {
                Console.WriteLine(fi + "\t" + fi_norm + "\tError:" + e.Message);
                w.WriteLine(fi + "\t" + fi_norm + "\tError:" + e.Message);
            }
        }
        static void Process(string folder, StreamWriter w)
        {
            Console.WriteLine("now: " + folder);

            foreach (string f in Directory.GetFiles(folder))
            {
                string f_norm = f.ToLowerInvariant().Replace(' ', '_');
                if (f == f_norm) //Problem with uppercase Dirs: move does nothing, but f!=f_norm
                {
                    continue;
                }

                FileInfo fi = new FileInfo(f);
                FileInfo fi_norm = new FileInfo(f_norm);

                if (fi.Exists)
                {
                    if (fi_norm.Exists)
                    {
                        w.WriteLine(fi + "\t" + fi_norm + "\texists");
                        if (fi.Length == fi_norm.Length && fi.Length > 0)
                        {
                            string f_norm_dup = f_norm.Replace(":\\", ":\\duplicates\\");
                            if (f_norm == f_norm_dup)
                            {
                                continue;
                            }
                            FileInfo fi_norm_dup = new FileInfo(f_norm_dup);
                            TryMove(fi, fi_norm_dup, w);
                            Console.WriteLine(fi + "\t" + fi_norm_dup + "\tequal_m:" + fi.Length);
                            w.WriteLine(fi + "\t" + fi_norm_dup + "\tequal_m:" + fi.Length);
                        }
                    }
                    else
                    {
                        TryMove(fi, fi_norm, w);
                    }
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
                    Console.WriteLine(dir + "\t" + folder + "\tempty_del");
                    w.WriteLine(dir + "\t" + folder + "\tempty_del");
                }
            }
        }

        static void Main(string[] args)
        {
            StreamWriter w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/norm.log", true);
            try
            {
                Process(args[0].ToLowerInvariant().Replace(' ', '_'), w);
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
