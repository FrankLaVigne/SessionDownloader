using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SessionDownloader
{
    public class Session
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public string EmbedUrl { get; set; }
        public string Code { get; set; }
        public string MediaUrl { get; set; }
        public int Duration { get; set; }
        public List<string> SpeakerNames { get; set; }
        public string Level { get; set; }
    }
}
