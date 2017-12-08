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
using GC_FinalProject_FFLTool.Models;
using Microsoft.AspNet.Identity;

namespace GC_FinalProject_FFLTool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public JObject ApiRequest(string pos, string player)
        {
            /*** Cumulative Game Stats API Call ***/
            //HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?position=qb,rb,wr,te,k");
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json{pos}{player}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            return apiDataJSON;
        }

        public JObject ApiRequest(string pos)
        {
            /*** Cumulative Game Stats API Call ***/
            //HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?position=qb,rb,wr,te,k");
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json{pos}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

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
            JObject WatchList = Table2();

            JObject players = ApiRequest("?position=qb");

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];

            if (WatchList != null && WatchList.Count != 0)
            {
                ViewBag.WatchList = WatchList["cumulativeplayerstats"]["playerstatsentry"];
            }

            return View("AllPlayersView");
        }

        public ActionResult SearchPlayers(string pos, string player)
        {
            JObject players;

            if (pos == "QB")
            {
                if (player != null)
                {
                    players = ApiRequest("?position=qb", "&player=" + player.ToLower());
                }
                else
                {
                    players = ApiRequest("?position=qb&sort=stats.Yds.D");
                }

            }
            else if (pos == "WR")
            {
                if (player != null)
                {
                    players = ApiRequest("?position=wr", "&player=" + player.ToLower());
                }
                else
                {
                    players = ApiRequest("?position=wr&sort=stats.TD.D");
                }

            }
            else if (pos == "RB")
            {
                if (player != null)
                {
                    players = ApiRequest("?position=rb", "&player=" + player.ToLower());
                }
                else
                {
                    players = ApiRequest("?position=rb&sort=stats.Yds.D");
                }

            }
            else if (pos == "TE")
            {
                if (player != null)
                {
                    players = ApiRequest("?position=te", "&player=" + player.ToLower());
                }
                else
                {
                    players = ApiRequest("?position=te&sort=stats.Yds.D");
                }

            }
            else if (pos == "K")
            {
                if (player != null)
                {
                    players = ApiRequest("?position=k", "&player=" + player.ToLower());
                }
                else
                {
                    players = ApiRequest("?position=k&sort=stats.Made.D");
                }
            }
            else
            {
                if (player != null)
                {
                    players = ApiRequest("?position=qb,rb,wr,te,k", "&player=" + player.ToLower());
                }
                else
                {
                    players = ApiRequest("?position=qb,rb,wr,te,k");
                }
            }

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Pos = pos;



            JObject WatchList = Table2();

           

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];

            if (WatchList != null && WatchList.Count != 0)
            {
                ViewBag.WatchList = WatchList["cumulativeplayerstats"]["playerstatsentry"];
            }




            return View("AllPlayersView");
        }


        public ActionResult SavePlayer(string PlayerId)
        {
            FFLToolEntities1 ORM = new FFLToolEntities1();

            tblUserWatchlist watchList = new tblUserWatchlist();

            string userId = User.Identity.GetUserId();

            //watchList.UserId = userId;
            //ORM.tblUserWatchlists.Add(watchList);
            //ORM.SaveChanges();

            string currWatchList = (from UW in ORM.tblUserWatchlists
                                    where UW.UserId == userId
                                    select UW.WatchlistId).Max().ToString();

            tblWatchlist watchList2 = new tblWatchlist();
            watchList2.WatchlistId = Convert.ToInt64(currWatchList);
            watchList2.PlayerId = Convert.ToInt32(PlayerId);
            ORM.tblWatchlists.Add(watchList2);
            ORM.SaveChanges();
            return Redirect("ShowAllPlayers");

        }

        public JObject Table2()
        {
            FFLToolEntities1 ORM = new FFLToolEntities1();

            string uID = User.Identity.GetUserId();

            List<tblWatchlist> bob = (from u in ORM.tblWatchlists
                                      where u.WatchlistId == (from UW in ORM.tblUserWatchlists
                                                              where UW.UserId == uID
                                                              select UW.WatchlistId).Max()
                                      select u).ToList();

            string newPlayer = "";
            JObject apiDataJSON = new JObject();

            if (bob.Count != 0)
            {
                for (int i = 0; i < bob.Count; i++)
                {

                    newPlayer += bob[i].PlayerId.ToString();

                    if (i < bob.Count - 1)
                    {
                        newPlayer = newPlayer + ",";
                    }
                }

                HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?player={newPlayer}");
                WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
                WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
                WebReq.Method = "GET";

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                StreamReader reader = new StreamReader(WebResp.GetResponseStream());
                string apiData = reader.ReadToEnd();

                apiDataJSON = JObject.Parse(apiData);

                return apiDataJSON;
            }

            return apiDataJSON;
        }

        public ActionResult WatchList()
        {
            FFLToolEntities1 ORM = new FFLToolEntities1();

            string uID = User.Identity.GetUserId();

            List<tblWatchlist> bob = (from u in ORM.tblWatchlists
                                      where u.WatchlistId == (from UW in ORM.tblUserWatchlists
                                                              where UW.UserId == uID
                                                              select UW.WatchlistId).Max()
                                      select u).ToList();

            string newPlayer = "";

            for (int i = 0; i < bob.Count; i++)
            {

                newPlayer += bob[i].PlayerId.ToString();

                if (i < bob.Count - 1)
                {
                    newPlayer = newPlayer + ",";
                }
            }

            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/cumulative_player_stats.json?player={newPlayer}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            ViewBag.WatchList = apiDataJSON["cumulativeplayerstats"]["playerstatsentry"];


            return View();
        }
        
        public ActionResult NewWatchlist ()
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return View("../Account/Login");
            }

            FFLToolEntities1 ORM = new FFLToolEntities1();

            tblUserWatchlist watchList = new tblUserWatchlist();

            userId = User.Identity.GetUserId();

            watchList.UserId = userId;
            ORM.tblUserWatchlists.Add(watchList);
            ORM.SaveChanges();

<<<<<<< HEAD


=======
            return RedirectToAction("ShowAllPlayers");
        }
>>>>>>> ee5cf5dc30a2a89311e24c9cbe0d5e7bd705aad3
    }

}