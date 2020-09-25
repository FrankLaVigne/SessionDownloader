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
    /* Important fields in the JSON
     * 
     * "slideDeck": "https://medius.studios.ms/video/asset/PPT/IG19-THR3034",
     * "onDemand": "https://medius.studios.ms/Embed/Video-nc/IG19-THR3034",
     * "downloadVideoLink": "https://medius.studios.ms/video/asset/HIGHMP4/IG19-THR3034",
     * "onDemandThumbnail": "https://medius.studios.ms/video/asset/THUMBNAIL/IG19-THR3034",
     * "captionFileLink" : 
     * 
     */

    
    public class SessionLoader : ISessionLoader
    {
        #region Public Properties
        public string FeedUri { get; set; }
        public List<Session> Sessions { get; set; }

        #endregion

        #region Constructors

        public SessionLoader()
        {
            this.Sessions = new List<Session>();
        }
        #endregion

        #region Public Methods
        public void LoadSessionList()
        {
            using (WebClient webClient = new WebClient())
            {
                string sessionFeedContents = webClient.DownloadString(this.FeedUri);
                ParseSessionFeed(sessionFeedContents);
            }
        }

        #endregion


        #region Private Methods

        private void ParseSessionFeed(string sessionFeedContents)
        {
            dynamic sessionFeed = JArray.Parse(sessionFeedContents);

            int i = 0;

            foreach (var sessionElement in sessionFeed)
            {
                string shortCode = ExtractShortCode(sessionElement);
                string mediaUrl = InferMediaUrl(sessionElement, shortCode);
                string thumbnailUrl = InferThumbnailUrl(shortCode);

                Session session = new Session()
                {
                    Index = i,
                    Duration = sessionElement.durationInMinutes,
                    EmbedUrl = sessionElement.onDemand,
                    Level = sessionElement.level,
                    MediaUrl = mediaUrl,
                    ThumbnailUrl = thumbnailUrl,
                    Code = sessionElement.sessionCode,
                    ShortCode = shortCode,
                    Title = sessionElement.title,
                    SlideDeckUrl = sessionElement.slideDeck,
                    CaptionsUrl = sessionElement.captionFileLink
                };

                this.Sessions.Add(session);

                i++;
           }


        }

        private string InferThumbnailUrl(string shortCode)
        {
            return $"https://medius.studios.ms/video/asset/THUMBNAIL/IG20-{shortCode}";
        }

        private string ExtractShortCode(dynamic sessionElement)
        {
            // HACK: for Ignite 2020
            // url that works
            //                                                     Session Code  
            //                                                    /
            // https://medius.studios.ms/video/asset/HIGHMP4/IG20-DB106

            string sessionCode = sessionElement.sessionCode;
            var shortSessionCode = sessionCode;

            int dashCount = sessionCode.Count(f => f == '-');

            if (dashCount > 0)
            {
                shortSessionCode = sessionCode.Split('-')[1];
            }

            return shortSessionCode;

        }

        private string InferMediaUrl(dynamic sessionElement, string shortSessionCode)
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

            string correctedSessionDownloadUrl = string.Empty;
            string onDemandUrl = sessionElement.onDemand;

            var downloadVideoLink = sessionElement.downloadVideoLink;

            if (downloadVideoLink != null)
            {
                correctedSessionDownloadUrl = downloadVideoLink;

                return correctedSessionDownloadUrl;
            }


            if (onDemandUrl != null && onDemandUrl != string.Empty)
            {
                int lastSlashIndex = onDemandUrl.LastIndexOf('/');
                string actualCode = onDemandUrl.Substring(lastSlashIndex);
                correctedSessionDownloadUrl = $"https://medius.studios.ms/video/asset/HIGHMP4{actualCode}";
            }
            else
            {
                correctedSessionDownloadUrl = $"https://medius.studios.ms/video/asset/HIGHMP4/IG20-{shortSessionCode}";
            }
            return correctedSessionDownloadUrl;
        }

        #endregion
    }
}
