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
