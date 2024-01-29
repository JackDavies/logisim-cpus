using System;
using System.IO;

namespace LogisimCpuAssembler
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Basic 8-bit assembler");

                if (args.Length == 0)
                {
                    Console.WriteLine("No file provided");
                    return;
                }

                string filename = args[0];

                if (File.Exists(filename))
                {
                    Console.WriteLine($"File to assemble: {filename}");
                    Basic8Bit.Assemble(filename);
                }
                else
                {
                    Console.WriteLine($"{filename} does not exist!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
