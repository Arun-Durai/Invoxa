# Phase 2 — Task Board

Jira-style breakdown for Phase 2. Implement **one task at a time** on its feature branch, merge into `phase-2`, then merge `phase-2` → `main` when the phase is complete.

**Integration branch:** `phase-2`  
**Base:** `main` (Phase 1 — `e92a45f`)

---

## Branch workflow

```
main
 └── phase-2                          ← integration branch for all Phase 2 work
      ├── feature/INVOXA-201-highest-discount-wins
      ├── feature/INVOXA-202-december-seasonal-discount
      ├── feature/INVOXA-203-premium-plus-customer
      ├── feature/INVOXA-204-buy2-get1-free
      └── feature/INVOXA-205-json-export
```

| Step | Action |
|------|--------|
| 1 | Checkout task branch from `phase-2` |
| 2 | Implement + tests |
| 3 | Merge task branch → `phase-2` |
| 4 | Repeat for next task |
| 5 | When all tasks done: merge `phase-2` → `main` and push |

---

## Recommended order

Tasks are ordered so each build builds on the last. **Do not skip ahead** — later tasks assume earlier ones exist.

| Order | ID | Why this order |
|-------|-----|----------------|
| 1 | INVOXA-201 | Foundation — multiple discounts must compete before adding more policies |
| 2 | INVOXA-202 | First new discount policy; uses `PricingContext.AsOf` |
| 3 | INVOXA-203 | New customer type + shipping fee (domain model change) |
| 4 | INVOXA-204 | Line-item promotion; may need `productId` on `LineItem` |
| 5 | INVOXA-205 | New output format; calculation layer should be stable by then |

---

## INVOXA-201 — Highest discount wins (no stacking)

| Field | Value |
|-------|-------|
| **Type** | Story |
| **Priority** | High |
| **Status** | To Do |
| **Branch** | `feature/INVOXA-201-highest-discount-wins` |
| **Depends on** | Phase 1 |

### Description

When more than one discount policy applies to a cart, apply **only the single discount with the highest monetary value**. Discounts must not stack.

`DiscountEngine` may already pick the max — this task verifies behavior, adds competing-policy tests, and documents the rule.

### Acceptance criteria

- [ ] Given two or more applicable discount policies, only one `AppliedDiscount` appears on the invoice
- [ ] The applied discount is the one with the **largest amount** (not percentage)
- [ ] Unit tests cover: premium vs seasonal (once seasonal exists), equal amounts edge case
- [ ] README / task doc updated to mark story Done

### Technical notes

- No change to individual policy classes required if engine already selects max
- Add explicit tests with a second stub/test discount policy

---

## INVOXA-202 — December seasonal discount

| Field | Value |
|-------|-------|
| **Type** | Story |
| **Priority** | High |
| **Status** | To Do |
| **Branch** | `feature/INVOXA-202-december-seasonal-discount` |
| **Depends on** | INVOXA-201 |

### Description

Business wants a **20% discount on subtotal**, applicable **only during December**, regardless of customer type. Implement as a new `IDiscountPolicy` without modifying existing discount classes.

### Acceptance criteria

- [ ] New class `DecemberSeasonalDiscount` (or equivalent) implements `IDiscountPolicy`
- [ ] Applies when `PricingContext.AsOf` is in December (any year)
- [ ] Does not apply in non-December months
- [ ] Works for Regular and Premium customers
- [ ] When Premium 10% and December 20% both apply, invoice shows only the higher discount (INVOXA-201)
- [ ] Unit tests for December vs January
- [ ] Registered in `Program.cs` composition root
- [ ] Sample/demo cart or test date demonstrates December behavior

### Technical notes

- Use injected `DateTime` via `PricingContext.AsOf` — no `DateTime.Now` inside the policy
- Discount amount = 20% of subtotal

---

## INVOXA-203 — Premium+ customer type & free shipping

| Field | Value |
|-------|-------|
| **Type** | Story |
| **Priority** | Medium |
| **Status** | To Do |
| **Branch** | `feature/INVOXA-203-premium-plus-customer` |
| **Depends on** | INVOXA-202 |

