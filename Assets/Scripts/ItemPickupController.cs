using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component attached to Player to handle item pickup and drop
/// Press G to pickup nearby items or drop currently held item
/// </summary>
public class ItemPickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Phím để nhặt/thả item")]
    [SerializeField] private KeyCode pickupKey = KeyCode.G;
    
    [Tooltip("Layer của items có thể nhặt")]
    [SerializeField] private LayerMask itemLayer;
    
    [Tooltip("Tag của items có thể nhặt (fallback)")]
    [SerializeField] private string itemTag = "Item";
    
    [Tooltip("Bán kính phát hiện items gần player")]
    [SerializeField] private float pickupRadius = 2f;

    [Header("Hold Settings")]
    [Tooltip("Transform để gắn item vào (nếu null, sẽ dùng player transform)")]
    [SerializeField] private Transform holdPoint;
    
    [Tooltip("Có smooth follow không? (false = instant snap)")]
    [SerializeField] private bool smoothFollow = true;

    [Header("UI References")]
    [Tooltip("UI hiển thị thông tin item (sẽ tự tìm nếu null)")]
    [SerializeField] private ItemInfoUI itemInfoUI;
    
    [Tooltip("Pickup prompt UI (sẽ tự tìm nếu null)")]
    [SerializeField] private PickupPrompt pickupPrompt;

    // State
    private GameObject heldItem = null;
    private DroppedItem heldItemComponent = null;
    private ItemDropData heldItemData = null;
    private List<DroppedItem> nearbyItems = new List<DroppedItem>();

    private void Awake()
    {
        // Auto-find UI components if not assigned
        if (itemInfoUI == null)
        {
            itemInfoUI = FindFirstObjectByType<ItemInfoUI>();
        }

        if (pickupPrompt == null)
        {
            pickupPrompt = FindFirstObjectByType<PickupPrompt>();
        }

        // If no hold point specified, use player transform
        if (holdPoint == null)
        {
            holdPoint = transform;
        }
    }

    private void Update()
    {
        UpdateNearbyItems();
        HandlePickupInput();
        UpdateHeldItemPosition();
    }

    /// <summary>
    /// Find all nearby pickupable items
    /// </summary>
    private void UpdateNearbyItems()
    {
        nearbyItems.Clear();

        // Find all DroppedItem components in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        
        foreach (Collider2D col in colliders)
        {
            DroppedItem droppedItem = col.GetComponent<DroppedItem>();
            if (droppedItem != null && droppedItem.CanBePickedUp() && droppedItem.IsPlayerNearby())
            {
                nearbyItems.Add(droppedItem);
            }
        }

        // Update pickup prompt
        UpdatePickupPrompt();
    }

    /// <summary>
    /// Update pickup prompt UI
    /// </summary>
    private void UpdatePickupPrompt()
    {
        if (pickupPrompt == null)
            return;

        // If holding item, show drop prompt
        if (heldItem != null)
        {
            pickupPrompt.ShowDropPrompt(holdPoint);
            return;
        }

        // If near items, show pickup prompt on closest item
        if (nearbyItems.Count > 0)
        {
            DroppedItem closest = GetClosestItem();
            if (closest != null)
            {
                pickupPrompt.ShowPickupPrompt(closest.transform);
                return;
            }
        }

        // Otherwise hide prompt
        pickupPrompt.Hide();
    }

    /// <summary>
    /// Get closest item to player
    /// </summary>
    private DroppedItem GetClosestItem()
    {
        if (nearbyItems.Count == 0)
            return null;

        DroppedItem closest = nearbyItems[0];
        float closestDist = Vector3.Distance(transform.position, closest.transform.position);

        for (int i = 1; i < nearbyItems.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, nearbyItems[i].transform.position);
            if (dist < closestDist)
            {
                closest = nearbyItems[i];
                closestDist = dist;
            }
        }

        return closest;
    }

    /// <summary>
    /// Handle G key input for pickup/drop
    /// </summary>
    private void HandlePickupInput()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldItem != null)
            {
                DropItem();
            }
            else if (nearbyItems.Count > 0)
            {
                PickupClosestItem();
            }
        }
    }

    /// <summary>
    /// Pickup the closest item
    /// </summary>
    private void PickupClosestItem()
    {
        DroppedItem itemToPickup = GetClosestItem();
        if (itemToPickup == null)
            return;

        PickupItem(itemToPickup);
    }

    /// <summary>
    /// Pickup specific item
    /// </summary>
    public void PickupItem(DroppedItem droppedItem)
    {
        if (droppedItem == null || !droppedItem.CanBePickedUp())
            return;

        // Can only hold one item at a time
        if (heldItem != null)
        {
            Debug.LogWarning("ItemPickupController: Already holding an item!");
            return;
        }

        heldItem = droppedItem.gameObject;
        heldItemComponent = droppedItem;
        heldItemData = droppedItem.GetItemData();

        // Notify item it was picked up
        droppedItem.OnPickedUp();

        // Show item info UI
        if (itemInfoUI != null && heldItemData != null)
        {
            itemInfoUI.ShowItemInfo(heldItemData);
        }

        Debug.Log($"ItemPickupController: Picked up {heldItemData?.itemName}");
    }

    /// <summary>
    /// Drop currently held item
    /// </summary>
    public void DropItem()
    {
        if (heldItem == null)
            return;

        // Position item at player position (slightly offset)
        Vector3 dropPosition = transform.position + Vector3.down * 0.3f;
        heldItem.transform.position = dropPosition;

        // Notify item it was dropped
        if (heldItemComponent != null)
        {
            heldItemComponent.OnDropped();
        }

        // Hide item info UI
        if (itemInfoUI != null)
        {
            itemInfoUI.HideItemInfo();
        }

        Debug.Log($"ItemPickupController: Dropped {heldItemData?.itemName}");

        // Clear references
        heldItem = null;
        heldItemComponent = null;
        heldItemData = null;
    }

    /// <summary>
    /// Update held item position to follow player
    /// </summary>
    private void UpdateHeldItemPosition()
    {
        if (heldItem == null || heldItemData == null)
            return;

        Vector3 targetPosition = holdPoint.position + heldItemData.holdOffset;

        if (smoothFollow)
        {
            heldItem.transform.position = Vector3.Lerp(
                heldItem.transform.position,
                targetPosition,
                heldItemData.followSpeed * Time.deltaTime
            );
        }
        else
        {
            heldItem.transform.position = targetPosition;
        }
    }

    // Public getters
    public bool IsHoldingItem() => heldItem != null;
    public ItemDropData GetHeldItemData() => heldItemData;
    public GameObject GetHeldItem() => heldItem;

    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
