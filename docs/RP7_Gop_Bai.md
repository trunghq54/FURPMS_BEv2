# RP7 — Final Report: cách gộp RP1–6 vào

> **Hạn: 10h00 ngày 25/09/2026.** Soạn 21/09.
> Mục 8 là thứ tự làm theo ngày — đọc mục đó trước nếu đang gấp.

---

## 0. Một điều anh nói đúng, và nó đổi thứ tự ưu tiên

> *"có khi thầy cô còn chỉ cho a.i đọc mỗi cái này là đủ"*

Đúng, và hệ quả quan trọng hơn anh nghĩ:

**RP7 là nơi mọi mâu thuẫn giữa RP1–6 va vào nhau.**

Ví dụ thật đang có:

| Ở file riêng | Trong RP7 |
|---|---|
| RP2 Table 11 ghi `MS SQL Server` | Nằm ở **§II.6** |
| RP4 kiến trúc ghi PostgreSQL | Nằm ở **§IV.2** |
| → hai file khác nhau, có thể lọt | → **cách nhau vài trang trong cùng một file.** Bất kỳ ai đọc, người hay AI, đều thấy |

Tương tự: RP1 nói *semantic search* ở 5 chỗ, RP2 Table 1 tính 10 man-day cho nó, RP4 không có
sơ đồ nào cho nó → trong RP7 ba chỗ đó nằm cùng một tài liệu.

⇒ **Sửa RP1–6 cho khớp nhau không phải việc phụ nữa, nó là điều kiện để RP7 không tự mâu thuẫn.**
Đặc biệt **Table 11 của RP2** (`RP2_Sua_Chu.md` mục 1) — sửa mất 20 phút, để lại thì thành mâu
thuẫn hiển nhiên trong RP7.

---

## 1. Copy-paste hay liên kết? → **Copy-paste**, và làm một lần duy nhất

| Cách | Đánh giá |
|---|---|
| **Copy-paste** một lần, gần cuối | ✅ **Chọn cách này** |
| `INCLUDETEXT` field (liên kết sống) | ❌ **Nguy hiểm** — xem dưới |
| Master Document / Subdocuments | ❌ Tính năng nổi tiếng làm hỏng file Word. Đừng đụng |

### Vì sao không dùng liên kết sống

Anh **bắt buộc** phải `Ctrl+A` → `F9` để đánh lại mục lục, số hình, số bảng, cross-reference.

Nếu RP7 dùng `INCLUDETEXT` thì `F9` sẽ **kéo lại nội dung từ file gốc** — mọi chỉnh sửa anh làm
bên trong RP7 bị ghi đè. Hai thao tác này xung khắc trực tiếp với nhau.

Thêm nữa: liên kết đứt khi file gốc đổi tên hoặc đổi thư mục. Nộp bài xong thầy mở trên máy khác
là trắng trang.

### Hệ quả về thứ tự làm

Mỗi lần sửa RP1–6 là phải chép lại vào RP7. ⇒ **Đóng băng RP1–6 trước, gộp RP7 sau.**
Nhưng phần **chỉ có ở RP7** thì làm ngay được, không chờ ai — xem mục 4.

---

## 2. ✅ ĐÍNH CHÍNH LỚN — cấu trúc **khớp** RP1–6, không phải lệch

> **Bản trước của mục này SAI hoàn toàn.** Tôi dựng nó từ **Mục lục** của template.
> Mục lục đó là **field đã cũ**, không khớp với thân bài. **Thân bài mới là chuẩn.**
>
> | | ToC template | **Thân bài template** |
> |---|---|---|
> | §II.3 | Master Schedule | **Project Deliverables** |
> | §II.4 | Project Organization | **Responsibility Assignments** |
> | §III.1 | Overall Description | **Product Overview** |
> | §III.5 | Other Requirements | **Requirement Appendix** |
> | §IV | 5 mục phẳng | **3 mục, có mục con** |
> | §V | 4 mục | **5 mục** |
>
> Bấm `Ctrl+A` → `F9` là ToC tự sinh lại theo thân bài. Không phải sửa tay heading nào.

### Bản đồ thật — từ thân bài template

