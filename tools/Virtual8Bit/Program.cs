using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Virtual8Bit
{
    class MainClass
    {
        private enum CpuState { HALT, RUN, STEP }

        private static BackgroundWorker worker;
        private static Cpu cpu;
        private static byte[] displayMemory;
        private static int sleepMs = 10;
        private static CpuState cpuState = CpuState.HALT;

        private static void PrintTerminal()
        {
            Console.WriteLine("Terminal Output");
            foreach (string s in cpu.Terminal.Text)
            {
                Console.Write("                                ");              //Clear any text on current terminal line
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(s);
            }

            Console.WriteLine("Terminal Inpit");
            byte bit = 8;
            while (bit != 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("|");
                bit--;
                if (cpu.Terminal.GetKeyboardBitState((byte)(bit)))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.Write("{0:000}", 0b00000001 << bit);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("|HEX:{0:x2}|", cpu.Terminal.KeyboardValue);
            Console.Write("|DEC:{0:000}|", cpu.Terminal.KeyboardValue);

            if (cpu.Terminal.KeyboardInputEnabled)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" ON ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" OFF");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintRegisters()
        {
            Console.WriteLine("Registers");

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"IR:");
            Console.Write("{0:x2}|", cpu.IR);
            Console.BackgroundColor = ConsoleColor.Black;

            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"IP:");
            Console.Write("{0:x2}|", cpu.IP);
            Console.BackgroundColor = ConsoleColor.Black;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"MP:");
            Console.Write("{0:x2}|", cpu.MP);
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"AR:");
            Console.Write("{0:x2}|", cpu.AR);
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write($"R0:");
            Console.Write("{0:x2}|", cpu.R0);

            Console.Write($"R1:");
            Console.Write("{0:x2}|", cpu.R1);

            Console.Write($"R2:");
            Console.Write("{0:x2}|", cpu.R2);

            Console.Write($"R3:");
            Console.Write("{0:x2}|", cpu.R3);

            Console.Write($"TC:");
            Console.Write("{0:x2}|", cpu.TC);

            if (cpu.LessThan)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write("<");
            Console.ForegroundColor = ConsoleColor.White;

            if (cpu.EqualTo)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write("=");
            Console.ForegroundColor = ConsoleColor.White;

            if (cpu.GraterThan)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write(">");
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine();
        }

        private static void PrintMemory()
        {
            Console.WriteLine("Memory");

            int pointer = 0;

            for (int row = 0; row <= 7; row++)
            {
                for (int column = 0; column <= 31; column++)
                {
                    if (pointer == cpu.IP)
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    if (pointer == cpu.MP)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    Console.Write("{0:x2}", cpu.Memory[pointer]);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write(" ");
                    pointer++;
                }
                Console.WriteLine();
            }
        }

        private static void PrintState()
        {
            if (cpuState == CpuState.HALT)
            {
                Console.WriteLine("HALT");
            }
            else if (cpuState == CpuState.RUN)
            {
                Console.WriteLine($"RUN Sleep:{sleepMs}");
            }
        }

        private static void Print()
        {
            Console.SetCursorPosition(0, 0);
            PrintTerminal();
            PrintRegisters();
            PrintMemory();
            PrintState();
        }

        private static bool LoadBin(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileInfo fileInfo = new FileInfo(fileName);

                if (fileInfo.Length > 255)
                {
                    Console.WriteLine("LoadBin failed, file to large");
                    return false;
                }
                else
                {
                    byte[] fileBytes = File.ReadAllBytes(fileName);

                    for (int i = 0; i <= fileBytes.Length - 1; i++)
                    {
                        cpu.Memory[i] = fileBytes[i];
                    }
                    return true;
                }
            }
            else
            {
                Console.WriteLine($"{fileName} Does not exist");
                return false;
            }
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //cpuState = CpuState.RUN;
            Print();
            while (worker.CancellationPending == false)
            {
                if (cpuState == CpuState.RUN)
                {
                    Print();
                    cpu.Step();
                    Thread.Sleep(sleepMs);
                }
                else
                {
                    Thread.Sleep(10); //Slow down thread when CPU halted 
                }
            }
            e.Cancel = true;
        }

        public static void Main(string[] args)
        {
            Console.Clear();
            displayMemory = new byte[256];
            cpu = new Cpu();


            if (args.Length == 0)
            {
                Console.WriteLine("No file provided");
                return;
            }

            if (LoadBin(args[0]) == false)
            {
                return;
            }

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;

            worker.RunWorkerAsync();

            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                {
                    worker.CancelAsync();
                    break;
                }

                if (key == ConsoleKey.Add)
                {
                    if (sleepMs > 1)
                    {
                        sleepMs--;
                    }
                }

                if (key == ConsoleKey.Subtract)
                {
                    sleepMs++;
                }

                if (key == ConsoleKey.H)
                {
                    cpuState = CpuState.HALT;
                }

                if (key == ConsoleKey.R)
                {
                    cpuState = CpuState.RUN;
                }

                if (key == ConsoleKey.S && cpuState != CpuState.RUN)
                {
                    Print();
                    cpu.Step();
                    while (Console.KeyAvailable && key == ConsoleKey.S)
                    {
                        key = Console.ReadKey(true).Key;                        
                    }
                }

                if (key == ConsoleKey.K)
                {
                    cpu.Terminal.KeyboardInputEnabled = !cpu.Terminal.KeyboardInputEnabled;
                }

                if (key == ConsoleKey.D0)
                {
                    cpu.Terminal.ToggleKeyboardBit(0);
                }

                if (key == ConsoleKey.D9)
                {
                    cpu.Terminal.ToggleKeyboardBit(1);
                }

                if (key == ConsoleKey.D8)
                {
                    cpu.Terminal.ToggleKeyboardBit(2);
                }

                if (key == ConsoleKey.D7)
                {
                    cpu.Terminal.ToggleKeyboardBit(3);
                }

                if (key == ConsoleKey.D6)
                {
                    cpu.Terminal.ToggleKeyboardBit(4);
                }

                if (key == ConsoleKey.D5)
                {
                    cpu.Terminal.ToggleKeyboardBit(5);
                }

                if (key == ConsoleKey.D4)
                {
                    cpu.Terminal.ToggleKeyboardBit(6);
                }

                if (key == ConsoleKey.D3)
                {
                    cpu.Terminal.ToggleKeyboardBit(7);
                }

                Thread.Sleep(1);
            }
        }
    }
}
