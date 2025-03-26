using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    [Header("Checkpoints")]
    public List<Transform> checkpoints = new List<Transform>();
    private int currentIndex = 0;
    private int lapCount = 0;

    [Header("Race Settings")]
    public int totalLaps = 2;
    public string playerTag = "Player";

    [Header("UI")]
    public Text lapsText; // Assignez un objet UI Text dans l'inspecteur

    private void Start()
    {
        SetupCheckpoints();
        UpdateLapsUI();
    }

    private void SetupCheckpoints()
    {
        foreach (Transform cp in checkpoints)
        {
            // Ajoute un collider trigger si absent
            if (!cp.TryGetComponent<Collider>(out var collider))
            {
                collider = cp.gameObject.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;

            // Ajoute le composant trigger
            if (!cp.TryGetComponent<CheckpointTrigger>(out _))
            {
                var trigger = cp.gameObject.AddComponent<CheckpointTrigger>();
                trigger.manager = this;
            }
        }
    }

    public void PlayerPassedCheckpoint(Transform checkpoint)
    {
        if (checkpoints[currentIndex] == checkpoint)
        {
            currentIndex++;
            
            if (currentIndex >= checkpoints.Count)
            {
                currentIndex = 0;
                lapCount++;
                UpdateLapsUI();
                
                if (lapCount > totalLaps)
                {
                    Debug.Log("Course terminée !");
                    // Ajoutez ici la logique de fin de course
                }
            }
        }
    }

    private void UpdateLapsUI()
    {
        if (lapsText != null)
        {
            lapsText.text = $"Tour: {lapCount}/{totalLaps}";
        }
    }
}

[RequireComponent(typeof(Collider))]
public class CheckpointTrigger : MonoBehaviour
{
    [HideInInspector] public CheckpointManager manager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(manager.playerTag))
        {
            manager.PlayerPassedCheckpoint(transform);
        }
    }
}