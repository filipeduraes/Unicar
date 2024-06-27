using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LoginScreen : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Callbacks")] 
    [SerializeField] private UnityEvent onLoginSuccess;
    [SerializeField] private UnityEvent onLoginFailed;

    public event Action<LoginRequestInfo> OnLoginRequested = delegate { };

    public void RequestLogin()
    {
        LoginRequestInfo loginRequestInfo = new(emailInput.text, passwordInput.text);
        OnLoginRequested(loginRequestInfo);
    }
    
    public void ReceiveLoginSuccess(bool success)
    {
        if (success)
            onLoginSuccess.Invoke();
        else
            onLoginFailed.Invoke();
    }
}