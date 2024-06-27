using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RegisterScreen : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField repeatedPasswordInput;

    [Header("Callbacks")]
    [SerializeField] private UnityEvent onRegisterSuccess;
    [SerializeField] private UnityEvent onRegisterFailed;

    public event Action<RegisterRequestInfo> OnRegisterRequest = delegate { };

    public void SendRegisterRequest()
    {
        RegisterRequestInfo requestInfo = new(nameInput.text, emailInput.text, passwordInput.text, repeatedPasswordInput.text);
        OnRegisterRequest(requestInfo);
    }

    public void ReceiveRegisterSuccess(bool success)
    {
        if (success)
            onRegisterSuccess.Invoke();
        else
            onRegisterFailed.Invoke();
    }
}