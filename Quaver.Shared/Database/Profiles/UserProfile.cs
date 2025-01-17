using System;
using System.Collections.Generic;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.Server.Common.Objects;
using Quaver.Shared.Database.Scores;
using Quaver.Shared.Online;
using Quaver.Shared.Online.API.User;
using SQLite;

namespace Quaver.Shared.Database.Profiles
{
    public class UserProfile
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// </summary>
        [Ignore]
        public Dictionary<int, UserProfileStats> Stats { get; } = new Dictionary<int, UserProfileStats>();

        /// <summary>
        /// </summary>
        [Ignore]
        public bool IsOnline { get; set; }

        /// <summary>
        /// </summary>
        public UserProfile PopulateStats()
        {
            for (var mode = 0; mode < Qua.MAX_KEY_COUNT; mode++)
            {
                if (!IsOnline && Stats.ContainsKey(mode))
                {
                    Stats[mode].FetchStats();
                    continue;
                }

                if (!Stats.ContainsKey(mode))
                    Stats.Add(mode, new UserProfileStats(this, mode));
            }

            // Fetch stats for the online profile
            if (IsOnline && OnlineManager.Connected)
            {
                var stats = new APIRequestUsersFull(OnlineManager.Self.OnlineUser.Id).ExecuteRequest();

                for (var mode = 0; mode < Qua.MAX_KEY_COUNT; mode++)
                {
                    APIResponseUsersFullMode modeStats = null;

                    switch (mode)
                    {
                        case 4:
                            modeStats = stats.User.Keys4;
                            break;
                        case 7:
                            modeStats = stats.User.Keys7;
                            break;
                        default:
                            // throw new ArgumentOutOfRangeException();
                            modeStats = stats.User.Keys4;
                            break;
                    }

                    var currentMode = Stats[mode];

                    currentMode.GlobalRank = modeStats.GlobalRank;
                    currentMode.CountryRank = modeStats.CountryRank;
                    currentMode.OverallRating = modeStats.Stats.OverallPerformanceRating;
                    currentMode.OverallAccuracy = modeStats.Stats.OverallAccuracy;
                    currentMode.TotalScore = modeStats.Stats.RankedScore;
                    currentMode.MaxCombo = modeStats.Stats.MaxCombo;
                    currentMode.PlayCount = modeStats.Stats.PlayCount;
                    currentMode.FailCount = modeStats.Stats.FailCount;
                    currentMode.Scores = new List<Score>();
                    currentMode.JudgementCounts = new Dictionary<Judgement, int>
                    {
                        [Judgement.Marv] = modeStats.Stats.TotalMarv,
                        [Judgement.Perf] = modeStats.Stats.TotalPerf,
                        [Judgement.Great] = modeStats.Stats.TotalGreat,
                        [Judgement.Good] = modeStats.Stats.TotalGood,
                        [Judgement.Okay] = modeStats.Stats.TotalOkay,
                        [Judgement.Miss] = modeStats.Stats.TotalMiss
                    };
                }
            }

            return this;
        }
    }
}