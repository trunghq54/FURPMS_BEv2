# RP2 — Project Management Plan: những chỗ cần sửa

> Đối chiếu với **code thật** (grep 21/09/2026) và **cẩm nang tránh lỗi**.
> **Hạn nộp: 10h00 ngày 25/09/2026** — còn ~3 ngày rưỡi tính từ lúc soạn file này.
> Mục 10 có kế hoạch chia theo ngày.

---

## Xếp theo mức nguy hiểm

| # | Vấn đề | Mục RP2 | Gạch cẩm nang | Mức |
|---|---|---|---|---|
| 1 | **Table 11 sai 6 dòng + thiếu 2 dòng về công nghệ** | 6.3 | §1.5, §13 Q3 | 🔴 |
| 2 | **Table 8 (RAM): Chinh giữ 5 ô `D` — 5 hạng mục không có người làm chính** | 4 | §9, §0 #6 | 🔴 |
| 3 | **Kế hoạch kết thúc 06/09, hôm nay 21/09 — dự án vẫn đang chạy** | 2.1, 3 | §9 | 🔴 |
| 4 | Table 1 (WBS): 28 man-day cho tính năng không tồn tại; thiếu 6 nhóm đã làm thật | 1.1 | §1.5 | 🟠 |
| 5 | **Mâu thuẫn nội bộ: họp GVHD thứ Sáu hay thứ Tư?** | 2.1 vs 5 | §1.5 | 🟠 |
| 6 | Ghi chú soạn thảo còn sót: *"(See attached RUP phase diagram — Figure 2.1)"* | 2.1 | §1.3 | 🟠 |
| 7 | Table 2: *"≥70% code coverage"* — **không hề được đo** | 1.2 | §6 | 🟠 |
| 8 | Table 10: nhánh production ghi `main`, thật ra là `master` | 6.2 | §1.5 | 🟠 |
| 9 | Table 6 + Table 9 còn tên Chinh / *"All 5 members"* | 2.3, 5 | §9 | 🟠 |
| 10 | Table 4 (Risks): 3 rủi ro **đã thật sự xảy ra** mà không cập nhật | 1.3 | §9 | 🟡 |
| 11 | Mobile: có training, không có WBS, không có ô trong RAM | 1.1, 2.3, 4 | §1.5 | 🟡 |
| 12 | Record of Changes chỉ 1 dòng | I | §1.2 | 🟡 |

---

## 1. 🔴 Table 11 (mục 6.3) — bảng công nghệ sai gần hết

Đây là bảng **hội đồng soi kỹ nhất** sau sơ đồ kiến trúc. Cẩm nang §13 câu 3:
*"chỉ vào sơ đồ kiến trúc rồi yêu cầu mở đúng đoạn code tương ứng"*.

| Dòng | RP2 đang ghi | Thật | Bằng chứng |
|---|---|---|---|
| **Database** | `MS SQL Server + Entity Framework Core` | **`PostgreSQL 16 + Entity Framework Core`** | `DependencyInjection.cs:25` gọi `.UseNpgsql(...)` |
| **LLM — Summarization & Feedback** | `Google Gemini API (gemini-2.0-flash)` | **`Google Gemini API (gemini-3.5-flash-lite, fallback gemini-3.1-flash-lite)`** | `appsettings.Development.json:25-26` |
| **LLM — Semantic Search** | `Gemini Embeddings (text-embedding-004)` | **`Gemini Embeddings (gemini-embedding-001)`** | `appsettings.Development.json:27` |
| **Meeting Integration** | `Google Meet API, Microsoft Teams API` | **Không có.** Bỏ hẳn dòng | grep toàn repo = 0 file logic |
| **Calendar Sync** | `Google Calendar API, Microsoft Outlook API` | **Không có.** Bỏ hẳn dòng | grep toàn repo = 0 file logic |
| **Deployment** | `Microsoft Azure (App Service + Azure SQL), Vercel` | **`Railway (backend + PostgreSQL), Vercel (frontend)`** | `furpms-web/vercel.json` có thật; không có file cấu hình Azure nào |
| **File Storage** | *thiếu* | **`Cloudinary`** ← thêm dòng | `CloudinaryFileStorage.cs`, `IFileStorage.cs` |
| **Transactional Email** | *thiếu* | **`Brevo SMTP relay (smtp-relay.brevo.com)`** ← thêm dòng | `appsettings.json` → `EmailSettings.SmtpServer`; gửi qua `System.Net.Mail.SmtpClient` trong `SmtpEmailService.cs` |

