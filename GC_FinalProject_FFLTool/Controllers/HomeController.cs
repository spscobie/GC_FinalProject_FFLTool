﻿using System;
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
            ViewBag.UserWatchlists = DropdownWatchLists();

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

        public ActionResult SavePlayerToNewList(string PlayerId)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            string userId = User.Identity.GetUserId();
            string currWatchList = (from UW in ORM.tblUserWatchlists
                                    where UW.UserId == userId
                                    select UW.WatchlistId).Max().ToString();

            tblWatchlist watchList = new tblWatchlist();
            watchList.WatchlistId = Convert.ToInt64(currWatchList);
            watchList.PlayerId = Convert.ToInt32(PlayerId);
            watchList.WatchlistName = "New Watch List";

            try
            {
                ORM.tblWatchlists.Add(watchList);
                ORM.SaveChanges();
            }
            catch
            {
                return Redirect("ShowAllPlayers");
            }

            return Redirect("ShowAllPlayers");
        }

        public ActionResult SavePlayerToExistingList(string PlayerId, string watchListName)
        {
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
                return Redirect("ShowAllPlayers");
            }


            return Redirect("ShowAllPlayers");
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

        public JObject Table2()
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

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

        public ActionResult WatchListManagement()
        {

            FFLToolEntities2 ORM = new FFLToolEntities2();

            string uID = User.Identity.GetUserId();

            if (uID == null)
            {
                return View("../Account/Login");
            }

            List<tblUserWatchlist> userWatchlists = ORM.tblUserWatchlists.Where(x => x.UserId == uID).Distinct().ToList();

            List<tblWatchlist> WL = ORM.tblWatchlists.Where(x => x.WatchlistName != null).ToList();

            //List<tblWatchlist> watchlists = new List<tblWatchlist>();

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

            ViewBag.WatchList = watchlists;
            ViewBag.WatchlistId = watchlistId;

            return View();
        }

        public ActionResult WatchList(string watchlistId)
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

            JObject playersCurr, players2016, players2015, players2014;
            playersCurr = ApiRequestHistorical("current", "?player=" + newPlayer);
            players2016 = ApiRequestHistorical("2016-2017-regular", "?player=" + newPlayer);
            players2015 = ApiRequestHistorical("2015-2016-regular", "?player=" + newPlayer);
            players2014 = ApiRequestHistorical("2014-2015-regular", "?player=" + newPlayer);

            ViewBag.PlayersCurr = playersCurr["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2016 = players2016["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2015 = players2015["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2014 = players2014["cumulativeplayerstats"]["playerstatsentry"];

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
    }
}