using Sharepoint.Webhooks.Domain;
using SharePoint.WebHooks.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SharepointWebHookManagement.Api.Controllers
{
    public class SpOperationsController : ApiController
    {

        [HttpPost]
        [Route("api/SpOperation/AddTrace")]
        public bool AddTrace([FromBody]NotificationModel notification)
        {
            SharepoingManager manager = new SharepoingManager();
            return manager.AddTrace(notification.Resource,notification.SiteUrl,notification.Origin);
        }
    }
}
