using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SessionDownloader.Utils;

namespace SessionDownloader
{

    // ************************************************************************
    // * Important URLs                                                       *
    // ************************************************************************
    // Build 2019
    // https://api.mybuild.techcommunity.microsoft.com/api/session/all
    // Build 2020
    // https://api.mybuild.microsoft.com/api/session/all
    // Ignite 2019 
    // https://api-myignite.techcommunity.microsoft.com/api/session/all
    // Ignite 2020 
    // https://api.myignite.microsoft.com/api/session/all
    // ************************************************************************

    class Program
    {
        #region Private Members

        private const int DESTINATION_PATH_ARG_INDEX = 0;
        private const int BASE_URL_ARG_INDEX = 1;
        private const int MEDIA_TYPE_ARG_INDEX = 2;

        private static Arguments arguments;

        private static ConsoleColor defaultForegroundConsoleColor = Console.ForegroundColor;
        private static ConsoleColor defaultBackgroundConsoleColor = Console.BackgroundColor;

        enum MessageLevel
        {
            Normal,
            Highlight,
            Warning,
            Error
        }

        #endregion

        static void Main(string[] args)
        {
            arguments = ParseArgs(args);

            if (arguments == null)
            {
                return;
            }

            var sessionLoader  = new SessionLoader();
            sessionLoader.FeedUri = arguments.FeedUrl;

            Console.WriteLine($"Feed: {arguments.FeedUrl}");

            WriteHighlight("Starting Feed Download");
            sessionLoader.LoadSessionList();
            WriteHighlight("Metadata Feed Complete");

            var slideDeckCount = sessionLoader.Sessions.Where(y => y.SlideDeckUrl != string.Empty).Count();
            var captionsCount = sessionLoader.Sessions.Where(y => y.CaptionsUrl != string.Empty).Count();
            var videoDownloadCount = sessionLoader.Sessions.Where(y => y.MediaUrl != string.Empty).Count();

            Console.WriteLine($"Session found: {sessionLoader.Sessions.Count}");
            Console.WriteLine($"Sessions with Video: {videoDownloadCount}");
            Console.WriteLine($"Sessions with Slides: {slideDeckCount}");
            Console.WriteLine($"Sessions with Captions: {captionsCount}");

            if (arguments.MediaType != MediaType.None)
            {
                DownloadSessions(sessionLoader.Sessions, arguments.MediaType);
            }                

            Console.WriteLine($"Finished at {DateTime.Now}");
            Console.ReadLine();

        }

        #region Methods to be moved out of this class
        private static void DownloadSessions(List<Session> sessions, MediaType mediaType)
        {

            foreach (var session in sessions)
            {
                Console.WriteLine("*****************************");
                Console.WriteLine($"Index: {session.Index}");
                Console.WriteLine($"Code: {session.Code}");
                Console.WriteLine($"Title: {session.Title}");
                Console.WriteLine($"Level: {session.Level}");
                Console.WriteLine($"Embed: {session.EmbedUrl}");
                Console.WriteLine($"Slides: {session.SlideDeckUrl}");
                Console.WriteLine($"Captions: {session.CaptionsUrl}");

                switch (mediaType)
                {
                    case MediaType.Video:
                        DownloadVideo(session);
                        break;
                    case MediaType.Slides:
                        DownloadSlide(session);
                        break;
                    case MediaType.Captions:
                        DownloadCaptions(session);
                        break;
                    case MediaType.All:
                        DownloadVideo(session);
                        DownloadSlide(session);
                        DownloadCaptions(session);
                        break;
                    default:
                        break;
                }

                Console.WriteLine("*****************************");

            }
        }

        private static void DownloadVideo(Session session)
        {
            string downloadUrl = session.MediaUrl;

            if (downloadUrl != string.Empty)
            {
                Console.WriteLine("Downloading Video");

                string remoteUri = downloadUrl;

                string scrubbedSessionTitle = FileSystem.ScrubFileName(session.Title);
                string destinationFilename = $"{arguments.DestinationPath}{session.Code}-{scrubbedSessionTitle}.mp4";

                if (File.Exists(destinationFilename) == true)
                {
                    Console.WriteLine("File exists. Skipping");
                }
                else
                {
                    DownloadFile(remoteUri, destinationFilename);
                }
            }
        }


        private static void DownloadSlide(Session session)
        {
            if (session.SlideDeckUrl != string.Empty)
            {
                Console.WriteLine("Downloading Slides");

                string remoteUri = session.SlideDeckUrl;

                string scrubbedSessionTitle = FileSystem.ScrubFileName(session.Title);
                string destinationFilename = $"{arguments.DestinationPath}{session.Code}-{scrubbedSessionTitle}.pptx";

                if (File.Exists(destinationFilename) == true)
                {
                    WriteWarning("File exists. Skipping");
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"Downloading ${destinationFilename}");
                        Console.WriteLine($"Starting at ${DateTime.Now}");
                        DownloadFile(remoteUri, destinationFilename);
                        Console.WriteLine($"Finishing at ${DateTime.Now}");
                    }
                    catch (Exception exception)
                    {
                        WriteError($"Error downloading {remoteUri} to {destinationFilename}");
                        WriteError($"{exception.Message}");
                    }

                }

