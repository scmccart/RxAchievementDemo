using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RxAchievementDemo.Web.Models
{
    public class Achievement
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}