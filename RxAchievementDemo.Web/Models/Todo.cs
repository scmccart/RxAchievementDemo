using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RxAchievementDemo.Web.Models
{
    public class Todo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }
}