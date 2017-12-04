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
using System.Configuration;

namespace GC_FinalProject_FFLTool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ApiRequest ()
        {

            HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.1/pull/nfl/2017-regular/active_players.json?position=qb,rb,wr,te,k,def");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            //ViewBag.Data = apiDataJSON;
            ViewBag.Players = apiDataJSON["activeplayers"]["playerentry"];

            return View("AllPlayersView");
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