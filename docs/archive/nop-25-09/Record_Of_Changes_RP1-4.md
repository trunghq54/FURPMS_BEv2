# Record of Changes RP1–RP4 + đoạn giải thích RUP — bản dán được (25/09/2026)

> Nguyên tắc: **chỉ ghi những gì đã thật sự sửa** trong bản nhóm gửi. Sót việc gì cũng không làm
> nhật ký nói sai. Sửa thêm được việc nào thì thêm đúng một dòng 25/09 cho việc đó.
> Mỗi bảng thay cho TOÀN BỘ mục **I. Record of Changes** của báo cáo tương ứng.

## RP1 — Project Introduction

| Date | A*/M, D | In charge | Change Description |
|---|---|---|---|
| 16/05/2026 | A | Hoàng Quốc Trung | Initial draft – Project Introduction. |
| 21/09/2026 | M | All members | Removed references to conferencing and calendar integrations that are not part of the product, and restated the meeting scope in LI-06. |
| 21/09/2026 | A | All members | Added the features introduced after the project review: project budget overview, lifecycle deadlines, decision dossier, duplicate detection, score-divergence justification and reviewer expertise matching. |
| 21/09/2026 | M | All members | Rewrote LI-07 to describe the scope and limits of duplicate detection. |
| 21/09/2026 | M | All members | Updated the table of contents to include the feature subsections 6.1.1–6.1.5. |

## RP2 — Project Management Plan

| Date | A*/M, D | In charge | Change Description |
|---|---|---|---|
| 22/05/2026 | A | Hoàng Quốc Trung | Initial version created. |
| 22/09/2026 | M | All members | Updated the tools and infrastructure table to the current stack: PostgreSQL, the current Gemini model identifiers, Railway and Vercel for deployment, and Cloudinary and Brevo for file storage and email. |
| 22/09/2026 | D | All members | Removed the conferencing API and calendar synchronisation items from the work breakdown structure. |
| 22/09/2026 | M | All members | Updated the risk register to record the three risks that materialised and the mitigations applied. |
| 22/09/2026 | M | All members | Removed an outdated editorial note from Section 2.1. |

⚠️ Dựng từ §II của RP7. Mở RP2 gốc `Ctrl+F` tìm `PostgreSQL` và `Triggered` — không ra thì xoá dòng 2 và 4.

Nếu dán đoạn RUP (dưới) vào RP2 thì thêm:

| 25/09/2026 | A | All members | Added a description of the development process and its mapping onto the RUP phases. |

## RP3 — Software Requirement Specification

| Date | A*/M, D | In charge | Change Description |
|---|---|---|---|
| 05/2026 | A | Hoàng Quốc Trung | Initial version: product overview, actors, use cases and functional requirements. |
| 16/08/2026 | M | All members | Internal review against the FLM template. |
| 24/09/2026 | M | All members | Removed the Finance review round from UC-19 and from the non-screen functions, as it was dropped from the design. |
| 24/09/2026 | M | All members | Corrected the domain entity table: renamed four entities to match the database (ResearchTrack, CouncilMeeting, ProposalReviewScore, ContractDisbursement) and described rubric templates as scoped rather than versioned. |
| 24/09/2026 | M | All members | Removed references to Microsoft Teams from the product overview, use cases and system function tables. |
| 25/09/2026 | M | All members | Updated the cover page, corrected typographical errors, and regenerated the table of contents and the lists of figures and tables. |

## RP4 — Software Design Document

| Date | A*/M, D | In charge | Change Description |
|---|---|---|---|
| 08/2026 | A | Hoàng Quốc Trung | Initial version: system architecture, package diagram, database design and seven detailed-design sections. |
| 16/08/2026 | M | All members | Internal review against the FLM template. |
| 09/2026 | A | All members | Added the List of Figures, the List of Tables and figure captions, with cross-references to every figure and table. |
| 09/2026 | M | All members | Rebuilt the architecture diagram against the current stack: PostgreSQL, Gemini, Brevo and Cloudinary. |
| 09/2026 | M | All members | Replaced the ERD with a physical schema exported from the live database. |
| 19/09/2026 | M | All members | Rewrote Table 1 from the actual repository layout and removed packages that do not exist in the source. |
| 20/09/2026 | M | All members | Updated Table 2 to 65 tables, adding project_decisions and user_research_tracks. |
| 20/09/2026 | M | All members | Redrew the class and sequence diagrams from the current source, correcting class names, entity properties and API routes. |
| 20/09/2026 | A | All members | Split three combined sequence diagrams into separate use cases (3.4.3, 3.6.3, 3.7.3) and added the Class Specifications tables. |
| 21/09/2026 | M | All members | Aligned the wording of sections 1.1, 3.5 and 3.6 with the implemented system. |

Ba dòng `09/2026` là việc bạn làm (hình, ERD, caption) — biết ngày thì điền.

## Cố ý KHÔNG ghi (chưa sửa)

Danh sách nhóm 4 người (RP1 Table 1, RP2 Table 8 vẫn còn Chinh) · phạm vi semantic search · đổi tên FE-08 ·
Phase 7 trong Table 5 RP2 · bỏ Meet/Calendar khỏi Table 11 RP2 · Meet/Calendar ở RP3 §1, UC-17, UC-18, Table 5 ·
tên model ở RP3 Table 5.

---

## Đoạn giải thích quy trình RUP

Dán dưới câu *"Figure N shows the development process the team followed."* ở **RP2 §2.1** và **RP7 §II.2.1**.

⚠️ Chữ `Table 5` là số bảng **trong RP2**. Trong RP7 bảng phase là **Table 7** — đổi cho đúng file.
⚠️ Câu cuối nhắc Phase 7: chỉ giữ nếu bảng phase **có** dòng Phase 7, không thì xoá câu đó.

> The team follows an iterative and incremental process based on the Rational Unified Process (RUP). The phases in Table 5 map onto the four RUP phases as follows. Inception (Phases 1–2) fixes the scope, the team and the requirements baseline (Report 1, Report 2 and SRS v0.9). Elaboration (Phase 3) settles the architecture and the database design (Report 4). Construction (Phases 4a–4c and 5) delivers the system in three iterations, each ending with a software package demonstrated to the supervisors, followed by a stabilisation iteration for system testing. Transition (Phase 6) covers user acceptance testing, deployment and the final reports.
>
> Within each phase the team works in one-week cycles. Requirements, design, implementation and testing run inside the same week rather than as separate stages, and the weekly supervisor meeting feeds changes into the next cycle. This is why the requirements and design documents were revised several times instead of being frozen after their first delivery.
>
> After the project review in September, the team ran one further Construction iteration (Phase 7) to address the board's comments on budgeting, lifecycle deadlines and the archiving of formal decisions, and revised all reports accordingly.

Dòng Phase 7 (nếu thêm vào bảng phase):

| Phase 7 — Revision after Project Review | Week 16–19 | 07/09 – 25/09 | Budget overview, lifecycle deadlines, decision dossier, duplicate detection; revision of all reports |
