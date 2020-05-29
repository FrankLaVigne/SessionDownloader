using System;
using System.Collections.Generic;
using System.Text;

namespace SessionDownloader
{
 
    /// <summary>
    /// Object to store arguments passed to the CLI app
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Path to save files 
        /// </summary>
        public string DestinationPath { get; set; }
        /// <summary>
        /// Type of media to download
        /// </summary>
        public MediaType MediaType { get; set; }
        /// <summary>
        /// URL to session data feed
        /// </summary>
        public string FeedUrl { get; set; }
    }

}
