using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.App.MessageActions
{
    public class MessageTypesActions
    {
        public static Dictionary<string, IMessageAction> MessageTypesActionsValuePair { get; } = new Dictionary<string, IMessageAction>();
    }
}
