# Invoxa

A C# store billing and invoice system. It calculates subtotals, applies discounts and tax, adds shipping, and prints invoices as plain text or JSON.

Built to practice extensible OOP: pluggable discount/tax policies, layered design, and unit-tested calculation logic separate from output.

---

## What it does

1. Build a cart for a customer with one or more line items.
2. Calculate **subtotal** → apply the **best single discount** (no stacking) → **tax on discounted amount** → **shipping** → **grand total**.
3. Render the invoice via console text or JSON.

**Customer types**

| Type | Typical discount | Shipping |
|------|------------------|----------|
| Regular | None (unless a promo applies) | ₹50 flat |
| Premium | 10% off subtotal (if best) | ₹50 flat |
| Premium+ | 15% off subtotal (if best) | Free |

**Other pricing rules**

- December seasonal: 20% off subtotal (any customer, if best)
- Buy 2 Get 1 Free on qualifying `productId` items
- Domain validation rejects invalid carts, customers, and line items

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

The console app prints sample invoices (and a JSON sample for the first scenario).

---

## Solution layout

```
Invoxa/
├── src/
│   ├── Invoxa/              # Core library (domain, pricing, discounts, tax, output)
│   └── Invoxa.App/          # Console composition root + demo data
├── tests/
│   └── Invoxa.Tests/        # Unit tests
├── Invoxa.slnx
└── README.md
```

### Calculation flow

```
Cart
  → Subtotal
  → DiscountEngine (highest applicable discount only)
  → Tax on (subtotal − discount)
  → Shipping
  → Invoice
  → ConsoleTextFormatter / JsonInvoiceFormatter
```

---

## Architecture (summary)

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `Customer`, `Cart`, `LineItem`, `Invoice`, validation |
| **Pricing** | `InvoiceCalculator`, subtotal, shipping |
| **Policies** | `IDiscountPolicy`, `ITaxPolicy` |
| **Output** | `IInvoiceFormatter` implementations |
| **App** | Wires dependencies in `Program.cs` |

Design goals: new discounts without editing old ones; new output formats without touching calculation; calculator unit-testable without I/O.

**Patterns in use:** Strategy (discount/tax), staged pricing pipeline, composition root, constructor injection, SOLID-friendly seams (especially Open/Closed).

---

## Git ignore

Build output (`bin/`, `obj/`), IDE folders, test results, and local secrets are ignored. Only source, project files, and this README are intended for the public repo.

---

## License

Private learning project — add a license if you open-source it later.
