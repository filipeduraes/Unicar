using System;
using TMPro;
using UnityEngine;

public class RegisterScreen : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField repeatedPasswordInput;

    public event Action<RegisterRequestInfo> OnRegisterRequest = delegate { };

    public void SendRegisterRequest()
    {
        RegisterRequestInfo requestInfo = new(nameInput.text, emailInput.text, passwordInput.text, repeatedPasswordInput.text);
        OnRegisterRequest(requestInfo);
    }
}