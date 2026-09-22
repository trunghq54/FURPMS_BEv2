# RP4 — Detailed Design diagrams rebuilt from real code

> Mở bằng **VS Code → Ctrl+Shift+V** (Markdown Preview) để xem hình render.
> Nhãn trong hình để **tiếng Anh** vì Report 4 là báo cáo tiếng Anh.
>
> **Quy tắc bắt buộc:** Class Diagram và Sequence Diagram của cùng một khu vực phải sửa
> **thành một cặp**. Cẩm nang có gạch riêng *"Sequence không khớp với kiến trúc class đã mô tả"* —
> sửa một cái mà bỏ cái kia là tạo ra lỗi mới.

## Lớp KHÔNG tồn tại — xoá khỏi mọi hình

Hình cũ vẽ **Unit of Work pattern**. `CLAUDE.md` dòng 7 tuyên bố pattern thật:
`N-tier: Controller → Service → Repository → DbContext`.

| Lớp trong hình cũ | Thực tế (`grep` = 0 file) |
|---|---|
| `BaseAPIController` / `BaseApiController` | ❌ — controller kế thừa thẳng `ControllerBase` |
| `BaseService` | ❌ — service không có lớp cha |
| `IUnitOfWork` / `UnitOfWork` | ❌ |
| `IGenericRepository` / `GenericRepository` | ❌ — tên thật `IRepository<T>` |
| `IMapper` | ❌ — AutoMapper không được cài, mapping làm tay |
| `ITokenService` / `TokenService` | ❌ — tên thật `IJwtService` |
| `IAIService` / `AIService` | ❌ — tên thật `IAiAdvisorService` |
| `IUserContext` / `UserContext` | ❌ |

## Bản đồ 7 khu vực

| Mục RP4 | Class | Sequence |
|---|---|---|
| 3.1 User Authentication & Session Management | Figure 4 | Figure 5 |
| 3.2 Research Cycle & Field Management | Figure 6 | Figure 7 |
| 3.3 Research Proposal Submission | Figure 8 | Figure 9 |
| 3.4 Council Setup & Reviewer Invitation | Figure 10 | Figure 11 (gửi thư mời) + Figure 11b (reviewer trả lời) |
| 3.5 Review Meeting Scheduling | Figure 12 | Figure 13 |
| 3.6 Proposal Evaluation & Scoring | Figure 14 | Figure 15 (chấm phiếu) + Figure 15b (biên bản) |
| 3.7 Research Contract & Report Submission | Figure 16 | Figure 17 (ký HĐ) + Figure 17b (báo cáo tiến độ) |

> **Tổng: 7 class + 9 sequence = 16 hình.** Quy tắc tách một sequence:
> **đổi actor VÀ cách thời gian** — hoặc khi hình vượt ~35 message.

---

## 1.2 — Package Diagram (Figure 2)

Hai bản, chọn một:

- **`docs/report/diagrams/rp4-moi/package-diagram.html`** — 7 hộp, gọn, có legend.
  Xuất SVG/PNG rồi chèn Word. Hợp slide hơn.
- **Mermaid dưới đây** — chi tiết tới cấp thư mục, import vào draw.io sửa được,
  đồng bộ phong cách với 17 hình class/sequence.

```mermaid
flowchart LR
    subgraph WEB["FURPMS-Web"]
        direction TB
        subgraph WEBC["components/"]
            W1[ui]
            W2[layout]
            W3[shared]
            W4[charts]
            W5[tables]
        end
        subgraph WEBF["features/"]
            W6[auth]
            W7[admin]
            W8[staff]
            W9[pi]
            W10[reviewer]
            W11[dashboard]
            W12[analytics]
        end
        subgraph WEBS["services/api/"]
            W13[axiosClient]
            W14["58 service wrappers"]
        end
        subgraph WEBST["store/"]
            W15[auth.store]
            W16[notification.store]
            W17[ui.store]
        end
        W18["i18n · hooks · types"]
    end

    subgraph MOB["FURPMS.Mobile"]
        direction TB
        subgraph MOBA["app/"]
            M1[auth]
            M2[pi]
            M3[reviewer]
        end
        subgraph MOBS["src/"]
            M4[components]
            M5[features]
            M6["services/http"]
            M7["services/secureStore"]
            M8[store]
        end
    end

    subgraph API["FURPMS.API"]
        direction TB
        subgraph APIC["Controllers/ — 51"]
            A1[AuthController]
            A2[ProposalsController]
            A3[CyclesController]
            A4[CouncilsController]
            A5[CouncilMeetingsController]
            A6[ReviewScoringController]
            A7[ContractsController]
            A8[ProgressReportsController]
        end
        A9["Middleware/<br/>GlobalExceptionMiddleware"]
        A10["Startup/<br/>DatabaseStartup · ProductionReadinessCheck"]
        A11["Serialization/<br/>UtcDateTimeConverter"]
        A12["Program.cs"]
    end

    subgraph APP["FURPMS.Application"]
        direction TB
        subgraph APPI["Interfaces/"]
            P1["Services/ — 40 contracts"]
            P2["Repositories/ — IRepository + 9"]
        end
        P3["DTOs/"]
        P4["Common/<br/>ApiResponse · AppException"]
        P5["Constants/<br/>DomainStatus · SystemSettingKeys"]
        P6["Settings/<br/>JwtSettings"]
    end

    subgraph INFRA["FURPMS.Infrastructure"]
        direction TB
        subgraph INFS["Services/ — 58"]
            I1[AuthService]
            I2[ProposalService]
            I3[CycleService]
            I4[CouncilService]
            I5[ReviewScoringService]
            I6[ContractService]
        end
        subgraph INFR["Repositories/"]
            I7["Repository.cs"]
            I8["9 repositories"]
        end
        subgraph INFD["Data/"]
            I9[FURPMSDbContext]
            I10[DatabaseSeeder]
            I11[DemoScenarioSeeder]
        end
        I12["Extensions/<br/>DependencyInjection · PostgresConnectionString"]
        I13["Migrations/"]
    end

    subgraph DOM["FURPMS.Domain"]
        direction TB
        subgraph DOME["Entities/"]
            D1[Users]
            D2[Projects]
            D3[Proposals]
            D4[Review]
            D5[Contracts]
            D6[Progress]
            D7[Cycles]
            D8[Financial]
            D9[MasterData]
            D10[AI]
            D11[Logs]
        end
    end

    subgraph TST["FURPMS.Tests"]
        T1["311 xUnit tests · 43 files"]
    end

    WEB -.->|HTTPS + JWT| API
    MOB -.->|HTTPS + JWT| API
    API -.-> INFRA
    API -.-> APP
    INFRA -.-> APP
    INFRA -.-> DOM
    APP -.-> DOM
    TST -.-> INFRA
    TST -.-> APP
```

> Chiều mũi tên là **phụ thuộc**. `FURPMS.Domain` không có mũi tên đi ra — đúng nguyên tắc
> N-tier, tầng trong cùng không phụ thuộc ai. Đây là câu trả lời sẵn nếu bị hỏi.

---

## 3.1 — User Authentication & Session Management

### Figure 4 — Class Diagram

