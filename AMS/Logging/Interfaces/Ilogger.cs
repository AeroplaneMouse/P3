using System.Collections.Generic;

namespace AMS.Logging.Interfaces
{
    public interface ILogger
    {
        Dictionary<string, string> PreviousValues { get; }
        bool LogCreate(ILoggableValues loggableValues);

        bool LogDelete(ILoggableValues loggableValues);

        bool LogUpdate(ILoggableValues loggableValues);

        bool SavePreviousValues(ILoggableValues loggableValues);
    }
}