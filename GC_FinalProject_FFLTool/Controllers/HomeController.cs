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
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using GC_FinalProject_FFLTool.Models;
using Microsoft.AspNet.Identity;

namespace GC_FinalProject_FFLTool.Controllers
{
    public class HomeController : CustomBaseController //this was changed by sscobie
    {

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public JObject ApiRequest(string pos, string player)
        {

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

        public JObject ApiRequestHistorical(string season, string playerIds)
        {
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/{season}/cumulative_player_stats.json{playerIds}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            return apiDataJSON;
        }

        public JObject ApiRequestPlayerLogs(string season, string playerIds)
        {
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/{season}/player_gamelogs.json{playerIds}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            return apiDataJSON;
        }

        public JObject ApiRequestSchedule()
        {
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/current/full_game_schedule.json?date=from-20171226-to-20180101");
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

        public ActionResult ShowAllPlayers(string watchlistId, string watchListName)
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return View("../Account/Login");
            }

            FFLToolEntities2 ORM = new FFLToolEntities2();
            tblWatchlist watchlist = new tblWatchlist();
            JObject WatchList = new JObject();
            if (watchListName != null)
            {

                watchlist.WatchlistName = watchListName.Trim();

                //need to error handle gracefully
                watchlist.WatchlistId = (from W in ORM.tblWatchlists
                                         where W.WatchlistName == watchListName.Trim()
                                         select W.WatchlistId).Distinct().Single();

                WatchList = Table2(watchlist.WatchlistId.ToString());

                ViewBag.WatchListID = watchlist.WatchlistId;
            }


            JObject players = ApiRequest("?position=qb");

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.UserWatchlists = DropdownWatchLists();

            if (WatchList != null && WatchList.Count != 0)
            {
                ViewBag.WatchList = WatchList["cumulativeplayerstats"]["playerstatsentry"];
            }

            ViewBag.WatchlistName = watchListName;

            if (watchListName != null)
            {
                ViewBag.WatchlistName = watchListName;
            }

            return View("AllPlayersView");
        }

        public ActionResult SearchPlayers(string pos, string player, string watchlistId)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

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

            JObject WatchList = new JObject();

            if (watchlistId != null)
            {
                WatchList = Table2(watchlistId);

            }

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.UserWatchlists = DropdownWatchLists();

            if (WatchList != null && WatchList.Count != 0)
            {
                ViewBag.WatchList = WatchList["cumulativeplayerstats"]["playerstatsentry"];
            }

            return View("AllPlayersView");
        }

        public ActionResult SavePlayerToNewList(string PlayerId, string watchListName)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            string name = watchListName;
            string userId = User.Identity.GetUserId();
            string currWatchList = (from UW in ORM.tblUserWatchlists
                                    where UW.UserId == userId
                                    select UW.WatchlistId).Max().ToString();

            tblWatchlist watchList = new tblWatchlist();
            watchList.WatchlistId = Convert.ToInt64(currWatchList);
            watchList.PlayerId = Convert.ToInt32(PlayerId);
            watchList.WatchlistName = watchListName;

            try
            {
                ORM.tblWatchlists.Add(watchList);
                ORM.SaveChanges();
            }
            catch
            {
                return RedirectToAction("ShowAllPlayers", new { watchListName = name });
            }