```mermaid
classDiagram
    direction LR

    class AuthController {
        -IAuthService _authService
        +Login(LoginRequest) Task~ActionResult~
        +Me() Task~ActionResult~
        +ChangePassword(ChangePasswordRequest) Task~ActionResult~
        +ForgotPassword(ForgotPasswordRequest) Task~ActionResult~
        +ResetPassword(ResetPasswordRequest) Task~ActionResult~
    }

    class IAuthService {
        <<interface>>
        +LoginAsync(LoginRequest) Task~LoginResponse~
        +GetCurrentUserAsync(Guid) Task~UserInfoResponse~
        +ChangePasswordAsync(Guid, ChangePasswordRequest) Task
        +ForgotPasswordAsync(ForgotPasswordRequest) Task
        +ResetPasswordAsync(ResetPasswordRequest) Task
    }

    class AuthService {
        -IUserRepository _users
        -IJwtService _jwt
        -INotifier _notifier
        -ResetTokenLifetime TimeSpan
        -HashToken(string) string
        -MapUserInfo(User, List~string~) UserInfoResponse
        +LoginAsync(LoginRequest) Task~LoginResponse~
    }

    class IRepository~T~ {
        <<interface>>
        +Query() IQueryable~T~
        +GetByIdAsync(object) Task~T~
        +AddAsync(T) Task
        +Update(T) void
        +SaveChangesAsync(CancellationToken) Task
    }

    class IUserRepository {
        <<interface>>
        +Roles IQueryable~Role~
        +UserRoles IQueryable~UserRole~
        +AcademicProfiles IQueryable~AcademicProfile~
        +AddUserRole(UserRole) void
    }

    class IJwtService {
        <<interface>>
        +GenerateToken(User, IEnumerable~string~) string
        +ExpiryMinutes int
    }

    class INotifier {
        <<interface>>
        +NotifyAsync(Guid, string, string, string, string) Task
    }

    class User {
        +Guid Id
        +string Email
        +string PasswordHash
        +string Status
        +DateTime? LastLoginAt
        +string? PasswordResetTokenHash
        +DateTime? PasswordResetExpiresAt
        +ICollection~UserRole~ UserRoles
    }

    class LoginRequest {
        +string Email
        +string Password
    }

    class LoginResponse {
        +string AccessToken
        +string TokenType
        +int ExpiresIn
        +UserInfoResponse User
    }

    class AppException {
        +int StatusCode
        +string Code
        +string Message
    }

    class GlobalExceptionMiddleware {
        +InvokeAsync(HttpContext) Task
        -Map(Exception) tuple
    }

    AuthService ..|> IAuthService : realize
    IUserRepository --|> IRepository~T~ : extends
    AuthController ..> IAuthService : use
    AuthController ..> LoginRequest : use
    AuthController ..> LoginResponse : use
    AuthService ..> IUserRepository : use
    AuthService ..> IJwtService : use
    AuthService ..> INotifier : use
    AuthService ..> AppException : throw
    GlobalExceptionMiddleware ..> AppException : catch
    IUserRepository ..> User : manages
    IJwtService ..> User : reads
```

### Figure 5 — Sequence Diagram (login & session initiation)

```mermaid
sequenceDiagram
    autonumber
    actor User as User
    participant LP as loginPage:LoginPage
    participant API as authApi:auth.service.ts
    participant ST as authStore:useAuthStore
    participant MW as :GlobalExceptionMiddleware
    participant C as :AuthController
    participant S as :AuthService
    participant J as :IJwtService
    participant R as :IUserRepository
    participant DB as db:PostgreSQL

    User->>LP: enter email & password, click "Login"
    LP->>API: authService.login({email, password})
    API->>MW: POST /api/auth/login (LoginRequest)
    MW->>C: forward request
    activate C
    C->>S: LoginAsync(request)
    activate S
    S->>R: Query().Include(UserRoles).FirstOrDefaultAsync(u => u.Email == request.Email)
    activate R
    R->>DB: SELECT ... FROM public.users u LEFT JOIN user_roles ... WHERE u.email = @email
    activate DB
    DB-->>R: user row (or empty)
    deactivate DB
    R-->>S: User?
    deactivate R

    alt user == null or BCrypt.Verify(password, hash) == false
        S-->>MW: throw AppException(INVALID_CREDENTIALS, 401)
        MW-->>API: HTTP 401 - "Invalid email or password."
        API-->>LP: show login error
    else user.Status != ACTIVE
        S-->>MW: throw AppException(ACCOUNT_INACTIVE, 401)
        MW-->>API: HTTP 401 - "Account is locked."
        API-->>LP: show account-locked error
    else login successful
        S->>R: LastLoginAt = UtcNow, SaveChangesAsync()
        activate R
        R->>DB: UPDATE public.users SET last_login_at = @now
        deactivate R
        S->>J: GenerateToken(user, roles)
        activate J
        J-->>S: JWT string
        deactivate J
        S-->>C: LoginResponse(AccessToken, TokenType, ExpiresIn, User)
        deactivate S
        C-->>MW: 200 ApiResponse(LoginResponse)
        deactivate C
        MW-->>API: HTTP 200
        API->>ST: setUser(user)
        ST-->>LP: authenticated state
        LP-->>User: redirect to Dashboard by role
    end
```

---

## 3.2 — Research Cycle & Field Management

`CycleService` nhận **6 dependency** thật: `ICycleRepository`, `IMasterDataRepository`,
`IProposalRepository`, `IReviewRepository`, `INotifier`, `IDeadlineResolver`.

### Figure 6 — Class Diagram

```mermaid
classDiagram
    direction LR

    class CyclesController {
        -ICycleService _cycles
        +GetCycles() Task~ActionResult~
        +CreateCycle(CreateCycleRequest) Task~ActionResult~
        +OpenCycle(int) Task~ActionResult~
        +CloseCycle(int) Task~ActionResult~
        +ExtendDeadline(int, ExtendDeadlineRequest) Task~ActionResult~
        +AttachTrack(int, int) Task~ActionResult~
    }

    class ICycleService {
        <<interface>>
        +GetCyclesAsync() Task~IEnumerable~
        +CreateCycleAsync(CreateCycleRequest, Guid) Task~CycleDto~
        +OpenCycleAsync(int) Task~CycleDto~
        +CloseCycleAsync(int) Task~CycleDto~
        +ExtendCycleDeadlineAsync(int, ExtendDeadlineRequest, Guid) Task~DeadlineExtensionDto~
        +AttachTrackToCycleAsync(int, int) Task
        +DetachTrackFromCycleAsync(int, int) Task
    }

    class CycleService {
        -ICycleRepository _cycles
        -IMasterDataRepository _masterData
        -IProposalRepository _proposals
        -IReviewRepository _review
        -INotifier _notifier
        -IDeadlineResolver _deadlines
        -ValidateCycleAsync(string, int, DateOnly, DateOnly, int, int?) Task
        +CreateCycleAsync(CreateCycleRequest, Guid) Task~CycleDto~
        +OpenCycleAsync(int) Task~CycleDto~
    }

    class ICycleRepository {
        <<interface>>
        +Tracks IQueryable~ResearchTrack~
        +CycleTracks IQueryable~CycleTrack~
        +Query() IQueryable~ResearchCycle~
        +AddAsync(ResearchCycle) Task
        +SaveChangesAsync(CancellationToken) Task
    }

    class IDeadlineResolver {
        <<interface>>
        +EffectiveAsync(string, string, DateOnly) Task~DateOnly~
    }

    class ResearchCycle {
        +int Id
        +int CycleYear
        +string SemesterCode
        +int ResearchTypeId
        +DateOnly SubmissionOpenDate
        +DateOnly SubmissionDeadline
        +DateOnly ReviewDeadline
        +string Status
        +Guid CreatedBy
    }

    class ResearchTrack {
        +int Id
        +string Name
        +Guid? OwnerId
        +bool IsActive
    }

    class CycleTrack {
        +int Id
        +int CycleId
        +int TrackId
    }

    class ResearchType {
        +int Id
        +string Name
        +decimal MaxBudgetCap
    }

    class DeadlineExtension {
        +Guid Id
        +string TargetType
        +string TargetId
        +DateOnly OldDeadline
        +DateOnly NewDeadline
        +string? Reason
        +Guid CreatedBy
    }

    CycleService ..|> ICycleService : realize
    CyclesController ..> ICycleService : use
    CycleService ..> ICycleRepository : use
    CycleService ..> IDeadlineResolver : use
    ICycleRepository ..> ResearchCycle : manages
    ResearchCycle "1" o-- "*" CycleTrack : opens
    ResearchTrack "1" o-- "*" CycleTrack : grouped by
    ResearchCycle ..> ResearchType : typed by
    IDeadlineResolver ..> DeadlineExtension : reads
```

### Figure 7 — Sequence Diagram (create & open a research cycle)

