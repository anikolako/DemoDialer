using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialerDemo
{
    public static class Repository
    {
        /// <summary>
        /// States on dialer
        /// </summary>
        public enum InterationState
        {
            ROUTING = 0,
            ALERTING,
            CONNECTED,
            ABANDONED,
            HANDLED,
            CLEARED
        }

        /// <summary>
        /// States for phone origin
        /// </summary>
        public enum OriginState
        {
            INBOUND = 1,
            OUTBOUND = 2
        }


        public static List<Dialer.Call> getData()
        {
            List<Dialer.Call> calls = new List<Dialer.Call>();
            calls.Add(new Dialer.Call { Id = "123ab", agentid = 123, ContactId = 31231, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.INBOUND, interactionState = (int)Repository.InterationState.ROUTING, Duration = 1, phonenumber = "2104848488", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ab", agentid = 123, ContactId = 31231, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.INBOUND, interactionState = (int)Repository.InterationState.ALERTING, Duration = 4, phonenumber = "2104848488", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ab", agentid = 123, ContactId = 31231, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.INBOUND, interactionState = (int)Repository.InterationState.CONNECTED, Duration = 63, phonenumber = "2104848488", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ab", agentid = 123, ContactId = 31231, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.INBOUND, interactionState = (int)Repository.InterationState.CLEARED, Duration = 0, phonenumber = "2104848488", timestamp = DateTime.Now });

            calls.Add(new Dialer.Call { Id = "123ac", agentid = 456, ContactId = 12141, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.OUTBOUND, interactionState = (int)Repository.InterationState.ALERTING, Duration = 5, phonenumber = "6984755636", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ac", agentid = 456, ContactId = 12141, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.OUTBOUND, interactionState = (int)Repository.InterationState.ABANDONED, Duration = 1, phonenumber = "6984755636", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ac", agentid = 456, ContactId = 12141, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.OUTBOUND, interactionState = (int)Repository.InterationState.CLEARED, Duration = 0, phonenumber = "6984755636", timestamp = DateTime.Now });

            calls.Add(new Dialer.Call { Id = "123ad", agentid = 257, ContactId = 14587, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.OUTBOUND, interactionState = (int)Repository.InterationState.ALERTING, Duration = 7, phonenumber = "697145256", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ad", agentid = 257, ContactId = 14587, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.OUTBOUND, interactionState = (int)Repository.InterationState.CONNECTED, Duration = 125, phonenumber = "697145256", timestamp = DateTime.Now });
            calls.Add(new Dialer.Call { Id = "123ad", agentid = 257, ContactId = 14587, InteractionKey = System.Guid.NewGuid().ToString(), interactionOrigin = (int)Repository.OriginState.OUTBOUND, interactionState = (int)Repository.InterationState.CLEARED, Duration = 0, phonenumber = "697145256", timestamp = DateTime.Now });
            return calls;

        }


    }
}
