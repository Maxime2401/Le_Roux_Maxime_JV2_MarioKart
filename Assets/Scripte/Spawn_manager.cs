using UnityEngine;
using Cinemachine; // N'oubliez pas d'ajouter cette directive

public class RaceManager : MonoBehaviour
{
    [Header("Spawn")]
    public Transform playerSpawnPoint;
    public GameObject[] availableCharacters;
    
    [Header("Camera")]
    public CinemachineVirtualCamera playerFollowCamera; // Assigner dans l'éditeur

    void Start()
    {
        GameObject player = SpawnPlayer();
        SetupCamera(player);
    }

    GameObject SpawnPlayer()
    {
        int index = BoutonsPersos.selectedCharacterIndex;
        
        if (index >= 0 && index < availableCharacters.Length)
        {
            GameObject player = Instantiate(
                availableCharacters[index], 
                playerSpawnPoint.position, 
                playerSpawnPoint.rotation
            );
            return player;
        }
        
        Debug.LogError("Index invalide - chargement par défaut");
        return Instantiate(availableCharacters[0], playerSpawnPoint.position, playerSpawnPoint.rotation);
    }

    void SetupCamera(GameObject player)
    {
        if (playerFollowCamera != null && player != null)
        {
            playerFollowCamera.Follow = player.transform;
            playerFollowCamera.LookAt = player.transform;
            
            // Option: Configurer dynamiquement pour un 3rd person shooter
            var transposer = playerFollowCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = new Vector3(0, 2, -4); // Vue en 3ème personne
            }
        }
        else
        {
            Debug.LogError("Camera ou joueur non assigné!");
        }
    }
}