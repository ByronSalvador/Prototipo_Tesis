using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Leaderboard
{
    public class PlayFabUpdatePlayerStatistics
    {
        public void UpdatePlayerStatistics(string leaderboardName, int score)
        {
            var request = new UpdatePlayerStatisticsRequest
                          {
                              
                              Statistics = new List<StatisticUpdate>
                                           {
                                               new StatisticUpdate
                                               {
                                                   StatisticName = leaderboardName,
                                                   Value = score
                                               }
                                           },
                          };
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerStatisticsSuccess, OnUpdatePlayerStatisticsFailure);

        }

        private void OnUpdatePlayerStatisticsFailure(PlayFabError error)
        {
            Debug.Log($"Here's some debug information: {error.GenerateErrorReport()}");
        }

        private void OnUpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Updated");
        }
    }
}