            return RedirectToAction("ShowAllPlayers", new { watchListName = name });
        }

        public ActionResult SavePlayerToExistingList(string PlayerId, string watchListName)
        {

            string name = watchListName;
            FFLToolEntities2 ORM = new FFLToolEntities2();

            tblWatchlist watchlist = new tblWatchlist();
            watchlist.PlayerId = Convert.ToInt32(PlayerId);
            watchlist.WatchlistName = watchListName.Trim();

            //need to error handle gracefully
            watchlist.WatchlistId = (from W in ORM.tblWatchlists
                                     where W.WatchlistName == watchListName.Trim()
                                     select W.WatchlistId).Distinct().Single();

            try
            {
                ORM.tblWatchlists.Add(watchlist);
                ORM.SaveChanges();
            }
            catch
            {
                return RedirectToAction("ShowAllPlayers", new { watchListName = name });
            }


            return RedirectToAction("ShowAllPlayers", new { watchListName = name });
        }

        public List<string> DropdownWatchLists()
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            string userId = User.Identity.GetUserId();
            List<string> lstWatchLists = (from UW in ORM.tblUserWatchlists
                                          join W in ORM.tblWatchlists
                                          on UW.WatchlistId equals W.WatchlistId
                                          where UW.UserId == userId
                                          select W.WatchlistName).Distinct().ToList();

            lstWatchLists.Insert(0, "New List");

            return lstWatchLists;
        }

        public JObject Table2(string watchlistId)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            string uID = User.Identity.GetUserId();

            List<tblWatchlist> bob = new List<tblWatchlist>();
            if (watchlistId != null)
            {
                long int_watchlistId = Int64.Parse(watchlistId);
                bob = (from u in ORM.tblWatchlists
                       where u.WatchlistId == int_watchlistId
                       select u).ToList();
            }


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

        public ActionResult WatchListManagement(string watchListName)
        {

            FFLToolEntities2 ORM = new FFLToolEntities2();

            string uID = User.Identity.GetUserId();

            if (uID == null)
            {
                return View("../Account/Login");
            }

            List<tblUserWatchlist> userWatchlists = ORM.tblUserWatchlists.Where(x => x.UserId == uID).Distinct().ToList();

            List<tblWatchlist> WL = ORM.tblWatchlists.Where(x => x.WatchlistName != null).ToList();

            List<string> watchlists = new List<string>();
            List<string> watchlistId = new List<string>();

            for (int i = 0; i < userWatchlists.Count; i++)
            {

                foreach (var item in WL)
                {

                    if (userWatchlists[i].WatchlistId == item.WatchlistId && !watchlists.Contains(item.WatchlistName))
                    {

                        watchlists.Add(item.WatchlistName);
                        watchlistId.Add(item.WatchlistId.ToString());

                    }
                }
            }

            if (watchListName != null)
            {
                ViewBag.WatchListName = watchListName;
            }

            ViewBag.WatchList = watchlists;
            ViewBag.WatchlistId = watchlistId;

            return View();
        }

        public ActionResult WatchList(string watchlistId, string watchlistName, string deletedPlayer = null)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            string userId = User.Identity.GetUserId();
            long int_watchlistId = Int64.Parse(watchlistId);
            List<tblWatchlist> watchlist = (from u in ORM.tblWatchlists
                                            where u.WatchlistId == int_watchlistId
                                            select u).ToList();

            string newPlayer = "";
            for (int i = 0; i < watchlist.Count; i++)
            {
                newPlayer += watchlist[i].PlayerId.ToString();

                if (i < watchlist.Count - 1)
                {
                    newPlayer = newPlayer + ",";
                }
            }

            JObject playersCurr, players2016, players2015, players2014, sched, playerlogsCurr, playerlogs2016, playerlogs2015, playerlogs2014;

            /* Summary stats */
            playersCurr = ApiRequestHistorical("current", "?player=" + newPlayer);
            players2016 = ApiRequestHistorical("2016-2017-regular", "?player=" + newPlayer);
            players2015 = ApiRequestHistorical("2015-2016-regular", "?player=" + newPlayer);
            players2014 = ApiRequestHistorical("2014-2015-regular", "?player=" + newPlayer);

            ViewBag.PlayersCurr = playersCurr["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2016 = players2016["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2015 = players2015["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2014 = players2014["cumulativeplayerstats"]["playerstatsentry"];

            /* Upcoming opponent */
            sched = ApiRequestSchedule();
            ViewBag.CurrOpp = sched["fullgameschedule"]["gameentry"];

            /* Game log stats */
            playerlogsCurr = ApiRequestPlayerLogs("current", "?player=" + newPlayer);
            playerlogs2016 = ApiRequestPlayerLogs("2016-2017-regular", "?player=" + newPlayer);
            playerlogs2015 = ApiRequestPlayerLogs("2015-2016-regular", "?player=" + newPlayer);
            playerlogs2014 = ApiRequestPlayerLogs("2014-2015-regular", "?player=" + newPlayer);

            ViewBag.PlayersLogsCurr = playerlogsCurr["playergamelogs"]["gamelogs"];
            ViewBag.PlayersLogs2016 = playerlogs2016["playergamelogs"]["gamelogs"];
            ViewBag.PlayersLogs2015 = playerlogs2015["playergamelogs"]["gamelogs"];
            ViewBag.PlayersLogs2014 = playerlogs2014["playergamelogs"]["gamelogs"];

            ViewBag.WatchlistName = watchlistName;
            ViewBag.WatchlistId = watchlistId;
            ViewBag.DeletedPlayer = deletedPlayer;

            return View();
        }

        public ActionResult NewWatchlist()
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return View("../Account/Login");
            }

            FFLToolEntities2 ORM = new FFLToolEntities2();

            tblUserWatchlist watchList = new tblUserWatchlist();

            userId = User.Identity.GetUserId();

            watchList.UserId = userId;
            ORM.tblUserWatchlists.Add(watchList);
            ORM.SaveChanges();

            return RedirectToAction("ShowAllPlayers");
        }

        public ActionResult DropPlayer(string watchlistId, string playerId)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            tblWatchlist watchlist = ORM.tblWatchlists.Find(Convert.ToInt64(watchlistId), Convert.ToInt32(playerId));

            ORM.tblWatchlists.Remove(watchlist);
            ORM.SaveChanges();

            JObject delPlayerJSON = ApiRequestHistorical("current", "?player=" + playerId);
            string delPlayer = (string)delPlayerJSON["cumulativeplayerstats"]["playerstatsentry"][0]["player"]["FirstName"] + " " + (string)delPlayerJSON["cumulativeplayerstats"]["playerstatsentry"][0]["player"]["LastName"];

            return RedirectToAction("WatchList", new
            {
                WatchlistId = watchlist.WatchlistId.ToString(),
                WatchlistName = watchlist.WatchlistName,
                DeletedPlayer = delPlayer
            });
        }

        public ActionResult DeleteWatchList(string watchlistId, string watchListName)
        {
            long ID = Int64.Parse(watchlistId);
            FFLToolEntities2 ORM = new FFLToolEntities2();

            ORM.tblUserWatchlists.Remove(ORM.tblUserWatchlists.Find(ID, User.Identity.GetUserId()));

            ORM.SaveChanges();

            string name = watchListName;

            return RedirectToAction("WatchListManagement", new { watchListName = name });
        }

    }
}