```mermaid
sequenceDiagram
    autonumber
    actor ADM as Admin / Staff
    participant UI as cyclesPage:CyclesPage
    participant API as cycleApi:cycle.service.ts
    participant MW as :GlobalExceptionMiddleware
    participant C as :CyclesController
    participant S as :CycleService
    participant MD as :IMasterDataRepository
    participant R as :ICycleRepository
    participant DB as db:PostgreSQL

    ADM->>UI: fill cycle form, click "Create"
    UI->>API: cycleService.create(payload)
    API->>MW: POST /api/cycles (CreateCycleRequest)
    MW->>C: forward request
    activate C
    C->>S: CreateCycleAsync(request, createdBy)
    activate S
    S->>MD: ResearchTypes.FirstOrDefaultAsync(t => t.Id == request.ResearchTypeId)
    MD-->>S: ResearchType?
    S->>S: ValidateCycleAsync(name, year, openDate, deadline, typeId)

    alt blank name, year out of range, or deadline not after opening date
        S-->>MW: throw ArgumentException
        MW-->>API: HTTP 400 - "The deadline must fall after the opening date."
        API-->>UI: show field error
    else research type not found
        S-->>MW: throw KeyNotFoundException
        MW-->>API: HTTP 404 - "Research type does not exist."
        API-->>UI: show error
    else cycle name already taken
        S->>R: Query().AnyAsync(c => c.SemesterCode == name)
        R-->>S: true
        S-->>MW: throw InvalidOperationException
        MW-->>API: HTTP 409 - "A cycle with this name already exists."
        API-->>UI: show duplicate-name error
    else valid
        S->>R: AddAsync(new ResearchCycle with Status = PLANNING)
        activate R
        R->>DB: INSERT INTO public.research_cycles ...
        deactivate R
        S->>R: SaveChangesAsync()
        S-->>C: CycleDto
        deactivate S
        C-->>MW: 200 ApiResponse(CycleDto)
        deactivate C
        MW-->>API: HTTP 200
        API-->>UI: cycle created with status PLANNING

        ADM->>UI: click "Open cycle"
        UI->>API: cycleService.open(cycleId)
        API->>C: POST /api/cycles/{id}/open
        activate C
        C->>S: OpenCycleAsync(cycleId)
        activate S
        S->>R: Query().FirstOrDefaultAsync(c => c.Id == cycleId)
        R-->>S: ResearchCycle?
        S->>R: Status = OPEN, SaveChangesAsync()
        R->>DB: UPDATE public.research_cycles SET status = 'OPEN'
        S-->>C: CycleDto
        deactivate S
        C-->>API: HTTP 200 - cycle is now OPEN
        deactivate C
    end
```

---

## 3.3 — Research Proposal Submission

`ProposalService` là service nặng nhất hệ thống — **13 dependency**. Class diagram liệt kê đủ 13 ở
ngăn thuộc tính, nhưng chỉ vẽ mũi tên tới 5 cái thật sự tham gia luồng nộp, để hình còn đọc được.

### Figure 8 — Class Diagram

```mermaid
classDiagram
    direction LR

    class ProposalsController {
        -IProposalService _proposals
        +Create(CreateProposalRequest) Task~ActionResult~
        +Update(Guid, CreateProposalRequest) Task~ActionResult~
        +Submit(Guid, SubmitProposalRequest) Task~ActionResult~
        +Withdraw(Guid) Task~ActionResult~
    }

    class IProposalService {
        <<interface>>
        +GetProposalsAsync(ProposalQueryParams, Guid, IEnumerable, bool) Task~IEnumerable~
        +GetProposalByIdAsync(Guid, Guid, IEnumerable) Task~ProposalDto~
        +CreateProposalAsync(CreateProposalRequest, Guid) Task~ProposalDto~
        +UpdateProposalAsync(Guid, CreateProposalRequest, Guid) Task~ProposalDto~
        +SubmitProposalAsync(Guid, Guid, bool) Task~ProposalDto~
        +WithdrawProposalAsync(Guid, Guid) Task~ProposalDto~
    }

    class ProposalService {
        -IProposalRepository _proposals
        -ICycleRepository _cycles
        -IMasterDataRepository _masterData
        -IUserRepository _users
        -IClock _clock
        -IReviewRoundService _reviewRounds
        -IReviewRepository _review
        -IBudgetPolicyService _budgetPolicy
        -INotifier _notifier
        -IAiSummaryQueue _summaryQueue
        -IDeadlineResolver _deadlines
        -IDecisionLogger _decisions
        -IEmbeddingQueue _embeddings
        +SubmitProposalAsync(Guid, Guid, bool) Task~ProposalDto~
    }

    class IBudgetPolicyService {
        <<interface>>
        +AssertWithinCapAsync(Guid, decimal) Task
    }

    class IDeadlineResolver {
        <<interface>>
        +EffectiveAsync(string, string, DateOnly) Task~DateOnly~
    }

    class IDecisionLogger {
        <<interface>>
        +Log(Guid, string, string, string, string) void
    }

    class IEmbeddingQueue {
        <<interface>>
        +Enqueue(Guid) void
    }

    class IProposalRepository {
        <<interface>>
        +Projects IQueryable~Project~
        +ProjectMembers IQueryable~ProjectMember~
        +Deliverables IQueryable~ProjectDeliverable~
        +Budgets IQueryable~ProposalBudget~
        +BudgetItems IQueryable~ProposalBudgetItem~
        +Activities IQueryable~ProposalActivity~
        +AddProject(Project) void
        +AddDeliverable(ProjectDeliverable) void
    }

    class Project {
        +Guid Id
        +Guid PiUserId
        +int CycleTrackId
        +int OrderId
        +string Status
        +bool IsDeleted
    }

    class Proposal {
        +Guid Id
        +Guid ProjectId
        +int VersionNo
        +bool IsCurrent
        +string Status
        +DateTime? SubmittedAt
        +int DurationMonths
    }

    class ProposalBudget {
        +int Id
        +Guid ProposalId
        +decimal TotalAmount
    }

    ProposalService ..|> IProposalService : realize
    ProposalsController ..> IProposalService : use
    ProposalService ..> IProposalRepository : use
    ProposalService ..> IBudgetPolicyService : use
    ProposalService ..> IDeadlineResolver : use
    ProposalService ..> IDecisionLogger : use
    ProposalService ..> IEmbeddingQueue : use
    Project "1" o-- "*" Proposal : versions
    Proposal "1" --> "1" ProposalBudget : budget
    IBudgetPolicyService ..> ProposalBudget : checks cap
```

### Figure 9 — Sequence Diagram (submit a proposal)

```mermaid
sequenceDiagram
    autonumber
    actor PI as Principal Investigator
    participant UI as proposalPage:ProposalDetailPage
    participant API as proposalApi:proposal.service.ts
    participant MW as :GlobalExceptionMiddleware
    participant C as :ProposalsController
    participant S as :ProposalService
    participant DL as :IDeadlineResolver
    participant BP as :IBudgetPolicyService
    participant LG as :IDecisionLogger
    participant R as :IProposalRepository
    participant DB as db:PostgreSQL

    PI->>UI: click "Submit proposal"
    UI->>API: proposalService.submit(id, confirmCv)
    API->>MW: POST /api/proposals/{id}/submit
    MW->>C: forward request
    activate C
    C->>S: SubmitProposalAsync(proposalId, userId, confirmCvUpToDate)
    activate S
    S->>R: Query().Include(Project.CycleTrack.Cycle).FirstOrDefaultAsync(...)
    activate R
    R->>DB: SELECT ... FROM public.proposals JOIN projects JOIN cycle_tracks JOIN research_cycles
    DB-->>R: proposal + project + cycle
    deactivate R
    R-->>S: Proposal?

    alt project.PiUserId != userId
        S-->>MW: throw ForbiddenException
        MW-->>API: HTTP 403 - "Only the PI can submit this proposal."
        API-->>UI: show permission error
    else proposal.Status != DRAFT
        S-->>MW: throw InvalidOperationException
        MW-->>API: HTTP 409 - "Only a draft proposal can be submitted."
        API-->>UI: show state error
    else past effective submission deadline
        S->>DL: EffectiveAsync(CYCLE, cycleId, cycle.SubmissionDeadline)
        DL-->>S: effective deadline including extensions
        S-->>MW: throw InvalidOperationException
        MW-->>API: HTTP 409 - "Submission deadline has passed."
        API-->>UI: show deadline error
    else budget exceeds cap (QD543 Article 14)
        S->>BP: AssertWithinCapAsync(proposalId, draftTotal)
        BP-->>S: cap exceeded
        S-->>MW: propagate exception
        MW-->>API: HTTP 409 - "Budget exceeds the cap for this research type."
        API-->>UI: show budget error
    else academic CV older than 6 months
        S-->>MW: throw InvalidOperationException
        MW-->>API: HTTP 409 - "Academic CV is out of date."
        API-->>UI: prompt to update CV or confirm
    else submission accepted
        S->>S: proposal.Status = SUBMITTED, project.Status = UNDER_REVIEW
        S->>LG: Log(projectId, PROPOSAL_SUBMITTED, summary, Proposal, proposalId)
        S->>R: SaveChangesAsync()
        activate R
        R->>DB: UPDATE public.proposals and public.projects, INSERT public.project_decisions
        deactivate R
        S-->>C: ProposalDto
        deactivate S
        C-->>MW: 200 ApiResponse(ProposalDto)
        deactivate C
        MW-->>API: HTTP 200
        API-->>UI: proposal submitted, editing locked
        UI-->>PI: show "Submitted" state
    end
```

