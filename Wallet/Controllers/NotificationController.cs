using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nethereum.JsonRpc.Client;
using Wallet.Notifications;
using Wallet.ViewModels;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private IHubContext<NotifyHub> _hubContext;

        public NotificationController(IHubContext<NotifyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public string Post([FromBody]Notification msg)
        {
            string retMessage = string.Empty;
            try
            {
                _hubContext.Clients.All.SendAsync("Message","Title","Important Text");
                retMessage = "Success";
            }
            catch (Exception e)
            {
                retMessage = e.ToString();
            }
            return retMessage;
        }
    }
}