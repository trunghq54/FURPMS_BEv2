# Cẩm nang tránh lỗi khi làm Capstone Project — ngành Kỹ thuật phần mềm

> Bản `.md` của `Cam-nang-tranh-loi-Capstone-SE.pdf` (19 trang), gõ lại để đọc và tra cứu nhanh.
> Giữ nguyên nội dung và cách đánh mục của bản gốc.

## Mục lục

| § | Nhóm lỗi | Liên quan |
|---|---|---|
| 0 | 6 nguyên nhân khiến nhóm KHÔNG ĐẠT | — |
| 1 | TÀI LIỆU (Final Report) | RP1–RP7 |
| 2 | **SƠ ĐỒ & MÔ HÌNH HÓA** | RP3, RP4 |
| 3 | PHÂN TÍCH NGHIỆP VỤ | RP3 |
| 4 | HARDCODE & CẤU HÌNH | code |
| 5 | AI trong đề tài | RP3, RP4, code |
| 6 | KIỂM THỬ | RP5 |
| 7 | DEMO | buổi bảo vệ |
| 8 | TRẢ LỜI CÂU HỎI | buổi bảo vệ |
| 9 | QUY TRÌNH & LÀM VIỆC NHÓM | RP2 |
| 10 | DỮ LIỆU, BẢO MẬT & PHÁP LÝ | RP3, code |
| 11 | UI/UX | sản phẩm |
| 12 | **CHECKLIST TỰ KIỂM TRƯỚC BẢO VỆ** | — |
| 13 | 20 CÂU HỎI HAY GẶP NHẤT | — |
| 14 | **BẢNG TỰ ĐÁNH GIÁ RỦI RO** | — |
| 15 | Ba lời khuyên quan trọng nhất | — |

---

## 0. Đọc nhanh: 6 nguyên nhân khiến nhóm KHÔNG ĐẠT

| # | Nguyên nhân | Mức độ phổ biến |
|---|---|---|
| 1 | Demo lỗi ở mainflow, hoặc không demo được hết luồng chính trong thời gian quy định | **Rất cao** |
| 2 | Hardcode các tham số nghiệp vụ (phí, %, ngưỡng, cấu hình) | **Rất cao** |
| 3 | Tài liệu sai/thiếu, không dùng template chính thức của trường | **Rất cao** |
| 4 | Nghiệp vụ không sát thực tế, thiếu ràng buộc ở các tình huống ngoại lệ | Cao |
| 5 | Không tiếp thu góp ý của các lần Review đánh giá | Cao |
| 6 | Không được GVHD duyệt ra bảo vệ / vắng mặt / thành viên đóng góp quá ít | Trung bình |

> Phần lớn nhóm rơi vào kết quả **"đạt nhưng phải chỉnh sửa và nộp lại tài liệu"** — sản phẩm
> chạy được nhưng tài liệu kéo điểm xuống. Đây là phần **dễ sửa nhất** nhưng **bị bỏ bê nhiều nhất**.

---

## 1. Nhóm lỗi TÀI LIỆU (Final Report)

Nhóm lỗi bị nhắc nhở nhiều nhất.

### 1.1. Không dùng template chính thức của trường

**Biểu hiện:** tài liệu khác hẳn template trên FLM — **sai cấu trúc mục, thiếu mục,
thêm mục không có trong mẫu**. Hậu quả nặng nhất là **phải làm lại và nộp lại toàn bộ tài liệu**.

**Cách tránh:**
- Tải template mới nhất từ FLM ngay tuần đầu, không dùng lại file khóa trước
- Đối chiếu từng mục trong template với tài liệu của nhóm trước khi nộp
- Không copy nguyên văn phần hướng dẫn/placeholder rồi để nguyên trong bài nộp

### 1.2. Thiếu các trang bắt buộc

Thường bị thiếu nhất:
- Trang **Acknowledgement** — và nếu có thì quên ghi tên GVHD cùng các thầy cô (lỗi bị nhắc riêng)
- **User Guide / User Manual**
- **Số trang**
- **Danh mục bảng (List of Tables)** và **danh mục hình (List of Figures)** — phải tách riêng
  khỏi mục lục chính và đánh lại số thứ tự
- **Bảng từ viết tắt** — thiếu, hoặc có nhưng chưa sắp xếp theo thứ tự
- **Class Diagram**
- **Test case / Testing document** (làm nhưng không nộp)
- Thông tin mentor, thành viên nhóm

### 1.3. Trình bày cẩu thả

- Trang bìa canh lề lệch, font lạ, trình bày chưa chỉn chu
- Trang bìa bị tràn dòng cuối (địa danh, năm) sang trang sau
- Mục lục "lố" qua trang bìa; mục lục lặp lại nội dung quá nhiều
- **Không phân trang cho các phần lớn** — Acknowledgement, phần I, II, III… mỗi phần
  phải bắt đầu ở trang mới
