# RP3 — SRS: soi chữ

> Soi **24/09/2026**. Hạn nộp **10h00 ngày 25/09** — còn chưa tới 24 giờ.
> Mục 1 là thứ phải làm **tối nay**. Mục 4 là bảng Find & Replace sửa được cả RP3 lẫn RP7 cùng lúc.
>
> Biên bản hội đồng gọi tên RP3 ⇒ **bắt buộc sửa riêng**, không gộp vào RP7 là xong.

---

## 0. ⚠️ ĐÍNH CHÍNH điều tôi nói hôm 22/09

Hôm 22/09 tôi viết về FE-04:

> *"**§I.6.1.4 FE-04** ghi 'Screening, Proposal Review, Acceptance' là **ĐÚNG**, đừng đụng vào."*

**Sai.** Tôi dựa vào chú thích trong `ReviewRound.cs:14` (`// SCREENING / REVIEW / ACCEPTANCE`) —
**chú thích đó đã cũ**. Code kiểm thật ở `ReviewRoundService.cs:104-105`:

```csharp
if (string.IsNullOrWhiteSpace(request.RoundType) ||
    !new[] { "REVIEW", "ACCEPTANCE" }.Contains(request.RoundType))
    // → ném lỗi
```

FE cũng chỉ có hai giá trị (`constants/statuses.ts`):

```ts
export const REVIEW_ROUND_TYPE = { REVIEW: "REVIEW", ACCEPTANCE: "ACCEPTANCE" } as const;
```

`SCREENING` chỉ còn sống như **nhãn hiển thị** cho dữ liệu cũ (`"SCREENING" => "Sơ loại"` trong
`ReviewScoringService.cs:115`) — **không tạo mới được**.

### Hệ quả

| Chỗ | Trạng thái |
|---|---|
| **RP3 §3.3.6**: *"Only two round types exist: REVIEW and ACCEPTANCE (Screening was removed project-wide)"* | ✅ **ĐÚNG** — giữ nguyên |
| **RP3 Table 6 #13**: *"Two kinds exist: proposal review and acceptance"* | ✅ **ĐÚNG** |
| **RP1 / RP7 §I.6.1.4 FE-04**: *"review rounds (**Screening**, Proposal Review, Acceptance)"* | ❌ **SAI — bỏ chữ `Screening`** |

⇒ Sửa **FE-04** thành: *"Create and manage review rounds (Proposal Review, Acceptance)."*

---

## 1. 🔴 Làm ngay tối nay — 3 việc, dưới 10 phút

### 1.1 Trang bìa còn nguyên placeholder của template

```
– Hanoi, August 2019 –
```

**Sai cả thành phố lẫn năm — lệch 7 năm.** Đây là loại lỗi bị bắt trong 2 giây, và nó nói thẳng
rằng nhóm chưa từng mở trang bìa ra đọc.

→ **`– Ho Chi Minh, September 2026 –`**

⚠️ **Mở luôn bìa RP5 và RP6 kiểm cùng lúc** — nếu RP3 sót thì nhiều khả năng chúng cũng sót.

### 1.2 Record of Changes trống hoàn toàn

Không một dòng nào. Cẩm nang §1.2 xếp đây vào **trang bắt buộc**.

| Date | A/M/D | In charge | Change Description |
|---|---|---|---|
| *(05/2026)* | A | | Initial version: product overview, actors, use cases and functional requirements. |
| **16/08/2026** | M | | Internal review against the FLM template. |
| **24/09/2026** | M | | Removed the conferencing and calendar integrations that are not part of the product; corrected the AI model identifiers; removed the Finance review round, which was dropped from the design. |
| **24/09/2026** | M | | Corrected the description of the rubric template: templates are scoped to a cycle and track rather than versioned. |

Ngày trong ngoặc anh tự điền. Cột `In charge` dùng **đúng 4 tên** nhóm hiện tại.

### 1.3 Mục lục và danh mục bảng chưa bao giờ được cập nhật

**Bằng chứng tự mâu thuẫn ngay trong hai trang đầu:**

| Nguồn | Nói gì |
|---|---|
| **List of Tables** | `Table 7. Business rules… ` **trang 26** · `Table 8. Application messages…` **trang 31** |
| **Table of Contents** | `5. Requirement Appendix` bắt đầu **trang 68** |

