using PlayFab;
using PlayFab.ClientModels;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Login
{
    public class PlayfabLoginManager : MonoBehaviour
    {
        [SerializeField] private LoginUi loginUi;
        [SerializeField] private RegisterUi registerUi;
        [SerializeField] private ResetPasswordUi resetPasswordUi;
        [SerializeField] private GameObject loginInProgress;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject resetPasswordPanel;
        //Manejo de Errores
        [SerializeField] private GameObject errorMessagePanel;
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private TextMeshProUGUI txtErrorTitle;
        [SerializeField] private GameObject cfmMessagePanel;
        [SerializeField] private TextMeshProUGUI txtCfm;
        [SerializeField] private TextMeshProUGUI txtCfmTitle;

        public void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;

            //Filling login credentials based on local saves
            //loginUi.username.text = PlayerPrefs.GetString(PlayFabConstants.SavedUsername, "");
            //loginUi.username.text = PlayerPrefs.GetString("displayName3", "");
            
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Usuario logeado");
            }
        }

        public void Login(ILogin loginMethod, object loginParams)
        {
            loginMethod.Login(loginInfoParams, OnLoginSuccess, OnLoginFailure, loginParams);
            loginInProgress.SetActive(true);
        }

        #region Email Login

        public void LoginWithEmail()
        {
            if (ValidateLoginData())
            {
                Login(new EmailLogin(), new EmailLogin.EmailLoginParams(loginUi.username.text, loginUi.password.text));
            }
        }

        private bool ValidateLoginData()
        {
            //Validating data
            string errorMessage = "";

            if (loginUi.username.text.Length < 5)
            {
                errorMessage = "El nombre de usuario debe tener almenos 5 caracteres";
            }
            else if (loginUi.password.text.Length < 8)
            {
                errorMessage = "La contraseña debe tener almenos 8 caracteres";
            }

            if (errorMessage.Length > 0)
            {
                Debug.Log(errorMessage);
                errorMessagePanel.SetActive(true);
                //errorText.text = errorMessage;
                txtErrorTitle.text = "Credenciales Inválidas";
                txtError.text = errorMessage;
                //EditorUtility.DisplayDialog("Credenciales Inválidas", errorMessage, "Aceptar");
                return false;
            }

            return true;
        }

        #endregion

        #region Reset Password

        public void ResetPassword()
        {
            if (!ValidateResetPasswordData()) return;

            var request = new SendAccountRecoveryEmailRequest
            {
                TitleId = PlayFabConstants.TitleID,
                Email = resetPasswordUi.email.text,
                //EmailTemplateId = PlayFabConstants.EmailTemplateID,
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnResetPasswordSuccess, OnResetPasswordFailure);

            loginInProgress.SetActive(true);
        }

        bool ValidateResetPasswordData()
        {
            //Validating data
            string errorMessage = "";

            if (!resetPasswordUi.email.text.Contains("@"))
            {
                errorMessage = "E-mail inválido";
            }
            else if (resetPasswordUi.email.text.Length < 5)
            {
                errorMessage = "El e-mail debe tener almenos 5 caracteres";
            }

            if (errorMessage.Length > 0)
            {
                Debug.Log(errorMessage);
                errorMessagePanel.SetActive(true);
                txtErrorTitle.text = "Email Inválido";
                txtError.text = errorMessage;
                //EditorUtility.DisplayDialog("Email Inválido", errorMessage, "Aceptar");
                return false;
            }

            return true;
        }

        private void OnResetPasswordSuccess(SendAccountRecoveryEmailResult result)
        {
            Debug.Log("Reset Password Success!");
            errorMessagePanel.SetActive(true);
            txtErrorTitle.text = "Contraseña Restablecida";
            txtError.text = "Se ha enviado un correo a su cuenta de correo electrónico";
            //EditorUtility.DisplayDialog("Contraseña Restablecida","Se ha enviado un correo a su cuenta de correo electrónico", "Aceptar");
            loginInProgress.SetActive(false);
            loginPanel.SetActive(true);
            resetPasswordPanel.SetActive(false);
        }

        private void OnResetPasswordFailure(PlayFabError error)
        {
            Debug.Log("Reset Password failure: " + error.Error + "  " + error.ErrorDetails + error + "  " +
                      error.ApiEndpoint + "  " + error.ErrorMessage);
            errorMessagePanel.SetActive(true);
            txtErrorTitle.text = "Error";
            txtError.text = "No se ha podido enviar el correo electrónico. \n Intente nuevamente.";
            //EditorUtility.DisplayDialog("Error", "No se ha podido enviar el correo electrónico. \n Intente nuevamente.", "Ok");
            loginInProgress.SetActive(false);
            Debug.Log(error.GenerateErrorReport());
        }

        #endregion

        #region Email Register

        public void Register()
        {
            if (!ValidateRegisterData()) return;

            var request = new RegisterPlayFabUserRequest
            {
                TitleId = PlayFabConstants.TitleID,
                Email = registerUi.email.text,
                Password = registerUi.password.text,
                Username = registerUi.username.text,
                DisplayName = registerUi.username.text,
                InfoRequestParameters = loginInfoParams,
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLoginFailure);

            loginInProgress.SetActive(true);
        }

        bool ValidateRegisterData()
        {
            //Validating data
            string errorMessage = "";

            if (!registerUi.email.text.Contains("@"))
            {
                errorMessage = "E-mail inválido";
            }
            else if (registerUi.email.text.Length < 5)
            {
                errorMessage = "El e-mail debe tener almenos 5 caracteres";
            }
            else if (registerUi.username.text.Length < 5)
            {
                errorMessage = "El nomber de usuario debe tener almenos 5 caracteres";
            }
            else if (registerUi.password.text.Length < 8)
            {
                errorMessage = "La contraseña debe tener almenos 8 caracteres";
            }
            else if (!registerUi.password.text.Equals(registerUi.verifyPassword.text))
            {
                errorMessage = "Las contraseñas no coinciden";
            }

            if (errorMessage.Length > 0)
            {
                Debug.Log(errorMessage);
                errorMessagePanel.SetActive(true);
                txtErrorTitle.text = "Credenciales Inválidas";
                txtError.text = errorMessage;
                //EditorUtility.DisplayDialog("Credenciales Inválidas", errorMessage, "Aceptar");
                return false;
            }

            return true;
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log("Register Success!");
            cfmMessagePanel.SetActive(true);
            txtCfmTitle.text = "Usuario Registrado";
            txtCfm.text = "Usuario registrado con éxito";
            //EditorUtility.DisplayDialog("Usuario Registrado", "Usuario registrado con éxito", "Aceptar");

            PlayerPrefs.SetString("USERNAME", registerUi.username.text);
            //PlayerPrefs.SetString("DISPLAYNAME", registerUi.username.text);
            PlayerPrefs.SetString("PW", registerUi.password.text);
            //PlayerPrefs.SetString("displayName", registerUi.username.text);
            Debug.Log(result.PlayFabId);
            Debug.Log(result.Username);
            //PlayerPrefs.SetString("displayName", result.Username);
            
            loginInProgress.SetActive(false);
            loginPanel.SetActive(true);
            registerPanel.SetActive(false);
        }

        #endregion

        #region GuestLogin

        public void GuestLogin()
        {
            Login(new GuestLogin(), new GuestLogin.GuestLoginParameters(PlayerPrefs.GetString("GUEST_ID")));
        }

        #endregion

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Login Success!");
            Debug.Log("identificador playfab: " + result.PlayFabId);
            
            Debug.Log("nombre usuario playfab: --displayname--" + result.InfoResultPayload.PlayerProfile.DisplayName);
            Debug.Log("nombre usuario playfab: --username--" + result.InfoResultPayload.AccountInfo.Username);
            PlayFabConstants.displayName = result.InfoResultPayload.PlayerProfile.DisplayName;

            if (result.InfoResultPayload.AccountInfo.Username == null)
            {
                PlayerPrefs.SetString("displayName2", "Invitado");
            }
            else
            {
                PlayerPrefs.SetString("displayName2", result.InfoResultPayload.PlayerProfile.DisplayName);
            }

            PlayerPrefs.SetString(PlayFabConstants.SavedUsername, loginUi.username.text);
            //PlayerPrefs.SetString("displayName3", loginUi.username.text);
            
            loginInProgress.SetActive(false);

            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        }

        private readonly GetPlayerCombinedInfoRequestParams loginInfoParams =
            new GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true,
                GetUserData = true,
                GetUserInventory = true,
                GetUserVirtualCurrency = true,
                GetUserReadOnlyData = true,
                GetPlayerProfile = true,
                GetPlayerStatistics = true,
            };

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.Log("Login failure: " + error.Error + "  " + error.ErrorDetails + error + "  " +
                      error.ApiEndpoint + "  " + error.ErrorMessage);
            errorMessagePanel.SetActive(true);
            txtErrorTitle.text = "Error";
            txtError.text = "Usuario o contraseña invalidos";
            //EditorUtility.DisplayDialog("Error", "Usuario o contraseña invalidos", "Ok");
            loginInProgress.SetActive(false);
            Debug.Log(error.GenerateErrorReport());
        }

    }
}