- Giật cấp heading sai (heading level không nhất quán)
- Đánh số trang và số thứ tự mục sai, thụt lề không đồng nhất
- **Còn nhiều khoảng trắng thừa**
- **Định dạng bảng/menu cẩu thả**
- ⚠️ **Chèn sơ đồ ở kích thước quá nhỏ, không đọc được chữ bên trong**
- Chưa xóa các phần gợi ý / tag của template
- Còn sót tiếng Việt trong tài liệu tiếng Anh (đặc biệt ở Test Report)
- Còn sót nội dung không liên quan trong mục Definition/Glossary
- Slide sai thuật ngữ tiếng Anh · ghi sai vai trò GVHD
- Bảng thuật ngữ thiếu và chọn lọc không chính xác
- Nguồn tham khảo không phù hợp, không rõ ràng, thông tin lỗi thời

**Cách tránh:** dành trọn một buổi cuối chỉ để đọc lại toàn bộ ở chế độ **print preview**.

### 1.4. Viết tài liệu bằng AI nhưng không kiểm tra lại

Lỗi đang tăng mạnh:
- Business Rule viết bằng AI nhưng nội dung không chính xác
- Test Report sinh tự động, nhiều thông tin không phù hợp hệ thống thật
- NFR ghi rất nhiều nhưng không rõ có hiện thực hay không
- Văn phong chung chung, không gắn với đặc thù đề tài

> **Nguyên tắc vàng: "có làm mới ghi, không làm đừng ghi."**
> Với mỗi NFR/BR viết ra, phải chỉ được chỗ đã hiện thực trong code hoặc chỗ đã kiểm thử.

### 1.5. Nội dung trong tài liệu không khớp với phần mềm

- **Có mục mô tả chức năng không tồn tại trong phần mềm**
- **Sơ đồ kiến trúc vẽ ra không khớp với code trên repository**
- Tồn tại hai bản Use Case Diagram khác nhau trong cùng tài liệu
- Tài liệu mô tả một đằng, sản phẩm chạy một nẻo
- Mục tiêu trong tài liệu rộng hơn nhiều so với thứ demo được
- ⚠️ **Thiếu hẳn một sơ đồ bắt buộc → mất trắng điểm mục đó**
- **Không mô tả giải thuật/thuật toán đang dùng trong tài liệu**
- Future Plan hứa những điều không khả thi
- **Không nộp source code của một phần đã trình bày trong tài liệu**
- Không có thông tin doanh nghiệp/khách hàng thật
- **Nộp link thay vì nộp file** → người chấm không mở được nên không đủ cơ sở chấm.
  Luôn **nhúng hình vào tài liệu** và nộp file thật

### 1.6. Caption và tham chiếu chéo

| Quy tắc | Đúng | Sai |
|---|---|---|
| Caption của **Hình** | Đặt **bên dưới** hình | Đặt bên trên |
| Caption của **Bảng** | Đặt **bên trên** bảng | Đặt bên dưới |
| Tham chiếu trong bài | *"…như mô tả trong Hình 3.2"* | *"hình dưới đây"*, *"bảng sau"* |

Các lỗi cụ thể:
- Caption hình đặt trên, caption bảng đặt dưới — ngược quy ước
- **Không có cross-reference**: hình/bảng chèn vào nhưng không đoạn văn nào nhắc tới
- **Đánh số caption thủ công** → sai thứ tự, trùng số, lệch khi thêm/bớt hình
- Caption chỉ ghi tên trống rỗng (*"Hình 5"*) thay vì mô tả nội dung
- Danh mục hình/bảng không khớp caption thật

**Cách tránh:** dùng `Insert Caption` · dùng `Insert → Cross-reference` · mỗi hình/bảng phải được
nhắc tới ít nhất một lần **trước khi xuất hiện** · xong rồi `Ctrl+A` → `F9`.

---

## 2. Nhóm lỗi SƠ ĐỒ & MÔ HÌNH HÓA

> Nhóm lỗi **bị bắt chi tiết nhất**, vì sơ đồ được chấm đúng theo chuẩn ký hiệu.

### 2.1. Use Case Diagram

| Lỗi | Ghi chú |
|---|---|
| Dùng sai `include` / `extend` | **Lỗi phổ biến nhất trong toàn bộ nhóm lỗi sơ đồ** |
| Vẽ sai quan hệ kế thừa (generalization) | |
| Sai cú pháp quan hệ, dùng mũi tên đen đặc | Quan hệ trong UC không dùng mũi tên tô đen |
| Use case thiếu actor (2 actor nhưng chỉ vẽ 1) | |
| Tên use case không phải động từ | Use case luôn phải là **động từ + tân ngữ** |
| Liên kết actor với use case chưa hợp lý | |
| Vẽ Use Case theo luồng xử lý | Use Case mô tả **chức năng**, không phải trình tự các bước |
| Sai nét nối giữa actor và use case | |
| Mô tả use case theo mẫu cũ | Phải dùng biểu mẫu mới nhất trên flm.fpt.edu.vn |

### 2.2. Sequence Diagram

- Thiếu **số thứ tự message** — hoặc tệ hơn: lúc có lúc không (thiếu nhất quán)
- Thiếu **thanh activation bar**
- Thiếu **dấu `:` trong tên object**, thiếu schema
- Thiếu các object tầng dưới: **controller, service, DAO/repository** — vẽ từ frontend
  đi thẳng lên hub/backend là sai
- Hộp `alt` không chia thành 2 lane ứng với nhánh if/else, hoặc không đánh nhãn
  thành công/thất bại
