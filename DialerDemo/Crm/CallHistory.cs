using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialerDemo.Crm
{
    /// <summary>
    /// This objects represents a dialer's call on crm appliaction.
    /// </summary>
    class CallHistory
    {
        public string Id { get; set; }
        public long clientId { get; set; }
        public string callId { get; set; }
        public string phoneNumber { get; set; }
        public int origin { get; set; }
        public int duration { get; set; }

        public override string ToString()
        {
            return $"Call with id {this.Id}, callId {this.callId}, phonenumber {this.phoneNumber}, origin {this.origin}, cleintId {this.clientId}, Duration {this.duration}, Time {DateTime.Now}";

        }
    }
}