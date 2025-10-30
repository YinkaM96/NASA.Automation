# NASA.Automation — Test Suite

Automated test suite for NASA public APIs and the NASA Open APIs sign-up UI.  
Built with **SpecFlow**, **RestSharp**, and **Playwright for .NET 9**.

This project demonstrates:
- API automation with SpecFlow (`@API` tagged scenarios)
- UI automation using Playwright (`@UI` tagged scenarios)
- CI/CD integration via GitHub Actions (headless UI tests)
- HTML reporting with **ExtentReports**

---

## Project structure & approach

| Layer | Description |
|--------|-------------|
| `NASA.Automation.Core` | Shared utilities (e.g., config, constants) |
| `NASA.Automation.API` | REST clients using **RestSharp** |
| `NASA.Automation.UI` | Page Objects and Playwright setup |
| `NASA.Automation.Tests` | SpecFlow features, steps, hooks, and ExtentReports integration |

### Tags and test filters
- `@API` → API tests → `dotnet test --filter "TestCategory=API"`
- `@UI` → UI tests → `dotnet test --filter "TestCategory=UI"`

### Headless vs. headed mode
- Controlled via `ConfigManager.Headless`
- Reads the environment variable `HEADLESS`
  - `HEADLESS=0` → headed (browser visible)
  - `HEADLESS=1` or unset → headless (no UI, ideal for CI)

---

## Prerequisites

To run tests locally, you’ll need:
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- PowerShell (Windows/macOS/Linux)
- Chrome/Chromium (automatically installed by Playwright)

> **Note:** Node.js is **not required** — this project uses **Playwright for .NET**, not the JavaScript version.

---

## Local setup

```powershell
# Restore dependencies
dotnet restore

# Build solution
dotnet build --configuration Release

# Install the .NET Playwright CLI tool (once per machine)
dotnet tool install --global Microsoft.Playwright.CLI

# Install browsers for .NET Playwright (must run after build)
playwright install --with-deps
```

This downloads and configures the browsers (Chromium, Firefox, WebKit) for .NET.  
You’ll only need to re-run it if Playwright updates.

---

## Running tests from the command line

### Run all tests
```bash
dotnet test
```

### Run only API tests
```bash
dotnet test --filter "TestCategory=API"
```

### Run only UI tests (headless by default)
```bash
dotnet test --filter "TestCategory=UI"
```

### Headed (visible) browser runs

#### PowerShell
```powershell
$env:HEADLESS = "0"
dotnet test --filter "TestCategory=UI"
```

#### Bash / macOS / WSL
```bash
HEADLESS=0 dotnet test --filter "TestCategory=UI"
```

#### Windows CMD
```cmd
set HEADLESS=0 && dotnet test --filter "TestCategory=UI"
```

> **Tip:** Keep `HEADLESS=1` in CI (headless mode) unless running on a self-hosted agent with a desktop session.

---

## Running in Visual Studio Test Explorer

1. Build the solution.  
2. (Optional) Add a `test.runsettings` file to the root (see below).  
3. In Visual Studio:  
   **Test → Configure Run Settings → Select Solution Wide Run Settings File**  
4. Open **Test Explorer**, and filter by `TestCategory=UI` or `TestCategory=API`.  
5. When `HEADLESS=0`, a visible browser will launch for UI tests.

### Example `test.runsettings`
```xml
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <HEADLESS>0</HEADLESS>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>
```

---

## ExtentReports Integration

### Overview
ExtentReports provides a rich HTML test report visualizing:
- Features → Scenarios → Steps hierarchy
- Pass/Fail status and stack traces
- Scenario Outline argument support
- System info (environment, tester, app, timestamps)

### Implementation
File: `NASA.Automation.Tests/Hooks/ExtentReportHooks.cs`

Key behavior:
- Initialized via `[BeforeTestRun]` and flushed in `[AfterTestRun]`
- Each report is timestamped:  
  ```csharp
  var reportName = $"NASA_Automation_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";
  ```
- Stored under:
  ```
  src/NASA.Automation.Tests/bin/Debug/net9.0/ExtentReports/
  ```

### Local Report Access
After running tests locally, open the report in your browser:
```bash
start src/NASA.Automation.Tests/bin/Debug/net9.0/ExtentReports/NASA_Automation_Report_*.html
```

### CI Integration
In `.github/workflows/tests.yml`:
```yaml
- name: Upload Extent HTML Report
  if: always()
  uses: actions/upload-artifact@v4
  with:
    name: ExtentReports
    path: src/NASA.Automation.Tests/bin/Release/net9.0/ExtentReports
```
You can download the artifact from GitHub Actions after each run.

---

## CI/CD (GitHub Actions)

Example excerpt:

```yaml
- name: Restore dependencies
  run: dotnet restore

- name: Build project
  run: dotnet build --configuration Release --no-restore

- name: Install Playwright browsers
  run: |
    dotnet tool install --global Microsoft.Playwright.CLI
    playwright install --with-deps

- name: Run SpecFlow UI tests (headless)
  env:
    HEADLESS: true
  run: dotnet test --configuration Release --filter "TestCategory=ui" --no-build
```

---

## Improvements & Considerations

### Simplifying API Test Architecture
While SpecFlow is excellent for BDD-style collaboration, the NASA public API endpoints (CME/FLR) are deterministic and input/output driven.  
They could be implemented more cleanly using **NUnit** or **xUnit**, reducing step overhead.

Hybrid approach:
- **SpecFlow** → UI workflows  
- **NUnit** → API functional validation

### Leveraging AI Copilot/Cursor & Playwright MCP
AI tools like GitHub Copilot and Playwright MCP can:
- Auto-generate PageObject models
- Heal broken locators
- Create new Gherkin scenarios from API specs
- Maintain locator stability automatically

---

**Author:** Yinka Merit  
**Tech Stack:** .NET 9 · SpecFlow · RestSharp · Playwright for .NET · ExtentReports · GitHub Actions