> ⚠️ Ghi **`Brevo SMTP relay`**, đừng ghi *"Brevo API"*. Code dùng `SmtpClient` chuẩn của .NET
> trỏ vào relay của Brevo, không gọi REST API của Brevo. Hội đồng hỏi *"dùng API nào của Brevo?"*
> mà trả lời sai là mất điểm ở chỗ lẽ ra rất dễ.

**Tại sao bảng này sai:** trong repo BE có hẳn một nhánh tên **`be-luc-van-con-la-sql-server-`**
và một nhánh **`thu-nghiem/postgres`**. Nhóm đã chuyển từ SQL Server sang PostgreSQL nhưng
tài liệu không đổi theo. Đây đúng 5 lỗi mà `RA_SOAT_BAO_CAO_RP1-7.md` đã ghi từ **16/08/2026**
cho sơ đồ kiến trúc RP4 — RP4 sửa rồi, RP2 còn nguyên.

### Sửa luôn 2 chỗ khác cùng nội dung

| Mục | Câu | Sửa |
|---|---|---|
| **1.2** Overall Objective | *"integrating LLM-powered AI features (Google Gemini API) **and online meeting tools**"* | Bỏ cụm *"and online meeting tools"* |
| **Table 11** Frontend | `ReactJS ( web), React Native (Mobile app)` | Sửa khoảng trắng thừa; xem mục 11 về React Native |

---

## 2. 🔴 Table 8 (mục 4) — 5 hạng mục mất người làm chính

Nhóm còn **4 người**: Trung (Leader) · Thứ (FE) · Phát (BE) · Dũng (FE).
Nhưng cột `ChinhNLH (BE)` đang giữ **`D` — người làm chính** ở 5 dòng:

| Dòng | Ai nhận lại? |
|---|---|
| Report 3 – SRS (Overview) | ☐ |
| Report 4 – Database Design | ☐ |
| Feature 1: Auth & User Mgmt (BE) | ☐ |
| Feature 3: Reviewer & Expert Mgmt (BE) | ☐ |
| Feature 4: Meeting Integration (BE) | ☐ ← **hoặc xoá dòng**, xem mục 1 |

**Đây là chỗ nguy hiểm nhất RP2.** Hội đồng mở RAM ra, thấy người làm Database Design và Auth
không còn trong nhóm, câu hỏi tiếp theo là *"vậy ai bảo vệ hai phần đó?"* Cẩm nang §0 xếp
*"thành viên đóng góp quá ít"* vào 6 nguyên nhân trượt, §9 gạch *"không phân biệt được ai làm phần nào"*.

### Cách sửa

1. **Xoá hẳn cột `ChinhNLH`** khỏi Table 8
2. **Xoá 2 dòng** `Feature 4: Meeting Integration (BE)` và `(FE)` — tính năng không tồn tại
3. **Phân lại 4 ô `D` còn lại** cho Phát (BE) hoặc Trung
4. **Thêm dòng cho 6 nhóm tính năng mới** đã làm sau bảo vệ lần 1:

| Responsibility | TrungHQ | ThuNT | PhatNT | DungDM |
|---|---|---|---|---|
| Feature 9: Project Budget Overview (BE) | | | | |
| Feature 9: Project Budget Overview (FE) | | | | |
| Feature 10: Lifecycle Deadlines & Timeline (BE) | | | | |
| Feature 11: Decision Dossier (BE) | | | | |
| Feature 12: AI Duplicate Detection (BE) | | | | |
| Feature 13: Score-Divergence Justification (BE) | | | | |
| Feature 14: Reviewer Expertise Matching (BE) | | | | |

> Điền `D/R/S/I` theo đúng người đã làm thật. **Đừng chia đều cho đẹp** — git log là bằng chứng
> đối chứng (§9). Mỗi người phải có ít nhất vài ô `D` để chứng minh đóng góp.

### Kéo theo

| Mục | Sửa |
|---|---|
| **Table 6** (2.3 Training Plan) | Chinh xuất hiện **2 lần** (Gemini API · Meet/Teams API). Xoá dòng *"Google Meet / MS Teams API"* hẳn; chuyển dòng Gemini sang người thật sự làm |
| **Table 9** (mục 5) | *"All **5** members"* → *"All **4** members"* |

---

## 3. 🔴 Kế hoạch đã kết thúc trước ngày hôm nay

