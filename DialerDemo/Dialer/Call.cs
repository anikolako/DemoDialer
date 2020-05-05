using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DialerDemo.Dialer
{
    /// <summary>
    /// An object that represents a call event on dialer.
    /// </summary>
    public class Call
    {
        //use lazy class instance initialization in order to guarantee thread safety.
        private static readonly Lazy<Call> _call = new Lazy<Call>(() => new Call());

        public static Call Instance
        {
            get { return _call.Value; }
        }

        public string Id { get; set; }

        /// <summary>
        /// Key for a specicfic interation on session
        /// </summary>
        public string InteractionKey { get; set; }
        public string phonenumber { get; set; }
        /// <summary>
        /// Incomcimg or OutComing Call
        /// </summary>
        public int interactionOrigin { get; set; }
        public long ContactId { get; set; }
        public int agentid { get; set; }
        public int interactionState { get; set; }
        public int Duration { get; set; }
        public DateTime timestamp { get; set; }

        /// <summary>
        /// An object that represents a client on a crm application
        /// </summary>
        public Crm.Client Client { get; set; }

    }
}
