using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemsSetProvider", fileName = "ItemsSetProvider", order = 51)]
public class ItemsSetProvider : ScriptableObject
{
    [SerializeField] 
    private Item[] _itemPrefabs;

    public Item[] GetItemsSet()
    {
        return _itemPrefabs;
    }
}