- Thiếu các **luồng exception** (nếu nghiệp vụ có)
- ⚠️ **Gom nhiều use case vào 1 sequence → không phù hợp**
- Frontend phải **chỉ cụ thể đối tượng**, không vẽ chung chung
- Luồng phải đi theo đúng **design pattern đã chọn**
- ⚠️ **Sequence không khớp với kiến trúc class đã mô tả ở phần thiết kế**

### 2.3. ERD / Database Design

- Vẽ ở mức entity nhưng lại có thuộc tính → sai mức trừu tượng
- **Nộp ERD Conceptual trong khi yêu cầu là Physical**
- Vẽ theo Peter Chen thì cardinality phải nằm sát thực thể
- Tách ERD thành nhiều hình nhỏ → nên gộp thành một sơ đồ tổng thể
- Đặt quan hệ sai bậc (n:n ở chỗ bản chất là 1:n)
- Đặt tên thực thể/quan hệ nhập nhằng
- Thiếu quan hệ giữa các thực thể → sinh tình huống vô lý về dữ liệu
- Kiểu dữ liệu / độ dài ký tự chưa hợp lý
- Lỗi chính tả, dư thực thể/thuộc tính không dùng đến
- Ký hiệu không phân biệt được các bậc quan hệ (`0..n` và `1..n` vẽ giống hệt nhau)
- Không giải thích được quy ước của chính mình (nét liền / nét đứt / ký hiệu đầu mút)
- ⚠️ **Database Design phải là ảnh chụp từ CSDL thật, không phải hình tự vẽ lý tưởng hóa**
- Nhắc tới NoSQL/cache nhưng không mô tả cách triển khai và dữ liệu lưu trong đó

### 2.4. System Architecture Diagram

> **Sơ đồ bị hỏi xoáy nhiều nhất**, vì nó dùng để kiểm tra bạn có thật sự hiểu hệ thống không.

- Vẽ overview lẫn detail trong cùng một hình
- Đặt component sai chỗ (đặt ORM ra ngoài framework chứa nó)
- **Thiếu đường nối tới third-party service**
- Vẽ third-party chỉ nối với Backend trong khi thực tế Frontend cũng gọi
- Frontend nối trực tiếp tới File Storage/Database mà không giải thích được
- Vẽ có message queue / websocket / cache nhưng **không chỉ ra được chỗ nào implement**
- Kiến trúc không nhất quán giữa các phần của tài liệu
- ⚠️ **Kiến trúc vẽ ra không khớp với repository thật**

**Cách tránh:** mỗi thành viên phải làm được — *"Chỉ vào từng khối trong sơ đồ và
**mở đúng đoạn code tương ứng**."*

### 2.5. Class Diagram & State Machine Diagram

> Hai sơ đồ này hay bị làm qua loa hoặc bỏ quên, nhưng vẫn bị chấm.

- **Class Diagram:** thiếu hẳn trong tài liệu; hoặc **số lượng/kiểu liên kết giữa các class sai**
- **State Machine Diagram:** vẽ sai; **thiếu đối tượng gây ra việc chuyển trạng thái
  trên mỗi transition**
- **Đặt State Machine Diagram ở phụ lục trong khi nó thuộc phần thiết kế chính**

### 2.6. Các sơ đồ khác

- **Context Diagram:** không được gộp 2 entity; nhãn trên luồng dữ liệu phải là **danh từ**,
  không phải động từ; sai ký hiệu hình chữ nhật
- **Activity Diagram:** node quyết định phải là **hình thoi**, không phải ngũ giác
- **Product Overview Diagram:** sai ký hiệu hình chữ nhật bo góc
- **Mô hình phát triển phần mềm:** vẽ sai, hoặc thiếu phần diễn giải quy trình
- Sơ đồ **không có chú thích, không có tiêu đề** → người đọc không nhận ra đó là sơ đồ gì
- ⚠️ **Các sơ đồ không đồng bộ với nhau**: cùng một chức năng nhưng tên gọi, actor,
  luồng khác nhau ở mỗi sơ đồ

---

## 3. Nhóm lỗi PHÂN TÍCH NGHIỆP VỤ

> Nhóm lỗi **nặng nhất về điểm số**, vì nó cho thấy nhóm chưa hiểu bài toán.

### 3.1. Nghiệp vụ không sát thực tế

- Ràng buộc phi thực tế (một khách chỉ thuê một đơn vị tài nguyên; đã tạo thì không bổ sung được)
- Quy trình bị ngược so với thực tế
- Không khảo sát thực tế, chỉ ngồi tự nghĩ tình huống
- **Bỏ sót giai đoạn của quy trình thật** (chỉ làm giai đoạn giữa, thiếu trước và sau)
- Cách tính giá/phí không theo cách ngành đó tính thật
- Bỏ qua thuế/VAT, hóa đơn điện tử khi bài toán liên quan tài chính
- **Cộng dồn số liệu có đơn vị tính khác nhau** · quy đổi đơn vị không tường minh
- Không xét điều kiện môi trường thật (mất điện, mất mạng…)
- **Dùng sai thuật ngữ chuyên ngành** → cho thấy chưa hiểu domain
- Đặt tên trạng thái không đúng ngữ nghĩa
- Giả định phi thực tế để né xử lý
- Chức năng có nhưng không phù hợp cách vận hành thật của ngành
- Tự phức tạp hóa bằng khái niệm trung gian không cần thiết
- Đơn vị tính không thống nhất trong cùng một nghiệp vụ
- **Hiểu sai ngữ nghĩa của chính dữ liệu mình có**
- Định dạng dữ liệu pháp lý không chuẩn (ngày tháng, số tiền, đơn vị tiền tệ)
- Không có phương án cho người dùng thiếu điều kiện tiêu chuẩn

