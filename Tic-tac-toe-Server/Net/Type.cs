﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Net
{
    public enum Type
    {
        PlayerInitialization,
        NewGameSession,
        NewGameData,
        NewAction,
        Restart
    }
}
