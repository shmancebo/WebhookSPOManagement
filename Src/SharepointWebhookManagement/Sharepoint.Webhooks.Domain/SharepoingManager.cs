using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using WebhookManagement.Shared.Models;

namespace Sharepoint.Webhooks.Domain
{
    public class SharepoingManager
    {
        private readonly string site = ConfigurationManager.AppSettings["app"];
        private readonly string userName = ConfigurationManager.AppSettings["userName"];
        private readonly string passWord = ConfigurationManager.AppSettings["passWord"];
        private readonly string traceLibrary = ConfigurationManager.AppSettings["traceLibrary"];
        public bool AddTrace(string listId, string web, string origin)
        {
            string webSite = string.Format("{0}/sps2017", web);
            bool success = false;
            var url = string.Format(site, webSite);
            try
            {
                using (ClientContext client = new ClientContext(url))
                {
                    client.Credentials = new SharePointOnlineCredentials(userName, GetPassword(passWord));
                    var list = client.Web.Lists.GetById(new Guid(listId));
                    client.Load(list);
                    client.ExecuteQuery();
                    ChangeQuery changes = new ChangeQuery(true, true);
                    changes.Item = true;
                    changes.RecursiveAll = true;
                    changes.Add = true;
                    changes.User = true;
                    changes.Update = true;
                    changes.List = true;
                    changes.Field = true;
                    if (list != null)
                    {
                        var listChages = list.GetChanges(changes);
                        client.Load(listChages);
                        client.ExecuteQuery();
                        var result = listChages.LastOrDefault();
                        if (result != null)
                        {
                            var itemChange = (ChangeItem)result;
                            var type = result.ChangeType;
                            if (type != ChangeType.NoChange)
                            {
                                var changeInfo = new SPChangeInfo()
                                {
                                    ItemId = itemChange?.ItemId.ToString(),
                                    ItemTitle = string.Format("{0}_{1}", itemChange.ListId.ToString(), itemChange.ItemId.ToString()),
                                    ListName = itemChange.ListId.ToString(),
                                    TypeChange = type.ToString()
                                };

                                success = InsertTraceChanges(traceLibrary, client, changeInfo, origin);
                            }
                            else
                                success = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //logger
            }
            return success;
        }

        public bool InsertTraceChanges(string traceLibrary, ClientContext client, SPChangeInfo changeInfo, string origin)
        {
            var result = true;
            var listName = GetListNambeById(changeInfo.ListName, client);
            try
            {
                var list = client.Web.Lists.GetByTitle(traceLibrary);
                client.Load(list);
                client.ExecuteQuery();
                ListItemCreationInformation createInfo = new ListItemCreationInformation();
                var item = list.AddItem(createInfo);
                item["Title"] = string.Format("Trace {0}_{1} {2}", listName, changeInfo.ItemId, DateTime.Now.ToShortDateString());
                item["TipoOperacion"] = changeInfo.TypeChange;
                item["Origen"] = origin;
                item["Elemento"] = string.Format("ID:{0} , ListTitle:{1}", changeInfo.ItemId, listName);
                item.Update();
                client.ExecuteQuery();
                result = true;
            }
            catch (Exception e)
            {
                //logger
            }
            return result;
        }

        public string GetListNambeById(string id, ClientContext client)
        {
            var list = client.Web.Lists.GetById(new Guid(id));
            client.Load(list);
            client.ExecuteQuery();
            return list.Title;
        }

        public static SecureString GetPassword(string pass)
        {
            SecureString result = new SecureString();
            pass.ToCharArray().ToList().ForEach(p => result.AppendChar(p));
            return result;
        }
    }
}
