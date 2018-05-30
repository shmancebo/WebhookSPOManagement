using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhookManagement.Shared.Models
{
    public class SPChangeInfo
    {
        public string ItemId { get; set; }

        public string ItemTitle { get; set; }
        public string ListName { get; set; }
        public string TypeChange { get; set; }
    }
}