| RP7 | Lấy từ | Khớp? |
|---|---|---|
| **§I** Project Introduction (1 Overview · 2 Product Background · 3 Existing Systems · 4 Business Opportunity · 5 Software Product Vision · 6 Project Scope & Limitations) | RP1 Part II | ✅ **1:1** |
| **§II** (1 Overview *[1.1 Scope & Estimation · 1.2 Project Objectives · 1.3 Project Risks]* · 2 Management Approach *[2.1 Project Process · 2.2 Quality Management · 2.3 Training Plan]* · 3 Project Deliverables · 4 Responsibility Assignments · 5 Project Communications · 6 Configuration Management *[6.1 Document · 6.2 Source Code · 6.3 Tools & Infrastructures]*) | RP2 | ✅ **1:1 tuyệt đối** |
| **§III** (1 Product Overview · 2 User Requirements · 3 Functional Requirements *[3.1 System Functional Overview · 3.2 Feature…]* · 4 Non-Functional Requirements *[4.1 External Interfaces · 4.2 Quality Attributes]* · **5 Requirement Appendix** *[5.1 Business Rules · 5.2 Common Requirements · 5.3 Application Messages List · 5.4 Other]*) | RP3 | ⚠️ đối chiếu §5.x |
| **§IV** (**1 System Design** *[1.1 System Architecture · 1.2 Package Diagram]* · **2 Database Design** · **3 Detailed Design** *[3.x → 3.x.1 Class Diagram · 3.x.2 Sequence Diagram]*) | RP4 | ✅ **1:1 tuyệt đối** |
| **§V** (1 Scope of Testing · 2 Test Strategy *[2.1 Testing Types · 2.2 Test Levels · 2.3 Supporting Tools]* · 3 Test Plan *[3.1 Human Resources · 3.2 Test Environment · 3.3 Test Milestones]* · 4 Test Cases · 5 Test Reports) | RP5 | ⚠️ **5 mục**, đối chiếu kỹ |
| **§VI** (1 Deliverable Package · 2 Installation Guides *[2.1 System Requirements · 2.2 Installation Instruction]* · 3 User Manual *[3.1 Overview · 3.2 Workflow 1 · 3.3 Workflow 2…]*) | RP6 | ⚠️ đối chiếu |

### §IV — **bỏ hai việc tôi bảo làm**

| Tôi từng nói | Thật |
|---|---|
| *"Database Design phải chuyển xuống cuối"* | ❌ **Không.** Thân bài để `2. Database Design` **trước** `3. Detailed Design` — **đúng như RP4 đang có** |
| *"Gom Tables 3–9 ra §IV.4 riêng"* | ❌ **Không.** Thân bài ghi rõ ở 3.1: *"It includes **Class Diagram, Class Specifications, and Sequence Diagram(s)**"* — Class Specifications nằm **trong từng mục 3.x**, **đúng chỗ đã đặt trong RP4** |

⇒ **§IV dán thẳng, không đụng gì.** Đây là chương **dễ nhất**, không phải khó nhất.

### §VI.1 — template cho sẵn bảng 9 dòng

| No. | Deliverable Item |
|---|---|
| 1 | Schedule/Task Tracking |
| 2 | Project Backlog |
| 3 | **Source Codes** |
| 4 | Database Script(s) |
| 5 | Final Report Document |
| 6 | Test Cases Document |
| 7 | Defects List |
| 8 | Issues List |
| 9 | Slide |

Template ghi *"can customize or delete if not using"* ⇒ được bỏ dòng không có, nhưng **phải điền
cột Description**.

> ⚠️ Dòng 3 **Source Codes** là lý do mục 12 của bảng §14 (secret key) không còn bỏ qua được:
> nhóm nộp cả mã nguồn, mà `appsettings.json` đang có mật khẩu SMTP Brevo thật.

### Trang bìa — ba trường mỗi người, không phải hai

Template: `<Member name><RollNo><Student code>` — **ba** trường.
Bìa hiện tại đang là *tên – vai trò – SE170589*. Kiểm xem `RollNo` và `Student code` ở trường mình
là hai thứ khác nhau không.

Template ghi `- Hanoi, <month>/<year> -`; bìa của nhóm để **Ho Chi Minh** — đúng, giữ nguyên.

---

## 3. 🔴 Xoá dữ liệu mẫu của dự án khác

Bảng `Definition and Acronyms` trong template đang chứa:

