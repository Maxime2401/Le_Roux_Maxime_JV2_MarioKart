using UnityEngine;
using System.Collections.Generic;

public class PlayerCheckpointTracker : MonoBehaviour
{
    [Header("Paramètres")]
    public List<GameObject> checkpoints;  // Liste manuelle OU auto-remplie
    public int toursComplets = 0;
    //public Text tourText;  // Optionnel : affichage UI

    private int currentCheckpointIndex = 0;

    void Start()
    {
        // Option 1 : Remplir automatiquement si les checkpoints ont un tag commun
        if (checkpoints.Count == 0)
        {
            GameObject[] foundCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
            // Trie par nom (ex: "Checkpoint1", "Checkpoint2"...)
            System.Array.Sort(foundCheckpoints, (a, b) => a.name.CompareTo(b.name));
            checkpoints = new List<GameObject>(foundCheckpoints);
        }

        //UpdateUI();
        HighlightCurrentCheckpoint();
    }

    void OnTriggerEnter(Collider other)
    {
        if (checkpoints.Count == 0) return;

        // Si le joueur touche le bon checkpoint
        if (other.gameObject == checkpoints[currentCheckpointIndex])
        {
            // Désactive le checkpoint (évite les retriggers)
            other.enabled = false;

            currentCheckpointIndex++;

            // Tour complet
            if (currentCheckpointIndex >= checkpoints.Count)
            {
                currentCheckpointIndex = 0;
                toursComplets++;
                Debug.Log($"Tour {toursComplets} complet !");
                // Réactive tous les checkpoints pour le prochain tour
                foreach (var cp in checkpoints) cp.SetActive(true);
            }

            //UpdateUI();
            HighlightCurrentCheckpoint();
        }
    }

    //void UpdateUI()
    // {
    //    if (tourText != null) tourText.text = "Tours: " + toursComplets;
    //}

    void HighlightCurrentCheckpoint()
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            MeshRenderer renderer = checkpoints[i].GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.material.color = (i == currentCheckpointIndex) ? Color.green : Color.gray;
        }
    }
}