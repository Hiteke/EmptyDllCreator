using System;
using System.IO;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace EmptyDllCreator
{
    internal class Program
    {
        public static string Output { get; set; }
        public static string[] Files { get; set; }

        static void Work()
        {
            foreach (var filename in Files)
            {
                Console.WriteLine(filename);

                ModuleContext modCtx = ModuleDef.CreateModuleContext();
                ModuleDefMD module = ModuleDefMD.Load(filename, modCtx);

                if (!module.IsILOnly)
                    continue;

                foreach (var type in module.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        method.Body = new CilBody();
                    }
                }

                module.Write(Path.Combine(Output, Path.GetFileName(filename)));
            }
        }

        static bool SelectFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Dll files|*.dll";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Files = ofd.FileNames;
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool SelectOutput()
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                Output = fbd.SelectedPath;
                return true;
            }
            else
            {
                return false;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            if(!SelectFiles())
            {
                Console.WriteLine("Error: Select dll files");
                return;
            }

            if (!SelectOutput())
            {
                Console.WriteLine("Error: Select the output folder");
                return;
            }

            Work();

            Console.WriteLine("Сompleted!");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