| Acronym | Definition | |
|---|---|---|
| **PWM** | **Psychology website** | ❌ **của dự án khác** — xoá ngay |
| **AWS** | Amazon Web Services | ❌ FURPMS không dùng AWS (dùng Railway + Vercel + Cloudinary) |
| PM | Project Manager | ❓ Nhóm có chức danh *Leader*, không có *PM* |
| BA | Business Analysis | ❓ Có xuất hiện trong bài không? |
| SPMP | Software Project Management Plan | ❓ RP2 tên là *Project Management Plan* |
| BR, ERD, GUI, SDD, SRS, UAT, UC, API | | ✅ giữ nếu có dùng |

**`PWM — Psychology website` nằm trong báo cáo hệ thống quản lý NCKH** là thứ lộ ngay lập tức.
Cẩm nang §1.3 gạch *"còn sót placeholder, nội dung mẫu"*.

**Quy tắc:** chỉ giữ từ viết tắt **thật sự xuất hiện trong RP7**. Thừa cũng là lỗi.

Cùng lúc, Ctrl+F tìm `[` để bắt mọi placeholder còn lại: `[Fill team's acknowledgement here…]`,
`[Provide the brief information…]`, `[Optional]`… — template đầy chỗ như vậy.

---

## 4. Phần **chỉ có ở RP7** — làm được ngay, không chờ RP1–6

| Mục RP7 | Nguồn | Trạng thái |
|---|---|---|
| **Trang bìa** | — | ⚠️ đang ghi **5 thành viên**, còn tên Chinh. Xem mục 5 |
| **Acknowledgement** | tự viết | ☐ chưa có bản nào |
| **Definition and Acronyms** | **`RP7_Phu_Luc_De_Danh.md` §1** | ✅ đã soạn sẵn 16 dòng, sắp alphabet |
| **§VII.2 References** | **`RP7_Phu_Luc_De_Danh.md` §2** | ✅ đã soạn sẵn 8 nguồn |
| §VII.1 Glossary *[Optional]* | — | Trùng với `Definition and Acronyms` ở đầu → **bỏ**, đừng làm hai lần |
| §VII.3 Others *[Optional]* | — | Bỏ nếu không có gì |

> **Quyết định hôm trước là đúng.** Template RP7 có `Definition and Acronyms` ở front matter
> (bắt buộc, có sẵn bảng) và `References` trong Appendix — còn template RP4 thì không có mục nào
> như vậy. Bảng viết tắt thuộc về đây, không thuộc RP4.
>
> Lưu ý: `Definition and Acronyms` ở đầu là **bắt buộc**, `Glossary` trong Appendix ghi *[Optional]*.
> Điền cái đầu, bỏ cái sau.

### Acknowledgement — gợi ý khung

Ba đoạn ngắn là đủ, đừng dài:

1. Cảm ơn **hai GVHD** (Đặng Ngọc Minh Đức, Phạm Minh Trí) — nêu cụ thể họ giúp gì, đừng viết chung chung
2. Cảm ơn **Phòng QLKH / bộ phận cung cấp nghiệp vụ** — nếu nhóm có gặp để khảo sát thì nói rõ,
   vì cẩm nang §3.1 gạch *"không khảo sát thực tế, chỉ ngồi tự nghĩ"*. Có gặp thật thì đây là chỗ ghi
3. Cảm ơn **Đại học FPT** đã cho phép dùng QĐ 543 làm căn cứ nghiệp vụ

---

## 5. Trang bìa — vẫn còn 5 thành viên

Đang ghi:

```
Hoàng Quốc Trung – Leader - SE170589
Đàm Mạnh Dũng – Member - SE170580
Nguyễn Lê Hoàng Chinh – Member - SE140506   ← đã rời nhóm
Nguyễn Tấn Phát – Member - SE184672
Nguyễn Trọng Thứ - Member - SE161992
```

Xử lý **giống hệt RP1 Table 1** — xem `RP1_Sua_Chu.md` mục 10. **Hỏi GVHD trước khi xoá.**
Trang bìa là chỗ nhạy nhất trong cả bộ tài liệu; đừng tự quyết.

Ba chỗ cần đồng bộ cùng lúc: **bìa RP7** · **RP1 Table 1** · **RP2 Table 8 (RAM)**.
Sửa một chỗ quên hai chỗ kia là tự tạo mâu thuẫn ngay trong RP7.