Table 7 và Table 8 nằm trong **§5.1** và **§5.3** — tức phải ở sau trang 68. Số trang 26 và 31 là
**của bản cũ**, §3 đã phình từ 9 lên 67 trang mà danh mục không đánh lại.

⇒ **`Ctrl+A` → `F9` chưa từng được chạy trên RP3.**

Đây là mục 17 trong checklist, làm **sau cùng** khi đã sửa hết nội dung. Nhưng phải biết là nó
đang sai, đừng tưởng số trang đúng.

---

## 2. 🔴 RP3 tự mâu thuẫn về RubricTemplate

Hai chỗ trong **cùng một tài liệu**, cách nhau khoảng 10 trang:

| Chỗ | Nói gì | |
|---|---|---|
| **Table 6 #17** | *"A **versioned** set of scoring criteria. The version active when a project is created stays **pinned** to it."* | ❌ **SAI** |
| **§3.3.6** Business Rules | *"Templates are assigned to (cycle, track) **scopes**. A review round uses the template associated with its cycle and track."* | ✅ **ĐÚNG** |

**Bằng chứng:** `grep Version FURPMS.Domain/Entities/Review/RubricTemplate.cs` → **0 kết quả**.
Trường thật: `Id, TemplateType, Name, MaxTotalScore, TrackId, OrderId, IsActive, AppliesBasic,
AppliesApplied`. Phạm vi do bảng nối `RubricTemplateScope` (`TemplateId`, `CycleId`, `TrackId`)
đảm nhiệm — đúng như §3.3.6 mô tả.

**Sửa Table 6 #17 thành:**

> RubricTemplate — A set of scoring criteria **scoped** to a research type, track or cycle rather
> than versioned. A review round uses the template associated with its cycle and track.

> Giữ §3.3.6 nguyên — nó viết đúng.

---

## 3. ✅ Tin tốt — hai mục tôi tưởng bịa nhưng có thật

| Mục | Kiểm chứng |
|---|---|
| **§3.2.2 Forgot Password** | `AuthController.cs:43` `[HttpPost("forgot-password")]` ✅ · FE `auth.service.ts:8` ✅ |
| **§3.2.3 Reset Password** | `AuthController.cs:53` `[HttpPost("reset-password")]` ✅ · FE `auth.service.ts:12` ✅ |

**Giữ nguyên cả hai.** Phần §3.2.x nói chung viết rất sát code thật (Zustand `authStore`,
`RoleGuard`, `NAV_ITEMS`, khoá localStorage `furpms-access-token`) — đây là phần mạnh nhất của RP3,
đừng đụng vào.

---

## 4. 🔴 Bảng Find & Replace — sửa RP3 và RP7 cùng lúc

> **Đây là thứ đáng làm nhất khi còn dưới 24 giờ.** Mở `Ctrl+H`, chạy lần lượt ở **cả hai file**.
> Mỗi dòng mất vài giây, và nó bắt được cả những chỗ tôi chưa liệt kê.

| # | Tìm | Thay bằng | Ghi chú |
|---|---|---|---|
| 1 | `gemini-2.0-flash` | `gemini-3.5-flash-lite` | Table 5 #1 |
| 2 | `text-embedding-004` | `gemini-embedding-001` | Table 5 #3, và RP7 Table 1 WBS 5.3 |
| 3 | `Hanoi, August 2019` | `Ho Chi Minh, September 2026` | Chỉ RP3 |
| 4 | `Satff` | `Staff` | §3.3.1 |
| 5 | `ctors/Roles` | `Actors/Roles` | §3.3.4 — mất chữ `A` |
| 6 | `reports, and expert profiles` | `submitted proposals` | §1, UC-24 |
| 7 | `proposals/reports/experts` | `submitted proposals` | Table 3 #23, Table 5 #3 |
| 8 | `Screening, Proposal Review, Acceptance` | `Proposal Review, Acceptance` | RP7 FE-04 — xem mục 0 |

### Sau đó `Ctrl+F` tìm và xoá/sửa tay (không replace máy được)

