using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageMetaScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Gets a list of path names

            Console.WriteLine("Skip entries (default 0):");
            int i = 0;
            Int32.TryParse(Console.ReadLine(), out i);

            Console.WriteLine("Enter 'n' for pathcheck only...");
            bool g_pathcheck = (Console.ReadLine() != "n");

            Console.WriteLine("Enter 'n' to skip md5 generation...");
            bool g_md5 = (Console.ReadLine() != "n");

            Console.WriteLine("Enter 'n' to skip res generation...");
            bool g_res = (Console.ReadLine() != "n");

            foreach(string arg in args)
            {
                string[] files = File.ReadAllLines(arg);
                Console.WriteLine("Queued " + files.Length + " files...");

                string outputf = arg + "_meta.csv";
                StringBuilder buffer = new StringBuilder();
                MD5 h = MD5.Create();
                DateTime lastdt = DateTime.Now;
                for (; i < files.Length; i++)
                {
                    string file = files[i];
                    //Writes:
                    //file: md5, size, creation time, last change time
                    //image: type, dimension, bpp, frames
                    try
                    {
                        FileInfo f = new FileInfo(file);
                        //File info:
                        buffer.Append(file).Append('\t');

                        if (g_pathcheck && f.Exists)
                        {
                            if (g_md5)
                            {
                                using (FileStream fst = File.OpenRead(file))
                                {
                                    byte[] hb = h.ComputeHash(fst);
                                    fst.Close();
                                    for (int j = 0; j < hb.Length; j++)
                                    {
                                        buffer.Append(hb[j].ToString("x2"));
                                    }
                                    buffer.Append('\t');
                                }
                            }
                            buffer.Append(f.Length).Append('\t');
                            buffer.Append(f.CreationTimeUtc.Ticks).Append('\t');
                            buffer.Append(f.LastWriteTimeUtc.Ticks).Append('\t');
                            buffer.Append(f.LastAccessTimeUtc.Ticks).Append('\t');

                            if (g_res)
                            {
                                using (Image img = Image.FromFile(file))
                                {
                                    buffer.Append(img.Width).Append('\t');
                                    buffer.Append(img.Height).Append('\t');
                                    buffer.Append(img.HorizontalResolution).Append('\t');
                                    buffer.Append(img.VerticalResolution);
                                    img.Dispose();
                                }
                            }
                        }

                        buffer.AppendLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(i + "/" + files.Length + " " + file + " => " + ex.ToString());
                    }

                    if (i % 1000 == 0)
                    {

                        StreamWriter w = File.AppendText(outputf);
                        w.Write(buffer.ToString());
                        w.Flush();
                        w.Close();

                        buffer = new StringBuilder();
                        Console.WriteLine(i + "/" + files.Length + " " + (DateTime.Now - lastdt).ToString());
                        lastdt = DateTime.Now;
                    }
                }
            }
        }
    }
}
