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

        // Confirme o cadastro (lógica de cadastro aqui)
        // Depois de confirmar, você pode redirecionar para a tela de login ou carona
        SceneManager.LoadScene("CaronaScreen");
    }

    public void OnGoToLoginButtonClicked()
    {
        SceneManager.LoadScene("LoginScreen");
    }
}
