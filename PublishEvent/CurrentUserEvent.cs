using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDCTestSystemPart1.PublishEvent
{
    public class CurrentUserEvent : PubSubEvent<string>
    {
    }
}
