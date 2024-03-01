using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.utils
{
    class IDCreator
    {
        public static string CreateUniqueID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
