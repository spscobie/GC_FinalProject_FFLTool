using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC_FinalProject_FFLTool.Controllers
{
    public class HomeController : Controller
    {
        private string un = "";
        private string pw = "";

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ApiRequest ()
        {
            var un_UTF8 = Encoding.UTF8.GetBytes(un);
            var pw_UTF8 = Encoding.UTF8.GetBytes(pw);

            HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.1/pull/nfl/2016-2017-regular/cumulative_player_stats.json?playerstats=G");
            //WebReq.Headers.Add("AUTHORIZATION", "Basic " + Convert.ToBase64String(un_UTF8) + ":" + Convert.ToBase64String(pw_UTF8));
            WebReq.Headers.Add("AUTHORIZATION", "Basic " + un + ":" + pw);
            WebReq.UserAgent = ".NET Framework Test Client for Grand Circus Final Proj";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            ViewBag.Data = apiDataJSON;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}