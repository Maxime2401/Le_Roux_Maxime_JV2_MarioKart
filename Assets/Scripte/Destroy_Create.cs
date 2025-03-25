using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ObjectData[] _possibleItems; // Liste des objets possibles

    [Header("Destruction Settings")]
    [SerializeField] private GameObject _destroyEffect; // Effet visuel lors de la destruction
    [SerializeField] private float _destroyDelay = 0.5f; // Délai avant la destruction de l'objet
    [SerializeField] private string _playerTag = "Player"; // Tag pour détecter le joueur

    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet entrant en collision est le joueur
        if (other.CompareTag(_playerTag))
        {
            DestroyCrate();
        }
    }

    public void DestroyCrate()
    {
        // Applique l'effet visuel de destruction
        if (_destroyEffect != null)
        {
            Instantiate(_destroyEffect, transform.position, transform.rotation);
        }

        // Donne un objet aléatoire au joueur
        if (_possibleItems.Length > 0)
        {
            ObjectData randomItem = _possibleItems[Random.Range(0, _possibleItems.Length)];
            GiveItemToPlayer(randomItem);
        }

        // Détruit la caisse après un délai
        Destroy(gameObject, _destroyDelay);
    }

    private void GiveItemToPlayer(ObjectData itemData)
    {
        PlayerItemHandler playerItemHandler = FindObjectOfType<PlayerItemHandler>();
        if (playerItemHandler != null)
        {
            playerItemHandler.SetCurrentItem(itemData);
        }
    }
}