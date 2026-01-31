# Hướng Dẫn Setup Hệ Thống Nhặt Item - TheMASK Game

> **Tài liệu hướng dẫn từng bước setup hệ thống pickup item với thông tin cốt truyện**

---

## 📋 Mục Lục

1. [Tổng Quan](#1-tổng-quan)
2. [Setup Player - ItemPickupController](#2-setup-player---itempickupcontroller)
3. [Tạo ItemInfoUI Canvas](#3-tạo-iteminfoui-canvas)
4. [Tạo PickupPrompt Canvas](#4-tạo-pickupprompt-canvas)
5. [Config ItemDropData với Story](#5-config-itemdropdata-với-story)
6. [Testing Hệ Thống](#6-testing-hệ-thống)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Tổng Quan

### Hệ Thống Hoạt Động Như Thế Nào?

```
Player tiến lại gần item (từ NPC drop)
  ↓
Item highlight (ánh sáng vàng)
  ↓
"Press G to pickup" xuất hiện
  ↓
Player nhấn G
  ↓
Item bay lên, follow player ở trên đầu
  ↓
UI góc màn hình hiển thị thông tin cốt truyện
  ↓
Player nhấn G lần nữa
  ↓
Item rơi xuống vị trí player
  ↓
Có thể nhặt lại
```

### Components Cần Thiết

- **ItemPickupController** (trên Player) - Xử lý nhặt/thả
- **ItemInfoUI** (Screen Space Canvas) - Hiển thị story
- **PickupPrompt** (World Space Canvas) - Hiển thị prompt "Press G"
- **ItemDropData** (ScriptableObject) - Chứa story data

---

## 2. Setup Player - ItemPickupController

### Bước 1: Thêm Component vào Player

1. Trong **Hierarchy**, select **Player** object
2. Trong **Inspector**, click **Add Component**
3. Tìm và chọn `ItemPickupController`

### Bước 2: Cấu Hình Inspector

Sau khi thêm component, bạn sẽ thấy các settings:

**Pickup Settings:**
- **Pickup Key**: Để mặc định = `G` (hoặc đổi phím khác nếu muốn)
- **Item Layer**: Chọn layer của items (thường để `Default`)
- **Item Tag**: Nhập `"Item"` (hoặc tag bạn dùng cho items)
- **Pickup Radius**: `2` (bán kính phát hiện item, tính bằng units)

**Hold Settings:**
- **Hold Point**: (Có thể để null - sẽ dùng Player transform)
  - Nếu muốn custom: Tạo Empty GameObject con của Player, đặt tên `ItemHoldPoint`, kéo vào đây
- **Smooth Follow**: ✓ (tick) - Item sẽ follow mượt mà

**UI References:**
- **Item Info UI**: Để null (sẽ tự tìm sau khi tạo UI)
- **Pickup Prompt**: Để null (sẽ tự tìm sau khi tạo UI)

### Bước 3: Kiểm Tra Player Tag

1. Vẫn đang select Player object
2. Ở đầu **Inspector**, check **Tag** = `Player`
3. Nếu chưa đúng, chọn dropdown → `Player`

> ✅ **Checkpoint:** Player đã có ItemPickupController component với pickup radius = 2

---

## 3. Tạo ItemInfoUI Canvas

UI này sẽ hiển thị ở góc màn hình (ví dụ: top-right) khi player cầm item.

### Bước 1: Tạo Canvas

1. **Hierarchy** → Chuột phải → **UI** → **Canvas**
2. Đặt tên Canvas: `ItemInfoCanvas`
3. Select canvas, trong **Inspector**:
   - **Render Mode**: Screen Space - Overlay
   - **Pixel Perfect**: ✓ (optional)
   - **Sort Order**: 10 (để hiển thị trên các UI khác)

### Bước 2: Tạo Info Panel

1. Select `ItemInfoCanvas`, chuột phải → **UI** → **Panel**
2. Đặt tên: `ItemInfoPanel`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Top-Right
   - Click anchor preset (góc trái trên Inspector) → Hold Alt+Shift → Click top-right preset
   - **Pivot**: (1, 1)
   - **Pos X**: -20
   - **Pos Y**: -20
   - **Width**: 300
   - **Height**: 200

4. Cấu hình **Image** component (background):
   - **Color**: Dark semi-transparent
   - Ví dụ: R:0, G:0, B:0, A:180

### Bước 3: Thêm Item Icon

1. Select `ItemInfoPanel`, chuột phải → **UI** → **Image**
2. Đặt tên: `ItemIcon`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Top-Left (trong panel)
   - **Pos X**: 20
   - **Pos Y**: -20
   - **Width**: 64
   - **Height**: 64

4. Cấu hình **Image**:
   - **Preserve Aspect**: ✓
   - **Source Image**: (sẽ set runtime, để placeholder hoặc null)

### Bước 4: Thêm Item Name Text

1. Select `ItemInfoPanel`, chuột phải → **UI** → **Text - TextMeshPro**
   - (Nếu prompt import TMP, click "Import TMP Essentials")
2. Đặt tên: `ItemNameText`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Top stretch (resize theo width panel)
   - Left: 100, Right: 20, Top: 20, Height: 30

4. Cấu hình **TextMeshProUGUI**:
   - **Text**: "Item Name" (placeholder)
   - **Font Size**: 20
   - **Font Style**: Bold
   - **Color**: White hoặc Yellow
   - **Alignment**: Left, Top
   - **Wrapping**: Enabled

### Bước 5: Thêm Story Title Text

1. Select `ItemInfoPanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên: `StoryTitleText`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Top stretch
   - Left: 100, Right: 20, Top: 55, Height: 25

4. Cấu hình **TextMeshProUGUI**:
   - **Text**: "Story Title" (placeholder)
   - **Font Size**: 16
   - **Font Style**: Italic
   - **Color**: Light yellow hoặc gold
   - **Alignment**: Left, Top

### Bước 6: Thêm Story Description Text

1. Select `ItemInfoPanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên: `StoryDescriptionText`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Stretch (cả 4 cạnh)
   - Left: 20, Right: 20, Top: 85, Bottom: 20

4. Cấu hình **TextMeshProUGUI**:
   - **Text**: "Story description will appear here..." (placeholder)
   - **Font Size**: 14
   - **Color**: Light gray hoặc white
   - **Alignment**: Left, Top
   - **Wrapping**: Enabled
   - **Overflow**: Truncate

### Bước 7: (Optional) Thêm Read More Button

1. Select `ItemInfoPanel`, chuột phải → **UI** → **Button - TextMeshPro**
2. Đặt tên: `ReadMoreButton`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Bottom-Right
   - Pos X: -10, Pos Y: 10, Width: 100, Height: 30

4. Cấu hình button text:
   - Text: "Read More..."
   - Font Size: 12

### Bước 8: Thêm ItemInfoUI Component

1. Select `ItemInfoPanel` (chính panel, không phải canvas)
2. **Add Component** → Tìm `ItemInfoUI`
3. Kéo references vào Inspector:

**UI References:**
- **Info Panel**: Kéo chính `ItemInfoPanel` vào đây
- **Item Icon Image**: Kéo `ItemIcon` vào
- **Item Name Text**: Kéo `ItemNameText` vào
- **Story Title Text**: Kéo `StoryTitleText` vào
- **Story Description Text**: Kéo `StoryDescriptionText` vào
- **Read More Button**: (Optional) Kéo `ReadMoreButton` vào

**Animation Settings:**
- **Fade Duration**: 0.3
- **Slide Distance**: 50

> ✅ **Checkpoint:** ItemInfoPanel có tất cả text fields và ItemInfoUI component đã link đầy đủ

---

## 4. Tạo PickupPrompt Canvas

UI này sẽ hiển thị ở World Space (trên item hoặc player) khi có thể pickup/drop.

### Bước 1: Tạo World Space Canvas

1. **Hierarchy** → **UI** → **Canvas**
2. Đặt tên: `PickupPromptCanvas`
3. Cấu hình Canvas:
   - **Render Mode**: **World Space** (quan trọng!)
   - **Event Camera**: Kéo Main Camera vào
   - **Sort Order**: 10

4. Cấu hình **Rect Transform**:
   - **Width**: 2
   - **Height**: 0.5
   - **Scale**: X: 0.01, Y: 0.01, Z: 0.01
   - **Position**: (0, 3, 0) - tạm thời, sẽ move runtime

### Bước 2: Tạo Prompt Panel

1. Select `PickupPromptCanvas`, chuột phải → **UI** → **Panel**
2. Đặt tên: `PromptPanel`
3. Cấu hình **Rect Transform**:
   - Stretch full (Left:0, Right:0, Top:0, Bottom:0)

4. Cấu hình **Image**:
   - **Color**: Semi-transparent dark background
   - Ví dụ: R:0, G:0, B:0, A:150

### Bước 3: Thêm Prompt Text

1. Select `PromptPanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên: `PromptText`
3. Cấu hình **Rect Transform**:
   - Stretch full

4. Cấu hình **TextMeshProUGUI**:
   - **Text**: "Press G to pickup"
   - **Font Size**: 32
   - **Color**: White hoặc bright yellow
   - **Alignment**: Center, Middle
   - **Wrapping**: Disabled

### Bước 4: (Optional) Thêm Icon Phím G

1. Select `PromptPanel`, chuột phải → **UI** → **Image**
2. Đặt tên: `KeyIcon`
3. Position bên cạnh text
4. Kéo sprite icon phím G vào (nếu có)

### Bước 5: Thêm PickupPrompt Component

1. Select `PickupPromptCanvas` (canvas root, không phải panel)
2. **Add Component** → `PickupPrompt`
3. Cấu hình Inspector:

**UI References:**
- **Prompt Text**: Kéo `PromptText` vào
- **Prompt Panel**: Kéo `PromptPanel` vào
- **Prompt Icon**: (Optional) Kéo `KeyIcon` vào nếu có

**Settings:**
- **Pickup Message**: "Press G to pickup"
- **Drop Message**: "Press G to drop"
- **Offset From Target**: X:0, Y:1, Z:0 (cao hơn item 1 unit)

**Animation:**
- **Animate Bounce**: ✓
- **Bounce Speed**: 2
- **Bounce Height**: 0.1

> ✅ **Checkpoint:** PickupPromptCanvas (World Space) đã setup xong với PickupPrompt component

---

## 5. Config ItemDropData với Story

Bây giờ thêm thông tin cốt truyện vào items có sẵn hoặc tạo mới.

### Bước 1: Mở ItemDropData Có Sẵn

1. Trong **Project** window, navigate đến `Assets/Data/Items/`
2. Select một ItemDropData asset (ví dụ: `GoldCoin_Drop.asset`)

### Bước 2: Điền Story Information

Trong **Inspector**, section **Story Information**:

**Ví dụ cho Gold Coin:**
```
Story Title: "Coins of the Lost Kingdom"

Story Description:
"These golden coins bear the mark of an ancient 
kingdom that vanished centuries ago. Local 
legends say they still hold mysterious power..."

Full Lore: (Optional - text dài hơn)
"Long before the current era, the Golden Kingdom 
ruled these lands with wisdom and prosperity. 
When darkness fell, the kingdom disappeared 
overnight, leaving only these coins as evidence 
of its existence. Those who hold them claim to 
hear whispers from the past..."

Detailed Icon: [Kéo high-res sprite nếu có, hoặc để null]
```

**Ví dụ khác - Mysterious Key:**
```
Story Title: "Key to the Vault of Secrets"

Story Description:
"An ornate key discovered in ancient ruins. 
No one knows what lock it opens, but its 
intricate design suggests great importance."

Full Lore:
"This key was forged by the last Guardian of 
the kingdom. It is said to unlock the Vault 
of Secrets, hidden deep beneath the castle. 
Inside lie the kingdom's greatest treasures 
and darkest mysteries..."

Detailed Icon: [high-res key sprite]
```

### Bước 3: Config Pickup Settings

Vẫn trong Inspector, section **Pickup Settings**:

- **Can Be Picked Up**: ✓ (tick)
- **Hold Offset**: 
  - X: 0
  - Y: 1.5 (item sẽ float 1.5 units trên player)
  - Z: 0
- **Follow Speed**: 10 (càng cao càng follow nhanh)

### Bước 4: Tạo ItemDropData Mới (Nếu Muốn)

1. `Assets/Data/Items/` → Chuột phải
2. **Create** → **TheMASK** → **Item Drop Data**
3. Đặt tên: `AncientScroll_Drop`
4. Điền đầy đủ thông tin theo mẫu trên

### Bước 5: Gán Item vào NPC (Nếu Chưa)

1. Select NPC trong Hierarchy
2. NPCController component → **Item Drops** → Click **+**
3. Kéo ItemDropData asset vào element mới tạo

> ✅ **Checkpoint:** Ít nhất 1 ItemDropData đã có story information đầy đủ

---

## 6. Testing Hệ Thống

### Test 1: Pickup Flow

1. **Play** game
2. Nói chuyện với NPC để item drop
3. Di chuyển Player lại gần item
4. ✓ Item sáng lên (yellow highlight)
5. ✓ "Press G to pickup" xuất hiện trên item
6. Nhấn **G**
7. ✓ Item bay lên, follow player
8. ✓ ItemInfoUI xuất hiện ở góc màn hình
9. ✓ Hiển thị: icon, tên item, story title, story description

### Test 2: Drop Flow

1. Đang cầm item (từ Test 1)
2. ✓ Prompt đổi thành "Press G to drop" (ở player)
3. Nhấn **G**
4. ✓ Item rơi xuống vị trí player
5. ✓ ItemInfoUI biến mất
6. ✓ Item có thể nhặt lại

### Test 3: Multiple Items

1. Drop 2-3 items khác nhau từ NPCs
2. Nhặt từng item
3. ✓ Mỗi item hiển thị story khác nhau
4. ✓ Closest item được highlight và show prompt

### Test 4: No Conflicts

Trong khi test, kiểm tra:
- ✓ WASD movement vẫn hoạt động
- ✓ Space jump vẫn hoạt động (khi không hold item)
- ✓ E toggle mask vẫn hoạt động
- ✓ F interact NPC vẫn hoạt động
- ✓ G không làm conflict các phím khác

### Test 5: Edge Cases

**Multiple items nearby:**
- Drop 3 items gần nhau
- Walk vào giữa
- ✓ Chỉ 1 prompt xuất hiện (trên closest item)
- ✓ Nhặt đúng item gần nhất

**Try pickup while holding:**
- Đang cầm item A
- Đi tới item B, nhấn G
- ✓ Item A drop, không pickup B (hoặc tùy logic)

**Item without story:**
- ItemDropData không có story info
- Pickup item
- ✓ Vẫn pickup được, UI có thể show placeholder hoặc hide

---

## 7. Troubleshooting

### Vấn Đề: Không Nhặt Được Item

**Nguyên nhân & Giải pháp:**

✗ **ItemPickupController không có trên Player**
→ Add component vào Player

✗ **Pickup radius quá nhỏ**
→ Tăng Pickup Radius trong ItemPickupController (thử 2-3)

✗ **Item không có DroppedItem component**
→ Items từ ItemDropSystem sẽ tự động có, check lại

✗ **Item tag/layer không match**
→ ItemPickupController → Check Item Tag = tag trên item GameObject

✗ **Can Be Picked Up = false**
→ ItemDropData → Set Can Be Picked Up = ✓

### Vấn Đề: Item Highlight Không Hiện

✗ **Player không có tag "Player"**
→ Player object → Tag = Player

✗ **DroppedItem không detect player**
→ DroppedItem Inspector → Set Player Tag = "Player"

✗ **Item không có collider trigger**
→ Item phải có CircleCollider2D với Is Trigger = ✓

### Vấn Đề: UI Không Hiển Thị

✗ **ItemInfoUI không được tìm thấy**
→ ItemPickupController → Manual drag ItemInfoPanel vào Item Info UI field

✗ **Canvas bị disable**
→ Check ItemInfoCanvas active trong Hierarchy

✗ **References chưa link**
→ ItemInfoUI component → Link hết các Text/Image references

✗ **CanvasGroup alpha = 0**
→ ItemInfoPanel → CanvasGroup → Alpha = 1 (để test)

### Vấn Đề: Prompt Không Xuất Hiện

✗ **PickupPrompt không được tìm thấy**
→ ItemPickupController → Drag PickupPromptCanvas vào

✗ **World Space Canvas không đúng**
→ PickupPromptCanvas → Render Mode = World Space

✗ **Event Camera không gán**
→ PickupPromptCanvas → Event Camera = Main Camera

### Vấn Đề: Item Không Follow Player

✗ **Item đã bị destroy**
→ Check Console log xem có errors không

✗ **Smooth Follow settings**
→ ItemPickupController → Smooth Follow = ✓

✗ **Follow Speed = 0**
→ ItemDropData → Follow Speed > 0 (khuyến nghị 10)

### Vấn Đề: Story Text Không Hiện

✗ **Story fields để trống**
→ ItemDropData → Điền Story Title và Story Description

✗ **Text components không active**
→ StoryTitleText, StoryDescriptionText → Check active

✗ **Font size quá nhỏ hoặc color trùng background**
→ Adjust font size và color trong TextMeshProUGUI

---

## 🎯 Quick Checklist

Checklist nhanh để đảm bảo setup đúng:

### Player Setup
- ✓ Player có ItemPickupController component
- ✓ Pickup Radius = 2
- ✓ Player Tag = "Player"

### UI Setup
- ✓ ItemInfoCanvas (Screen Space) với ItemInfoPanel
- ✓ ItemInfoPanel có: Icon, NameText, TitleText, DescriptionText
- ✓ ItemInfoPanel có ItemInfoUI component với references đầy đủ
- ✓ PickupPromptCanvas (World Space) với PromptText
- ✓ PickupPromptCanvas có PickupPrompt component

### Data Setup
- ✓ Ít nhất 1 ItemDropData có:
  - Story Title filled
  - Story Description filled
  - Can Be Picked Up = ✓
  - Hold Offset = (0, 1.5, 0)
  - Follow Speed = 10

### Testing
- ✓ Item drops from NPC
- ✓ Item highlights when player near
- ✓ G picks up item
- ✓ UI shows story info
- ✓ G drops item
- ✓ No conflicts with other keys

---

## 📝 Tóm Tắt Setup Nhanh

**3 bước chính:**

1. **Player**: Add ItemPickupController, set radius = 2
2. **UI**: Create 2 canvases (ItemInfo + PickupPrompt) với các text/image cần thiết
3. **Data**: Fill story info vào ItemDropData

**Test:**
- Drop item → Walk near → Press G → See story → Press G again → Drop

---

## 🌟 Tips & Best Practices

### Story Writing Tips

**Good Story Description:**
- Ngắn gọn (2-4 câu)
- Tạo mystery hoặc curiosity
- Liên kết với lore game
- Khuyến khích exploration

**Example:**
```
✓ Good:
"A weathered journal from the last Guardian. 
Its pages speak of a hidden power deep within 
the mask..."

✗ Too Long:
"This is a journal that belonged to someone 
who was a Guardian a long time ago and it has 
many pages and contains information about..."
```

### UI Layout Tips

**ItemInfoUI Position:**
- Top-right: Không chặn gameplay view
- Bottom-right: Gần minimap nếu có
- Left side: Nếu có inventory UI right side

**Kích thước:**
- Width: 250-350px (đủ đọc, không quá to)
- Height: Auto-fit dựa vào content

### Performance Tips

- Chỉ tạo 1 ItemInfoCanvas cho toàn game (singleton)
- Chỉ tạo 1 PickupPromptCanvas (reuse cho mọi item)
- Disable canvas khi không dùng thay vì destroy

---

*Hệ thống pickup đã sẵn sàng! Bắt đầu tạo items với câu chuyện thú vị nhé!* ✨