| Tìm | Làm gì |
|---|---|
| `Teams` | Xoá mọi chỗ — §1 đoạn 2 · UC-17 · Table 3 #14 · Table 5 #4 |
| `Outlook` | Xoá — §1 đoạn 2 · UC-18 · Table 5 #5 |
| `Calendar` | Xoá — **UC-18 xoá cả use case** (nhớ xoá khỏi **Figure 2**) · Table 5 #5 |
| `Finance` | UC-19 và Table 5 #9 — xem mục 5 |
| `versioned` | Table 6 #17 — xem mục 2 |

---

## 5. 🔴 Vòng FINANCE đã bị bỏ khỏi code

| Chỗ | Câu |
|---|---|
| **UC-19** | *"Manage review rounds — Open **Science** round, then **Finance** round after Science passes."* |
| **Table 5 #9** | `OpenFinanceRoundOnSciencePass — Opens the Finance review round after the Science round result is PASSED.` |

`ReviewRoundService.cs:99` chặn thẳng mọi `Dimension` khác `SCIENCE`, và chú thích dòng 128 ghi
nguyên văn: *"FINANCE bỏ rồi nên nhánh đó thành code chết, không chặn được gì."*

**Sửa:**
- **UC-19** → *"Manage review rounds — open a review round for a research field, with an optional prerequisite round that must pass first."*
- **Table 5 #9** → **xoá dòng**, hoặc đổi thành `OpenRoundOnPrerequisitePass`

⚠️ Kiểm **Figure 2 (Use Case Diagram)** xem có vẽ UC-19 kèm Finance không.

---

## 6. 🟠 Những chỗ nhỏ hơn

