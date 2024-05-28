using System;
using TMPro;
using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    public event Action<LoginRequestInfo> OnLoginRequested = delegate { };

    public void RequestLogin()
    {
        LoginRequestInfo loginRequestInfo = new(emailInput.text, passwordInput.text);
        OnLoginRequested(loginRequestInfo);
    }
}