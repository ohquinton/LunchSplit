# LunchSplit Code Map

## Project Structure

LunchSplit contains three projects:

| Project | Purpose |
| --- | --- |
| `src/LunchSplit.Core` | Bill splitting, validation, and payment workflow logic |
| `src/LunchSplit.Infrastructure` | Square Sandbox integration |
| `tests/LunchSplit.Tests` | Original LunchSplit tests and Assignment 3 payment tests |

The payment code is separated so the main application can use `IPaymentProcessor` without depending directly on the Square SDK.

## Core Payment Classes

**`src/LunchSplit.Core/Payments/`**

| File | Purpose |
| --- | --- |
| `IPaymentProcessor.cs` | Defines payment creation and status operations |
| `PaymentRequest.cs` | Holds the amount, currency, card nonce, and idempotency key |
| `PaymentResult.cs` | Holds payment results and transaction details |
| `PaymentStatus.cs` | Defines the payment status values |
| `PaymentCoordinator.cs` | Validates payments, calls the processor, and handles attendee batches |
| `BatchPaymentResult.cs` | Stores individual batch results and success/failure counts |
| `PaymentGatewayTimeoutException.cs` | Represents payment gateway timeout and network failures |

## Square Integration (Tier 1)

**`src/LunchSplit.Infrastructure/Payments/SquarePaymentService.cs`**

Implements `IPaymentProcessor` using the Square Sandbox API. It converts amounts into cents, sends payment requests, handles Square errors, and retrieves payment status.

The service uses a five-second request timeout. Sandbox credentials are stored in a `.env` file at the solution root rather than hardcoded in the source code.

## Payment Flow

1. `PaymentCoordinator` receives a `PaymentRequest`.
2. It validates that the amount is greater than zero and has no more than two decimal places.
3. Valid requests are passed to `IPaymentProcessor`.
4. `SquarePaymentService` communicates with Square Sandbox. Unit tests use a mocked processor instead.
5. The coordinator returns the result. For batch payments, each attendee receives an individual result, allowing the remaining payments to continue if one fails.

Successful payments are not automatically refunded if another payment in the batch fails.

## Assignment 3 Test Files

| Test File | Type | Test Cases |
| --- | --- | --- |
| `tests/LunchSplit.Tests/Integration/SquareSandboxTests.cs` | Square Sandbox integration | M3-01 successful payment; M3-02 declined card; M3-08 unsupported currency; M3-09 idempotency; M3-10 transaction status |
| `tests/LunchSplit.Tests/Unit/PaymentProcessorMoqTests.cs` | Moq unit tests | M3-03 network error; M3-04 amount and call verification; M3-05 zero-dollar validation; M3-06 four attendees; M3-07 partial batch failure |

The five Moq tests run without external API calls. The five integration tests communicate with Square Sandbox.

All ten Assignment 3 tests passed locally.

## Running the Tests

From the solution directory:

```powershell
dotnet restore
dotnet build
dotnet test
```

To run only the five Square Sandbox integration tests:

```powershell
dotnet test --filter "FullyQualifiedName~SquareSandboxTests"
```

To run only the five Moq unit tests:

```powershell
dotnet test --filter "FullyQualifiedName~PaymentProcessorMoqTests"
```

The Square Sandbox tests automatically load credentials from the `.env` file at the solution root. The file must contain valid `SQUARE_SANDBOX_ACCESS_TOKEN` and `SQUARE_SANDBOX_LOCATION_ID` values.