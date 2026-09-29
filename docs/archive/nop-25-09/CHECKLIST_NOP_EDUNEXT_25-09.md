# Checklist nộp Edunext — hạn 10:00 ngày 25/09/2026

> Theo thông báo P. Khảo thí FUHCM (tài liệu nộp Edunext, chuyên ngành KTPM) + 8 quy tắc trên trang nộp.
> File đã chuẩn bị sẵn: `D:\capstone\newroot\nop_edunext\Others\`

## Quy tắc trang nộp
- Tên file: không dấu cách, không ký tự đặc biệt (kể cả `[ ]`), **không mã nhóm** (`SU26SE053`, `GSU26SE70`)
- Một người đại diện upload · upload xong **tải lại từng file kiểm** · GVHD duyệt / quá hạn là **khoá, không sửa được**
- Thư mục: `Final Thesis + Reports` (tài liệu) · `Slides` · `Others` (code, DB, file khác)

## Final Thesis + Reports
- [x] Report1 … Report7 (PDF, đã đổi tên)
- [x] Report5_1_Unit_Test.xlsx · Report5_2_Integration_Test.xlsx · Report5_3_System_Test.xlsx
- [x] Capstone_Project_Register.pdf (1a) — nội dung giữ nguyên bản đã ký; bảng sinh viên mục 2 phải được điền
- [x] BienBan_Review1.pdf · BienBan_Review2.pdf (1d)
- [ ] **Team_Meeting_Notes.pdf** (1e) — in ra PDF các email ĐÃ GỬI thầy: Tóm tắt họp Tuần 5 (17/6), Tuần 9 (22/7), Tuần 12 (8/8), Báo cáo tỉ lệ đóng góp (24/9). Có ảnh chụp buổi họp trên Discord/Zalo thật thì gộp thêm. **Không dựng biên bản mới cho các tuần không có bản ghi.**
- [ ] **Official_Emails.pdf** (1f) — ảnh chụp thấy rõ người gửi/nhận/ngày/tiêu đề, mỗi email 1 trang Word + 1 dòng chú thích:
  [FINAL DECISION SEP490] Kết luận Hội đồng Review cấp Khoa/Bộ môn · [SUBMIT SEP490] Nhận xét của Hội đồng Review (19/8) ·
  [FE-STD SEP490] Thông báo lịch Review cấp Khoa/Bộ môn · [Khảo thí] Thời gian tổ chức BV lần 2 (10/9) · [Khảo thí] Lịch BV lần 2 (23/9).
  Bỏ email "Đăng ký đề tài Student Research Competition Platform" (8/5) — tên đề tài khác.

## Progress reports (1 file nén duy nhất)
- [ ] `Progress_Reports.zip` chứa `Team_Meeting_Notes.pdf` (email tóm tắt họp tuần 5/9/12 = báo cáo tiến độ)

## Slides
- [x] Defense_Slides.pdf

## Others
- [x] FURPMS_BEv2.zip · FURPMS_Web.zip (3c) — từ HEAD nhánh `dev`; bỏ `docs/`, `.claude/`, `.history`, `node_modules`, `bin`; `FromEmail` → noreply@example.com; không key, không mật khẩu SMTP
- [x] 01_create_tables.sql · 02_demo_data.sql (3a) — đã nạp thử vào DB trắng: 65 bảng, 9 tài khoản, 15 đề tài
- [x] Configuration_and_Demo_Accounts.txt (3b)
- [ ] **Video cài đặt + chạy thử** (2d, bắt buộc). Giải nén 2 zip → chép `appsettings.Development.json` của máy mình vào `FURPMS.API` (để có AI; KHÔNG mở file này khi quay) → `docker compose up -d` → `dotnet run --project FURPMS.API` → `npm install && npm run dev` → đăng nhập → 1 luồng. Được cắt đoạn chờ, không ghép cảnh từ lần chạy khác. Cắt bằng Clipchamp.

## Không nộp
- 1b, 1c, 1g — không làm với doanh nghiệp
- `Report3_Project Tracking.xlsx` — không có trong danh sách Khảo thí; bản hiện tại còn dữ liệu mẫu dự án khác (Blogs List, HuyNMG…) → nộp còn tệ hơn không nộp
- `Project Weekly Report_GroupName.xlsx` — tuỳ chọn; nếu làm thì CHỈ chép lại 3 tuần có email (5, 9, 12), đúng ngày trong email

## Cuối cùng
- [ ] Upload → tải lại kiểm → **Confirm Submission** → nhắn thầy duyệt

## Nếu bị hỏi "sao chỉ có biên bản vài tuần?"
Trả lời thật: *nhóm họp hằng tuần nhưng chỉ gửi tóm tắt bằng email ở một số tuần; tiến độ các tuần còn lại thể hiện qua
lịch sử commit Git và hai biên bản review.* Đừng nói là có mà không nộp.
