using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KartController kart = other.GetComponent<KartController>();
            if (kart != null)
            {
                kart.OnCheckpointReached(checkpointNumber);
            }
        }
    }
}