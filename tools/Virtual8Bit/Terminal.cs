using System;
using System.Collections.Generic;

namespace Virtual8Bit
{
    public class Terminal
    {

        #region Public Properties

        public string[] Text { get; private set; } = new string[8];

        public byte CurrentRow { get; private set; } = 0;

        public byte CurrentColumn { get; private set; } = 0;

        public byte KeyboardValue { get; private set; } = 0x00;

        public bool KeyboardInputEnabled { get; set; } = false;

        #endregion

        #region Private Methods

        private void NewLine()
        {
            CurrentColumn = 0;

            if (CurrentRow < 7)
            {
                CurrentRow++;
            }
            else
            {
                for (int i = 0; i <= 6; i++)
                {
                    Text[i] = Text[i + 1];
                }
                Text[7] = "";
            }
        }

        #endregion

        #region Public Methods

        public void AddChar(byte c)
        {
            if (c == '\n')
            {
                NewLine();
            }
            else if (CurrentColumn == 32)
            {
                NewLine();
                Text[CurrentRow] += (char)c;
                CurrentColumn++;
            }
            else
            {
                Text[CurrentRow] += (char)c;
                CurrentColumn++;
            }
        }

        public bool GetKeyboardBitState(byte bitNumber)
        {
            if (bitNumber >= 0 && bitNumber <= 7)
            {
                byte bit = (byte)(0b00000001 << bitNumber);
                return (byte)(KeyboardValue & bit) > 0;
            }
            else
            {
                throw new Exception($"GetKeyboardBitState bit {bitNumber} out of range");
            }
        }

        public void SetKeyboardBitOff(byte bitNumber)
        {
            if (bitNumber >= 0 && bitNumber <= 7)
            {
                byte bit = (byte)((0b11111110 << bitNumber) | (0b11111110 >> 8 - bitNumber));
                KeyboardValue = (byte)(KeyboardValue & bit);
            }
            else
            {
                throw new Exception($"SetKeyboardBitOff bit {bitNumber} out of range");
            }
        }

        public void SetKeyboardBitOn(byte bitNumber)
        {
            if (bitNumber >= 0 && bitNumber <= 7)
            {
                byte bit = (byte)(0x01 << bitNumber);
                KeyboardValue = (byte)(KeyboardValue | bit);
            }
            else
            {
                throw new Exception($"SetKeyboardBitOn bit {bitNumber} out of range");
            }
        }

        public void ToggleKeyboardBit(byte bitNumber)
        {
            if (GetKeyboardBitState(bitNumber))
            {
                SetKeyboardBitOff(bitNumber);
            }
            else
            {
                SetKeyboardBitOn(bitNumber);
            }
        }

        #endregion

        public Terminal()
        {
            for (int i = 0; i <= Text.Length - 1; i++)
            {
                Text[i] = "";
            }
        }
    }
}