**Cách tránh:** đi hỏi người làm nghề thật ít nhất một lần · mỗi quy trình vẽ đủ
**trước – trong – sau** · tra cứu quy định pháp lý của domain.

### 3.2. Thiếu ràng buộc và tình huống ngoại lệ

> Câu hỏi *"Nếu … thì sao?"* gần như chắc chắn sẽ được hỏi.

- Người tạo và người duyệt là cùng một người
- Đối tượng đã duyệt rồi vẫn sửa được
- Hủy hợp đồng/đơn hàng nhưng không có bước phê duyệt việc hủy
- Thanh toán một phần nhưng hệ thống ghi "đã thanh toán xong"
- Không ràng buộc thứ tự xử lý trong quy trình
- Không xử lý khi thanh toán thất bại
- **Không xử lý xung đột khi 2 người cùng thao tác trên một đối tượng**
- Trạng thái "chờ" không có cơ chế thoát
- Không có deadline hủy, hoặc deadline 2 dịch vụ liên quan không khớp
- Phân quyền sai: thu hồi quyền vai trò này làm khóa luôn vai trò khác
- Không ràng buộc dữ liệu → giá trị vô lý (ngày 1/1/1, số tiền âm)
- Không kiểm soát sự phù hợp giữa người dùng và tài nguyên họ tạo
- Không xử lý khi hệ thống/thiết bị chết giữa chừng
- **Quy trình phụ thuộc một người duyệt thủ công mà không có cơ chế nhắc/timeout**
- **Không có khả năng truy vết (audit log)**
- Thao tác vẫn thực hiện được ở thời điểm lẽ ra bị chặn

**Bảng "Nếu… thì sao?"** — mỗi mainflow viết tối thiểu 5 câu và **trả lời bằng code**:
1. Nếu thanh toán fail giữa chừng thì sao?
2. Nếu 2 người thao tác cùng lúc thì sao?
3. Nếu người dùng hủy sau khi đã được duyệt thì sao?
4. Nếu dữ liệu vượt giới hạn hợp lý (âm, quá lớn, quá xa) thì sao?
5. Nếu vai trò A cố tình làm việc của vai trò B thì sao?

### 3.3. Con số trên UI không giải thích được

> Dạng kiểm tra rất hay gặp: chỉ vào một con số cụ thể trên màn hình và yêu cầu giải thích.

- Không giải thích được công thức tính phí
- Lợi nhuận tính sai vì bỏ sót khoản khấu trừ
- Tổng doanh thu không có lịch sử giao dịch để đối chiếu
- Không phân biệt được tiền vào / tiền ra trên UI
- Xuất hóa đơn ra số âm
- Số liệu thống kê không lọc được theo phạm vi
- Dòng tiền vào minh bạch nhưng dòng tiền ra thì không
- Không ghi log doanh thu / phần trích lại của nền tảng
- Số dư khả dụng không khớp khi rút tiền
- Thời gian ghi nhận giao dịch sai
- **Trọng số/tỷ lệ tính điểm áp dụng như nhau cho các tập dữ liệu có quy mô khác nhau**
- Không giải thích được công thức tính lương, hoa hồng, KPI

**Cách tránh:** chuẩn bị sẵn **file ghi rõ mọi công thức tính tiền/điểm kèm ví dụ số cụ thể**.

### 3.4. Business Rule viết cho có

- **Số lượng BR quá ít so với quy mô ứng dụng**
- BR viết ra nhưng **code không hề kiểm tra**
- BR mô tả sai so với thứ đã hiện thực
- Thiếu BR cho những ràng buộc quan trọng nhất
- BR không ghi nhận các tham số cấu hình đang tồn tại
- BR viết bằng AI, nội dung chung chung

**Cách tránh:** sau khi code xong, **rà ngược từ code ra BR** — mọi câu `if` mang tính nghiệp vụ
đều phải có một BR tương ứng, và ngược lại.

---

## 4. Nhóm lỗi HARDCODE & CẤU HÌNH

> Bị bắt gần như **trong mọi buổi bảo vệ**, và là một trong những lý do chính dẫn tới không đạt.

Hay bị phát hiện hardcode:
- Phần trăm hoa hồng, phí nền tảng, tỷ lệ trích giữ lại
- **Ngưỡng nghiệp vụ** (số tiêu chí cần đạt, tỷ lệ tối thiểu…)
- **Ngưỡng tin cậy của model AI**
- Giá gói dịch vụ, giới hạn khoảng cách, trọng số thuật toán
- **Nội dung email/thông báo**
- **Danh mục cấu hình** (loại đề, số vòng của quy trình…)
- Thông tin thiết bị IoT khai báo từng cái một
- Mã OTP cố định trong code — vừa hardcode vừa là lỗ hổng bảo mật
- Thậm chí cả kết quả trả về của module AI

