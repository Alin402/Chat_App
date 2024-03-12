using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.App.Utils
{
    public class IDGenerator
    {
        public static string GenerateRandomID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
