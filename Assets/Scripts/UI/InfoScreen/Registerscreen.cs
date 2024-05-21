using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterScreen : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;

    public void OnRegisterButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Adicione a lógica de cadastro aqui, como salvar os dados no banco de dados
        Debug.Log("Registrando com Email: " + email + " e Senha: " + password);
    }

    public void OnGoToLoginButtonClicked()
    {
        SceneManager.LoadScene("LoginScene");
    }
}

