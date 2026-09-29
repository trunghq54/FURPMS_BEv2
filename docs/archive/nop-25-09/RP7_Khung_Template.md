# RP7 — khung template gốc (thân bài) để đối chiếu bản nộp

> Chép từ template RP7 **trống** user gửi ngày 25/09/2026.
> ⚠️ **Mục lục của template đã cũ, không khớp thân bài** — luôn đối chiếu theo danh sách DƯỚI ĐÂY (lấy từ thân bài).
> Dùng làm checklist: bản RP7 nộp phải có đủ mọi heading dưới, đúng cấp.

## Trang đầu

- [ ] Trang bìa: tên dự án · tên nhóm · thành viên `<Member name><RollNo><Student code>` · Supervisor · Ext Supervisor · Capstone Project code · `- <Thành phố>, <tháng>/<năm> -`
- [ ] Table of Contents
- [ ] Acknowledgement
- [ ] Definition and Acronyms (bảng 2 cột: Acronym · Definition) — **xoá dòng mẫu `PWM — Psychology website`, `AWS`**
- (không bắt buộc theo template, nhưng cẩm nang §1.2 đòi) List of Figures · List of Tables

## I. Project Introduction  ← RP1 Part II
- [ ] 1. Overview — 1.1 Project Information · 1.2 Project Team
- [ ] 2. Product Background
- [ ] 3. Existing Systems
- [ ] 4. Business Opportunity
- [ ] 5. Software Product Vision
- [ ] 6. Project Scope & Limitations

## II. Project Management Plan  ← RP2
- [ ] 1. Overview — 1.1 Scope & Estimation · 1.2 Project Objectives · 1.3 Project Risks
- [ ] 2. Management Approach — 2.1 Project Process (*vẽ + mô tả* mô hình quy trình) · 2.2 Quality Management · 2.3 Training Plan
- [ ] 3. Project Deliverables
- [ ] 4. Responsibility Assignments
- [ ] 5. Project Communications
- [ ] 6. Configuration Management — 6.1 Document Management · 6.2 Source Code Management · 6.3 Tools & Infrastructures

## III. Software Requirement Specification  ← RP3
- [ ] 1. Product Overview
- [ ] 2. User Requirements (actor, use case diagram, use case description)
- [ ] 3. Functional Requirements — 3.1 System Functional Overview (screen flow, screen descriptions, roles, screen authorization, non-screen functions, **ERD**) · 3.2… các feature, mỗi feature có các function 3.x.y
- [ ] 4. Non-Functional Requirements — 4.1 External Interfaces · 4.2 Quality Attributes
- [ ] 5. Requirement Appendix — 5.1 Business Rules · 5.2 Common Requirements · 5.3 Application Messages List · 5.4 Other Requirements

## IV. Software Design Description  ← RP4
- [ ] 1. System Design — 1.1 System Architecture · 1.2 Package Diagram
- [ ] **2. Database Design** (file/table description, quan hệ bảng, mô tả bảng)
- [ ] **3. Detailed Design** — mỗi feature 3.x gồm **Class Diagram · Class Specifications · Sequence Diagram(s)**

## V. Software Testing Documentation  ← RP5
- [ ] 1. Scope of Testing
- [ ] 2. Test Strategy — 2.1 Testing Types · 2.2 Test Levels · 2.3 Supporting Tools
- [ ] 3. Test Plan — 3.1 Human Resources · 3.2 Test Environment · 3.3 Test Milestones
- [ ] 4. Test Cases (tham chiếu `Report5_Unit Test.xls`, `Report5_Test Report.xls`)
- [ ] 5. Test Reports

## VI. Release Package & User Guides  ← RP6
- [ ] 1. Deliverable Package — bảng 9 dòng mẫu (Schedule/Task Tracking · Project Backlog · Source Codes · Database Script(s) · Final Report Document · Test Cases Document · Defects List · Issues List · Slide), **phải điền cột Description**, được bỏ dòng không dùng
- [ ] 2. Installation Guides — 2.1 System Requirements · 2.2 Installation Instruction
- [ ] 3. User Manual — 3.1 Overview · 3.2, 3.3… mỗi workflow một mục

## VII. Appendix
Chỉ có trong **mục lục**, **thân bài template không có** ⇒ tuỳ chọn. Nếu có: 1. Glossary · 2. References · 3. Others.

## Lỗi hay gặp khi đối chiếu
- Heading dán vào sai cấp ⇒ mục lục mất mục (đã gặp 22/09: mất `II.`, `V.`, `VII.`)
- Dán thiếu nửa sau của chương (đã gặp 25/09: §IV chỉ có `1. System Design`, mất `2. Database Design` và `3. Detailed Design`)
- Còn sót đoạn chữ xanh `[...]` hướng dẫn của template — `Ctrl+F` tìm `[`
