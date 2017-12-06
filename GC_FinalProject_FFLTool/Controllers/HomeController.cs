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
        

        public JObject ApiRequest(string pos, string lastName)
        {
            /*** Cumulative Game Stats API Call ***/
            //HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?position=qb,rb,wr,te,k");
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?position={pos}&player={lastName}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            //ViewBag.Data = apiDataJSON;
            //ViewBag.Players = apiDataJSON["activeplayers"]["playerentry"];
            //ViewBag.Players = apiDataJSON["cumulativeplayerstats"]["playerstatsentry"];

            // return View("AllPlayersView");
            //return View();

            return apiDataJSON;
        }







        public JObject ApiRequest(string pos)
        {
            /*** Cumulative Game Stats API Call ***/
            //HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?position=qb,rb,wr,te,k");
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?{pos}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            //ViewBag.Data = apiDataJSON;
            //ViewBag.Players = apiDataJSON["activeplayers"]["playerentry"];
            //ViewBag.Players = apiDataJSON["cumulativeplayerstats"]["playerstatsentry"];

            // return View("AllPlayersView");
            //return View();

            return apiDataJSON;
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

        public ActionResult ShowAllPlayers()
        {
            JObject players = ApiRequest("position=qb,rb,wr,te,k");

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];

            return View("AllPlayersView");
        }

        public ActionResult SearchPlayers(string player, string pos)
        {
            JObject players;
            if (pos == "QB")
            {
                players = ApiRequest("position=qb", player);

            }
            else if (pos == "WR")
            {
                players = ApiRequest("position=wr", player);

            }
            else if (pos == "RB")
            {
                players = ApiRequest("position=rb", player);

            }
            else if (pos == "TE")
            {
                players = ApiRequest("position=te", player);

            }
            else if (pos == "K")
            {
                players = ApiRequest("position=k", player);
            }
            else
            {
                players = ApiRequest("position=qb,rb,wr,te,k", player);
            }
            //JObject players = ApiRequest("position=qb,rb,wr,te,k&player=" + player);
            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Pos = pos;

            return View("AllPlayersView");

        }
    }
}