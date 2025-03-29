using UnityEngine;
using System.Collections;

public class RespawnObject : MonoBehaviour
{
    public GameObject objectPrefab;  // Le préfabriqué à faire réapparaître
    public Transform spawnPoint;     // Point de réapparition
    public float respawnDelay = 10f; // Délai avant réapparition

    private GameObject currentInstance; // Instance actuelle de l'objet

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        if (objectPrefab != null && spawnPoint != null)
        {
            // Crée une nouvelle instance de l'objet
            currentInstance = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);

            // Ajoute le détecteur de destruction et abonne l'événement
            var destroyDetector = currentInstance.AddComponent<DestroyDetector>();
            destroyDetector.OnDestroyed += HandleObjectDestroyed;
        }
        else
        {
            Debug.LogError("ObjectPrefab ou SpawnPoint non assigné !");
        }
    }

    private void HandleObjectDestroyed()
    {
        // Vérifie que ce script existe toujours avant de relancer la coroutine
        if (this != null && gameObject != null)
        {
            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnObject(); // Réapparition après le délai
    }

    void OnDestroy()
    {
        // Nettoyage : se désabonne de l'événement quand ce script est détruit
        if (currentInstance != null)
        {
            var destroyDetector = currentInstance.GetComponent<DestroyDetector>();
            if (destroyDetector != null)
            {
                destroyDetector.OnDestroyed -= HandleObjectDestroyed;
            }
        }
    }
}

public class DestroyDetector : MonoBehaviour
{
    public delegate void DestroyedEvent();
    public event DestroyedEvent OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed?.Invoke(); // Déclenche l'événement quand l'objet est détruit
    }
}