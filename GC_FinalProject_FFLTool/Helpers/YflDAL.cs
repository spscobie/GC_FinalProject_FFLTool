using GC_FinalProject_FFLTool.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GC_FinalProject_FFLTool.Helpers
{

    public class YflDAL
    {

        private FFLToolEntities2 ORM = new FFLToolEntities2();

        public tblJsonDump GetAllData ()
        {

            tblJsonDump apiData = ORM.tblJsonDump.Where(data => data.ImportId == 1).Single();
            return apiData;
        }

        public JObject GetCumulativeData (string season)
        {

            switch (season)
            {

                case "2014":
                    return GetCumulativeData2014();
                case "2015":
                    return GetCumulativeData2015();
                case "2016":
                    return GetCumulativeData2016();
                case "2017":
                    return GetCumulativeData2017();
                case "2018":
                    return GetCumulativeData2018();
                default:
                    return null;
            }
        }

        public string[] GetPlayerLogData(string season)
        {

            switch (season)
            {

                case "2014":
                    return GetPlayerLogData2014();
                case "2015":
                    return GetPlayerLogData2015();
                case "2016":
                    return GetPlayerLogData2016();
                case "2017":
                    return GetPlayerLogData2017();
                case "2018":
                    return GetPlayerLogData2018();
                default:
                    return null;
            }
        }

        /***************************/
        /* Cumulative Data Methods */
        /***************************/
        public JObject GetCumulativeData2018 ()
        {

            vwMySportsFeedsData2018 apiData = ORM.vwMySportsFeedsData2018.Where(data => data.ImportId == 1).Single();
            return JObject.Parse(apiData.MySportsFeedsData2018);
        }

        public JObject GetCumulativeData2017()
        {

            vwMySportsFeedsData2017 apiData = ORM.vwMySportsFeedsData2017.Where(data => data.ImportId == 1).Single();
            return JObject.Parse(apiData.MySportsFeedsData2017);
        }

        public JObject GetCumulativeData2016()
        {

            vwMySportsFeedsData2016 apiData = ORM.vwMySportsFeedsData2016.Where(data => data.ImportId == 1).Single();
            return JObject.Parse(apiData.MySportsFeedsData2016);
        }

        public JObject GetCumulativeData2015()
        {

            vwMySportsFeedsData2015 apiData = ORM.vwMySportsFeedsData2015.Where(data => data.ImportId == 1).Single();
            return JObject.Parse(apiData.MySportsFeedsData2015);
        }

        public JObject GetCumulativeData2014()
        {

            vwMySportsFeedsData2014 apiData = ORM.vwMySportsFeedsData2014.Where(data => data.ImportId == 1).Single();
            return JObject.Parse(apiData.MySportsFeedsData2014);
        }

        /***************************/
        /* Player Log Data Methods */
        /***************************/
        public string[] GetPlayerLogData2018()
        {

            vwMySportsFeedsDataPlayerLogs2018 apiData = ORM.vwMySportsFeedsDataPlayerLogs2018.Where(data => data.ImportId == 1).Single();
            return apiData.MySportsFeedsDataPlayerLogs2018.Split(new char[] { '|' });
        }

        public string[] GetPlayerLogData2017()
        {

            vwMySportsFeedsDataPlayerLogs2017 apiData = ORM.vwMySportsFeedsDataPlayerLogs2017.Where(data => data.ImportId == 1).Single();
            return apiData.MySportsFeedsDataPlayerLogs2017.Split(new char[] { '|' });
        }

        public string[] GetPlayerLogData2016()
        {

            vwMySportsFeedsDataPlayerLogs2016 apiData = ORM.vwMySportsFeedsDataPlayerLogs2016.Where(data => data.ImportId == 1).Single();
            return apiData.MySportsFeedsDataPlayerLogs2016.Split(new char[] { '|' });
        }

        public string[] GetPlayerLogData2015()
        {

            vwMySportsFeedsDataPlayerLogs2015 apiData = ORM.vwMySportsFeedsDataPlayerLogs2015.Where(data => data.ImportId == 1).Single();
            return apiData.MySportsFeedsDataPlayerLogs2015.Split(new char[] { '|' });
        }

        public string[] GetPlayerLogData2014()
        {

            vwMySportsFeedsDataPlayerLogs2014 apiData = ORM.vwMySportsFeedsDataPlayerLogs2014.Where(data => data.ImportId == 1).Single();
            return apiData.MySportsFeedsDataPlayerLogs2014.Split(new char[] { '|' });
        }
    }
}