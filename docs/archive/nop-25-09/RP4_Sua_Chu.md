# RP4 — Những đoạn CHỮ cần sửa

> File này chỉ nói về **phần chữ** trong `Report4_Software Design Document.docx`.
> Phần hình đã có ở [RP4_Hinh_Da_Sua.md](RP4_Hinh_Da_Sua.md).
>
> Mọi đoạn tiếng Anh trong file này **dán thẳng vào Word được**.
> Đã đối chiếu với `Cam-nang-tranh-loi-Capstone-SE.pdf` và với code thật.

---

## A. Tóm tắt: 17 hình → 24 hình

## A. Tóm tắt: 17 hình → 20 hình

| Lý do | Số hình thêm |
|---|---|
| Tách "gửi thư mời" thành 2 (Staff gửi · Reviewer trả lời) | +1 |
| Tách "chấm điểm" thành 2 (uỷ viên chấm · biên bản & duyệt) | +1 |
| Tách "hợp đồng" thành 2 (ký HĐ · báo cáo tiến độ) | +1 |

Lý do tách: cẩm nang **§2.2** — *"Gom nhiều use case vào 1 sequence → không phù hợp"*.

Việc tách này **đúng template**: mẫu RP4 cho phép nhiều sequence trong cùng một feature
(`3.1.2 <Sequence Diagram Name1>`, `3.1.3 <Sequence Diagram Name2>`, `3.1.4 …`).

> ⚠️ **State Machine KHÔNG thêm vào RP4.** Xem mục F — bản trước của file này khuyên sai.

### Bảng đánh số mới

| Mới | Nội dung | Cũ |
|---|---|---|
| Figure 1 | Overall system architecture | 1 |
| Figure 2 | Package structure | 2 |
| Figure 3 | ERD | 3 |
| Figure 4 | Class — authentication | 4 |
| Figure 5 | Sequence — login | 5 |
| Figure 6 | Class — research cycle | 6 |
| Figure 7 | Sequence — create & open cycle | 7 |
| Figure 8 | Class — proposal submission | 8 |
| Figure 9 | Sequence — submit proposal | 9 |
| Figure 10 | Class — council setup | 10 |
| Figure 11 | Sequence — send invitations | 11 (tách) |
| **Figure 12** | **Sequence — reviewer responds** | 🆕 |
| Figure 13 | Class — meeting management | 12 |
| Figure 14 | Sequence — schedule meeting | 13 |
| Figure 15 | Class — evaluation & scoring | 14 |
| Figure 16 | Sequence — submit ballot | 15 (tách) |
| **Figure 17** | **Sequence — minutes & chair approval** | 🆕 |
| Figure 18 | Class — contracts & reports | 16 |
| Figure 19 | Sequence — create & sign contract | 17 (tách) |
| **Figure 20** | **Sequence — submit progress report** | 🆕 |

> **Đừng gõ số bằng tay.** Dùng `Insert Caption` của Word rồi `Ctrl+A` → `F9`.
> Cẩm nang §1.6 nêu đích danh lỗi *"Đánh số caption thủ công → sai thứ tự, trùng số"*.

---

## B. 🔴 Table 1 — 10 dòng phải xoá hoặc sửa

Đây là chỗ nguy hiểm nhất trong toàn bộ phần chữ. Cẩm nang **§1.5**:
*"Có mục mô tả chức năng **không tồn tại** trong phần mềm"*.

### B1. ⚠️ 6 dòng mobile (số 11–16) — CẦN XÁC MINH, chưa kết luận

**Đính chính:** bản trước của file này ghi *"không có repo mobile nào"*. Sai.
Bằng chứng khi đó chỉ là thư mục `d:\capstone\newroot` không chứa repo mobile —
tức là **chưa thấy trên máy này**, không phải không tồn tại.
Chủ dự án xác nhận **mobile có thật, nằm ở repo khác do một thành viên khác làm**.

`docs/RA_SOAT_BAO_CAO_RP1-7.md` (16/08) cũng kết luận nhầm y như vậy, khả năng cao
cùng một lý do: chỉ kiểm thư mục local.

**Vậy giữ hay xoá?** Giữ được — **với điều kiện** trả lời được cả 4 câu dưới đây.
Cẩm nang phạt ở chỗ *mô tả thứ không chứng minh được*, không phạt ở chỗ có mobile.

| # | Câu hỏi | Gạch cẩm nang |
|---|---|---|
| 1 | Repo mobile có được **nộp kèm** cùng RP4 không? | §1.5 — *"Không nộp source code của một phần đã trình bày trong tài liệu"* |
| 2 | Hôm bảo vệ có **mở được code mobile** tại chỗ không? | §13 câu 3 — *"Chỉ vào sơ đồ kiến trúc và mở đoạn code tương ứng"* |
| 3 | Có **demo được** app mobile không? | §7.1 — *"Có chức năng đã làm nhưng quên không demo"* |
| 4 | Thành viên làm mobile có **đi bảo vệ** không? | §8.1 — *"Phần đó bạn khác làm nhưng bạn ấy nghỉ rồi"* là câu trả lời bị chấm rất nặng |

- **Đủ 4 câu "có"** → giữ 6 dòng, nhưng phải làm bước xác minh bên dưới.
- **Có câu "không"** → bỏ mobile khỏi RP4 an toàn hơn. Mô tả thứ không chứng minh được
  chính là lỗi hội đồng đã bắt lần trước.

### B1b. Nếu giữ — phải đối chiếu 6 dòng với repo thật

Table 1 đang mô tả rất cụ thể: `App` (Expo Router) · `Src/components` (NativeWind) ·
`Src/services/http` (Axios) · `Src/services/secureStore` (Expo SecureStore) ·
`Src/features` (auth, proposals, reviews, notifications) · `Src/store` (AsyncStorage).

**Chưa ai kiểm 6 dòng này có khớp repo mobile thật không.** Cách làm:

```
clone repo mobile vào d:\capstone\newroot\  (cạnh FURPMS-Web và FURPMS_BEv2)
```

Rồi nhờ đối chiếu từng dòng: thư mục có đúng tên đó không, có thật dùng
Expo Router / NativeWind / SecureStore / AsyncStorage không.

⚠️ Riêng dòng 15 ghi mobile có màn **grading/rubric forms (reviews)** — tức là uỷ viên hội đồng
chấm điểm trên điện thoại. Nếu tính năng đó chưa có thật thì đây là dòng rủi ro nhất trong 6 dòng.

Và dòng 16 ghi *"Mobile **Substance** state"* — chữ này không có nghĩa,
gần như chắc là gõ nhầm (Zustand?). Sửa lại dù giữ hay bỏ.

### B2. Xoá dòng 26 — AutoMapper

`grep AutoMapper` toàn bộ backend = **0 file `.cs` / `.csproj`**. Thư viện không được cài.
Mapping làm tay bằng `.Select()` và các hàm `MapXxx()` riêng trong từng service.

**Hoặc** thay bằng dòng mô tả đúng:

```
26 | Manual mapping | Entities are projected to DTOs directly inside each service using LINQ
projection (.Select) and private MapXxx helpers. No object-mapping library is used, which keeps
the generated SQL explicit and avoids loading unused columns.
```

### B3. Gộp dòng 24–25 — Middleware

Bảng ghi `ProblemDetailsMiddleware` và `ExceptionMiddleware`. Thư mục
`FURPMS.API/Middleware/` thật **chỉ có đúng một file**: `GlobalExceptionMiddleware.cs`.

