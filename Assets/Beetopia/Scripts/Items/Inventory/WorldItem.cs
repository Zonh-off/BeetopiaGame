using UnityEngine;

public class WorldItem : MonoBehaviour {
    private ItemSO itemSO;

    public static WorldItem Create(ItemSO itemSO, Vector2 position) {
        Transform worldItemTransform = Instantiate(G.GameAssets.pfWorldItem);
        worldItemTransform.position = position;
        worldItemTransform.Find("Visual").GetComponent<SpriteRenderer>().sprite = itemSO.icon;
        
        WorldItem worldItem = worldItemTransform.GetComponent<WorldItem>();
        worldItem.itemSO = itemSO;
        
        G.TaskManager.AddTask(new CollectWorldItemTask(worldItem));
        
        return worldItem;
    }

    public ItemSO GetItemSO() {
        return itemSO;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}