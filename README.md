# NASA.Automation — Test Suite

This repository contains automated tests for NASA public APIs and the NASA website sign-up UI using SpecFlow + Playwright and .NET 9.

This README describes the approach, prerequisites, and how to run tests:
- with the Visual Studio Test Runner (Test Explorer)
- from the command line using `dotnet test`
- how to toggle Playwright headless vs. headed modes via the `HEADLESS` environment variable

---

## Project & approach

- Tests are grouped by SpecFlow tags/traits:
  - `@api` — API tests (use `TestCategory=api`)
  - `@ui` — UI tests (use `TestCategory=ui`)
- Playwright is used for UI automation. Browser launch is controlled by `ConfigManager.Headless`.
  - Implementation: `ConfigManager.Headless` reads the environment variable `HEADLESS` and treats `"0"` as non-headless (headed). Any other value (or unset) is treated as headless.
  - Location: `src/NASA.Automation.Core/ConfigManager.cs`
  - Key behavior:
    - HEADLESS = "0" => Headed (visible) browser
    - HEADLESS != "0" or unset => Headless browser

- CI: GitHub Actions workflow is at `.github/workflows/tests.yml`. The UI job installs Playwright browsers before running UI tests.

---

## Prerequisites

- .NET 9 SDK installed
- Node.js (for Playwright tooling) — used to install Playwright browsers: `npx playwright install --with-deps`
- Runner with GUI if you want headed (visible) Playwright runs (local desktop). CI machines usually require headless mode.

---

## Install Playwright browsers (one-time, local / CI step)

From repo root:
- npm installed:
  - `npx playwright install --with-deps`

(That is done in the GitHub Actions workflow for UI jobs.)

---

## Running tests from the command line

General pattern:
- Run all tests:
  - `dotnet test`
- Run only API tests:
  - `dotnet test --filter "TestCategory=api"`
- Run only UI tests:
  - `dotnet test --filter "TestCategory=ui"`

Toggling headless vs. headed (examples):

- PowerShell (headed / visible)
  - `$env:HEADLESS = "0"`
  - `dotnet test --filter "TestCategory=ui"`
- PowerShell (headless)
  - `$env:HEADLESS = "1"`
  - `dotnet test --filter "TestCategory=ui"`

- Cmd.exe (headed)
  - `set HEADLESS=0 && dotnet test --filter "TestCategory=ui"`

- Bash / WSL (headed)
  - `HEADLESS=0 dotnet test --filter "TestCategory=ui"`

Notes:
- If you want to run UI and API together, omit the filter or run multiple invocations.
- Running headed tests on CI usually fails because no graphical session is present. Keep UI tests headless in CI unless you configure a display server (Xvfb) or a self-hosted runner with a desktop session.

---

## Running tests in Visual Studio Test Explorer

1. Ensure the solution is built.
2. Add a run settings file (recommended name: `test.runsettings`) at the solution root containing the `HEADLESS` environment variable (example below).
3. In Visual Studio go to __Test > Configure Run Settings > Select Solution Wide Run Settings File__ and choose the `test.runsettings` file.
4. Open the __Test Explorer__ window.
5. Use the search box or run filters to run tests by category/trait:
   - e.g., filter by `TestCategory` trait `ui` or `api`, or run all tests.
6. When the runsettings file sets `HEADLESS` to `0`, UI tests will launch a visible browser (if you have a desktop session).

Example `test.runsettings`: