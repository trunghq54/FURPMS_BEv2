# RP7 bản gộp — soi chữ

> Soi ngày 22/09/2026 trên bản bạn gộp. Hạn nộp **10h00 ngày 25/09**.
> Mọi kết luận đều verify bằng grep code thật, không đoán.
>
> **Điều đã lường trước đã xảy ra:** gộp xong thì mâu thuẫn giữa các chương nằm cạnh nhau.
> Phần lớn lỗi dưới đây **bắt được mà không cần mở repo** — chỉ cần đọc RP7 từ đầu đến cuối.

---

## 0. Sửa ở RP7 thôi hay sửa cả RP1–6?

**Biên bản hội đồng đã trả lời sẵn:**

> *"Chỉnh sửa lại toàn bộ document theo góp ý — **đặc biệt report 3, 4 và 5**"*

Hội đồng **gọi tên ba báo cáo cụ thể**. Nộp RP7 sạch mà RP3/RP4/RP5 còn nguyên lỗi chính là
gạch §0 nguyên nhân #5 — *"không tiếp thu góp ý của các lần Review"*, mức **Cao**.

### Quy tắc

| Báo cáo | Sửa riêng? | Vì sao |
|---|---|---|
| **RP3** | ✅ **Bắt buộc** | Biên bản gọi tên |
| **RP4** | ✅ **Bắt buộc** | Biên bản gọi tên — *đã sửa xong rồi* |
| **RP5** | ✅ **Bắt buộc** | Biên bản gọi tên |
| **RP7** | ✅ **Bắt buộc** | Là bài nộp chính, và là nơi mâu thuẫn lộ ra |
| RP1 · RP2 · RP6 | ⬜ **Chỉ nếu còn giờ** | Không được gọi tên. Sửa trong RP7 là đủ cho phần được chấm |

> **Lưu ý về cách nghĩ:** không sửa RP2 **không tạo ra** mâu thuẫn mới — RP2 vốn đã sai sẵn
> (ghi `MS SQL Server`). Sửa RP7 chỉ là không vá chỗ cũ, chứ không làm hỏng thêm gì.

### Thứ tự thao tác — để không sửa hai lần

RP7 **đã gộp xong rồi**, nên chiều chép phải **ngược lại**:

```
1. Sửa trong RP7 trước          ← mâu thuẫn nằm cạnh nhau, nhìn là thấy, sửa xong verify ngay
2. Chép NGƯỢC đoạn đã sửa từ RP7 sang RP3 / RP5
3. Đừng sửa RP3/RP5 trước rồi dán lại vào RP7 — sẽ đè mất các sửa đổi chỉ-có-ở-RP7
```

Chép ngược chỉ mất công với **đoạn văn và ô bảng**, không phải cả chương. Phần lớn lỗi dưới đây
là **một ô bảng hoặc một câu** — chép lại mất vài giây mỗi chỗ.

⚠️ Khi chép ngược, **đừng chép caption** (`Table 5`, `Figure 2`…). Số hiệu trong RP7 chạy liên
tục toàn tài liệu, còn trong RP3/RP5 đánh riêng theo báo cáo — chép sang là sai số ngay.

### Việc BẮT BUỘC làm ở mọi báo cáo được nộp

| Việc | Vì sao không bỏ qua được |
|---|---|
| **Danh sách nhóm 4 người** | Đây là sự thật hành chính, không phải lỗi kỹ thuật. Báo cáo liệt kê người đã rời nhóm là chuyện khác hẳn |
| **Record of Changes** | Mỗi báo cáo có bảng riêng, không dùng chung được |

### ⚠️ Phải xác minh trước khi quyết

**Ngày 25/09 nộp những gì — chỉ RP7, hay cả bộ RP1–7?** Mở cổng nộp trên FLM xem có mấy ô tải
lên. Đây là việc 2 phút nhưng nó quyết định toàn bộ khối lượng còn lại.

---

## 🔴 NHÓM 1 — Google Meet / Calendar quay lại, mâu thuẫn với chính LI-06

RP1 đã sửa sạch. Nhưng bản gộp **có 5 chỗ khác** vẫn còn, và giờ chúng nằm cùng file với LI-06.

> **LI-06 (§I.6.2):** *"FURPMS **does not provide video conferencing**… it **does not create or manage
> the meeting** on the conferencing platform."*

| # | Chỗ | Câu | Sửa |
|---|---|---|---|
| 1 | **§II.6.3 Table 11** | `Meeting Integration \| Google Meet API` | **XOÁ CẢ DÒNG** |
| 2 | **§II.6.3 Table 11** | `Calendar Sync \| Google Calendar API` | **XOÁ CẢ DÒNG** |
| 3 | **§III.1** đoạn 2 | *"It integrates with **Google Meet / Microsoft Teams** for online committee meetings and **Google Calendar / Microsoft Outlook** for scheduling."* | **XOÁ CẢ CÂU.** Thay: *"The system records whether each committee meeting is held in person or online, together with the meeting link supplied by the organiser."* |
| 4 | **§III.2.2 Table 2 — UC-17** | *"Schedule committee meeting — **Create Google Meet / Teams meeting** and notify participants."* | → *"Schedule committee meeting — record the time, place or meeting link, and notify participants."* |
| 5 | **§III.2.2 Table 2 — UC-18** | *"**Sync calendar** — Sync meetings to Google Calendar / Outlook."* | **XOÁ CẢ USE CASE.** ⚠️ Nhớ xoá luôn khỏi **Figure 2 (Use Case Diagram)** |
| 6 | **§III.3.1.2 Table 3** dòng 14 | *"Meeting Scheduler (pop-up) — Create Meet/Teams meeting; **sync calendar**"* | → *"Meeting Scheduler (pop-up) — set time, place or meeting link, assign presentation slots"* |
| 7 | **§III.3.1.4 Table 5** dòng 4 | `CreateOnlineMeeting — Creates a Google Meet / Teams meeting via API` | **XOÁ DÒNG** |
| 8 | **§III.3.1.4 Table 5** dòng 5 | `SyncCalendarEvent — Pushes meeting events to Google Calendar / Outlook` | **XOÁ DÒNG** |
| 9 | **§II.2.3 Table 6** | dòng đào tạo *"Google Meet / MS Teams API"* | **XOÁ DÒNG** |

**Bằng chứng:** grep `microsoft.teams` / `msteams` / `outlook` / `google.calendar` toàn bộ `.cs` `.ts`
`.tsx` → chỉ trúng file ngôn ngữ, **0 file logic**. `CouncilMeeting.Platform` chỉ có `IN_PERSON` và
`ONLINE`; `CouncilMeetingService.cs:158-159` map mọi giá trị khác về `Online`.

> Riêng UC-18 nguy hiểm nhất: nó là **cả một use case** cho tính năng không tồn tại, nên nó có
> mặt trong sơ đồ use case, trong bảng phân quyền, và có thể cả trong test case của RP5.

---

## 🔴 NHÓM 2 — Hai bảng trong cùng tài liệu ghi tên model khác nhau

| Bảng | Ghi gì | Đúng chưa |
|---|---|---|
| **§II.6.3 Table 11** | `gemini-3.5-flash-lite` (fallback `gemini-3.1-flash-lite`) | ✅ đúng |
| **§II.6.3 Table 11** | `gemini-embedding-001` | ✅ đúng |
| **§III.3.1.4 Table 5** dòng 1 | *"Calls Gemini (**gemini-2.0-flash**)"* | ❌ **sai** |
| **§III.3.1.4 Table 5** dòng 3 | *"Generates **text-embedding-004** vectors"* | ❌ **sai** |
| **§II.1.1 Table 1** dòng 5.3 | *"Semantic search — Gemini Embeddings (**text-embedding-004**)"* | ❌ **sai** |

Anh đã sửa Table 11 nhưng **ba chỗ khác còn nguyên tên model cũ**. Hai tên model khác nhau cho
cùng một thứ, cách nhau ~70 trang trong cùng một file — ai đọc hết cũng thấy.

**Nguồn sự thật:** `appsettings.Development.json:25-27`.

Sửa cả ba thành `gemini-3.5-flash-lite` và `gemini-embedding-001`.

---

## 🔴 NHÓM 3 — Nhóm 4 người hay 5 người?

**Trang bìa liệt kê 4 người.** Nhưng bên trong:

| Chỗ | Tình trạng |
|---|---|
| **§I.1.2 Table 1** | **7 dòng**, vẫn có `Nguyễn Lê Hoàng Chinh` |
| **§II.4 Table 8** | **5 cột**, vẫn có cột `ChinhNLH (BE)` với **5 ô `D`** |
| **§II.2.3 Table 6** | Chinh xuất hiện **2 lần** (Gemini API, Meet/Teams API) |
| **§II.5 Table 9** | *"All **5** members"* |

⚠️ **Và đây là chỗ đau nhất:**

> **§II.1.3 Table 4, rủi ro #3:** *"**Triggered.** One member left the project; the responsibility
> matrix **in Section 4 was redistributed across the remaining four members**."*

Bảng rủi ro **tự khẳng định** Section 4 đã chia lại cho 4 người — rồi **hai trang sau**, Section 4
vẫn là 5 cột có Chinh. Tài liệu tự tố cáo chính nó. Đây là loại mâu thuẫn không cần biết gì về
dự án cũng bắt được.

**Phải sửa đồng bộ 5 chỗ:** bìa (đã đúng) · §I.1.2 Table 1 · §II.2.3 Table 6 · §II.4 Table 8 ·
§II.5 Table 9. Xem `RP1_Sua_Chu.md` mục 10 và `RP2_Sua_Chu.md` mục 2.

---

## 🔴 NHÓM 4 — Table 1 (WBS) sai số học ngay trên mặt bảng

```
4    Online Committee Meeting Integration          28     ← tổng nhóm
4.1  Auto-send invitations (link, agenda, docs)     7     ← chỉ còn MỘT mục con
```

Anh xoá hai mục con (`4.1 Google Meet/Teams API` 12 và `4.3 Calendar sync` 9) nhưng **để nguyên
tổng nhóm 28**. Cộng lại chỉ ra **7**. Thiếu **21 man-day không có mục nào.**

Và **`Total Estimated Effort` vẫn là 206** — chưa trừ.

### Phải sửa

1. Mục 4: đổi tên thành **`Council Meeting Scheduling & Invitations`**, tổng **7**
2. **Thêm mục 10** cho 6 nhóm tính năng làm sau hội đồng kín (xem `RP2_Sua_Chu.md` mục 4) — hiện
   RP7 **không có dòng WBS nào** cho ngân sách / mốc thời gian / lưu quyết định / rà trùng /
   cảnh báo điểm lệch / chuyên môn người chấm, dù §I.6.1 liệt kê đủ 11 mục FE mới
3. Tính lại tổng: 206 − 21 + 46 ≈ **231**
4. **Sửa luôn Table 3** — mọi `%` và `man-days` ở đó tính trên 206

> Không sửa thì thành: §I hứa 11 tính năng mới, §II không ước lượng một ngày công nào cho chúng.

---

## 🔴 NHÓM 5 — Vòng SCIENCE → FINANCE đã bị bỏ khỏi code

Đây là lỗi **tôi chưa từng phát hiện** ở các báo cáo trước.

| Chỗ | Câu |
|---|---|
| **§III.2.2 UC-19** | *"Manage review rounds — Open **Science** round, then **Finance** round after Science passes."* |
| **§III.3.1.4 Table 5** dòng 9 | `OpenFinanceRoundOnSciencePass — Opens the Finance review round after the Science round result is PASSED.` |

**Code thật:** `ReviewRoundService.cs:99` chặn thẳng —

```csharp
if (request.Dimension != ReviewRoundDimension.Science) // ném lỗi
```

Và chú thích ngay dưới, dòng 128, ghi nguyên văn:

> `// Dimension == FINANCE — FINANCE bỏ rồi nên nhánh đó thành code chết, không chặn được gì.`

⇒ **Không tạo được vòng FINANCE.** Hai mục trên mô tả một quy trình đã bị gỡ.

### Sửa

- **UC-19** → *"Manage review rounds — open a review round for a research field, with an optional prerequisite round that must pass first."*
- **Table 5 dòng 9** → **xoá**, hoặc đổi thành `OpenRoundOnPrerequisitePass`

⚠️ **Kiểm luôn Figure 2 (Use Case Diagram)** và RP5 xem có test case nào cho vòng FINANCE không.

> ⚠️ **ĐÍNH CHÍNH 24/09:** đoạn này bản trước viết *"FE-04 ghi 'Screening, Proposal Review,
> Acceptance' là ĐÚNG, đừng đụng vào"* — **sai**. Tôi dựa vào chú thích cũ trong
> `ReviewRound.cs:14`. Code kiểm thật ở `ReviewRoundService.cs:104-105` chỉ nhận
> `{"REVIEW", "ACCEPTANCE"}`, và FE cũng chỉ có hai giá trị đó. `SCREENING` chỉ còn là
> **nhãn hiển thị** cho dữ liệu cũ.
>
> ⇒ **§I.6.1.4 FE-04 phải bỏ chữ `Screening`** → *"Create and manage review rounds (Proposal
> Review, Acceptance)."* Xem `RP3_Sua_Chu.md` mục 0.

---

## 🔴 NHÓM 6 — `RubricTemplate` lại bị mô tả là "versioned"

> **§III.3.1.5 Table 6, dòng 17:** *"RubricTemplate — A **versioned** set of scoring criteria.
> The version active when a project is created stays **pinned** to it."*

Đây **đúng cái lỗi đã sửa trong RP4**, giờ quay lại qua chương RP3.

**Bằng chứng:** `grep Version FURPMS.Domain/Entities/Review/RubricTemplate.cs` → **0 kết quả.**
Các trường thật: `Id, TemplateType, Name, MaxTotalScore, TrackId, OrderId, IsActive, AppliesBasic,
AppliesApplied`. Việc giới hạn phạm vi do bảng nối `RubricTemplateScope` (`TemplateId`, `CycleId`,
`TrackId`) đảm nhiệm.

**Sửa thành:**

> RubricTemplate — A set of scoring criteria **scoped** to a research type, track or cycle rather
> than versioned. A round references the template that applies to it.

---

## 🟠 NHÓM 7 — Mục lục hỏng nặng

```
I. Project Introduction      1
  1. Overview                1
  2. Product Background      2      ← thiếu 3, 4, 5, 6
  1. Overview                8      ← "II. Project Management Plan" BIẾN MẤT
  3. Project Deliverables   13      ← thiếu "2. Management Approach"
...
IV. Software Design Description  89
  3. Detailed Design            98
  1. Scope of Testing          124  ← "V. Software Testing Documentation" BIẾN MẤT
...
                                    ← "VII. Appendix" BIẾN MẤT hoàn toàn
```

| Thiếu | Nguyên nhân gần như chắc chắn |
|---|---|
| §I mục 3, 4, 5, 6 | Heading bị đặt sai cấp (không phải Heading 2) |
| **`II. Project Management Plan`** | Heading bị đặt sai cấp (không phải Heading 1) |
| §II mục 2 | như trên |
| **`V. Software Testing Documentation`** | như trên |
| **`VII. Appendix`** | Hoặc bị xoá hẳn khỏi thân bài, hoặc sai cấp |

**Cách sửa:** mở `Xem → Ngăn Dẫn hướng`, nhìn cây heading. Chỗ nào không thụt đúng bậc thì gán lại
style `Heading 1` / `Heading 2`. **Rồi mới** `Ctrl+A` → `F9`.

Đừng gõ tay vào mục lục — nó là field, `F9` sẽ xoá hết.

### Và số trang đang lẫn hệ

Acknowledgement `3`, Definition `4`, rồi §I lại bắt đầu từ `1`. Front matter phải là **La Mã**
(`i, ii, iii`) còn thân bài mới là `1, 2, 3` — cần một **ngắt phần (section break)** giữa hai
vùng rồi đặt lại định dạng số trang cho từng vùng.

---

## 🟠 NHÓM 8 — Số hiệu bảng và hình lặp lại giữa các chương

| Số hiệu | Xuất hiện ở |
|---|---|
| **Table 1** | §I.1.2 (nhóm) · §II.1.1 (WBS) · §III.2.1 (Actors) — **3 lần** |
| **Table 2** | §I.6.2 (Limitations) · §II.1.2 (Testing) · §III.2.2 (Use cases) — **3 lần** |
| **Figure 1** | §I.6.1.5 · §II.2.1 · §III.1 — **3 lần** |
| **Figure 2** | §II.3 · §III.2.2 — **2 lần** |

Nói *"xem Table 1"* trong RP7 không xác định được gì. Cẩm nang §1.6.

**Nguyên nhân:** hoặc caption được gõ tay (không phải field `SEQ`), hoặc chưa chạy `F9`.

**Kiểm nhanh:** bấm chuột vào chữ `Table 1` — nếu **cả cụm chuyển nền xám** thì đó là field thật,
`Ctrl+A` → `F9` sẽ tự đánh lại liên tục. Nếu **không xám** thì là chữ gõ tay, phải thay bằng
`Tham khảo → Chèn Chú thích`.

⚠️ Và mọi chỗ **nhắc tới** bảng/hình trong câu văn cũng phải là cross-reference, không gõ tay.

---

## 🟠 NHÓM 9 — Semantic search: phạm vi hứa rộng hơn thực tế

`EmbeddingWorker` **chỉ vector hoá đề cương đã nộp**. Báo cáo, hồ sơ chuyên gia, đơn đặt hàng
nghiên cứu **đều chưa có vector**.

| Chỗ | Câu | Sửa |
|---|---|---|
| **§I.5** đoạn 2 | *"semantic search across proposals, **reports, and expert profiles**"* | → *"semantic search across submitted proposals"* |
| **§I.6.1.2 FE-14** | *"Search **research orders** and prior projects semantically"* | → *"Search previously submitted proposals semantically, ranked by meaning rather than keywords"* |
| **§III.1** đoạn 2 | *"semantic search across proposals, **reports, and expert profiles**"* | → như §I.5 |
| **§III.2.2 UC-24** | *"Search proposals, **reports, and expert profiles** semantically"* | → *"Search submitted proposals semantically"* |
| **§III.3.1.4 Table 5** dòng 3 | *"indexes **proposals/reports/experts**"* | → *"indexes submitted proposals"* |
| **§III.3.1.2 Table 3** dòng 23 | *"Semantic Search — Search proposals/**reports**/**experts**"* | → *"Search submitted proposals"* |

---

## 🟠 NHÓM 10 — Table 6 (Entities) dùng tên không có trong DB

Table 6 nằm **ngay dưới Figure 7 (ERD)**. Nếu Figure 7 là ảnh chụp schema thật thì tên trong bảng
phải khớp với tên trong hình. Bốn dòng đang lệch:

| Table 6 ghi | Thật trong code |
|---|---|
| `ResearchField` | **`ResearchTrack`** (danh mục) + **`CycleTrack`** (mở theo đợt) |
| `Meeting` | **`CouncilMeeting`** |
| `Disbursement` | **`ContractDisbursement`** |
| `Evaluation` | **`ProposalReviewScore`** + **`ReviewScoreDetail`** (và `AcceptanceEvaluation` cho vòng nghiệm thu) |

`RA_SOAT_BAO_CAO_RP1-7.md` đã ghi lỗi này từ 16/08 cho `erd-conceptual.mmd` (`RESEARCH_FIELD` vs
`cycle_track`). Nó theo sang RP3 rồi vào RP7.

**Mở Figure 7 ra đối chiếu từng dòng** — đây đúng gạch §2.6 *"các sơ đồ không nhất quán với nhau
và với tài liệu"*.

---

## 🟠 NHÓM 11 — Mobile: hứa 6 chỗ, không có một ngày công nào

| Chỗ | Nói gì |
|---|---|
| §I.1.1 | *"Software type: Web Application, **Mobile Application**"* |
| §I.5 đoạn 1 | *"is a **web and mobile** application"* |
| §I.6.2 **LI-03** | *"The **mobile application** will be developed for Faculty (PIs) and Review Committee roles only"* |
| §II.6.3 Table 11 | *"React Native (Mobile app)"* |
| §II.2.3 Table 6 | đào tạo *"React Native — Đàm Mạnh Dũng"* |
| §III.1 | *"is a **web and mobile** application"* |
| §III.2.1 Table 1 | PI và Review Committee: *"Access is available on both web and mobile."* |

**Nhưng:** §II.1.1 Table 1 (WBS) **không có mục mobile nào** trong 206 man-day, và §II.4 Table 8
(RAM) **không có dòng mobile nào**. Học React Native, khai React Native, hứa app mobile — mà
không ước lượng công, không phân công ai làm.

**Phải quyết một lần:** còn mobile thì thêm dòng vào WBS + RAM và **demo được**; không còn thì
sửa cả 7 chỗ trên. Cẩm nang §0 xếp *"không demo được hết luồng chính"* ở mức **Rất cao**.

---

## 🟠 NHÓM 12 — Mâu thuẫn và sai sót lẻ

| # | Chỗ | Vấn đề | Sửa |
|---|---|---|---|
| 1 | §II.2.1 vs §II.5 Table 9 | *"**Friday**: Supervisor meeting"* vs *"Every **wednesday**, 60 min"* | Chọn một, viết hoa `Wednesday` |
| 2 | §II.6.2 Table 10 | `main — Production-ready code only` | Repo BE dùng **`master`**, Railway deploy từ đó. Repo FE mới dùng `main` — ghi rõ hai repo khác nhau |
| 3 | **Definition and Acronyms** | `PWM — Psychology website`, `AWS — Amazon Web Services` | **XOÁ.** `PWM` là của dự án khác; FURPMS dùng Railway/Vercel/Cloudinary chứ không AWS. Xem `RP7_Phu_Luc_De_Danh.md` §1 cho bảng đã soạn |
| 4 | §I.6.1.2 **FE-08** | *"AI **Similarity** Check … if matching score is **low**"* | Trùng tên với rà trùng (cảnh báo khi điểm **cao**) → đổi thành **`AI Order Alignment Check`** |
| 5 | §II.1.2 Table 2 | *"≥ 70% code coverage"* | CI (`ci.yml`) chạy `dotnet test` trần, **không** `--collect:"XPlat Code Coverage"` ⇒ chưa ai đo. Đo thật (15 phút) rồi ghi số thật, hoặc đổi chỉ tiêu |
| 6 | §II.1.2 Table 2 | *"Integration Test — Postman + GitHub Actions, 100% API endpoints"* | CI không chạy Postman. 51 controller mà phủ 100% là khó tin — có collection thật không? |
| 7 | §II.2.1 Table 5 | Phase 6 kết thúc **06/09**, kèm *"Defense"* | Hôm nay 22/09, hạn nộp 25/09 ⇒ kế hoạch kết thúc **trước hiện tại**. Thêm **Phase 7** kéo tới 25/09 (xem `RP2_Sua_Chu.md` mục 3) |
| 8 | §II.3 Table 7 | Deliverable cuối due **06/09/2026** | Thêm 2 dòng: Software Package 4 và Revised Reports 1–7 |
| 9 | §I.4 | *"dozens to hundreds of research projects each academic year"* | §3.3 đòi con số giải thích được nguồn. Lấy số thật từ Phòng QLKH, hoặc bỏ con số |
| 10 | Acknowledgement dòng cuối | *"Ho Chi Minh City, September 2026 GSU26SE70"* dính liền một dòng | Tách hai dòng |
| 11 | §III.1 | gọi *"regulation **QĐ 543/QĐ-ĐHFPT**"* | Nơi khác gọi *"Decision 543/QĐ-ĐHFPT"*. Thống nhất một cách gọi trong cả tài liệu |
| 12 | §III.3.1.3 Table 4 | `Meeting Scheduler` có `Staff X` và `RC X` nhưng Admin trống | Admin thường xem được mọi màn — kiểm lại |

---

## Thứ tự làm — còn ~2 ngày rưỡi

### Đợt 1 — tối 22/09, khoảng 1 giờ, gỡ hết lỗi 🔴 rẻ nhất

| Việc | Ước lượng |
|---|---|
| Xoá 2 dòng Table 11 (Meeting Integration, Calendar Sync) | 2 phút |
| Xoá `PWM` và `AWS` khỏi bảng acronym, dán bảng đã soạn sẵn | 5 phút |
| Sửa 3 chỗ tên model → `gemini-3.5-flash-lite` / `gemini-embedding-001` | 5 phút |
| Xoá câu Meet/Teams/Calendar ở §III.1 | 3 phút |
| Xoá UC-18, sửa UC-17, sửa UC-19 | 10 phút |
| Xoá 2 dòng Table 5 (CreateOnlineMeeting, SyncCalendarEvent), sửa dòng 9 | 5 phút |
| Sửa Table 6 dòng RubricTemplate | 3 phút |
| Thống nhất Thứ Sáu / Thứ Tư · `main` → `master` | 5 phút |
| Sửa 6 chỗ phạm vi semantic search | 10 phút |
| Đổi tên FE-08, sửa FE-14 | 5 phút |

### Đợt 2 — 23/09, phần nặng

- **Nhóm 4 người**: §I.1.2 Table 1 · §II.2.3 Table 6 · §II.4 Table 8 (bỏ cột, chia lại 5 ô `D`) · §II.5 Table 9
- **Table 1 WBS**: sửa tổng mục 4, thêm mục 10, tính lại 231, sửa Table 3
- **Phase 7** vào Table 5 và Table 7
- **Quyết định mobile**, sửa đồng bộ 7 chỗ
- **Table 6 entities** đối chiếu Figure 7

### Đợt 3 — 24/09, định dạng

1. Mở Ngăn Dẫn hướng, sửa cấp heading cho 5 mục đang mất khỏi mục lục
2. Kiểm caption là field thật hay chữ gõ tay
3. Ngắt phần giữa front matter và thân bài, đặt lại số trang La Mã / Ả Rập
4. `Ctrl+A` → `F9`
5. Đọc **print preview** trọn một lượt

### Kiểm cuối — Ctrl+F, mỗi dòng phải ra 0 kết quả

| Tìm | Phải ra |
|---|---|
| `Teams` | 0 |
| `Outlook` | 0 |
| `Calendar` | 0 |
| `SQL Server` | 0 |
| `Azure` | 0 |
| `text-embedding-004` | 0 |
| `gemini-2.0-flash` | 0 |
| `Chinh` | 0 |
| `PWM` | 0 |
| `Error!` | 0 |
| `[` | 0 (placeholder template) |
| `Finance round` | 0 |

Mười hai dòng này bắt gần hết nhóm lỗi §1.5 *"tài liệu không khớp phần mềm"* — đúng nhóm đã khiến
RP4 phải làm lại.

---

## Soi lại lần 2 — 25/09 (ngày nộp) — ĐỌC MỖI MỤC NÀY LÀ ĐỦ

> **25/09: thầy yêu cầu GIỮ tên Nguyễn Lê Hoàng Chinh** trong báo cáo. ⇒ Bìa 5 người, Table 1,
> Table 8 (training), Table 10 (RAM), *"All 5 members"* ở Table 11 — **đều để nguyên, không phải lỗi.**
> Nhóm 3 ở trên (nhóm 4 hay 5 người) **bỏ qua**.

### ✅ Đã sửa
Mục lục đủ I–VI · số hình/bảng chạy liên tục (Figure 1–58, Table 1–41) · câu dẫn trước mỗi bảng ·
bảng acronym sạch, sắp alphabet · số trang front matter La Mã · thêm đoạn giải thích RUP.

### 🔴 Bắt buộc trước khi nộp

| # | Chỗ | Lỗi | Sửa |
|---|---|---|---|
| 1 | **§IV** | Chỉ còn `1. System Design` (trang 102–105). **Thiếu `2. Database Design` và `3. Detailed Design`** — không class/sequence diagram, không Class Specifications, không bảng DB | Copy từ RP4 `2. Database Design` đến hết `3. Detailed Design`, dán sau §IV.1 **trong một lần** |
| 2 | **§II.2.1** đoạn RUP | *"The phases in **Table 5**"* — trong RP7 bảng phase là **Table 7** | Đổi thành Table 7 (chèn cross-reference) |
| 3 | **§II.2.1** đoạn RUP | Câu cuối nhắc **Phase 7** nhưng Table 7 không có dòng Phase 7 | Thêm dòng `Phase 7 — Revision after Project Review \| Week 16–19 \| 07/09 – 25/09 \| …` **hoặc** xoá câu cuối |
| 4 | **§II.1.3 Table 6** rủi ro #3 | *"…the responsibility matrix in Section 4 was redistributed across the remaining four members"* — nhưng Section 4 vẫn giữ Chinh (theo lời thầy) ⇒ tự mâu thuẫn | Đổi thành: *"Triggered. One member's availability dropped during the project; the leader redistributed the outstanding tasks weekly, as planned."* |

### 🟠 Làm được bao nhiêu thì làm

| Chỗ | Sửa |
|---|---|
| **Figure 12** caption | *"…frontend, backend **and mobile client**"* → *"…frontend and backend"* |
| **Table 13** | Xoá 2 dòng `Meeting Integration: Google Meet API` và `Calendar Sync: Google Calendar API` |
| **§III.1** đoạn 2 | *"semantic search across proposals, submitted proposals"* → *"across submitted proposals"*; câu *"It integrates with Google Meet … Google Calendar / Microsoft Outlook"* → *"The system records whether each committee meeting is held in person or online, together with the meeting link supplied by the organiser."* |
| **Table 15** UC-17 / UC-18 | UC-17 → *"Record the time, place or meeting link, and notify participants."*; UC-18 xoá |
| **Table 3** WBS | Mục 4 ghi 28 nhưng chỉ có 7 → sửa thành 7, đổi tên *"Council Meeting Scheduling & Invitations"*; 5.3 `text-embedding-004` → `gemini-embedding-001` |
| **§I.6.1.4 FE-04** | Bỏ chữ `Screening` |
| **§II.2.1** vs **Table 11** | *"Friday"* vs *"Every wednesday"* — chọn một |
| **Table 12** | `main` → `master` |
| **Acknowledgement** dòng cuối | Tách *"Ho Chi Minh City, September 2026"* và *"GSU26SE70"* thành hai dòng |

### Kiểm cuối (`Ctrl+F`, phải ra 0)
`Teams` · `Outlook` · `Calendar` · `text-embedding-004` · `gemini-2.0-flash` · `Finance round` · `Error!` · `[`

---

## Nếu làm Google Calendar (quyết định 25/09)

> ⛔ **ĐỔI QUYẾT ĐỊNH 25/09 (sau):** vẫn code tính năng lịch, nhưng **KHÔNG đưa vào tài liệu nộp 25/09**. ⇒ Mục này bỏ qua; mọi chỗ Meet/Calendar/Outlook **XOÁ** như bảng 🟠 phía trên. Viết lại tài liệu sau khi tính năng chạy xong.

> **Google Meet vẫn không làm** ⇒ mọi chỗ Meet/Teams/Outlook vẫn xoá như bảng trên. Chỉ các chỗ
> **Calendar** đổi từ "xoá" sang "viết lại". LI-06 giữ nguyên (nói về hội nghị trực tuyến, không phải lịch).
>
> Câu chữ dưới viết cho cách làm **nhẹ nhất**: nút *"Add to Google Calendar"* + file `.ics` đính kèm thư mời
> (không cần OAuth). Nếu làm đồng bộ thật qua Google Calendar API thì dùng cột "Nếu đồng bộ qua API".

| Chỗ | Viết thành (link + .ics) | Nếu đồng bộ qua API |
|---|---|---|
| **Table 13** | Giữ dòng, đổi thành `Calendar \| Google Calendar (event link, .ics invitation)`. Vẫn xoá dòng `Meeting Integration` | `Calendar Sync \| Google Calendar API` |
| **§III.1** đoạn 2 | *"…together with the meeting link supplied by the organiser, and lets participants add each council meeting to Google Calendar."* | *"…and creates the meeting as an event in Google Calendar, inviting every council member."* |
| **UC-17** | *"Record the time, place or meeting link, and notify participants."* | như cột trái |
| **UC-18** | Giữ, sửa: *"Add meeting to calendar — Staff, PI, Committee — Add a scheduled council meeting to Google Calendar from the meeting details or the invitation email."* | *"Sync calendar — Staff — Create or update the meeting event in Google Calendar and invite council members."* |
| **Table 16** dòng Meeting Scheduler | *"Set time, place or meeting link; assign presentation slots; add to Google Calendar."* | *"…; sync to Google Calendar."* |
| **Table 18** | Xoá `CreateOnlineMeeting`. Đổi `SyncCalendarEvent` → `BuildCalendarEvent — Builds a Google Calendar event link and an .ics attachment for each scheduled council meeting.` | `SyncCalendarEvent — Creates or updates the council meeting event in Google Calendar through the Google Calendar API.` |
| **Table 3** WBS | Thêm `4.2 Add-to-calendar link and .ics invitation \| Simple \| 3`; tổng mục 4 = **10** | `4.2 Google Calendar API sync \| Complex \| 10`; tổng mục 4 = **17** |
| **§I.6.1.3** Reviewer | Thêm *"FE-12: Add a scheduled council meeting to Google Calendar."* | như cột trái |
| **§I.6.1.2** PI | Thêm *"FE-24: Add a scheduled review meeting to Google Calendar."* | như cột trái |

`.ics` mở được bằng cả Outlook ⇒ nếu làm `.ics` thì được viết *"Google Calendar or any calendar application
that accepts .ics files"*. Không làm `.ics` thì **đừng** nhắc Outlook.

⚠️ Tài liệu nộp 25/09 sẽ **hứa** tính năng này ⇒ phải chạy được trước buổi bảo vệ, và nên thêm một
message vào sequence diagram *"Đặt lịch họp"* ở §IV.3.5 cho khớp.

---

## Soi lần 3 — 25/09, bản gần cuối

**Cấu trúc khớp đủ template** (đối chiếu `RP7_Khung_Template.md`): I–VI đủ mục, §IV đã có `2. Database Design` +
`3. Detailed Design` (Figure 13–30, Table 23–30). Đã sửa: Phase 7, đoạn RUP trỏ Table 7, rủi ro #3, Wednesday,
`master`, Table 13 bỏ Meet/Calendar, WBS mục 4 = 7 / tổng 185 / Table 5 tính lại, §III.1, UC-17, bỏ UC-18,
Table 16 #14, Figure 12, Acknowledgement tách dòng, SĐT, FE-04.

Còn lại (bản dán bị cắt sau Table 16 — phần sau chưa soi được):
1. **UC-24** vẫn *"proposals, reports, and expert profiles"* — chọi §III.1 ngay trên (*"across submitted proposals"*)
2. **§I.5** đoạn 2 cùng lỗi; **FE-14** vẫn *"research orders and prior projects"*
3. **Figure 10** và **Figure 13** cùng caption *"Entity relationship diagram of the FURPMS database."*
4. **§II.1.2** *"(Google Gemini API)- within"* thừa dấu `-`
5. **Table 9** chưa có dòng Software Package 4 / Revised Reports dù Phase 7 nhắc tới

---

## Soi lần 4 — nửa sau RP7 (§IV.3 → §VI.2), 25/09

