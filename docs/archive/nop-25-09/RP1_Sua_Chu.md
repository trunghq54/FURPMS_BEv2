# RP1 — Project Introduction: những chỗ cần sửa

> Đối chiếu bản RP1 hiện tại với **code thật** (grep ngày 21/09/2026) và **cẩm nang tránh lỗi**.
> Mọi kết luận dưới đây đều đã verify bằng lệnh, không lấy từ trí nhớ.

---

## Xếp theo mức nguy hiểm

| # | Vấn đề | Mục RP1 | Gạch cẩm nang | Mức |
|---|---|---|---|---|
| 1 | **6 nhóm tính năng mới đã code xong nhưng RP1 không nhắc** | 6.1 | §0 nguyên nhân #5 | 🔴 |
| 2 | **Microsoft Teams / Outlook / Google Calendar — 0 dòng code, nhắc 6 lần** | 3.1, 3.2, 4, 5, 6.1.4, 6.2 | §1.5 | 🔴 |
| 3 | **LI-07 mâu thuẫn thẳng với tính năng vừa làm** | 6.2 | §1.5 | 🔴 |
| 4 | ~~AI Semantic Search không có endpoint~~ → **✅ đã làm 22/09**; còn 2 chỗ sửa chữ về phạm vi | 6.1.2 FE-14, 5 | §1.5 | 🟢 |
| 5 | FE-08 (PI) mô tả ngược chiều với tính năng rà trùng mới | 6.1.2 | §3.1 | 🟠 |
| 6 | Mã `FE-01` lặp cho 5 vai trò khác nhau | 6.1 | traceability | 🟠 |
| 7 | Record of Changes chỉ có 1 dòng | I | §1.2 | 🟡 |
| 8 | Table 1 thiếu SĐT của 5 sinh viên | 1.2 | §1.3 | 🟡 |
| 9 | Chỉ 1 hình, và hình đó chỉ vẽ cho Admin | 6.1.5 | §1.2 | 🟡 |
| 10 | Con số *"dozens to hundreds"* không có nguồn | 4 | §3.3 | 🟡 |
| 11 | "Mobile Application" — cần xác minh như Table 1 của RP4 | 1.1, 5, LI-03 | §1.5 | 🟡 |
| 12 | Mục lục thiếu 6.1.1–6.1.5 | ToC | §1.2 | 🟢 |

---

## 1. 🔴 Sáu nhóm tính năng mới không có trong RP1

Đây là **lỗi nặng nhất**, vì cẩm nang §0 xếp *"Không tiếp thu góp ý của các lần Review đánh giá"*
ở mức **Cao**. Hội đồng đòi 4 gạch, nhóm đã code xong — nhưng RP1, tức tài liệu trả lời câu
*"sản phẩm làm được gì"*, **không thể hiện điều đó**. Mở RP1 ra vẫn là bản trước khi bị trả.

Bằng chứng code đã có:

| Nhóm | Controller / Service | Migration |
|---|---|---|
| Ngân sách | `ProjectBudgetOverviewController` · `ProposalBudgetController` · `BudgetExpenseCategoriesController` | — |
| Mốc thời gian | `ProjectTimelineController` · `ProjectTimelineService` | `AddReviewRoundScoringDeadline` |
| Lưu quyết định | `ProjectDecisionsController` · `DecisionLogger` | `AddProjectDecisions` |
| AI rà trùng | `DuplicateCheckController` · `DuplicateCheckService` · `EmbeddingQueue` · `EmbeddingWorker` · `EvalSetController` | `AddSemanticVectorEmbedding` |
| Cảnh báo điểm lệch | trong `ReviewScoringService` | `AddCouncilDecisionResultJustification` |
| Chuyên môn người chấm | trong `CouncilService` | `AddUserResearchTracks` |

### Cần thêm vào mục 6.1 — bản nháp, kiểm lại với FE trước khi dán

**6.1.2 Faculty (PI)** — thêm:

- FE-20: View the full budget of my project — approved estimate, contract value, planned and disbursed tranches, and remaining balance.
- FE-21: View my project timeline showing every stage of the research lifecycle with its deadline and the rule that deadline comes from.
- FE-22: View the decision dossier of my project — every formal decision from proposal submission to settlement, each linked to its source record.
- FE-23: See the duplicate-check result for my draft proposal, listing the most similar existing projects with a similarity score.

**6.1.3 Reviewer** — thêm:

- FE-11: See the scoring deadline of each review round I am assigned to, with a countdown and an overdue indicator.

**6.1.4 Research Staff** — thêm:

- FE-20: Set and extend the scoring deadline of a review round; extensions are recorded rather than overwritten.
- FE-21: Review the duplicate-check result of a proposal and record a formal verdict (not duplicate / needs revision / duplicate) with a justification.
- FE-22: Record a written justification when a council conclusion diverges from the average score; the system requires it before the minutes can be saved.
- FE-23: Select council members from a list ranked by research-track expertise, with conflict-of-interest candidates flagged.

**6.1.5 System Administrator** — thêm:

- FE-14: Assign research tracks to users so that council eligibility can be checked against expertise.
- FE-15: Manage budget expense categories used in proposal cost estimates.
- FE-16: Configure deadline and threshold settings — scoring window, revision window, final-report lead time, review pass threshold, and duplicate-similarity thresholds.

> ⚠️ **Kiểm trước khi dán:** mở FE chạy thật, xác nhận từng màn hình đúng như mô tả.
> Cẩm nang §1.5 phạt mô tả tính năng không có; mô tả tính năng **có nhưng khác cách hoạt động**
> cũng cùng một gạch.

### Và phải thêm dòng tương ứng vào mục 4 (Business Opportunity)

Mục 4 hiện chỉ nêu AI + tích hợp họp trực tuyến + dashboard. Thêm một gạch:

> ●	A complete administrative record of each project — the approved budget, the deadline of every
> lifecycle stage, and a dossier of every formal decision — so that the university can demonstrate
> compliance with Decision 543/QĐ-ĐHFPT without assembling evidence from separate files.

---

## 2. 🔴 Microsoft Teams / Outlook / Google Calendar — không tồn tại trong code

Kiểm chứng: grep `microsoft.teams` / `msteams` / `outlook` / `google.calendar` trên toàn bộ
`.cs`, `.ts`, `.tsx` chỉ trúng `furpms-web/src/i18n/locales/{en,vi}.ts` — tức **chuỗi hiển thị**,
0 file logic.

`CouncilMeeting.Platform` chỉ có hai giá trị: `IN_PERSON` và `ONLINE`.
`CouncilMeetingService.cs:158-159` map **mọi** giá trị khác về `Online` — chính comment trong code
ghi `// ONLINE, GOOGLE_MEET, TEAMS, ZOOM, giá trị lạ…`. Không có tích hợp nào cả: người dùng
tự dán link vào ô `MeetingLink`.

Google Meet: `furpms-web/src/services/google-meet.service.ts` có tồn tại, nhưng
`useGenerateGoogleMeetLink()` **không nơi nào gọi**, và **không có controller BE** tương ứng.

**Đây đúng là lỗi vừa sửa xong trong RP4.** RP1 còn nguyên, và nhắc tới 6 lần:

| Mục | Câu hiện tại | Sửa thành |
|---|---|---|
| **3.1** Cons (NAFOSTED) | *"no support for online committee meetings"* | *"no support for recording online committee meetings"* — hoặc bỏ hẳn gạch này |
| **3.2** Cons (eSM) | *"no integration with video conferencing tools (Google Meet, Teams)"* | *"no support for recording the platform and link of online review meetings"* |
| **4** gạch 2 | *"Integration with modern collaboration tools (Google Meet, Microsoft Teams, Google Calendar, Microsoft Outlook) to support online committee meetings without switching platforms."* | *"Support for both in-person and online review meetings: the system records the meeting platform, link, agenda and presentation order, and notifies every council member, so that scheduling no longer happens over email."* |
| **5** đoạn 2, câu cuối | *"The system integrates with Google Meet and Microsoft Teams for online committee meetings, and with Google Calendar and Microsoft Outlook for seamless scheduling."* | **Xoá hẳn câu này.** Thay bằng: *"The system schedules review meetings, records whether each meeting is held in person or online, and tracks invitation responses from every council member."* |
| **6.1.4** FE-08 | *"(Google Meet/MS Teams link generation, offline location, agenda, timing)"* | *"(meeting platform — in person or online — meeting link, location, agenda, timing)"* |
| **6.2** LI-06 | *"Video conferencing is handled by Google Meet or Microsoft Teams; FURPMS does not provide its own video conferencing infrastructure."* | *"FURPMS does not provide video conferencing. For online meetings the system stores the meeting link supplied by the organiser; it does not create or manage the meeting on the conferencing platform."* |

