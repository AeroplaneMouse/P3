using System;

namespace Asset_Management_System
{
    public static class ConsoleWriter
    {
        /// <summary>
        /// This allows for timestamps being added to console output.
        /// Use the same way as Console.WriteLine
        /// </summary>
        /// <param name="message">The message to get printed.</param>
        public static void ConsoleWrite(string message){
            Console.WriteLine(DateTime.Now + " " + message);
        }
    }
}