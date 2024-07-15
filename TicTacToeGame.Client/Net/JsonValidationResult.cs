using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net
{
    public record JsonValidationResult(bool IsValid, string JsonMessage);
}
