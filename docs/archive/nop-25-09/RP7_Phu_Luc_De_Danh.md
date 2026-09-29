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

---

# BẢN CHỐT 25/09 — dán vào RP7

## A. Bổ sung bảng `Definition and Acronyms` (đầu tài liệu)

Các từ viết tắt **có xuất hiện trong RP7** nhưng bảng hiện tại còn thiếu. Chèn vào đúng vị trí alphabet:

| Acronym | Definition |
|---|---|
| BM | *Biểu mẫu* — standard form defined by Decision 543/QĐ-ĐHFPT (e.g. BM03, BM05) |
| CI | Continuous Integration |
| COI | Conflict of Interest |
| CRUD | Create, Read, Update, Delete |
| CV | Curriculum Vitae (academic profile of a researcher) |
| DTO | Data Transfer Object |
| EF Core | Entity Framework Core |
| HTTP | Hypertext Transfer Protocol |
| JSON | JavaScript Object Notation |
| KPI | Key Performance Indicator |
| OJT | On-the-Job Training |
| QĐ | *Quyết định* — Decision (as in Decision 543/QĐ-ĐHFPT) |
| RUP | Rational Unified Process |
| SMTP | Simple Mail Transfer Protocol |
| UTC | Coordinated Universal Time |
| WBS | Work Breakdown Structure |

## B. VII. Appendix → 1. Glossary

| Term | Vietnamese | Definition |
|---|---|---|
| Acceptance | Nghiệm thu | The final evaluation of a completed project by an acceptance council, after the final report is submitted. |
| Decision dossier | Hồ sơ quyết định | The per-project record of every formal decision, from proposal submission to settlement, each linked to its source record. |
| Disbursement tranche | Đợt giải ngân | A portion of the contract value released once its condition is met. The system records tranches; it does not transfer money. |
| Duplicate check | Rà trùng | Comparison of a proposal with existing projects by semantic similarity. The system reports candidates; research staff record the conclusion. |
| Effective deadline | Hạn hiệu lực | The original deadline adjusted by the latest recorded extension. |
| Minutes | Biên bản | The written record of a council meeting and its conclusion, drafted by the Secretary and locked by the Chair. |
| Principal Investigator | Chủ nhiệm đề tài | The faculty member who leads a research project and submits its proposal. |
| Research cycle | Đợt đăng ký | A time-bounded submission period for one research type. |
| Research field (track) | Lĩnh vực | The subject area under which proposals are grouped and reviewed within a cycle. |
| Research order | Đơn đặt hàng | A research topic requested by an ordering unit; applied research proposals register against it. |
| Research type | Loại đề tài | Basic or Applied research, each with its own funding ceiling. |
| Review council | Hội đồng | The group of Chair, Secretary, Opponents and Members that evaluates the projects of a round. Its size must be odd. |
| Review round | Vòng xét duyệt | An evaluation stage for one research field within a cycle: proposal review or acceptance. |
| Rubric | Bộ tiêu chí đánh giá | The set of scoring criteria whose maximum scores add up exactly to the template maximum (form BM03). |
| Semantic search | Tìm kiếm ngữ nghĩa | Search that ranks submitted proposals by meaning rather than by matching keywords. |

## C. VII. Appendix → 2. References (IEEE)

[1] FPT University, *Decision No. 543/QĐ-ĐHFPT on the regulation for managing scientific research and technology projects of FPT Education*, Hanoi, 14 May 2025.
[2] P. Kruchten, *The Rational Unified Process: An Introduction*, 3rd ed. Boston, MA, USA: Addison-Wesley, 2003.
[3] Object Management Group, *OMG Unified Modeling Language (OMG UML), Version 2.5.1*, Dec. 2017. [Online]. Available: https://www.omg.org/spec/UML/2.5.1
[4] M. Jones, J. Bradley and N. Sakimura, "JSON Web Token (JWT)," RFC 7519, IETF, May 2015. [Online]. Available: https://www.rfc-editor.org/rfc/rfc7519
[5] N. Provos and D. Mazières, "A future-adaptable password scheme," in *Proc. USENIX Annual Technical Conference, FREENIX Track*, 1999.
[6] Microsoft, "ASP.NET Core documentation." [Online]. Available: https://learn.microsoft.com/aspnet/core
[7] Microsoft, "Entity Framework Core documentation." [Online]. Available: https://learn.microsoft.com/ef/core
[8] The PostgreSQL Global Development Group, "PostgreSQL 16 documentation." [Online]. Available: https://www.postgresql.org/docs/16/
[9] Meta Open Source, "React documentation." [Online]. Available: https://react.dev
[10] Google, "Gemini API documentation." [Online]. Available: https://ai.google.dev/gemini-api/docs
[11] Cloudinary, "Cloudinary documentation." [Online]. Available: https://cloudinary.com/documentation
[12] Brevo, "Brevo developer documentation." [Online]. Available: https://developers.brevo.com
[13] National Foundation for Science and Technology Development (NAFOSTED). [Online]. Available: https://nafosted.gov.vn

Mỗi nguồn online thêm dòng *"Accessed: dd Month 2026."* — **bấm mở từng link rồi mới ghi ngày**.
eSM (§I.3.2) chưa có nguồn: có link chính thức thì thêm dòng [14], không có thì thôi — đừng bịa link.
