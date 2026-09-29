# Lưu trữ — hồ sơ sửa báo cáo, đã nộp 25/09/2026

> Cổng Edunext đóng 10:00 25/09/2026, nhóm đã nộp đủ. Bảo vệ lần 2: **04/10/2026**.
> Các file dưới đây là **hướng dẫn sửa chữ trong báo cáo `.docx`** — đã dùng xong, cất ở đây để
> tra lại khi hội đồng hỏi "báo cáo ghi X, sao code làm Y?". **Không sửa tiếp các file này**;
> báo cáo đã nộp là bản chốt.
>
> Gói nộp thật nằm ở `D:\capstone\newroot\nop_edunext\` (không nằm trong repo).

| File | Dùng để tra gì |
|---|---|
| `CHECKLIST_NOP_EDUNEXT_25-09.md` | Đã nộp những gì, ở mục nào trên Edunext, file nào chứa gì (SQL, zip nguồn, key chấm) |
| `RP7_Soi_Chu.md` | **Mở đầu tiên khi bị hỏi về báo cáo.** 5 lượt soi RP7 — các chỗ đã sửa, và các chỗ **cố ý để nguyên** kèm lý do |
| `RP7_Gop_Bai.md` | Cách gộp RP1–6 vào RP7; mục 10 là danh sách việc hoãn sau nộp |
| `RP7_Khung_Template.md` | Khung heading thân bài template RP7 (mục lục của template cũ, sai) |
| `RP7_Phu_Luc_De_Danh.md` | Bảng viết tắt, glossary, nguồn tham khảo IEEE — bản đã dán vào RP7 |
| `RP7_Acknowledgement.md` | Lời cảm ơn đã dùng |
| `Record_Of_Changes_RP1-4.md` | Record of Changes RP1–4 (chỉ ghi việc đã sửa thật) + đoạn giải thích quy trình RUP |
| `RP1_Sua_Chu.md` … `RP4_Sua_Chu.md` | Từng báo cáo sai gì so với code, đã sửa thành gì |
| `RP4_Hinh_Da_Sua.md` | 17 khối mermaid (class + sequence) dựng lại từ code cho RP4 |
| `RA_SOAT_BAO_CAO_RP1-7.md` | Đợt rà soát đầu tiên 16/08 — lịch sử |

Các chỗ **báo cáo đã nộp lệch với code** mà biết được sau khi nộp (để chuẩn bị câu trả lời, không sửa doc):

- Bảng phân quyền màn hình (RP3/RP7 Table 17) ghi *Semantic Search* cho Admin, Staff, PI, RC — menu
  web hiện chỉ hiện cho Faculty (`furpms-web/src/constants/nav.ts`, mục `nav.aiSearch`); backend
  thì cho mọi vai đã đăng nhập.
- Tìm kiếm ngữ nghĩa trả về đề cương **đã nộp của mọi PI** (tên + đoạn tóm tắt), backend không lọc
  theo vai — chuẩn bị câu trả lời nếu bị hỏi "có lộ ý tưởng không".