> **Câu hỏi kinh điển:** *"Giờ tôi muốn đổi con số này từ 70% lên 80% thì làm thế nào?
> Hãy demo ngay."*

**Cách tránh:**
- Mọi tham số nghiệp vụ phải nằm trong **bảng cấu hình trong DB**, có **màn hình admin**
  để sửa, và **sửa xong có hiệu lực ngay**
- ⚠️ **Sửa file JSON/appsettings rồi restart vẫn bị coi là hardcode**
- Chuẩn bị sẵn màn hình *"System Configuration"* và **chủ động demo** — gây ấn tượng rất tốt

---

## 5. Nhóm lỗi liên quan đến AI trong đề tài

- Tên đề tài có AI nhưng khi demo phần AI mờ nhạt, bài toán quy về CRUD thông thường
- ⚠️ **Gọi là AI nhưng thực chất chỉ là if-else / rule-based**
- Dùng model có sẵn, không training, không chỉ ra được đóng góp riêng của nhóm
- Model chỉ đúng với đúng một mẫu dữ liệu đã chuẩn bị sẵn
- Độ chính xác quá thấp nhưng vẫn báo cáo là đạt
- Không trả lời được model lưu ở đâu, phiên bản nào
- ⚠️ **Không có metric đánh giá chất lượng đầu ra**
- ⚠️ **Không có actor duyệt lại kết quả do AI sinh ra**
- Chatbot trả lời sai vì dùng dữ liệu cũ
- **Không quản lý giới hạn token / chi phí AI**
- Xử lý dữ liệu làm mất thông tin
- AI sinh nội dung nhưng không ràng buộc tính phù hợp
- AI hỗ trợ quá nhiều làm vai trò con người mờ nhạt
- ⚠️ **Không giải thích được ngưỡng tin cậy đang dùng: con số đó từ đâu ra, dựa trên thí nghiệm nào**
- Chiến lược training không hợp lý về kiến trúc
- Gán nhãn/gom nhóm dữ liệu sai bản chất
- **Lạm dụng từ "smart" / "thông minh"** mà không có gì tương xứng
- Đưa AI vào chỗ mà một truy vấn hoặc công thức đơn giản giải quyết tốt hơn

**Cách tránh — trả lời rõ 4 câu:**
1. AI giải quyết vấn đề gì?
2. Dữ liệu ở đâu?
3. Đo bằng metric nào, kết quả bao nhiêu?
4. Ai kiểm chứng đầu ra?

> Nếu là rule-based thì **nói thẳng là rule-based**, đừng gọi là AI.

---

## 6. Nhóm lỗi KIỂM THỬ (Test)

- Test Report còn nhiều case **Fail** mà vẫn nộp, không giải thích
- **Số liệu tổng hợp trong test report sai và không nhất quán**
- Test report chưa nhập đầy đủ các trường
- Test case viết như hướng dẫn sử dụng thay vì test case
- Cách viết không phải system test; nhiều test case nhưng không có giá trị
- **Bước (STEP) không đủ chi tiết để người khác tái hiện được**
- Unit Test sai, không tách theo từng tham số
- Test chỉ phủ được một phần luồng
- Các test case không liên kết thành kịch bản có nghĩa

**Cách tránh:** mỗi FR có ít nhất 1 test case · mỗi step người lạ đọc vào làm lại được ·
còn case Fail thì **ghi rõ known issue + lý do + kế hoạch xử lý, đừng giấu**.

---

## 7. Nhóm lỗi khi DEMO

> Nơi **mất điểm nhanh nhất**, vì người chấm nhìn thấy trực tiếp.

### 7.1. Các thất bại điển hình

- Chức năng lỗi khi demo, demo lại vẫn lỗi
- Hết thời gian quy định vẫn chưa demo được luồng chính
- ⚠️ **Không có dữ liệu để demo tiếp → buổi bảo vệ bị dừng giữa chừng** (đã thực sự xảy ra)
- Thiết bị phần cứng hỏng ngay hôm bảo vệ → không đạt
- **Gặp lỗi UI nhưng lướt qua, bấm next, không giải thích → bị đánh giá thiếu trung thực**
- Không chuẩn bị dữ liệu demo trước; dữ liệu giả lập không giống thực tế
- Kịch bản demo lộn xộn, nhảy giữa các nghiệp vụ không liên quan
- **Có chức năng đã làm nhưng quên không demo**
- Setup hệ thống dễ dãi "cho tiện demo" (bỏ ràng buộc) rồi bị phát hiện
- **Dùng chung một tài khoản/email cho nhiều vai trò** → không chứng minh được phân quyền
- Trình bày xong toàn bộ lý thuyết rồi mới demo — nên **trình bày tới luồng nào thì demo ngay**
- Không giải thích được nguyên nhân lỗi

### 7.2. Nguyên tắc demo

