using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    public GameObject objectPrefab;  // L'objet à faire apparaître/réapparaître
    public Transform spawnPoint;     // Position de réapparition
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
            // Créer une nouvelle instance de l'objet
            currentInstance = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Ajouter un détecteur de destruction
            var destroyDetector = currentInstance.AddComponent<DestroyDetector>();
            destroyDetector.OnDestroyed += HandleObjectDestroyed;
        }
        else
        {
            Debug.LogError("ObjectPrefab or SpawnPoint not assigned!");
        }
    }

    void HandleObjectDestroyed()
    {
        // Planifier la réapparition après le délai
        Invoke("SpawnObject", respawnDelay);
    }
}

// Script pour détecter la destruction
public class DestroyDetector : MonoBehaviour
{
    public delegate void DestroyedEvent();
    public event DestroyedEvent OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}