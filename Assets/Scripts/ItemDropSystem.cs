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
/// Supports pickup detection and visual feedback
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DroppedItem : MonoBehaviour
{
    private ItemDropData itemData;
    private bool playerNearby = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    [Header("Pickup Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool enableHighlight = true;
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.5f, 1f); // Slight yellow tint
    [SerializeField] private float highlightIntensity = 1.2f;

    private void Awake()
    {
        // Get sprite renderer for highlight
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Ensure collider is trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    public void Initialize(ItemDropData data)
    {
        itemData = data;
        
        // Store original color after initialization in case sprite was set
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public ItemDropData GetItemData()
    {
        return itemData;
    }

    public bool IsPlayerNearby()
    {
        return playerNearby;
    }

    public bool CanBePickedUp()
    {
        return itemData != null && itemData.canBePickedUp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            playerNearby = true;
            
            // Visual highlight
            if (enableHighlight && CanBePickedUp())
            {
                ApplyHighlight(true);
            }

            Debug.Log($"DroppedItem: Player near {itemData?.itemName}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            playerNearby = false;
            
            // Remove highlight
            if (enableHighlight)
            {
                ApplyHighlight(false);
            }

            Debug.Log($"DroppedItem: Player left {itemData?.itemName}");
        }
    }

    /// <summary>
    /// Check if collider is the player
    /// </summary>
    private bool IsPlayer(Collider2D collision)
    {
        // Check by layer
        if (playerLayer != 0 && ((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            return true;
        }

        // Fallback: check by tag
        if (!string.IsNullOrEmpty(playerTag) && collision.CompareTag(playerTag))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Apply visual highlight when player nearby
    /// </summary>
    private void ApplyHighlight(bool highlight)
    {
        if (spriteRenderer == null)
            return;

        if (highlight)
        {
            spriteRenderer.color = originalColor * highlightColor * highlightIntensity;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    /// <summary>
    /// Called when item is picked up - disable physics and visuals
    /// </summary>
    public void OnPickedUp()
    {
        // Disable physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }

        // Disable trigger to prevent re-pickup
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Reset color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        playerNearby = false;
    }

    /// <summary>
    /// Called when item is dropped - re-enable physics and collisions
    /// </summary>
    public void OnDropped()
    {
        // Re-enable physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }

        // Re-enable trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }
}