1. Chuẩn bị **dữ liệu mẫu đầy đủ, thật, đa dạng** — có lịch sử, nhiều user, nhiều trạng thái
2. Viết kịch bản theo mainflow, phân vai rõ, **tập chạy thử ít nhất 2 lần có bấm giờ**
3. **Không bao giờ lướt qua lỗi.** Dừng lại: *"Đây là lỗi X, nguyên nhân là Y, nhóm đã ghi nhận"*
4. **Đừng sửa hệ thống cho "dễ demo".** Cần demo tình huống phụ thuộc thời gian thì
   **chỉnh giờ server** thay vì gỡ ràng buộc
5. Chuẩn bị **video ghi màn hình mainflow** làm phương án dự phòng
6. **Chuẩn bị sẵn IDE đã mở repository** — rất hay bị yêu cầu mở code ngay tại chỗ
7. Đặt **mật khẩu đơn giản** cho tài khoản demo

### 7.3. Slide thuyết trình

- Phải có phần **tổng kết những gì đạt được và hạn chế còn tồn tại** — thiếu bị nhắc riêng
- Chỉn chu, đúng thuật ngữ tiếng Anh
- Giới thiệu rõ các thiết bị/thành phần đặc thù
- Trình bày theo mạch, không nhảy cóc

---

## 8. Nhóm lỗi khi TRẢ LỜI CÂU HỎI

### 8.1. Không hiểu code của chính mình

Tình huống rất xấu: được yêu cầu mở source code, sinh viên trả lời *"em dùng thư viện gì
em không nhớ"*, hoặc thừa nhận dùng AI sinh code nhưng không hiểu nguyên lý.

Các biến thể:
- *"Phần đó bạn khác làm nhưng bạn ấy nghỉ rồi"*
- Không chỉ ra được chỗ nào trong code xử lý một nghiệp vụ cụ thể
- **Không giải thích được cơ chế authentication/authorization trên code**
- **Không trả lời được service call bị fail thì được handle ở đâu**

**Cách tránh:** mọi thành viên phải hiểu **toàn bộ mainflow**, không chỉ phần mình làm ·
tổ chức buổi **"hỏi chéo"**: mỗi người bị hỏi 10 câu về phần người khác làm.

### 8.2. Trả lời không rõ ràng / vòng vo

> Trả lời theo cấu trúc: **Có/Không → Lý do → Chỗ hiện thực → Hạn chế nếu có.**
>
> Nếu chưa làm, **nói thẳng**: *"Nhóm chưa xử lý tình huống này"*.
> Ngắn gọn và trung thực tốt hơn vòng vo. **Không tranh cãi.**

### 8.3. Không nắm được định vị của sản phẩm

- *"Đây là hệ thống hay nền tảng?"* (khác nhau ở chỗ **ai là người cấu hình**)
- *"Sản phẩm này khác gì với các sản phẩm đã có trên thị trường?"*
- *"Đề tài này có gì đặc sắc, tại sao cần một hệ thống riêng?"*
- *"Nhóm focus vào ứng dụng hay vào AI?"*

**Cách tránh:** chuẩn bị **elevator pitch 60 giây**: bài toán – ai dùng – khác biệt – giá trị cốt lõi.

---

## 9. Nhóm lỗi QUY TRÌNH & LÀM VIỆC NHÓM

> Nhóm lỗi dẫn tới **hậu quả nghiêm trọng nhất**.

| Lỗi | Hậu quả |
|---|---|
| Không được GVHD duyệt ra bảo vệ | **Không đạt** |
| Nhóm không có mặt đúng giờ bảo vệ | **Không đạt** |
| Thành viên vắng các buổi review, đóng góp rất ít | **Cá nhân đó không đạt và phải làm lại đồ án, dù cả nhóm đạt** |
| Không cải thiện theo góp ý của các lần Review | Không đạt hoặc phải bảo vệ lần 2 |
| Thành viên bỏ nhóm giữa chừng | Không giải thích được khi bị hỏi tới phần đó |
| Git tổ chức lộn xộn | Không có căn cứ chứng minh đóng góp cá nhân |

**Cách tránh:** tự tổ chức buổi tổng duyệt · lập **bảng theo dõi từng góp ý Review → trạng thái
xử lý → bằng chứng đã sửa** và **mang bảng này đi bảo vệ** · ghi nhận đóng góp qua commit
history + biên bản họp · xác nhận GVHD duyệt ra bảo vệ **trước hạn ít nhất 2 tuần**.

---

## 10. Nhóm lỗi về DỮ LIỆU, BẢO MẬT & PHÁP LÝ

- Thu thập thông tin quá mức cần thiết (SĐT, địa chỉ, CCCD khi không cần)
- **Rò rỉ thông tin**: nhập một trường định danh thì hiển thị luôn thông tin cá nhân
- Yêu cầu xác thực giấy tờ tùy thân ở nơi không hợp lý
- Xác minh giấy tờ/mã số doanh nghiệp thủ công bằng mắt
- ⚠️ **Không nghiên cứu policy của nền tảng bên thứ ba khi đề tài phụ thuộc vào nó**
- Không xử lý bản quyền nội dung người dùng tải lên
- Cập nhật quy định pháp luật quá trễ
- Không ghi rõ nguồn dữ liệu lấy từ bên thứ ba
- Không kiểm soát tính phù hợp nội dung với lứa tuổi
- NFR ghi *"dữ liệu phải bảo mật"* nhưng chưa hiện thực
- ⚠️ **API key / secret để lộ trong source code**
- ⚠️ **Lộ đường dẫn trực tiếp tới tài nguyên** → ai có link đều tải được
- Bảo vệ nội dung chỉ ở phía frontend (chặn chuột phải) → **không phải là bảo mật**
- Chuyển tiền cho người bán ngay sau giao dịch

