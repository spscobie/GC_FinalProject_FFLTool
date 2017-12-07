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
            JObject players = ApiRequest("?position=qb,rb,wr,te,k");

            ViewBag.Players = players["cumulativeplayerstats"]["playerstatsentry"];

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

            return View("AllPlayersView");
        }

<<<<<<< HEAD
=======

        public ActionResult SavePlayers (string PlayerIds)
        {
            FFLToolEntities ORM = new FFLToolEntities();

            string un = User.Identity.GetUserId();

            tblUserWatchlist w = new tblUserWatchlist();

            w.UserId = un;
            ORM.tblUserWatchlists.Add(w);
            ORM.SaveChanges();

            tblWatchlist w2 = new tblWatchlist();
            
            string[] players = PlayerIds.Split(',');

            for ()

            return View("WatchlistView");
        }

            
            
>>>>>>> ddae049ba274c8e17c612d2b051d3cd68aee27ac
    }

}