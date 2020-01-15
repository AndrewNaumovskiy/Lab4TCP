using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;

namespace Lab4
{
    public static class Logger
    {
        public static void Log(string filePath, string line)
        {
            if (!File.Exists(filePath))
                File.Create(filePath);

            using (Stream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            { 
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine(line);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }
    }
}
