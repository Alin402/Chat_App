﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Message
    {
        public User? From { get; set; }
        public Object? Content { get; set; }
        public string? Type { get; set; }
        public User? To { get; set; }
        public string? RoomID { get; set; }
    }
}
