LunchSplit
A restaurant bill-splitting engine built with Test-Driven Development. the core domain logic: tip calculation, weighted splitting, rounding with penny reconciliation, input validation, and receipt formatting.

Assignment 3 adds Square Sandbox payment integration and isolated unit tests using Moq. The payment interface keeps the core payment workflow separate from the Square SDK.

Author: Quinton Jackson
Requirements
- .NET 8 SDK
- Check the installed version with dotnet --version.
Project structure
LunchSplit.sln
CODEMAP.md
README.md
.env                 Local Square Sandbox credentials (not tracked by Git)
src/
  LunchSplit.Core/
    Models/              Bill, Attendee, Share, TipMode, RoundingMode, ValidationResult
    Engine/              Splitter and Rounder
    Helpers/             BillValidator
    Output/              ReceiptFormatter
    Future/              Earlier CSV export and payment placeholders
    Payments/            Payment models, interface, and coordinator
  LunchSplit.Infrastructure/
    Payments/
      SquarePaymentService.cs
tests/
  LunchSplit.Tests/
    Integration/
      SquareSandboxTests.cs
    Unit/
      PaymentProcessorMoqTests.cs
    [existing LunchSplit test files]
Build
From the directory containing LunchSplit.sln:
dotnet restore
dotnet build
Run the tests
Run the entire suite:
dotnet test
Two existing tests (TDD-26 and TDD-27) are intentionally unfinished RED/PENDING TDD placeholders from earlier work. To exclude those tests:
dotnet test --filter "TestCategory!=Pending"
To run tests that do not require the Square Sandbox:
dotnet test --filter "TestCategory!=Pending&TestCategory!=SquareSandbox"
Run one existing test class:
dotnet test --filter "FullyQualifiedName~RounderTests"
Run with code coverage (results are written under tests/LunchSplit.Tests/TestResults/):
dotnet test --collect:"XPlat Code Coverage"
Design decisions
- Money is always decimal. double cannot represent some decimal fractions exactly, which can produce rounding errors.
- Tip is calculated on the subtotal only, not subtotal plus tax ($100 subtotal + $13 tax + 15% tip = $128).
- Penny reconciliation: Rounder rounds each share, then compares the sum to the total rounded with the same mode. Leftover pennies go to shares that lost the most in rounding (ties go in list order). $10.00 / 3 becomes $3.34, $3.33, and $3.33, so sum(Shares) == Bill.GrandTotal.
- RoundingMode.None keeps raw decimals, so the exact-cents sum check only applies to Bankers, Up, and Down.
- Excluded attendees still get a Share of $0.00 so the receipt lists everyone.
- Records for models (Bill, Attendee, Share) keep them immutable; use with to make a changed copy.
- ReceiptFormatter takes a clock (Func<DateTime>) so tests can use a fixed time instead of DateTime.Now.
- Payment integration uses IPaymentProcessor so unit tests can replace Square with a Moq object. See CODEMAP.md for the payment class paths.
Original test matrix
ID	Test	File
M1-1 to M1-5	ComputeTip	SplitterTipTests.cs
M2-6 to M2-11	RoundShares	RounderTests.cs
M3-12 to M3-16 (+ negative weight)	Validate	BillValidatorTests.cs
M4-17 to M4-23	CalculateShares	SplitterCalculateSharesTests.cs
M5-24, M5-25	Format	ReceiptFormatterTests.cs
TDD-26, TDD-27	RED/PENDING	PendingTddTests.cs


Notes:
- M3-16 tests a list where nobody is included, because an empty list is already covered by M3-13.
- Validate_NegativeWeight_ReturnsFail was added because the specification requires blocking negative weights, but the original matrix had no test for it.
Assignment 3 payment tests
Tier	File	Test cases
Square Sandbox integration	tests/LunchSplit.Tests/Integration/SquareSandboxTests.cs	M3-01, M3-02, M3-08, M3-09, M3-10
Moq unit tests	tests/LunchSplit.Tests/Unit/PaymentProcessorMoqTests.cs	M3-03, M3-04, M3-05, M3-06, M3-07


All five Square Sandbox tests and five Moq tests passed during local verification.
Square Sandbox setup
The integration tests use a Square Sandbox account with a CAD location. The access token and location ID are stored in a .env file beside LunchSplit.sln, not in the C# code.

The .env file contains:
SQUARE_SANDBOX_ACCESS_TOKEN=your_sandbox_access_token
SQUARE_SANDBOX_LOCATION_ID=your_sandbox_location_id
The integration tests load these values from .env when they are not already set as environment variables. The .env file is ignored by Git.

Run the Square Sandbox integration tests:
dotnet test --filter "FullyQualifiedName~SquareSandboxTests"
Run the Moq unit tests (no Square credentials needed):
dotnet test --filter "FullyQualifiedName~PaymentProcessorMoqTests"