Table 5 (mục 2.1): Phase 6 — Transition, **10/08 – 06/09**, kết thúc bằng *"Defense"*.
Table 7 (mục 3): deliverable #10 *"Report 7 + Defense Slides"* due **06/09/2026**.

Nhưng hôm nay là **21/09/2026**, và nhóm đang chuẩn bị **bảo vệ lần 2**, hạn nộp **25/09**.

⇒ Chính tài liệu quản lý dự án đang nói **dự án đã xong 2 tuần trước**. Hội đồng đọc RP2 rồi
nhìn lịch là thấy ngay nhóm không cập nhật kế hoạch — đúng gạch §9.

### Thêm vào Table 5

| Phase | Weeks | Period | Key Activities |
|---|---|---|---|
| Phase 7 — Revision after First Defence | Week 16–19 | 07/09 – 25/09 | Budget overview, lifecycle deadlines, decision dossier, AI duplicate detection, score-divergence justification, reviewer expertise matching; full document revision |

### Thêm vào Table 7

| # | Deliverable | Due Date | Notes |
|---|---|---|---|
| 11 | Software Package 4 — features required by the first defence board | 21/09/2026 | Budget, deadlines, decisions, duplicate detection |
| 12 | Revised Reports 1–7 | 25/09/2026 | Second defence submission |

> Điều chỉnh ngày theo thực tế. Nhưng **phải có một phase kéo tới 25/09**, nếu không tài liệu
> tự mâu thuẫn với chính buổi bảo vệ đang diễn ra.

---

## 4. 🟠 Table 1 (mục 1.1) — WBS

### Bỏ 21 man-day cho việc không tồn tại

| Item | RP2 | Xử lý |
|---|---|---|
| 4.1 Schedule meetings via Google Meet / MS Teams API | Complex, 12 | **Xoá** |
| 4.2 Auto-send invitations (link, agenda, documents) | Medium, 7 | **Giữ** — có thật (`send-invitations`) |
| 4.3 Sync with Google Calendar / MS Outlook | Complex, 9 | **Xoá** |
| **4** Online Committee Meeting Integration | 28 | Đổi tên → **`Council Meeting Scheduling & Invitations`**, còn **7** |

### Thiếu 6 nhóm đã làm thật — thêm mục 10

| # | WBS Item | Complexity | Est. Effort |
|---|---|---|---|
| **10** | **Post-Defence Enhancements** | | **46** |
| 10.1 | Project budget overview (estimate → contract → disbursement → settlement) | Complex | 10 |
| 10.2 | Lifecycle deadline model & reminder scanners | Complex | 12 |
| 10.3 | Project decision dossier with backfill | Medium | 8 |
| 10.4 | AI duplicate detection (embedding + LLM explanation + eval set) | Complex | 12 |
| 10.5 | Score-divergence warning & justification | Simple | 2 |
| 10.6 | Reviewer expertise matching for council formation | Simple | 2 |

**Tổng mới:** 206 − 21 + 46 = **231 man-days**.
Nhớ sửa luôn **Table 3** (Effort distribution) vì mọi % tính trên tổng 206.

> Con số 46 là ước lượng của tôi theo khối lượng code thật (5 migration, 6 controller,
> 3 service AI, 19 file test mới). Chỉnh theo cảm nhận thật của nhóm — nhưng **đừng để
> trống mục 10**, vì đó chính là phần trả lời biên bản hội đồng.

---

## 5. 🟠 Mâu thuẫn nội bộ — thứ Sáu hay thứ Tư?

| Mục | Câu |
|---|---|
| **2.1** Weekly cadence | *"**Friday**: Supervisor meeting — demo progress, get feedback"* |
| **Table 9** (mục 5) | *"Supervisor Meeting … Every **wednesday**, 60 min"* |

Hai câu trong cùng một tài liệu. Chọn đúng một ngày, sửa cả hai chỗ.
Tiện thể viết hoa **`Wednesday`**.

---

## 6. 🟠 Ghi chú soạn thảo còn sót (mục 2.1)

> *"(See attached RUP phase diagram — Figure 2.1)"*

**Trong RP2 không có Figure 2.1 nào** — chỉ có Figure 1 và Figure 2. Đây là ghi chú lúc soạn
bị bỏ quên. Cẩm nang §1.3 gạch *"còn sót ghi chú cá nhân, placeholder"*; `RA_SOAT_BAO_CAO_RP1-7.md`
đã bắt lỗi placeholder trong RP2 từ 16/08.

