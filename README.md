# LunchSplit

A restaurant bill-splitting engine developed for Software Quality I.

Assignment 1 covers the core functionality, including tip calculation, weighted splitting, rounding, penny reconciliation, input validation, and receipt formatting.

Assignment 3 adds payment processing using the Square Sandbox API, along with integration testing and unit testing using Moq.

**Author:** Quinton Jackson

## Requirements

- .NET 8 SDK
- Visual Studio 2022 (optional)
- Square Sandbox credentials for integration testing

Check the installed .NET version:

```powershell
dotnet --version
```

## Credentials

The Square Sandbox access token and location ID are stored in a `.env` file at the solution root.

The integration tests load these credentials automatically, so no additional configuration is required when the file is present and valid.

The `.env` file is excluded from Git through `.gitignore` to prevent credentials from being uploaded to GitHub.

## Project Structure

```text
LunchSplit.sln
src/
  LunchSplit.Core/
    Models/
    Engine/
    Helpers/
    Output/
    Payments/
    Future/

  LunchSplit.Infrastructure/
    Payments/
      SquarePaymentService.cs

tests/
  LunchSplit.Tests/
    Unit/
      PaymentProcessorMoqTests.cs
    Integration/
      SquareSandboxTests.cs
```

- **LunchSplit.Core:** Contains the bill-splitting logic, domain models, and payment interfaces.
- **LunchSplit.Infrastructure:** Implements payment processing through the Square SDK.
- **LunchSplit.Tests:** Contains the original unit tests and Assignment 3 payment tests.

## Build

From the solution root:

```powershell
dotnet restore
dotnet build
```

## Run All Tests

```powershell
dotnet test
```

Expected results after removing the two unfinished TDD tests:

- 36 tests total
- 36 passed
- 0 failed

## Assignment 3 — Payment Integration

Assignment 3 implements payment processing through Square Sandbox.

The implementation uses `IPaymentProcessor` to separate the payment logic from the Square SDK. This allows the payment coordinator to be tested independently using mocked responses.

The payment implementation also handles declined payments, timeouts, and batch payment results.

### Square Sandbox Integration Tests

These five tests communicate with Square Sandbox and require valid credentials.

```powershell
dotnet test --filter "FullyQualifiedName~SquareSandboxTests"
```

| Test ID | Description |
|---|---|
| M3-01 | Successful payment |
| M3-02 | Declined payment |
| M3-08 | Payment with a unique idempotency key |
| M3-09 | Additional payment integration scenario |
| M3-10 | Additional payment integration scenario |

### Moq Unit Tests

These five tests use Moq to simulate payment processor responses without contacting Square.

```powershell
dotnet test --filter "FullyQualifiedName~PaymentProcessorMoqTests"
```

| Test ID | Test Type |
|---|---|
| M3-03 | Moq unit test |
| M3-04 | Moq unit test |
| M3-05 | Moq unit test |
| M3-06 | Moq unit test |
| M3-07 | Moq unit test |

## Assignment 1 — Original Test Matrix

| ID | Test | File |
|---|---|---|
| M1-1 to M1-5 | ComputeTip | `SplitterTipTests.cs` |
| M2-6 to M2-11 | RoundShares | `RounderTests.cs` |
| M3-12 to M3-16 (+ negative weight) | Validate | `BillValidatorTests.cs` |
| M4-17 to M4-23 | CalculateShares | `SplitterCalculateSharesTests.cs` |
| M5-24, M5-25 | Format | `ReceiptFormatterTests.cs` |

**Notes:**

- M3-16 tests a list where nobody is included because an empty list is already covered by M3-13.
- `Validate_NegativeWeight_ReturnsFail` was added because the specification requires blocking negative weights.

## Design Decisions

- **Money uses `decimal`** to avoid floating-point precision problems.
- **Tips are calculated on the subtotal**, excluding tax.
- **Penny reconciliation** ensures rounded shares add up to the correct grand total.
- **Excluded attendees** receive a $0.00 share so they remain visible on receipts.
- **Immutable records** are used for the main domain models.
- **ReceiptFormatter accepts a clock** so tests can use a fixed date and time.
- **Dependency injection** allows the payment coordinator to work with either a real Square implementation or a mocked payment processor.
- **Moq** allows payment outcomes to be tested without making external API calls.
- **Square Sandbox** is used for integration testing without processing real payments.



## Code Map

See `CODEMAP.md` for a description of the main classes, payment architecture, and testing structure.