using System;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Leaderboard
{
    public class PlayFabGetLeaderboard
    {
        public event Action<string> OnSuccess;
        
        public void GetLeaderboardEntries(int startPosition, int maxResultsCount, string leaderboardName)
        {
            var request = new GetLeaderboardRequest
                          {
                              StatisticName = leaderboardName,
                              StartPosition = startPosition,
                              MaxResultsCount = maxResultsCount,
                          };
            PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
        }

        private void OnGetLeaderboardFailure(PlayFabError error)
        {
            Debug.Log($"Here's some debug information: {error.GenerateErrorReport()}");
        }

        private void OnGetLeaderboardSuccess(GetLeaderboardResult response)
        {
            var leaderboard = new StringBuilder();
            // Formato de alineación con 10 caracteres de espacio y alineación a la izquierda
            //string format = "{0,10} | {1,-35} | {2,18}";
            //string format = "{0,10}{1,30}{2,18}";
            string format = "{0,8}{1}{2}";
            foreach (var playerLeaderboardEntry in response.Leaderboard)
            {
                leaderboard.AppendLine($"{playerLeaderboardEntry.Position + 1}.                      {playerLeaderboardEntry.DisplayName}                           {playerLeaderboardEntry.StatValue}");
                //leaderboard.AppendLine(String.Format(format, (playerLeaderboardEntry.Position + 1).ToString().PadLeft(10), (playerLeaderboardEntry.DisplayName).PadRight(30), (playerLeaderboardEntry.StatValue).ToString().PadLeft(18)));

            }

            OnSuccess?.Invoke(leaderboard.ToString());
        }
    }
}