---

## 3.4 — Council Setup & Reviewer Invitation

`CouncilService` nhận 7 dependency: `IReviewRepository`, `IProposalRepository`,
`ISystemSettingService`, `INotificationRepository`, `IEmailService`, `IDecisionLogger`,
`FURPMSDbContext`.

Luồng gửi thư mời có **6 cửa chốt** trước khi gửi — trong đó quy tắc "số thành viên phải LẺ"
là ràng buộc nghiệp vụ riêng của FURPMS (để phiếu Đạt/Không đạt luôn có chênh lệch).

### Figure 10 — Class Diagram

```mermaid
classDiagram
    direction LR

    class CouncilsController {
        -ICouncilService _councils
        +GetCouncils(CouncilQueryParams) Task~ActionResult~
        +CreateCouncil(CreateCouncilRequest) Task~ActionResult~
        +AddMember(Guid, AddCouncilMemberRequest) Task~ActionResult~
        +SendInvitations(Guid, DateTime?) Task~ActionResult~
        +AssignProject(Guid, Guid) Task~ActionResult~
        +RespondToMembership(Guid, bool, string) Task~ActionResult~
    }

    class ICouncilService {
        <<interface>>
        +GetCouncilsAsync(CouncilQueryParams) Task~IEnumerable~
        +CreateCouncilAsync(CreateCouncilRequest, Guid) Task~CouncilResponse~
        +AddMemberAsync(Guid, AddCouncilMemberRequest) Task~CouncilMemberResponse~
        +SendInvitationsAsync(Guid, DateTime?) Task~int~
        +AssignProjectToCouncilAsync(Guid, Guid, bool, string) Task
        +RespondToMembershipAsync(Guid, Guid, bool, string) Task~CouncilMemberResponse~
        +GetScheduleConflictsAsync(Guid) Task~IEnumerable~
    }

    class CouncilService {
        -IReviewRepository _review
        -IProposalRepository _proposals
        -ISystemSettingService _settings
        -INotificationRepository _notifications
        -IEmailService _email
        -IDecisionLogger _decisions
        -FURPMSDbContext _db
        -IsChair(string) bool
        -OddSizesFor(int, int) IEnumerable~int~
        +SendInvitationsAsync(Guid, DateTime?) Task~int~
    }

    class IReviewRepository {
        <<interface>>
        +Meetings IQueryable~CouncilMeeting~
        +CouncilMembers IQueryable~CouncilMember~
        +Decisions IQueryable~CouncilDecision~
        +Query() IQueryable~ReviewCouncil~
        +SaveChangesAsync(CancellationToken) Task
    }

    class IEmailService {
        <<interface>>
        +SendAsync(string, string, string, string, Guid) Task
    }

    class ISystemSettingService {
        <<interface>>
        +GetIntAsync(string, int) Task~int~
    }

    class INotificationRepository {
        <<interface>>
        +EmailLogs IQueryable~EmailLog~
        +AddAsync(Notification) Task
        +AddEmailLog(EmailLog) void
        +SaveChangesAsync(CancellationToken) Task
    }

    class ReviewCouncil {
        +Guid Id
        +Guid? RoundId
        +string? EstablishmentDecisionNo
        +int MinMembersRequired
        +int MaxMembersAllowed
        +string Status
    }

    class CouncilMember {
        +Guid Id
        +Guid CouncilId
        +Guid UserId
        +string MemberRole
        +string Status
        +DateTime? InvitationSentAt
        +string? InvitationToken
        +DateTime? TokenExpiresAt
        +DateTime? ConfirmedAt
        +DateTime? DeclinedAt
        +string? DeclineReason
    }

    class CouncilProjectAssignment {
        +int Id
        +Guid CouncilId
        +Guid ProjectId
    }

    class Notification {
        +Guid Id
        +Guid UserId
        +string NotificationType
        +string Title
        +string Body
        +string ActionUrl
        +string Priority
    }

    CouncilService ..|> ICouncilService : realize
    CouncilsController ..> ICouncilService : use
    CouncilService ..> IReviewRepository : use
    CouncilService ..> IEmailService : use
    CouncilService ..> ISystemSettingService : use
    CouncilService ..> INotificationRepository : use
    ReviewCouncil "1" o-- "*" CouncilMember : members
    ReviewCouncil "1" o-- "*" CouncilProjectAssignment : evaluates
    INotificationRepository ..> Notification : creates
```

### Figure 11 — Sequence Diagram (reviewer assignment & invitation)

```mermaid
sequenceDiagram
    autonumber
    actor Staff as Staff
    actor Rev as Reviewer
    participant UI as membersPanel:CouncilMembersPanel
    participant MW as :GlobalExceptionMiddleware
    participant C as :CouncilsController
    participant S as :CouncilService
    participant SET as :ISystemSettingService
    participant N as :INotificationRepository
    participant M as :IEmailService
    participant R as :IReviewRepository
    participant DB as db:PostgreSQL

    Staff->>UI: click "Send invitations"
    UI->>MW: POST /api/councils/{councilId}/send-invitations
    MW->>C: forward request
    activate C
    C->>S: SendInvitationsAsync(councilId, confirmDeadline)
    activate S
    S->>R: Query().Include(Members).Include(ProjectAssignments.Project).FirstOrDefaultAsync(...)
    activate R
    R->>DB: SELECT ... FROM public.review_councils JOIN council_members JOIN council_project_assignments
    DB-->>R: council graph
    deactivate R
    R-->>S: ReviewCouncil?

    alt no Chair or no Secretary assigned
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "Council has no Chair / Secretary."
        C-->>UI: show error
    else no meeting scheduled yet
        S->>R: Meetings.AnyAsync(m => m.CouncilId == councilId)
        R-->>S: false
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "Schedule the meeting before sending invitations."
        C-->>UI: show error
    else no project assigned to the council
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "Assign at least one project first."
        C-->>UI: show error
    else members below minimum or member count is even
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "Council size must reach the minimum and be an ODD number (QD543 Art. 8.2 / 12.2)."
        C-->>UI: show error
    else all gates passed
        S->>SET: GetIntAsync(COUNCIL_INVITE_DEADLINE_DAYS, default)
        SET-->>S: inviteDays
        S->>S: pending = members not Declined and not yet invited
        S->>S: set InvitationToken, TokenExpiresAt, Status = INVITED
        S->>R: SaveChangesAsync()
        activate R
        R->>DB: UPDATE public.council_members SET status, invitation_token, token_expires_at
        deactivate R

        loop for each invited member
            S->>N: AddAsync(Notification COUNCIL_INVITATION)
            N->>DB: INSERT INTO public.notifications ...
            S->>M: SendAsync(email, title, body, COUNCIL_INVITATION, userId)
            M->>DB: INSERT INTO public.email_logs ...
        end

        S-->>C: number of invitations sent
        deactivate S
        C-->>MW: 200 ApiResponse(int)
        deactivate C
        MW-->>UI: HTTP 200 - invitations sent
        M-->>Rev: invitation email with confirm deadline
    end
```