Tiện thể: `Ext Supervisor` đang trống — không có thì để trống, đừng xoá dòng (template đòi).

---

## 6. Ba bẫy kỹ thuật khi dán vào Word

### 6.1 Dán **cả chương một lần**, đừng dán từng mẩu

Cross-reference trong Word trỏ vào **bookmark ẩn**. Nếu dán caption và câu nhắc tới caption ở
**hai thao tác khác nhau**, bookmark không đi theo → `Error! Reference source not found`.

Dán trọn một chương trong **một lần Ctrl+C / Ctrl+V** thì caption và cross-reference đi cùng nhau.

**Cách dán:** bôi đen chương ở file gốc → `Ctrl+C` → trong RP7 `Home → Paste → **Paste Special →
Merge Formatting`**.
Đừng `Ctrl+V` thẳng (kéo theo style của file gốc, làm loạn style RP7).
Đừng `Ctrl+A → Ctrl+C` (kéo theo header, footer, section break).

### 6.2 Số hình sẽ đụng nhau — và tự hết sau `F9`

RP1 có Figure 1–2, RP2 có Figure 1–2, RP4 có Figure 1–20. Dán vào là RP7 có ba "Figure 1".

**Không phải sửa tay** — nếu caption được tạo bằng `References → Insert Caption` (field `SEQ`)
thì `Ctrl+A → F9` sẽ đánh lại liên tục 1, 2, 3… toàn tài liệu.

⚠️ Nhưng nếu ở file gốc có caption nào **gõ tay** *"Figure 3."* thay vì dùng Insert Caption thì
nó **đứng im** trong khi các caption khác đổi số → lệch. Rà trước khi dán: bấm vào từng caption,
nếu **cả cụm "Figure N" chuyển nền xám** thì đó là field thật, tốt.

### 6.3 Heading — **đính chính**: nhiều khả năng KHÔNG phải hạ cấp

Hôm qua tôi viết *"toàn bộ heading của RP1–6 tụt xuống một bậc"*. **Sai.**

Mở RP1 và RP7 cạnh nhau thì thấy cả hai dùng **cùng một quy ước**:

| Cấp | RP1 | RP7 |
|---|---|---|
| H1 | `II. Project Introduction` | `I. Project Introduction` |
| H2 | `1. Overview` | `1. Overview` |
| H3 | `1.1 Project Information` | `1.1 Project Information` |

⇒ **Khớp 1:1, không phải hạ cấp gì cả.** Nếu hạ cấp theo lời khuyên cũ là làm hỏng mục lục.

⚠️ Tôi mới nhìn được RP1 và RP7. **Kiểm RP2–RP6 trước khi dán:** mở `Xem → Ngăn Dẫn hướng`
(Navigation Pane), nhìn cây heading. Nếu `1. Overview` nằm thụt vào dưới `II. …` thì khớp —
dán thẳng, không đụng cấp.

### 6.4 RP7 **đã có sẵn khung heading** — dán đè, đừng dán thêm

Đây là điều làm mọi thứ dễ hơn hẳn. RP7 không phải trang trắng: nó đã có đủ
`1. Overview` → `1.1 Project Information` → `1.2 Project Team` → `2. Product Background` → …
mỗi mục kèm một đoạn hướng dẫn chữ xanh in nghiêng trong `[...]`.

**Cách dán đúng — một lần cho cả chương:**

1. **Ở RP1:** bôi đen từ `1. Overview` xuống hết `6. Project Scope & Limitations`
   (gồm cả Table 2). **Đừng lấy dòng `II. Project Introduction`** — RP7 đã có `I. Project Introduction` rồi
2. `Ctrl+C`
3. **Ở RP7:** bôi đen từ `1. Overview` xuống hết đoạn `[...]` cuối của mục 6
4. `Home → Paste → Paste Special → **Merge Formatting**` — dán **đè** lên vùng vừa bôi đen

Một thao tác, cả chương. Cross-reference đi cùng caption nên không gãy, `I. Project Introduction`
của RP7 giữ nguyên, và toàn bộ đoạn `[...]` bị thay luôn — không phải xoá tay từng cái.

**Đoạn `[...]` không phải chỗ điền chữ.** Nó là hướng dẫn của template, xoá hẳn. Riêng đoạn
`[Provide final project introduction information follow the template as part II in the Report #1]`
nằm ngay dưới `I. Project Introduction` — xoá luôn, không viết gì thay vào.

