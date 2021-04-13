//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace CapsuleParser
{
    public class Log
    {
        /// <summary>
        /// Buffer to collect Log messages
        /// </summary>
        private static List<string> LogMessages;

        public static Log GetInstance { get; } = new Log();

        public List<string> GetLogMessages
        {
            get
            {
                return LogMessages;
            }
        }

        private Log()
        {
            LogMessages = new List<string>();
        }

        /// <summary>
        /// Log a message to LogMessages collection
        /// </summary>
        /// <param name="message"></param>
        public void Append(string message)
        {
            LogMessages.Add(message);
        }

        /// <summary>
        /// Log messages to LogMessages collection
        /// </summary>
        /// <param name="messages"></param>
        public void Append(List<string> messages)
        {
            LogMessages.AddRange(messages);
        }

        /// <summary>
        /// Output LogMessages to the console
        /// </summary>
        public void Dump()
        {
            foreach (string msg in LogMessages)
            {
                Console.WriteLine(msg);
            }
            this.Clear();
        }

        /// <summary>
        /// Clear LogMessages
        /// </summary>
        public void Clear()
        {
            LogMessages.Clear();
        }
    }
}
