# HANDOFF — trạng thái hiện hành (cập nhật 26/08/2026)

> **Đọc file này ĐẦU TIÊN** nếu bạn là người/AI mới tiếp nhận dự án. Nó thay cho
> `HANDOFF_Week10.md` (đã lỗi thời ở phần hạ tầng). Sau file này thì đọc `../CLAUDE.md` (quy tắc
> nghiệp vụ đánh số) và `KICH_BAN_DEMO.md` (bấm theo từng bước).
>
> 🔴 **25/08 — nhóm bị hội đồng cho BẢO VỆ LẦN 2.** Danh sách việc hiện hành nay nằm ở
> **`KE_HOACH_BAO_VE_LAN2.md`** (7 nhóm việc, có bảng hiện trạng đã xác minh bằng cách đọc code).
> Đọc file này để biết *hệ thống đang chạy thế nào*, rồi sang file kia để biết *phải làm gì tiếp*.

---

## 1. Hai thay đổi lớn nhất gần đây — đọc kỹ, dễ hiểu nhầm

### 1.1 Cơ sở dữ liệu: SQL Server → **PostgreSQL** (14/08)

**Vì sao đổi:** Railway (nơi deploy) không cung cấp SQL Server. Không phải vì Postgres tốt hơn.

**Việc phải sửa chỉ có 5 dòng** — vì thứ cho phép đổi DB là **EF Core**, không phải repository
pattern (repository ở đây mỏng, chỉ phơi `IQueryable<T>`; nó giúp test/mock chứ không giúp đổi DB):

| Sửa gì | Chỗ |
|---|---|
| `UseSqlServer` → `UseNpgsql` | `DependencyInjection.cs` |
| Cú pháp T-SQL `[cột]` → `"cột"` (3 chỗ) | `FURPMSDbContext.cs` — `HasFilter`, `HasComputedColumnSql`, `HasCheckConstraint` |
| Thêm package `Npgsql.EntityFrameworkCore.PostgreSQL` | `FURPMS.Infrastructure.csproj` |

Toàn bộ **19 migration cũ (T-SQL) đã bị xoá** và thay bằng **một migration nền cho Postgres**.
Không sửa tay dòng nào trong đó.

> ⚠️ **Nếu sau này cần đổi DB lần nữa:** ba thứ gây khó là raw SQL (`FromSqlRaw`), ghim kiểu dữ
> liệu (`HasColumnType`), và `DateTime.Now` (Npgsql ném lỗi vì `Kind=Local`). Codebase này
> **không có cái nào** — đã kiểm: 0 / 0 / 0 (cả 168 chỗ đều dùng `DateTime.UtcNow`).

### 1.2 Seeder **không còn** đổ dữ liệu demo ngoài môi trường Development

**Trước đây** seeder chạy y hệt ở mọi môi trường ⇒ deploy lên máy chủ là DB thật có ngay **9 tài
khoản demo, tất cả mật khẩu `password`, kể cả `admin@furpms.edu.vn`** — mà danh sách email đó nằm
trong `docs/` của repo. Ai đọc repo cũng đăng nhập được bằng quyền quản trị.

Nay `SeedAsync()` tách hai phần:

- **Bắt buộc (mọi môi trường):** vai trò · quản trị viên · danh mục · cấu hình · bộ tiêu chí
- **Demo (chỉ Development):** tài khoản mẫu · đề tài mẫu · kịch bản đã duyệt · reset mật khẩu demo

Mật khẩu quản trị ngoài Development lấy từ biến `SeedAdminPassword`; **không đặt thì sinh ngẫu
nhiên và ghi log MỘT LẦN**.

> ⚠️ Biến `SeedAdminPassword` chỉ có tác dụng **khi chưa có** tài khoản admin. Admin đã tồn tại thì
> seeder bỏ qua hẳn — muốn đổi phải xoá dòng admin trong DB rồi khởi động lại.

### 1.3 Hai lỗi Dũng (FE) báo 18/08 — đã sửa, ghi lại vì cả hai đều dễ tái phát

**(a) Tên file hiển thị sai — sửa ở BE.** Màn xem tài liệu của hội đồng và câu *"AI đã đọc file
đính kèm (…)"* hiện `02%20%C4%90e%CC%82%CC%80%20cu%CC%9Bo%CC%9Bng…` thay vì *02 Đề cương nghiên
cứu…*. **Không phải FE hiện sai:** DB lưu đúng chuỗi đó, vì file trên máy người nộp vốn đã mang tên
**bị mã hoá URL** (tải về từ link không kèm `Content-Disposition`) và dấu tiếng Việt ở dạng **NFD**
(macOS). BE chép thẳng `IFormFile.FileName` vào cột `original_file_name`.

Sửa bằng `FURPMS.Application/Common/FileNames.cs` — giải mã `%XX` (**chỉ khi tên không có khoảng
trắng**, nên tên thật kiểu `Giải ngân 50% đợt 1.pdf` không bị đụng), gom về NFC, bỏ đường dẫn và ký
tự điều khiển. Áp **cả lúc lưu lẫn lúc đọc**: đọc cũng chuẩn hoá vì các bản ghi đã nằm trên DB
Railway vẫn mang tên hỏng và **không có migration nào đi sửa dữ liệu cũ**.

Tiện thể gộp 3 bản sao validate upload trùng lặp vào `AssertUploadAllowedAsync` — nay trả về
`(tên đã chuẩn hoá, đuôi file)` và dùng chung cho cả 6 luồng upload.

**(b) Đổi vai bị văng ra "Bạn không có quyền" — sửa ở FE.** Triệu chứng **giống hệt** lỗi đã sửa
12/08 (xem `BACKLOG_Uu_tien.md`, mục "Đã sửa xong trong lượt này") nhưng **nguyên nhân khác hẳn**:
lần trước là khoá localStorage dùng chung; lần này là **react-router v7 bọc mọi thay đổi địa chỉ
trong `React.startTransition`**, trong khi zustand (`useSyncExternalStore`) không hoãn được ⇒ React
commit một lượt trung gian *(vai MỚI, địa chỉ CŨ)* và `RoleGuard` của trang cũ bắn
`<Navigate to="/unauthorized">` đè lên điều hướng về dashboard. Sửa bằng
`<BrowserRouter useTransitions={false}>` + đưa `<Suspense>` vào trong `AppLayout`. Chi tiết ở
`AGENTS.md` §3.5 của repo FE.

> ⚠️ Rút ra: thấy lại triệu chứng "đổi vai ăn 403 oan" thì **đừng cho là hồi quy của bản vá cũ** —
> đã có hai nguyên nhân độc lập cùng ra một màn hình.

### 1.4 🔴 Cloudinary CHẶN phát hành file `raw` — mọi tài liệu mới nộp đều không mở được (18/08)

**Triệu chứng:** nộp file xong, mở ra ăn **404 "File không còn trên storage"**. Nhưng file **cũ**
vẫn mở bình thường — nên rất dễ tưởng hệ thống vẫn ổn.

**Nguyên nhân:** Cloudinary chặn *delivery* các tài nguyên kiểu `raw` (mọi tài liệu ở đây đều là
.docx/.pdf ⇒ rơi hết vào diện này). URL CDN mà `CloudinaryFileStorage` dựng ra nay trả `401` kèm
`X-Cld-Error: deny or ACL failure`, BE dịch thành "file không còn trên storage". Tài sản upload từ
trước khi Cloudinary đổi mặc định vẫn phát hành được — đó là lý do lỗi chỉ lộ ra với file mới.

**Đã đo từng cách trước khi sửa** (cloud `dq6dp3g3v`, thư mục `furpms-dev`):

