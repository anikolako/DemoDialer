using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialerDemo.Crm
{
    /// <summary>
    /// An object that represents a client on crm application.
    /// </summary>
    public class Client
    {
        public long Id { get; set; }
        public long ContactId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
