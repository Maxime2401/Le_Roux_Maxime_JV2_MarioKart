// BoostObject.cs
using UnityEngine;

public class BoostObject : MonoBehaviour
{
    public enum BoostType
    {
        Normal,      // Boost standard
        Super,       // Boost plus puissant
        Infinite     // Boost longue durée
    }

    public BoostType boostType;
    public float boostDuration = 2f;
}