| Cách đọc | Kết quả |
|---|---|
| URL CDN trần | 401 |
| URL CDN + chữ ký `s--…--` (SHA1/SHA256, có/không version) | 401 |
| Upload kèm `access_mode=public` rồi đọc URL CDN | 401 |
| URL CDN của tài sản **không tồn tại** | 404 ⇒ chặn theo TỪNG tài sản, không phải cả tài khoản |
| **`api.cloudinary.com/v1_1/{cloud}/raw/download` có chữ ký** | **200, đúng nội dung** |

**Đã sửa:** `CloudinaryFileStorage.ReadAllBytesAsync` đọc qua endpoint `/download` có chữ ký thay
vì URL CDN (`SignedDownloadUrl`). Cách này **an toàn hơn** bản cũ: tài liệu không còn phát hành
công khai, ai có URL cũng không tải được — muốn lấy phải qua BE có `[Authorize]`.

> ⚠️ **Phải kiểm trên bản deploy.** Railway dùng **cùng tài khoản Cloudinary**, chỉ khác thư mục,
> nên nhiều khả năng đang dính y hệt. Cách kiểm nhanh: đăng nhập bản deploy → nộp một file bất kỳ →
> bấm mở lại. Ra 404 thì đúng lỗi này, deploy bản vá là hết. **Đây là lỗi chặn demo**: hội đồng
> không mở được đề cương thì không chấm được gì.

### 1.4b 🔴 Cloudinary lưu file dưới tên HỎNG `…/x.docx.docx"` — tài liệu nộp từ ~25/09 không mở được (29/09)

**Triệu chứng:** nộp đề cương báo thành công, nhưng hội đồng mở ra thấy *"Không tải được bản xem
trước"*, và AI hỗ trợ chấm ghi *"chưa có file đề cương đính kèm"*. Cột `storage_container` vẫn ghi
`Cloudinary` nên rất dễ tưởng lưu trữ ổn.

**Nguyên nhân (đã đo bằng upload thử):** `MultipartFormDataContent.Add(content, name, fileName)`
tự ghi thêm `filename*=utf-8''%22x.docx%22` — mã hoá luôn cả dấu nháy. Khoảng giữa 15/09 và 25/09
Cloudinary bắt đầu đọc `filename*`, lấy đuôi `docx"` và **nối vào public_id** ⇒ file nằm ở
`furpms-dev/proposals/{id}/{guid}.docx.docx"`, còn BE tìm `…/{guid}.docx`. File trước 15/09 vẫn đúng tên.

**Đã sửa (`CloudinaryFileStorage`):** tự dựng Content-Disposition chỉ có `filename="…"`; kiểm
`public_id` Cloudinary trả về, lệch là báo lỗi NGAY lúc nộp; khi đọc/xoá thử thêm tên hỏng ⇒ file đã
nộp trong khoảng lỗi **mở lại được, không phải nộp lại**. Đã chạy thật trên máy local (cùng cloud
`dq6dp3g3v`, cùng thư mục `furpms-dev` với Railway): nộp mới → tải về đúng từng byte; file tên hỏng
của bản deploy → tải được. Đề tài nào đã chạy AI lúc file chưa đọc được thì bấm **Chạy lại**.

> Backend thật hiện ở Railway project **`delightful-intuition`**, URL
> `furpmsbev2-production-04bd.up.railway.app` — KHÔNG còn là `furpmsbev2-production.up.railway.app`
> (project `abundant-endurance`) như các chỗ khác trong docs/`.env` ghi.

### 1.4c Thư gửi qua HÀNG ĐỢI NỀN — hết "Không kết nối được máy chủ" khi nộp/tạo (29/09)

**Triệu chứng trên Railway:** tạo tài khoản, nộp đề cương… chờ lâu rồi báo *"Không kết nối được
máy chủ"*, nhưng dữ liệu ĐÃ LƯU; bấm lại thì ăn lỗi trùng.

**Nguyên nhân:** `Notifier` gửi thư NGAY TRONG request, lần lượt từng người nhận; SMTP từ Railway
chậm/bị chặn nên mỗi lá chờ tới trần 10 giây ⇒ vài người nhận là quá 15 giây của axios.

**Đã sửa:** `IEmailService` giờ là `QueuedEmailService` — chỉ xếp hàng rồi trả về; `EmailDispatchWorker`
gửi ngầm bằng `SmtpEmailService` (vẫn ghi `email_logs` như cũ). Đo local: tạo tài khoản 2,1 s → 0,3 s,
thư vẫn `SENT`. **Đánh đổi:** hàng đợi trong bộ nhớ — thư chưa kịp gửi mà redeploy thì mất (thông báo
trong app vẫn còn). Thư có tới được hay không vẫn tuỳ SMTP của Railway — kiểm bằng `email_logs`.

**30/09 — Railway CHẶN SMTP** (587 và 2525 đều `FAILED … không phản hồi sau 10 giây`). Đã thêm đường
gửi qua **API HTTPS của Brevo**: có env `EmailSettings__BrevoApiKey` (`xkeysib-…`) thì gửi qua
`api.brevo.com`, không có thì SMTP như cũ. Hai bẫy đã gặp: (1) Brevo mặc định **chặn IP lạ** cho API
key — phải vào Brevo → Security → Authorized IPs → *Deactivate blocking* cho API keys, vì IP ra của
Railway không cố định; (2) lỗi của Brevo ghi nguyên văn vào `email_logs.error_message`, đọc ở đó trước.
Đã gửi thật từ local qua API → `SENT`. **Trên Railway cũng `SENT` (30/09 02:33 UTC)** — nhưng chỉ sau
khi bấm **⋮ → Redeploy**: Railway chụp biến lúc TẠO bản deploy, thêm biến trong lúc bản build từ
GitHub đang chạy thì bản đó không có biến (dấu hiệu: lỗi vẫn ghi `(SMTP)`).

### 1.4d Chuyên môn người chấm nay NHẬP ĐƯỢC ở form người dùng (30/09)

**Lỗ hổng tìm ra:** "đúng chuyên môn" khi lập hội đồng (`ReviewShared`, `CouncilCandidateService`) so
bảng `user_research_tracks` — nhưng bảng này **chỉ seeder điền**, không màn hình/API nào nhập được ⇒
trên deploy mọi người chấm thật đều "Chưa khai chuyên môn". Ô "Chuyên ngành" gõ tay ở Lý lịch khoa học
**không được dùng để so** (chữ tự do, không khớp được danh mục lĩnh vực).

**Đã sửa:** `CreateUserRequest`/`UpdateUserRequest` có `researchTrackIds` (không bắt buộc; khi sửa,
`null` = giữ nguyên, `[]` = xoá hết), `UserDto` trả kèm. Form tạo/sửa người dùng có ô tick "Lĩnh vực
chuyên môn"; khung chi tiết người dùng hiện lĩnh vực + **Lý lịch khoa học (chỉ xem)**. Cùng đợt: bật lại
**Kho tài liệu** (`/documents`) — ẩn từ 17/08 vì FE đọc `data.items` trong khi BE trả mảng nên luôn trống.

### 1.4e Tiến trình đề tài hiện ĐỦ LỘ TRÌNH từ lúc nộp (30/09)

- **Trước:** mỗi bước chỉ hiện khi đã có bản ghi ⇒ đề tài vừa nộp thấy 2 dòng. Nay `ProjectTimelineService.AddPlannedStagesAsync`
  thêm các bước chưa tới (`IsPlanned`, status `NOT_STARTED`, `DeadlineBasis` = điều kiện mở bước) — không bịa ngày.
  Không thêm khi đề tài đã đóng, hoặc khi có bước ĐỨNG SAU đã xong (bước bị bỏ qua thật thì không hiện "chưa tới").
