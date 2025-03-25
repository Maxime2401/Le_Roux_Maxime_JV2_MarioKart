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

    // Nouveau: Sauvegarde l'INDEX du perso sélectionné
    public static int selectedCharacterIndex = -1;

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
    }

    void SelectionnerPerso(int index)
    {
        // Même système de prévisualisation
        foreach (Transform child in spawnPoint) Destroy(child.gameObject);
        Instantiate(modelesPersos[index], spawnPoint.position, Quaternion.identity, spawnPoint);

        // Sauvegarde l'INDEX au lieu du nom
        selectedCharacterIndex = index;
        boutonCommencer.interactable = true;
    }

    void Update()
    {
        // Rotation identique
        if (spawnPoint.childCount > 0)
            spawnPoint.GetChild(0).Rotate(0, 50 * Time.deltaTime, 0);
    }

    void ChangerScene()
    {
        if (selectedCharacterIndex >= 0)
            SceneManager.LoadScene(sceneDeCourse);
    }
}