using Cysharp.Threading.Tasks;
using Unicar.Persistence;
using UnityEngine;

namespace Unicar.UI.InfoScreen.ViewModel
{
    public class LoginViewModel : MonoBehaviour
    {
        [SerializeField] private LoginScreen loginScreen;

        private void OnEnable()
        {
            loginScreen.OnLoginRequested += ValidateLogin;
        }

        private void OnDisable()
        {
            loginScreen.OnLoginRequested -= ValidateLogin;
        }

        private void ValidateLogin(LoginRequestInfo loginRequestInfo)
        {
            _ = ValidateLoginAsync(loginRequestInfo);
        }

        private async UniTask ValidateLoginAsync(LoginRequestInfo loginRequestInfo)
        {
            ProfileCredentials profileCredentials = new(loginRequestInfo.Email, loginRequestInfo.Password);
            ProfileData profileData = await PersistenceManager.FindProfileWithDataAsync(profileCredentials);

            loginScreen.ReceiveLoginSuccess(profileData != null);
        }
    }
}