- **Lỗi thứ tự đã sửa:** vòng NGHIỆM THU dùng chung bộ đếm với vòng xét duyệt ⇒ "Họp hội đồng nghiệm thu" đứng TRƯỚC
  "Ký hợp đồng". Nay chấm nghiệm thu có mã riêng `ACCEPTANCE_REVIEW`, xếp sau báo cáo tổng kết (order 82+).
- Panel lộ trình nay có ở: tab **Tiến trình đề tài** màn chi tiết đề cương (Staff) và trang chi tiết đề cương của **PI**
  — trước chỉ nằm trong chi tiết hợp đồng nên đề tài chưa ký hợp đồng không xem được.

### 1.4f Màn "Hội đồng & Chấm" + hộp thoại thêm thành viên + ô nhập tiền (01/10)

- **Dải mốc thời gian của vòng** (`RoundTimelineStrip`): mở vòng → hạn chấm → hạn họp hội đồng → chốt kết quả,
  hiện cho MỌI vòng kể cả vòng đã chốt (trước chỉ vòng còn mở mới có hạn chấm). Hạn họp = hạn ghi trên hội đồng,
  chưa ghi thì suy ra **15 ngày làm việc** từ ngày lập (đếm bỏ T7/CN, `ReviewBoardService.DeriveMeetingDeadline`).
  BE trả thêm `openedAt/closedAt/isScoringExtended` (vòng), `establishedAt/meetingDeadline` (hội đồng), `proposalId/piName` (đề tài).
- **Bấm tên đề tài trong vòng** → `ProposalQuickViewSheet` (chủ nhiệm, kinh phí, mục tiêu, xem file thuyết minh).
- **Thêm thành viên hội đồng**: vai chọn bằng 4 nút ở đầu; ứng viên là danh sách thẻ trong hộp thoại
  (`CouncilCandidatePicker`), người không chọn được dồn cuối. Logic cảnh báo chuyên môn + API giữ nguyên.
- **`MoneyInput`**: ô tiền có dấu chấm ngăn nghìn — bảng dự toán khi nộp đề cương, kinh phí đã chi ở báo cáo tiến độ.

### 1.4g Lịch báo cáo tiến độ, tạo hợp đồng, tiến trình gọn hơn (01/10)

- **Xem trước lịch báo cáo tiến độ** (`ProgressSchedulePreview`): hợp đồng từ đâu tới đâu, mặc định mấy kỳ
  (`ContractDto.DefaultProgressRounds` — có đặt hàng 2, không 1, QĐ543 Điều 10.1), mỗi kỳ hạn ngày nào. Phép chia
  giống hệt `GenerateScheduledRoundsAsync` (đã so bằng API thật: khớp từng ngày).
- **Lịch "khoá ngày" không phải lỗi**: hạn nộp ∈ [max(hôm nay, bắt đầu HĐ, bắt đầu kỳ), kết thúc HĐ]; giờ họp ∈
  [bây giờ, kết thúc HĐ]. Hộp thoại nay ghi rõ khoảng + lý do mốc dưới.
- **Chi tiết hợp đồng thiếu `ResearchType`** (Include bị sót) ⇒ trả `researchTypeName = null` trong khi danh sách
  có. Đã thêm Include.
- **Tạo hợp đồng**: đại diện Bên A HIỆN SẴN tên từ cấu hình (`GET /system-settings/contract-defaults`, Staff đọc
  được), ngày bắt đầu mặc định hôm nay.
- **Tiến trình**: bước đã xong chỉ hiện "Xong ngày …"; bỏ "Vòng N —" (số vòng đếm theo cả lĩnh vực) và "chưa đặt
  hạn họp"; bước theo điều kiện (giải ngân, quyết toán) không còn nhãn "Chưa đặt hạn"; bước lặp đánh số "lần N".
  Bước dự kiến 50/60/70 (báo cáo, sản phẩm, giải ngân) chạy song song — chỉ coi là bị vượt khi đã sang báo cáo tổng kết.
- **DataTable nhớ ẩn/hiện cột** theo `exportFileName` (localStorage, bọc try/catch); mô tả lĩnh vực cắt 2 dòng.

### 1.4h Phòng QLKH KHÔNG tự chấm tiến độ nữa — chỉ GHI NHẬN kết luận hội đồng (01/10)

Thầy góp ý: cán bộ phòng không có chuyên môn để cho Đạt/Không đạt. QĐ543 Điều 10.1 giao việc này cho **Hội đồng
đánh giá tiến độ do Trường tổ chức**. Đổi:

- `EvaluateAsync` bắt buộc có tài liệu `DocumentCategory = "PROGRESS_MINUTES"` (biên bản họp), nếu thiếu thì ném lỗi.
  Biên bản **không** được tính là "bản báo cáo của PI". Nhật ký quyết định ghi vai "Hội đồng đánh giá tiến độ
  (Phòng QLKH ghi nhận)".
- Tải biên bản: `POST /progress-reports/{id}/documents?kind=minutes`, chỉ Staff/Admin. Không có `kind` thì vẫn là
  file của PI như cũ.
- **Giữ** 3 mức Đạt / Đạt có điều kiện / Không đạt, vì đợt giải ngân 2–3 (Điều 16) khoá theo kết luận này. **Giữ** %
  hoàn thành nhưng ghi rõ là "PI tự đánh giá" (số PI tự khai trong BM06).
- **Giới hạn có chủ đích:** hệ thống KHÔNG quản lý thành viên/phiếu của hội đồng tiến độ (khác hội đồng xét duyệt và
  nghiệm thu). Biên bản là bằng chứng. Nếu bị hỏi thì nói thẳng đây là hướng mở rộng.
- Lỗi cũ lộ ra khi test: `ProgressReportSummaryDto` (API danh sách) **không có** `EvaluationResult` ⇒ danh sách chưa
  bao giờ hiện kết luận, và nút đánh giá hiện mãi. Đã chuyển 3 trường đánh giá lên Summary. Panel Staff nay ẩn nút khi
  đã có kết luận; `StatusBadge` tô màu PASS/CONDITIONAL/FAIL.
- **Số 0 đứng đầu** ("05"): sửa chung trong `components/ui/input.tsx` — xem chi tiết dưới.

### 1.4n Biên bản hội đồng: xuất BM04/BM12 + PI và Staff xem trên trang đề tài (03/10)

- **Xuất Word:** `DocumentExportService.ExportMinutesDocAsync(councilId, projectId)`, theo loại phiên.
  - **BM04 (xét duyệt):** thông tin chung, bảng thành viên, ý kiến chuyên môn/kinh phí, tổng điểm/phiếu/điểm TB,
    kết luận + kiến nghị.
  - **BM12 (nghiệm thu):** QĐ thành lập, vắng mặt theo điểm danh, ý kiến phản biện, phiếu Đạt/Không đạt,
    kiến nghị.
  - Chỉ xuất bản ĐÃ KHOÁ.
  - API `GET /api/review-scoring/councils/{id}/minutes/export-word?projectId=`; ai được tải do
    `CanReadMinutesAsync` quyết.
- **Xem biên bản:** `GET /api/review-scoring/projects/{projectId}/minutes`.
  - Được xem: Staff/Admin, PI của đề tài, thành viên hội đồng đó.
  - FE `CouncilMinutesCard` hiện ở trang đề cương của PI và tab "Biên bản hội đồng" trong trang đề tài của Staff.
  - Nút tải Word cũng có trong `MinutesPanel` khi biên bản đã khoá.
- **Thông báo:** trước đây khoá biên bản phiên XÉT DUYỆT không báo PI. `REVIEW_ROUND_CLOSED` chỉ bắn khi Staff đóng
  vòng thủ công, luồng biên bản thường không đi qua. Nay có `REVIEW_MINUTES_FINALIZED`, mức HIGH nếu cần
  hoàn thiện / không duyệt.