Thay 2 dòng bằng 1 dòng:

```
24 | Middleware/GlobalExceptionMiddleware | Single centralised exception handler. Maps
AppException, ForbiddenException, KeyNotFoundException, ArgumentException and
InvalidOperationException onto the matching HTTP status codes and returns a uniform
ApiResponse error body.
```

### B4. 🔴 Xoá dòng 17–19 — thư mục `Extension/` không tồn tại

Bảng ghi `Extension/ServiceExtension`, `Extension/AuthenticationExtension`,
`Extension/ControllerExtension, SwaggerExtension`.

Thực tế `FURPMS.API/` có: `Controllers/` · `Middleware/` · `Startup/` · `Serialization/` ·
`Program.cs`. **Không có thư mục `Extension/`, không có file `*Extension*.cs` nào.**

Thay 3 dòng bằng 2 dòng đúng:

```
17 | Program.cs | Application bootstrap: dependency injection registration, JWT bearer
authentication, CORS policy, Swagger, and automatic database migration on startup.

18 | Startup/ | DatabaseStartup applies EF Core migrations and runs the seeder;
ProductionReadinessCheck validates that required configuration is present before the
application accepts traffic.
```

Có thể thêm một dòng nữa nếu muốn đủ:

```
19 | Serialization/UtcDateTimeConverter | JSON converter that guarantees every DateTime is
serialised as UTC, so the client never receives an ambiguous timestamp.
```

### B5. Sửa tên controller ở dòng 20–23

Tên thật đều ở dạng **số nhiều**, và một số tên khác hẳn:

| Bảng đang ghi | Tên thật |
|---|---|
| `ProposalController` | `ProposalsController` |
| `CycleController` | `CyclesController` |
| `CouncilController` | `CouncilsController` |
| `MeetingController` | `CouncilMeetingsController` |
| `ContractController` | `ContractsController` |
| `ReviewController` | `ReviewScoringController` |
| `ReportController` | `ProgressReportsController` |

Dự án có **51 controller**. Nếu bảng ghi con số nào khác thì sửa lại cho khớp.

---

## B6. BẢNG TABLE 1 HOÀN CHỈNH — dán đè cả bảng cũ

29 dòng cũ → **25 dòng mới**. Không còn dòng nào mô tả thứ không tồn tại trong repo.

```
Table 1. Source code packages and their responsibilities.

No | Package | Description

01 | components/ui | Low-level presentation controls wrapped from shadcn/ui — buttons, inputs,
     checkboxes, dialogs, select dropdowns.
02 | components/layout | Global structural layout: sidebar navigation, headers, footers and
     breadcrumbs.
03 | components/shared | Complex components reused by many pages — the paginated DataTable,
     KpiCard, ProgressBar, StatusBadge and DeadlineBadge.
04 | components/charts, components/tables | Chart cards and table primitives used by the
     dashboard and analytics screens.
05 | features/auth | Screens handling credentials: login, change password, forgot and reset
     password, and the academic profile.
06 | features/admin, features/staff, features/pi, features/reviewer | Role-scoped workspaces:
     research-office administration, university staff operations, principal investigators, and
     council members.
07 | features/dashboard, features/analytics | Entry-point dashboards per role and the analytics
     panels reporting on a research cycle.
08 | services/api/axiosClient | Axios instance that attaches the JWT bearer token to every
     request and normalises server errors through interceptors.
09 | services/api | Fifty-eight typed wrappers over the REST endpoints, one per resource —
     proposals, councils, meetings, contracts, reports.
10 | store | Zustand stores: auth.store holds the authenticated user and token,
     notification.store the unread count, ui.store client-side view state.
11 | i18n, hooks, types | Vietnamese and English translation bundles, shared React Query hooks,
     and the TypeScript contracts mirroring the backend DTOs.
12 | app (mobile) | Folder-based routing configured with Expo Router, defining bottom tabs and
     nested stacks per user type.
13 | src/components (mobile) | UI controls optimised for touch input, styled with Tailwind via
     NativeWind.
14 | src/services/http (mobile) | Axios instance targeting the same backend endpoints, with
     request and response interceptors.
15 | src/services/secureStore (mobile) | Persists the JWT access token through Expo SecureStore.
16 | src/features (mobile) | Mobile screen flows: authentication, proposal list, scoring forms
     and push notifications.
17 | src/store (mobile) | Client state combined with AsyncStorage so the application keeps its
     state offline.
18 | Controllers | Fifty-one REST controllers returning a uniform ApiResponse — AuthController,
     ProposalsController, CyclesController, CouncilsController, CouncilMeetingsController,
     ReviewScoringController, ContractsController, ProgressReportsController and others.
19 | Middleware/GlobalExceptionMiddleware | The single centralised exception handler. Maps
     AppException, ForbiddenException, KeyNotFoundException, ArgumentException and
     InvalidOperationException onto HTTP status codes and a uniform error body.
20 | Startup, Serialization, Program.cs | Application bootstrap: dependency injection, JWT bearer
     authentication, CORS, Swagger, automatic migration on start, a production readiness check,
     and a JSON converter that forces every timestamp to UTC.
21 | Interfaces | Service and repository contracts — forty service interfaces plus the generic
     IRepository<T> and nine typed repositories. Implementation lives in the infrastructure
     layer, never here.
22 | DTOs, Common, Constants, Settings | Request and response models, the ApiResponse envelope
     and AppException, the status and setting-key constants, and the JWT options.
23 | Services (infrastructure) | Fifty-eight service implementations holding the business rules,
     validation and transaction boundaries. Entities are projected to DTOs by hand with LINQ
     projection; no object-mapping library is used.
24 | Repositories, Data, Extensions, Migrations | Nine repositories over Repository.cs, the
     FURPMSDbContext with its seeders, dependency-injection registration, and the EF Core
     migration history.
25 | Entities (domain) | Plain POCO entities in eleven groups — Users, Projects, Proposals,
     Review, Contracts, Progress, Cycles, Financial, MasterData, AI and Logs. No business logic,
     and no dependency on any other project.
```

### Đã sửa những gì

| Dòng cũ | Vấn đề |
|---|---|
| 04–06 `Pages/...` | Thư mục thật là **`features/`**, không phải `Pages/` |
| 10 `Store/themeStore` | ❌ **không tồn tại**. Store thật: `auth.store` · `notification.store` · `ui.store` |
| 16 *"Mobile **Substance** state"* | Chữ vô nghĩa, gõ nhầm |
| 17–19 `Extension/...` | ❌ API **không có** thư mục `Extension/` |
| 20–23 controller số ít | Tên thật số nhiều; `ReviewController` → `ReviewScoringController`, `ReportController` → `ProgressReportsController` |
| 24–25 hai middleware | ❌ chỉ có **một** |
| 26 `AutoMapper` | ❌ **không cài** |
| 28–29 Service Interface/Implement | Interface ở **Application**, implementation ở **Infrastructure** |
| *(thiếu)* | **`Entities` (Domain)** — cả một project bị bỏ quên |

⚠️ **Dòng 12–17 (mobile)** giữ theo bản cũ vì chưa xem được repo mobile. Nhờ bạn làm mobile
đối chiếu tên thư mục và thư viện. Riêng **dòng 16** — mobile có màn chấm điểm thật chưa?

---

## C. Figure 2 — caption

Phụ thuộc quyết định ở mục B1.

