// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharpDataProcessor.Commands;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharpDataProcessor
{
    /// <summary>
    /// Parsers commands given to this data processor.
    /// </summary>
    public static class CommandParser
    {
        /// <summary>
        /// Holds a list of supported commands.
        /// </summary>
        private static List<Command> _commands = CommandLoader.LoadCommands();

        /// <summary>
        /// Parses the command line arguments into a sorted list of commands.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Command[] ParseCommands(string[] args)
        {
            // initialize the list of commands.
            var commands = new List<Command>();

            // start parsing arguments one-by-one.
            int idx = 0;
            while (idx < args.Length)
            { // check the next argument for a switch.
                if (!CommandParser.IsSwitch(args[idx]))
                {
                    throw new CommandLineParserException(args[idx], "Invalid switch!");
                }

                // parse the command.
                Command command;
                int eatenArguments = CommandParser.ParseCommand(args, idx, out command);

                // increase idx.
                idx = idx + eatenArguments;

                // add resulting command.
                commands.Add(command);
            }

            return commands.ToArray();
        }

        /// <summary>
        /// Parses one commands and returns the number of eaten arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static int ParseCommand(string[] args, int idx, out Command command)
        {
            int eatenArguments = 1; // eat the first argument.
            string switchCommand = args[idx]; // get the switch command.

            // make sure no caps mess things up.
            switchCommand = switchCommand.ToLower();
            switchCommand = switchCommand.Trim();

            // loop over commands and try to parse.
            foreach(var current in _commands)
            {
                if(current.HasSwitch(switchCommand))
                { // this command has the be the one!
                    eatenArguments = eatenArguments +
                                     current.Parse(args, idx + 1, out command);
                    return eatenArguments;
                }
            }
            throw new CommandLineParserException(args[idx], "Switch not found!");
        }

        /// <summary>
        /// Remove quotes from strings if they occur at exactly the beginning and end.
        /// </summary>
        /// <param name="stringToParse"></param>
        /// <returns></returns>
        public static string RemoveQuotes(string stringToParse)
        {
            if (string.IsNullOrEmpty(stringToParse))
            {
                return stringToParse;
            }

            if (stringToParse.Length < 2)
            {
                return stringToParse;
            }

            if (stringToParse[0] == '"' && stringToParse[stringToParse.Length - 1] == '"')
            {
                return stringToParse.Substring(1, stringToParse.Length - 2);
            }

            if (stringToParse[0] == '\'' && stringToParse[stringToParse.Length - 1] == '\'')
            {
                return stringToParse.Substring(1, stringToParse.Length - 2);
            }

            return stringToParse;
        }

        /// <summary>
        /// Returns true if the given string can be a switch.
        /// </summary>
        /// <param name="switchCommand"></param>
        /// <returns></returns>
        public static bool IsSwitch(string switchCommand)
        {
            // make sure no caps mess things up.
            switchCommand = switchCommand.ToLower();
            switchCommand = switchCommand.Trim();

            // test all commands.
            foreach(var current in _commands)
            {
                if(current.HasSwitch(switchCommand))
                {
                    return true;
                }
            }

            return false; // no switch found!
        }

        /// <summary>
        /// Returns true if the given string contains a key value like 'key=value'.
        /// </summary>
        /// <param name="keyValueString"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public static bool SplitKeyValue(string keyValueString, out string[] keyValue)
        {
            keyValue = null;
            if (keyValueString.Count(x => x == '=') == 1)
            { // there is only one '=' sign here.
                int idx = keyValueString.IndexOf('=');
                if (idx > 0 && idx < keyValueString.Length - 1)
                {
                    keyValue = new string[2];
                    keyValue[0] = keyValueString.Substring(0, idx);
                    keyValue[1] = keyValueString.Substring(idx + 1, keyValueString.Length - (idx + 1));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given string contains one or more comma seperated values.
        /// </summary>
        /// <param name="valuesArray"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool SplitValuesArray(string valuesArray, out string[] values)
        {
            values = valuesArray.Split(',');
            return true;
        }
    }
}
