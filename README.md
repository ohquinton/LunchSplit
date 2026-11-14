LunchSplit
A restaurant bill-splitting engine built with Test-Driven Development for
CSCN72010 Software Quality I (Conestoga College). Assignment 1 covers the core
domain logic: tip calculation, weighted splitting, rounding with penny
reconciliation, input validation and receipt formatting.

Author: Quinton Jackson
Requirements
- .NET 8 SDK
- Check with dotnet --version (should start with 8.)
Project structure
LunchSplit.sln
src/LunchSplit.Core/          Production code (class library)
  Models/                     Bill, Attendee, Share, TipMode, RoundingMode, ValidationResult
  Engine/                     Splitter (tip + share math), Rounder (rounding + penny reconciliation)
  Helpers/                    BillValidator
  Output/                     ReceiptFormatter
  Future/          LunchSplit
A restaurant bill-splitting engine built with Test-Driven Development for
CSCN72010 Software Quality I (Conestoga College). Assignment 1 covers the core
domain logic: tip calculation, weighted splitting, rounding with penny
reconciliation, input validation and receipt formatting.

Author: Quinton Jackson
Requirements
- .NET 8 SDK
- Check with dotnet --version (should start with 8.)
Project structure
LunchSplit.sln
src/LunchSplit.Core/          Production code (class library)
  Models/                     Bill, Attendee, Share, TipMode, RoundingMode, ValidationResult
  Engine/                     Splitter (tip + share math), Rounder (rounding + penny reconciliation)
  Helpers/                    BillValidator
  Output/                     ReceiptFormatter
  Future/                     Stubs for F5 (CSV export) and F6 (Stripe payments) - not implemented yet
tests/LunchSplit.Tests/       MSTest unit tests (28 tests)
Build
dotnet restore
dotnet build
Run the tests
Run everything (26 pass, 2 fail on purpose):
dotnet test
The 2 failing tests (TDD-26, TDD-27) are RED/PENDING TDD placeholders for
features planned in later assignments. To run only the tests that should pass:
dotnet test --filter "TestCategory!=Pending"
Run one test class:
dotnet test --filter "FullyQualifiedName~RounderTests"
Run with code coverage (results in tests/LunchSplit.Tests/TestResults/):
dotnet test --collect:"XPlat Code Coverage"
Design decisions
- Money is always decimal. double cannot store values like 10.325
  exactly, which breaks Banker's rounding.
- Tip is calculated on the subtotal only, not subtotal + tax
  ($100 + $13 tax + 15% tip = $128).
- Penny reconciliation: Rounder rounds each share, then compares the sum
  to the total rounded with the same mode. Leftover pennies go to the shares
  that lost the most in rounding (ties go in list order).
  $10.00 / 3 → $3.34, $3.33, $3.33. Result: sum(Shares) == Bill.GrandTotal.
- RoundingMode.None keeps raw decimals, so the exact-cents sum check only
  applies to Bankers, Up and Down.
- Excluded attendees still get a Share of $0.00 so the receipt lists everyone.
- Records for models (Bill, Attendee, Share) so they are immutable;
  use with to make a changed copy.
- ReceiptFormatter takes a clock (Func<DateTime>) so tests can use a
  fixed time instead of DateTime.Now.
Test matrix
ID	Test	File
M1-1 to M1-5	ComputeTip	SplitterTipTests.cs
M2-6 to M2-11	RoundShares	RounderTests.cs
M3-12 to M3-16 (+ negative weight)	Validate	BillValidatorTests.cs
M4-17 to M4-23	CalculateShares	SplitterCalculateSharesTests.cs
M5-24, M5-25	Format	ReceiptFormatterTests.cs
TDD-26, TDD-27	RED/PENDING	PendingTddTests.cs


Notes:
- M3-16 tests a list where nobody is included, because an empty list is
  already covered by M3-13.
- Validate_NegativeWeight_ReturnsFail was added because the spec requires
  blocking negative weights but the matrix had no test for it.
Square Sandbox access for grading
The five Square Sandbox integration tests were run successfully using a CAD Sandbox location. The Square access token is not stored in this repository or the submitted ZIP. The instructor can obtain the Sandbox credentials through a private, secure sharing link provided separately (for example, through the course LMS). No Square account password is required.

After retrieving the Sandbox access token and location ID, open PowerShell in the solution directory and set the environment variables for that terminal session:
$env:SQUARE_SANDBOX_ACCESS_TOKEN = Read-Host "Enter Sandbox Access Token"
$env:SQUARE_SANDBOX_LOCATION_ID = Read-Host "Enter Sandbox Location ID"
Run the integration tests:
dotnet test --filter "FullyQualifiedName~SquareSandboxTests"
The five Moq unit tests do not require Square credentials:
dotnet test --filter "FullyQualifiedName~PaymentProcessorMoqTests"
The shared credentials must be set on the machine where the tests run. Neither the access token nor the secure-sharing link is included in this repository.
