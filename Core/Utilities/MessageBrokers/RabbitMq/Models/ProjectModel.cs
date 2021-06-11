using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.MessageBrokers.RabbitMq.Models
{
    public class ProjectModel
    {
        public int UserId { get; set; }
        public string ProjectKey { get; set; }
    }
}
