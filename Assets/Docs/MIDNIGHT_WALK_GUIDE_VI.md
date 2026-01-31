# Hướng Dẫn Setup Level Midnight Walk - TheMASK Game

> **Tài liệu hướng dẫn chi tiết setup level Midnight Walk với mask-dependent dialogue và multiple endings**

---

## 📋 Mục Lục

1. [Tổng Quan Level](#1-tổng-quan-level)
2. [Setup Managers](#2-setup-managers)
3. [Setup Ending UI](#3-setup-ending-ui)
4. [Tạo Dialogue Data Cho NPCs](#4-tạo-dialogue-data-cho-npcs)
5. [Setup NPCs](#5-setup-npcs)
6. [Setup Player](#6-setup-player)
7. [Testing Level](#7-testing-level)
8. [Troubleshooting](#8-troubleshooting)

---

## 1. Tổng Quan Level

### Level Flow

```
START
  ↓
Player đi bộ trên phố (không có combat, peaceful)
  ↓
Gặp NPC 1 (The Smoker)
  → Nếu không đeo mask → NPC tấn công
  → Nếu đeo mask → Dialogue (3 variants theo mask type)
  ↓
Gặp NPC 2 (The Shopkeeper)
  → Tương tự NPC 1
  ↓
Gặp NPC 3 (Person on Bench)
  → Tương tự NPC 1 & 2
  ↓
Kết thúc level
  → Ending được tính dựa trên 3 lựa chọn mask
  → 3 loại ending: Good / Neutral / Bad
```

### Mechanics Chính

1. **3 Masks** (Keys 1, 2, 3): Honesty, Kindness, Indifference
2. **Mask Requirement**: NPCs yêu cầu player đeo mask
3. **Combat**: NPC tấn công nếu player không đeo mask
4. **Branching Dialogue**: 3 dialogues khác nhau per NPC
5. **Choice Tracking**: System ghi lại mask đã dùng
6. **Multiple Endings**: Kết thúc dựa trên tổ hợp choices

---

## 2. Setup Managers

### Bước 1: Tạo MaskChoiceTracker

1. **Hierarchy** → Chuột phải → **Create Empty**
2. Đặt tên: `MaskChoiceTracker`
3. **Add Component** → Tìm `MaskChoiceTracker`
4. Không cần config gì thêm (Singleton tự động hoạt động)

> ✅ **Checkpoint:** GameObject "MaskChoiceTracker" có component MaskChoiceTracker

### Bước 2: Tạo MidnightWalkManager

1. **Hierarchy** → Chuột phải → **Create Empty**
2. Đặt tên: `MidnightWalkManager`
3. **Add Component** → `MidnightWalkManager`
4. Cấu hình trong **Inspector**:
   - **Required NPC Count**: `3` (số NPCs cần interact để kết thúc)
   - **Auto Trigger Ending**: ✓ (tự động show ending khi đủ)
   - **Level NPCs**: Để trống (hoặc drag 3 NPCs vào - optional)

### Bước 3: Tạo EndingManager

1. **Hierarchy** → Chuột phải → **Create Empty**
2. Đặt tên: `EndingManager`
3. **Add Component** → `EndingManager`
4. Cấu hình:
   - **Ending UI**: Để null (sẽ gán sau khi tạo UI)
   - **Disable Player On Ending**: ✓

> ✅ **Checkpoint:** 3 managers đã được tạo và configured

---

## 3. Setup Ending UI

### Bước 1: Tạo Canvas

1. **Hierarchy** → **UI** → **Canvas**
2. Đặt tên: `EndingCanvas`
3. Cấu hình Canvas:
   - **Render Mode**: Screen Space - Overlay
   - **Sort Order**: 100 (hiển thị trên tất cả UI khác)

### Bước 2: Tạo Ending Panel

1. Select `EndingCanvas`, chuột phải → **UI** → **Panel**
2. Đặt tên: `EndingPanel`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Stretch full (All sides = 0)
4. Cấu hình **Image**:
   - **Color**: Trong suốt hoàn toàn (Alpha = 0)
   - Hoặc disable Image component

### Bước 3: Tạo Black Overlay

1. Select `EndingPanel`, chuột phải → **UI** → **Image**
2. Đặt tên: `BlackOverlay`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Stretch full (All sides = 0)
4. Cấu hình **Image**:
   - **Color**: Black (R:0, G:0, B:0)
   - **Alpha**: 0 (sẽ fade lên 1 khi ending)
   - **Source Image**: None (hoặc white square)

### Bước 4: Tạo Ending Text

1. Select `EndingPanel`, chuột phải → **UI** → **Text - TextMeshPro**
2. Đặt tên: `EndingText`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Center
   - **Width**: 800
   - **Height**: 400
   - **Pos X**: 0
   - **Pos Y**: 50

4. Cấu hình **TextMeshProUGUI**:
   - **Text**: "Ending will appear here..." (placeholder)
   - **Font Size**: 36
   - **Color**: White
   - **Alignment**: Center, Middle
   - **Wrapping**: Enabled
   - **Auto Size**: Disable

5. **Add CanvasGroup** component to EndingText
   - Component menu → **Canvas Group**
   - **Alpha**: 0 (sẽ fade lên)

### Bước 5: (Optional) Tạo Play Again Button

1. Select `EndingPanel`, chuột phải → **UI** → **Button - TextMeshPro**
2. Đặt tên: `PlayAgainButton`
3. Cấu hình **Rect Transform**:
   - **Anchor**: Bottom-Center
   - **Pos X**: 0
   - **Pos Y**: 80
   - **Width**: 200
   - **Height**: 50

4. Cấu hình button text:
   - Text: "Chơi Lại"
   - Font Size: 20

5. **Initially hide** button:
   - Disable GameObject (sẽ enable sau ending)

### Bước 6: Add EndingUI Component

1. Select `EndingPanel` (chính panel, không phải canvas)
2. **Add Component** → `EndingUI`
3. Kéo references vào Inspector:

**UI Components:**
- **Ending Panel**: Kéo chính `EndingPanel` GameObject vào
- **Ending Text**: Kéo `EndingText` vào
- **Black Overlay**: Kéo `BlackOverlay` Image vào
- **Play Again Button**: (Optional) Kéo `PlayAgainButton` vào

**Animation Settings:**
- **Fade Duration**: 2 (thời gian fade to black)
- **Text Fade Duration**: 1.5 (thời gian fade in text)
- **Text Delay**: 1 (delay trước khi show text)

**Ending Texts:**
- **Good Ending Text**:
```
Trong đêm vắng,
chỉ một chút dịu dàng
cũng đủ làm người ta nhớ.
```

- **Neutral Ending Text**:
```
Bạn đã nói những điều thật.
Nhưng không phải đêm nào
cũng cần sự thật.
```

- **Bad Ending Text**:
```
Bạn đi hết con đường.
Nhưng chẳng ai nhớ
bạn đã từng đi qua.
```

### Bước 7: Link EndingUI to EndingManager

1. Select `EndingManager` GameObject
2. **EndingUI** field → Kéo `EndingPanel` vào

### Bước 8: Hide Ending Panel Initially

1. Select `EndingPanel`
2. **Disable GameObject** trong Inspector
3. (EndingUI sẽ tự enable khi cần)

> ✅ **Checkpoint:** EndingCanvas setup xong, EndingUI component đã link đầy đủ

---

## 4. Tạo Dialogue Data Cho NPCs

Mỗi NPC cần **3 DialogueData** (1 cho mỗi mask) và **1 MaskDialogueSet**.

### NPC 1: The Smoker

#### Step 1: Tạo 3 DialogueData

**Location:** `Assets/Data/Dialogues/Smoker/`

**A. Smoker_Honesty.asset**
```
Right-click → Create → TheMASK → Dialogue Data
Name: "Smoker_Honesty"

Inspector settings:
  NPC Name: "The Smoker"
  NPC Icon: [cigarette icon hoặc character icon]
  
  Dialogue Lines:
    [0] Speaker: NPC
        Text: "Đêm nay… yên tĩnh quá."
        
    [1] Speaker: Player
        Text: "Yên tĩnh vì chẳng còn ai muốn nói chuyện."
        
    [2] Speaker: NPC
        Text: "..."
        
    [3] Speaker: NPC
        Text: "*Anh ta nhìn xuống, như bị chạm vào nỗi cô đơn sâu thẳm*"
  
  Settings:
    - Typing Speed: 0.05
    - Can Repeat: ☐ (không cho repeat)
    - Pause Game: ✓
```

**B. Smoker_Kindness.asset**
```
Name: "Smoker_Kindness"

Dialogue Lines:
    [0] Speaker: NPC
        Text: "Đêm nay… yên tĩnh quá."
        
    [1] Speaker: Player
        Text: "Yên tĩnh đôi khi cũng dễ chịu."
        
    [2] Speaker: NPC
        Text: "Ừ... có lẽ vậy."
        
    [3] Speaker: NPC
        Text: "*Anh ta thở ra làn khói, thả lỏng vai*"
```

**C. Smoker_Indifference.asset**
```
Name: "Smoker_Indifference"

Dialogue Lines:
    [0] Speaker: NPC
        Text: "Đêm nay… yên tĩnh quá."
        
    [1] Speaker: Player
        Text: "..."
        
    [2] Speaker: NPC
        Text: "..."
        
    [3] Speaker: NPC
        Text: "*Anh ta quay đi, tiếp tục hút thuốc trong im lặng*"
```

#### Step 2: Tạo MaskDialogueSet

```
Location: Assets/Data/Dialogues/
Right-click → Create → TheMASK → Mask Dialogue Set
Name: "Smoker_MaskDialogues"

Inspector:
  - Honesty Dialogue: Drag "Smoker_Honesty"
  - Kindness Dialogue: Drag "Smoker_Kindness"
  - Indifference Dialogue: Drag "Smoker_Indifference"
```

### NPC 2: The Shopkeeper

#### Create DialogueData assets:

**Shopkeeper_Honesty.asset**
```
NPC Name: "The Shopkeeper"

Lines:
  [0] NPC: "Cậu đi muộn thế?"
  [1] Player: "Vì ban ngày quá mệt mỏi."
  [2] NPC: "*Thở dài*"
  [3] NPC: "Ta hiểu mà... Ta cũng thế."
```

**Shopkeeper_Kindness.asset**
```
Lines:
  [0] NPC: "Cậu đi muộn thế?"
  [1] Player: "Cháu chỉ muốn đi cho nhẹ đầu."
  [2] NPC: "*Mỉm cười nhẹ*"
  [3] NPC: "Đi nhẹ nhàng nhé, con."
```

**Shopkeeper_Indifference.asset**
```
Lines:
  [0] NPC: "Cậu đi muộn thế?"
  [1] Player: "Không liên quan đâu."
  [2] NPC: "..."
  [3] NPC: "*Ngạc nhiên, im lặng nhìn theo*"
```

#### Create MaskDialogueSet:
```
Name: "Shopkeeper_MaskDialogues"
Link 3 dialogues tương tự Smoker
```

### NPC 3: Person on Bench

**PersonOnBench_Honesty.asset**
```
NPC Name: "Person on Bench"

Lines:
  [0] NPC: "Cậu có tin là ban đêm con người thật hơn không?"
  [1] Player: "Có. Vì chẳng ai nhìn thấy ta."
  [2] NPC: "*Nhìn thẳng vào mắt bạn*"
  [3] NPC: "Đúng vậy..."
```

**PersonOnBench_Kindness.asset**
```
Lines:
  [0] NPC: "Cậu có tin là ban đêm con người thật hơn không?"
  [1] Player: "Có lẽ vì ta không cần diễn."
  [2] NPC: "*Cười nhẹ*"
  [3] NPC: "Cậu nói hay đấy."
```

**PersonOnBench_Indifference.asset**
```
Lines:
  [0] NPC: "Cậu có tin là ban đêm con người thật hơn không?"
  [1] Player: "Tôi không nghĩ vậy."
  [2] NPC: "..."
  [3] NPC: "*Quay mặt đi, nhìn ra phía khác*"
```

#### Create MaskDialogueSet:
```
Name: "PersonOnBench_MaskDialogues"
Link 3 dialogues
```

> ✅ **Checkpoint:** 9 DialogueData + 3 MaskDialogueSet assets đã được tạo

---

## 5. Setup NPCs

### Bước 1: Tạo NPC GameObject

**For NPC 1 (The Smoker):**

1. **Hierarchy** → **Create Empty** (hoặc drag character sprite)
2. Đặt tên: `NPC_Smoker`
3. **Position**: Đặt ở vị trí phù hợp trong scene (dưới đèn đường)

### Bước 2: Add Required Components

**A. Add SpriteRenderer** (nếu chưa có)
```
Add Component → Sprite Renderer
- Sprite: [smoker character sprite]
- Sorting Layer: Characters (hoặc tương tự)
```

**B. Add Collider2D**
```
Add Component → Circle Collider 2D (hoặc Box Collider 2D)
- Is Trigger: ✓ (QUAN TRỌNG!)
- Radius/Size: Đủ lớn để detect player (thử 2-3 units)
```

**C. Add NPCController**
```
Add Component → NPC Controller
```

**D. Add NPCCombat**
```
Add Component → NPC Combat
```

### Bước 3: Configure NPCController

Select `NPC_Smoker`, trong **NPCController** Inspector:

**NPC Data:**
- **Dialogue Data**: Để null (không dùng single dialogue)
- **Mask Dialogue Set**: Drag `Smoker_MaskDialogues`
- **Item Drops**: (Optional - có thể thêm items drop sau dialogue)

**Mask Requirement:**
- **Requires Mask**: ✓ (TICK - quan trọng!)
- **NPC ID**: `"NPC_1"` (unique cho mỗi NPC)

**Interaction Settings:**
- **Player Layer**: Default (hoặc layer của Player)
- **Player Tag**: `"Player"`
- **Interact Key**: F

**References:**
- **Interaction Prompt**: Để null (sẽ tự tìm)
- **NPC Combat**: Auto-assigned (hoặc drag component)

**Item Drop Settings:**
- **Drop Items After Dialogue**: ☐ (không drop items trong level này)

### Bước 4: Configure NPCCombat

Vẫn trong `NPC_Smoker`, **NPCCombat** Inspector:

**Combat Settings:**
- **Attack Damage**: `10`
- **Attack Range**: `1.5`
- **Attack Cooldown**: `1.0`
- **Chase Player**: ✓
- **Chase Speed**: `3.0`

**Detection:**
- **Player Tag**: `"Player"`

### Bước 5: Repeat for NPC 2 & 3

**NPC 2 (Shopkeeper):**
```
GameObject name: "NPC_Shopkeeper"
NPC ID: "NPC_2"
Mask Dialogue Set: Shopkeeper_MaskDialogues
Position: Trước cửa tiệm
(Same components & settings như NPC 1)
```

**NPC 3 (Person on Bench):**
```
GameObject name: "NPC_PersonOnBench"
NPC ID: "NPC_3"
Mask Dialogue Set: PersonOnBench_MaskDialogues
Position: Ghế công viên
(Same components & settings)
```

> ✅ **Checkpoint:** 3 NPCs đã được setup với MaskDialogueSet và NPCCombat

---

## 6. Setup Player

### Bước 1: Verify PlayerController

Select **Player** GameObject, check **PlayerController** component:

**Health System** (đã được thêm):
- **Max HP**: `100`
- **Invulnerability Duration**: `0.5`

**Mask System** (existing):
- **Mask Controller**: Reference should be assigned
- **Mask UI Controller**: (Optional)
- **Mask Visual**: GameObject showing mask

### Bước 2: Verify MaskController

Select **Player**, check **MaskController** component:

**Phải có 3 masks configured:**
```
Masks (List):
  [0] Mask 1 (Honesty)
      - Mask Name: "Honesty Mask"
      - Mask Color: White/Gray
      - Mask ID: 0
      
  [1] Mask 2 (Kindness)
      - Mask Name: "Kindness Mask"
      - Mask Color: Warm Yellow
      - Mask ID: 1
      
  [2] Mask 3 (Indifference)
      - Mask Name: "Indifference Mask"
      - Mask Color: Dark Blue/Purple
      - Mask ID: 2
```

**Mask Visual Renderer:**
- Gán SpriteRenderer của mask visual

### Bước 3: Verify Player Tag

1. Select **Player** GameObject
2. Ở đầu Inspector, check **Tag** dropdown
3. Phải là `"Player"` (nếu chưa, chọn Player từ dropdown)

### Bước 4: Test Controls

**Existing controls (KHÔNG thay đổi):**
- **WASD / Arrow Keys**: Movement
- **Space**: Jump
- **E**: Toggle mask on/off
- **1, 2, 3**: Select mask type
- **F**: Interact with NPC
- **G**: Pickup/drop items (từ previous system)

> ✅ **Checkpoint:** Player có đầy đủ components và controls

---

## 7. Testing Level

### Test 1: Manager Setup

1. **Play** game
2. Check **Console** for:
   ```
   "MaskChoiceTracker: ..." (không có errors)
   "MidnightWalkManager: Level started. Need to interact with 3 NPCs"
   ```
3. ✓ Không có missing reference errors

### Test 2: No Mask → Combat

1. **Play** game
2. **KHÔNG nhấn E** (không đeo mask)
3. Đi lại gần `NPC_Smoker`
4. ✓ "Press F to interact" xuất hiện
5. Nhấn **F**
6. ✓ NPC enters combat mode
7. ✓ NPC di chuyển lại gần player (chase)
8. ✓ NPC attack player
9. ✓ Console: "Player took 10 damage. HP: 90/100"
10. Nhấn **E** để đeo mask
11. Nhấn **1** để chọn mask
12. ✓ NPC stops attacking
13. ✓ Dialogue starts

### Test 3: Honesty Mask → Dialogue Branch 1

1. Reload scene
2. Nhấn **E** (đeo mask)
3. Nhấn **1** (Honesty mask)
4. Tiến đến `NPC_Smoker`, nhấn **F**
5. ✓ Dialogue "Yên tĩnh vì chẳng còn ai muốn nói chuyện" plays
6. ✓ Console: "Recorded HONESTY for NPC_1"
7. Finish dialogue (Space)
8. ✓ Dialogue ends normally

### Test 4: Kindness Mask → Dialogue Branch 2

1. Reload scene
2. **E** + **2** (Kindness mask)
3. Interact with `NPC_Smoker`
4. ✓ Dialogue "Yên tĩnh đôi khi cũng dễ chịu" plays
5. ✓ Console: "Recorded KINDNESS for NPC_1"

### Test 5: Indifference Mask → Dialogue Branch 3

1. Reload scene
2. **E** + **3** (Indifference mask)
3. Interact with `NPC_Smoker`
4. ✓ Dialogue "..." plays (player silent)
5. ✓ Console: "Recorded INDIFFERENCE for NPC_1"

### Test 6: Full Good Ending Playthrough

1. **Start fresh** (reload scene)
2. **NPC 1**: E + 2 (Kindness) → Interact → Finish dialogue
3. **NPC 2**: (Keep Kindness mask) → Interact → Finish
4. **NPC 3**: (Keep Kindness mask) → Interact → Finish
5. ✓ Console: "MidnightWalkManager: NPC interactions: 3/3"
6. ✓ Console: "MidnightWalkManager: Level complete!"
7. ✓ Console: "=== Mask Choice Tracker State ===
   Total choices: 3
   KINDNESS: 3
   Predicted ending: GOOD"
8. ✓ Screen fades to black (2 giây)
9. ✓ Good ending text xuất hiện:
   "Trong đêm vắng,
   chỉ một chút dịu dàng
   cũng đủ làm người ta nhớ."
10. ✓ "Play Again" button xuất hiện

### Test 7: Bad Ending Path

```
NPC 1: Indifference (key 3)
NPC 2: Indifference (key 3)
NPC 3: Honesty (key 1)

Result: Bad ending
"Bạn đi hết con đường.
Nhưng chẳng ai nhớ
bạn đã từng đi qua."
```

### Test 8: Neutral Ending Path

```
NPC 1: Honesty (key 1)
NPC 2: Honesty (key 1)
NPC 3: Kindness (key 2)

Result: Neutral ending
"Bạn đã nói những điều thật.
Nhưng không phải đêm nào
cũng cần sự thật."
```

### Test 9: Replay Functionality

1. Complete any ending
2. ✓ Click "Play Again" button
3. ✓ Scene reloads
4. ✓ Can interact with all NPCs again
5. ✓ Make different choices
6. ✓ Get different ending

---

## 8. Troubleshooting

### Vấn Đề: NPC Không Tấn Công Khi Player Không Đeo Mask

**Nguyên nhân & Giải pháp:**

✗ **Requires Mask = false**
→ NPCController → Set Requires Mask = ✓

✗ **NPCCombat component không có**
→ Add NPCCombat component to NPC GameObject

✗ **Player tag không đúng**
→ Player GameObject → Tag = "Player"

✗ **Collider không phải trigger**
→ NPC Collider2D → Is Trigger = ✓

### Vấn Đề: Dialogue Không Thay Đổi Theo Mask

✗ **MaskDialogueSet chưa gán**
→ NPCController → Drag MaskDialogueSet asset vào

✗ **DialogueData trong MaskDialogueSet null**
→ Open MaskDialogueSet asset → Assign all 3 dialogues

✗ **Player không đeo mask**
→ Nhớ nhấn E để wear mask trước khi interact

✗ **MaskController không có 3 masks**
→ MaskController → Masks list phải có 3 elements

### Vấn Đề: Ending Không Hiển Thị

✗ **EndingUI chưa link to EndingManager**
→ EndingManager → Ending UI field → Drag EndingPanel

✗ **EndingPanel bị disable**
→ EndingUI sẽ tự enable, nhưng check initial state

✗ **MaskChoiceTracker không ghi choices**
→ Check Console for "Recorded [MASK] for [NPC_ID]"

✗ **Required NPC Count sai**
→ MidnightWalkManager → Required NPC Count = 3

### Vấn Đề: Ending Tính Toán Sai

✗ **NPC ID trùng lặp**
→ Mỗi NPC phải có unique ID: "NPC_1", "NPC_2", "NPC_3"

✗ **Choices không được recorded**
→ Check NPCController có call MaskChoiceTracker.RecordChoice()

✗ **Can Repeat = true**
→ DialogueData → Can Repeat = false (để không record multiple lần)

### Vấn Đề: Player Chết Quá Nhanh

✗ **HP quá thấp**
→ PlayerController → Max HP = 100 (hoặc cao hơn)

✗ **Attack Damage quá cao**
→ NPCCombat → Attack Damage = 10 (giảm xuống 5 nếu cần)

✗ **Attack Cooldown quá ngắn**
→ NPCCombat → Attack Cooldown = 1.0 (tăng lên 1.5-2.0)

### Vấn Đề: NPC Chase Quá Nhanh/Chậm

✗ **Chase Speed không phù hợp**
→ NPCCombat → Chase Speed:
  - Quá nhanh: Giảm từ 3.0 → 2.0
  - Quá chậm: Tăng từ 3.0 → 4.0

✗ **Player moveSpeed vs chase speed**
→ Đảm bảo Player moveSpeed > NPC chaseSpeed

---

## 🎯 Quick Checklist

Checklist nhanh để đảm bảo setup đúng:

### Scene Setup
- ✓ MaskChoiceTracker GameObject với component
- ✓ MidnightWalkManager với Required NPC Count = 3
- ✓ EndingManager với EndingUI reference
- ✓ EndingCanvas với EndingPanel + EndingUI component

### Dialogue Data
- ✓ 9 DialogueData assets (3 per NPC × 3 NPCs)
- ✓ 3 MaskDialogueSet assets (1 per NPC)
- ✓ Mỗi MaskDialogueSet có đủ 3 dialogues assigned

### NPC Setup (× 3 NPCs)
- ✓ Collider2D với Is Trigger = ✓
- ✓ NPCController component
  - Requires Mask = ✓
  - Unique NPC ID
  - MaskDialogueSet assigned
- ✓ NPCCombat component
  - Attack settings configured

### Player Setup
- ✓ PlayerController với Max HP = 100
- ✓ MaskController với 3 masks
- ✓ Player Tag = "Player"
- ✓ All controls working (E, 1,2,3, F)

### Testing
- ✓ No mask → NPC attacks
- ✓ With mask → Dialogue plays
- ✓ 3 dialogue variants per NPC
- ✓ Choices recorded in Console
- ✓ Ending displays after 3 NPCs
- ✓ Correct ending based on choices

---

## 📝 Ending Calculation Reference

### Công Thức Tính Ending

```
COUNT(KINDNESS) >= 2       → GOOD ENDING
COUNT(INDIFFERENCE) >= 2   → BAD ENDING
Else                       → NEUTRAL ENDING
```

### Bảng Tất Cả Tổ Hợp (27 possibilities)

| NPC1 | NPC2 | NPC3 | KIND | INDIFF | HONEST | Ending |
|------|------|------|------|--------|--------|--------|
| K | K | K | 3 | 0 | 0 | GOOD |
| K | K | H | 2 | 0 | 1 | GOOD |
| K | K | I | 2 | 1 | 0 | GOOD |
| K | H | K | 2 | 0 | 1 | GOOD |
| H | K | K | 2 | 0 | 1 | GOOD |
| K | I | K | 2 | 1 | 0 | GOOD |
| I | K | K | 2 | 1 | 0 | GOOD |
| I | I | I | 0 | 3 | 0 | BAD |
| I | I | K | 1 | 2 | 0 | BAD |
| I | I | H | 0 | 2 | 1 | BAD |
| I | K | I | 1 | 2 | 0 | BAD |
| K | I | I | 1 | 2 | 0 | BAD |
| I | H | I | 0 | 2 | 1 | BAD |
| H | I | I | 0 | 2 | 1 | BAD |
| H | H | H | 0 | 0 | 3 | NEUTRAL |
| H | H | K | 1 | 0 | 2 | NEUTRAL |
| H | H | I | 0 | 1 | 2 | NEUTRAL |
| ...  | ... | ... | ... | ... | ... | NEUTRAL |

*(Tất cả combinations không phải GOOD hoặc BAD → NEUTRAL)*

**Legend:**
- K = Kindness
- H = Honesty
- I = Indifference

---

## 🌟 Tips & Best Practices

### Dialogue Writing Tips

**Ngắn gọn & Có tác động:**
- Mỗi line dialogue: 1-2 câu
- Tập trung vào emotion & reaction
- Tránh exposition dài

**Good Example:**
```
✓ Player (Honesty): "Yên tĩnh vì chẳng còn ai muốn nói chuyện."
✓ NPC: "..."  [im lặng có tác động]
```

**Bad Example:**
```
✗ Player: "Tôi nghĩ rằng đêm nay yên tĩnh là do nhiều lý do, có thể là vì..."
✗ NPC: "Vâng, tôi hiểu, và tôi cũng nghĩ rằng..."
```

### NPC Placement Tips

**Spacing:**
- Khoảng cách giữa NPCs: 20-30 units
- Cho player thời gian suy nghĩ giữa các encounters

**Environment:**
- NPC 1: Đèn đường (isolating)
- NPC 2: Cửa tiệm (domestic)
- NPC 3: Ghế công viên (reflective)

**Visual Cues:**
- Mỗi NPC có pose/sprite khác nhau
- Lighting nhấn mạnh mood

### Balancing Tips

**HP vs Damage:**
- 100 HP / 10 damage = 10 hits to die
- Với cooldown 1s = ~10 giây để player đeo mask
- Đủ time nhưng có pressure

**Chase Speed:**
- Player speed: 5
- NPC chase: 3
- Player có thể thoát nhưng phải cố gắng

**Attack Range:**
- 1.5 units = gần nhưng không quá gần
- Player có thời gian react

---

## 🎬 Advanced: Tạo Cinematic Ending

### Optional: Camera Zoom for Ending

Thêm vào `EndingUI.cs`:

```csharp
// In EndingSequence() coroutine, sau fade to black:
if (Camera.main != null)
{
    Camera.main.orthographicSize = Mathf.Lerp(
        Camera.main.orthographicSize, 
        3f,  // Zoom in closer
        Time.deltaTime * 2f
    );
}
```

### Optional: Sound Effects

**Thêm AudioSource references:**
```csharp
[SerializeField] private AudioClip fadeSound;
[SerializeField] private AudioClip textAppearSound;

// Play khi appropriate:
AudioSource.PlayOneShot(fadeSound);
```

### Optional: Particle Effects

**Khi ending trigger:**
```csharp
[SerializeField] private ParticleSystem endingParticles;

// In EndingSequence():
if (endingParticles != null)
{
    endingParticles.Play();
}
```

---

*Level Midnight Walk đã sẵn sàng! Narrative-driven experience với meaningful choices và multiple endings hoàn chỉnh.*