### Figure 11b — Sequence Diagram (reviewer responds to the invitation)

> **Vì sao tách khỏi Figure 11:** đây là **use case riêng** — actor khác (Reviewer, không phải Staff),
> endpoint khác (`POST /api/councils/members/{memberId}/respond`), và xảy ra **nhiều ngày sau**.
> Cẩm nang có gạch *"Gom nhiều use case vào 1 sequence → không phù hợp"*.

```mermaid
sequenceDiagram
    autonumber
    actor Rev as Reviewer
    participant UI as invitationsPage:InvitationsPage
    participant MW as :GlobalExceptionMiddleware
    participant C as :CouncilsController
    participant S as :CouncilService
    participant R as :IReviewRepository
    participant DB as db:PostgreSQL

    Rev->>UI: open /invitations, click "Accept" or "Decline"
    UI->>MW: PATCH /api/council-members/{memberId}/respond
    MW->>C: forward request
    activate C
    C->>S: RespondToMembershipAsync(memberId, userId, accept, declineReason)
    activate S
    S->>R: CouncilMembers.Include(User).FirstOrDefaultAsync(m => m.Id == memberId)
    activate R
    R->>DB: SELECT ... FROM public.council_members JOIN users WHERE id = @memberId
    DB-->>R: member row (or empty)
    deactivate R
    R-->>S: CouncilMember?

    alt member not found
        S-->>MW: throw KeyNotFoundException
        MW-->>C: HTTP 404 - "Council member not found."
        C-->>UI: show error
    else member.UserId != userId
        S-->>MW: throw ForbiddenException
        MW-->>C: HTTP 403 - "You may only answer an invitation addressed to you."
        C-->>UI: show permission error
    else member.Status == DECLINED
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "You already declined this invitation."
        C-->>UI: show state error
    else accept and TokenExpiresAt has passed
        S->>R: Status = EXPIRED, SaveChangesAsync()
        R->>DB: UPDATE public.council_members SET status = 'EXPIRED'
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "The invitation has expired."
        C-->>UI: show expiry notice, Staff must find a replacement
    else accept within the deadline
        S->>S: Status = CONFIRMED, ConfirmedAt = UtcNow
        S->>R: SaveChangesAsync()
        activate R
        R->>DB: UPDATE public.council_members SET status = 'CONFIRMED', confirmed_at = @now
        deactivate R
        S-->>C: CouncilMemberResponse
        deactivate S
        C-->>MW: 200 ApiResponse(CouncilMemberResponse)
        deactivate C
        MW-->>UI: HTTP 200 - membership confirmed
        UI-->>Rev: show "Confirmed" state
    else decline
        S->>S: Status = DECLINED, DeclinedAt = UtcNow, DeclineReason = reason
        S->>R: SaveChangesAsync()
        R->>DB: UPDATE public.council_members SET status = 'DECLINED', decline_reason = @reason
        S-->>C: CouncilMemberResponse
        C-->>UI: HTTP 200 - invitation declined
        UI-->>Rev: show "Declined" state
    end
```

---

## 3.5 — Review Meeting Scheduling

`CouncilMeetingService` chỉ nhận **một** dependency: `IReviewRepository`. Đây là service gọn nhất
trong 7 khu vực — đừng vẽ thêm gì cho "cân hình".

### Figure 12 — Class Diagram

```mermaid
classDiagram
    direction LR

    class CouncilMeetingsController {
        -ICouncilMeetingService _meetings
        +GetAll() Task~ActionResult~
        +GetForPi() Task~ActionResult~
        +GetForCouncilMember() Task~ActionResult~
        +Schedule(Guid, ScheduleMeetingRequest) Task~ActionResult~
        +Update(Guid, UpdateMeetingRequest) Task~ActionResult~
        +Start(Guid) Task~ActionResult~
        +End(Guid) Task~ActionResult~
        +SaveAttendance(Guid, SaveAttendanceRequest) Task~ActionResult~
    }

    class ICouncilMeetingService {
        <<interface>>
        +GetAllAsync() Task~IEnumerable~
        +GetForPiAsync(Guid) Task~IEnumerable~
        +GetForCouncilMemberAsync(Guid) Task~IEnumerable~
        +ScheduleAsync(Guid, ScheduleMeetingRequest) Task~MeetingDto~
        +UpdateAsync(Guid, UpdateMeetingRequest) Task~MeetingDto~
        +StartAsync(Guid) Task~MeetingDto~
        +UndoStartAsync(Guid) Task~MeetingDto~
        +EndAsync(Guid) Task~MeetingDto~
        +GetAttendanceAsync(Guid) Task~IEnumerable~
        +SaveAttendanceAsync(Guid, Guid, bool, SaveAttendanceRequest) Task
    }

    class CouncilMeetingService {
        -IReviewRepository _review
        -ValidateAndNormalize(ScheduleMeetingRequest, bool) string
        -AssertNoScheduleConflictAsync(Guid, DateTime, int, Guid?) Task
        -Map(CouncilMeeting) MeetingDto
        +ScheduleAsync(Guid, ScheduleMeetingRequest) Task~MeetingDto~
    }

    class IReviewRepository {
        <<interface>>
        +Meetings IQueryable~CouncilMeeting~
        +CouncilMembers IQueryable~CouncilMember~
        +Query() IQueryable~ReviewCouncil~
        +AddMeetingAsync(CouncilMeeting) Task
        +SaveChangesAsync(CancellationToken) Task
    }

    class ReviewCouncil {
        +Guid Id
        +Guid? RoundId
        +string? EstablishmentDecisionNo
        +string Status
    }

    class CouncilMeeting {
        +Guid Id
        +Guid CouncilId
        +string Title
        +string Platform
        +string? MeetingLink
        +string? Location
        +DateTime ScheduledAt
        +int DurationMinutes
        +string Agenda
        +string Status
        +DateTime? ActualEndAt
    }

    class MeetingAttendance {
        +int Id
        +Guid MeetingId
        +Guid MemberId
        +string RsvpStatus
        +bool? ActuallyAttended
        +string? AbsenceReason
    }

    class MeetingPlatform {
        <<enumeration>>
        ONLINE
        IN_PERSON
    }

    class MeetingStatus {
        <<enumeration>>
        SCHEDULED
        IN_PROGRESS
        COMPLETED
    }

    CouncilMeetingService ..|> ICouncilMeetingService : realize
    CouncilMeetingsController ..> ICouncilMeetingService : use
    CouncilMeetingService ..> IReviewRepository : use
    IReviewRepository ..> CouncilMeeting : manages
    ReviewCouncil "1" o-- "*" CouncilMeeting : meetings
    CouncilMeeting "1" o-- "*" MeetingAttendance : attendance
    CouncilMeeting ..> MeetingPlatform : uses
    CouncilMeeting ..> MeetingStatus : uses
```

### Figure 13 — Sequence Diagram (schedule a review meeting)