**§IV.3 Detailed Design: chuẩn.** Đã đối chiếu code: CV cũ quá 6 tháng (`ProposalService.cs:604-611`), trần kinh phí Điều 14 và
tỷ lệ mục chi Điều 15 (`BudgetPolicyService.cs`), `MeetingStatus` = SCHEDULED / IN_PROGRESS / COMPLETED, 6 cổng gửi thư mời.
Tuỳ chọn: §IV.3.1.1 *"RFC-compliant HTTP status codes"* → *"appropriate HTTP status codes"*.

**§V (từ RP5) — nhiều câu là GHI CHÚ RÀ SOÁT bị dán thẳng vào báo cáo** (văn phong "evidence package",
"supplied artifacts", "workbook says not executed"). Cẩm nang §1.4. RP5 là báo cáo biên bản gọi tên ⇒ sửa cả file RP5.

### 🔴 Bắt buộc
1. **§V.3.1 Table 43** — XOÁ đoạn *"Environment note: the project documents contain inconsistent database references (Project Management Plan: MS SQL Server; …)"*. Tài liệu tự khai mâu thuẫn, và giờ còn sai vì Table 13 đã ghi PostgreSQL.
2. **§V.5.1** — XOÁ câu *"No defect-tracker export … was supplied…"* và câu *"Documentation/data-quality issues are tracked separately: … (233 vs. 200) … broken/incomplete formula references…"*. Thay bằng: *"Table 45 summarises the open defects recorded in the test report workbook, by module and severity. Defects found during development were fixed in the iteration in which they were found, and the automated test suite is re-run on every push by the continuous integration pipeline."* ⚠️ Chỉ giữ bảng toàn số 0 nếu sheet Defects trong workbook thật sự không còn defect mở.
3. **§V.5.2 Table 46** — thay cột Evidence và dòng Unit Test (xem bảng trong chat 25/09). Số test tự động lấy từ `dotnet test` trên nhánh nộp (25/09 trên `dev` = 384).
4. **§V.1** câu mở, **§V.1.2** câu mở và 3 gạch cuối — bỏ văn phong "evidence package / supplied artifacts".
5. **§V.3 thiếu `Human Resources`** — template đòi 3.1 Human Resources · 3.2 Test Environment · 3.3 Test Milestones.

### 🟠 Nên
§V.1.1.2 Usability (giao diện song ngữ) · Performance (bỏ "external meeting-service") · Table 35 (semantic search chỉ đề cương) ·
Reliability (bỏ "calendar") · Table 37 (dòng "Meeting provider unavailable") · §V.1.2 "Plagiarism detection" (khớp LI-07) ·
Table 44 (dòng Phase 7) · §VI.1 bỏ câu *"nothing in this list is a placeholder"*.

---

## Soi lần 5 — §IV.1 và §IV.2 (25/09). User quyết định bỏ qua §V.

Đếm lại bằng code 25/09 (nhánh `dev`, sau khi thêm SemanticSearchController):
controllers **52** · service interfaces **49** · service implementations **59** · typed repositories **8** + generic ·
DbSet **65** · FE `*.service.ts` **56** · entity groups 11 · `UtcDateTimeConverter` · store auth/notification/ui.

| Chỗ | Sửa |
|---|---|
| Table 22 #09 | Fifty-eight → **Fifty-six** typed wrappers |
| Table 22 #18 | Fifty-one → **Fifty-two** REST controllers |
| Table 22 #21 | forty service interfaces … nine typed repositories → **forty-nine** … **eight** |
| Table 22 #23 | Fifty-eight → **Fifty-nine** service implementations |
| Table 22 #24 | Nine repositories over Repository.cs → **Eight typed repositories over the generic Repository.cs** |
| §IV.1.1 gạch Third-Party Services | Câu liệt kê bị vỡ (`; Online council meetings…;` chen giữa danh sách) — viết lại |

---

## Đơn đăng ký đề tài (Capstone Project Register, ký 09/04/2026) — 25/09

**Không sửa nội dung.** Đây là văn bản đã ký trước khi làm, sai lệch so với sản phẩm cuối là chuyện bình thường;
sửa lùi lại văn bản đã ký tệ hơn nhiều. Chỉ điền phần hành chính còn trống (bảng sinh viên mục 2, Class, Specialty).

Chỗ đăng ký hứa mà sản phẩm không có, và nơi RP7 đã/chưa giải thích:

| Đăng ký hứa | Sản phẩm | RP7 giải thích ở đâu |
|---|---|---|
| Google Meet / Teams, sync Calendar / Outlook | Chỉ lưu link họp | Rủi ro #1 (Triggered, fallback) · LI-06 ✅ |
| LLM "e.g. xAI API" | Gemini | Đăng ký ghi "e.g." ✅ |
| DB "e.g. MS SQL Server, MySQL", Azure | PostgreSQL, Railway | Đăng ký ghi "e.g." / "like" ✅ |
| Semantic search cả reports, experts, feedback | Chỉ đề cương đã nộp | Chưa — chuẩn bị câu trả lời |
| 2.7 Allocate research resources (lab access, software licenses) | Không có | ❌ → thêm LI-09 |
| 2.8 Internal messaging | Chỉ thông báo trong app + email | ❌ → thêm LI-08 |

Phát hiện phụ: mục 1 ghi **Đặng Ngọc Minh Đức — Assoc.Prof**, Phạm Minh Trí — Mr ⇒ dùng cho xưng hô ở Acknowledgement.
