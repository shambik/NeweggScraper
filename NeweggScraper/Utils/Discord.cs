using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace NeweggScraper.Utils
{
    public class Discord
    {
        public static void SendDiscord(string WebHooksToken, EntryModel item)
        {
            string token = WebHooksToken;
            WebRequest request = (HttpWebRequest)WebRequest.Create(token);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    username = "Spidey Bot",
                    embeds = new[]
                    {
                        new
                        {
                            title = "Stock Notification",
                            description = $"**Product Page**\n{item.Link}\n\n**Add To Cart**\n{item.AddToCart}\n\n**Store**\nNewegg\n\n**Brand**\n{item.Brand}\n\n**Price**\n{item.Price}\n\n**Description**\n{item.Description}",
                            color = "9697064"
                        }
                    }
                });
                sw.Write(json);
            }
            var responnd = (HttpWebResponse)request.GetResponse();
        }
    }

}