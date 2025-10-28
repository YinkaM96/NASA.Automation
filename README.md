# NASA.Automation — Test Suite

Automated test suite for NASA public APIs and the NASA Open APIs sign-up UI.  
Built with **SpecFlow**, **RestSharp**, and **Playwright for .NET 9**.

This project demonstrates:
- API automation with SpecFlow (`@api` tagged scenarios)
- UI automation using Playwright (`@ui` tagged scenarios)
- CI/CD integration via GitHub Actions (headless UI tests)

---

## Project structure & approach

| Layer | Description |
|--------|-------------|
| `NASA.Automation.Core` | Shared utilities (e.g., config, constants) |
| `NASA.Automation.API` | REST clients using **RestSharp** |
| `NASA.Automation.UI` | Page Objects and Playwright setup |
| `NASA.Automation.Tests` | SpecFlow features, steps, and hooks |

### Tags and test filters
- `@api` → API tests → `dotnet test --filter "TestCategory=api"`
- `@ui` → UI tests → `dotnet test --filter "TestCategory=ui"`

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

Run the following commands once in your project root:

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
dotnet test --filter "TestCategory=api"
```

### Run only UI tests (headless by default)
```bash
dotnet test --filter "TestCategory=ui"
```

### Headed (visible) browser runs

#### PowerShell
```powershell
$env:HEADLESS = "0"
dotnet test --filter "TestCategory=ui"
```

#### Bash / macOS / WSL
```bash
HEADLESS=0 dotnet test --filter "TestCategory=ui"
```

#### Windows CMD
```cmd
set HEADLESS=0 && dotnet test --filter "TestCategory=ui"
```

> **Tip:** Keep `HEADLESS=1` in CI (headless mode) unless running on a self-hosted agent with a desktop session.

---

## Running in Visual Studio Test Explorer

1. Build the solution.  
2. (Optional) Add a `test.runsettings` file to the root (see below).  
3. In Visual Studio:  
   **Test → Configure Run Settings → Select Solution Wide Run Settings File**  
4. Open **Test Explorer**, and filter by `TestCategory=ui` or `TestCategory=api`.  
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

## CI/CD (GitHub Actions)

The GitHub Actions workflow (`.github/workflows/tests.yml`) runs UI tests in **headless mode**.

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

## Summary

| Step | Command |
|------|----------|
| Restore dependencies | `dotnet restore` |
| Build solution | `dotnet build` |
| Install Playwright CLI | `dotnet tool install --global Microsoft.Playwright.CLI` |
| Install browsers | `playwright install --with-deps` |
| Run API tests | `dotnet test --filter "TestCategory=api"` |
| Run UI tests (headless) | `dotnet test --filter "TestCategory=ui"` |
| Run UI tests (headed) | `HEADLESS=0 dotnet test --filter "TestCategory=ui"` |

---

## Notes

- Always build the project **before installing browsers**, since the Playwright binaries are generated in the `bin` directory.
- On CI, Playwright automatically runs in headless mode.
- If you update Playwright NuGet packages, re-run `playwright install`.

---

## Troubleshooting

**Error:**
```
Executable doesn't exist at .../headless_shell
Please run: pwsh bin/Debug/netX/playwright.ps1 install
```

**Fix:**
Re-install browsers for .NET Playwright:
```powershell
playwright install --with-deps
```

or rebuild the project and run:
```powershell
dotnet build
pwsh src/NASA.Automation.Tests/bin/Debug/net9.0/playwright.ps1 install
```

---

 **Author:** Yinka Merit  
 **Tech Stack:** .NET 9 · SpecFlow · RestSharp · Playwright for .NET · GitHub Actions
