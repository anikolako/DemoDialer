using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Linq;
using DialerDemo.Helper;
using NLog;

namespace DialerDemo
{
    /// <summary>
    /// This application demonstrates an example interaction with a supposed dialer system.
    /// We assume that we have a system that makes calls, produces events enqueue them on a rabbit mq system and then
    /// inserts data on another system( eg crm)
    /// </summary>
    class Program
    {
        //a collection with call ids that are supposed to coming from a dialer system.
        private static List<Dialer.Call> calls;
        private static DialerDemo.RabitMQ.Enqueuer enqueuer;
        private static NLog.Logger nLog = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            nLog.Info("Application Started");
            //Start application
            StartApp().GetAwaiter().GetResult();

        }

        public static Task StartApp()
        {
            calls = new List<Dialer.Call>();
            //get sample data
            calls = Repository.getData();
            //create a singleton instance of enqueuer class. Initializes connection to rabbit mq 
            enqueuer = DialerDemo.RabitMQ.Enqueuer.Instance;
            enqueuer.Initialize(Properties.Settings.Default.rmqhostname, Properties.Settings.Default.rmqport, Properties.Settings.Default.rmqusername, Properties.Settings.Default.rmqpassword);

            //assign a rabbitmq queue consumer
            var process = ProcessQMessages();
            var dialer = ProcessDialerCalls();

            return Task.WhenAll(new[] { process, dialer });
        }


        public static async Task ProcessDialerCalls()
        {
            //enqeue messages in parallel with our custom extension.
            await calls.ParallelForEachAsync(async (item) =>
             {
                 var call = Dialer.Call.Instance;//create a singleton instance of dialer class.
                 call.Id = item.Id;
                 call.agentid = item.agentid;
                 call.ContactId = item.ContactId;
                 call.Duration = item.Duration;
                 call.InteractionKey = item.InteractionKey;
                 call.phonenumber = item.phonenumber;
                 call.interactionState = item.interactionState;
                 call.timestamp = item.timestamp;
                 call.Client = await getClient(call.ContactId);
                 //Console.WriteLine("thread id {0} time {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, DateTime.Now);

                 var _enqueuer = DialerDemo.RabitMQ.Enqueuer.Instance;
                 _enqueuer.enqueueMessage<Dialer.Call>(call, "Dialer");// add call object on rabbit mq queue.
                 nLog.Info("Message enqueued with call id {0} phone number {1} state {2} and duration {3}", call.Id, call.phonenumber, call.interactionState, call.Duration);
             }, maxDegreeOfParallelism: 5);
        }

        public static Task ProcessQMessages()
        {
            //run rabbit mq consumer on a backthround thread. Fire and forget.
            Task task = Task.Run(() =>
            {
                enqueuer.channel.QueueDeclare(queue: "Dialer",
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                Crm.CallHistory callHistory = null;

                var consumer = new AsyncEventingBasicConsumer(enqueuer.channel);
                consumer.Received += async (model, ea) =>
               {

                   var body = ea.Body;
                   var message = JsonConvert.DeserializeObject<Dialer.Call>(System.Text.Encoding.UTF8.GetString(body));//deserialize message on queue with a message object.
                   //we cache received dialer events i order to get all events for a call
                   //and make the sum of call duration for a specific call. Systems always returns multiple events for a single call.
                   var calls = CacheManager<Dialer.Call>.getCahedData(message.Id);

                   if (calls == null)
                   {
                       calls = new List<Dialer.Call>();
                   }

                   calls.Add(message);
                   CacheManager<Dialer.Call>.insertCachedData(message.Id, calls);

                   //we assume that always last interaction state on a phone call will be CLEARED.
                   if (message.interactionState == (int)Repository.InterationState.CLEARED)
                   {
                       callHistory = new Crm.CallHistory()
                       {
                           Id = System.Guid.NewGuid().ToString(),
                           origin = calls.Select(x => x.interactionOrigin).FirstOrDefault(),
                           phoneNumber = calls.Select(x => x.phonenumber).FirstOrDefault(),
                           clientId = calls.Select(x => x.Client.Id).FirstOrDefault(),
                           callId = calls.Select(x => x.Id).FirstOrDefault(),
                           duration = calls.Select(x => x.Duration).Sum()
                       };

                       //Now we are ready to insert our third party system.
                       Console.WriteLine(callHistory.ToString());
                       nLog.Info(callHistory.ToString());

                       CacheManager<Dialer.Call>.removeCachedData(callHistory.callId);
                   }

                   await Task.Yield();
               };

                enqueuer.channel.BasicConsume(queue: "Dialer", autoAck: true, consumer: consumer);//assign listener on consumer.Received event.
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// This funtion demonstrates an api call to 3rd party system(eg crm) in order to get data for a client.
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        private static async Task<Crm.Client> getClient(long contactId)
        {
            try
            {
                using (var api = new HttpClient())
                {
                    api.BaseAddress = new Uri(string.Format("http://{0}", "xxxxxx"));
                    api.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    //HTTP GET
                    var result = await api.GetAsync(string.Format("/AltitudeContact/api/Contacts/GetContactAttributes?easyCode={0}&campaignName=Conceptus_Good_02", contactId));

                    if (result.IsSuccessStatusCode)
                    {
                        var client = await result.Content.ReadAsAsync<Crm.Client>();
                        return client;
                    }
                    else
                    {
                        return new Crm.Client { Id = 31231, ContactId = 12, Firstname = "Aggelos", Lastname = "Nikolakopoulos" };//Since we don't have a real api in order to get data, so i made a some dummy data.
                    }
                }

            }
            catch (Exception ex)
            {
                return new Crm.Client { Id = 31231, ContactId = 12, Firstname = "Aggelos", Lastname = "Nikolakopoulos" };//Since we don't have a real api in order to get data, so i made a some dummy data.
            }

        }
    }
}