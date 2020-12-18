using Zal.Domain.ActiveRecords;
using Zal.Domain.ItemSets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Zal.Domain
{
    public delegate void OfflineCommandDelegate(XDocument commands);
    public delegate void SessionStateDelegate(Session session);

    public static class Zalesak
    {
        public static event OfflineCommandDelegate CommandExecutedOffline;
        public static event SessionStateDelegate UsersSessionStateChanged;


        public static bool IsConnected { get; private set; } = true;
        public static bool UserIsLogged => Session.IsUserLogged;

        public static Session Session { get; set; } = new Session();

        public static DocumentSet Documents { get; private set; } = new DocumentSet();
        public static BadgeSet Badges { get; private set; } = new BadgeSet();
        public static UserSet Users { get; private set; } = new UserSet();
        public static ActualitySet Actualities { get; private set; } = new ActualitySet();
        public static ActionSet Actions { get; private set; } = new ActionSet();
        [Obsolete]
        public static GallerySet Galleries { get; private set; } = new GallerySet();
        public static GraphGallerySet GraphGalleries { get; private set; } = new GraphGallerySet();
        public static GameSet Games { get; private set; } = new GameSet();

        [Obsolete]
        public static bool LoginAsGuest()
        {
            if (IsConnected)
            {
                //Me = User.Empty();
                Session = new Session();
                ReSynchronizeAsync();
            }
            return false;
        }

        [Obsolete]
        public static void Logout()
        {
            //Me = User.Empty();
            Session.Stop();
        }

        public static Task<Version> CurrentVersion()
        {
            return VersionAR.GetCurrentVersion();
        }

        public static void LoadOfflineCommands(XDocument commands)
        {
            //Database.LoadOfflineCommands(commands);
        }

        //public static bool Connect() {
        //    IsConnected = Database.Connect(CONNECTION_STRING);
        //    if (IsConnected) {//???
        //        StartSynchronizing();
        //    }
        //    return IsConnected;
        //}

        private static void OnCommandExecutedOffline(XDocument command)
        {
            if (CommandExecutedOffline != null)
            {
                CommandExecutedOffline.Invoke(command);
            }
            else
            {
                throw new Exception("Command is executed when offline, but no-one is listening this event");
            }
        }

        public static async Task StartSynchronizingAsync()
        {
            //Documents.Synchronize();
            //Badgets.Synchronize();
            await Users.Synchronize();
            await Actualities.Synchronize();
            await Actions.SynchronizeAllCurrentlyActive();
        }

        private static async Task ReSynchronizeAsync()
        {
            //Documents.ReSynchronize();
            //Badges.ReSynchronize();
            await Users.ReSynchronize();
            //Actualities.ReSynchronize();
            await Actions.ReSynchronizeAsync();
        }

        public static void LoadDataFrom(string json)
        {
            try
            {
                JObject jObject = JObject.Parse(json);
                Session.LoadFrom(jObject.GetValue("session"));
                GraphGalleries.LoadFrom(jObject.GetValue("gallery"));
                //Users.LoadFrom(jObject.GetValue("users"));
                //Actions.LoadFrom(jObject.GetValue("actions"));
                //Actualities.LoadFrom(jObject.GetValue("actualities"));
            }
            catch (Exception ex)
            {
            }
        }

        public static string GetDataJson()
        {
            JObject jObject = new JObject {
                {"session", Session.GetJson() },
                {"gallery", GraphGalleries.GetJson() },
                //{"users", Users.GetJson() },
                //{"actions", Actions.GetJson() },
                //{"actualities", Actualities.GetJson() },
            };
            return jObject.ToString();
        }
    }
}