- **Giữ mobile:** caption hiện tại đúng, không đổi. Nhưng **hình** phải thật sự vẽ đủ 3 nhánh
  (web · mobile · backend) và nhánh mobile phải trỏ được tới repo thật.
- **Bỏ mobile:** sửa caption thành
  ```
  Figure 2. Package structure of the frontend and backend.
  ```
  và bỏ nhánh mobile khỏi hình.

Tương tự cho **Figure 1 (System Architecture)** — nếu giữ mobile thì phải có node mobile
nối vào API, và trả lời được *"cho xem app"*.

---

## D. Lỗi chính tả + tiêu đề mục

| Chỗ | Đang ghi | Sửa thành |
|---|---|---|
| Mục 3.2.1 | `Class Diagram (Reseach Cycle)` | `Class Diagram (Research Cycle)` |
| Mục 3.4.2 | `Reviewer Assignment & Invitation Workflow` | `Sending Council Invitations` |
| Mục 3.6.2 | `Proposal Review & Scoring Workflow` | `Submitting a Review Ballot` |
| Mục 3.7.2 | `Contract & Reporting Workflow` | `Creating and Signing a Contract` |

Ba mục sau đổi tên vì đã tách đôi — chữ *Workflow* ngụ ý gộp nhiều bước, giờ mỗi hình
chỉ còn một use case.

**Thêm 3 mục con mới:**

```
3.4.3 Responding to a Council Invitation
3.6.3 Drafting and Approving the Minutes
3.7.3 Submitting a Progress Report
```

---

## E. 🟡 Thêm đoạn diễn giải cho mục 3 — góp ý của Gemini là ĐÚNG

Hiện tại mỗi mục chỉ có một câu rồi dán hình. Cẩm nang không có gạch ghi thẳng
*"phải có N đoạn văn"*, nhưng **ba gạch này cùng chỉ về đó**:

- **§1.5** — *"Không mô tả giải thuật/thuật toán đang dùng trong tài liệu"*
- **§2.6** — *"Sơ đồ không có chú thích... người đọc không nhận ra đó là sơ đồ gì"*
- **§13 câu 3** — *"Chỉ vào sơ đồ kiến trúc và mở đoạn code tương ứng"* ← đoạn văn nêu đúng
  tên lớp/file chính là thứ giúp trả lời câu này

Dưới đây là đoạn văn cho từng mục — **dán ngay dưới câu "Figure N shows…"**.

### 3.1.1 — Class diagram (authentication)

```
Authentication follows the project's N-tier convention: Controller to Service to Repository to
DbContext. AuthController exposes five endpoints and holds no logic of its own; it delegates
every call to IAuthService. AuthService is the only implementation and depends on exactly three
abstractions: IUserRepository for data access, IJwtService for token generation, and INotifier
for the password-reset mail. Password verification uses BCrypt, and the reset token is stored
only as a SHA-256 hash with a thirty-minute lifetime, so a leaked database row cannot be replayed.
Errors are raised as AppException and translated into HTTP status codes by
GlobalExceptionMiddleware rather than by the controller.
```

### 3.1.2 — Sequence (login)

```
On the happy path the browser posts an email and password to /api/auth/login. AuthService loads
the user together with the assigned roles in a single query, verifies the password with BCrypt,
stamps LastLoginAt, and asks IJwtService for a signed token carrying the role claims. The
response is a LoginResponse wrapped in ApiResponse, and the front end stores the user in the
Zustand auth store before redirecting by role.

Three failures are handled explicitly and all of them return HTTP 401 so that the response never
reveals which half of the credentials was wrong: the account does not exist or has no password
hash; BCrypt verification fails; or the account status is not ACTIVE. The first two produce the
error code INVALID_CREDENTIALS, the third produces ACCOUNT_INACTIVE with a different message so
the user knows to contact an administrator.
```

### 3.2.1 — Class diagram (research cycle)

```
CyclesController delegates to ICycleService, implemented by CycleService. The service depends on
ICycleRepository for cycles and fields, IMasterDataRepository for research types, and
IDeadlineResolver for the effective submission deadline. A ResearchCycle carries exactly one
ResearchType, and the many-to-many link between a cycle and the research fields it opens is
modelled by the CycleTrack junction entity. Deadline changes are never written over the original
date: every extension is appended to DeadlineExtension, and IDeadlineResolver computes the
effective deadline from that log.
```

### 3.2.2 — Sequence (create & open cycle)

```
Creating a cycle is a two-step administrative flow. The service first resolves the research type,
then runs ValidateCycleAsync, which rejects a deadline earlier than the opening date and rejects
a duplicate cycle for the same year and research type. Validation lives in the service layer, not
in the form, so calling the API directly cannot bypass it. A new cycle is written with status
PLANNING; proposals can only be submitted once the administrator opens it, which flips the status
to OPEN in a separate request.

Two failure paths are shown: an unparsable year or date returns HTTP 400, and an unknown research
type returns HTTP 404.
```

### 3.3.1 — Class diagram (proposal submission)

```
ProposalService is the largest service in the system and takes thirteen dependencies; the diagram
lists all of them on the class but draws associations only to the five that take part in
submission. The domain model separates Project from Proposal: Project is the root entity that
carries the lifecycle status, while Proposal is a versioned document belonging to it. Only one
version is current at a time, so a revision creates version n+1 instead of overwriting the
previous text, and the review history stays intact. IBudgetPolicyService enforces the funding
ceiling defined by QD543 Article 14, and IDecisionLogger appends a row to the decision register
for every state change.
```

### 3.3.2 — Sequence (submit proposal)

```
Submission is guarded by five checks, applied in order: the caller must be the principal
investigator of the project; the proposal must still be a DRAFT; the effective submission
deadline must not have passed; the budget total must stay within the cap for the research type;
and the academic CV must have been updated within the last six months, unless the investigator
explicitly confirms it is still accurate.

The deadline check is worth noting. It does not read ResearchCycle.SubmissionDeadline directly.
It asks IDeadlineResolver for the effective deadline, which takes any recorded extension into
account. Reading the raw column would let the system notify an investigator that a deadline has
been extended and then reject the submission against the old date.

When all checks pass, the proposal becomes SUBMITTED, the project moves to UNDER_REVIEW, a row is
written to the decision register, and editing is locked.
```

### 3.4.1 — Class diagram (council setup)

```
CouncilService owns the whole invitation life cycle and depends on IReviewRepository,
ISystemSettingService for the confirmation window, INotificationRepository for in-app
notifications, IEmailService for outbound mail, and IDecisionLogger. A ReviewCouncil holds
CouncilMember rows, each of which carries its own invitation token, expiry and response status,
and CouncilProjectAssignment rows that record which projects the council evaluates. One
confirmation from a member applies to every project assigned to that council.
```

### 3.4.2 — Sequence (send invitations)

```
Invitations cannot be sent until the council is complete. Six gates are checked before a single
message leaves the system: a Chair must be assigned; a Secretary must be assigned; a meeting must
already be scheduled with a date and a location or link; at least one project must be assigned;
the member count must reach the minimum required by QD543 Article 8.2 for review councils and
Article 12.2 for acceptance councils; and the member count must be an odd number.

The odd-number rule is specific to this system. Every member votes, including the Secretary, and
the Secretary drafts the conclusion from the difference between pass and fail ballots. An even
council can tie, which would leave no basis for the written conclusion.

Once the gates pass, each pending member receives a single-use token with an expiry taken from
the COUNCIL_INVITE_DEADLINE_DAYS setting, an in-app notification, and an email.
```

