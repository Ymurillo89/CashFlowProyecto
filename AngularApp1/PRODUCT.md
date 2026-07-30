# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users
- **Cashiers (Cajeros):** Employees at various point-of-sale locations who make physical bank deposits and need to quickly upload proof (photos/receipts) of the transaction.
- **Store Managers (Gerentes de Sucursal):** Oversee store operations and validate the cashiers' deposits.
- **Administrators (Administradores):** Centralized financial controllers who monitor all company cash flows, audit discrepancies, and manage master data (Stores, Users, Banks).

## Product Purpose
To control, validate, and centralize cash deposits across multiple retail locations. It replaces manual verification by using Artificial Intelligence (OCR) to automatically extract data (amount, bank, date, reference) from uploaded bank deposit receipts and compare it against the declared amounts.

## Positioning
An AI-powered financial reconciliation tool that eliminates human error in cash deposit verification by directly reading physical banking receipts and matching them against POS records in real-time.

## Operating Context
Users operate in fast-paced retail environments. Cashiers upload photos of paper receipts often captured via mobile devices or scanned at the store back-office. The system acts as the central source of truth for cash reconciliation between the physical store and the corporate bank accounts.

## Capabilities and Constraints
- **Tech Stack:** Angular, Tailwind CSS, PrimeNG for frontend. .NET Core API and PostgreSQL for backend.
- **Language Rule:** All codebase (frontend & backend) must be strictly in English. The Database schema is in Spanish (`Flow_tbl...`).
- **Core Features:** User Authentication, Role-based Access, Multi-tenant (Companies/Stores), Deposit Registration, File Uploads, OCR Integration (Gemini Vision), Excel Export.

## Brand Commitments
- Name: CashFlow Control AI.
- Design: Must feel premium, modern, dynamic, and trustworthy.

## Evidence on Hand
- Currently an MVP (Minimum Viable Product). No existing user base or historical data yet.

## Product Principles
1. **Accuracy above all:** Financial data must be trustworthy and traceable; every deposit needs an auditable trail.
2. **Frictionless entry:** Cashiers should spend minimal time uploading receipts; the AI should do the heavy lifting.
3. **Clear discrepancy resolution:** When the AI and the declared amount differ, the UI must make the conflict obvious and easy to resolve for managers.
4. **Professional & Secure:** As a financial tool, the interface must inspire absolute confidence through clean, modern, and impeccable design.
