using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tic_tac_toe_Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Player;
using Tic_tac_toe_Server.Net;

namespace Tic_tac_toe_Server.Game.Tests
{
    [TestClass()]
    public class PlayerManagerTests
    {
        [TestMethod()]
        public void ConnectClientToPlayerTest()
        {
            List<PlayerBase> players = new List<PlayerBase>
            {
                new Player.Player(Symbol.X, false, PlayerStatus.Connected),
                new Player.Player(Symbol.X, false, PlayerStatus.Connected),
                new Player.Player(Symbol.X, false, PlayerStatus.Connected)
            };

            Client client = new Client();

            PlayerManager playerManager = new PlayerManager(3);
            playerManager.Players = players;

            playerManager.ConnectClientToPlayer(ref client);


            Assert.Equals(client.Id, playerManager.Players.FirstOrDefault(p => p.Status == PlayerStatus.Connected).Id);
        }
    }
}