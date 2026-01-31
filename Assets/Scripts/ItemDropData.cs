using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject to define items that can be dropped by NPCs
/// Create via: Right-click in Project > Create > TheMASK > Item Drop Data
/// </summary>
[CreateAssetMenu(fileName = "NewItemDrop", menuName = "TheMASK/Item Drop Data", order = 2)]
public class ItemDropData : ScriptableObject
{
    [Header("Item Information")]
    [Tooltip("Tên của item")]
    public string itemName = "Item";
    
    [Tooltip("Mô tả về item")]
    [TextArea(2, 5)]
    public string itemDescription = "An item";
    
    [Tooltip("Sprite/Icon của item")]
    public Sprite itemIcon;

    [Header("Story Information")]
    [Tooltip("Tiêu đề câu chuyện/lore của item")]
    public string storyTitle = "";
    
    [Tooltip("Mô tả ngắn về câu chuyện hoặc nguồn gốc item")]
    [TextArea(2, 5)]
    public string storyDescription = "";
    
    [Tooltip("Full lore text - câu chuyện đầy đủ về item (optional)")]
    [TextArea(5, 10)]
    public string fullLore = "";
    
    [Tooltip("Icon chi tiết hơn để hiển thị trong Item Info UI (optional, nếu null dùng itemIcon)")]
    public Sprite detailedIcon;

    [Header("Pickup Settings")]
    [Tooltip("Item này có thể được nhặt lên không?")]
    public bool canBePickedUp = true;
    
    [Tooltip("Vị trí offset khi player cầm item (Y thường > 0 để item ở trên đầu)")]
    public Vector3 holdOffset = new Vector3(0, 1.5f, 0);
    
    [Tooltip("Tốc độ follow player khi đang cầm (1-20, cao = nhanh hơn)")]
    [Range(1f, 20f)]
    public float followSpeed = 10f;

    [Header("Drop Settings")]
    [Tooltip("Prefab của item sẽ spawn vào scene (có thể để null nếu chỉ cần spawn sprite)")]
    public GameObject itemPrefab;
    
    [Tooltip("Xác suất drop item (0-100). 100 = luôn drop, 50 = 50% drop, 0 = không drop")]
    [Range(0, 100)]
    public float dropChance = 100f;
    
    [Tooltip("Số lượng item drop (min-max). Random giữa 2 giá trị này.")]
    public Vector2Int dropQuantityRange = new Vector2Int(1, 1);

    [Header("Spawn Settings")]
    [Tooltip("Offset khoảng cách spawn từ NPC (tính bằng units)")]
    public Vector2 spawnOffset = new Vector2(0, 0.5f);
    
    [Tooltip("Random range cho spawn position (tạo variation)")]
    public float spawnRandomRadius = 0.3f;
    
    [Tooltip("Lực bật khi spawn (để item bay ra)")]
    public Vector2 launchForce = new Vector2(0, 2f);

    /// <summary>
    /// Check if this item should drop based on drop chance
    /// </summary>
    public bool ShouldDrop()
    {
        float roll = Random.Range(0f, 100f);
        return roll <= dropChance;
    }

    /// <summary>
    /// Get random drop quantity
    /// </summary>
    public int GetDropQuantity()
    {
        return Random.Range(dropQuantityRange.x, dropQuantityRange.y + 1);
    }

    /// <summary>
    /// Get randomized spawn position offset
    /// </summary>
    public Vector2 GetRandomSpawnOffset()
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRandomRadius;
        return spawnOffset + randomOffset;
    }

    /// <summary>
    /// Validate item data
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(itemName);
    }
}