```mermaid
sequenceDiagram
    autonumber
    actor Staff as Staff
    participant UI as scheduleSheet:ScheduleMeetingSheet
    participant MW as :GlobalExceptionMiddleware
    participant C as :CouncilMeetingsController
    participant S as :CouncilMeetingService
    participant R as :IReviewRepository
    participant DB as db:PostgreSQL

    Staff->>UI: pick date, time, duration, location or link
    UI->>MW: POST /api/councils/{councilId}/meetings (ScheduleMeetingRequest)
    MW->>C: forward request
    activate C
    C->>S: ScheduleAsync(councilId, request)
    activate S
    S->>R: Query().FirstOrDefaultAsync(c => c.Id == councilId)
    activate R
    R->>DB: SELECT ... FROM public.review_councils WHERE id = @councilId
    DB-->>R: council row (or empty)
    deactivate R
    R-->>S: ReviewCouncil?

    alt council not found
        S-->>MW: throw KeyNotFoundException
        MW-->>C: HTTP 404 - "Council not found."
        C-->>UI: show error
    else ScheduledAt is in the past
        S->>S: ValidateAndNormalize(request, requireFuture = true)
        S-->>MW: throw ArgumentException
        MW-->>C: HTTP 400 - "Meeting must be scheduled in the future."
        C-->>UI: show field error
    else DurationMinutes <= 0
        S-->>MW: throw ArgumentException
        MW-->>C: HTTP 400 - "Duration must be greater than 0 minutes."
        C-->>UI: show field error
    else a council member has an overlapping meeting
        S->>R: AssertNoScheduleConflictAsync(councilId, scheduledAt, duration, null)
        R->>DB: SELECT overlapping meetings for shared members
        R-->>S: conflict found
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "A member already has a meeting at that time."
        C-->>UI: show conflict warning
    else schedule accepted
        S->>S: platform = ONLINE or IN_PERSON, clear the unused field
        S->>R: AddMeetingAsync(new CouncilMeeting with Status = SCHEDULED)
        activate R
        R->>DB: INSERT INTO public.council_meetings ...
        deactivate R
        S->>R: SaveChangesAsync()
        S-->>C: MeetingDto
        deactivate S
        C-->>MW: 200 ApiResponse(MeetingDto)
        deactivate C
        MW-->>UI: HTTP 200
        UI-->>Staff: meeting scheduled, invitations can now be sent
    end
```

---

## 3.6 — Proposal Evaluation & Scoring

Ba vai khác nhau trong một luồng: **Uỷ viên chấm → Thư ký soạn biên bản → Chủ tịch duyệt (khoá)**.
Rule #12 của dự án: **hệ thống không tự chốt kết quả** — nó chỉ cung cấp căn cứ, `Result` luôn
chép lại kết luận của Chủ tịch.

`ReviewScoringService` nhận 7 dependency: `IReviewRepository`, `IMasterDataRepository`,
`IProposalRepository`, `IClock`, `ISystemSettingService`, `INotifier`, `IDecisionLogger`.

### Figure 14 — Class Diagram

```mermaid
classDiagram
    direction LR

    class ReviewScoringController {
        -IReviewScoringService _scoring
        +SubmitScore(Guid, SubmitScoreRequest) Task~ActionResult~
        +GetCouncilScores(Guid, Guid?) Task~ActionResult~
        +SaveMinutes(Guid, SaveMinutesRequest) Task~ActionResult~
        +ApproveMinutes(Guid, Guid?) Task~ActionResult~
        +RequestMinutesRevision(Guid, string, Guid?) Task~ActionResult~
        +GetBallotTally(Guid, Guid?) Task~ActionResult~
    }

    class IReviewScoringService {
        <<interface>>
        +SubmitScoreAsync(Guid, Guid, SubmitScoreRequest) Task~ReviewScoreDto~
        +GetCouncilScoresAsync(Guid, Guid?) Task~IEnumerable~
        +SaveMinutesAsync(Guid, Guid, SaveMinutesRequest) Task~CouncilDecisionDto~
        +ApproveMinutesAsync(Guid, Guid, Guid?) Task~CouncilDecisionDto~
        +RequestMinutesRevisionAsync(Guid, Guid, string, Guid?) Task~CouncilDecisionDto~
        +FinalizeDecisionAsync(Guid, FinalizeDecisionRequest) Task~CouncilDecisionDto~
        +GetBallotTallyAsync(Guid, Guid?) Task~BallotTallyDto~
    }

    class ReviewScoringService {
        -IReviewRepository _review
        -IMasterDataRepository _masterData
        -IProposalRepository _proposals
        -IClock _clock
        -ISystemSettingService _settings
        -INotifier _notifier
        -IDecisionLogger _decisions
        -IsChair(string) bool
        -AssertConfirmed(CouncilMember) void
        -AssertQuorumAsync(ReviewCouncil, Guid, string) Task
        -MarkDecidedIfAllProjectsFinalizedAsync(ReviewCouncil, Guid) Task
        +ApproveMinutesAsync(Guid, Guid, Guid?) Task~CouncilDecisionDto~
    }

    class IReviewRepository {
        <<interface>>
        +Meetings IQueryable~CouncilMeeting~
        +CouncilMembers IQueryable~CouncilMember~
        +Decisions IQueryable~CouncilDecision~
        +Query() IQueryable~ReviewCouncil~
        +SaveChangesAsync(CancellationToken) Task
    }

    class ISystemSettingService {
        <<interface>>
        +GetIntAsync(string, int) Task~int~
        +GetBoolAsync(string, bool) Task~bool~
    }

    class IDecisionLogger {
        <<interface>>
        +Log(Guid, string, string, string, string) void
    }

    class INotifier {
        <<interface>>
        +NotifyAsync(Guid, string, string, string, string) Task
    }

    class ReviewCouncil {
        +Guid Id
        +Guid? RoundId
        +string? EstablishmentDecisionNo
        +int MinMembersRequired
        +string Status
    }

    class ProposalReviewScore {
        +int Id
        +Guid CouncilId
        +Guid ProjectId
        +Guid EvaluatorMemberId
        +int TemplateId
        +string? GeneralComments
        +string? OtherRecommendations
        +DateTime? SubmittedAt
        +bool IsValidBallot
    }

    class ReviewScoreDetail {
        +int Id
        +int ScoreId
        +int CriterionId
        +decimal GivenScore
        +string? Comments
    }

    class CouncilDecision {
        +int Id
        +Guid CouncilId
        +Guid ProjectId
        +int TotalMembers
        +int AttendingMembers
        +int ValidBallots
        +decimal? AverageScore
        +string Result
        +string? ResultJustification
        +Guid? ChairUserId
        +DateTime? FinalizedAt
    }

    class RubricTemplate {
        +int Id
        +string TemplateType
        +string Name
        +decimal MaxTotalScore
        +int? TrackId
        +bool IsActive
    }

    class RubricCriterion {
        +int Id
        +int TemplateId
        +string CriterionName
        +decimal MaxScore
        +int Sequence
        +bool IsActive
    }

    class ReviewRound {
        +Guid Id
        +int CycleTrackId
        +string Dimension
        +string Status
        +DateOnly? ScoringDeadline
    }

    ReviewScoringService ..|> IReviewScoringService : realize
    ReviewScoringController ..> IReviewScoringService : use
    ReviewScoringService ..> IReviewRepository : use
    ReviewScoringService ..> ISystemSettingService : use
    ReviewScoringService ..> IDecisionLogger : use
    ReviewScoringService ..> INotifier : use
    ReviewCouncil "1" o-- "*" ProposalReviewScore : ballots
    ProposalReviewScore "1" *-- "*" ReviewScoreDetail : per criterion
    ReviewCouncil "1" o-- "*" CouncilDecision : one per project
    RubricTemplate "1" *-- "*" RubricCriterion : criteria
    ReviewRound "1" o-- "*" ReviewCouncil : councils
```

### Figure 15 — Sequence Diagram (council member submits a ballot)

> Tách khỏi phần biên bản: bản gộp cũ dài **46 message**, lệch hẳn dải 25–35 của các hình khác.

