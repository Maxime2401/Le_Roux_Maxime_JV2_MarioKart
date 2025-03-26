using UnityEngine;
using UnityEngine.UI;

public class PlayerItemHandler : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Image _itemIconUI;
    
    [Header("Item Settings")]
    [SerializeField] private GameObject _bananaPrefab;
    [SerializeField] private GameObject _greenShellPrefab;
    [SerializeField] private Transform _rearSpawnPoint; // Pour les bananes
    [SerializeField] private Transform _forwardSpawnPoint; // Pour les carapaces
    [SerializeField] private float _shellForce = 50f;

    private ObjectData _currentItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _currentItem != null)
        {
            UseCurrentItem();
        }
    }

    public void SetCurrentItem(ObjectData itemData)
    {
        _currentItem = itemData;

        if (_itemIconUI != null)
        {
            _itemIconUI.sprite = itemData.itemIcon;
            _itemIconUI.enabled = true;
        }
    }

    private void UseCurrentItem()
    {
        if (_currentItem != null)
        {
            ApplyItemEffect(_currentItem);
            _currentItem = null;

            if (_itemIconUI != null)
            {
                _itemIconUI.enabled = false;
            }
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
                
            case "bombe":
                // Ajouter le système foudre ici
                break;
        }
    }

    private void SpawnBanana()
    {
        if (_bananaPrefab == null || _rearSpawnPoint == null) return;
        Instantiate(_bananaPrefab, _rearSpawnPoint.position, _rearSpawnPoint.rotation);
    }

    private void ThrowGreenShell()
    {
        if (_greenShellPrefab == null || _forwardSpawnPoint == null) return;
        
        GameObject shell = Instantiate(_greenShellPrefab, 
                                    _forwardSpawnPoint.position, 
                                    _forwardSpawnPoint.rotation);
        
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(_forwardSpawnPoint.forward * _shellForce, ForceMode.Impulse);
        }
    }
}