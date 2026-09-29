# Kế hoạch: đưa lịch họp hội đồng vào lịch cá nhân bằng `.ics`

> Soạn 25/09/2026. Chốt với user: **không** dùng Google Calendar API, **không** phụ thuộc tài khoản
> Google nào. Hệ thống chỉ gửi thư kèm file `.ics` (chuẩn RFC 5545) — Gmail, Outlook, lịch Apple
> đều đọc được.
>
> **Nguyên tắc số 1: FURPMS là bản chuẩn, lịch cá nhân chỉ là bản sao.** Tạo `.ics` hỏng hoặc gửi
> thư hỏng **không bao giờ** được làm hỏng việc tạo / sửa / xoá cuộc họp.

---

## 1. Hiện trạng (đã đọc code)

| Chỗ | Hiện trạng |
|---|---|
| `CouncilMeeting` | Có `ScheduledAt`, `DurationMinutes`, `Platform`, `Location`, `MeetingLink`, `Agenda`, `Status`. Có sẵn cột `CalendarEventId` **chưa ai dùng** |
| `CouncilMeetingService.ScheduleAsync / UpdateAsync / DeleteAsync` | ⚠️ **Không báo cho ai cả.** Staff dời giờ hay xoá buổi họp thì ủy viên không nhận được gì — đây là lỗ hổng có sẵn, kế hoạch này vá luôn |
| `CouncilService.SendInvitationsAsync` (`:375`) | Gửi thư mời tham gia hội đồng (nhận / từ chối), qua `IEmailService.SendAsync` |
| `CouncilService` `:580` và `:626` | Hai chỗ chuyển thành viên sang `Confirmed`: tự nhận lời, và Staff xác nhận hộ |
| `IEmailService.SendAsync` | **Chưa nhận file đính kèm.** 4 lớp implement: `SmtpEmailService`, `LogEmailService`, `TestNotifier` (test), một lớp trong `DeadlineReminderScannerTests` |
| FE | `MyMeetingsPage.tsx` (PI) · `MeetingsAgenda.tsx`, `MeetingsPage.tsx` (Staff) · `MeetingsPanel.tsx`, `ScheduleMeetingSheet.tsx` |

---

## 2. Quyết định thiết kế

### 2.1 Gửi `.ics` khi thành viên **đã nhận lời**, không gửi kèm thư mời

Thư mời của FURPMS là để **nhận / từ chối tham gia hội đồng**. Nếu kèm `.ics` ngay từ thư mời, Gmail
hiện nút *Có / Không* của riêng nó — ủy viên bấm *Không* trong Gmail sẽ **tưởng** mình đã từ chối,
nhưng FURPMS không hề biết. Hai nơi nhận lời, lệch nhau.

⇒ Thư mời giữ nguyên như bây giờ. `.ics` chỉ gửi **sau khi** thành viên `Confirmed` — lúc đó nó là
thông tin hậu cần (giờ, chỗ, link), không phải câu hỏi.

### 2.2 `METHOD:REQUEST`, người nhận ghi sẵn là đã chấp nhận

`ATTENDEE;PARTSTAT=ACCEPTED;RSVP=FALSE` ⇒ lịch hiện sự kiện là "đã tham gia", không giục bấm trả lời.
Dùng `REQUEST` (không dùng `PUBLISH`) vì lịch xử lý **cập nhật và huỷ** ổn định hơn.

⚠️ Cách Gmail hiển thị phải **kiểm bằng thư thật** ở bước 5 — không đoán.

### 2.3 Không cần migration

| Trường `.ics` | Lấy từ |
|---|---|
| `UID` | `meeting-{meeting.Id}@furpms` — cố định, không cần lưu |
| `SEQUENCE` | Số giây Unix lúc gửi — luôn tăng, lịch nhận bản mới đè bản cũ |
| `DTSTART` / `DTEND` | `ScheduledAt` / `ScheduledAt + DurationMinutes`, ghi dạng UTC có hậu tố `Z` |
| `LOCATION` | `Location` nếu họp trực tiếp, `MeetingLink` nếu online |
| `ORGANIZER` | Địa chỉ gửi của hệ thống (`EmailSettings.FromEmail`) |

⚠️ Phải xác nhận `ScheduledAt` đang lưu **UTC** (bước 1). Nếu là giờ địa phương thì phải đổi trước khi ghi.

### 2.4 Ai nhận

**Giai đoạn này:** thành viên hội đồng ở trạng thái `Confirmed`.
**Để sau (nếu kịp):** chủ nhiệm đề tài được xếp slot trong buổi họp.

---

## 3. Các bước

