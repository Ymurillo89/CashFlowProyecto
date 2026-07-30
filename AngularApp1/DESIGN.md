---
name: CashFlow Control AI
description: Centralized cash deposit verification and AI OCR validation.
---

<!-- SEED: established with the user before implementation; re-run /impeccable document once there's code to capture the actual tokens and components. -->

# Design System: CashFlow Control AI

## Overview

**Creative North Star: "High-Precision Optical Sorting Bench"**

The application feels like a piece of clinical, industrial optical sorting equipment. It is structured, high-contrast, and built for exactness. When a cashier uploads a receipt, it behaves as a physical document sliding into a laser scanner; when a manager reviews discrepancies, the interface highlights mismatches using sharp, high-contrast targeting reticles (glassmorphism accents). It rejects the soft, friendly defaults of typical SaaS dashboards in favor of serious, trustworthy, and precise financial authority.

**Key Characteristics:**
- Clinical and structured.
- High-contrast precision markers.
- Laser-scanner interaction feel.
- Complete financial transparency.

## Colors

The palette strategy relies on a clean, industrial neutral background with sharp, saturated accent colors used specifically to highlight data states (validated vs. anomalous).

### Primary
- **[to be resolved during implementation]**

### Secondary
- **[to be resolved during implementation]**

### Neutral
- **[to be resolved during implementation]**

**The Precision State Rule.** Colors do not exist for decoration. A saturated color only appears on screen to indicate a verified state (e.g., successful OCR match) or an anomaly (e.g., amount mismatch).

## Typography

**Display Font:** [to be resolved during implementation]
**Body Font:** [to be resolved during implementation]
**Label/Mono Font:** [to be resolved during implementation]

**Character:** Highly legible, monospaced or technical sans-serif typography that feels like aerospace or banking spec sheets.

### Hierarchy
- **Display**: [to be resolved during implementation]
- **Headline**: [to be resolved during implementation]
- **Title**: [to be resolved during implementation]
- **Body**: [to be resolved during implementation]
- **Label**: [to be resolved during implementation]

**The Data-First Rule.** All financial figures use tabular (monospaced) lining figures so columns align perfectly and discrepancies stand out visually.

## Layout

A rigid, paneled spatial grammar. Content is organized in distinct, bolted-down sections similar to a laboratory dashboard. The screen does not use floating cards; instead, it uses a full-bleed grid where panels touch or have hairline borders to maximize data density and precision. Responsive behavior collapses horizontal panels into vertically stacked modules without losing the hairline structure.

## Elevation & Depth

Surfaces are strictly flat. There are no ambient drop shadows. Depth is conveyed only through physical layering (e.g., a modal sliding in like a glass inspection plate) or through high-contrast borders and dimming of inactive panels.

**The Glass Plate Rule.** Active inspection states (like reviewing a scanned receipt) sit above the flat interface as an un-shadowed, semi-transparent plate (glassmorphism) with sharp borders, mimicking a physical slide under a microscope.

## Shapes

Corners are razor-sharp or minimally rounded (e.g., 2px radius). Buttons, inputs, and panels share a hard, mechanical form language. Borders are crisp hairlines (1px), emphasizing the grid-like nature of a technical instrument.

## Do's and Don'ts

### Do:
- **Do** use strict alignment for all numerical data.
- **Do** treat the receipt upload and scanning interaction as a central, mechanical event.
- **Do** use a dark or muted industrial background for the dashboard frame to let the data panels stand out.

### Don'ts:
- **Don't** use soft, large border radii on cards or buttons.
- **Don't** use ambient drop shadows to create depth.
- **Don't** clutter the screen with decorative illustrations or colors; keep the focus on the data and the scanner.
