using UnityEngine;

[System.Serializable]
public class DamageObject : MonoBehaviour
{
    public enum DamageType
    {
        Banana,
        // Causes a short slowdown and spin
        Oil,           // Causes a medium slowdown and faster spin
        Spike          // Causes a complete stop and very fast spin
    }

    public DamageType damageType;
    public float slowdownDuration = 1f; // Duration of the effect in seconds

    // You can add other properties here if needed, like visual effects
}