> Bản sửa LI-06 ở trên **mạnh hơn bản cũ** về mặt bảo vệ: nó nói rõ ranh giới, và đúng với
> code. Hội đồng hỏi *"sao không tích hợp Meet?"* thì trả lời được ngay: nằm ngoài phạm vi,
> đã ghi trong LI-06.

---

## 3. 🔴 LI-07 mâu thuẫn với chính sản phẩm

> LI-07 hiện tại: *"The system will not support plagiarism detection for submitted proposals or reports in this version."*

Nhưng `DuplicateCheckController` đã tồn tại với 4 endpoint, `DuplicateCheckService` +
`EmbeddingWorker` + bộ eval set đã chạy. **Đây là mâu thuẫn bắt được mà không cần mở repo** —
chỉ cần đọc RP1 rồi xem demo.

**Sửa LI-07 thành:**

> LI-07	The system detects potential duplication between a new proposal and existing projects by
> semantic similarity, and reports the most similar candidates for human review. It does not perform
> sentence-level plagiarism detection against external publication databases, and it does not reject a
> proposal automatically — the conclusion is always recorded by research staff.

Câu này vừa gỡ mâu thuẫn, vừa nêu đúng hai giới hạn thật của cách làm (không đối chiếu kho
ngoài, không tự kết luận) — cẩm nang §5 đòi nói rõ ai kiểm chứng đầu ra AI.

---

## 4. ✅ ĐÃ LÀM 22/09 — AI Semantic Search giờ chạy thật

Trước 22/09 đây là tính năng ghi trong RP1 mà **không có endpoint BE**, trang bị ẩn khỏi nav,
hook đọc dữ liệu giả từ `mock/data/ai-tools.ts`. **Đã làm xong:**

| Thành phần | File |
|---|---|
| Endpoint `POST /api/ai/search` | `FURPMS.API/Controllers/SemanticSearchController.cs` |
| Dịch vụ | `FURPMS.Infrastructure/Services/SemanticSearchService.cs` |
| DTO | `FURPMS.Application/DTOs/AI/SemanticSearchDtos.cs` |
| Test (9 ca, chạy offline) | `FURPMS.Tests/Ai/SemanticSearchServiceTests.cs` |
| FE gọi thật | `src/services/api/ai.service.ts` |
| Trang bật lại trong menu | `src/constants/nav.ts:78` |

Dùng **chung kho vector với rà trùng lặp** nên không tốn thêm quota vector hoá — mỗi lần tìm chỉ
tốn một lần nhúng câu hỏi. Kiểm: `dotnet build` · `dotnet test` (382 xanh) · `npm run typecheck`
· `npm run build` — sạch cả bốn.

### ⚠️ Nhưng câu chữ trong RP1 vẫn phải sửa — phạm vi hẹp hơn lời hứa

`EmbeddingWorker` **chỉ vector hoá đề cương đã nộp**. **Đơn đặt hàng nghiên cứu
(`research_orders`) chưa được vector hoá.** Nên câu FE-14 hiện tại vẫn sai:

| Mục | Hiện tại | Sửa thành |
|---|---|---|
| **6.1.2** FE-14 | *"Search **research orders** and prior projects semantically via AI Semantic Search."* | *"Search previously submitted proposals semantically, ranked by meaning rather than keywords."* |
| **2** đoạn 3 | *"…such as AI-generated summaries or semantic search"* | ✅ giữ nguyên — nói về vấn đề, không hứa phạm vi |
| **3.1** Cons | *"limited semantic search"* | ✅ giữ nguyên — nói về NAFOSTED |
| **4** gạch 3 | *"…and enable semantic search"* | ✅ giữ nguyên |
| **5** đoạn 2 | *"enable semantic search across proposals, reports, and **expert profiles**"* | *"enable semantic search across submitted proposals"* — **báo cáo và hồ sơ chuyên gia cũng chưa vector hoá** |

⇒ Chỉ còn **2 chỗ** phải sửa (6.1.2 FE-14 và mục 5), không phải 5 chỗ như bản trước.

Giao diện đã sửa đồng bộ: `aiSearch.subtitle` ở cả `en.ts` và `vi.ts` bỏ cụm *"imported research
topics"*, và thêm trạng thái riêng cho **kho vector còn rỗng** (khác hẳn "không tìm thấy").

### RP2 cũng được lợi

`RP2_Sua_Chu.md` mục 4 định bỏ dòng WBS 5.3 *"Semantic search — Gemini Embeddings"* (Complex, 10
man-day). **Giữ lại dòng đó** — giờ là công việc có thật. Chỉ sửa tên model:
`text-embedding-004` → **`gemini-embedding-001`**.

---

## 5. 🟠 FE-08 (mục 6.1.2) mô tả ngược chiều

> FE-08 hiện tại: *"Use AI Similarity Check to compare draft documents with selected applied research
> order topics, displaying warnings if matching score is **low**."*

Bây giờ hệ thống có **hai** tính năng dùng chữ "similarity", **cảnh báo ngược chiều nhau**:

| | FE-08 cũ | Rà trùng mới |
|---|---|---|
| So với | đơn **đặt hàng** đã chọn | **kho đề tài đã có** |
| Cảnh báo khi điểm | **THẤP** (lệch đề bài) | **CAO** (trùng đề tài) |

Để nguyên hai mục cùng gọi là "similarity" thì người đọc — và AI chấm bài — sẽ tưởng nhóm
mô tả sai chính sản phẩm mình. **Đổi tên cho tách bạch:**

- FE-08 → *"Use **AI Order Alignment Check** to compare a draft with the selected research order and warn when the draft drifts from the ordered topic."*
- FE-23 (mới, mục 1 ở trên) giữ chữ *duplicate*, không dùng chữ *similarity*.

---

## 6. 🟠 Mã FE-01 lặp cho 5 vai trò

Guest FE-01, PI FE-01, Reviewer FE-01, Staff FE-01, Admin FE-01 — **năm thứ khác nhau cùng một mã**.
Nói *"FE-01"* không xác định được gì.

**Trước khi sửa, kiểm RP3:** nếu RP3 đã trích các mã này vào use case thì đổi số sẽ vỡ RP3.

| Nếu RP3… | Làm gì |
|---|---|
| chưa trích mã FE | Đổi tiền tố theo vai: `GU-01` · `PI-01` · `RV-01` · `ST-01` · `AD-01` |
| đã trích rồi | Giữ số, nhưng mọi chỗ trích phải viết đủ vai — *"PI FE-08"*, không viết trống *"FE-08"* |

---

## 7. 🟡 Record of Changes — mục I

Đang có đúng 1 dòng (16/05/2026, *Initial draft*). Nhưng RP1 đã qua ít nhất 2 đợt sửa.
Sau khi dán các thay đổi ở trên, thêm:

| Date | A/M/D | In charge | Change Description |
|---|---|---|---|
| **21/09/2026** | M | | Removed references to integrations that are not part of the product (Microsoft Teams, Microsoft Outlook, Google Calendar) and restated the meeting scope in LI-06. |
| **21/09/2026** | A | | Added the features introduced after the first defence: project budget overview, lifecycle deadlines, decision dossier, duplicate detection, score-divergence justification and reviewer expertise matching. |
| **21/09/2026** | M | | Rewrote LI-07 to describe the scope and the limits of duplicate detection. |