- **File không được trống khi Thư ký ghi sơ sài (lần 2):**
  - "Ý kiến thành viên" trống thì dựng từ phiếu chấm; kiến nghị trống thì lấy kiến nghị trong phiếu.
  - Luôn có PHỤ LỤC tổng hợp phiếu: tiêu chí / tối đa / điểm / nhận xét của từng người.
  - `Clean()` bỏ chuỗi là khoá dịch, vd `review.quickFillCriterionNote` do nút "Điền nhanh" lưu nguyên khoá
    trước khi sửa i18n.
  - `MinutesPanel` tự điền sẵn ý kiến từ phiếu cho biên bản mới.
  - Dữ liệu rác trên DB deploy (nếu có) sửa bằng 2 lệnh UPDATE thay khoá bằng câu tiếng Việt tương ứng.
- **Trình bày (lần 3):**
  - Bảng biên bản dùng `MinutesTable`/`MinutesRow`: độ rộng cố định ~9356 twip, tiêu đề in đậm căn giữa có nền,
    lặp tiêu đề khi sang trang, cột số căn giữa, mỗi dòng là một đoạn, cỡ 12.
  - Danh sách thành viên chỉ gồm người CONFIRMED; tổng số đếm từ danh sách đó.
  - Mục 2 có bảng điểm từng thành viên.
  - Cột "Về chuyên môn" chỉ ghi nhận xét chung; chi tiết từng tiêu chí ở phụ lục.
  - Ngày đầu biên bản là ngày họp.
- **BM12 (lần 4):** hội đồng nghiệm thu chấm bằng `AcceptanceEvaluations` (Đạt/Không đạt + lý do), không dùng phiếu
  điểm ⇒ thêm:
  - Bảng thành viên (mục 6).
  - Bảng sản phẩm + kết quả nghiệm thu (mục 8, từ `ProjectDeliverables`).
  - Bảng phiếu từng người (mục 10.1).
  - Lý do phiếu Không đạt làm ý kiến khi Thư ký không ghi.
  - Mọi bảng có `TableGap` phía sau và hàng tiêu đề `KeepNext`.
- **Số/ngày Quyết định thành lập hội đồng:** BE nhận từ lâu (`PUT /api/councils/{id}`) nhưng không màn nào cho
  nhập. Nay có `CouncilDecisionNoForm` ở đầu chi tiết hội đồng.
  - Lần 5: nhập được ngay khi tạo hội đồng, qua `CreateCouncilPackageRequest.EstablishmentDecisionNo`/`EstablishedAt`
    và 2 ô trong `CreateCouncilSheet`.
- **Phản biện trong BM12 (lần 5, theo QĐ543 Điều 12.3.b + Biểu mẫu 10):**
  - Mục 9.1/9.2 in phiếu nhận xét BM10 của từng phản biện: mức 1–5 từng nội dung kèm chữ (Yếu…Xuất sắc), nhận xét
    khác, đánh giá chung.
  - Phụ lục đổi thành "NHẬN XÉT CỦA PHẢN BIỆN", cột "Mức (1–5)".
  - Phiếu BM10 được lưu ở `proposal_review_scores` (rubric nghiệm thu id 1, 4 tiêu chí × 25). `Rating()` quy về
    thang 1–5.
  - **Luật mới:**
    - Hội đồng NGHIỆM THU phải có ≥1 Phản biện: `ReviewBoardService` chặn từ lúc tạo, FE mặc định 5 dòng
      CT/TK/PB/UV/UV.
    - `AssertQuorumAsync` không cho khoá biên bản khi phản biện chưa nộp BM10.
    - `ComputeTallyAsync` của nghiệm thu chỉ đếm phiếu BM11 (`AcceptanceEvaluations`), điểm TB = null.
  - Ảnh minh hoạ: `anh_giao_dien_moi/48_bm12_co_phan_bien_trang1-3.png`, `49_tao_hoi_dong_nghiem_thu.png`.
- **Giữ "Cần chỉnh sửa":** QĐ543 Điều 9.1 nói Hiệu trưởng phê duyệt dựa trên kết quả hội đồng "và các Hồ sơ đã
  được hoàn thiện". BM04 in kết luận này là "Đề nghị phê duyệt sau khi chủ nhiệm hoàn thiện đề cương".

### 1.4m Phản biện chỉ ở hội đồng nghiệm thu · lý do mời khác lĩnh vực được LƯU (03/10) — có migration

- **Vai Phản biện:**
  - QĐ543 Điều 8.2: hội đồng xét duyệt gồm Chủ tịch/Thư ký/thành viên. Điều 12.3.b: phản biện (BM10) chỉ ở
    hội đồng nghiệm thu.
  - `ReviewShared.AssertRoleFitsRound` chặn `Opponent` ở phiên không phải `ACCEPTANCE`, áp ở 4 đường: tạo trọn
    gói, thêm 1 người, đổi vai, thêm theo vòng.
  - FE `councilRolesFor(roundType)` chỉ hiện Phản biện ở phiên nghiệm thu. Sao chép hội đồng sang phiên xét
    duyệt thì Phản biện chuyển thành Thành viên.
  - Seeder demo không còn tạo phản biện cho hội đồng xét duyệt.
  - Dữ liệu cũ KHÔNG tự sửa.
- **Lý do mời người khác lĩnh vực:**
  - Trước đây form tạo hội đồng (hội đồng chưa có đề tài) làm BE bỏ qua kiểm tra ⇒ lý do bị bỏ, lúc gán đề tài
    lại bắt gõ lần hai.
  - Nay `AssertExpertiseAsync` so với lĩnh vực của PHIÊN khi chưa có đề tài và lưu lý do vào
    `CouncilMember.ExpertiseNote` (migration `AddCouncilMemberExpertiseNote`, tự chạy khi khởi động).
  - Gán đề tài thì dùng lại lý do đã lưu để ghi `EXPERTISE_OVERRIDE` vào hồ sơ quyết định của đề tài; PI cũng
    xem được qua "Tiến trình đề tài".
  - Danh sách thành viên hội đồng hiện "Mời ngoài lĩnh vực — lý do: …".
- Bộ chọn người tự cuộn vào tầm nhìn khi mở ở thẻ gần đáy form.

### 1.4l Tạo hội đồng / tạo hợp đồng gọn hơn, giữ hợp đồng đang mở khi tải lại (03/10, lần 2)

- **Tạo hội đồng:**
  - Mỗi thành viên là một thẻ (vai trò · tên · đơn vị/lĩnh vực · nhãn).
  - Bộ chọn người mở NGAY dưới thẻ, trong form: chế độ `compact` của `CouncilCandidatePicker`, cao `max-h-64`,
    tự cuộn. Ô nổi (`Popover`) cũ nằm ngoài khung form nên con lăn chuột bị khoá cuộn chặn.
  - Cảnh báo khác lĩnh vực thu còn một dòng + ô lý do (vẫn bắt buộc); lời giải thích mở bằng "Vì sao?".
- **`FormSheet`:** lớp trong của Radix ScrollArea là `display:table` nên chữ dài đẩy rộng form (thẻ tràn ngang, mất
  nút X). Đã ép về `block` cho MỌI form dạng sheet.
- **Tạo hợp đồng:**
  - `ApprovedProposalPicker` (thẻ + ô tìm) thay dropdown bị tràn vì tên đề tài dài.
  - Ngày kết thúc tự tính = ngày bắt đầu + `durationMonths` PI đăng ký; Staff sửa tay thì không ghi đè.
