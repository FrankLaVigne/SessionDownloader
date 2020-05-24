using System;
using System.Collections.Generic;
using System.Text;

namespace SessionDownloader
{
    public interface ISessionLoader
    {
        public string FeedUri { get; set; }
        public List<Session> Sessions { get; set; }

        public void LoadSessionList();
    }
}