Cách chia cột `In charge` xem `RP4_Sua_Chu.md` mục J.

---

## 8. 🟡 Các mục nhỏ

| Mục | Vấn đề | Sửa |
|---|---|---|
| **1.2 / Table 1** | 5 sinh viên bỏ trống cột `Mobile`, 2 GVHD thì có | Điền đủ, hoặc bỏ hẳn cột nếu không muốn công khai. **Đừng để nửa đầy nửa trống** |
| **1.1** | *"Software type: Web Application, Mobile Application"* | Xác minh repo mobile như đã làm với Table 1 của RP4. Có thật thì giữ, kèm được tên repo càng tốt |
| **4** | *"dozens to hundreds of research projects each academic year"* | §3.3: con số phải giải thích được nguồn. Lấy số thật từ Phòng QLKH, hoặc đổi thành câu không có số |
| **3.1 / 3.2** | Mỗi gạch `Cons` khớp chính xác một tính năng của FURPMS | Dễ bị hỏi *"em khảo sát thật hay viết ngược từ tính năng của mình ra?"*. Giữ 2–3 `Cons` thật sự quan sát được, bỏ phần còn lại |
| **3** | Chỉ có 2 hệ thống, và **không có bảng so sánh** | Thêm 1 bảng 3 cột (NAFOSTED · eSM · FURPMS) × 5–6 tiêu chí. Rẻ, và trả lời gọn câu *"khác gì hệ thống có sẵn?"* |
| **6.1.5** | Chỉ có 1 hình trong cả RP1, lại chỉ vẽ cho Admin | Hoặc thêm hình cho 4 vai còn lại, hoặc đổi Figure 1 thành **một hình gộp cả 5 vai** rồi sửa caption |
| **ToC** | Thiếu 6.1.1–6.1.5 | Đặt Heading đúng cấp cho 5 mục con rồi `Ctrl+A` → `F9` |

---

## 9. Thứ tự làm

1. **Sửa 6 chỗ Teams/Outlook/Calendar** (mục 2) ← nguy hiểm nhất, và rẻ nhất
2. **Sửa LI-07** (mục 3) ← một ô bảng
3. **Quyết định Semantic Search**: bỏ khỏi tài liệu hay làm endpoint (mục 4)
4. **Đổi tên FE-08** (mục 5)
5. **Chạy FE thật, đối chiếu rồi dán 11 mục FE mới** (mục 1) ← tốn thời gian nhất, đừng dán mò
6. Thêm bảng so sánh ở mục 3, tỉa bớt `Cons`
7. Điền Record of Changes, sửa Table 1, sửa mục lục
8. `Ctrl+A` → `F9`, kiểm ngày trang bìa

> Bước 1–4 làm được trong một buổi tối và gỡ cả 3 lỗi 🔴 lẫn 1 lỗi 🟠.

---

## 10. 🔴 Nhóm còn 4 người — Table 1 và mọi bảng phân công

**Ghi nhận 21/09/2026:** Nguyễn Lê Hoàng Chinh đã rời nhóm. Còn 4 thành viên:
Hoàng Quốc Trung (Leader) · Nguyễn Tấn Phát · Đàm Mạnh Dũng · Nguyễn Trọng Thứ.

### Xử lý trong RP1

**Bỏ dòng Chinh khỏi Table 1 (mục 1.2), và ghi nhận việc đó vào Record of Changes.**

Lý do: hội đồng nhận danh sách nhóm **từ phòng đào tạo**, không phải từ báo cáo. Bảng
trong RP1 phải khớp danh sách chính thức hiện tại. Nhưng xoá im lặng thì bản v1 có 5 tên,
bản v2 có 4 tên, không một dòng giải thích — **Record of Changes sinh ra đúng để ghi việc này**.

Thêm vào mục I:

| Date | A/M/D | In charge | Change Description |
|---|---|---|---|
| **21/09/2026** | D | | Updated the project team to reflect its current composition of four members. |

Câu trên không nêu tên người rời — đủ minh bạch mà không cần phơi chuyện cá nhân.