### 3.4.3 — Sequence (reviewer responds) 🆕

```
Answering an invitation is a separate use case: a different actor, a different endpoint, and
typically several days later. The reviewer opens the invitations page and accepts or declines.

Six outcomes are modelled. A member id that does not exist returns HTTP 404. Answering an
invitation addressed to somebody else returns HTTP 403. An invitation already declined cannot be
answered again. If the confirmation window has closed, the membership is marked EXPIRED and the
request is rejected, which is the signal for staff to find a replacement. Accepting within the
window sets the status to CONFIRMED with a timestamp, and declining records the reason so the
research office can see why a reviewer withdrew.
```

### 3.5.1 — Class diagram (meeting management)

```
CouncilMeetingService is deliberately thin and depends on a single abstraction,
IReviewRepository. A CouncilMeeting belongs to one ReviewCouncil and records attendance through
MeetingAttendance. Only two meeting modes exist, ONLINE and IN_PERSON. The system does not
integrate with any conferencing provider: for an online meeting staff paste a joining link of
their choice into MeetingLink, and for an in-person meeting they fill Location instead. Legacy
rows that still carry provider-specific values are mapped to ONLINE when read.
```

### 3.5.2 — Sequence (schedule meeting)

```
Scheduling validates the request before writing anything. The meeting must be in the future, the
duration must be greater than zero, and the chosen slot must not overlap a meeting that any
member of this council is already attending, which prevents a reviewer being booked twice at the
same hour. Depending on the selected mode the service clears the field that does not apply, so an
online meeting never keeps a stale physical location.

Validation is shared between the create and the update path. Keeping them in one method prevents
the looser of the two becoming a way to bypass the stricter one.
```

### 3.6.1 — Class diagram (evaluation & scoring)

```
Scoring is split across three entities. ProposalReviewScore is one member's ballot for one
project, ReviewScoreDetail holds the score and comment given for each rubric criterion, and
CouncilDecision holds the minutes and the single conclusion for that project. A rubric is scoped
rather than versioned: RubricTemplate declares the maximum total and the research types it
applies to, RubricTemplateScope maps it to a cycle and a research field, and each ballot pins the
TemplateId it was scored against, so changing the active rubric later does not rewrite ballots
already cast.

The design deliberately keeps AverageScore and Result independent. The system computes the
average and counts the ballots, but it never derives the conclusion from them; Result always
records what the Chair decided. Where the conclusion diverges from the score, the service
requires a written justification instead of blocking the decision.
```

### 3.6.2 — Sequence (submit ballot)

```
Before a ballot is accepted the service checks that the council exists, that the minutes for this
project have not already been locked by the Chair, that the caller is a confirmed member of the
council, and that the rubric is usable.

The rubric check is a business rule taken from QD543 form BM03: the active criteria must add up
exactly to the template maximum. A rubric summing to 95 out of 100 makes every derived figure -
the council average, the percentage - meaningless, so it is rejected with an explanatory message
rather than silently accepted. The permitted decimal step for a score is read from the
SCORE_DECIMAL_PLACES setting, so the research office can allow or forbid fractional marks without
a redeploy.
```

### 3.6.3 — Sequence (minutes & chair approval) 🆕

```
The minutes follow QD543 Article 8.3.c: the Secretary writes them and the council approves them.
The Secretary saves a draft, which stays unlocked, and only the Chair can approve it.

Approval is refused if the minutes are already locked, if the caller is not a confirmed Chair, if
there is no draft or more than one draft for the council, or if attendance does not meet the
quorum required by Article 8.3.b. When approval succeeds the decision is stamped and locked, any
meeting still open is closed, and the proposal and project statuses are updated from the result.
An acceptance round behaves differently from a review round: a pass moves the project to
COMPLETED, while a fail returns it to IN_PROGRESS so the work can continue.
```

### 3.7.1 — Class diagram (contracts & reports)

```
Two services share this area. ContractService manages the contract life cycle and
ProgressReportService manages the periodic reports raised against it. A Project may hold several
Contract rows, one per funded phase, and each contract owns its disbursement tranches and its
reporting rounds. Signed contract files are kept through the polymorphic Document entity, which
is also how every other attachment in the system is stored.
```

### 3.7.2 — Sequence (create & sign contract)

```
A contract is created from an approved proposal with status PENDING_SIGNATURE. Signing is guarded
by a rule taken from QD543 form BM05 Article 7.2: the contract is signed electronically outside
this system, so the system refuses to mark it signed until the countersigned copy has been
uploaded as evidence. A future signing date is also rejected. Once signed, the contract becomes
ACTIVE and the project moves to IN_PROGRESS, which is the point at which the delivery schedule
starts running.
```

### 3.7.3 — Sequence (submit progress report) 🆕

```
Reporting rounds are not created by the investigator. Staff generate them in advance from the
contract, and the default number of rounds follows the research type: two for applied research
and one for basic research. The investigator therefore edits an existing DRAFT round rather than
creating a report.

Submission checks that the report exists, that the caller is the principal investigator of the
project, and that the report is still a draft. Once submitted the report is locked and the
research office is notified that a review is waiting.
```

---

## F. ❌ KHÔNG thêm mục State Machine vào RP4

**Bản trước của file này khuyên thêm "mục 4. State Machine Design". Khuyên vậy là sai.**

Template gốc RP4 chỉ có đúng 4 mục:

```
1. System Design
   1.1 System Architecture
   1.2 Package Diagram
2. Database Design
3. Detailed Design
   3.x <Feature Name>
      3.x.1 Class Diagram
      3.x.2 <Sequence Diagram Name1>
      3.x.3 <Sequence Diagram Name2>
```

**Không có mục nào cho State Machine.** Cẩm nang **§1.1**:

> *"Không dùng template chính thức của trường — sai cấu trúc mục, thiếu mục,
> **thêm mục không có trong mẫu**. Hậu quả nặng nhất là **phải làm lại và nộp lại
> toàn bộ tài liệu**."*

Thêm mục 4 để chữa §2.5 sẽ dính §1.1 — hậu quả nặng hơn nhiều.

### Vậy State Machine đặt ở đâu?

Cẩm nang **§2.5** chỉ nói *"nó thuộc phần thiết kế chính"*, **không nói là RP4**.
§12 mốc D-7 liệt kê 8 loại sơ đồ để **rà soát**, không nói cả 8 phải nằm trong một báo cáo.
Thực tế 8 loại đó trải ra: Use Case / Context / Activity thường ở **RP3 (SRS)**;
Class / Sequence / Architecture / ERD ở **RP4**.

**Việc cần làm: mở template RP3 trên FLM, tìm mục nào nhận State Machine.**
Tôi chưa đọc template RP3 nên không khẳng định được. Nếu RP3 cũng không có chỗ,
thì để nguyên ở `Review2_Diagrams.md` làm tài liệu nội bộ, đừng nhét bừa vào RP4.

---

## F2. 🔴 RP4 đang THIẾU một mục mà template CÓ

Template mục 3.x ghi rõ:

> *"It includes **Class Diagram**, **Class Specifications**, and **Sequence Diagram(s)**"*

RP4 hiện tại có Class Diagram và Sequence Diagram, **không có Class Specifications**.
Đây là *"thiếu mục"* theo §1.1 — cùng mức lỗi với thêm mục thừa.

