﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Net
{
    internal static class EndPointValidation
    {
        internal static (bool, string) IsValidEndPoint(IPEndPoint iPEndPoint)
        {
            string ipPattern = @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}$";
            Regex regex = new Regex(ipPattern);
            if (!regex.IsMatch(iPEndPoint.Address.ToString()))
            {
                return (false, $"IP address: {iPEndPoint.Address} is not valid.");
            }
            else if(iPEndPoint.Port > 65535 && iPEndPoint.Port < 0)
            {
                return (false, $"Port: {iPEndPoint.Port} is not valid.");
            }

            return (true, "IP endpoint is valid.");
        }
    }
}
