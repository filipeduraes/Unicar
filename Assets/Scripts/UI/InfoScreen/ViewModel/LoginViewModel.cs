using System;
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
            
        }
    }
}