```mermaid
sequenceDiagram
    autonumber
    actor Rev as Council Member
    participant MW as :GlobalExceptionMiddleware
    participant C as :ReviewScoringController
    participant S as :ReviewScoringService
    participant MD as :IMasterDataRepository
    participant SET as :ISystemSettingService
    participant R as :IReviewRepository
    participant DB as db:PostgreSQL

    Rev->>MW: POST /api/review-scoring/councils/{councilId}/scores (SubmitScoreRequest)
    MW->>C: forward request
    activate C
    C->>S: SubmitScoreAsync(councilId, userId, request)
    activate S
    S->>R: Query().FirstOrDefaultAsync(c => c.Id == councilId)
    activate R
    R->>DB: SELECT ... FROM public.review_councils WHERE id = @councilId
    DB-->>R: council row (or empty)
    deactivate R
    R-->>S: ReviewCouncil?
    S->>S: ResolveProjectIdAsync(councilId, request.ProjectId)
    S->>R: Decisions.AnyAsync(d => d.ProjectId == projectId and d.FinalizedAt != null)
    R-->>S: minutes already locked?
    S->>R: CouncilMembers.FirstOrDefaultAsync(councilId, userId)
    R-->>S: CouncilMember?
    S->>MD: RubricTemplates.Include(Criteria).FirstOrDefaultAsync(request.TemplateId)
    MD-->>S: RubricTemplate?

    alt council not found
        S-->>MW: throw KeyNotFoundException
        MW-->>Rev: HTTP 404 - "Council not found."
    else minutes already locked by the Chair
        S-->>MW: throw InvalidOperationException
        MW-->>Rev: HTTP 409 - "Minutes are locked - scores can no longer be changed."
    else caller is not a confirmed council member
        S-->>MW: throw ForbiddenException
        MW-->>Rev: HTTP 403 - "You are not a member of this council."
    else rubric criteria do not add up to MaxTotalScore (QD543 BM03)
        S-->>MW: throw InvalidOperationException
        MW-->>Rev: HTTP 409 - "Rubric adds up to 95 of 100 - ask the research office to fix it."
    else some criteria have no score
        S-->>MW: throw ArgumentException
        MW-->>Rev: HTTP 400 - "Missing scores for some criteria."
    else ballot accepted
        S->>SET: GetIntAsync(SCORE_DECIMAL_PLACES, default)
        SET-->>S: allowed decimal places
        S->>S: validate every score against MaxScore and the decimal step
        S->>R: save ProposalReviewScore and ReviewScoreDetail per criterion
        activate R
        R->>DB: INSERT INTO public.proposal_review_scores, public.review_score_details
        deactivate R
        S->>R: SaveChangesAsync()
        S-->>C: ReviewScoreDto
        deactivate S
        C-->>MW: 200 ApiResponse(ReviewScoreDto)
        deactivate C
        MW-->>Rev: HTTP 200 - ballot recorded
    end
    Note over S,DB: the system never tallies a final result on its own (project rule 12)
```

### Figure 15b — Sequence Diagram (secretary drafts the minutes, chair approves)

```mermaid
sequenceDiagram
    autonumber
    actor Sec as Secretary
    actor Chair as Chair
    participant MW as :GlobalExceptionMiddleware
    participant C as :ReviewScoringController
    participant S as :ReviewScoringService
    participant LG as :IDecisionLogger
    participant N as :INotifier
    participant R as :IReviewRepository
    participant DB as db:PostgreSQL

    Sec->>MW: POST /api/review-scoring/councils/{councilId}/minutes (SaveMinutesRequest)
    MW->>C: forward request
    activate C
    C->>S: SaveMinutesAsync(councilId, secretaryUserId, request)
    activate S
    S->>R: upsert CouncilDecision draft (FinalizedAt = null)
    R->>DB: INSERT or UPDATE public.council_decisions
    S-->>C: CouncilDecisionDto (draft, not locked)
    deactivate S
    C-->>Sec: HTTP 200 - draft saved
    deactivate C

    Chair->>MW: POST /api/review-scoring/councils/{councilId}/minutes/approve
    MW->>C: forward request
    activate C
    C->>S: ApproveMinutesAsync(councilId, chairUserId, projectId)
    activate S
    S->>R: Query().Include(Members).FirstOrDefaultAsync(c => c.Id == councilId)
    R-->>S: ReviewCouncil with members

    alt council.Status == DECIDED
        S-->>MW: throw InvalidOperationException
        MW-->>Chair: HTTP 409 - "Minutes are already locked."
    else caller is not a confirmed Chair
        S-->>MW: throw ForbiddenException
        MW-->>Chair: HTTP 403 - "Only the council Chair may approve the minutes."
    else no draft minutes, or more than one draft
        S-->>MW: throw InvalidOperationException
        MW-->>Chair: HTTP 409 - "No draft to approve, or several drafts - approve per project."
    else quorum not met (QD543 Art. 8.3.b)
        S->>R: AssertQuorumAsync(council, projectId, "lock minutes")
        R-->>S: attendance below quorum
        S-->>MW: throw InvalidOperationException
        MW-->>Chair: HTTP 409 - "Quorum not met for this project."
    else approval accepted
        S->>S: decision.ChairUserId = chairUserId, FinalizedAt = clock.UtcNow
        S->>R: close every meeting still open (Status = COMPLETED)
        R->>DB: UPDATE public.council_meetings SET status = 'COMPLETED', actual_end_at = @now
        S->>S: MarkDecidedIfAllProjectsFinalizedAsync(council, projectId)
        S->>R: update proposal.Status and project.Status from decision.Result
        Note over S,R: ACCEPTANCE round - PASSED becomes COMPLETED, FAILED returns to IN_PROGRESS
        R->>DB: UPDATE public.proposals, public.projects, public.review_councils
        S->>LG: Log(projectId, COUNCIL_DECISION_FINALIZED, result, CouncilDecision, decisionId)
        S->>N: NotifyAsync(piUserId, COUNCIL_DECISION, title, body, actionUrl)
        S->>R: SaveChangesAsync()
        S-->>C: CouncilDecisionDto (locked)
        deactivate S
        C-->>MW: 200 ApiResponse(CouncilDecisionDto)
        deactivate C
        MW-->>Chair: HTTP 200 - minutes locked, project status updated
    end
```

---

## 3.7 — Research Contract & Report Submission

Hai service trong cùng một mục: `ContractService` (7 dependency) và `ProgressReportService`
(6 dependency).

Cửa chốt đáng chú ý khi **ký hợp đồng**: hệ thống bắt buộc phải có **bản hợp đồng đã ký tải lên**
trước khi cho bấm ký — theo QĐ543 (BM05 Điều 7.2), hợp đồng ký điện tử ở phần mềm ngoài, hệ thống
chỉ lưu bản đã ký làm minh chứng.

### Figure 16 — Class Diagram

