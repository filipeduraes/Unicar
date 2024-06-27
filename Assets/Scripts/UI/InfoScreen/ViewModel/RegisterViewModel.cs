using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Unicar.Persistence;
using UnityEngine;

namespace Unicar.UI.InfoScreen.ViewModel
{
    public class RegisterViewModel : MonoBehaviour
    {
        [SerializeField] private RegisterScreen registerScreen;
        [SerializeField] private string emailPattern = @"^[a-zA-Z0-9._%+-]+@ufvjm\.edu\.br$";
        
        private Regex _emailRegex;

        private void Awake()
        {
            _emailRegex = new Regex(emailPattern);
        }

        private void OnEnable()
        {
            registerScreen.OnRegisterRequest += Register;
        }

        private void OnDisable()
        {
            registerScreen.OnRegisterRequest -= Register;
        }

        private void Register(RegisterRequestInfo registerRequestInfo)
        {
            Match match = _emailRegex.Match(registerRequestInfo.Email);
            
            bool canRegister = !string.IsNullOrEmpty(registerRequestInfo.Name) &&
                               !string.IsNullOrEmpty(registerRequestInfo.Password);

            canRegister &= registerRequestInfo.Password == registerRequestInfo.RepeatedPassword;
            canRegister &= match.Success;

            if (canRegister)
                _ = SaveProfileAsync(registerRequestInfo);
            else
                registerScreen.ReceiveRegisterSuccess(false);
        }

        private async UniTask SaveProfileAsync(RegisterRequestInfo registerRequestInfo)
        {
            ProfileCredentials profileCredentials = new(registerRequestInfo.Email, registerRequestInfo.Password);
            ProfileData profileData = new(registerRequestInfo.Name, profileCredentials);

            bool registerSuccess = await PersistenceManager.SaveProfileAsync(profileData);
            registerScreen.ReceiveRegisterSuccess(registerSuccess);
        }
    }
}