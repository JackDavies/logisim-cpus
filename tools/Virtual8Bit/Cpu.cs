using System;
namespace Virtual8Bit
{

    #region Internal Classes

    public static class OpCodes
    {
        public const byte NOP = 0b0000;
        public const byte SET = 0b0001;
        public const byte LRA = 0b0111;
        public const byte ADD = 0b1000;
        public const byte CMP = 0b1100;
        public const byte JLT = 0b1101;
        public const byte JGT = 0b1011;
        public const byte JET = 0b1001;
        public const byte WRA = 0b1110;
    }

    public static class RegisterCodes
    {
        public const byte IR = 0b0000;
        public const byte MP = 0b0001;
        public const byte IP = 0b1000;
        public const byte TC = 0b0011;
        public const byte AR = 0b0110;
        public const byte R0 = 0b0100;
        public const byte R1 = 0b1100;
        public const byte R2 = 0b0010;
        public const byte R3 = 0b1010;
    }

    #endregion

    public class Cpu
    {

        #region Feilds

        bool _ipModified = false;

        #endregion

        #region Properties

        public byte[] Registers { get; private set; } = new byte[13];

        public byte[] Memory { get; private set; }

        public byte[] TerminalMemory { get; private set; }
        public byte TerminalPosition { get; private set; } = 0x00;

        public bool EqualTo { get; private set; } = false;
        public bool GraterThan  { get; private set; } = false;
        public bool LessThan { get; private set; } = false;

        public byte IR => Registers[RegisterCodes.IR];
        public byte MP => Registers[RegisterCodes.MP];
        public byte IP => Registers[RegisterCodes.IP];
        public byte AR => Registers[RegisterCodes.AR];
        public byte R0 => Registers[RegisterCodes.R0];
        public byte R1 => Registers[RegisterCodes.R1];
        public byte R2 => Registers[RegisterCodes.R2];
        public byte R3 => Registers[RegisterCodes.R3];
        public byte TC => Registers[RegisterCodes.TC];

        #endregion

        #region Private Methods

        private void SetRegister(byte register, byte value)
        {
            if (register >= 0 && register < Registers.Length)
            {
                Registers[register] = value;
            }
            else
            {
                throw new Exception($"SetRegister {register} out of range");
            }
        }

        private void Set()
        {
            byte register = (byte)(Registers[RegisterCodes.IR] >> 4);
            //TickIp();
            Registers[register] = Memory[Registers[RegisterCodes.IP]];

            if (register == RegisterCodes.IP)
            {
                _ipModified = true;
            }
        }

        private void Wra()
        {
            byte register = (byte)(Registers[RegisterCodes.IR] >> 4);
            //TickIp();
            Memory[Registers[RegisterCodes.MP]] = Registers[register];
        }

        private void Lra()
        {
            byte register = (byte)(Registers[RegisterCodes.IR] >> 4);
            //TickIp();
            Registers[register] = Memory[Registers[RegisterCodes.MP]];

            if (register == RegisterCodes.IP)
            {
                _ipModified = true;
            }
        }

        private void Cmp()
        {
            byte register = (byte)(Registers[RegisterCodes.IR] >> 4);
            //TickIp();
            EqualTo = (Registers[RegisterCodes.AR] == Registers[register]);
            GraterThan = ((uint)Registers[RegisterCodes.AR] > (uint)Registers[register]);
            LessThan = ((uint)Registers[RegisterCodes.AR] < (uint)Registers[register]);
        }

        private void Add()
        {
            byte register = (byte)(Registers[RegisterCodes.IR] >> 4);
            //TickIp();
            Registers[RegisterCodes.AR] += Registers[register];
        }

        private void Jet()
        {
            //TickIp();
            if (EqualTo)
            {
                Registers[RegisterCodes.IP] = Memory[Registers[RegisterCodes.IP]];
                _ipModified = true;
            }
        }

        private void Jgt()
        {
            //TickIp();
            if (GraterThan)
            {
                Registers[RegisterCodes.IP] = Memory[Registers[RegisterCodes.IP]];
                _ipModified = true;
            }
        }

        private void Jlt()
        {
            //TickIp();
            if (LessThan)
            {
                Registers[RegisterCodes.IP] = Memory[Registers[RegisterCodes.IP]];
                _ipModified = true;
            }
        }

        private void Interperate()
        {
            Registers[RegisterCodes.IR] = Memory[Registers[RegisterCodes.IP]];
            TickIp();

            switch (Registers[RegisterCodes.IR] & 0b00001111)
            {
                case OpCodes.SET:
                    Set();
                    break;

                case OpCodes.WRA:
                    Wra();
                    break;

                case OpCodes.LRA:
                    Lra();
                    break;

                case OpCodes.CMP:
                    Cmp();
                    break;

                case OpCodes.ADD:
                    Add();
                    break;

                case OpCodes.JET:
                    Jet();
                    break;

                case OpCodes.JGT:
                    Jgt();
                    break;

                case OpCodes.JLT:
                    Jlt();
                    break;
            }

            UpdateTerminalMemory();
        }

        private void UpdateTerminalMemory()
        {
            if (Registers[RegisterCodes.TC] != 0x00)
            { 
                TerminalMemory[TerminalPosition] = Registers[RegisterCodes.TC];
                TerminalPosition++;
                Registers[RegisterCodes.TC] = 0x00;
            }
        }

        private void TickIp()
        {
            Registers[RegisterCodes.IP]++;
        }

        #endregion

        #region Public Methods

        public void Step()
        {
            Interperate();
            if (_ipModified == false)
            {
                TickIp();
            }
            _ipModified = false;
        }

        #endregion

        #region Constructors

        public Cpu()
        {
            Memory = new byte[256];
            for (int i = 0; i <= Memory.Length - 1; i++)
            {
                Memory[i] = 0x00;
            }

            TerminalMemory = new byte[256];
            for (int i = 0; i <= TerminalMemory.Length - 1; i++)
            {
                TerminalMemory[i] = 0x00;
            }
        }

        #endregion
    }
}
