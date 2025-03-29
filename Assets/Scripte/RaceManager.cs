using UnityEngine;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [SerializeField] private int totalCheckpoints = 5;
    public int TotalCheckpoints => totalCheckpoints;

    private Dictionary<int, PlayerProgress> playersProgress = new Dictionary<int, PlayerProgress>();

    private class PlayerProgress
    {
        public int checkpoints;
        public int laps;
        public float lastCheckpointTime;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerProgress(int playerNumber, int checkpoint, int lap)
    {
        if (!playersProgress.ContainsKey(playerNumber))
        {
            playersProgress[playerNumber] = new PlayerProgress();
        }

        playersProgress[playerNumber].checkpoints = checkpoint;
        playersProgress[playerNumber].laps = lap;
        playersProgress[playerNumber].lastCheckpointTime = Time.time;
    }

    public int GetPlayerPosition(int playerNumber)
    {
        if (!playersProgress.ContainsKey(playerNumber)) return 1;

        int position = 1;
        var currentPlayer = playersProgress[playerNumber];
        int currentScore = currentPlayer.laps * 1000 + currentPlayer.checkpoints;

        foreach (var player in playersProgress)
        {
            if (player.Key != playerNumber)
            {
                int otherScore = player.Value.laps * 1000 + player.Value.checkpoints;
                if (otherScore > currentScore ||
                    (otherScore == currentScore && player.Value.lastCheckpointTime < currentPlayer.lastCheckpointTime))
                {
                    position++;
                }
            }
        }

        return position;
    }
}