| # | Việc | File | Ước lượng |
|---|---|---|---|
| **1** | Kiểm `ScheduledAt` là UTC hay giờ địa phương (đọc FE gửi lên + một bản ghi trong DB) | — | 10' |
| **2** | `IcsBuilder` — hàm **thuần**, không phụ thuộc gì: dựng `VCALENDAR` cho `REQUEST` và `CANCEL`. Escape `\ , ; \n`, gập dòng ở 75 byte, xuống dòng `CRLF` | `FURPMS.Application/Common/IcsBuilder.cs` | 45' |
| **3** | `IEmailService.SendAsync` thêm tham số tuỳ chọn `attachments` (mặc định `null` ⇒ mọi chỗ gọi cũ không phải sửa). `SmtpEmailService` gắn `.ics` vừa làm `AlternateView` `text/calendar; method=REQUEST` vừa làm file `invite.ics`. Sửa chữ ký ở 3 lớp còn lại | `IEmailService.cs`, `SmtpEmailService.cs`, `LogEmailService.cs`, 2 lớp test | 40' |
| **4** | `IMeetingCalendarService` — một chỗ duy nhất lo việc gửi. **Bọc try/catch, không bao giờ ném lỗi ra ngoài**, lỗi thì ghi log | `FURPMS.Infrastructure/Services/MeetingCalendarService.cs` | 40' |
| **5** | Gắn vào 4 điểm:<br>• `:580` / `:626` nhận lời → gửi `REQUEST` cho **người đó**, mọi buổi họp `SCHEDULED` còn ở tương lai của hội đồng<br>• `ScheduleAsync` → gửi `REQUEST` cho các thành viên đã `Confirmed`<br>• `UpdateAsync` → nếu đổi giờ / thời lượng / địa điểm / link thì gửi `REQUEST` mới **kèm thông báo trong app** "Lịch họp đã thay đổi"<br>• `DeleteAsync` → gửi `CANCEL` **kèm thông báo** "Lịch họp đã huỷ" | `CouncilService.cs`, `CouncilMeetingService.cs` | 45' |
| **6** | Endpoint `GET /api/meetings/{id}/ics` (người trong hội đồng + Staff/Admin) — tải lại file nếu lỡ mất thư | `CouncilMeetingsController` | 20' |
| **7** | FE: hai nút ở màn chi tiết buổi họp — **"Thêm vào Google Calendar"** (link `calendar.google.com/calendar/render?action=TEMPLATE…`, không gọi API) và **"Tải .ics"** | `MeetingsAgenda.tsx`, `MyMeetingsPage.tsx`, màn hội đồng của ủy viên | 45' |
| **8** | Test (mục 4) | `FURPMS.Tests/Meetings/` | 45' |
| **9** | Kiểm tay bằng thư thật (mục 5) | — | 30' |
| **10** | Cập nhật tài liệu (mục 6) | — | 20' |

**Tổng ≈ 5–6 giờ.**

---

## 4. Test tự động

**`IcsBuilderTests`** (thuần, không DB):
- Có `BEGIN:VCALENDAR`, `METHOD:REQUEST`, `UID` cố định theo `meeting.Id`
- `DTSTART`/`DTEND` đúng UTC, hậu tố `Z`, `DTEND = DTSTART + DurationMinutes`
- Dấu `,` `;` `\` và xuống dòng trong tiêu đề / địa điểm được escape
- Dòng dài hơn 75 byte được gập đúng chuẩn (kể cả chữ tiếng Việt nhiều byte)
- `CANCEL` có `METHOD:CANCEL` và `STATUS:CANCELLED`, cùng `UID`
- Lần gửi sau có `SEQUENCE` lớn hơn lần trước

**`MeetingCalendarServiceTests`** (DB in-memory, email giả ghi lại thư):
- Nhận lời → đúng người đó nhận thư có `.ics`
- Dời giờ → mọi thành viên `Confirmed` nhận `REQUEST` mới **và** có thông báo trong app
- Xoá họp → nhận `CANCEL`
- Thành viên chưa nhận lời / đã từ chối → **không** nhận gì
- **Email ném lỗi → dời giờ vẫn thành công** (nguyên tắc số 1)

---

## 5. Kiểm tay — bắt buộc, vì cách Gmail hiển thị không đoán được

Cấu hình dev đã hứng thư của tài khoản seed về Gmail của user.

1. Tạo hội đồng, đặt lịch, gửi thư mời, cho một ủy viên nhận lời
2. Mở Gmail: thẻ sự kiện hiện đúng giờ **Việt Nam**, đúng địa điểm/link? Có nút *Có/Không* không?
3. Mở Google Calendar: sự kiện đã nằm trong lịch chưa?
4. Staff dời giờ → Google Calendar **tự đổi giờ** không?
5. Staff xoá buổi họp → sự kiện **tự biến mất** không?
6. Mở file `.ics` bằng Outlook nếu có máy

Bước nào khác mong đợi thì chỉnh `IcsBuilder` rồi chạy lại.

---

## 6. Tài liệu phải sửa khi xong

> ⛔ 25/09: tài liệu nộp ngày 25/09 **không** nhắc tính năng này. Chỉ cập nhật tài liệu sau khi code chạy và kiểm tay xong.

| Chỗ | Việc |
|---|---|
| **RP7** | Dùng cột **"link + .ics"** trong mục *"Nếu làm Google Calendar"* cuối `archive/nop-25-09/RP7_Soi_Chu.md` |
| **RP4 §3.5** sequence *Đặt lịch họp* | Thêm message gửi thư kèm `.ics` (tuỳ, không kịp thì bỏ qua) |
| `API_CONTRACT.md` | Endpoint `GET /api/meetings/{id}/ics` |
| `BUSINESS_RULES.md` | Luật: lịch cá nhân là bản sao; FURPMS là bản chuẩn |

**Câu trả lời chuẩn bị cho hội đồng — "sao không dùng Google Calendar API?":**
> Tạo sự kiện hộ người khác qua API cần quản trị Google Workspace của trường cấp quyền
> domain-wide delegation, hoặc app phải qua Google xác minh. Nhóm chọn chuẩn `.ics` (RFC 5545) vì
> chạy với mọi ứng dụng lịch, không phụ thuộc quyền của trường, và Google có sập thì lịch trong
> FURPMS vẫn nguyên.

---

## 7. Để sau

- Gửi `.ics` cho chủ nhiệm đề tài theo slot trình bày
- Đẩy việc gửi thư ra hàng đợi nền (xem `archive/nop-25-09/RP7_Gop_Bai.md` mục 10)
