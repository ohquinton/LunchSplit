# Assignment 3 — setup in Visual Studio

1. **Do not overwrite your newer local working tree blindly.** Compare this ZIP with the local branch where you already created `LunchSplit.Infrastructure`.
2. Open `LunchSplit.sln`; restore NuGet packages (`Square` 47.0.0, `Moq` 4.20.72).
3. Build solution; resolve any compiler differences in the installed SDK (the provided source targets Square 47.0.0).
4. Run isolated tests: `dotnet test --filter "TestCategory!=Pending&TestCategory!=SquareSandbox"`.
5. Set user-level environment variables in PowerShell (replace placeholders **locally**, do not paste tokens into chat or commit them):

```powershell
[Environment]::SetEnvironmentVariable('SQUARE_SANDBOX_ACCESS_TOKEN', 'YOUR_SANDBOX_TOKEN', 'User')
[Environment]::SetEnvironmentVariable('SQUARE_SANDBOX_LOCATION_ID', 'YOUR_CAD_LOCATION_ID', 'User')
```

6. Restart Visual Studio to pick up environment changes. Run `dotnet test --filter "TestCategory=SquareSandbox"` in a fresh terminal.
7. Confirm the sandbox location is CAD and that the five live tests actually execute; do not count inconclusive tests as passes.
8. The two old `PendingTddTests` were retained deliberately. `dotnet test` without filtering remains red because they throw NotImplementedException.
9. Preserve actual ten separate Git commits with each test's ID as it is implemented. The ZIP has no Git history.

**Note:** This is a source implementation, not an independently verified build. Live Square Sandbox requires your own configured Sandbox app and token.