**Class Specifications** = bảng mô tả từng lớp: tên lớp, trách nhiệm, thuộc tính chính,
phương thức chính. Thêm một bảng sau mỗi class diagram, ví dụ cho mục 3.1:

```
Class Specifications

No | Class | Responsibility | Key members
01 | AuthController | Receives authentication requests and returns ApiResponse. Holds no
     business logic. | Login, Me, ChangePassword, ForgotPassword, ResetPassword
02 | IAuthService | Contract for the authentication use cases. | LoginAsync,
     GetCurrentUserAsync, ChangePasswordAsync, ForgotPasswordAsync, ResetPasswordAsync
03 | AuthService | Verifies credentials with BCrypt, stamps LastLoginAt, requests a token and
     sends the reset mail. | _users, _jwt, _notifier, HashToken, MapUserInfo
04 | IUserRepository | Data access for users, roles and academic profiles. Extends
     IRepository<User>. | Query, Roles, UserRoles, AddUserRole, SaveChangesAsync
05 | IJwtService | Issues the signed access token carrying role claims. | GenerateToken,
     ExpiryMinutes
06 | User | Account entity holding credentials, status and the reset-token hash. | Email,
     PasswordHash, Status, LastLoginAt, PasswordResetTokenHash
07 | GlobalExceptionMiddleware | Maps exceptions onto HTTP status codes and a uniform error
     body. | InvokeAsync, Map
```

Sáu bảng còn lại ở **mục F2b** bên dưới. Bảng này cũng là **đạn trả lời** cho câu hỏi
§13 số 3 (*"chỉ vào sơ đồ và mở code tương ứng"*).

---

## F2b. Sáu bảng Class Specifications còn lại

Quy ước chung cho cả 7 bảng:
- Caption đặt **TRÊN** bảng, dùng `Insert Caption` (cẩm nang §1.6)
- Mỗi bảng phải được nhắc tới trong đoạn văn trước khi xuất hiện —
  ví dụ *"Table 4 lists the responsibility of each class."*

### Table 4 — mục 3.2 Research Cycle & Field Management

```
Table 4. Class specifications for research cycle and field management.

No | Class | Responsibility | Key members
01 | CyclesController | Exposes the cycle and research-field endpoints. Holds no business
     logic. | GetCycles, CreateCycle, OpenCycle, CloseCycle, ExtendDeadline, AttachTrack
02 | ICycleService | Contract for managing submission cycles and the research fields opened
     inside them. | GetCyclesAsync, CreateCycleAsync, OpenCycleAsync, CloseCycleAsync,
     ExtendCycleDeadlineAsync, AttachTrackToCycleAsync
03 | CycleService | Validates and persists cycles. Server-side validation lives here so that
     calling the API directly cannot bypass the form. | _cycles, _masterData, _deadlines,
     ValidateCycleAsync, CreateCycleAsync, OpenCycleAsync
04 | ICycleRepository | Data access for cycles, research fields and the cycle-field junction.
     | Query, Tracks, CycleTracks, AddAsync, SaveChangesAsync
05 | IDeadlineResolver | Computes the effective deadline from the original date plus any
     recorded extension. | EffectiveAsync
06 | ResearchCycle | A submission period carrying exactly one research type, its opening date
     and its deadline. | CycleYear, SemesterCode, ResearchTypeId, SubmissionOpenDate,
     SubmissionDeadline, ReviewDeadline, Status
07 | ResearchTrack | A research field used to group proposals. | Code, Name, OwnerId, IsActive
08 | CycleTrack | Junction entity attaching a research field to a cycle. | CycleId, TrackId
09 | ResearchType | Research category with its funding ceiling. | Name, MaxBudgetCap
10 | DeadlineExtension | Append-only log of deadline changes. The original date is never
     overwritten. | TargetType, TargetId, OldDeadline, NewDeadline, Reason
```

### Table 5 — mục 3.3 Research Proposal Submission

```
Table 5. Class specifications for research proposal submission.

No | Class | Responsibility | Key members
01 | ProposalsController | Exposes the proposal endpoints for the principal investigator. |
     Create, Update, Submit, Withdraw
02 | IProposalService | Contract for the proposal lifecycle. | CreateProposalAsync,
     UpdateProposalAsync, SubmitProposalAsync, WithdrawProposalAsync
03 | ProposalService | Applies the five submission gates: ownership, draft status, effective
     deadline, budget cap and CV freshness. Takes thirteen dependencies; only the five used
     during submission are drawn on the diagram. | _proposals, _budgetPolicy, _deadlines,
     _decisions, _embeddings, SubmitProposalAsync
04 | IProposalRepository | Data access for projects, proposals, budgets, activities and
     deliverables. Extends IRepository<Proposal>. | Query, Projects, Budgets, BudgetItems,
     Deliverables, SaveChangesAsync
05 | IBudgetPolicyService | Enforces the funding ceiling defined by QD543 Article 14 at the
     moment of submission. | AssertWithinCapAsync
06 | IDeadlineResolver | Supplies the effective submission deadline including extensions. |
     EffectiveAsync
07 | IDecisionLogger | Appends a row to the decision register for every state change. | Log
08 | IEmbeddingQueue | Queues the proposal for background embedding used by duplicate
     detection. | Enqueue
09 | Project | Root entity of a research project, carrying the status read by the rest of the
     system. | PiUserId, CycleTrackId, OrderId, Status, IsDeleted
10 | Proposal | Versioned proposal document. Only one version is current at a time, so a
     revision creates version n+1 instead of overwriting the text. | ProjectId, VersionNo,
     IsCurrent, Status, SubmittedAt, DurationMonths
11 | ProposalBudget | Budget header holding the total checked against the cap. | ProposalId,
     TotalAmount
```

### Table 6 — mục 3.4 Council Setup & Reviewer Invitation

```
Table 6. Class specifications for council setup and reviewer invitation.

No | Class | Responsibility | Key members
01 | CouncilsController | Exposes council management, project assignment and invitation
     endpoints. | GetCouncils, CreateCouncil, AddMember, SendInvitations, AssignProject,
     RespondToMembership
02 | ICouncilService | Contract for forming a council and running the invitation cycle. |
     CreateCouncilAsync, AddMemberAsync, SendInvitationsAsync, AssignProjectToCouncilAsync,
     RespondToMembershipAsync, GetScheduleConflictsAsync
03 | CouncilService | Applies the six gates checked before any invitation is sent, issues the
     single-use tokens and dispatches notification and mail. | _review, _settings,
     _notifications, _email, _decisions, IsChair, OddSizesFor, SendInvitationsAsync
04 | IReviewRepository | Data access for councils, members, meetings and decisions. | Query,
     CouncilMembers, Meetings, Decisions, SaveChangesAsync
05 | ISystemSettingService | Supplies runtime configuration such as the confirmation window,
     changeable by an administrator without a redeploy. | GetIntAsync, GetBoolAsync
06 | INotificationRepository | Creates in-app notifications and records outbound mail. |
     AddAsync, AddEmailLog, EmailLogs
07 | IEmailService | Sends the invitation mail through the external SMTP provider. | SendAsync
08 | ReviewCouncil | A council formed to evaluate the projects of a review round. |
     EstablishmentDecisionNo, MinMembersRequired, MaxMembersAllowed, RoundId, Status
09 | CouncilMember | Membership row carrying the role, the single-use invitation token, its
     expiry and the response. | UserId, MemberRole, Status, InvitationToken, TokenExpiresAt,
     ConfirmedAt, DeclinedAt, DeclineReason
10 | CouncilProjectAssignment | Records which projects a council evaluates. One confirmation
     applies to every project assigned. | CouncilId, ProjectId
11 | Notification | In-app notification delivered to the invited reviewer. | NotificationType,
     Title, Body, ActionUrl, Priority
```