### Description

Introduce customer type **Premium+** with:
- **15% discount** on subtotal (via new discount policy)
- **Free shipping** — waive a flat **₹50 shipping fee**

### Acceptance criteria

- [ ] `CustomerType.PremiumPlus` added to domain
- [ ] Flat ₹50 shipping fee applied to all orders by default (new line on invoice or fee component)
- [ ] Premium+ customers have shipping fee waived (₹0 or not shown as charge)
- [ ] Premium+ gets 15% subtotal discount via dedicated `IDiscountPolicy`
- [ ] Invoice shows shipping line where applicable
- [ ] Grand total = subtotal − discount + tax + shipping (confirm formula in tests)
- [ ] Unit tests for Regular (pays shipping), Premium+ (free shipping + 15% off)
- [ ] Console formatter displays shipping line
- [ ] Sample Premium+ cart in `Program.cs`

### Technical notes

- Shipping is a new concept — likely `ShippingFee` on `Invoice` or similar; keep calculation testable
- Premium+ discount competes with other policies per INVOXA-201

---

## INVOXA-204 — Buy 2 Get 1 Free promotion

| Field | Value |
|-------|-------|
| **Type** | Story |
| **Priority** | Medium |
| **Status** | To Do |
| **Branch** | `feature/INVOXA-204-buy2-get1-free` |
| **Depends on** | INVOXA-203 |

### Description

A **Buy 2 Get 1 Free** promotional discount on **specific items**, independent of customer type. For every 3 units of a qualifying item, one unit is free (lowest-cost unit free, unless specified otherwise).

### Acceptance criteria

- [ ] Qualifying items identifiable (e.g. by name or `productId` on `LineItem`)
- [ ] New `IDiscountPolicy` implements B2G1 logic
- [ ] Applies to Regular, Premium, and Premium+ customers
- [ ] Correct discount for qty 2 (no discount), qty 3 (1 free), qty 6 (2 free), etc.
- [ ] Competes with other discounts — only highest value applied (INVOXA-201)
- [ ] Unit tests for qualifying vs non-qualifying items and quantity edge cases
- [ ] Demo line item in `Program.cs` showing promotion

### Technical notes

- May require adding optional `ProductId` to `LineItem` — refactor only if needed
- Discount amount = value of free unit(s), not a percentage of subtotal

---

## INVOXA-205 — JSON invoice export

| Field | Value |
|-------|-------|
| **Type** | Story |
| **Priority** | Medium |
| **Status** | To Do |
| **Branch** | `feature/INVOXA-205-json-export` |
| **Depends on** | INVOXA-204 |

### Description

Invoices must be exportable as **JSON** for an external accounting API, in addition to console text. Add a new formatter without changing `InvoiceCalculator`.

### Acceptance criteria

- [ ] New `JsonInvoiceFormatter` implements `IInvoiceFormatter` (or separate `IInvoiceExporter` if cleaner)
- [ ] JSON includes: customer, line items, subtotal, discount, tax, shipping (if present), grand total, generated timestamp
- [ ] `InvoiceCalculator` unchanged — formatter only reads `Invoice`
- [ ] Unit test asserts JSON structure / key fields (deserialize and assert)
- [ ] `Program.cs` outputs JSON for at least one sample invoice (or writes beside console output)
- [ ] No breaking change to existing console output

### Technical notes

- Use `System.Text.Json`
- Keep formatting logic out of domain classes

---

## Phase 2 definition of done

- [ ] All five stories marked Done
- [ ] All unit tests pass (`dotnet test`)
- [ ] App runs and demonstrates new behavior (`dotnet run`)
- [ ] README updated: Phase 2 status, new features documented
- [ ] `phase-2` merged into `main` and pushed to GitHub

---

## Quick reference — git commands per task

```bash
# Start a task
git checkout phase-2
git pull origin phase-2
git checkout feature/INVOXA-20X-<name>
# ... implement, commit ...
git checkout phase-2
git merge feature/INVOXA-20X-<name>
git push origin phase-2

# Finish Phase 2
git checkout main
git merge phase-2
git push origin main
```
