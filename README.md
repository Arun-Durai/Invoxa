# Invoxa — Store Billing & Invoice System

A C# billing system that calculates invoices with discounts and tax, then prints them as formatted plain-text output. Built in phases to practice extensible OOP design — each phase adds requirements that stress the architecture.

Uses **layered architecture**, **Strategy-pattern** policies, **SOLID**-friendly seams, and **constructor injection** so new discounts and output formats plug in without rewriting the calculator. See [Architecture & OOP concepts](#architecture--oop-concepts).

**Current status:** Phase 2 complete on `main` and `phase-2`. See [`docs/PHASE2-TASKS.md`](docs/PHASE2-TASKS.md).

---

## Requirements by phase

### Phase 1 — Core (implemented)

**Entities:** Customer, Line Item, Invoice, Discount, Tax

| Rule | Behavior |
|------|----------|
| Cart | A customer purchases one or more items (name, unit price, quantity). |
| Subtotal | Sum of `unit price × quantity` for all items. |
| Tax | 8% applied on the **discounted** subtotal (`subtotal − discount`). |
| Invoice | Shows item list, subtotal, tax, discount (if any), and grand total. |
| Output | Printable plain-text format (console). |

**Customer types (discount only):**

| Type | Discount |
|------|----------|
| Regular | None |
| Premium | 10% off subtotal |

### Phase 2 — Complete

| # | Feature | Status |
|---|---------|--------|
| 1 | Highest discount wins (no stacking) | Done |
| 2 | December seasonal 20% off | Done |
| 3 | Premium+ (15% off + free ₹50 shipping) | Done |
| 4 | Buy 2 Get 1 Free on qualifying items | Done |
| 5 | JSON invoice export | Done |

**Customer types:**

| Type | Discount | Shipping |
|------|----------|----------|
| Regular | Best applicable policy only | ₹50 flat fee |
| Premium | 10% off subtotal (if best) | ₹50 flat fee |
| Premium+ | 15% off subtotal (if best) | Free |

**Other Phase 2 rules:**
- December 20% off subtotal (any customer, if best discount)
- B2G1 on qualifying `productId` items (1 free unit per 3 purchased)
- Console text + JSON output formats

### Phase 3 — Stretch

- **Audit log** — log customer name, timestamp, and total on every invoice generation.
- **Category-based tax** — e.g. electronics 8%, groceries 2%.

---

## Design principles

These non-functional goals drive the structure:

| Goal | How it's addressed |
|------|---------------------|
| New discount types without editing existing discount code | `IDiscountPolicy` + one class per policy |
| New output formats without touching calculation | `IInvoiceFormatter` + `Invoice` result object |
| Unit-testable calculation | `InvoiceCalculator` has no console or file I/O |

**Tax order:** discount first, then 8% tax on the reduced amount.

**Domain model:** classes with constructors and read-only properties (object-oriented, immutable after construction).

---

## Architecture & OOP concepts

This is intentionally structured as a **small but real** billing engine — not a single `Program.cs` script with hardcoded math. The sample data in `Program.cs` is only the **input layer**; pricing rules, tax, and output live in testable, swappable components.

### Layered architecture

| Layer | Responsibility | Key types |
|-------|----------------|-----------|
| **Domain** | Business nouns — what exists in the problem space | `Customer`, `Cart`, `LineItem`, `Invoice` |
| **Pricing** | Orchestration and calculation pipeline | `InvoiceCalculator`, `SubtotalCalculator`, `PricingContext` |
| **Policies** | Pluggable rules (discount & tax) | `IDiscountPolicy`, `ITaxPolicy` |
| **Output** | Presentation only — no pricing logic | `IInvoiceFormatter`, `ConsoleTextFormatter` |
| **Application** | Composition root — wires dependencies | `Program.cs`, `BillingService` |

Calculation never imports formatters. Formatters never compute totals. That **separation of concerns** is what keeps Phase 2 changes local instead of rippling everywhere.

### OOP pillars in this codebase

| Concept | What it means here | Where |
|---------|-------------------|--------|
| **Encapsulation** | State is hidden behind constructors and read-only properties; services own their dependencies | `Cart`, `Customer`, `InvoiceCalculator` |
| **Abstraction** | Callers depend on *what* a component does, not *how* | `IDiscountPolicy`, `ITaxPolicy`, `IInvoiceFormatter` |
| **Polymorphism** | Same interface, different behavior at runtime | `PremiumCustomerDiscount` vs future `DecemberSeasonalDiscount`; `FlatRateTaxPolicy` vs future category-based tax |
| **Composition** | Behavior is built by combining objects, not deep inheritance trees | `DiscountEngine` holds a list of `IDiscountPolicy`; `InvoiceCalculator` composes engine + tax policy |

### SOLID principles (applied)

| Principle | How Invoxa follows it |
|-----------|----------------------|
| **S** — Single Responsibility | `SubtotalCalculator` only sums lines; `DiscountEngine` only resolves discounts; `ConsoleTextFormatter` only renders text |
| **O** — Open/Closed | Add a new discount by creating a new `IDiscountPolicy` class and registering it — no edits to `PremiumCustomerDiscount` or the engine's core loop |
| **L** — Liskov Substitution | Any `IDiscountPolicy` / `ITaxPolicy` implementation can be plugged in without breaking `InvoiceCalculator` |
| **I** — Interface Segregation | Small, focused interfaces (`IsApplicable` + `Calculate`) instead of one giant "billing" interface |
| **D** — Dependency Inversion | `InvoiceCalculator` depends on `DiscountEngine` and `ITaxPolicy` abstractions, not concrete tax/discount details |

### Design patterns

| Pattern | Role in Invoxa |
|---------|----------------|
| **Strategy** | `IDiscountPolicy` and `ITaxPolicy` — interchangeable algorithms selected at runtime |
| **Pipeline / staged calculation** | Subtotal → discount → tax → grand total, each stage isolated |
| **Result object (DTO)** | `Invoice` is an immutable snapshot of a completed calculation, safe to format or (later) serialize to JSON |
| **Composition root** | `Program.cs` constructs and wires all dependencies in one place — the only file that knows the full graph |
| **Constructor injection** | `InvoiceCalculator(DiscountEngine, ITaxPolicy)` — dependencies are explicit and mockable in tests |

### Cross-cutting technical choices

| Topic | Choice | Why |
|-------|--------|-----|
| **Immutability** | Read-only properties on domain classes | Invoice data must not change after calculation; avoids accidental mutation bugs |
| **Sealed classes** | `sealed` on concrete types | Clear intent: extend via interfaces/policies, not inheritance |
| **Decimal for money** | `decimal` + `Money.Round()` | Avoid floating-point rounding errors on currency |
| **Enum for customer type** | `CustomerType.Regular \| Premium` | Type-safe domain vocabulary |
| **Unit tests without I/O** | Tests call `InvoiceCalculator` directly | Proves calculation logic is independent of console output |
| **Manual DI (for now)** | Wiring in `Program.cs` | Simple and explicit; can move to `appsettings.json` + factory later without touching the calculator |

### How Phase 2 maps to this design (why the structure matters)

| Upcoming requirement | Extension point — no rewrite |
|---------------------|------------------------------|
| December 20% off | New `IDiscountPolicy` implementation; uses `PricingContext.AsOf` |
| Premium+ + free shipping | New customer enum value + new policy (shipping as a line or fee object later) |
| Buy 2 Get 1 Free | New `IDiscountPolicy` per promotion |
| JSON export | New `IInvoiceFormatter` (or `IInvoiceExporter`) |
| Highest discount wins | Already in `DiscountEngine.Resolve()` — Phase 2 only adds competing policies |
| Category-based tax (Phase 3) | New `ITaxPolicy` implementation |

The hardcoded carts in `Program.cs` are **demo fixtures**, not the architecture. The architecture is the policy pipeline, domain model, and output abstraction underneath.

---

## Solution structure

```
Invoxa/
├── src/
│   ├── Invoxa/                 # Core library
│   │   ├── Domain/             # Customer, Cart, LineItem, Invoice, …
│   │   ├── Pricing/            # InvoiceCalculator, SubtotalCalculator
│   │   ├── Discounts/          # IDiscountPolicy, DiscountEngine
│   │   ├── Tax/                # ITaxPolicy, FlatRateTaxPolicy
│   │   └── Output/             # IInvoiceFormatter, ConsoleTextFormatter
│   └── Invoxa.App/             # Console entry point (sample data in Program.cs)
├── tests/
│   └── Invoxa.Tests/           # Unit tests for calculation & formatting
├── Invoxa.slnx
└── README.md
```

### Calculation flow

```
Cart
  → Subtotal
  → DiscountEngine (pick highest applicable discount)
  → Tax on (subtotal − discount)
  → Invoice
  → ConsoleTextFormatter (app layer only)
```

---

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (or compatible SDK for `net10.0`)

```bash
dotnet --version
```

---

## Build & run

From the repository root:

```bash
# Build entire solution
dotnet build

# Run unit tests
dotnet test

# Run the console demo (prints two sample invoices)
dotnet run --project src/Invoxa.App/Invoxa.App.csproj
```

### Sample output

The app runs two hardcoded carts in `Program.cs`:

1. **Regular customer** — no discount, tax on full subtotal.
2. **Premium customer** — 10% discount, tax on discounted amount.

---

## Configuration (future)

Sample carts and wiring (tax rate, discount policies) live in `Program.cs` for now. The plan is to move them to a config file (e.g. `appsettings.json`) without changing `InvoiceCalculator` or domain logic.

---

## What is not committed to git

Build artifacts and local tooling files are ignored via `.gitignore`, including:

- `bin/` and `obj/` — compiled output and MSBuild intermediates
- `.vs/`, `.idea/` — IDE metadata
- `TestResults/`, coverage files — test run output
- `.DS_Store` — macOS folder metadata
- Local secrets / env files — `appsettings.*.local.json`, `.env`

Only source, project files, solution file, and docs should be pushed to GitHub.

---

## License

Private learning project — add a license if you open-source it later.
