# Invoxa

A C# store billing and invoice system. It calculates subtotals, applies discounts and tax, adds shipping, and prints invoices as plain text or JSON.

Built to practice extensible OOP: pluggable discount/tax policies, layered design, and unit-tested calculation logic separate from output.

---

## What it does

1. Load demo scenarios from `appsettings.json` (or build carts in code).
2. Calculate **subtotal** → apply the **best single discount** (no stacking) → **category tax on discounted amount** → **shipping** → **grand total**.
3. Assign an invoice number, write an audit entry, and render via console text or JSON.

**Customer types**

| Type | Typical discount | Shipping |
|------|------------------|----------|
| Regular | None (unless a promo applies) | ₹50 flat |
| Premium | 10% off subtotal (if best) | ₹50 flat |
| Premium+ | 15% off subtotal (if best) | Free |

**Other pricing rules**

- December seasonal: 20% off subtotal (any customer, if best)
- Buy 2 Get 1 Free on qualifying `productId` items
- Category tax: electronics / general 8%, groceries 2% (proportional after discount)
- Domain validation rejects invalid carts, customers, and line items
- Sequential invoice numbers (`INV-YYYY-#####`, in-memory per process)
- Audit log to console and `logs/audit.log`

---

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (or compatible with `net10.0`)

```bash
dotnet --version
```

---

## Build & run

From the repository root:

```bash
dotnet build
dotnet test
dotnet run --project src/Invoxa.App/Invoxa.App.csproj
```

Edit demo data without recompiling: `src/Invoxa.App/appsettings.json` (copied to the output folder on build).

CI runs `dotnet test` on pushes and pull requests to `main` (see `.github/workflows/ci.yml`).

---

## Solution layout

```
Invoxa/
├── src/
│   ├── Invoxa/              # Core library (domain, pricing, policies, output, audit)
│   └── Invoxa.App/          # Console app + appsettings.json
├── tests/
│   └── Invoxa.Tests/        # Unit tests
├── .github/workflows/       # CI
├── Invoxa.slnx
└── README.md
```

### Calculation flow

```
Config / Cart
  → Subtotal
  → DiscountEngine (highest applicable discount only)
  → Category tax on (subtotal − discount)
  → Shipping policy
  → Invoice (+ number + audit)
  → ConsoleTextFormatter / JsonInvoiceFormatter
```

---

## Architecture (summary)

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `Customer`, `Cart`, `LineItem`, `Invoice`, validation |
| **Pricing** | `InvoiceCalculator`, subtotal |
| **Policies** | `IDiscountPolicy`, `ITaxPolicy`, `IShippingPolicy` |
| **Cross-cutting** | `IAuditLogger`, `IInvoiceNumberGenerator` |
| **Output** | `IInvoiceFormatter` implementations |
| **App** | Config load + wiring in `Program.cs` |

Design goals: new discounts/shipping/tax rules without editing old ones; new output formats without touching calculation; calculator unit-testable without I/O.

**Patterns in use:** Strategy, staged pricing pipeline, composition root, constructor injection, SOLID-friendly seams (especially Open/Closed).

---

## Git ignore

Build output (`bin/`, `obj/`), IDE folders, test results, runtime logs, and local secrets are ignored. Source, project files, CI workflow, and this README are intended for the public repo.

---

## License

Private learning project — add a license if you open-source it later.
