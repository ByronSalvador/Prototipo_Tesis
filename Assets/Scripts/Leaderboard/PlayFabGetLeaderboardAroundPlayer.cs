using System;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Leaderboard
{
    public class PlayFabGetLeaderboardAroundPlayer
    {
        public event Action<string> OnSuccess;
        
        public void GetLeaderboardAroundPlayer(string playerId, int maxResultsCount, string leaderboardName)
        {
            var request = new GetLeaderboardAroundPlayerRequest
                          {
                              PlayFabId = playerId,
                              MaxResultsCount = maxResultsCount,
                              StatisticName = leaderboardName
                          };
            PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, GetLeaderboardAroundPlayerFailure);
        }
        
        private void GetLeaderboardAroundPlayerFailure(PlayFabError error)
        {
            Debug.Log($"Here's some debug information: {error.GenerateErrorReport()}");
        }

        private void OnGetLeaderboardAroundPlayerSuccess(GetLeaderboardAroundPlayerResult response)
        {
            var leaderboard = new StringBuilder();
            
            // Formato de alineación con 10 caracteres de espacio y alineación a la izquierda
            string formato = "{0,10}{1,35}{2,18}";
            
            foreach (var playerLeaderboardEntry in response.Leaderboard)
            {
                //leaderboard.AppendLine($"{playerLeaderboardEntry.Position + 1}                     {playerLeaderboardEntry.DisplayName}                             {playerLeaderboardEntry.StatValue}");
                leaderboard.AppendLine(String.Format(formato, playerLeaderboardEntry.Position + 1, playerLeaderboardEntry.DisplayName, playerLeaderboardEntry.StatValue));
            }
            OnSuccess?.Invoke(leaderboard.ToString());
            
        }
    }
}
