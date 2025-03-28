using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoutonsPersos : MonoBehaviour
{
    // Configuration existante
    public Button[] boutonsPersos;
    public GameObject[] modelesPersos;
    public Transform spawnPoint;
    public Button boutonCommencer;
    public string sceneDeCourse = "Course";

    // Modifié pour 2 joueurs
    public static int[] selectedCharacterIndices = new int[2] { -1, -1 };
    private int currentPlayerSelecting = 0; // 0 = Joueur 1, 1 = Joueur 2
    public Text selectionText;

    void Start()
    {
        // Initialisation des boutons (identique)
        for (int i = 0; i < boutonsPersos.Length; i++)
        {
            int index = i;
            boutonsPersos[i].onClick.AddListener(() => SelectionnerPerso(index));
        }

        boutonCommencer.interactable = false;
        boutonCommencer.onClick.AddListener(ChangerScene);
        
        UpdateSelectionUI();
    }

    void SelectionnerPerso(int index)
    {
        // Même système de prévisualisation
        foreach (Transform child in spawnPoint) Destroy(child.gameObject);
        Instantiate(modelesPersos[index], spawnPoint.position, Quaternion.identity, spawnPoint);

        // Sauvegarde l'INDEX pour le joueur actuel
        selectedCharacterIndices[currentPlayerSelecting] = index;
        
        // Passe au joueur suivant ou active le bouton Commencer
        if (currentPlayerSelecting < 1)
        {
            currentPlayerSelecting++;
            UpdateSelectionUI();
        }
        else
        {
            boutonCommencer.interactable = true;
            selectionText.text = "Prêt à commencer!";
        }
    }

    void UpdateSelectionUI()
    {
        selectionText.text = $"Joueur {currentPlayerSelecting + 1}, sélectionnez votre personnage";
    }

    void Update()
    {
        // Rotation identique
        if (spawnPoint.childCount > 0)
            spawnPoint.GetChild(0).Rotate(0, 50 * Time.deltaTime, 0);
    }

    void ChangerScene()
    {
        if (selectedCharacterIndices[0] >= 0 && selectedCharacterIndices[1] >= 0)
            SceneManager.LoadScene(sceneDeCourse);
    }
}