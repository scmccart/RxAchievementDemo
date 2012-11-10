using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxAchievementDemo.Models
{
    public class TodoEvent
    {
        public int Id { get; set; }
        public EventType Type { get; set; }
    }
}
