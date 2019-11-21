using System;
using System.Collections.Generic;
using AMS.Models;

namespace AMS.Logging
{
    public interface ILoggableValues
    {
        /// <summary>
        /// Should return a dictionary of the properties that should be logged.
        /// The key should be property name, and the value should be property value
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetLoggableValues();

        /// <summary>
        /// Returns the name of the class being logged as a string.
        /// This is the type that will be written in the log
        /// </summary>
        /// <returns></returns>
        string GetLoggableTypeName();
    }
}