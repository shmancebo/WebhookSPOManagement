using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhookManagement.Shared.Models
{
    public class SPListInfo
    {
        public List<SPListInfoItem> value { get; set;}
     
    }

    public class SPListInfoItem
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string BaseTemplate { get; set; }
    }
}
