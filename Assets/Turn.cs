using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("Race Settings")]
    [SerializeField] private int totalCheckpoints = 5;
    public int TotalCheckpoints => totalCheckpoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionnel - si vous changez de scène
        }
        else
        {
            Destroy(gameObject);
        }
    }
}