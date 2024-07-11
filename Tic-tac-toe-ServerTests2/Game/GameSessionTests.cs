using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tic_tac_toe_Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Net;

namespace Tic_tac_toe_Server.Game.Tests
{
    [TestClass()]
    public class GameSessionTests
    {
        public void AddPlayerTest()
        {
            GameSession session = new GameSession();

            Client client = new();

            session.AddPlayer(client);
            session.AddPlayer(client);

            Assert.Equals(client.Id, session.GetPlayerManager().Players.FirstOrDefault());
        }
    }
}