**Xoá hẳn dòng đó.** Ngay dưới nó đã có câu *"Figure 1 shows the development process the team
followed."* — đúng chuẩn §1.6 rồi, không cần gì thêm.

**Rà thêm:** Ctrl+F trong file `.docx` tìm `(`, `[`, `<` để bắt placeholder còn sót khác.

---

## 7. 🟠 Table 2 (mục 1.2) — chỉ tiêu không đo được

> *"Unit Test | BE developers | **≥ 70% code coverage** | < 5 minor | xUnit (.NET)"*

CI thật (`FURPMS_BEv2/.github/workflows/ci.yml`) chạy:

```
dotnet test --no-build -c Release --verbosity normal
```

**Không có** `--collect:"XPlat Code Coverage"`. Gói `coverlet.collector` có trong
`FURPMS.Tests.csproj` nhưng **không được gọi**. ⇒ Không ai biết coverage thật là bao nhiêu.

Câu hỏi §6 rất dễ bị hỏi: *"70% đó đo bằng gì, cho xem báo cáo coverage."*

**Hai đường:**

| Đường | Làm gì | Thời gian |
|---|---|---|
| **A. Đo thật** (nên làm) | Chạy `dotnet test --collect:"XPlat Code Coverage"`, lấy số thật, **ghi đúng số đó vào Table 2** | ~15 phút |
| **B. Hạ chỉ tiêu** | Đổi ô thành *"All service-layer business rules covered by xUnit tests (62 test files)"* — đo được, kiểm được | ~2 phút |

Đường A tốt hơn nhiều nếu số ra đẹp. Nếu ra 30% thì dùng đường B — **đừng để nguyên 70%
khi không đo**.

### Hai ô khác trong Table 2 cũng cần soát

| Ô | Vấn đề |
|---|---|
| Integration Test — *"Postman + GitHub Actions"*, *"100% API endpoints"* | CI chỉ chạy `dotnet test`, **không chạy Postman**. 51 controller mà phủ 100% endpoint bằng Postman là khó tin — có bộ collection thật không? |
| — | `furpms-web` **không có thư mục `.github`** ⇒ FE không có CI. Nếu Table 2 hàm ý có thì sửa |

---

## 8. 🟠 Table 10 (mục 6.2) — nhánh production

| RP2 ghi | Thật |
|---|---|
| `main` — Production-ready code only | Repo BE có **`master`** và `dev`; CI trigger on `[master, dev, dev1]`. Railway deploy từ **`master`** |
| — | Có nhánh `dev1` trong CI nhưng Table 10 không nhắc |

Repo FE có `main`, repo BE có `master` — **hai repo khác nhau**. Ghi rõ trong bảng, hoặc
thêm một dòng ghi chú.

---

## 9. 🟡 Table 4 (mục 1.3) — 3 rủi ro đã thật sự xảy ra

Đây không phải lỗi, mà là **cơ hội ăn điểm** đang bỏ phí. Bảng rủi ro mà mọi dòng đều
*"chưa xảy ra"* trông như viết cho có. Ba dòng dưới đây đã thành hiện thực và nhóm **đã xử lý đúng
như kế hoạch dự phòng** — viết ra thì bảng này thành bằng chứng quản trị rủi ro thật.

| # | Rủi ro | Đã xảy ra thế nào | Thêm vào cột `Response Plan` |
|---|---|---|---|
| **1** | Meet/Teams API phức tạp | **Đúng như dự đoán.** Phương án dự phòng *"fallback to link generation only"* đã được kích hoạt — hệ thống chỉ lưu link người tổ chức dán vào | *"**Triggered.** The fallback was adopted: the system stores the organiser-supplied meeting link instead of integrating with a conferencing API."* |
| **2** | Hết quota Gemini free tier | Đã xây cache `llm_outputs` + `ContentHash` để không gọi lại khi nội dung không đổi, + hàng đợi có giới hạn | *"**Triggered.** Responses are cached by content hash and the embedding queue is rate-limited, so unchanged content is never re-sent."* |
| **3** | Thành viên tụt lại vì môn khác / OJT | **Đã xảy ra** — một thành viên rời dự án; công việc được phân lại cho 4 người còn lại | *"**Triggered.** One member left the project; the responsibility matrix in Section 4 was redistributed across the remaining four members."* |

> Dòng 3 vừa là quản trị rủi ro thật, vừa **giải thích sẵn** việc Table 8 chỉ còn 4 cột —
> hội đồng không phải hỏi.

