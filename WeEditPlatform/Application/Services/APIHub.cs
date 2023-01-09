using Domain;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class APIHub : Hub
    {
        public async Task BroadcastChartData(List<Job> data) =>
        await Clients.All.SendAsync("broadcastJobData", data);


    }
}
