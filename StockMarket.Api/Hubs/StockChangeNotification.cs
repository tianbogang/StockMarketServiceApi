using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarket.Api.Hubs
{
    public class StockChangeNotification : Hub
    {
        public async Task SendMessage(string param)
        {
            if (param == "Added")
            {
                await Clients.All.SendAsync("StockAdded");
            }
            else if (param == "Updated")
            {
                await Clients.All.SendAsync("StockUpdated");
            }
            else
            {
                await Clients.All.SendAsync("ReceivedMessage");
            }
        }
    }
}