| # | Chỗ | Vấn đề |
|---|---|---|
| 1 | **§3.2.3** | Còn sót dấu backtick Markdown: ``` `/reset-password?token=<token>` ``` và ``` ` {token, newPassword}’ ``` — dấu nháy cuối là **nháy cong `’`** chứ không phải `}`. Sửa thành `{token, newPassword}`, bỏ backtick |
| 2 | **Table 6** | Bốn tên entity không có trong DB: `ResearchField` → **`ResearchTrack`/`CycleTrack`** · `Meeting` → **`CouncilMeeting`** · `Disbursement` → **`ContractDisbursement`** · `Evaluation` → **`ProposalReviewScore`/`ReviewScoreDetail`**. Đối chiếu với **Figure 7** ngay phía trên |
| 3 | **§1** đoạn 1 | *"is a web and **mobile** application"* — mobile chưa quyết. Xem `RP7_Soi_Chu.md` nhóm 11 |
| 4 | **Table 1** (Actors) #3, #4 | *"Access is available on both web and **mobile**"* — như trên |
| 5 | **§1** | gọi *"regulation **QĐ 543/QĐ-ĐHFPT**"*, nơi khác gọi *"Decision 543/QĐ-ĐHFPT"* — thống nhất một cách |
| 6 | **Table 4** | `Meeting Scheduler` có `Staff X` và `RC X`, Admin trống — Admin thường xem được mọi màn, kiểm lại |
| 7 | **§3.3.3** | Actors ghi *"Staff, Administrator"* nhưng Table 4 xếp mục Cycle/Funding là Admin-only — kiểm lại cho khớp |

---

## 7. Kế hoạch còn dưới 24 giờ

### Tối 24/09 — khoảng 1 giờ 15

| Việc | File | Ước lượng |
|---|---|---|
| Sửa trang bìa RP3, kiểm luôn bìa RP5/RP6 | RP3 | 5 phút |
| Chạy 8 dòng Find & Replace | **RP3 + RP7** | 15 phút |
| Xoá Teams / Outlook / Calendar (5 chỗ mỗi file) | **RP3 + RP7** | 20 phút |
| Xoá UC-18, sửa UC-17, sửa UC-19, sửa Table 5 #9 | **RP3 + RP7** | 15 phút |
| Sửa Table 6 #17 (RubricTemplate) | **RP3 + RP7** | 5 phút |
| Điền Record of Changes RP3 | RP3 | 10 phút |
| Xoá `PWM` / `AWS` khỏi bảng acronym RP7 | RP7 | 3 phút |
| Bỏ cột Chinh ở RP7 Table 8, sửa Table 1 / Table 6 / Table 9 | RP7 | 20 phút |

> Nếu chỉ làm được **ba việc**: trang bìa RP3 · 8 dòng Find & Replace · bỏ Chinh khỏi RP7.
> Ba cái đó gỡ những lỗi bị bắt nhanh nhất.

### Sáng 25/09 — trước 10h, khoảng 40 phút

1. `Ctrl+A` → `F9` trên **cả RP3 và RP7**, rồi kiểm ngày trang bìa không bị nhảy
2. `Ctrl+F` chạy 12 dòng kiểm cuối ở `RP7_Soi_Chu.md`, cộng thêm `Hanoi` và `2019`
3. Đọc **print preview** một lượt — ít nhất là trang bìa, mục lục, và các trang có bảng
4. **Nộp sớm.** Đừng để sát 10h00

### Chấp nhận bỏ qua

Với thời gian còn lại, **đừng** động vào: Table 6 đổi tên 4 entity · quyết định mobile ·
*"dozens to hundreds"* · đo code coverage · Table 1 WBS tính lại tổng · RP1/RP2/RP6 riêng lẻ.

Đó là những lỗi **cần đọc kỹ mới thấy**. Ưu tiên nhóm lỗi **nhìn phát thấy ngay** — trang bìa sai
năm, tên model chọi nhau, thành viên đã rời nhóm vẫn nằm trong bảng phân công.

---

## 8. Soi lại lần 2 — 25/09 (ngày nộp)

### ✅ Đã sửa
Trang bìa · Record of Changes · ToC/LoT đã F9 (Table 7 → trang 68, khớp §5) · UC-19 · xoá Table 5 dòng FINANCE ·
Table 6 đổi tên 4 entity · Table 6 #17 RubricTemplate · `Satff` · `ctors` · Table 3 #23.

### ❌ Còn sót — và Record of Changes đang KHẲNG ĐỊNH là đã sửa

Dòng 24/09 ghi *"Removed the conferencing and calendar integrations… corrected the AI model identifiers"*.
Nhưng các chỗ dưới vẫn còn ⇒ **nhật ký thay đổi nói sai về chính tài liệu**. Tệ hơn để trống.

| Chỗ | Hiện tại | Sửa thành |
|---|---|---|
| §1 đoạn 2 | *"semantic search across proposals, submitted proposals"* (Find&Replace ghép hỏng) | *"semantic search across submitted proposals"* |
| §1 đoạn 2 | *"It integrates with Google Meet for online committee meetings and Google Calendar / Microsoft Outlook for scheduling."* | *"The system records whether each committee meeting is held in person or online, together with the meeting link supplied by the organiser."* |
| UC-17 | *"Create Google Meet and notify participants."* | *"Record the time, place or meeting link, and notify participants."* |
| UC-18 | *"Sync calendar"* | **xoá cả dòng** (và khỏi Figure 2 nếu có) |
| UC-24 | *"Search proposals, reports, and expert profiles semantically."* | *"Search submitted proposals semantically."* |
| Table 3 #14 | *"Create Meet meeting; sync calendar."* | *"Set time, place or meeting link; assign presentation slots."* |
| Table 5 #1 | `gemini-2.0-flash` | `gemini-3.5-flash-lite` |
| Table 5 #3 | *"Generates text-embedding-004 vectors and indexes proposals/reports/experts"* | *"Generates gemini-embedding-001 vectors and indexes submitted proposals"* |
| Table 5 #4 `CreateOnlineMeeting` | | **xoá dòng** |
| Table 5 #5 `SyncCalendarEvent` | | **xoá dòng** |
| Table 6 #17 | *"RubricTemplate - A set of…"* (lặp tên) | bỏ *"RubricTemplate - "* ở đầu mô tả |
| §3.2.3 Function Trigger | *"…which navigates {token, newPassword}."* (dán nhầm) | *"…which navigates to /reset-password?token=<token>."* |
| §3.2.3 Function Description | `` `token` ``, `` `POST /api/auth/reset-password` ``, `` ` {token, newPassword}’ `` | bỏ hết backtick; `{token, newPassword}` |
| Bìa | `September 2026–` | `September 2026 –` (thiếu khoảng trắng) |

### ⚠️ Mục lục nhảy số
`3.4 → 3.6` (không có 3.5) · `3.8 → 3.10` (không có 3.9). Và `3.10.3 Receive System Notifications` là mục
cấp 3 duy nhất hiện trong ToC. Nếu đã xoá mục 3.5/3.9 thì **đánh lại số heading** (3.6→3.5…), rồi F9.