- **Màn Hợp đồng:** hợp đồng đang mở và tab nằm trên URL (`?contract=…&tab=…`, `replace`), tải lại vẫn ở đúng chỗ.

### 1.4k Góp ý test deploy 03/10: tạo hội đồng, AI điền thành viên, từ chối lời mời, AI chấm, biên bản

- **Tạo hội đồng** (`CreateCouncilSheet`): mỗi dòng có vai trò rồi đến nút "Bấm để chọn người". Nút mở `Popover`
  chứa `CouncilCandidatePicker` (ô tìm + thẻ). Người đã chọn ở dòng khác không hiện lại. Sheet rộng 640
  (`FormSheet.width`).
- **AI điền thành viên lúc nộp đề cương:**
  - BE trích xuất đúng.
  - FE trước đây bỏ qua CẢ danh sách nếu đã có ai (vd bấm "Điền dữ liệu mẫu"). Nay chỉ thêm người còn thiếu
    (so theo tên).
  - Email thành viên không bắt buộc nữa (BE vốn không cần; thuyết minh thường không ghi email).
- **Từ chối lời mời hội đồng:**
  - `CouncilMemberResponse.DeclineReason` nay được trả ra; trước đây có lưu nhưng không trả.
  - `RespondToMembershipAsync` báo cả phòng Staff (`COUNCIL_INVITATION_DECLINED`, HIGH).
  - Danh sách thành viên hiện vai trò tiếng Việt và lý do từ chối.
- **AI hỗ trợ chấm:**
  - Thẻ tự chạy BÙ phần còn thiếu. Trước đây đã có tóm tắt là không chạy gì, nên không bao giờ tự có gợi ý điểm.
  - Có hai nút "Tóm tắt lại" / "Gợi ý điểm lại".
  - Điểm gợi ý làm tròn theo `SCORE_DECIMAL_PLACES` ở cả BE (prompt + `Math.Round`) lẫn FE.
  - Thêm nút "Áp dụng tất cả gợi ý".
  - Khoá i18n `review.quickFill*` trước nằm nhầm namespace.
- **Biên bản:**
  - Bỏ mục "Nhận xét của thành viên" (đọc `ReviewerFeedback`, không màn nào cho nhập nên luôn trống).
  - Dưới điểm từng người hiện thêm nhận xét theo tiêu chí và kiến nghị.
  - Nút "Điền từ phiếu chấm" đổ vào "Ý kiến thành viên hội đồng" (BM04 II.1): tiêu chí kinh phí vào cột kinh phí,
    còn lại vào cột chuyên môn; kiến nghị vào ô Kiến nghị nếu ô đó còn trống.
- Bỏ "vòng N" khỏi biểu đồ dashboard (`AnalyticsController`) và dòng tóm tắt kết quả trong sổ quyết định.

### 1.4j 🔴 Deploy: đề tài Ứng dụng chỉ ra 1 đợt giải ngân + 1 kỳ báo cáo (01/10)

- **Giải ngân:** mốc (`disbursement_templates`) chỉ được seed cho loại có mã đúng `APPLIED`/`BASIC`, mà hai loại này
  chỉ tự tạo ở Development. Không màn nào cấu hình mốc ⇒ loại do nhóm tự tạo trên deploy không có mốc, và trước
  đây thiếu mốc thì mọi loại đều ra 1 đợt 100%. Nay thiếu mốc thì sinh theo Điều 16 dựa vào
  `RequireOrderingUnit` (4 đợt 30–30–30–10 / 1 đợt 100%).
- **Báo cáo tiến độ:** số kỳ cũng dựa vào `RequireOrderingUnit`. Loại "Ứng dụng" trên deploy phải bật "Yêu cầu
  đơn vị đặt hàng" ở Admin → Loại đề tài, nếu không vẫn chỉ có 1 kỳ.
- Hợp đồng đã sinh lịch giải ngân không tự đổi (`GenerateAsync` từ chối sinh lại). Muốn test lại thì tạo hợp đồng
  mới, hoặc xoá các đợt còn `PENDING` của hợp đồng đó.

### 1.4i Rà trùng còn 2 lựa chọn · một đường xin điều chỉnh · "vòng" → "phiên" (01/10)

- **Rà trùng** (thầy góp ý): chỉ còn `NOT_DUPLICATE` / `DUPLICATE`. Trước đây có cả `NEEDS_REVISION`, và **cả
  ba đều không tác động gì tới đề cương**: kết luận "trùng lặp" xong, đề cương vẫn đi tiếp vào vòng chấm. Nay
  `DUPLICATE` thì bắt buộc ghi lý do, đề cương chuyển `REJECTED`, đề tài chuyển `CANCELLED`, ghi
  `Proposal.RejectionReason`, và PI nhận thông báo mức HIGH. Chỉ từ chối được khi đề cương đang
  `SUBMITTED`/`REVISION_REQUIRED`. `NEEDS_REVISION` giữ trong hằng số để đọc dữ liệu cũ, API không nhận nữa.
- **PI đọc được lý do từ chối**: `RejectionReason` có trong API từ lâu nhưng không màn nào hiển thị. Nay
  `ProposalDetailPage` có khung đỏ. Giải thích của AI hiển thị qua `LiteMarkdown`: chỉ hiểu `**đậm**` và gạch đầu
  dòng, không render HTML.
- **Một đường xin điều chỉnh**:
  - Luồng `ChangeRequests` (5 loại, trong trang đề cương) duyệt xong **không có tác dụng gì** và trùng việc với
    `Amendments` (BM07). Đã gỡ khỏi giao diện PI; thay bằng thẻ trỏ sang "Điều chỉnh hợp đồng" khi đề cương đã duyệt.
  - Menu Staff `/change-requests` nay là `PendingAmendmentsPage`, dùng `GET /api/amendments/pending`
    (Staff/Admin) để duyệt hoặc từ chối ngay tại đó; từ chối phải ghi ý kiến.
  - `AmendmentService` nay báo Staff khi PI gửi đơn và báo PI khi đơn được duyệt hoặc bị từ chối; trước đây không
    báo ai.
  - Giữ 4 loại vì khớp 4 nhóm của BM07, mỗi loại có dòng giải thích hệ quả.
  - Code `ChangeRequest` BE và file FE cũ vẫn còn để đọc dữ liệu cũ.
- **"Vòng N" → "<Loại> · phiên N"** (`ReviewRoundNaming` BE, `utils/review-round.ts` FE):
  - `RoundNumber` đếm chung mọi loại trong một (đợt + lĩnh vực), nên mới có "Vòng 3" trong khi QĐ543 chỉ có 2
    hội đồng. Số phiên nay đếm riêng theo loại.
  - Vòng có kết quả hiện "Đã chốt" thay cho "Đạt".
  - Chữ "vòng" trong namespace `reviewBoard`/`roundDeadline` đã đổi thành "phiên"; tên khóa và mã DB giữ nguyên. Ô `type="number"` đang là "0" thì bôi đen khi
  focus; rời ô thì bỏ số 0 thừa. Cả ~20 form đều dùng `Input` này, đừng vá riêng từng form.

### 1.5 🔴 Rule #15 đã được VIẾT LẠI (25/08) — đọc trước khi đụng bất cứ chỗ nào có tiền

