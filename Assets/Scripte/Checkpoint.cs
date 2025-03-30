using UnityEngine; // Cette ligne doit absolument être en haut du fichier

public class Checkpoint : MonoBehaviour // MonoBehaviour est maintenant reconnu
{
    public int checkpointNumber;
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KartController kart = other.GetComponent<KartController>();
            if (kart != null && respawnPoint != null)
            {
                kart.OnCheckpointReached(checkpointNumber, respawnPoint.position, respawnPoint.rotation);
            }
        }
    }
}