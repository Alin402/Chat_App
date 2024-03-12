using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.App.Rooms
{
    public class AppRooms
    {
        private AppMessages messages;
        public List<Room> Rooms { get; set; }

        public AppRooms(AppMessages messages, List<Room> rooms)
        {
            this.messages = messages;
            Rooms = rooms;
        }

        public void AddRoom(Room newRoom)
        {
            if (Rooms.Contains(newRoom)) return;
            Rooms.Add(newRoom);
        }
    }
}