```mermaid
classDiagram
    direction LR

    class ContractsController {
        -IContractService _contracts
        +GetList(Guid?) Task~ActionResult~
        +Create(CreateContractRequest) Task~ActionResult~
        +Sign(Guid, DateOnly?) Task~ActionResult~
        +Terminate(Guid, string) Task~ActionResult~
    }

    class ProgressReportsController {
        -IProgressReportService _reports
        +GetByContract(Guid) Task~ActionResult~
        +Submit(CreateProgressReportRequest) Task~ActionResult~
        +Evaluate(Guid, EvaluateProgressReportRequest) Task~ActionResult~
    }

    class IContractService {
        <<interface>>
        +GetListAsync(Guid?) Task~IEnumerable~
        +GetByIdAsync(Guid) Task~ContractDetailResponse~
        +CreateAsync(CreateContractRequest, Guid) Task~ContractDetailResponse~
        +UpdateAsync(Guid, UpdateContractRequest) Task~ContractDetailResponse~
        +SignAsync(Guid, Guid, DateOnly?) Task~ContractDetailResponse~
        +TerminateAsync(Guid, Guid, string) Task~ContractDetailResponse~
    }

    class ContractService {
        -IContractRepository _contracts
        -IProposalRepository _proposals
        -IClock _clock
        -ISystemSettingService _settings
        -INotifier _notifier
        -IDocumentRepository _documents
        -IDecisionLogger _decisions
        -QueryWithProject() IQueryable~Contract~
        +SignAsync(Guid, Guid, DateOnly?) Task~ContractDetailResponse~
    }

    class IProgressReportService {
        <<interface>>
        +GetByContractAsync(Guid) Task~IEnumerable~
        +SubmitAsync(CreateProgressReportRequest, Guid) Task~ProgressReportDto~
        +EvaluateAsync(Guid, EvaluateProgressReportRequest, Guid) Task~ProgressReportDto~
    }

    class ProgressReportService {
        -IContractRepository _contracts
        -IProposalRepository _proposals
        -IClock _clock
        -IDocumentRepository _documents
        -INotifier _notifier
        -IDecisionLogger _decisions
    }

    class IDocumentRepository {
        <<interface>>
        +Query() IQueryable~Document~
        +AddAsync(Document) Task
    }

    class IContractRepository {
        <<interface>>
        +Disbursements IQueryable~ContractDisbursement~
        +ProgressReports IQueryable~ProgressReport~
        +Query() IQueryable~Contract~
        +AddAsync(Contract) Task
        +SaveChangesAsync(CancellationToken) Task
    }

    class IDecisionLogger {
        <<interface>>
        +Log(Guid, string, string, string, string) void
    }

    class INotifier {
        <<interface>>
        +NotifyAsync(Guid, string, string, string, string) Task
    }

    class Project {
        +Guid Id
        +Guid PiUserId
        +string Status
    }

    class Contract {
        +Guid Id
        +Guid ProjectId
        +string ContractNumber
        +string ScopeTitle
        +decimal TotalAmount
        +DateOnly StartDate
        +DateOnly EndDate
        +DateTime? SignedAt
        +string Status
    }

    class ContractDisbursement {
        +int Id
        +Guid ContractId
        +int RoundNumber
        +decimal Percentage
        +decimal PlannedAmount
        +decimal? ActualAmount
        +DateTime? ConditionMetAt
        +string Status
    }

    class ProgressReport {
        +Guid Id
        +Guid ContractId
        +DateOnly DueDate
        +DateTime? SubmittedAt
        +string Status
        +string? EvaluationResult
    }

    class Document {
        +Guid Id
        +string EntityType
        +string EntityId
        +string DocumentCategory
        +string OriginalFileName
        +string StorageUrl
        +bool IsDeleted
    }

    ContractService ..|> IContractService : realize
    ProgressReportService ..|> IProgressReportService : realize
    ContractsController ..> IContractService : use
    ProgressReportsController ..> IProgressReportService : use
    ContractService ..> IDocumentRepository : use
    ContractService ..> IDecisionLogger : use
    ContractService ..> INotifier : use
    ProgressReportService ..> IContractRepository : use
    Project "1" o-- "*" Contract : one per phase
    Contract "1" o-- "*" ContractDisbursement : tranches
    Contract "1" o-- "*" ProgressReport : periodic reports
    IDocumentRepository ..> Document : manages
    Contract ..> Document : signed copy evidence
```

### Figure 17 — Sequence Diagram (create and sign a contract)

> Tách khỏi phần báo cáo tiến độ: đổi actor (Staff → PI) **và** cách nhau nhiều tháng.

```mermaid
sequenceDiagram
    autonumber
    actor Staff as Staff
    participant MW as :GlobalExceptionMiddleware
    participant C as :ContractsController
    participant S as :ContractService
    participant D as :IDocumentRepository
    participant LG as :IDecisionLogger
    participant N as :INotifier
    participant R as :IContractRepository
    participant DB as db:PostgreSQL

    Staff->>MW: POST /api/contracts (CreateContractRequest with proposalId)
    MW->>C: forward request
    activate C
    C->>S: CreateAsync(request, createdBy)
    activate S
    S->>R: resolve proposal to project, require proposal.Status == APPROVED
    S->>R: AddAsync(new Contract with Status = PENDING_SIGNATURE)
    R->>DB: INSERT INTO public.contracts ...
    S-->>C: ContractDetailResponse
    deactivate S
    C-->>Staff: HTTP 200 - contract created, awaiting signature
    deactivate C

    Note over Staff,DB: export to Word, sign outside the system, upload the signed copy (QD543 BM05 Art. 7.2)

    Staff->>MW: POST /api/contracts/{id}/sign
    MW->>C: forward request
    activate C
    C->>S: SignAsync(contractId, signedBy, signedOn)
    activate S
    S->>R: QueryWithProject().FirstOrDefaultAsync(c => c.Id == contractId)
    activate R
    R->>DB: SELECT ... FROM public.contracts JOIN projects WHERE id = @contractId
    DB-->>R: contract + project
    deactivate R
    R-->>S: Contract?

    alt contract.Status != PENDING_SIGNATURE
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "Only a contract awaiting signature can be signed."
        C-->>Staff: show state error
    else no signed copy uploaded
        S->>D: Query().AnyAsync(d => d.EntityType == "Contract" and d.EntityId == contractId)
        D-->>S: false
        S-->>MW: throw InvalidOperationException
        MW-->>C: HTTP 409 - "Upload the signed contract copy first (QD543 BM05 Art. 7.2)."
        C-->>Staff: show evidence-required error
    else sign date is in the future
        S-->>MW: throw ArgumentException
        MW-->>C: HTTP 400 - "Sign date cannot be in the future."
        C-->>Staff: show field error
    else signature accepted
        S->>S: contract.Status = ACTIVE, SignedAt = signDate
        S->>S: contract.Project.Status = IN_PROGRESS
        S->>LG: Log(projectId, CONTRACT_SIGNED, summary, Contract, contractId)
        S->>R: SaveChangesAsync()
        activate R
        R->>DB: UPDATE public.contracts and public.projects, INSERT public.project_decisions
        deactivate R
        S->>N: NotifyAsync(piUserId, CONTRACT_SIGNED, title, body, actionUrl)
        S-->>C: ContractDetailResponse
        deactivate S
        C-->>MW: 200 ApiResponse(ContractDetailResponse)
        deactivate C
        MW-->>Staff: HTTP 200 - contract ACTIVE, project IN_PROGRESS
    end
```

### Figure 17b — Sequence Diagram (PI submits a progress report)

```mermaid
sequenceDiagram
    autonumber
    actor PI as Principal Investigator
    participant UI as reportsPage:ProgressReportsPage
    participant MW as :GlobalExceptionMiddleware
    participant C as :ProgressReportsController
    participant S as :ProgressReportService
    participant N as :INotifier
    participant R as :IContractRepository
    participant DB as db:PostgreSQL
    actor Staff as Staff

    Note over PI,DB: Precondition - Staff already ran POST /api/progress-reports/generate,<br/>so the DRAFT rounds exist (Applied 2 rounds, Basic 1 by default)

    PI->>UI: open the draft round, fill content, attach evidence
    UI->>MW: PUT /api/progress-reports/{id}
    MW->>C: forward request
    activate C
    C->>S: UpdateAsync(reportId, request, userId)
    S->>R: update report items while status is still DRAFT
    R->>DB: UPDATE public.progress_reports, public.progress_report_items
    C-->>UI: HTTP 200 - draft saved
    deactivate C

    PI->>UI: click "Submit"
    UI->>MW: POST /api/progress-reports/{id}/submit
    MW->>C: forward request
    activate C
    C->>S: SubmitAsync(reportId, userId)
    activate S
    S->>R: load report with its contract and project
    activate R
    R->>DB: SELECT ... FROM public.progress_reports JOIN contracts JOIN projects
    DB-->>R: report + contract + project
    deactivate R
    R-->>S: ProgressReport?

    alt report not found
        S-->>MW: throw KeyNotFoundException
        MW-->>UI: HTTP 404 - "Progress report not found."
    else caller is not the PI of this project
        S-->>MW: throw ForbiddenException
        MW-->>UI: HTTP 403 - "Only the PI may submit this report."
    else report is no longer a draft
        S-->>MW: throw InvalidOperationException
        MW-->>UI: HTTP 409 - "This report has already been submitted."
    else submission accepted
        S->>S: Status = SUBMITTED, SubmittedAt = clock.UtcNow
        S->>R: SaveChangesAsync()
        activate R
        R->>DB: UPDATE public.progress_reports SET status = 'SUBMITTED'
        deactivate R
        S->>N: NotifyAsync(staff, PROGRESS_REPORT_SUBMITTED, title, body, actionUrl)
        S-->>C: ProgressReportDto
        deactivate S
        C-->>MW: 200 ApiResponse(ProgressReportDto)
        deactivate C
        MW-->>UI: HTTP 200 - report submitted
        UI-->>PI: show "Submitted" state
        N-->>Staff: notification - a progress report is waiting for review
    end
```
