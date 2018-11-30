using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net;
using System.IO;
using System.Collections;

public partial class StoredProcedures
{

    /* API call for all data */
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void MySF_ApiRequest_Position_QB_RB_WR_TE_K (string season)
    {
        /*** You must set permissions so that 'Everyone' user has access to the directory cited below. ***/
        /*** Updated credentials from J Snovers to my new account on 4/7/2018                          ***/

        HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.2/pull/nfl/{season}/cumulative_player_stats.json?position=qb,rb,wr,te,k");
        WebReq.Headers.Add("Authorization", "");
        WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
        WebReq.Method = "GET";

        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
        StreamReader reader = new StreamReader(WebResp.GetResponseStream());
        string apiData = reader.ReadToEnd();
        File.WriteAllText($"C:\\Users\\sscobie\\Documents\\Visual Studio 2017\\Projects\\GC_FinalProject_FFLTool\\SQL\\cumulativestats_nfl_{season}.txt", apiData);

        WebResp.Close();
        reader.Close();
    }

    /* API call for player logs by team */
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void MySF_ApiRequest_PlayerLogs (string season, string team)
    {
        /*** You must set permissions so that 'Everyone' user has access to the directory cited below. ***/
        /*** Updated credentials from J Snovers to my new account on 4/7/2018                          ***/

        HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.2/pull/nfl/{season}/player_gamelogs.json?team={team}");
        WebReq.Headers.Add("Authorization", "");
        WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
        WebReq.Method = "GET";

        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
        StreamReader reader = new StreamReader(WebResp.GetResponseStream());
        string apiData = reader.ReadToEnd();
        File.WriteAllText($"C:\\Users\\sscobie\\Documents\\Visual Studio 2017\\Projects\\GC_FinalProject_FFLTool\\SQL\\response_nfl_{season}_playerlogs_{team}.txt", apiData);

        WebResp.Close();
        reader.Close();
    }

    /* API call for current week's schedule */
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void MySF_ApiRequest_Schedules (string season)
    {
        /*** You must set permissions so that 'Everyone' user has access to the directory cited below. ***/
        /*** Updated credentials from J Snovers to my new account on 4/7/2018                          ***/

        HttpWebRequest WebReq = WebRequest.CreateHttp($"https://api.mysportsfeeds.com/v1.2/pull/nfl/{season}/full_game_schedule.json?date=from-20171226-to-20180101");
        WebReq.Headers.Add("Authorization", "");
        WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
        WebReq.Method = "GET";

        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
        StreamReader reader = new StreamReader(WebResp.GetResponseStream());
        string apiData = reader.ReadToEnd();
        File.WriteAllText("C:\\Users\\sscobie\\Documents\\Visual Studio 2017\\Projects\\GC_FinalProject_FFLTool\\SQL\\response_nfl_2017reg_schedule.txt", apiData);

        WebResp.Close();
        reader.Close();
    }

    /* Combine playerlogs files into a singel file */
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void MySF_CombinePlayerLogs (string teams, string season)
    {
        string dirName = "C:\\Users\\sscobie\\Documents\\Visual Studio 2017\\Projects\\GC_FinalProject_FFLTool\\SQL\\";
        string sourceFileName;
        string targetFileName;
        string playerLogsText;
        string[] teamsArr;
        string team;

        targetFileName = dirName + $"combined_nfl_{season}-regular_playerlogs.txt";
        if (File.Exists(targetFileName))
        {
            File.Delete(targetFileName);
        }

        teamsArr = teams.Split(new char[] { ',' });
        for (int i = 0; i < teamsArr.Length; i++)
        {

            team = teamsArr[i];
            sourceFileName = dirName + $"response_nfl_{season}-regular_playerlogs_{team}.txt";

            if (File.Exists(sourceFileName))
            {
                if (i != teamsArr.Length - 1)
                {
                    playerLogsText = File.ReadAllText(sourceFileName) + "|";
                }
                else
                {
                    //if it's the last element of the array, don't append the delimiter
                    playerLogsText = File.ReadAllText(sourceFileName);
                }

                File.AppendAllText(targetFileName, playerLogsText);
                //File.Delete(sourceFileName);
            }
        }
    }
}