### Table 7 — mục 3.5 Review Meeting Scheduling

```
Table 7. Class specifications for review meeting management.

No | Class | Responsibility | Key members
01 | CouncilMeetingsController | Exposes meeting scheduling, lifecycle and attendance
     endpoints for staff, principal investigators and council members. | GetAll, GetForPi,
     GetForCouncilMember, Schedule, Update, Start, End, SaveAttendance
02 | ICouncilMeetingService | Contract for scheduling and running council meetings. |
     ScheduleAsync, UpdateAsync, StartAsync, UndoStartAsync, EndAsync, SaveAttendanceAsync
03 | CouncilMeetingService | Validates the slot, rejects overlaps with meetings the same
     members already attend, and normalises the meeting mode. Depends on a single
     abstraction. | _review, ValidateAndNormalize, AssertNoScheduleConflictAsync, Map
04 | IReviewRepository | Data access for councils and meetings. | Query, Meetings,
     CouncilMembers, AddMeetingAsync, SaveChangesAsync
05 | ReviewCouncil | Owns the meetings scheduled for a review round. |
     EstablishmentDecisionNo, RoundId, Status
06 | CouncilMeeting | A scheduled meeting with its mode, joining link or physical location. |
     Title, Platform, MeetingLink, Location, ScheduledAt, DurationMinutes, Agenda, Status
07 | MeetingAttendance | Attendance of each member, with a reason when absent. Feeds the
     quorum check required by QD543 Article 8.3.b. | MeetingId, MemberId, RsvpStatus,
     ActuallyAttended, AbsenceReason
08 | MeetingPlatform | Enumeration of the two supported modes. The system stores a joining
     link supplied by staff and does not integrate with any conferencing provider. | ONLINE,
     IN_PERSON
09 | MeetingStatus | Enumeration of the meeting lifecycle. | SCHEDULED, IN_PROGRESS, COMPLETED
```

### Table 8 — mục 3.6 Proposal Evaluation & Scoring

```
Table 8. Class specifications for proposal evaluation and scoring.

No | Class | Responsibility | Key members
01 | ReviewScoringController | Exposes ballot submission, minutes drafting and chair approval
     endpoints. | SubmitScore, GetCouncilScores, SaveMinutes, ApproveMinutes,
     RequestMinutesRevision, GetBallotTally
02 | IReviewScoringService | Contract for scoring and for the minutes workflow. |
     SubmitScoreAsync, SaveMinutesAsync, ApproveMinutesAsync, RequestMinutesRevisionAsync,
     GetBallotTallyAsync
03 | ReviewScoringService | Validates ballots against the rubric, enforces the quorum and locks
     the minutes on chair approval. The system computes the average but never derives the
     conclusion from it. | _review, _masterData, _settings, _notifier, _decisions, IsChair,
     AssertConfirmed, AssertQuorumAsync, MarkDecidedIfAllProjectsFinalizedAsync
04 | IReviewRepository | Data access for councils, ballots, decisions and meetings. | Query,
     CouncilMembers, Decisions, Meetings, SaveChangesAsync
05 | ISystemSettingService | Supplies the permitted decimal step for a score and the pass
     threshold, both changeable by an administrator. | GetIntAsync, GetBoolAsync
06 | IDecisionLogger | Records the finalised council decision in the decision register. | Log
07 | INotifier | Notifies the principal investigator once the minutes are locked. |
     NotifyAsync
08 | ReviewCouncil | Owns the ballots and the per-project decisions of a round. |
     EstablishmentDecisionNo, MinMembersRequired, Status
09 | ProposalReviewScore | One council member's ballot for one project. | CouncilId,
     ProjectId, EvaluatorMemberId, TemplateId, GeneralComments, SubmittedAt, IsValidBallot
10 | ReviewScoreDetail | The score and comment given for a single rubric criterion. | ScoreId,
     CriterionId, GivenScore, Comments
11 | CouncilDecision | The minutes and the single conclusion for one project. Result always
     records what the chair decided; a divergence from the average requires a written
     justification. | ProjectId, TotalMembers, AttendingMembers, ValidBallots, AverageScore,
     Result, ResultJustification, ChairUserId, FinalizedAt
12 | RubricTemplate | Versioned scoring rubric. Its active criteria must add up exactly to
     MaxTotalScore, following QD543 form BM03. | TemplateType, Name, MaxTotalScore, TrackId,
     IsActive
13 | RubricCriterion | A single criterion with its own maximum score. | TemplateId,
     CriterionName, MaxScore, Sequence, IsActive
14 | ReviewRound | A review round opened for one research field within one cycle. |
     CycleTrackId, Dimension, Status, ScoringDeadline
```

### Table 9 — mục 3.7 Research Contract & Report Submission

```
Table 9. Class specifications for research contracts and reporting.

No | Class | Responsibility | Key members
01 | ContractsController | Exposes contract creation, signing and termination endpoints. |
     GetList, Create, Sign, Terminate
02 | ProgressReportsController | Exposes the periodic reporting endpoints for the principal
     investigator and the research office. | GetByContract, Submit, Evaluate
03 | IContractService | Contract for the agreement lifecycle. | CreateAsync, UpdateAsync,
     SignAsync, TerminateAsync
04 | ContractService | Creates the agreement from an approved proposal and refuses to mark it
     signed until the countersigned copy has been uploaded, following QD543 form BM05
     Article 7.2. | _contracts, _documents, _settings, _notifier, _decisions,
     QueryWithProject, SignAsync
05 | IProgressReportService | Contract for generating, editing, submitting and evaluating the
     periodic reports. | GenerateScheduledRoundsAsync, CreateAsync, UpdateAsync, SubmitAsync,
     EvaluateAsync
06 | ProgressReportService | Generates the reporting rounds for a contract and applies the
     ownership and draft-status checks on submission. | _contracts, _documents, _notifier,
     _decisions
07 | IContractRepository | Data access for contracts, disbursements and progress reports. |
     Query, Disbursements, ProgressReports, AddAsync, SaveChangesAsync
08 | IDocumentRepository | Data access for the polymorphic attachment store used to hold the
     signed contract copy. | Query, AddAsync
09 | IDecisionLogger | Records the signing event in the decision register. | Log
10 | INotifier | Notifies the principal investigator when the contract becomes active, and the
     research office when a report is submitted. | NotifyAsync
11 | Project | Owns one contract per funded phase and carries the status that moves to
     IN_PROGRESS once the agreement is signed. | PiUserId, Status
12 | Contract | Research contract generated from an approved proposal. | ContractNumber,
     ScopeTitle, TotalAmount, StartDate, EndDate, SignedAt, Status
13 | ContractDisbursement | A disbursement tranche with its unlocking condition. Money is
     never released automatically; staff confirm each tranche. | RoundNumber, Percentage,
     PlannedAmount, ActualAmount, ConditionMetAt, Status
14 | ProgressReport | A periodic report raised against a contract. Rounds are generated in
     advance, so the investigator edits an existing draft rather than creating a report. |
     ContractId, DueDate, SubmittedAt, Status, EvaluationResult
15 | Document | Polymorphic attachment. EntityType and EntityId identify what each file
     belongs to. | EntityType, EntityId, DocumentCategory, OriginalFileName, StorageUrl,
     IsDeleted
```

