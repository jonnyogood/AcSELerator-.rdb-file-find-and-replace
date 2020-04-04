using System;
using System.IO;

namespace SET_C.TXT
{
    class Set_C_autoedit
    {
        [STAThread]
        static void Main(string[] args)
        {
            string line;
            try
            {
                StreamReader sr = new StreamReader("C:\\Users\\jgood\\Desktop\\SEL-651R-2_Calibration_Settings_2_25_2020 1_04_43 PM.txt");
                line = sr.ReadLine();
                line = sr.ReadLine();
                line = sr.ReadLine();
                line = sr.ReadLine();
                line = sr.ReadLine();
                line = sr.ReadLine();
                line = line.Replace(":= ", ",\"");
                line = line.Replace(":=", ",\"");
                while (line.Contains(" ,"))
                {
                    line = line.Replace(" ,", ",");
                }
                while (line.Contains("  "))
                {
                    line = line.Replace("  ", " ");
                }
                string[] write = line.Split(' ');
                string folder = write[2];
                foreach (var word in write)
                {
                    if (word.Contains(',') && word.Contains("SNUMB"))
                    {
                        folder = word;
                    }
                }
                folder = folder.Replace("SNUMB,\"", "");
                Directory.CreateDirectory("C:\\Users\\jgood\\Desktop\\" + folder);
                StreamWriter sw = new StreamWriter("C:\\Users\\jgood\\Desktop\\" + folder + "\\SET_C.txt");
                sw.WriteLine("[INFO]");
                sw.WriteLine("RELAYTYPE=0651R");
                sw.WriteLine("FID=SEL-651R-2-R406-V2-Z006003-D20190308");
                sw.WriteLine("BFID=SLBT-3CF1-R200-V0-Z100100-D20120321");
                sw.WriteLine("PARTNO=0651R2ADXGA8AE2123LX10");
                sw.WriteLine("[C]");
                while (line != null)
                {
                    line = line.Replace(":= ", ",\"");
                    line = line.Replace(":=", ",\"");
                    while (line.Contains(" ,"))
                    {
                        line = line.Replace(" ,", ",");
                    }
                    while (line.Contains("  "))
                    {
                        line = line.Replace("  ", " ");
                    }
                    write = line.Split(' ');
                    foreach (var word in write)
                    {
                        if (word.Contains(',') && !word.Contains("TRPPULA3,") && !word.Contains("CLSPULA3,") && !word.Contains("DSBL4STFL,"))
                        {
                            sw.WriteLine($"{word}" + "\"");
                        }
                    }
                    line = sr.ReadLine();
                }
                sw.WriteLine("");
                sr.Close();
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
}