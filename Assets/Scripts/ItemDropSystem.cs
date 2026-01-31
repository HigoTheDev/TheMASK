using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// System to handle dropping items from NPCs
/// Can spawn items with physics and randomization
/// </summary>
public class ItemDropSystem : MonoBehaviour
{
    public static ItemDropSystem Instance { get; private set; }

    [Header("Default Item Settings")]
    [Tooltip("Layer cho dropped items (để tránh collision với NPCs)")]
    [SerializeField] private string droppedItemLayer = "Default";
    
    [Tooltip("Prefab mặc định cho item nếu ItemDropData không có prefab")]
    [SerializeField] private GameObject defaultItemPrefab;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Drop items from NPC position
    /// </summary>
    public void DropItems(Vector3 npcPosition, List<ItemDropData> itemDropList)
    {
        if (itemDropList == null || itemDropList.Count == 0)
        {
            Debug.Log("ItemDropSystem: No items to drop");
            return;
        }

        foreach (ItemDropData itemData in itemDropList)
        {
            if (itemData == null || !itemData.IsValid())
                continue;

            // Check drop chance
            if (!itemData.ShouldDrop())
            {
                Debug.Log($"ItemDropSystem: {itemData.itemName} failed drop chance roll");
                continue;
            }

            // Get drop quantity
            int quantity = itemData.GetDropQuantity();
            
            for (int i = 0; i < quantity; i++)
            {
                SpawnItem(npcPosition, itemData);
            }
        }
    }

    /// <summary>
    /// Drop a single item type from NPC
    /// </summary>
    public void DropItem(Vector3 npcPosition, ItemDropData itemData)
    {
        if (itemData == null || !itemData.IsValid())
        {
            Debug.LogWarning("ItemDropSystem: Invalid item data!");
            return;
        }

        List<ItemDropData> singleItemList = new List<ItemDropData> { itemData };
        DropItems(npcPosition, singleItemList);
    }

    /// <summary>
    /// Spawn a single item instance
    /// </summary>
    private void SpawnItem(Vector3 npcPosition, ItemDropData itemData)
    {
        // Determine which prefab to use
        GameObject prefabToSpawn = itemData.itemPrefab != null ? itemData.itemPrefab : defaultItemPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"ItemDropSystem: No prefab for {itemData.itemName}. Creating simple sprite object.");
            CreateSimpleItemSprite(npcPosition, itemData);
            return;
        }

        // Calculate spawn position
        Vector2 spawnOffset = itemData.GetRandomSpawnOffset();
        Vector3 spawnPosition = npcPosition + new Vector3(spawnOffset.x, spawnOffset.y, 0);

        // Instantiate item
        GameObject item = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        item.name = $"{itemData.itemName}_Drop";

        // Set layer
        if (!string.IsNullOrEmpty(droppedItemLayer))
        {
            item.layer = LayerMask.NameToLayer(droppedItemLayer);
        }

        // Apply launch force if has Rigidbody2D
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null && itemData.launchForce != Vector2.zero)
        {
            rb.AddForce(itemData.launchForce, ForceMode2D.Impulse);
        }

        // Store item data reference (for future use)
        DroppedItem droppedItemComponent = item.GetComponent<DroppedItem>();
        if (droppedItemComponent == null)
        {
            droppedItemComponent = item.AddComponent<DroppedItem>();
        }
        droppedItemComponent.Initialize(itemData);

        Debug.Log($"ItemDropSystem: Spawned {itemData.itemName} at {spawnPosition}");
    }

    /// <summary>
    /// Create a simple sprite-based item (fallback when no prefab exists)
    /// </summary>
    private void CreateSimpleItemSprite(Vector3 position, ItemDropData itemData)
    {
        GameObject item = new GameObject($"{itemData.itemName}_Drop");
        item.transform.position = position + (Vector3)itemData.GetRandomSpawnOffset();

        // Add sprite renderer
        SpriteRenderer sr = item.AddComponent<SpriteRenderer>();
        if (itemData.itemIcon != null)
        {
            sr.sprite = itemData.itemIcon;
        }
        sr.sortingOrder = 5;

        // Add collider
        CircleCollider2D collider = item.AddComponent<CircleCollider2D>();
        collider.radius = 0.25f;

        // Add rigidbody
        Rigidbody2D rb = item.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        if (itemData.launchForce != Vector2.zero)
        {
            rb.AddForce(itemData.launchForce, ForceMode2D.Impulse);
        }

        // Add DroppedItem component
        DroppedItem droppedItem = item.AddComponent<DroppedItem>();
        droppedItem.Initialize(itemData);

        Debug.Log($"ItemDropSystem: Created simple sprite for {itemData.itemName}");
    }
}

/// <summary>
/// Component attached to dropped items to store their data
/// Can be extended for pickup functionality
/// </summary>
public class DroppedItem : MonoBehaviour
{
    private ItemDropData itemData;

    public void Initialize(ItemDropData data)
    {
        itemData = data;
    }

    public ItemDropData GetItemData()
    {
        return itemData;
    }

    // Future: Add pickup functionality
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Player"))
    //     {
    //         // Handle pickup
    //         Destroy(gameObject);
    //     }
    // }
}
