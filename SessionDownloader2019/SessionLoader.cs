using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SessionDownloader
{
    public class SessionLoader : ISessionLoader
    {
        public string FeedUri { get; set; }
        public List<Session> Sessions { get; set; }

        public SessionLoader()
        {
            this.Sessions = new List<Session>();
        }

        public void LoadSessionList()
        {
            using (WebClient webClient = new WebClient())
            {
                string sessionFeedContents = webClient.DownloadString(this.FeedUri);
                ParseSessionFeed(sessionFeedContents);
            }



        }

        private void ParseSessionFeed(string sessionFeedContents)
        {
            dynamic sessionFeed = JArray.Parse(sessionFeedContents);

            int i = 0;

            foreach (var sessionElement in sessionFeed)
            {
                Session session = new Session()
                {
                    Index = i,
                    Duration = sessionElement.durationInMinutes,
                    EmbedUrl = sessionElement.onDemand,
                    Level = sessionElement.level,
                    MediaUrl = InferMediaUrl(sessionElement),
                    Code = sessionElement.sessionCode,
                    Title = sessionElement.title
                };

                this.Sessions.Add(session);

                i++;
            }


        }

        private string InferMediaUrl(dynamic sessionElement)
        {
            // *************************************************************************
            // What the embed link is
            //https://medius.studios.ms/Embed/video-nc/B20-INT104B

            // What the session code is
            // B20-INT104C
            // which gets turned into 
            // https://medius.studios.ms/video/asset/HIGHMP4/B20-INT104C

            // what actually works
            //https://medius.studios.ms/video/asset/HIGHMP4/B20-INT104B
            // *************************************************************************

            string correctedSessionCode = string.Empty;
            string onDemandUrl = sessionElement.onDemand;

            if (onDemandUrl != string.Empty)
            {
                int lastSlashIndex = onDemandUrl.LastIndexOf('/');
                string actualCode = onDemandUrl.Substring(lastSlashIndex);
                correctedSessionCode = $"https://medius.studios.ms/video/asset/HIGHMP4{actualCode}";
            }

            return correctedSessionCode;
        }
    }
}
