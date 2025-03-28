using UnityEngine;
using System.Collections;

public class RespawnObject : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform spawnPoint;
    public float respawnDelay = 10f;

    private GameObject currentInstance;

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        if (objectPrefab != null && spawnPoint != null)
        {
            currentInstance = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
            var destroyDetector = currentInstance.AddComponent<DestroyDetector>();
            destroyDetector.OnDestroyed += () => StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            Debug.LogError("ObjectPrefab or SpawnPoint not assigned!");
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnObject(); // Réapparition après le délai
    }
}

public class DestroyDetector : MonoBehaviour
{
    public delegate void DestroyedEvent();
    public event DestroyedEvent OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}