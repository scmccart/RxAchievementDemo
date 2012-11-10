using RxAchievementDemo.Web.Hubs;
using RxAchievementDemo.Web.Models;
using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RxAchievementDemo.Web.Controllers
{
    public class AchievementController : ApiController
    {
        public HttpResponseMessage Post(Achievement achievement)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Notify>();

            hubContext.Clients.NewAchievement(achievement.Message);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
