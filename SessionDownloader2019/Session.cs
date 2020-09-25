using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SessionDownloader
{
    /// <summary>
    /// POCO to store session data
    /// </summary>
    public class Session
    {
        public int Index { get; set; }
        public string ShortCode { get; set; }
        public string Title { get; set; }
        public string EmbedUrl { get; set; }
        public string Code { get; set; }
        public string MediaUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string SlideDeckUrl { get; set; }
        public string CaptionsUrl { get; set; }
        public int Duration { get; set; }
        public List<string> SpeakerNames { get; set; }
        public string Level { get; set; }
    }
}
