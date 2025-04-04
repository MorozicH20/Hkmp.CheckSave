﻿using System;
using System.IO;
using System.Reflection;

namespace Hkmp.CheckSave
{
    public class FileEdit
    {
        private string file;

        public FileEdit()
        {
            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            file = Path.Combine(dllDir ?? string.Empty, "Logs.txt");
        }
        public FileEdit(string path)
        {
            file = path;
        }

        public void Write(string content)
        {
            try
            {
                // Используйте File.AppendAllText для добавления текста в файл
                File.AppendAllText(file, content + Environment.NewLine);
            }
            catch(Exception ex)
            {
                File.AppendAllText(file, ex + Environment.NewLine);
            }
        }
    }
}