                Console.WriteLine($"{destinationFilename}");
            }
        }


        private static void DownloadCaptions(Session session)
        {
            if (session.CaptionsUrl != string.Empty)
            {
                Console.WriteLine("Downloading captions");

                string remoteUri = session.CaptionsUrl;

                string scrubbedSessionTitle = FileSystem.ScrubFileName(session.Title);
                string destinationFilename = $"{arguments.DestinationPath}{session.Code}-{scrubbedSessionTitle}.txt";

                if (File.Exists(destinationFilename) == true)
                {
                    WriteWarning("File exists. Skipping");
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"Downloading ${destinationFilename}");
                        Console.WriteLine($"Starting at ${DateTime.Now}");
                        DownloadFile(remoteUri, destinationFilename);
                        Console.WriteLine($"Finishing at ${DateTime.Now}");
                    }
                    catch (Exception exception)
                    {
                        WriteError($"Error downloading {remoteUri} to {destinationFilename}");
                        WriteError($"{exception.Message}");
                    }

                }

                Console.WriteLine($"{destinationFilename}");
            }
        }


        private static void DownloadFile(string remoteUri, string destinationFilename)
        {
            Console.WriteLine($"\t Download started at {DateTime.Now}");
            var fd = new FileDownloader();
            fd.DownloadFile(remoteUri, destinationFilename);
        }


        #endregion

        #region Private Methods

        private static Arguments ParseArgs(string[] args)
        {

            if (args.Length < 2)
            {
                Console.WriteLine("Please enter a destination path and base RSS feed URL!");
                return null;
            }

            var destinationPath = args[DESTINATION_PATH_ARG_INDEX];

            if (destinationPath.Last() != '\\')
            {
                destinationPath = destinationPath + '\\';
            }

            var baseUrl = args[BASE_URL_ARG_INDEX];

            try
            {
                Uri feed = new Uri(baseUrl);
            }
            catch (Exception)
            {
                Console.WriteLine("that is not a valid URL");
                return null;
            }

            var downloadMediaType = ReadMediaTypeArg(args[MEDIA_TYPE_ARG_INDEX]);

            return new Arguments()
            {
                DestinationPath = destinationPath,
                MediaType = downloadMediaType,
                FeedUrl = baseUrl
            };
        }
        private static MediaType ReadMediaTypeArg(string mediaTypeArgument)
        {
            MediaType mediaType = MediaType.None;
            try
            {
                var selectedMedia = mediaTypeArgument.ToLower().First();

                switch (selectedMedia)
                {
                    case 'v':
                        mediaType = MediaType.Video;
                        break;
                    case 'a':
                        mediaType = MediaType.All;
                        break;
                    case 's':
                        mediaType = MediaType.Slides;
                        break;
                    case 'c':
                        mediaType = MediaType.Captions;
                        break;
                    default:
                        mediaType = MediaType.None;
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"Error: {exception.Message}");
                Console.WriteLine("--------------------------------------------------");
            }

            return mediaType;

        }
        private static void ResetConsoleColors()
        {
            Console.ForegroundColor = defaultForegroundConsoleColor;
            Console.BackgroundColor = defaultBackgroundConsoleColor;
        }
        private static void WriteError(string message)
        {
            WriteMessage(message, MessageLevel.Error);
        }
        private static void WriteWarning(string message)
        {
            WriteMessage(message, MessageLevel.Warning);
        }
        private static void WriteHighlight(string message)
        {
            WriteMessage(message, MessageLevel.Highlight);
        }
        private static void WriteMessage(string message, MessageLevel messageLevel)
        {
            switch (messageLevel)
            {
                case MessageLevel.Normal:
                    break;
                case MessageLevel.Highlight:
                    InvertConsoleColors();
                    break;
                case MessageLevel.Warning:
                    WarningConsoleColors();
                    break;
                case MessageLevel.Error:
                    ErrorConsoleColors();
                    break;
                default:
                    break;
            }

            Console.WriteLine(message);

            ResetConsoleColors();


        }
        private static void InvertConsoleColors()
        {
            Console.ForegroundColor = defaultBackgroundConsoleColor;
            Console.BackgroundColor = defaultForegroundConsoleColor;
        }
        private static void ErrorConsoleColors()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = defaultBackgroundConsoleColor;
        }
        private static void WarningConsoleColors()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = defaultBackgroundConsoleColor;
        }
        #endregion
    }
}
