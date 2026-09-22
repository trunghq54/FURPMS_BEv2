# Phụ lục để dành cho RP7 — bảng viết tắt & nguồn tham khảo

> **Vì sao nằm ở đây mà không nằm trong RP4:** template RP4 chỉ có 4 mục
> (Record of Changes · System Design · Database Design · Detailed Design).
> Không có Glossary, không có References. Cẩm nang §1.1 phạt *"thêm mục không có
> trong mẫu"* — cùng cái bẫy đã làm hỏng lời khuyên về State Machine.
>
> Cẩm nang §1.2 có liệt kê *"bảng từ viết tắt"* vào **trang bắt buộc**, nhưng danh
> sách đó (Acknowledgement · User Guide · số trang · danh mục bảng/hình · bảng viết
> tắt) là **front matter của Final Report**, không phải của RP4.
>
> ⚠️ **Chưa xác minh:** tôi chưa đọc template RP7. Khi mở nó ra, xem RP7 **chứa lại**
> nội dung RP1–6 hay chỉ **trỏ tới**:
> - *Chứa lại* → để bảng viết tắt ở front matter RP7, RP4 đứng riêng không cần
> - *Chỉ trỏ tới* → cân nhắc thêm vào RP4 dưới dạng **front matter KHÔNG đánh số**,
>   đặt ngay sau List of Tables (không chạm cấu trúc mục đánh số ⇒ §1.1 không bắt được)

---

## 1. Bảng từ viết tắt

Cẩm nang §1.2 gạch *"có nhưng chưa sắp xếp theo thứ tự"* — bảng dưới **đã sắp alphabet**,
giữ nguyên thứ tự khi dán.

| Abbreviation | Meaning |
|---|---|
| API | Application Programming Interface |
| BM02, BM03, BM05, BM06, BM12, BM13 | Standard forms defined by Decision 543/QĐ-ĐHFPT |
| CORS | Cross-Origin Resource Sharing |
| DTO | Data Transfer Object |
| EF Core | Entity Framework Core |
| FK | Foreign Key |
| FURPMS | FPT University Research Project Management System |
| JWT | JSON Web Token |
| LLM | Large Language Model |
| PI | Principal Investigator |
| POCO | Plain Old CLR Object |
| QĐ543 | Decision 543/QĐ-ĐHFPT on research project management |
| REST | Representational State Transfer |
| SPA | Single-Page Application |
| UML | Unified Modeling Language |
| UTC | Coordinated Universal Time |

**Trước khi dán:** rà lại báo cáo, chữ viết tắt nào **không xuất hiện trong bài** thì
xoá khỏi bảng. Bảng liệt kê thuật ngữ không dùng cũng là một dạng "viết cho có".

---

## 2. Nguồn tham khảo

Cẩm nang §1.3 gạch: *"trích từ blog/nguồn không kiểm chứng, **không ghi ngày truy cập**"*.
⇒ Mỗi dòng **phải có ngày truy cập**.

Xếp theo mức chính danh, hai nguồn đầu quan trọng nhất:

| # | Nguồn | Vì sao cần |
|---|---|---|
| 1 | **Quyết định 543/QĐ-ĐHFPT** — quy định quản lý đề tài NCKH, Đại học FPT | Gốc của **mọi** ràng buộc nghiệp vụ trong bài: hội đồng số lẻ, rubric cộng đúng 100, bắt tải bản đã ký, hạn nộp báo cáo nghiệm thu 30 ngày (Điều 11.2.a) |
| 2 | **OMG Unified Modeling Language Specification, version 2.5.1** | Căn cứ khi bị hỏi về ký hiệu class/sequence diagram |
| 3 | Microsoft — ASP.NET Core documentation | |
| 4 | Microsoft — Entity Framework Core documentation | |
| 5 | PostgreSQL Global Development Group — PostgreSQL 16 documentation | |
| 6 | Google — Gemini API documentation | |
| 7 | Cloudinary — documentation | |
| 8 | Brevo — transactional email API documentation | |

⚠️ **Tôi không đưa URL cụ thể** vì không tra được lúc soạn. Tự lấy link chính chủ
(không lấy qua blog trung gian) và ghi ngày truy cập.

**Đừng trích:** Stack Overflow, Medium, viblo, TutorialsPoint, W3Schools, bài blog
so sánh framework. Đó đúng là loại nguồn §1.3 gạch.

---

## 3. Ctrl+A — hai thao tác khác hẳn nhau, đừng lẫn

| Phím | Làm gì | Dùng không? |
|---|---|---|
| `Ctrl+A` → **`F9`** | **Cập nhật field**: số hiệu hình/bảng, danh mục, cross-reference | ✅ Bắt buộc (§1.6) |
| `Ctrl+A` → **`Ctrl+C`** | **Chọn hết rồi chép** — dính header/footer/section break/style lạ | ❌ Đừng dùng khi gộp RP7 |

Rủi ro duy nhất của `F9`: trường `{ DATE }` nhảy về ngày hôm nay. Bấm xong kiểm trang bìa.

**Gộp RP1–6 vào RP7 thì dùng cách khác:** `Insert → Object → Text from File`, hoặc
copy từng chương rồi `Paste Special → Merge Formatting`. Không `Ctrl+A → Ctrl+C`.
