using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tic_tac_toe_Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Net;
using Newtonsoft.Json;

namespace Tic_tac_toe_Server.Net.Tests
{
    [TestClass()]
    public class ServerJsonDataSerializerTests
    {
        [TestMethod()]
        public void DeserializeActionTest()
        {
            string json = @"
        {
            ""CellIndex"": 5,
            ""IsRestart"": true,
            ""ClientId"": ""d290f1ee-6c54-4b01-90e6-d701748f0851""
        }";

            ClientToServerConfig expected = new(5, true, Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851"));

            ClientToServerConfig config = ServerJsonDataSerializer.DeserializeAction(json);

            Assert.AreEqual(expected, config);

        }

        [TestMethod()]
        public void DeserializeActionThrowJsonExeptionsTest()
        {
            string ivalidJson = "Invalid json";

            try
            {
                ClientToServerConfig config = ServerJsonDataSerializer.DeserializeAction(ivalidJson);
                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

        }


    }
}