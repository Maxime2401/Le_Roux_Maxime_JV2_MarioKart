using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PlayerItemHandler : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerNumber = 1; // 1 ou 2
    [SerializeField] private KeyCode useItemKey = KeyCode.E; // Configurable dans l'inspecteur

    [Header("UI Settings")]
    [SerializeField] private Image itemIconUI;
    
    [Header("Item Settings")]
    [SerializeField] private GameObject MushPrefab;
    [SerializeField] private GameObject bananaPrefab;
    [SerializeField] private GameObject greenShellPrefab;
    [SerializeField] private Transform rearSpawnPoint;
    [SerializeField] private Transform forwardSpawnPoint;
    [SerializeField] private float shellForce = 50f;
    [SerializeField] private GameObject bloupEffectPlayer1; // Effet pour le joueur 1
    [SerializeField] private GameObject bloupEffectPlayer2; // Effet pour le joueur 2
    [SerializeField] private float bloupDuration = 5f;
    private ObjectData currentItem;

    private void Awake()
    {
        // Configuration automatique si non défini
        if (playerNumber == 2 && useItemKey == KeyCode.E)
        {
            useItemKey = KeyCode.RightShift;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(useItemKey) && currentItem != null)
        {
            UseCurrentItem();
        }
    }

    public void SetCurrentItem(ObjectData itemData)
    {
        currentItem = itemData;

        if (itemIconUI != null)
        {
            itemIconUI.sprite = itemData.itemIcon;
            itemIconUI.enabled = true;
        }
    }

    private void UseCurrentItem()
    {
        if (currentItem != null)
        {
            ApplyItemEffect(currentItem);
            currentItem = null;
            if (itemIconUI != null) itemIconUI.enabled = false;
        }
    }

    private void ApplyItemEffect(ObjectData itemData)
    {
        switch (itemData.itemName)
        {
            case "Banane":
                SpawnBanana();
                break;
            case "Vert":
                ThrowGreenShell();
                break;
            case "Bloup":
                SpawnBlop();
                break;
            case "Mushrum":
                ApplyMush();
                break;

        }
    }
    private void SpawnBlop()
    {
        GameObject bloupEffect = playerNumber == 1 ? bloupEffectPlayer1 : bloupEffectPlayer2;

        if (bloupEffect != null)
        {
            StartCoroutine(ActivateBloupEffect(bloupEffect));
        }
    }
    private void ApplyMush()
    {
        if (MushPrefab == null || forwardSpawnPoint == null) return;

        GameObject mush = Instantiate(MushPrefab, forwardSpawnPoint.position, forwardSpawnPoint.rotation);
        
    }
    private IEnumerator ActivateBloupEffect(GameObject effect)
    {
        effect.SetActive(true);
        yield return new WaitForSeconds(bloupDuration);
        effect.SetActive(false);
    }
    private void SpawnBanana()
    {
        if (bananaPrefab != null && rearSpawnPoint != null)
            Instantiate(bananaPrefab, rearSpawnPoint.position, rearSpawnPoint.rotation);
    }

    private void ThrowGreenShell()
    {
        if (greenShellPrefab == null || forwardSpawnPoint == null) return;
        
        GameObject shell = Instantiate(greenShellPrefab, forwardSpawnPoint.position, forwardSpawnPoint.rotation);
        if (shell.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(forwardSpawnPoint.forward * shellForce, ForceMode.Impulse);
        }
    }
}