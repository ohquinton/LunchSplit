# LunchSplit Code Map

## Project structure

LunchSplit is split into three projects:

| Project | Purpose |
| --- | --- |
| `src/LunchSplit.Core` | Bill splitting, validation, and payment workflow logic |
| `src/LunchSplit.Infrastructure` | Square Sandbox integration |
| `tests/LunchSplit.Tests` | Existing LunchSplit tests and Assignment 3 payment tests |

The payment code is separated so the main application can work with `IPaymentProcessor` without depending directly on the Square SDK.

## Core Payment Classes

**`src/LunchSplit.Core/Payments/`**

| File | Purpose |
| --- | --- |
| `IPaymentProcessor.cs` | Defines the operations for creating a payment and checking its status |
| `PaymentRequest.cs` | Holds the amount, currency, card nonce, and idempotency key |
| `PaymentResult.cs` | Holds payment success/failure details and the transaction ID |
| `PaymentStatus.cs` | Defines the payment status values used by Core |
| `PaymentCoordinator.cs` | Validates payment amounts, calls the processor, and handles attendee batches |
| `BatchPaymentResult.cs` | Keeps individual batch results and counts successful and failed payments |
| `PaymentGatewayTimeoutException.cs` | Represents network and timeout failures during payment processing |

## Square Integration (Tier 1) — Verified

**`src/LunchSplit.Infrastructure/Payments/SquarePaymentService.cs`**

Implements `IPaymentProcessor` using Square's Sandbox API. It converts amounts into cents, sends payment requests, handles Square errors, and retrieves transaction status. The service uses a five-second request timeout. Square credentials are loaded from environment variables rather than stored in the repository.

## Payment flow

1. `PaymentCoordinator` receives a `PaymentRequest`.
2. It checks that the amount is greater than zero and has no more than two decimal places.
3. A valid request is passed to `IPaymentProcessor`.
4. `SquarePaymentService` sends the request to Square when using the real Sandbox implementation. Unit tests substitute a Moq implementation instead.
5. The coordinator returns the result. For batch payments, it keeps a result for each attendee so a declined payment does not stop the remaining attempts.

Previously successful batch payments are not automatically refunded when a later attendee's payment fails.

## Assignment 3 test files

| Test file | Type | Test cases |
| --- | --- | --- |
| `tests/LunchSplit.Tests/Integration/SquareSandboxTests.cs` | Square Sandbox integration | M3-01 successful payment; M3-02 declined card; M3-08 unsupported currency; M3-09 idempotency; M3-10 transaction status |
| `tests/LunchSplit.Tests/Unit/PaymentProcessorMoqTests.cs` | Isolated Moq tests | M3-03 network error; M3-04 correct amount and call count; M3-05 zero-dollar validation; M3-06 four attendees; M3-07 partial batch failure |

### Moq Unit Tests (Tier 2) — Verified

The Moq tests use `IPaymentProcessor` and make no external API calls. The Sandbox tests exercise the Square service itself and require valid Sandbox credentials.

**Implementation Status:** The LunchSplit payment architecture builds successfully. All five Moq unit tests and all five Square Sandbox integration tests passed locally. Square integration was tested using Sandbox credentials configured through environment variables.

## Running the tests

From the solution directory:

```powershell
dotnet restore
dotnet build
dotnet test --filter "TestCategory!=Pending&TestCategory!=SquareSandbox"
```

For the Square integration tests, first configure these environment variables on the machine running the tests:

- `SQUARE_SANDBOX_ACCESS_TOKEN`
- `SQUARE_SANDBOX_LOCATION_ID` (a CAD Sandbox location)

Then run:

```powershell
dotnet test --filter "TestCategory=SquareSandbox"
```

The existing `Pending` tests cover unfinished work from an earlier assignment and are separate from the Assignment 3 payment tests.
