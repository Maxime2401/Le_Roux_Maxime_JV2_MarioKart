using UnityEngine;

public class PlayerCustomizer : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("Référence à l'objet du joueur")]
    public GameObject playerObject; // Assigner dans l'éditeur

    [Header("Player Index (0 = Joueur 1, 1 = Joueur 2)")]
    public int playerIndex = 0;

    [Header("Customization Options")]
    public Mesh[] availableMeshes;
    public Material[] availableMaterials;

    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player reference not set in PlayerCustomizer!");
            return;
        }

        ApplyCustomization();
    }

    void ApplyCustomization()
    {
        int selectedIndex = BoutonsPersos.selectedCharacterIndices[playerIndex];
        
        // Vérification des index
        if (selectedIndex < 0 || 
            selectedIndex >= availableMeshes.Length || 
            selectedIndex >= availableMaterials.Length)
        {
            Debug.LogWarning("Invalid selection index, using default (0)");
            selectedIndex = 0;
        }

        // Récupération des composants
        MeshFilter meshFilter = playerObject.GetComponentInChildren<MeshFilter>();
        Renderer renderer = playerObject.GetComponentInChildren<Renderer>();

        // Application des changements
        if (meshFilter != null && availableMeshes.Length > 0)
        {
            meshFilter.mesh = availableMeshes[selectedIndex];
        }

        if (renderer != null && availableMaterials.Length > 0)
        {
            renderer.material = availableMaterials[selectedIndex];
        }
    }
}