using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceEndButon : MonoBehaviour
{
    public void RestartRace()
    {
        Time.timeScale = 1f; // Réactive le temps si vous l'aviez gelé
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recharge la scène actuelle
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Remplacez par le nom de votre scène de menu
    }
}