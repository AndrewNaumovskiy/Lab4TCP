using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;

namespace Lab4
{
    public static class ClassIO
    {
        public static void Log(string filePath, string line)
        {
            if (!File.Exists(filePath))
                File.Create(filePath);

            using (Stream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine($"{line}");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        private static int indexFromFile = -1;

        public static int GetNextRndNumber()
        {
            if (!File.Exists(Program.RandomFilePath))
                File.Create(Program.RandomFilePath);
            List<int> intArray = new List<int>();
            using (Stream stream = new FileStream(Program.RandomFilePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    var stringArray = streamReader.ReadToEnd().Split(' ');
                    foreach (var item in stringArray)
                    {
                        intArray.Add(Convert.ToInt32(item));
                    }
                }
            }

            if (indexFromFile > intArray.Count)
            {
                Random rand = new Random();
                return rand.Next(0, 100);
            }
            indexFromFile++;
            return intArray[indexFromFile];
        }
    }
}
