using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net;
using System.IO;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void MySF_ApiRequest_Position_QB_RB_WR_TE_K ()
    {
        /*** You must set permissions so that 'Everyone' user has access to the directory cited below. ***/
        /*** Updated credentials from J Snovers to my new account on 4/7/2018                          ***/

        HttpWebRequest WebReq = WebRequest.CreateHttp("https://api.mysportsfeeds.com/v1.2/pull/nfl/2017-regular/cumulative_player_stats.json?position=qb,rb,wr,te,k");
        WebReq.Headers.Add("Authorization", "Basic " + "");
        WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
        WebReq.Method = "GET";

        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
        StreamReader reader = new StreamReader(WebResp.GetResponseStream());
        string apiData = reader.ReadToEnd();
        File.WriteAllText("C:\\Users\\sscobie\\Documents\\Visual Studio 2017\\Projects\\GC_FinalProject_FFLTool\\SQL\\response_nfl_2017reg.txt", apiData);

        WebResp.Close();
        reader.Close();
    }
}