---

## F3. 🔴 Table 2 sai định dạng so với template

Template yêu cầu:

```
No | Table | Description
01 | <Table name> | <Description of the table>
                   - Primary keys: <<list of primary key fields>>
                   - Foreign keys: <<list of foreign key fields>>
```

RP4 hiện tại dùng cột: `No | Table | Cols | FKs | Description` — **đếm số lượng** khoá ngoại
thay vì **liệt kê tên trường** khoá chính/khoá ngoại.

Đây là lệch template (§1.1). Hai lựa chọn:

- **Đúng chuẩn:** đổi lại 3 cột, mỗi dòng ghi rõ `- Primary keys: id` và
  `- Foreign keys: project_id, cycle_track_id, ...`
- **Giữ nguyên:** nếu GVHD đã duyệt định dạng hiện tại thì giữ, nhưng nên hỏi trước.

Tên trường khoá lấy từ `docs/ERD_v3_Project_Centric.dbml`.

---

## G. 🔴 Figure 3 (ERD) — phải chụp từ DB thật

Cẩm nang **§2.3** có hai gạch đánh trúng chỗ này:

> *"Database Design phải là **ảnh chụp từ CSDL thật**, không phải hình tự vẽ lý tưởng hóa"*
> *"Nộp **ERD Conceptual** trong khi yêu cầu là **Physical**"*

**Cách làm:** mở `docs/ERD_v3_Project_Centric.dbml` → dán vào **dbdiagram.io** → chụp/xuất ảnh.
File đó sinh từ schema thật nên đúng mức Physical.

**Không dùng** `docs/report/diagrams/erd-conceptual.mmd` — nó là mức Conceptual và còn
sai tên bảng (`RESEARCH_FIELD`, `EVALUATION` không tồn tại trong database).

### Table 2 — dòng 58 mô tả sai

Dòng `rubric_templates` hiện ghi *"**Versioned** scoring rubrics. The **version** active when a
proposal is created is pinned to it."*

`grep Version` trong `RubricTemplate.cs` và `RubricCriterion.cs` = **0 kết quả**. Không có trường
version nào. Rubric được **scope** (theo loại đề tài · lĩnh vực · đơn đặt hàng), không versioned.

Sửa thành:

```
58 | rubric_templates | 9 | 0 | Scoring rubrics, scoped by research type and optionally by
     research field or ordering unit. Each ballot pins the template it was scored against.
```

### Table 2 — kiểm lại số bảng

Bảng đang liệt kê **63 bảng**. `FURPMSDbContext.cs` khai báo **65 `DbSet<>`**.
Rà lại xem thiếu 2 bảng nào, hoặc sửa con số cho khớp.
Cẩm nang §6: *"Số liệu tổng hợp... sai và không nhất quán"*.

---

## H. Cấu trúc file chữ — đánh giá theo cẩm nang

### Đang đạt ✅

| Gạch cẩm nang | |
|---|---|
| §1.2 — có List of Figures, List of Tables tách riêng | ✅ |
| §1.6 — caption hình ở **dưới** hình | ✅ |
| §1.6 — caption bảng ở **trên** bảng | ✅ |
| §1.6 — mọi hình đều được nhắc tới trong bài trước khi xuất hiện | ✅ |
| §1.2 — có Record of Changes | ✅ (nhưng đang rỗng, xem dưới) |
| Cấu trúc mục theo template FLM | ✅ |

### Còn thiếu ❌

| Gạch cẩm nang | Việc |
|---|---|
| §1.1 — **thiếu mục có trong template**: Class Specifications | Mục F2 ở trên |
| §1.1 — **Table 2 sai định dạng template** (đếm số thay vì liệt kê tên khoá) | Mục F3 ở trên |
| §1.2 — **bảng từ viết tắt** (Glossary / Abbreviations) | Chưa có. Thêm sau List of Tables: JWT, DTO, EF Core, PI, QLKH, BM03/BM05… |
| §1.2 — **số trang** | Kiểm lại có chưa |
| §2.3 — ERD phải chụp từ DB thật | Mục G ở trên |
| §1.2 — Record of Changes đang **rỗng hoàn toàn** | Điền tối thiểu 1 dòng: ngày · A/M/D · người phụ trách · mô tả |
| §1.3 — *"Chèn sơ đồ ở kích thước quá nhỏ, không đọc được chữ bên trong"* | Sequence 25–37 message → chèn ở **trang ngang (landscape)** |

### Ghi chú của anh trong file

Mấy dòng *"Có 2 hình cần sửa"*, *"Cũng có 2 hình cần sửa"* — nhớ **xoá hết trước khi nộp**.
Cẩm nang §1.3: *"Chưa xóa các phần gợi ý / tag của template để nguyên trong bài nộp"*.

---

## I0. BẢNG TRẠNG THÁI — còn gì phải làm cho RP4

### Phần CHỮ — đã soạn sẵn, chỉ việc dán

| # | Việc | Nguồn | Xong? |
|---|---|---|---|
| 1 | **Table 1** — 25 dòng thay cho 29 dòng cũ | mục B6 | ☐ |
| 2 | **Table 2** — thêm `project_decisions` (35) và `user_research_tracks` (63), đánh lại 01–65 | mục G | ☐ |
| 3 | **Tables 3–9** — Class Specifications cho 7 khu vực | mục F2 + F2b | ☐ |
| 4 | **17 đoạn diễn giải** cho mục 3.1–3.7 | mục E | ☐ |
| 5 | Đổi tên 3 mục con + thêm 3 mục con mới (3.4.3 · 3.6.3 · 3.7.3) | mục D | ☐ |
| 6 | Sửa chính tả `Reseach` → `Research` (mục 3.2.1) | mục D | ☐ |

### Phần HÌNH

| # | Hình | Tình trạng | Xong? |
|---|---|---|---|
| 7 | **Figure 1** System Architecture | ⚠️ chưa làm — thiếu Cloudinary, giữ nhánh mobile | ☐ |
| 8 | **Figure 2** Package | ✅ có 2 bản: `package-diagram.html` hoặc mermaid chi tiết | ☐ |
| 9 | **Figure 3** ERD | ⚠️ chưa làm — dán `.dbml` vào dbdiagram.io rồi chụp | ☐ |
| 10 | **Figure 4–20** Class + Sequence (17 hình) | ✅ mermaid sẵn ở `RP4_Hinh_Da_Sua.md`; đã import ~10 vào draw.io | ☐ |
| 11 | Sửa nhãn các hình đã import | ✅ danh sách ở mục I1 | ☐ |

## I1. HÌNH NÀO PHẢI SỬA — danh sách đầy đủ

### Không đụng gì (4 hình)

`Class — Authentication` · `Sequence — Login` · `Class — Proposal Submission` ·
`Sequence — Create & sign contract`

### Import lại (3 hình) — đổi quá nhiều, sửa tay không đáng

| Hình | Vì sao |
|---|---|
| **Sequence — Tạo & mở đợt** | Actor `Staff` → `Admin / Staff`, thêm lane 409 trùng tên đợt |
| **Sequence — Nộp báo cáo tiến độ** | Viết lại hẳn: `SubmitAsync` nhận `reportId`, 3 bước generate → edit → submit |
| **Class — Chấm điểm** | 6 lớp đổi tên thuộc tính cùng lúc |