Rủi ro **#4** *"Semantic search performance slow on large datasets"* — hiện chưa có endpoint
search. Giữ dòng này nếu nhóm định làm nốt; nếu không thì đổi thành rủi ro về ngưỡng rà trùng.

---

## 10. 🟡 Mobile — RP2 tự mâu thuẫn ba chiều

| Chỗ | Nói gì |
|---|---|
| **Table 6** (2.3) | *"React Native — Đàm Mạnh Dũng — Mandatory"* → có đào tạo |
| **Table 11** (6.3) | *"React Native (Mobile app)"* → có công nghệ |
| **Table 1** (1.1 WBS) | **không có một mục mobile nào** trong 206 man-day |
| **Table 8** (mục 4 RAM) | **không có một dòng mobile nào** |

Tức là: học React Native, khai công nghệ React Native, nhưng **không ước lượng công, không phân
công ai làm**. Ba bảng trong cùng tài liệu nói ba chuyện khác nhau.

Đây liên quan thẳng câu hỏi *"mobile còn trong phạm vi không?"* ở mục 10 của `RP1_Sua_Chu.md`.
**Quyết định một lần, sửa đồng bộ cả RP1 và RP2.**

---

## 11. 🟡 Record of Changes (mục I)

Đang có 1 dòng (22/05/2026). Thêm:

| Date | A/M/D | In charge | Change Description |
|---|---|---|---|
| **21/09/2026** | M | | Corrected the technology stack: PostgreSQL in place of SQL Server, the current Gemini model identifiers, Railway as the backend host, and the addition of Cloudinary and Brevo. Removed the conferencing and calendar integrations that are not part of the product. |
| **21/09/2026** | M | | Updated the project team to four members and redistributed the responsibility matrix accordingly. |
| **21/09/2026** | A | | Added Phase 7 and the work breakdown for the features required after the first defence. |
| **21/09/2026** | M | | Recorded the three project risks that materialised and the mitigations that were applied. |

---

## 12. Một dòng về bảo mật

`FURPMS.API/appsettings.json` **đã được commit** và chứa mật khẩu SMTP Brevo cùng địa chỉ
Gmail cá nhân trong `FromEmail`. Cẩm nang §10 và §12 (D-3) đều gạch chỗ này.

Trước 25/09 thì đây **không phải ưu tiên** — nhưng nếu hội đồng mở repo trong buổi bảo vệ
thì nên biết trước là nó nằm ở đó. Xử lý sau kỳ bảo vệ: xoay mật khẩu Brevo, chuyển sang
biến môi trường.

---

## 13. Kế hoạch 3 ngày rưỡi — hạn 10h00 ngày 25/09

| Khi | Việc | Ước lượng |
|---|---|---|
| **21/09 tối** | Table 11 — sửa 6 dòng, thêm 2 dòng (mục 1) | 20 phút |
| | Xoá ghi chú *"Figure 2.1"* (mục 6) | 2 phút |
| | Thống nhất thứ Sáu / thứ Tư (mục 5) | 3 phút |
| | Table 10 `main` → `master` (mục 8) | 3 phút |
| | *"All 5 members"* → 4; Table 6 bỏ Chinh (mục 2) | 5 phút |
| **22/09** | **Table 8 — bỏ cột Chinh, phân lại 4 ô `D`, thêm 7 dòng tính năng mới** (mục 2) | 1–2 giờ |
| | Table 1 WBS — bỏ 4.1/4.3, thêm mục 10, tính lại tổng + Table 3 (mục 4) | 1 giờ |
| **23/09** | Table 5 + Table 7 — thêm Phase 7 và 2 deliverable (mục 3) | 40 phút |
| | Table 4 — viết 3 dòng *Triggered* (mục 9) | 20 phút |
| | Chạy `dotnet test --collect:"XPlat Code Coverage"`, sửa Table 2 (mục 7) | 30 phút |
| | Quyết định mobile, sửa đồng bộ RP1 + RP2 (mục 10) | 30 phút |
| **24/09** | Record of Changes cả RP1 và RP2 · `Ctrl+A` → `F9` · đọc print preview một lượt | 1 giờ |
| | **Dự phòng** cho việc phát sinh | phần còn lại |
| **25/09 sáng** | Nộp — **đừng để sát 10h00** | |

> Tối 21/09 làm hết **5 việc đầu** là gỡ xong lỗi 🔴 nguy hiểm nhất (Table 11) và 4 lỗi 🟠,
> tổng chưa tới 35 phút. Phần nặng thật sự chỉ có Table 8 và Table 1.
