using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogisimCpuAssembler
{
    public static class Basic8Bit
    {
        public static byte[] programBytes = new byte[0xff];

        public static byte programCounter = 0;

        public static byte variableCounter = 0;

        public static List<string> variables = new List<string>();

        public static Dictionary<string, byte> strings = new Dictionary<string, byte>();

        public static Dictionary<string, byte> Registers = GetRegisterList();

        public static Dictionary<string, byte> JumpLabels = new Dictionary<string, byte>();

        public static Dictionary<string, byte> GetRegisterList()
        {
            Dictionary<string, byte> registers = new Dictionary<string, byte>();

            registers.Add("IP", 0b1000);
            registers.Add("R0", 0b0100);
            registers.Add("R1", 0b1100);
            registers.Add("R2", 0b0010);
            registers.Add("R3", 0b1010);
            registers.Add("AR", 0b0110);
            registers.Add("SP", 0b1110);
            registers.Add("MP", 0b0001);
            registers.Add("TC", 0b0011);

            return registers;
        }

        public static void AddByteToProgram(byte newByte)
        {
            if (programCounter < 0xFF)
            {
                programBytes[programCounter] = newByte;
                programCounter++;
            }
            else
            {
                throw new Exception("Program out of memory!");
            }
        }

        public static void ProcessLines(List<string> lines)
        {
            foreach(string line in lines)
            {
                Console.WriteLine($"Assembling {line}");

                string[] lexes = line.Split(' ');

                if (lexes[0] == "$VAR")
                {
                    ProcessVar(lexes);
                }
                else if (lexes[0].Replace(" ", "").StartsWith(":", StringComparison.CurrentCulture))
                {
                    ProcessJumpLabel(lexes);
                }
                else if (lexes[0].Replace(" ", "").StartsWith("#", StringComparison.CurrentCulture))
                {
                    SetProgramCounter(lexes[0]);
                }
                else 
                {
                    ProcessOpCommand(lexes);
                }
            }
        }

        public static void ProcessVar(string[] command) 
        {
            string name = command[1];

            byte value = (byte)((command.Length == 3) ? Convert.ToByte(command[2], 16): 0);

            variables.Add(name);

            programBytes[(0xff - variableCounter) - 1] = value;

            variableCounter++;
        }

        public static void ProcessJumpLabel(string[] command)
        {
            JumpLabels.Add(command[0], programCounter);
        }

        public static void ProcessOpCommand(string[] command)
        {
            switch (command[0])
            {
                case "SET":
                    {
                        ProcessSetCommand(command);
                        break;
                    }
                case "ADD":
                    {
                        ProcessAddCommand(command);
                        break;
                    }
                case "MUL":
                    {
                        ProcessMulCommand(command);
                        break;
                    }
                case "WRA":
                    {
                        ProcessWraCommand(command);
                        break;
                    }
                case "LRA":
                    {
                        ProcessLraCommand(command);
                        break;
                    }
                case "CMP":
                    {
                        ProcessCmpCommand(command);
                        break;
                    }
                case "JET":
                    {
                        ProcessJetCommand(command);
                        break;
                    }
                case "JGT":
                    {
                        ProcessJgtCommand(command);
                        break;
                    }
                case "JLT":
                    {
                        ProcessJltCommand(command);
                        break;
                    }
                case "":
                    {
                        break;
                    }
                default:
                    {
                        throw new Exception($"Error unknown command {command[0]}");
                    }
            }
        }

        public static void ProcessSetCommand(string[] command)
        {
            byte opcode = 0b00000001;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            byte value = 0;

            if (command[2].StartsWith("$", StringComparison.CurrentCulture))
            {
                value = GetVariableAddress(command[2]);
            }
            else if (command[2].StartsWith("#", StringComparison.CurrentCulture))
            {
                value = Convert.ToByte(command[2].Replace("#", ""), 16);
            }
            else if (command[2].StartsWith(":", StringComparison.CurrentCulture))
            {
                value = GetJumpLabelAddress(command[2]);
            }
            else
            {
                value = Convert.ToByte(command[2]);
            }
            //byte value = (command[2].StartsWith("$", StringComparison.Ordinal)) ? GetVariableAddress(command[2]) : Convert.ToByte(command[2]);

            AddByteToProgram(value);
        }

        public static void ProcessAddCommand(string[] command)
        {
            byte opcode = 0b00001000;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            AddByteToProgram(0x00);
        }

        public static void ProcessWpmCommand(string[] command)
        {
            byte opcode = 0b00001010;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            byte value = Convert.ToByte(command[2]);

            AddByteToProgram(value);
        }

        public static void ProcessMulCommand(string[] command)
        {
            byte opcode = 0b00001010;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            AddByteToProgram(0x00);
        }

        public static void ProcessWraCommand(string[] command)
        {
            byte opcode = 0b00001110;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            byte address = 0x00;

            //if (command[2].StartsWith("$", StringComparison.CurrentCulture))
            //{
            //    address = GetVariableAddress(command[2]);
            //}
            //else
            //{
            //    address = Convert.ToByte(command[2], 16);
            //}

            AddByteToProgram(address);
        }

        public static void ProcessLraCommand(string[] command)
        {
            byte opcode = 0b00000111;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            //byte value = Convert.ToByte(command[2]);

            AddByteToProgram(0x00);
        }

        public static void ProcessCmpCommand(string[] command)
        {
            byte opcode = 0b00001100;

            byte register = Registers[command[1]];

            opcode = (byte)((register << 4) + opcode);

            AddByteToProgram(opcode);

            AddByteToProgram(0x00);
        }

        public static void ProcessJetCommand(string[] command)
        {
            byte opcode = 0b00001001;

            AddByteToProgram(opcode);

            byte value = 0;

            if (command[1].StartsWith("$", StringComparison.CurrentCulture))
            {
                value = GetVariableAddress(command[1]);
            }
            else if (command[1].StartsWith("#", StringComparison.CurrentCulture))
            {
                value = Convert.ToByte(command[1].Replace("#", ""), 16);
            }
            else if (command[1].StartsWith(":", StringComparison.CurrentCulture))
            {
                value = GetJumpLabelAddress(command[1]);
            }
            else
            {
                value = Convert.ToByte(command[1]);
            }

            AddByteToProgram(value);
        }

        public static void ProcessJgtCommand(string[] command)
        {
            byte opcode = 0b00001011;

            AddByteToProgram(opcode);

            byte value = 0;

            if (command[1].StartsWith("$", StringComparison.CurrentCulture))
            {
                value = GetVariableAddress(command[1]);
            }
            else if (command[1].StartsWith("#", StringComparison.CurrentCulture))
            {
                value = Convert.ToByte(command[1].Replace("#", ""), 16);
            }
            else if (command[1].StartsWith(":", StringComparison.CurrentCulture))
            {
                value = GetJumpLabelAddress(command[1]);
            }
            else
            {
                value = Convert.ToByte(command[1]);
            }

            AddByteToProgram(value);
        }

        public static void ProcessJltCommand(string[] command)
        {
            byte opcode = 0b00001101;

            AddByteToProgram(opcode);

            //byte value = Convert.ToByte(command[1]);

            byte value = 0;

            if (command[1].StartsWith("$", StringComparison.CurrentCulture))
            {
                value = GetVariableAddress(command[1]);
            }
            else if (command[1].StartsWith("#", StringComparison.CurrentCulture))
            {
                value = Convert.ToByte(command[1].Replace("#", ""), 16);
            }
            else if (command[1].StartsWith(":", StringComparison.CurrentCulture))
            {
                value = GetJumpLabelAddress(command[1]);
            }
            else
            {
                value = Convert.ToByte(command[1]);
            }

            AddByteToProgram(value);
        }

        public static byte GetJumpLabelAddress(string labelName)
        {
            byte address = 0;

            if (JumpLabels.TryGetValue(labelName, out address) == false)
            {
                throw new Exception($"Jump Label {labelName} does not exist!");
            }

            return address;
        }

        public static byte GetVariableAddress(string lex)
        {
            string name = lex.Replace("$", "");

            int address = 0;
            byte txtAddress = 0;

            if (variables.Contains(name))
            {
                address = variables.IndexOf(name);
                address = (0xff - address) - 1;
            }
            else if (strings.TryGetValue(name, out txtAddress))
            {
                return txtAddress;
            }
            else
            {
                Console.WriteLine($"Error variable {name} does not exist!");
                throw new Exception($"Error variable {name} does not exist!");
            }

            return (byte)address;
        }

        public static void SetProgramCounter(string lex)
        {
            lex = lex.Replace("#", "");

            byte address = Convert.ToByte(lex, 16);

            if (address <= 0xff)
            {
                programCounter = address;
            }
        }

        public static void ExportLogisimRaw(string directory)
        {
            StreamWriter streamWriter = new StreamWriter($"{directory}/output.rom");

            streamWriter.WriteLine("v2.0 raw");

            int n = 0;

            string lineOut = "";

            for (int i = 0; i <= 0xff - 1; i++)
            {
                byte opCode = programBytes[i];
                lineOut += opCode.ToString("x");

                if (n == 7)
                {
                    streamWriter.WriteLine(lineOut);
                    lineOut = "";
                    n = 0;
                }
                else
                {
                    lineOut += " ";
                    n++;
                }
            }

            streamWriter.WriteLine(lineOut);

            streamWriter.Close();
        }

        public static void ExportBin(string directory)
        {
            File.WriteAllBytes($"{directory}/output.bin", programBytes);
        }

        public static void Assemble(string file)
        {
            FileInfo fileInfo = new FileInfo(file);

            string directory = fileInfo.DirectoryName;

            List<string> programLines = new List<string>();

            StreamReader streamReader = new StreamReader(file);

            while(streamReader.EndOfStream == false)
            {
                string line = streamReader.ReadLine().ToUpper().Replace("  ", " ");
                if (line != "" && line != " ")
                {
                    programLines.Add(line);
                }
            }

            ProcessLines(programLines);

            ExportLogisimRaw(directory);

            ExportBin(directory);
        }

    }
}

