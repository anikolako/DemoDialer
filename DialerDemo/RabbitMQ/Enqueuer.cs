using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace DialerDemo.RabitMQ
{
    /// <summary>
    /// A class that handles rabbit mq functionality.
    /// </summary>
    public class Enqueuer
    {
        private static readonly Lazy<Enqueuer> _Enqueuer = new Lazy<Enqueuer>(() => new Enqueuer());
        public static Enqueuer Instance
        {
            get { return _Enqueuer.Value; }
        }

        private Enqueuer()
        {

        }
        public string hostname { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public ConnectionFactory factory { get; set; }
        public IConnection connection { get; set; }
        public IModel channel { get; set; }

        /// <summary>
        /// initialize rabbit mq connection and channel.
        /// </summary>
        /// <param name="_hostname"></param>
        /// <param name="_port"></param>
        /// <param name="_username"></param>
        /// <param name="_password"></param>
        public void Initialize(string _hostname, int _port, string _username, string _password)
        {
            this.hostname = _hostname;
            this.port = _port;
            this.username = _username;
            this.password = _password;

            this.factory = new ConnectionFactory()
            {
                HostName = this.hostname,
                Port = this.port,
                UserName = this.username,
                Password = this.password,
                DispatchConsumersAsync = true
            };

            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();
        }

        /// <summary>
        /// Declare and bind a queue on rabbit mq based on flat table name.
        /// If it is already exists, does not create it again.
        /// </summary>
        /// <param name="qname"></param>
        public void assignQueue(string qname)
        {
            //durable true, means that qeueu will be available after server restart or mq rabbit service restart.
            this.channel.QueueDeclare(qname, true, false, false, null);
            this.channel.QueueBind(qname, Properties.Settings.Default.Exchange, qname, null);
        }

        /// <summary>
        /// Function that enqueues message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="_channel"></param>
        public void enqueueMessage<T>(T obj, string qname)
        {
            using (var _connection = factory.CreateConnection())
            {
                using (var _channel = _connection.CreateModel())
                {
                    //start enqueuing mesagge.
                    _channel.QueueDeclare(qname, true, false, false, null);
                    _channel.QueueBind(qname, Properties.Settings.Default.Exchange, qname, null);

                    string message = JsonConvert.SerializeObject(obj);
                    var body = Encoding.UTF8.GetBytes(message);
                    //Create persistent message.When rabbit mq service is restarted, message will be available.
                    IBasicProperties basicProperties = _channel.CreateBasicProperties();
                    basicProperties.Persistent = true;
                    basicProperties.DeliveryMode = 2;

                    _channel.BasicPublish(Properties.Settings.Default.Exchange, qname, basicProperties, body);
                }
            }
        }


    }
}


