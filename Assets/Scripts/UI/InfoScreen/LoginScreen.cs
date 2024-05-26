uusing UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Adicione a lógica de login aqui, como verificar credenciais no banco de dados
        Debug.Log("Tentando login com Email: " + email + " e Senha: " + password);

        // Se a verificação de login for bem-sucedida, navegue para a tela CaronaScreen
        SceneManager.LoadScene("CaronaScreen");
    }

    public void OnGoToRegisterButtonClicked()
    {
        SceneManager.LoadScene("RegisterScreen");
    }
}
