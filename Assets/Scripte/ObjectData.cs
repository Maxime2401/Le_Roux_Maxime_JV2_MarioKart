using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ObjectData : ScriptableObject
{
    public string itemName; // Nom de l'item
    public Sprite itemIcon; // Icône de l'item
    public GameObject itemPrefab; // Préfabriqué de l'item (optionnel)
    public string itemDescription; // Description de l'item
}