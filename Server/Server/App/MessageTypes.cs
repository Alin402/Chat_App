using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.App
{
    public class MessageTypes
    {
        public static string CONN = "conn";
        public static string GENERAL = "general";
        public static string RECEIVE_USERS = "receive_users";
        public static string ROOM = "room";
        public static string ROOM_INVITATION = "room_invitation";
        public static string ROOM_INVITATION_ACCEPT = "room_invitation_accept";
        public static string ROOM_INVITATION_DENY = "room_invitation_deny";
    }
}
