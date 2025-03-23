using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Hkmp.CheckSave
{
    public class FileEdit
    {
        private string file = ;

        public FileEdit()
        {
            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            file = Path.Combine(dllDir ?? string.Empty, "Logs.txt");
        }
        public FileEdit(string path)
        {
            file = path;
        }

        public void Write(string text)
        {
            if (file != null)
            {
                using (StreamWriter W = new StreamWriter(file))
                {
                    W.WriteLine(text);
                }
            }
        }
    }
