using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ObjectData[] possibleItems;
    [SerializeField] private float itemGiveRadius = 3f;

    [Header("Destruction Settings")]
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private float destroyDelay = 0.5f;
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            DestroyCrate(other.transform);
        }
    }

    public void DestroyCrate(Transform hittingPlayer)
    {
        // Effet visuel
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Donne un objet aléatoire
        if (possibleItems.Length > 0)
        {
            GiveItemToPlayer(hittingPlayer, possibleItems[Random.Range(0, possibleItems.Length)]);
        }

        Destroy(gameObject, destroyDelay);
    }

    private void GiveItemToPlayer(Transform playerTransform, ObjectData itemData)
    {
        // Option 1: Le joueur qui a cassé la caisse
        if (playerTransform.TryGetComponent<PlayerItemHandler>(out var handler) || 
           (handler = playerTransform.GetComponentInParent<PlayerItemHandler>()) != null)
        {
            handler.SetCurrentItem(itemData);
            return;
        }

        // Option 2: Trouve le joueur le plus proche
        PlayerItemHandler nearestHandler = null;
        float minDistance = Mathf.Infinity;

        foreach (var player in FindObjectsOfType<PlayerItemHandler>())
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance && distance <= itemGiveRadius)
            {
                minDistance = distance;
                nearestHandler = player;
            }
        }

        nearestHandler?.SetCurrentItem(itemData);
    }
}