> Chữ **"final"** trong câu đó có ý: §I của RP7 phải là bản **cuối** của RP1, tức là bản
> **đã sửa xong**. Dán RP1 vào lúc chưa sửa là phải làm lại lần hai.

---

## 7. List of Figures / List of Tables

Mục lục template RP7 anh gửi **không có** LoF/LoT. Nhưng RP1–6 đều có, và cẩm nang §1.2 xếp
chúng vào **trang bắt buộc**.

**Cách an toàn:** đặt LoF và LoT làm **front matter không đánh số**, ngay sau `Table of Contents`,
trước `Acknowledgement`. Không chạm vào cấu trúc mục đánh số ⇒ §1.1 không bắt được, mà §1.2 vẫn đủ.

Đây đúng nguyên tắc đã dùng cho bảng viết tắt hôm trước.

---

## 8. Thứ tự làm — 3 ngày rưỡi

### 🔴 Việc quan trọng nhất: **chạy thử một chương TỐI NAY**

Đừng để tới 2h sáng 24/09 mới phát hiện caption không đánh lại số, hoặc heading sai cấp.

**Dán thử §I (RP1 → RP7), rồi đi trọn một vòng:** hạ cấp heading → `Ctrl+A` → `F9` → xem mục lục,
xem số hình, xem có `Error! Reference source not found` không.

Một chương mất ~30 phút. Nếu có vấn đề, anh còn 3 ngày để giải. Đây là khoản đầu tư tốt nhất
trong cả kế hoạch.

### Lịch

| Khi | Việc |
|---|---|
| **21/09 tối** | ▸ **Chạy thử dán §I** (30 phút) ← làm đầu tiên<br>▸ Xoá `PWM` / `AWS` khỏi bảng acronym (2 phút)<br>▸ Dán bảng viết tắt + References từ `RP7_Phu_Luc_De_Danh.md` (10 phút)<br>▸ 5 việc nhanh của RP2 ở `RP2_Sua_Chu.md` mục 13 (35 phút) |
| **22/09** | ▸ Viết Acknowledgement (30 phút)<br>▸ RP2 Table 8 + Table 1 — phần nặng nhất (2–3 giờ)<br>▸ Hỏi GVHD về trang bìa / danh sách thành viên |
| **23/09** | ▸ Xong nốt RP1 và RP2<br>▸ **Đọc RP3, RP5, RP6** đối chiếu cấu trúc với §III/§V/§VI (mục 2)<br>▸ **Tối: ĐÓNG BĂNG RP1–6.** Từ lúc này không sửa file gốc nữa |
| **24/09** | ▸ **Gộp RP7 trong một lượt** — dán 6 chương, mỗi chương một lần Ctrl+C<br>▸ Hạ cấp heading từng chương<br>▸ Đảo thứ tự §IV.3 / §IV.5, gom Tables 3–9 vào §IV.4<br>▸ `Ctrl+A` → `F9`, đọc **print preview** trọn một lượt |
| **25/09 sáng** | ▸ Kiểm lần cuối, nộp. **Đừng để sát 10h00** |

> **Nếu hụt giờ:** cắt theo thứ tự — §VII.3 Others → §VII.1 Glossary (trùng front matter) →
> Acknowledgement rút ngắn còn 1 đoạn. **Không cắt** phần gộp chương, cũng không cắt `F9` cuối.

---

## 9. Sau khi gộp — kiểm 6 thứ

| ☐ | Kiểm |
|---|---|
| ☐ | Ctrl+F tìm `[` — hết placeholder `[Fill…]`, `[Provide…]`, `[Optional]` |
| ☐ | Ctrl+F tìm `Error!` — không còn cross-reference gãy |
| ☐ | Ctrl+F tìm `SQL Server` — **phải 0 kết quả** |
| ☐ | Ctrl+F tìm `Teams`, `Outlook`, `Calendar`, `Azure` — phải 0 kết quả |
| ☐ | Số hình chạy liên tục từ 1, không lặp, không nhảy |
| ☐ | Mục lục khớp heading thật, số trang đúng |

Bốn dòng đầu bắt gần hết lỗi kiểu *"tài liệu không khớp phần mềm"* — nhóm lỗi §1.5 đã khiến
RP4 phải làm lại.