**Cách tránh:** nguyên tắc **thu thập tối thiểu** · có mục **Legal & Compliance** ngắn trong
tài liệu · **quét repository tìm secret/API key trước khi nộp** · ghi nguồn cho mọi dữ liệu bên thứ ba.

---

## 11. Nhóm lỗi về UI/UX

- UI dày đặc — hoặc ngược lại: trống, đơn điệu
- **Nút "Duyệt" và "Không duyệt" đặt quá gần nhau**
- Giao diện dư thừa (cùng một điều khiển ở hai nơi)
- **Không thống nhất ngôn ngữ trên giao diện**
- Nhãn sai nghĩa so với chức năng thật của nút
- Biểu đồ thống kê không mang nhiều ý nghĩa
- Thiếu thông tin quan trọng trên màn hình
- Nhập liệu quá nhiều, quy trình thủ công trong khi có thể tự động
- Chọn sai định dạng xuất báo cáo (PDF trong khi Excel phù hợp hơn)
- Giao diện chưa phù hợp đối tượng người dùng
- Dùng thuật ngữ kỹ thuật khó hiểu với người dùng cuối
- ⚠️ **Danh sách dài không có phân trang, không có tìm kiếm/lọc**
- Bố cục không cân đối
- Luồng thao tác trên mobile quá nhiều bước
- **Định dạng hiển thị số tiền, ngày tháng không theo quy ước chung**

---

## 12. CHECKLIST TỰ KIỂM TRƯỚC BẢO VỆ

### D-14 (hai tuần trước)

- [ ] Tải template chính thức mới nhất từ FLM, đối chiếu toàn bộ tài liệu
- [ ] Lập bảng: mọi góp ý Review và GVHD → đã xử lý chưa → bằng chứng
- [ ] Rà soát toàn bộ magic number trong code → đưa vào config có UI quản trị
- [ ] Xác nhận GVHD đồng ý duyệt ra bảo vệ
- [ ] Kiểm tra mọi thành viên đều có commit và phần việc rõ ràng

### D-7 (một tuần trước)

- [ ] **Kiểm tra lại 8 loại sơ đồ theo mục 2** (Use Case, Sequence, Class, State Machine,
      ERD, Architecture, Context, Activity)
- [ ] Kiểm tra các sơ đồ **đồng bộ với nhau**: cùng tên chức năng, cùng actor, cùng luồng
- [ ] **Đối chiếu sơ đồ kiến trúc với repository thật** — từng khối phải trỏ được tới code
- [ ] Rà lại toàn bộ FR/NFR/BR: có làm mới ghi; mọi `if` nghiệp vụ đều có BR tương ứng
- [ ] Hoàn thiện Test Report: không còn case Fail chưa giải thích, số liệu nhất quán
- [ ] Kiểm tra danh mục tài liệu: Acknowledgement (có tên GVHD), User Guide, Class Diagram,
      danh mục bảng/hình, bảng viết tắt, số trang, mục lục
- [ ] Xóa hết placeholder/tag còn sót; **phóng to mọi sơ đồ tới mức đọc được**
- [ ] Caption: hình ở dưới, bảng ở trên; mọi hình/bảng có cross-reference
- [ ] `Ctrl+A` → `F9` cập nhật toàn bộ trường tự động
- [ ] Kiểm tra danh mục tài liệu tham khảo
- [ ] **Lập bảng công thức tính toán kèm ví dụ số cụ thể**
- [ ] Chuẩn bị dữ liệu demo phong phú, nhiều trạng thái, có lịch sử

### D-3

- [ ] Viết kịch bản demo, phân vai, chạy thử có bấm giờ ít nhất 2 lần
- [ ] Buổi **"hỏi chéo"**: mỗi thành viên trả lời 10 câu về phần người khác làm
- [ ] Chuẩn bị trả lời **"Nếu… thì sao?"** cho mỗi mainflow (tối thiểu 5 tình huống)
- [ ] Chuẩn bị **elevator pitch 60 giây**
- [ ] **Quét repository tìm API key / secret bị lộ**
- [ ] Đọc lại tài liệu ở chế độ **print preview**

### D-1

- [ ] Ghi **video màn hình mainflow** làm phương án dự phòng
- [ ] Chuẩn bị máy dự phòng, kiểm tra mạng
- [ ] Kiểm tra dữ liệu demo còn nguyên vẹn
- [ ] Đặt mật khẩu đơn giản cho tài khoản demo, ghi ra giấy
- [ ] Mở sẵn: app đã deploy, **IDE với repository**, tài liệu, file công thức, bảng theo dõi góp ý
- [ ] Xác nhận lại giờ và địa điểm bảo vệ

### Ngày bảo vệ

- [ ] Có mặt sớm ít nhất 30 phút
- [ ] Demo đúng kịch bản, **không lướt qua lỗi**
- [ ] Trả lời: **Có/Không → Lý do → Chỗ hiện thực → Hạn chế**
- [ ] Ghi chép lại toàn bộ góp ý để chỉnh sửa và nộp lại đúng hạn

