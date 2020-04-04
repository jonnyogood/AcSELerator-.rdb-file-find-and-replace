using System;
using System.IO;
using System.Text;

namespace SET_C.TXT
{
    class SCM_autoedit
    {
        [STAThread]
        public static void Main(string[] args)
        {
            int blocks = 64;
            int[] position = new int[blocks];
            int[] size = new int[blocks];
            string line;
            string directory = Directory.GetCurrentDirectory();
            Console.WriteLine(directory);
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@"C:\Users\jgood\Desktop\Relays");
                System.IO.FileInfo[] SCNfileNames = di.GetFiles("SCN*.txt");
                System.IO.FileInfo[] fileNames = di.GetFiles("*.rdb");
                foreach (System.IO.FileInfo fi in fileNames)
                {
                    int optimized = (int)fi.Length;
                    using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        byte[] buffer = new byte[optimized];
                        Array.Resize(ref buffer, optimized);
                        stream.Read(buffer, 0, optimized);
                        string rdb = new UTF8Encoding().GetString(buffer);
                        int n = blocks - 1;
                        do
                        {
                            position[n] = rdb.LastIndexOf("\u0000\u0000\u0000\u0000\u0000[INFO]", StringComparison.Ordinal) + 7;
                            size[n] = rdb.Length - position[n];
                            rdb = rdb.Remove(position[n]);
                            Console.WriteLine(position[n]/64 + "  " + size[n]/64);
                            n--;
                        } while (position[n + 1] > 6 && n >= 0);
                        foreach (System.IO.FileInfo SCNfi in SCNfileNames)
                        {
                            using (StreamReader sr = new StreamReader(SCNfi.FullName))
                            {
                                while ((line = sr.ReadLine()) != null)
                                {
                                    string[] words = line.Split(';');
                                    for (int i = 0; i < blocks; i++)
                                    {
                                        if (position[i] > 5)
                                        ApplySCN(stream, words, size[i], position[i], buffer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        private static void ApplySCN(FileStream stream, string[] words, int size, int position, byte[] buffer)
        {
            try
            {
                stream.Position = position;
                Array.Resize(ref buffer, size);
                stream.Read(buffer, 0, size);
                Encoding utf8 = new UTF8Encoding();
                string sgroup = utf8.GetString(buffer);
                if (sgroup.Contains("\u003D\u0001\u0000\u0000\u003E\u0001\u0000\u0000\u003F\u0001\u0000\u0000\u0040\u0001\u0000\u0000", StringComparison.Ordinal))
                {
                    size -= 512;
                    Array.Resize(ref buffer, size);
                }
                if (sgroup.Contains(words[0]) && sgroup.Contains(words[1]))
                {
                    int offset = sgroup.IndexOf(words[0]);
                    sgroup = sgroup.Substring(offset);
                    int offset2 = sgroup.IndexOf(words[1]);
                    if (offset2 != -1)
                    {
                        offset += offset2;
                        sgroup = sgroup.Substring(offset2);
                        int end = sgroup.LastIndexOf("\u0000\u0000\u0000\u0000\u0000");
                        int append = size - offset - words[1].Length;
                        for (int i = 0; i < append; i++)
                            buffer[i] = buffer[i + size - append];
                        if (end != -1 && end > size - 512)
                            Array.Resize(ref buffer, end);
                        else
                            Array.Resize(ref buffer, append + words[1].Length - words[2].Length);
                        stream.Position = position + offset;
                        byte[] newwords = utf8.GetBytes(words[2]);
                        stream.Write(newwords, 0, newwords.Length);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in method: " + e.Message);
            }
        }
    }
}