> ⚠️ **Hỏi GVHD trước khi sửa.** Việc thành viên rời nhóm có thủ tục hành chính riêng, và
> thầy có thể muốn xử lý theo cách khác (ví dụ giữ tên kèm ghi chú giai đoạn tham gia).
> Đây là chỗ duy nhất trong cả tài liệu mà tôi khuyên **hỏi người trước khi gõ**.

### Kéo theo ở nơi khác — đừng quên

| Chỗ | Việc |
|---|---|
| **RP2** (Process / teamwork) | Bảng phân công chia lại cho 4 người. Cẩm nang §9 gạch *"không phân biệt được ai làm phần nào"*; chia cho 4 khác hẳn chia cho 5 |
| **RP7** | Danh sách nhóm ở front matter |
| **Cột `In charge`** mọi Record of Changes | Chỉ dùng 4 tên |
| **Mục lục + List of Tables** | Bớt 1 dòng trong Table 1 làm lệch số trang → `Ctrl+A` → `F9` |

### Và một câu hỏi thật sự cần trả lời

**Mobile còn nằm trong phạm vi không?**

Mục 1.1 ghi *"Software type: Web Application, Mobile Application"*, mục 5 ghi *"web and
mobile application"*, LI-03 hứa app cho PI và Reviewer. Với 4 người, **ai đang làm mobile?**

- Còn người làm → giữ nguyên, nhưng phải demo được
- Không còn → sửa cả 3 chỗ trên **trước khi bảo vệ**, đừng để hội đồng hỏi *"cho xem app mobile"*

Cẩm nang §0 xếp *"demo lỗi hoặc không demo được hết luồng chính"* ở mức **Rất cao** — cao
nhất trong 6 nguyên nhân trượt.

---

## 11. Trạng thái sau đợt sửa 21/09/2026

### ✅ Đã sửa

| Mục | Nội dung |
|---|---|
| 3.1, 3.2 | Bỏ Teams / Google Meet khỏi phần `Cons` |
| 4 gạch 2 | Viết lại thành in-person / online, bỏ Calendar + Outlook |
| 4 gạch 5 | **Thêm mới** — hồ sơ hành chính (ngân sách · mốc · quyết định) |
| 5 đoạn 2 | Bỏ câu tích hợp Meet/Teams/Calendar/Outlook |
| 6.1.4 FE-08 | Bỏ *"Google Meet/MS Teams link generation"* |
| LI-06 | Viết lại ranh giới video conferencing |
| LI-07 | Viết lại — hết mâu thuẫn với rà trùng |
| 6.1.2–6.1.5 | **Thêm đủ 11 mục FE mới** |
| ToC | Đã có 6.1.1–6.1.5 |

### ☐ Còn mở

| # | Việc | Mức |
|---|---|---|
| 1 | ~~Semantic search~~ **✅ code đã xong 22/09** — còn sửa chữ 2 chỗ (mục 4) | 🟢 |
| 2 | **FE-08 (6.1.2) vẫn ghi *AI Similarity Check … score is low*** | 🟠 |
| 3 | Mã `FE-01` lặp cho 5 vai | 🟠 |
| 4 | **Bỏ Chinh khỏi Table 1** (mục 10) | 🔴 |
| 5 | **Mobile còn trong phạm vi không** (mục 10) | 🔴 |
| 6 | Record of Changes — mới có 1 dòng | 🟡 |
| 7 | Table 1 thiếu SĐT 5 sinh viên (giờ là 4) | 🟡 |
| 8 | *"dozens to hundreds"* không nguồn | 🟡 |
| 9 | Figure 1 chỉ vẽ cho Admin | 🟡 |
| 10 | Chưa có bảng so sánh ở mục 3 | 🟡 |

> Việc **#1 và #2 không phải lỗi hình thức.** #1 là mô tả một tính năng trả dữ liệu giả;
> #2 là hai tính năng cùng tên nhưng cảnh báo ngược chiều. Cả hai đều rơi vào §1.5 — đúng
> nhóm lỗi đã khiến RP4 phải làm lại. Mỗi việc sửa mất dưới 10 phút.
