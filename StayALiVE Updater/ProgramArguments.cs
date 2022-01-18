using System;
using System.Collections.Generic;
using System.Linq;

namespace StayALiVE_Updater
{
    internal class ProgramArguments
    {
        public bool Silent { get; private set; } = false;
        public bool Purge { get; private set; } = false;
        public bool Restart { get; private set; } = false;

        private void SetArgument(string argument)
        {
            //Console.WriteLine($"Arg: {argument}\n");
            switch (argument)
            {
                case "-silent": Silent = true; break;
                case "-purge": Purge = true; break;
                case "-restart": Restart = true; break;
            }
        }

        internal ProgramArguments(List<string> arguments)
        {
            if (arguments.Any())
                arguments.ForEach(SetArgument);
        }
    }
}
