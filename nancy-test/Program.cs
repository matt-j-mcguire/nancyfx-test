using Nancy;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace nancy_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //self hosting needs elevated to admin to run correct
            using (var host = new NancyHost(new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Running on http://localhost:1234");
                Console.ReadLine();
            }
        }
    }

    public class ns : NancyModule
    {
        public ns() : base("/") //base root of api's
        {

            Get("hello", x => "hello yourself"); //you can put an optional '/' infront if necessary

            Get("bob", x => { return getdata(); });

            Get("damnit-bob", x => { return what(this.Request.Query["args"]); }); //pass in a ?args=sometext

            Get("passmesomejson", x => { return doSomeWork(); }); //pass any json structure with "justatest" (number) and "someOther" (string)

            Post("SaveForLater", x => { Later = GetBodyData(1024); return ""; });

            Get("ItsLater", x => Later); //works with props well

            Get("GenError", x => new Response() { StatusCode = HttpStatusCode.BadRequest });
        }

        private string getdata()
        {
            return "hello";
        }

        private string what(string data)
        {
            return data.ToUpper();
        }

        private string doSomeWork()
        {
            string jsondata = GetBodyData(1024);
            dynamic j = JObject.Parse(jsondata);
            j.justatest = j.justatest + 1; //++ operator does not work on dynamic data
            j.someOther = "goodby";
            return JsonConvert.SerializeObject(j);
        }



        //default configuration will be one instance per call, 
        //make it static for just testing on setting and retrieving
        //values
        private static string _later;

        private string Later
        {
            get => _later;
            set => _later = value;
        }



        #region helpers
        public string GetBodyData(int maxbfr)
        {
            byte[] x = new byte[maxbfr];
            int cnt = this.Request.Body.Read(x, 0, maxbfr);
            return System.Text.ASCIIEncoding.ASCII.GetString(x, 0, cnt);
        }
        #endregion

    }

}
