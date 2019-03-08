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
using GC_FinalProject_FFLTool.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace GC_FinalProject_FFLTool.Controllers
{
    public class HomeController : CustomBaseController //this was changed by sscobie
    {

        private string currentSeason = "2018-regular";
        YflDAL DAL = new YflDAL();

        /*** BEGIN TESTING AREA ***/

        public ActionResult TestReq ()
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            tblJsonDump apiData = (from data in ORM.tblJsonDump
                                   where data.ImportId == 1
                                   select data).Single();

            JObject apiDataJSON = JObject.Parse(apiData.MySportsFeedsData2018);

            ViewBag.apiDataJSON = apiDataJSON["cumulativeplayerstats"]["playerstatsentry"];

            return View("TestMe");
        }

        public ActionResult TestReqPos(string pos, string stat = "PassYards")
        {
            pos = "QB";
            string season = "2018";

            JObject apiDataJSON = DAL.GetCumulativeData(season);

            JArray tempData = new JArray();
            for (int i = 0; i < apiDataJSON["cumulativeplayerstats"]["playerstatsentry"].Count(); i++)
            {
                if (apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][i]["player"]["Position"].ToString() == pos)
                {
                    tempData.Add(apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][i]);
                }
            }

            JArray outData = new JArray(tempData.OrderByDescending( obj => Int32.Parse(obj["stats"][stat]["#text"].ToString())));
            ViewBag.theData = outData;
            return View("TestMe");
        }

        public ActionResult TestReqPosPlayer(string pos, List<string> players)
        {
            pos = "QB";
            players = new List<string>();
            players.Add("6825");

            JObject apiDataJSON = DAL.GetCumulativeData("2018");

            JArray outData = new JArray();
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < apiDataJSON["cumulativeplayerstats"]["playerstatsentry"].Count(); j++)
                {
                    if (apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]["player"]["ID"].ToString() == players[i])
                    {
                        outData.Add(apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]);
                    }
                }
            }

            // JArray outData = new JArray(tempData.OrderByDescending(obj => Int32.Parse(obj["stats"][stat]["#text"].ToString())));
            ViewBag.theData = outData;
            return View("TestMe");
        }

        public ActionResult TestReqHistorical(string season, List<string> players)
        {
            players = new List<string>();
            players.Add("6825");
            season = "2017";

            JObject apiDataJSON = DAL.GetCumulativeData(season);

            JArray outData = new JArray();
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < apiDataJSON["cumulativeplayerstats"]["playerstatsentry"].Count(); j++)
                {
                    if (apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]["player"]["ID"].ToString() == players[i])
                    {
                        outData.Add(apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]);
                    }
                }
            }

            ViewBag.theData = outData;
            return View("TestMe");
        }

        public ActionResult TestReqPlayerLogs(string season, List<string> players)
        {
            players = new List<string>();
            players.Add("6825");
            players.Add("13349");
            season = "2018";

            string[] tempJSON = DAL.GetPlayerLogData(season);
            JObject apiDataJSON = new JObject();
            JArray outData = new JArray();

            foreach (string team in tempJSON)
            {

                apiDataJSON = JObject.Parse(team);

                for (int i = 0; i < players.Count; i++)
                {
                    for (int j = 0; j < apiDataJSON["playergamelogs"]["gamelogs"].Count(); j++)
                    {
                        if (apiDataJSON["playergamelogs"]["gamelogs"][j]["player"]["ID"].ToString() == players[i])
                        {
                            outData.Add(apiDataJSON["playergamelogs"]["gamelogs"][j]);
                        }
                    }
                }
            }


            ViewBag.theData = outData;
            return View("TestMe");
        }


        /*** END TESTING AREA ***/

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public JObject ApiRequest(string pos, string player)
        {

            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/{currentSeason}/cumulative_player_stats.json{pos}{player}");
            WebReq.Headers.Add("Authorization", "Basic " + ConfigurationManager.AppSettings["AccessKey"]);
            WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            StreamReader reader = new StreamReader(WebResp.GetResponseStream());
            string apiData = reader.ReadToEnd();

            JObject apiDataJSON = JObject.Parse(apiData);

            return apiDataJSON;
        }

        public JArray DataRequestHistorical(string season, List<string> players)
        {

            JObject apiDataJSON = DAL.GetCumulativeData(season);

            JArray outData = new JArray();
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < apiDataJSON["cumulativeplayerstats"]["playerstatsentry"].Count(); j++)
                {
                    if (apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]["player"]["ID"].ToString() == players[i])
                    {
                        outData.Add(apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]);
                    }
                }
            }

            return outData;
        }

        public JArray DataRequestPlayerLogs(string season, List<string> players)
        {

            string[] tempJSON = DAL.GetPlayerLogData(season);
            JObject apiDataJSON = new JObject();
            JArray outData = new JArray();

            foreach (string team in tempJSON)
            {

                apiDataJSON = JObject.Parse(team);

                for (int i = 0; i < players.Count; i++)
                {
                    for (int j = 0; j < apiDataJSON["playergamelogs"]["gamelogs"].Count(); j++)
                    {
                        if (apiDataJSON["playergamelogs"]["gamelogs"][j]["player"]["ID"].ToString() == players[i])
                        {
                            outData.Add(apiDataJSON["playergamelogs"]["gamelogs"][j]);
                        }
                    }
                }
            }

            return outData;
        }

        public JArray DataRequestPlayers(List<string> players, string season = "2018")
        {

            JObject apiDataJSON = DAL.GetCumulativeData(season);

            JArray outData = new JArray();
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < apiDataJSON["cumulativeplayerstats"]["playerstatsentry"].Count(); j++)
                {
                    if (apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]["player"]["ID"].ToString() == players[i])
                    {
                        outData.Add(apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][j]);
                    }
                }
            }

            return outData;
        }

        public JArray DataRequestPos(string pos, string stat)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            tblJsonDump apiData = (from data in ORM.tblJsonDump
                                   where data.ImportId == 1
                                   select data).Single();

            JObject apiDataJSON = JObject.Parse(apiData.MySportsFeedsData2018);

            JArray tempData = new JArray();
            if (pos.ToUpper() == "ALL")
            {
                return new JArray(apiDataJSON);
            }
            else
            {
                for (int i = 0; i < apiDataJSON["cumulativeplayerstats"]["playerstatsentry"].Count(); i++)
                {
                    if (apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][i]["player"]["Position"].ToString() == pos)
                    {
                        tempData.Add(apiDataJSON["cumulativeplayerstats"]["playerstatsentry"][i]);
                    }
                }

                JArray outData = new JArray(tempData.OrderByDescending( obj => Int32.Parse(obj["stats"][stat]["#text"].ToString()) ));
                return outData;
            }
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
            HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.1/pull/nfl/{currentSeason}/full_game_schedule.json?date=from-20171226-to-20180101");
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
            tblWatchlists watchlist = new tblWatchlists();
            JArray WatchList = new JArray();

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

            // Defaults to QB for position
            JArray players = DataRequestPos("QB", "PassYards");

            ViewBag.Players = players;
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

            // used dynamic here until a single type is defined consistently
            dynamic players;
            List<string> playerList = player.Split(',').ToList();

            if (pos == "QB")
            {
                if (player != null)
                {
                    players = DataRequestPlayers(playerList);
                }
                else
                {
                    players = DataRequestPos(pos, "PassYards");
                }

            }
            else if (pos == "WR")
            {
                if (player != null)
                {
                    players = DataRequestPlayers(playerList);
                }
                else
                {
                    players = DataRequestPos(pos, "RecYards");
                }

            }
            else if (pos == "RB")
            {
                if (player != null)
                {
                    players = DataRequestPlayers(playerList);
                }
                else
                {
                    players = DataRequestPos(pos, "RushYards");
                }

            }
            else if (pos == "TE")
            {
                if (player != null)
                {
                    players = DataRequestPlayers(playerList);
                }
                else
                {
                    players = DataRequestPos(pos, "RecYards");
                }

            }
            else if (pos == "K")
            {
                if (player != null)
                {
                    players = DataRequestPlayers(playerList);
                }
                else
                {
                    players = DataRequestPos(pos, "FgMade");
                }
            }
            else
            {
                if (player != null)
                {
                    players = DataRequestPlayers(playerList);
                }
                else
                {
                    players = DataRequestPos("ALL", "PassYards");
                }
            }

            ViewBag.Players = players;
            ViewBag.Pos = pos;

            JArray WatchList = new JArray();

            if (watchlistId != null)
            {
                WatchList = Table2(watchlistId);

            }

            ViewBag.Players = players;
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

            tblWatchlists watchList = new tblWatchlists();
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

            tblWatchlists watchlist = new tblWatchlists();
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

        public JArray Table2(string watchlistId)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            string uID = User.Identity.GetUserId();

            List<tblWatchlists> wList = new List<tblWatchlists>();
            if (watchlistId != null)
            {
                long int_watchlistId = Int64.Parse(watchlistId);
                wList = (from u in ORM.tblWatchlists
                             where u.WatchlistId == int_watchlistId
                             select u).ToList();
            }

            JArray watchListPlayers = new JArray();
            List<string> playerList = new List<string>();

            if (wList.Count != 0)
            {
                foreach (var player in wList)
                {

                    playerList.Add(player.PlayerId.ToString());
                }

                watchListPlayers = DataRequestPlayers(playerList);

                return watchListPlayers;
            }

            return watchListPlayers;
        }

        public ActionResult WatchListManagement(string watchListName)
        {

            FFLToolEntities2 ORM = new FFLToolEntities2();

            string uID = User.Identity.GetUserId();

            if (uID == null)
            {
                return View("../Account/Login");
            }

            List<tblUserWatchlists> userWatchlists = ORM.tblUserWatchlists.Where(x => x.UserId == uID).Distinct().ToList();

            List<tblWatchlists> WL = ORM.tblWatchlists.Where(x => x.WatchlistName != null).ToList();

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
            List<tblWatchlists> watchlist = (from u in ORM.tblWatchlists
                                            where u.WatchlistId == int_watchlistId
                                            select u).ToList();

            List<string> newPlayer = new List<string>();
            foreach (var player in watchlist)
            {
                newPlayer.Add(player.PlayerId.ToString());
            }

            JArray playersCurr, players2017, players2016, players2015, players2014, playerlogsCurr, playerlogs2017, playerlogs2016, playerlogs2015, playerlogs2014;
            JObject sched;

            /* Summary stats */
            playersCurr = DataRequestHistorical("2018", newPlayer);
            players2017 = DataRequestHistorical("2017", newPlayer);
            players2016 = DataRequestHistorical("2016", newPlayer);
            players2015 = DataRequestHistorical("2015", newPlayer);
            players2014 = DataRequestHistorical("2014", newPlayer);


            ViewBag.PlayersCurr = playersCurr["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2017 = players2017["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2016 = players2016["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2015 = players2015["cumulativeplayerstats"]["playerstatsentry"];
            ViewBag.Players2014 = players2014["cumulativeplayerstats"]["playerstatsentry"];

            /* Upcoming opponent */
            sched = ApiRequestSchedule();
            ViewBag.CurrOpp = sched["fullgameschedule"]["gameentry"];

            /* Game log stats */
            playerlogsCurr = DataRequestPlayerLogs("2018", newPlayer);
            playerlogs2017 = DataRequestPlayerLogs("2017", newPlayer);
            playerlogs2016 = DataRequestPlayerLogs("2016", newPlayer);
            playerlogs2015 = DataRequestPlayerLogs("2015", newPlayer);
            playerlogs2014 = DataRequestPlayerLogs("2014", newPlayer);

            ViewBag.PlayersLogsCurr = playerlogsCurr["playergamelogs"]["gamelogs"];
            ViewBag.PlayersLogs2017 = playerlogs2017["playergamelogs"]["gamelogs"];
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

            tblUserWatchlists watchList = new tblUserWatchlists();

            userId = User.Identity.GetUserId();

            watchList.UserId = userId;
            ORM.tblUserWatchlists.Add(watchList);
            ORM.SaveChanges();

            return RedirectToAction("ShowAllPlayers");
        }

        public ActionResult DropPlayer(string watchlistId, string playerId)
        {
            FFLToolEntities2 ORM = new FFLToolEntities2();

            tblWatchlists watchlist = ORM.tblWatchlists.Find(Convert.ToInt64(watchlistId), Convert.ToInt32(playerId));

            ORM.tblWatchlists.Remove(watchlist);
            ORM.SaveChanges();

            JObject delPlayerJSON = ApiRequestHistorical(currentSeason, "?player=" + playerId);
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