### Sửa nhãn tay (9 hình)

| Hình | Sửa gì |
|---|---|
| **Sequence — Nộp đề cương** | message 2: `confirmCvUpToDate` → **`confirmCv`** |
| **Sequence — Đặt lịch họp** | cột: `meetingPage:MeetingSchedulePage` → **`scheduleSheet:ScheduleMeetingSheet`** |
| **Sequence — Gửi thư mời** | cột: `councilPage:CouncilDetailPage` → **`membersPanel:CouncilMembersPanel`**<br>route: → **`POST /api/councils/{councilId}/send-invitations`** |
| **Sequence — Reviewer trả lời** | cột: `MyInvitationsPage` → **`InvitationsPage`**<br>route: → **`PATCH /api/council-members/{memberId}/respond`** |
| **Sequence — Uỷ viên chấm phiếu** | route: → **`POST /api/review-scoring/councils/{councilId}/scores`** |
| **Sequence — Biên bản & duyệt** | 2 route: → **`/api/review-scoring/councils/{councilId}/minutes`** và **`.../minutes/approve`** |
| **Class — Đợt nghiên cứu** | `DeadlineExtension`: `int Id` → **`Guid Id`**, thêm `OldDeadline` |
| **Class — Hội đồng** | `ReviewCouncil`: `DecisionNumber` → **`EstablishmentDecisionNo`**<br>`CouncilMember`: thêm `ConfirmedAt`, `DeclinedAt`, `DeclineReason` |
| **Class — Lịch họp** | `ReviewCouncil`: như trên<br>`MeetingAttendance`: `IsPresent` → **`ActuallyAttended`**, thêm `RsvpStatus` |
| **Class — Hợp đồng** | `Document`: `FileUrl` → **`StorageUrl`**, thêm `DocumentCategory`, `OriginalFileName` |

> Tìm theo **nội dung chữ**, đừng tìm theo số message — số có thể lệch tuỳ lúc import.

### Phần ĐỊNH DẠNG — cẩm nang §1

| # | Việc | Gạch | Xong? |
|---|---|---|---|
| 12 | ~~Thêm bảng từ viết tắt~~ → **để dành cho RP7**, xem `RP7_Phu_Luc_De_Danh.md` | §1.1 vs §1.2 | ✅ đã quyết |
| 13 | Điền **Record of Changes** — nội dung sẵn ở **mục J** dưới | §1.2 | ☐ |
| 14 | Xoá ghi chú cá nhân (*"Có 2 hình cần sửa"*…) | §1.3 | ☐ |
| 15 | Mọi sơ đồ để **trang ngang**, phóng to đủ đọc chữ | §1.3 | ☐ |
| 16 | Bảng căn **trái**, không justify (khoảng trắng thừa) | §1.3 | ☐ |
| 17 | Caption dùng `Insert Caption`, rồi `Ctrl+A` → `F9` | §1.6 | ☐ |
| 18 | Mỗi hình/bảng phải được nhắc trong đoạn văn trước khi xuất hiện | §1.6 | ☐ |

### Cần người khác xác nhận

| # | Việc | Ai |
|---|---|---|
| 19 | Đối chiếu 6 dòng mobile (12–17 Table 1) với repo thật | bạn làm mobile |
| 20 | Table 2 giữ định dạng `Cols/FKs` hay đổi về 3 cột theo template? | hỏi GVHD |

---

## I. Thứ tự làm

1. **Table 1** — xoá 6 dòng mobile, 1 dòng AutoMapper, sửa 3 dòng Extension, gộp 2 dòng
   middleware, sửa 7 tên controller ← nguy hiểm nhất, làm trước
2. **Figure 2 caption** — bỏ chữ *mobile client*
3. **Sửa chính tả** `Reseach` → `Research`
4. **Dán 17 đoạn diễn giải** ở mục E
5. **Đổi tên 3 mục con + thêm 3 mục con mới** ở mục D
6. **Thêm bảng Class Specifications** cho cả 7 feature ← template đòi, đang thiếu (mục F2)
7. **Thay Figure 3** bằng ảnh chụp từ dbdiagram.io (mục G)
8. **Quyết định Table 2**: đổi về định dạng template hay giữ nguyên (mục F3)
9. **Điền Record of Changes** (mục J). Bảng viết tắt + nguồn tham khảo **không thêm vào RP4** — để dành RP7, xem `RP7_Phu_Luc_De_Danh.md`
10. **Xoá ghi chú cá nhân** (*"Có 2 hình cần sửa"*…)
11. `Ctrl+A` → `F9` đánh lại toàn bộ số hiệu và danh mục

> **Không thêm mục State Machine vào RP4** — xem mục F.

---

## J. Record of Changes — mục đầu tiên của RP4

> **Không bịa ngày.** Cẩm nang §9 ghi *git history là căn cứ chứng minh đóng góp*,
> §13 câu 16 ghi *mở template ra đối chiếu*. Ngày bịa mà lệch repo là lộ; ngày thật
> thì hỏi gì cũng trả lời được. Lịch sử thật của nhóm đã đủ dày — 3 đợt sửa lớn.
>
> `git log` trên `docs/report/` trả về rỗng (thư mục không được track), nên ngày
> trong ngoặc phải tự điền theo trí nhớ. Ngày **in đậm** là ngày thật có bằng chứng
> trong repo.

| Date | A/M/D | In charge | Change Description |
|---|---|---|---|
| *(08/2026)* | A | | Initial version: system architecture, package diagram, database design and seven detailed-design sections. |
| **16/08/2026** | M | | Internal review against the FLM template. Recorded missing captions, missing List of Figures and List of Tables, and five technology mismatches in the architecture diagram. |
| *(08–09/2026)* | A | | Added List of Figures, List of Tables and figure captions; linked every figure and table by cross-reference. |
| *(09/2026)* | M | | Rebuilt the architecture diagram against the real stack: PostgreSQL, Gemini, Brevo, Cloudinary. |
| *(09/2026)* | M | | Replaced the ERD with a physical schema exported from the live database. |
| **19/09/2026** | M | | Rewrote Table 1 from the real repository layout. Removed packages that do not exist in the source (AutoMapper, Unit of Work, generic repository, extension namespaces) and added the domain entity project. |
| **20/09/2026** | M | | Updated Table 2 to 65 tables, adding `project_decisions` and `user_research_tracks`. |
| **20/09/2026** | M | | Redrew all class and sequence diagrams from the current source; corrected class names, entity properties and API routes. |
| **20/09/2026** | A | | Split three combined sequence diagrams into separate use cases (3.4.3, 3.6.3, 3.7.3) and added Class Specifications tables 3-9. |
| **21/09/2026** | M | | Removed remaining references to features not implemented in the product and aligned wording between sections 1.1, 3.5 and 3.6. |

**Cột `In charge`:** ghi tên người thật, và **đừng để một tên cho cả 10 dòng** —
cẩm nang §9 gạch *"không phân biệt được ai làm phần nào"*. Chia theo nội dung dòng:

| Dòng | Thuộc về ai |
|---|---|
| Table 1, Table 2, class/sequence, Class Specifications, ERD | người đọc code **BE** |
| Hình kiến trúc | người vẽ hình |
| Caption / LoF / LoT / rà template | người soát định dạng |

**`A/M/D`** = Added / Modified / Deleted.
