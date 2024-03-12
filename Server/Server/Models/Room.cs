using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Room
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public User? User1 { get; set; }
        public Socket? Client1 { get; set; }
        public User? User2 { get; set; }
        public Socket? Client2 {  get; set; }
    }
}
