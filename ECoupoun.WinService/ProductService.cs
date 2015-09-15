using ECoupoun.Common;
using ECoupoun.Common.Helper;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceProcess;
using System.Threading;

namespace ECoupoun.WinService
{
    public partial class ProductService : ServiceBase
    {
        Timer _reconnectionTimer;
        System.Timers.Timer _timer = new System.Timers.Timer();
        private string timeString;
        private DayOfWeek dayOfWeek;

        public ProductService()
        {
            string _dayOfWeek = Convert.ToString(ConfigurationManager.AppSettings["DayOfWeek"]);
            DayOfWeek MyDays = (DayOfWeek)DayOfWeek.Parse(typeof(DayOfWeek), _dayOfWeek);

            dayOfWeek = MyDays;
            timeString = Convert.ToString(ConfigurationManager.AppSettings["WeeklyeventTriggerTime"]); ;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogHelper.WriteTraceLog("OnStart Called");
            CreateScheduleTimer();
        }

        protected override void OnStop()
        {
            LogHelper.WriteTraceLog("OnStop Called");
            _timer.Stop();
        }

        /// <summary>
        /// Create Schedule Timer based on Configuration
        /// </summary>
        private void CreateScheduleTimer()
        {
            LogHelper.WriteTraceLog("CreateScheduleTimer Called");
            _reconnectionTimer = new Timer(new System.Threading.TimerCallback(GetProducts), "Timer Call", 0, Convert.ToInt64(GetNextInterval()));
        }

        /// <summary>
        /// Called when time elapsed
        /// </summary>
        /// <param name="obj"></param>
        private void GetProducts(object obj)
        {
            try
            {
                LogHelper.WriteTraceLog("GetProducts Called");

                var url = ECoupounConstants.BestBuyRESTServiceURL + ECoupounConstants.InsertData;
                string responseText = string.Empty;
                ExtendedWebClient client = new ExtendedWebClient(new Uri(url));
                client.Headers[ECoupounConstants.ContentTypeText] = ECoupounConstants.ContentTypeValue;

                MemoryStream stream = new MemoryStream();
                byte[] data = client.UploadData(string.Format("{0}", url), "POST", stream.ToArray());
                stream = new MemoryStream(data);

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
                serializer = new DataContractJsonSerializer(typeof(string));
                responseText = (string)serializer.ReadObject(stream);
                LogHelper.WriteTraceLog(responseText);
            }
            catch (Exception ex)
            {
                LogHelper.WriteTraceLog(ex.Message);
            }
        }

        private double GetNextInterval()
        {
            LogHelper.WriteTraceLog("GetNextInterval Called");
            int hours = Convert.ToInt32(timeString);
            DateTime t = DateTime.Now.Date.Add(TimeSpan.FromHours(hours));
            TimeSpan ts = new TimeSpan();
            int x;

            if (System.DateTime.Now.DayOfWeek > dayOfWeek)
            {
                x = System.DateTime.Now.DayOfWeek - dayOfWeek;
                t = t.AddDays(7 - x);

            }
            else if (System.DateTime.Now.DayOfWeek == dayOfWeek)
            {
                if (t < System.DateTime.Now)
                {
                    t = t.AddDays(7);
                }
            }
            else
            {
                x = dayOfWeek - System.DateTime.Now.DayOfWeek;
                t = t.AddDays(x);
            }

            ts = (TimeSpan)(t - System.DateTime.Now);

            LogHelper.WriteTraceLog("TotalMilliseconds = " + ts.TotalMilliseconds.ToString());
            return ts.TotalMilliseconds;
        }
    }
}
