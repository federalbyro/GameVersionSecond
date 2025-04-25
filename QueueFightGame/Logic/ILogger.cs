using System.Collections.Generic;

namespace QueueFightGame
{
    public interface ILogger
    {
        void Log(string message);
        List<string> GetLogHistory();
        void ClearLog();
    }
}