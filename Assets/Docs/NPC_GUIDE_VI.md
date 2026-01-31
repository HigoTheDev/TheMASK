# Hướng Dẫn Hệ Thống NPC - TheMASK Game

> **Tài liệu này hướng dẫn chi tiết cách setup và sử dụng hệ thống NPC với tính năng Dialogue và Item Drop**

---

## 📋 Mục Lục

1. [Tổng Quan Hệ Thống](#1-tổng-quan-hệ-thống)
2. [Setup Ban Đầu](#2-setup-ban-đầu)
3. [Tạo Dialogue Data](#3-tạo-dialogue-data)
4. [Tạo Item Drop Data](#4-tạo-item-drop-data)
5. [Setup NPC trong Scene](#5-setup-npc-trong-scene)
6. [Setup UI System](#6-setup-ui-system)
7. [Testing & Verification](#7-testing--verification)
8. [Import/Export Data](#8-importexport-data)
9. [Troubleshooting](#9-troubleshooting)

---

## 1. Tổng Quan Hệ Thống

### Các Components Chính

- **DialogueData** (ScriptableObject): Lưu trữ nội dung đối thoại
- **ItemDropData** (ScriptableObject): Định nghĩa items có thể rơi
- **NPCController**: Component chính của NPC, quản lý interaction
- **DialogueSystem**: Singleton manager điều khiển dialogue flow
- **ItemDropSystem**: Singleton manager xử lý item spawning
- **DialogueUI**: UI hiển thị dialogue
- **InteractionPrompt**: UI "Press F to interact"

### Luồng Hoạt Động

```
Player tiến lại gần NPC 
  ↓
InteractionPrompt xuất hiện
  ↓
Player nhấn F
  ↓
DialogueSystem bắt đầu dialogue
  ↓
Player nhấn Space để next câu thoại
  ↓
Dialogue kết thúc
  ↓
ItemDropSystem spawn items (nếu có)
```

---

## 2. Setup Ban Đầu

### Bước 1: Tạo Game Manager Objects

1. Trong **Hierarchy**, tạo Empty GameObject, đặt tên `GameManagers`
2. Thêm 2 child objects:
   - `DialogueSystem`
   - `ItemDropSystem`

### Bước 2: Thêm Components vào Managers

**DialogueSystem:**
1. Select `DialogueSystem` object trong Hierarchy
2. Trong **Inspector**, click **Add Component**
3. Tìm và thêm `DialogueSystem` script
4. Script sẽ tự động yêu cầu `DialogueUI` reference (sẽ setup sau)

**ItemDropSystem:**
1. Select `ItemDropSystem` object trong Hierarchy
2. Trong **Inspector**, click **Add Component**
3. Tìm và thêm `ItemDropSystem` script
4. (Optional) Set **Dropped Item Layer** = "Default" hoặc layer bạn muốn

---

## 3. Tạo Dialogue Data

### Bước 1: Tạo Folder Chứa Data

1. Trong **Project** window, navigate đến `Assets/`
2. Chuột phải → **Create** → **Folder**, đặt tên `Data`
3. Vào folder `Data`, tạo thêm folder `Dialogues`

### Bước 2: Tạo DialogueData ScriptableObject

1. Trong folder `Assets/Data/Dialogues/`, chuột phải
2. **Create** → **TheMASK** → **Dialogue Data**
3. Đặt tên file, ví dụ: `Merchant_Dialogue`

### Bước 3: Cấu Hình DialogueData trong Inspector

Sau khi tạo, click vào DialogueData asset, trong **Inspector**:

**NPC Information:**
- **NPC Name**: Nhập tên NPC (ví dụ: "Thương Nhân")
- **NPC Avatar**: (Tùy chọn) Kéo sprite avatar vào đây

**Dialogue Content:**
- **Dialogue Lines**: Click **+** để thêm từng câu thoại
  - Mỗi element là 1 câu
  - Nhập nội dung vào text area
  - Ví dụ:
    ```
    Element 0: "Chào mừng đến với cửa hàng của ta!"
    Element 1: "Ta có nhiều vật phẩm quý giá đây."
    Element 2: "Hãy quay lại sau nếu cần gì nhé!"
    ```

**Settings:**
- **Can Repeat**: ✓ (tick) nếu muốn player có thể nói chuyện lại
- **Auto Progress Delay**: Để = 0 (player phải nhấn Space để next)

### Ví Dụ DialogueData Hoàn Chỉnh

```
NPC Name: "Lão Giả Kim"
NPC Avatar: [Drag sprite here]
Dialogue Lines:
  - "Ngươi tìm gì ở đây?"
  - "Ta có thể dạy ngươi về giả kim thuật..."
  - "Nhưng ngươi cần chứng minh bản thân trước."
Can Repeat: ✓
Auto Progress Delay: 0
```

---

## 4. Tạo Item Drop Data

### Bước 1: Tạo Folder

1. Trong `Assets/Data/`, tạo folder `Items`

### Bước 2: Tạo ItemDropData ScriptableObject

1. Trong folder `Assets/Data/Items/`, chuột phải
2. **Create** → **TheMASK** → **Item Drop Data**
3. Đặt tên, ví dụ: `GoldCoin_Drop`

### Bước 3: Cấu Hình ItemDropData trong Inspector

**Item Information:**
- **Item Name**: Tên item (ví dụ: "Gold Coin")
- **Item Description**: Mô tả ngắn
- **Item Icon**: Kéo sprite của item vào đây

**Drop Settings:**
- **Item Prefab**: (Tùy chọn) Kéo prefab item vào đây
  - Nếu để null, hệ thống sẽ tạo simple sprite object
- **Drop Chance**: Xác suất drop (0-100)
  - 100 = luôn drop
  - 50 = 50% cơ hội
  - 0 = không drop
- **Drop Quantity Range**: Min-Max số lượng
  - X = Min (ví dụ: 1)
  - Y = Max (ví dụ: 3)
  - Kết quả: Random từ 1-3 items

**Spawn Settings:**
- **Spawn Offset**: Vị trí spawn so với NPC
  - X = 0 (không offset ngang)
  - Y = 0.5 (spawn cao hơn NPC 0.5 units)
- **Spawn Random Radius**: Độ phân tán (ví dụ: 0.3)
- **Launch Force**: Lực bật item
  - X = 0 (không bật ngang)
  - Y = 2 (bật lên trên)

### Ví Dụ ItemDropData Hoàn Chỉnh

**Gold Coin:**
```
Item Name: "Gold Coin"
Item Description: "Xu vàng quý giá"
Item Icon: [coin_sprite]
Item Prefab: [null hoặc coin_prefab]
Drop Chance: 100
Drop Quantity Range: (1, 3)
Spawn Offset: (0, 0.5)
Spawn Random Radius: 0.3
Launch Force: (0, 2)
```

**Rare Gem:**
```
Item Name: "Rare Gem"
Item Description: "Viên ngọc hiếm"
Item Icon: [gem_sprite]
Item Prefab: [null]
Drop Chance: 30
Drop Quantity Range: (1, 1)
Spawn Offset: (0, 0.5)
Spawn Random Radius: 0.2
Launch Force: (0, 3)
```

---

## 5. Setup NPC trong Scene

### Bước 1: Tạo NPC GameObject

1. Trong **Hierarchy**, chuột phải → **Create Empty**
2. Đặt tên `NPC_Merchant` (hoặc tên NPC của bạn)
3. Set **Position** phù hợp trong scene

### Bước 2: Thêm Visual cho NPC

1. Select NPC object
2. Chuột phải trong Hierarchy → **2D Object** → **Sprite**
3. Kéo sprite vào object con này
4. Đặt tên sprite object là `Visual`

### Bước 3: Thêm Collider (Trigger)

1. Select NPC object chính (không phải Visual)
2. **Inspector** → **Add Component** → `Circle Collider 2D`
3. Trong Circle Collider 2D component:
   - ✓ Tick **Is Trigger**
   - **Radius**: Set = 2 (phạm vi detect player)

### Bước 4: Thêm NPCController Component

1. Vẫn đang select NPC object
2. **Add Component** → Tìm `NPCController`
3. Inspector sẽ hiện các fields cần setup

### Bước 5: Cấu Hình NPCController trong Inspector

**NPC Data:**
- **Dialogue Data**: Kéo DialogueData asset đã tạo vào đây
  - Ví dụ: Kéo `Merchant_Dialogue` từ Project window
- **Item Drops**: Click **+** để thêm items
  - Kéo từng ItemDropData asset vào các element
  - Ví dụ: Element 0 = `GoldCoin_Drop`, Element 1 = `Rare_Gem`

**Interaction Settings:**
- **Player Layer**: Chọn layer của Player (thường là "Default")
- **Player Tag**: Nhập "Player" (đảm bảo Player có tag này)
- **Interact Key**: Để mặc định = `F`

**References:**
- **Interaction Prompt**: Để null (sẽ tự tìm), hoặc kéo InteractionPrompt object vào

**Item Drop Settings:**
- **Drop Items After Dialogue**: ✓ (tick) để drop items sau khi dialogue xong

### Bước 6: Setup Player Tag

1. Select Player object trong Hierarchy
2. Trong **Inspector**, phía trên cùng, chọn **Tag** → **Player**
   - Nếu chưa có tag "Player", tạo mới: **Add Tag** → **+** → Nhập "Player"

---

## 6. Setup UI System

### Phần A: Tạo Dialogue UI Canvas

#### Bước 1: Tạo Canvas

1. **Hierarchy** → Chuột phải → **UI** → **Canvas**
2. Đặt tên `DialogueCanvas`
3. Select Canvas, trong **Inspector**:
   - **Render Mode**: Screen Space - Overlay
   - **Pixel Perfect**: ✓ (optional)

#### Bước 2: Tạo Dialogue Panel

1. Select `DialogueCanvas`, chuột phải → **UI** → **Panel**
2. Đặt tên `DialoguePanel`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Bottom
   - **Pos Y**: 150
   - **Width**: 1000
   - **Height**: 200
4. Cấu hình **Image** component:
   - **Color**: Dark semi-transparent (R:0, G:0, B:0, A:200)

#### Bước 3: Thêm NPC Name Text

1. Select `DialoguePanel`, chuột phải → **UI** → **Text - TextMeshPro**
   - (Nếu lần đầu dùng TMP, click Import TMP Essentials)
2. Đặt tên `NPCNameText`
3. Cấu hình:
   - **Rect Transform**: Anchor top-left, Pos (20, -10), Width: 300, Height: 40
   - **Text**: "NPC Name" (placeholder)
   - **Font Size**: 24
   - **Color**: Yellow hoặc highlight color
   - **Alignment**: Left, Top

#### Bước 4: Thêm Dialogue Text

1. Select `DialoguePanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên `DialogueText`
3. Cấu hình:
   - **Rect Transform**: Stretch cả 4 cạnh, Left:20, Right:20, Top:50, Bottom:40
   - **Text**: "Dialogue content will appear here..." (placeholder)
   - **Font Size**: 20
   - **Color**: White
   - **Alignment**: Left, Top
   - **Wrapping**: Enabled

#### Bước 5: Thêm Continue Indicator

1. Select `DialoguePanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên `ContinueIndicator`
3. Cấu hình:
   - **Rect Transform**: Anchor bottom-right, Pos (-30, 10), Width: 200, Height: 30
   - **Text**: "Press SPACE to continue..."
   - **Font Size**: 16
   - **Color**: Light gray
   - **Alignment**: Right, Bottom

#### Bước 6: Thêm DialogueUI Component

1. Select `DialoguePanel`
2. **Add Component** → `DialogueUI`
3. Kéo references vào Inspector:
   - **Dialogue Panel**: Kéo chính object `DialoguePanel` vào đây
   - **Npc Name Text**: Kéo `NPCNameText` vào
   - **Dialogue Text**: Kéo `DialogueText` vào
   - **Npc Avatar Image**: (Optional) Tạo UI Image cho avatar, kéo vào
   - **Continue Indicator**: Kéo `ContinueIndicator` vào
4. Cấu hình settings:
   - **Typewriter Speed**: 30 (characters/second)
   - **Fade Duration**: 0.3

#### Bước 7: Link DialogueUI với DialogueSystem

1. Select `DialogueSystem` object trong Hierarchy
2. Trong **Inspector**, DialogueSystem component:
   - **Dialogue UI**: Kéo `DialoguePanel` (có DialogueUI component) vào đây
   - **Pause Game During Dialogue**: ✓

### Phần B: Tạo Interaction Prompt UI

#### Bước 1: Tạo Canvas cho Prompt (World Space)

1. **Hierarchy** → **UI** → **Canvas**
2. Đặt tên `InteractionPromptCanvas`
3. Cấu hình Canvas:
   - **Render Mode**: World Space
   - **Rect Transform**: Width: 2, Height: 0.5, Scale: 0.01 (cả X,Y,Z)

#### Bước 2: Tạo Prompt Panel

1. Select `InteractionPromptCanvas`, chuột phải → **UI** → **Panel**
2. Đặt tên `PromptPanel`
3. Cấu hình:
   - **Rect Transform**: Stretch full
   - **Image**: Background color semi-transparent

#### Bước 3: Thêm Prompt Text

1. Select `PromptPanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên `PromptText`
3. Cấu hình:
   - **Rect Transform**: Stretch full
   - **Text**: "Press F to interact"
   - **Font Size**: 32
   - **Color**: White
   - **Alignment**: Center, Middle

#### Bước 4: Thêm InteractionPrompt Component

1. Select `InteractionPromptCanvas` (root canvas)
2. **Add Component** → `InteractionPrompt`
3. Kéo references:
   - **Prompt Text**: Kéo `PromptText` vào
   - **Prompt Panel**: Kéo `PromptPanel` vào
4. Cấu hình settings:
   - **Interact Message**: "Press F to interact"
   - **Offset From Target**: X:0, Y:1.5 (cao hơn NPC)
   - **Animate Bounce**: ✓
   - **Bounce Speed**: 2
   - **Bounce Height**: 0.1

---

## 7. Testing & Verification

### Test 1: Basic Interaction

1. **Play** game
2. Di chuyển Player lại gần NPC
3. ✓ Interaction prompt xuất hiện trên NPC
4. Nhấn **F**
5. ✓ Dialogue UI hiển thị
6. ✓ NPC name và câu thoại đầu tiên xuất hiện
7. ✓ Game pause (Time.timeScale = 0)

### Test 2: Dialogue Progression

1. Trong dialogue, nhấn **Space**
2. ✓ Câu thoại thứ 2 hiển thị
3. Tiếp tục nhấn **Space** cho đến hết
4. ✓ Dialogue đóng lại
5. ✓ Game resume (Time.timeScale = 1)

### Test 3: Item Drop

1. Setup NPC với ItemDropData (drop chance 100%)
2. Nói chuyện với NPC đến hết
3. ✓ Item spawn tại vị trí NPC
4. ✓ Item có physics (rơi xuống, có thể có bounce)

### Test 4: Repeat Dialogue

**Nếu Can Repeat = true:**
1. Sau khi dialogue xong, đi ra xa rồi lại lại gần
2. ✓ Prompt xuất hiện lại
3. Nhấn F
4. ✓ Có thể nói chuyện lại

**Nếu Can Repeat = false:**
1. Sau dialogue xong, lại gần NPC
2. ✓ Prompt KHÔNG xuất hiện
3. ✓ Không thể nói chuyện lại

### Test 5: No Conflicts

1. Chạy game bình thường
2. ✓ Player movement vẫn hoạt động (WASD)
3. ✓ Player jump vẫn hoạt động (Space) - khi KHÔNG trong dialogue
4. Nhấn **E** để toggle mask
5. ✓ Mask system vẫn hoạt động bình thường
6. ✓ Camera follow vẫn theo player
7. ✓ Không có errors trong Console

---

## 8. Import/Export Data

### Cách 1: Duplicate DialogueData (Đơn Giản Nhất)

1. Trong **Project** window, tìm DialogueData asset muốn copy
2. **Ctrl+D** để duplicate
3. Đổi tên và chỉnh sửa nội dung trong Inspector

### Cách 2: Copy/Paste Dialogue Lines

1. Mở DialogueData trong Inspector
2. Expand **Dialogue Lines**
3. Chuột phải vào element → **Copy**
4. Mở DialogueData khác
5. Chuột phải → **Paste**

### Cách 3: Sử dụng JSON (Advanced)

Tạo file JSON để import bulk dialogue:

```json
{
  "npcName": "Blacksmith",
  "dialogueLines": [
    "Welcome to my forge!",
    "I can craft powerful weapons.",
    "Come back when you have materials."
  ],
  "canRepeat": true
}
```

**Lưu ý:** Unity không tự động import JSON vào ScriptableObject, cần viết thêm Editor script hoặc nhập thủ công.

### Best Practice: Quản Lý Data

**Cấu trúc folder đề xuất:**
```
Assets/
  Data/
    Dialogues/
      Merchants/
        Merchant_1.asset
        Merchant_2.asset
      NPCs/
        Villager_1.asset
        Villager_2.asset
      Bosses/
        Boss_Dialogue.asset
    Items/
      Common/
        Coin.asset
        Potion.asset
      Rare/
        Gem.asset
        Key.asset
```

---

## 9. Troubleshooting

### Vấn Đề: Interaction Prompt Không Hiện

**Nguyên nhân & Giải pháp:**

✗ **Player không có tag "Player"**
→ Select Player object → Set Tag = Player

✗ **Collider không phải trigger**
→ Select NPC → CircleCollider2D → ✓ Is Trigger

✗ **Layer hoặc tag không match**
→ NPCController Inspector → Kiểm tra Player Layer và Player Tag

✗ **InteractionPrompt không được tìm thấy**
→ Đảm bảo InteractionPromptCanvas có InteractionPrompt component

### Vấn Đề: Dialogue Không Bắt Đầu

✗ **DialogueSystem không có trong scene**
→ Tạo GameObject → Add DialogueSystem component

✗ **DialogueData chưa gán vào NPC**
→ NPCController Inspector → Gán DialogueData

✗ **DialogueUI không được link**
→ DialogueSystem Inspector → Gán DialogueUI reference

✗ **Dialogue Lines rỗng**
→ DialogueData → Thêm ít nhất 1 dialogue line

### Vấn Đề: Item Không Spawn

✗ **ItemDropSystem không có trong scene**
→ Tạo GameObject → Add ItemDropSystem component

✗ **Drop Chance = 0**
→ ItemDropData → Set Drop Chance > 0

✗ **Item Drops list rỗng trong NPC**
→ NPCController → Thêm ItemDropData vào Item Drops

### Vấn Đề: Game Không Resume Sau Dialogue

✗ **Time.timeScale bị stuck**
→ Check Console xem có errors không
→ Thử manually: Chạy code `Time.timeScale = 1f;` trong Console

✗ **Dialogue không kết thúc đúng**
→ Kiểm tra DialogueSystem.EndDialogue() có được gọi không

### Vấn Đề: Conflict Với Phím Bấm

✗ **Space bị conflict**
→ Trong dialogue, Space dùng để next câu thoại (OK)
→ Ngoài dialogue, Space dùng cho jump (OK)
→ Không conflict vì DialogueSystem check `IsDialogueActive()`

✗ **F bị conflict với tính năng khác**
→ Đổi Interact Key trong NPCController Inspector

---

## 🎯 Quick Reference

### Phím Tắt Trong Game

- **WASD / Arrow Keys**: Di chuyển
- **Space**: Jump (ngoài dialogue), Next dialogue (trong dialogue)
- **E**: Toggle mask
- **F**: Tương tác với NPC
- **1/2/3**: Đổi mask

### Script Components Cần Thiết

| GameObject | Required Components |
|------------|---------------------|
| DialogueSystem | DialogueSystem.cs |
| ItemDropSystem | ItemDropSystem.cs |
| NPC | NPCController.cs, CircleCollider2D (trigger) |
| DialoguePanel | DialogueUI.cs, CanvasGroup |
| InteractionPromptCanvas | InteractionPrompt.cs |

### Inspector Quick Checklist

**NPCController:**
- ✓ DialogueData assigned
- ✓ ItemDrops có items (nếu muốn drop)
- ✓ Player Tag = "Player"
- ✓ Interact Key = F

**DialogueSystem:**
- ✓ DialogueUI reference assigned
- ✓ Pause Game = true (nếu muốn)

**ItemDropSystem:**
- ✓ Dropped Item Layer set
- ✓ (Optional) Default Item Prefab

---

## 📝 Tổng Kết

Hệ thống NPC đã hoàn chỉnh với:

✅ Dialogue system dễ dàng mở rộng qua ScriptableObject
✅ Item drop system linh hoạt với drop chance và randomization
✅ UI clean và có animation
✅ Không conflict với hệ thống hiện tại (Player, Mask, Camera)
✅ Dễ dàng thêm NPC mới mà không cần code

**Để thêm NPC mới:**
1. Tạo DialogueData + ItemDropData
2. Tạo GameObject + NPCController
3. Gán data vào Inspector
4. Done! ✨

---

*Tài liệu được tạo cho TheMASK Game - v1.0*
