using System;
using PlayFab;
using PlayFab.ClientModels;

namespace Login {
    public interface ILogin {
        void Login(GetPlayerCombinedInfoRequestParams loginInfoParams, Action<LoginResult> loginSuccess, Action<PlayFabError> loginFailure, object loginParams);
    }
}