Câu cũ — *"Tài chính = minh chứng, hệ thống KHÔNG quản tiền… scope = **strip + ẩn nav/UI tiền**"* —
bị hiểu thành **"không được HIỆN tiền"**. Hậu quả: `Contract.TotalAmount` bị giấu khỏi mọi màn dù BE
vẫn trả về, và **hội đồng bảo vệ lần 2 bắt đúng chỗ này** (yêu cầu số 1: *"thể hiện rõ ngân sách
tương ứng cho các đề tài"*).

Nay tách hai khái niệm bị gộp nhầm:

| | |
|---|---|
| **CÓ — hiển thị đầy đủ** | dự toán duyệt (Điều 15) · trần loại đề tài (Điều 14) · **giá trị hợp đồng** · lịch giải ngân theo %/mốc · trạng thái từng đợt · quyết toán. Đây là **hồ sơ hành chính** của đề tài |
| **KHÔNG làm** | thanh toán · nối ngân hàng · thay sổ kế toán. `ActualAmount` là **ghi nhận lại** con số Phòng Tài chính báo, không tự tính |

⇒ Ẩn *cấu hình* tài chính thì đúng; ẩn *số tiền của đề tài* thì sai. Toàn văn ở `CLAUDE.md` #15.

**Hệ quả đang chạy trong code:** màn Hợp đồng có tab **"Ngân sách"**
(`GET /api/projects/{projectId}/budget`, xem `API_CONTRACT.md` §13.2b) · danh sách có cột **Giá trị
hợp đồng** + bộ lọc giai đoạn · đợt giải ngân hiện **% và số tiền** · KPI Thống kê hết 0.

> Một nguyên tắc phải giữ khi làm tiếp: **đợt đã đánh dấu chi mà chưa có `actualAmount` thì tổng là
> TẠM TÍNH theo kế hoạch, và màn hình phải nói ra**. Im lặng là để người đọc tưởng đó là số quyết
> toán — hệ thống không được tự quyết thay kế toán.

**Trạng thái công việc bảo vệ lần 2:** Sprint 0 và **Nhóm 1–6 ✅ xong** (Ngân sách · Deadline · Lưu
trữ quyết định · Cảnh báo điểm lệch · Chuyên môn người chấm · AI rà trùng lặp + bộ đo metric).
Còn lại **Nhóm 7** (tài liệu — nhóm tự sửa `.docx`) — xem `KE_HOACH_BAO_VE_LAN2.md` §12.

⚠️ **Sau khi deploy, ngoài backfill hồ sơ quyết định (§1.6) còn phải:** gọi
`POST /api/admin/reindex-embeddings` **một lần** để vector hoá kho đề cương đã có từ trước — không
chạy thì tab "Rà trùng lặp" báo *"chưa lập chỉ mục"* cho những đề tài cũ. Cần `GeminiAI:ApiKey` trên
môi trường deploy.

> Từ 26/08, **đề cương nộp MỚI tự vector hoá** (`EmbeddingWorker`, chạy nền), và worker còn quét bù
> lúc khởi động. Nên lệnh trên chỉ cần cho **kho cũ**, không phải chạy định kỳ.

### 1.6 🔴 SAU KHI DEPLOY phải chạy backfill hồ sơ quyết định (25/08)

Nhóm 3 thêm bảng `project_decisions` — sổ các quyết định đã ra với một đề tài (yêu cầu số 2 của hội
đồng). **Sổ chỉ bắt đầu ghi từ lúc tính năng lên.** Mọi đề tài đã chạy xong trước đó — gồm cả dữ liệu
thật đang nằm trên Railway — sẽ mở ra **hồ sơ trống trơn**, đúng lúc bảo vệ.

```bash
# 1. Xem trước, KHÔNG ghi gì
POST /api/admin/backfill-decisions?dryRun=true     # Admin
# 2. Chạy thật
POST /api/admin/backfill-decisions
```

**Chạy lại bao nhiêu lần cũng được** — nhận diện theo `sourceEntityType + sourceEntityId +
decisionType`, đã có thì bỏ qua. Đúng cả khi luồng thật đã ghi trước đó (đã kiểm: chạy lại sau khi
nộp một đề cương thật → thêm 0, đếm bản trùng trong DB = 0).

### 1.7 Đếm ngược tới hạn — **chỉ máy chủ được tính** (25/08)

Nhóm 2 làm xong thì lộ ra một lỗi rất dễ tái phát: thẻ *"Hạn sắp tới"* trên bảng điều khiển hiện
**"Còn 7 ngày"** trong khi tab **Sản phẩm** của chi tiết hợp đồng hiện **"Còn 6 ngày"** — cho **cùng
một sản phẩm**, trên cùng một phiên đăng nhập. Nguyên nhân: máy chủ chạy UTC, trình duyệt chạy giờ
máy người dùng (UTC+7); sát nửa đêm là lệch nguyên một ngày.

Cách chữa đã áp dụng: **bỏ hẳn phép trừ ngày phía giao diện.** Mọi DTO có hạn nay trả `daysLeft`
(âm = quá hạn, `null` = chưa đặt hạn), tính tập trung ở `FURPMS.Application/Common/DeadlineMath.cs`
qua `IClock` — nên công cụ tua thời gian tác động đúng vào đây y như bộ quét nhắc hạn qua email.
Khoá bằng `FURPMS.Tests/Deadlines/DaysLeftConsistencyTests.cs`.

> **Ngoại lệ duy nhất, đừng "dọn" nhầm:** `daysUntil()` bên FE vẫn còn và chỉ phục vụ **lịch họp**.
> Buổi họp là *cuộc hẹn*, không phải hạn nộp — nhãn "quá hạn 3 ngày" cho một buổi họp đã diễn ra là
> sai nghĩa, nên `MeetingsAgenda` cố ý **không** dùng `DeadlineBadge`.

### 1.9 Bốn lỗ hổng lộ ra khi user tự bấm thử luồng rà trùng lặp + hạn chấm (26/08)

User hỏi bốn câu sau khi đọc hướng dẫn test, cả bốn đều đúng là thiếu:

1. **Chốt kết luận rà trùng lặp không báo cho chủ nhiệm.** `DuplicateCheckService.ReviewAsync`
   chỉ ghi sổ quyết định, PI không hề biết đề cương mình bị đối chiếu — kể cả khi kết luận là
   "cần chỉnh sửa"/"trùng lặp" và họ phải làm gì đó ngay. Đã thêm `NotifyAsync` cho cả ba kết luận
   (kể cả "không trùng" — minh bạch cả tin tốt, không chỉ tin xấu).
2. **Hồ sơ quyết định không xem được trước khi có hợp đồng.** `DecisionDossierPanel` chỉ nằm trong
   tab hợp đồng — đề tài chưa qua vòng 1 (chưa có hợp đồng) không có chỗ nào xem chuỗi quyết định
   của chính nó, dù sổ đã ghi từ lúc nộp đề cương. Thêm tab "Hồ sơ quyết định" vào
   `ProposalReviewWorkspace` (màn Đề cương), dùng `proposal.projectId` — trường có sẵn ở BE
   (`ProposalDto.ProjectId`) nhưng FE chưa khai (đúng bẫy DTO chép tay, AGENTS §3.2).
3. **Không có cách nào xem tổng quan đề tài nào cần chú ý.** Phải mở từng đề cương một mới biết có
   bị cảnh báo trùng lặp không. Thêm `POST /api/proposals/duplicate-flags` (batch, so cosine trong
   bộ nhớ — kho nhỏ nên rẻ) + cột "Trùng lặp" trên danh sách Đề cương.
4. **Không có chỗ đặt hạn chấm ở màn quản lý vòng.** Luật hạn chấm (nhóm 2) chỉ lộ ra ở màn
   "Đề cương" của TỪNG đề tài — phải mở đúng đề tài rồi đúng vòng mới đặt được, trong khi
   "Hội đồng & Chấm" mới là màn Staff thực sự quản lý vòng. Thêm badge hạn + nút đặt hạn ngay trên
   thanh vòng của `ReviewBoardPage`; `ReviewBoardDto` nay trả kèm `scoringDeadline`/`isScoringOverdue`.

**Ba lỗi tự bắt được khi VIẾT TEST cho bốn việc trên — không lỗi nào lộ ra khi chạy tay:**
- `DuplicateCheckService.ReviewAsync` gọi `_db.LlmOutputs.Update()` cho một bản ghi CHƯA TỪNG được
  EF theo dõi (`Add()` mới đúng) → `DbUpdateConcurrencyException`. Không lộ ra khi chạy tay vì mọi
  lần thử trước đó đều gọi "giải thích AI" (tạo sẵn bản ghi) trước khi chốt kết luận — nhánh "chốt
  luôn không cần AI" (một nhánh hợp lệ, tài liệu ghi rõ) chưa từng được test.
- `ReviewBoardService`: `IDeadlineResolver.EffectiveManyAsync` chỉ trả về vòng **đã từng gia hạn**
  (nó đọc bảng `deadline_extensions`, không biết `ScoringDeadline` gốc) — thiếu bước rơi về hạn gốc
  khi tra không ra, nên MỌI vòng chưa từng dời hạn (đa số) hiện `null` dù rõ ràng đã có hạn.
- Badge hạn ở `ReviewBoardPage` quên truyền `daysLeft` — chỉ hiện ngày trơ ("20/10/2026") thay vì
  "Còn 55 ngày". Bắt được khi bấm thử trên trình duyệt, không phải qua API.

**Một khoá i18n bỏ sót:** tab mới thêm hiện nguyên literal `"staff.decisionsTab"` thay vì bản dịch —
quên thêm khoá. Cũng chỉ lộ ra khi mở màn thật.

> **Kiểm chứng đã chạy thật (26/08):** rà trùng lặp trên toàn kho demo lộ thêm hai đề tài trùng tên
> y hệt (0.9953) mà trước đó chưa ai để ý · badge hạn "Hội đồng & Chấm" hiện đúng "Còn 55 ngày" cho
> vòng đã dời hạn tới 2026-10-20 · tab "Hồ sơ quyết định" mở được cho đề tài CHƯA có hợp đồng, hiện
> đủ 2 quyết định · chốt kết luận "Không trùng lặp" sinh đúng thông báo `DUPLICATE_CHECK_REVIEWED`
> cho chủ nhiệm · **đã dọn sạch dữ liệu thử**. 323/323 test xanh (từ 314, cộng cả test hồi quy cho
> 3 lỗi bắt được ở trên).

### 1.8 Timeline từng đề tài không được lẫn vòng chung của đề tài khác (26/08)### 1.8 Timeline từng đề tài không được lẫn vòng chung của đề tài khác (26/08)

Vòng chấm thuộc chung `cycle_track`, còn đề tài tham gia qua bảng nối `project_round`. Truy vấn
timeline trước đây lọc mỗi `CycleTrackId`, nên một đề tài đã hoàn thành có thể hiện thêm vòng nghiệm
thu của đề tài khác cùng đợt/lĩnh vực. `ProjectTimelineService.AddReviewStagesAsync` nay chỉ lấy vòng
có `ProjectRound.ProjectId` đúng đề tài đang xem; test hồi quy
`VongCuaDeTaiKhacCungLinhVuc_KhongDuocXuatHienTrongTimeline` khoá lại lỗi này.

Kiểm lại trên DB demo: timeline `NCKH-2026-008` chỉ còn vòng xét duyệt và vòng nghiệm thu mà chính
đề tài tham gia; vòng nghiệm thu của `NCKH-2026-007` đã biến mất. Cổng kiểm hiện là **314/314 test**.

---

## 2. Chạy dự án

### Local

```bash
# BE
cd FURPMS_BEv2
docker compose up -d                 # PostgreSQL 16, cổng 5433
dotnet run --project FURPMS.API      # :5068 — tự Migrate + seed

# FE (cửa sổ khác)
cd ../furpms-web && npm run dev     # :5173
```

Tài khoản demo (chỉ có ở Development): `admin@furpms.edu.vn` · `staff.demo@…` · `pi.demo@…` ·
`pi2.demo@…` · `reviewer1..5.demo@…` — **mật khẩu tất cả là `password`**.

> ⚠️ Container tên `furpms-db-1` trùng tên với container SQL Server cũ. Máy còn container cũ thì
> `docker rm -f furpms-db-1` trước, không thì port map sai do Docker giữ cấu hình cũ.

Làm lại từ dữ liệu sạch:
```bash
docker exec -i furpms-db-1 psql -U postgres -c "DROP DATABASE IF EXISTS furpms"
docker exec -i furpms-db-1 psql -U postgres -c "CREATE DATABASE furpms"
```

### Bản deploy

| | |
|---|---|
| BE | Railway — `https://furpmsbev2-production.up.railway.app` (project `abundant-endurance`) |
| DB | PostgreSQL service **cùng project** (bắt buộc — mạng nội bộ `.railway.internal` không thông giữa hai project) |
| FE | Vercel — `https://furpms-web.vercel.app` (project `furpms-web`, tài khoản `trunghq54`, deploy từ **fork**) |
| Nhánh BE đang deploy | **`master`** (đổi 18/08; trước là `thu-nghiem/postgres`). Commit ở `dev` **không tự lên Railway** — phải merge sang `master` |
| Nhánh FE đang deploy | **`dev`** — repo gốc `github.com/immanhdung/FURPMS-Web`. `main` đi sau `dev` **154 commit** (dừng ở 15/07), deploy nhầm `main` là ra bản của tháng trước |

Fork mặc định lấy nhánh `main`, nên sau khi fork **phải đổi Production Branch sang `dev`** (GitHub →
Settings → Default branch, hoặc Vercel → Settings → Git). Biến trên Vercel: `VITE_API_BASE_URL` =
URL Railway + `/api` — không đặt cũng chạy vì `.env.production` đã commit sẵn giá trị đó, đặt ở
dashboard thì **đè lên file** (đổi URL BE khỏi phải commit).

Biến môi trường trên Railway (`:` trong JSON → `__`):

```
ConnectionStrings__DefaultConnection = ${{Postgres.DATABASE_URL}}
JwtSettings__SecretKey               (bắt buộc — khoá trong repo là khoá mẫu)
EmailSettings__FrontendUrl           = https://furpms-web.vercel.app   ← ĐẶT NGAY nếu chưa
                                       (mọi nút "bấm vào đây" trong email ghép từ biến này;
                                        bỏ trống là rơi về localhost:5173 ⇒ link chết với người nhận)
EmailSettings__SmtpUsername / __SmtpPassword
GeminiAI__ApiKey / __Model
Cloudinary__CloudName / __ApiKey / __ApiSecret / __Folder   (thiếu là tệp bay mỗi lần redeploy)
```

Ứng dụng **tự soi cấu hình** khi khởi động ngoài Development (`ProductionReadinessCheck`) và ghi
cảnh báo cho từng thứ còn thiếu. Đọc Deploy Logs là biết.

---

## 3. Bốn cái bẫy đã gặp thật khi deploy — đừng mất thời gian lại

| Triệu chứng | Nguyên nhân thật | Đã xử lý thế nào |
|---|---|---|
| FE: bấm menu thì chạy, **F5 giữa chừng hoặc dán link `/proposals/my` cho người khác → 404** | App dùng `BrowserRouter`; các đường dẫn đó chỉ tồn tại trong trình duyệt, trên đĩa không có file nào tên vậy. Rất dễ bỏ sót vì tự test toàn vào từ trang chủ | `vercel.json` rewrite mọi đường dẫn về `/index.html`. Rewrite chạy **sau** bước tìm file thật nên `/assets/*.js` vẫn được phục vụ đúng |
| `Couldn't set data source` + `KeyNotFoundException` | Biến vẫn là chuỗi **SQL Server**, Npgsql không hiểu `Server=`/`Data Source=` | `PostgresConnectionString` bắt sớm, báo rõ và chỉ chỗ sửa |
| `SocketException: Name or service not known` | Postgres ở **project khác** ⇒ DNS nội bộ không phân giải được | Phải đặt DB **cùng project** |
| Vẫn `Name or service not known` dù cùng project | Mạng nội bộ Railway mất **vài giây** mới sẵn sàng; app gọi `Migrate()` ngay dòng đầu | `DatabaseStartup` thử lại 10 lần × 3 giây; chỉ thử lại lỗi **mạng** |

`PostgresConnectionString.Resolve` chấp nhận **cả ba** dạng: URI (`postgresql://…` của
Railway/Render/Heroku), khoá=giá trị của Npgsql, và bộ biến rời `PGHOST/PGPORT/…`. Tự chọn SSL theo
tên máy chủ (`.internal` → tắt, công khai → bật).

---

## 4. Kiến trúc — điểm cần biết để không phá

- **N-tier:** Controller → Service → Repository → `FURPMSDbContext`. Không MediatR, không AutoMapper.
- **Logic nghiệp vụ nằm ở Service**, không nhét trong controller. Controller chỉ lấy danh tính từ
  claim rồi gọi service.
- **Mã lỗi ổn định:** mọi phản hồi thất bại kèm `errorCode` (xem `ErrorCodes`). Giao diện tra
  `errors.<mã>` trong bảng dịch, không có thì rơi về câu tiếng Việt máy chủ gửi kèm. Nhờ vậy chuyển
  đổi được từng phần.
  - Dùng `AppException(code, message, status, details)` khi cần mã **cụ thể**; ngoại lệ chuẩn
    (`KeyNotFoundException`…) vẫn chạy và nhận mã chung do middleware suy ra.
  - ⚠️ **401 = chưa/hết đăng nhập ⇒ giao diện TỰ ĐĂNG XUẤT.** Đừng dùng 401 cho lỗi nghiệp vụ.
    (Đã từng sai: gõ sai mật khẩu hiện tại khi đổi mật khẩu trả 401 ⇒ người dùng bị đá ra ngoài.)
- **Văn bản xuất ra ghim culture `vi-VN`** (`DocumentExportService.Vi`). Dấu `/` trong
  `"dd/MM/yyyy"` là *chỗ dành cho dấu phân cách của culture*, không phải ký tự cố định — không ghim
  thì cùng một hợp đồng xuất ở hai máy ra hai kiểu ngày.
- **Ngày tháng ở giao diện dùng `DD/MM/YYYY` dạng số**, không dùng `MMM` (tên tháng theo locale
  dayjs, mặc định là `en`).

---

## 5. Việc còn lại

### Bắt buộc trước khi bảo vệ

| Việc | Ai làm |
|---|---|
| ~~Đặt `EmailSettings__FrontendUrl` trên Railway~~ | ✅ **Xong** — kiểm 18/08 trên Railway: đã đặt `https://furpms.vercel.app`. ⚠️ Lưu ý có **hai** tên miền cùng sống: `furpms.vercel.app` và `furpms-web.vercel.app`. Biến đang trỏ cái thứ nhất; chốt hẳn một cái rồi sửa mọi doc theo, không thì link trong email dẫn về bản người dùng không dùng |
| ~~Deploy FE lên Vercel~~ | ✅ **Xong 14/08** — `furpms-web.vercel.app`, nhánh `dev`. Đã thử thật: đăng nhập admin vào được `/dashboard`, F5 giữa chừng vẫn đúng, không lỗi console, không API ≥ 400 |
| Tạo tài khoản thật cho từng vai trên bản deploy | Bạn — không còn tài khoản demo. DB deploy đang **rỗng**: 0 đợt, 0 đề cương, 0 hội đồng, 0 hợp đồng |
| Đi hết luồng chính trên bản deploy | Bạn |

### Backlog còn treo (`BACKLOG_Uu_tien.md`) — đều P2/P3, không chặn luồng chính

`P2-3` gợi ý tiêu đề lịch họp · `P2-6` màu ô nhận xét ·
`P2-9` phạm vi Admin sửa hồ sơ người khác *(cần bạn quyết, không phải việc code)* ·
`P3-1` Google Meet · `P3-3` đo token AI · `P3-4` `/ai/search` · `HD-7` hash file bản ký

> Tôi đã khuyên **bỏ hẳn** `P3-2`/`HD-9` (làm lại giao diện hợp đồng) — chụp màn rồi, màn đó là
> màn khá nhất hệ thống. Thầy chê **file Word**, đã sửa theo Nghị định 30/2020.

---

## 6. Cách kiểm tra nhanh mọi thứ còn chạy

```bash
cd FURPMS_BEv2 && dotnet build && dotnet test        # phải 323/323 xanh
cd furpms-web  && npx tsc -p tsconfig.app.json --noEmit && npm run build
```

> ⚠️ `npx tsc --noEmit` ở thư mục gốc FE **không kiểm gì cả** (`tsconfig.json` có `"files": []` +
> references). **Phải** dùng `-p tsconfig.app.json`.

### Quét giao diện bằng trình duyệt

Script nằm ở `.e2e/` trong repo FE (thư mục này **gitignore**, phải viết lại nếu clone mới).
Cách làm đúng — đã trả giá để rút ra:

1. **Lấy danh sách route từ `src/constants/nav.ts`**, đừng tự gõ. Tôi từng quét `/cycles`,
   `/tracks`, `/statistics`, `/proposals/my` — **không route nào tồn tại**, và trang 404 render
   sạch nên báo "OK". Kết quả sạch giả.
2. Khi bóc `nav.ts`, **bỏ qua dòng comment** — vài mục bị ẩn cố ý (`budget-categories`,
   `financial-config` theo rule #15; `documents` chưa dùng). Không bỏ thì báo 404 giả.
3. **Xoá `localStorage` giữa các vai.** Còn token cũ thì vào `/login` bị chuyển thẳng về
   `/dashboard`, kịch bản kẹt ở vai đầu tiên.
4. **Phân biệt bốn kết cục**, đừng chỉ hỏi "có lỗi không": *ok* · *404 route không tồn tại* ·
   *bị chặn quyền* · *lỗi thật*. Gộp lại là mất hẳn loại 2 và 3.
5. **Đừng `await r.text()` trong handler `response`** của Playwright — nó chặn luồng, làm mọi
   điều hướng sau đó treo.

Kết quả lần chạy 14/08: **40 route × 4 vai — 0 vấn đề** (local, PostgreSQL).

---

## 7. Nếu bàn giao cho AI khác — nói đúng những câu này

> Dự án FURPMS, capstone SU26SE053, quản lý đề tài NCKH theo **QĐ 543/QĐ-ĐHFPT**.
> BE .NET 8 + **PostgreSQL** (`D:\capstone\newroot\FURPMS_BEv2`, nhánh `thu-nghiem/postgres`).
> FE React 19 + Vite (`furpms-web`, nhánh `dev`).
>
> Đọc theo thứ tự: `docs/HANDOFF_HIEN_HANH.md` → `CLAUDE.md` (quy tắc nghiệp vụ #1–29, **không
> được tự đoán**) → `docs/KICH_BAN_DEMO.md`.
>
> Quy ước làm việc: mọi thay đổi phải **build xanh + test xanh** trước khi coi là xong; sửa code
> thì **cập nhật doc cùng lượt**; **không tự commit** trừ khi được yêu cầu; mọi kết luận nghiệp vụ
> phải **tra QĐ543** chứ không đoán.