---

## 13. NHỮNG CÂU HỎI HAY GẶP NHẤT

**Về cấu hình**
1. Con số này (%, ngưỡng, phí) có thay đổi được không? **Demo ngay cho tôi xem.**
2. Ai là người cấu hình cái này — admin hay chủ tài nguyên?

**Về kiến trúc & code**

3. **Chỉ vào sơ đồ kiến trúc và mở đoạn code tương ứng.**
4. Thành phần X trong sơ đồ dùng để làm gì? Tại sao chọn công nghệ này mà không phải cái kia?
5. Service này đồng bộ hay bất đồng bộ? Call fail thì handle ở đâu?

**Về nghiệp vụ**

6. Nếu người dùng hủy sau khi đã duyệt thì sao?
7. Nếu thanh toán thất bại giữa chừng thì sao?
8. Người tạo và người duyệt có thể là cùng một người không?
9. Nghiệp vụ này ngoài thực tế có đúng như vậy không? Nhóm đã khảo sát chưa?

**Về con số**

10. Giải thích con số này trên màn hình được tính như thế nào?
11. Cho xem dòng tiền vào và dòng tiền ra.

**Về AI**

12. AI trong hệ thống làm gì? Có training không? Đánh giá chất lượng bằng gì?
13. Ai kiểm chứng kết quả AI trả về?

**Về Business Rule**

14. Ràng buộc này nhóm ghi trong BR — chỉ cho tôi chỗ code kiểm tra nó.
15. Vì sao hệ thống lớn thế này mà chỉ có ngần này Business Rule?

**Về tài liệu**

16. Mở template chính thức của trường ra, đối chiếu với tài liệu của nhóm.
17. Chức năng ghi trong tài liệu này có trong phần mềm không?
18. Sơ đồ này vẽ tay hay chụp từ hệ thống thật?
19. Hình/bảng này được nhắc tới ở đoạn nào trong bài?
20. Thuật toán nhóm dùng là gì? Mô tả ở mục nào trong tài liệu?

---

## 14. BẢNG TỰ ĐÁNH GIÁ RỦI RO

> Cho điểm mỗi dòng: **0** = chưa làm · **1** = làm một phần · **2** = làm tốt

| # | Tiêu chí | Điểm |
|---|---|:---:|
| 1 | Tài liệu đúng template chính thức, đủ mọi trang bắt buộc | |
| 2 | 8 loại sơ đồ đúng chuẩn ký hiệu, nhất quán với nhau và với code | |
| 3 | Không còn tham số nghiệp vụ nào hardcode | |
| 4 | Mọi mainflow đã xử lý ít nhất 5 tình huống ngoại lệ | |
| 5 | Giải thích được mọi con số hiển thị trên UI | |
| 6 | Nghiệp vụ đã được khảo sát/kiểm chứng với người làm nghề thật | |
| 7 | Test Report đầy đủ, số liệu nhất quán, không còn Fail chưa giải thích | |
| 8 | Mọi thành viên hiểu và giải thích được toàn bộ mainflow + code | |
| 9 | Đã xử lý hết góp ý của các lần Review và của GVHD, có bằng chứng | |
| 10 | Kịch bản demo đã chạy thử có bấm giờ, có dữ liệu mẫu đầy đủ | |
| 11 | AI (nếu có) có metric đánh giá và cơ chế con người kiểm chứng | |
| 12 | Đã rà soát dữ liệu cá nhân, bản quyền, secret key, đường dẫn tài nguyên | |
| 13 | Business Rule đầy đủ và được code kiểm tra thật, không chỉ nằm trên giấy | |
| 14 | Có phương án dự phòng cho dữ liệu demo và phần cứng (nếu có) | |
| 15 | Caption đúng quy ước và mọi hình/bảng có cross-reference trong nội dung | |

**Cách đọc kết quả**

| Điểm | Kết luận |
|---|---|
| **26 – 30** | Sẵn sàng bảo vệ |
| **18 – 25** | Có khả năng rơi vào *"đạt nhưng phải chỉnh sửa và nộp lại tài liệu"*. Tập trung vào các dòng đang 0–1 |
| **Dưới 18** | **Rủi ro cao phải bảo vệ lần 2.** Ưu tiên ngay các mục 1, 3, 4, 9, 10, 14 |

---

## 15. Ba lời khuyên quan trọng nhất

1. **Trung thực luôn được điểm cao hơn che giấu.** Nhóm chủ động nói ra hạn chế của mình
   được đánh giá tốt; nhóm lướt qua lỗi hoặc trả lời vòng vo bị trừ điểm nặng.

2. **Các buổi Review là cơ hội sửa sai, không phải thủ tục.** *"Không tiếp thu góp ý của các
   lần Review"* là một trong những lý do trực tiếp dẫn tới không đạt.

3. **Tài liệu là phần dễ ăn điểm nhất và bị mất điểm nhiều nhất.** Sản phẩm chạy tốt nhưng
   tài liệu sai sơ đồ, thiếu trang, sai template vẫn khiến nhóm phải làm lại và nộp lại.
   Đây là công việc **không cần sáng tạo, chỉ cần cẩn thận**.
