using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Logging
{
    public interface ILogger
    {
        void LogMessage(string message);

        void LogError(string message);

        void LogWarning(string message);

        void LogSuccess(string message);
    }
}
