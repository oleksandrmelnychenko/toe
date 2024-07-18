﻿using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public abstract class MessageBase
    {
        MessageType type { get; set; }

        public abstract void Handle(GameMaster gameMaster);
    }
}
