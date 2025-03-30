using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceEndButon : MonoBehaviour
{
    public void RestartRace()
    {
        Time.timeScale = 1f; // R�active le temps si vous l'aviez gel�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recharge la sc�ne actuelle
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Remplacez par le nom de votre sc�ne de menu
    }
}