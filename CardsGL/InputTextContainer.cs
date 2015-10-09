using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardsGL
{
    class InputTextContainer
    {
        public List<Char> TextString { get; set; }
        public int MaxLength { get; set; }

        public InputTextContainer()
        {
            TextString = new List<char>();
            MaxLength = 10;
        }

        public void PutChar(char button)
        {
            if (this.TextString.Count == this.MaxLength)
            {
                this.TextString.Remove(this.TextString.First());
            }

            this.TextString.Add(button);
        }

        public bool Contains(string s)
        {
            string str = this.ToString();

            return str.Contains(s);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (char letter in this.TextString)
            {
                sb.Append(letter);
            }

            return sb.ToString();
        }
    }
}
