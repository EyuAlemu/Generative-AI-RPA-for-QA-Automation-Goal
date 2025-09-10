# Generative AI + RPA for QA Automation (POM Framework)

A mini pipeline that:
1) **Part 1 (GenAI):** Generates structured test cases from user stories using an LLM.  
2) **Part 2 (RPA/Selenium + POM):** Executes a login-flow test via Page Object Model (NUnit).  
3) **Integration:** The LLM output (`testcases.json`) feeds the Selenium POM tests.

---

## Requirements & How This Project Satisfies Them

### Part 1: Test Case Generation with GenAI
**Requirement**
- **Input:** One or more user stories in plain text/JSON.  
- **Output:** Structured test cases in **Gherkin** or **tabular** (JSON/CSV with Steps, Expected Result, Pass/Fail).  
- **Constraints:** Must use an **LLM** (OpenAI/Anthropic/etc.) and include **basic validation** (e.g., at least one expected result).

**This project**
- **Input:** `input/user_stories.json` (add multiple stories if needed).  
- **LLM:** `genai/generate_testcases.py` calls an LLM (OpenAI by default) via your `OPENAI_API_KEY`.  
- **Output:** Strict **JSON** at `out/testcases.json` with both:
  - `gherkin` for each test case, and
  - a **tabular-friendly** structure: `steps[]`, `expected_results[]` (CSV-ready).  
- **Validation:** The generator enforces:
  - Non-empty `expected_results` per test case.  
  - `steps[].action` ∈ `{navigate, type, click, assert_contains}`.

> Swap models/providers by changing endpoint & payload in `genai/generate_testcases.py`.

### Part 2: Test Case Execution with RPA
**Requirement**
- Choose one generated test case and automate execution using **UiPath**, **Power Automate Desktop**, or **Selenium/Playwright/Puppeteer**.  
- Use a **demo web app** and capture **Pass/Fail** in a log/report.

**This project**
- Implements **Selenium (C#) + NUnit** with **Page Object Model**.  
- Demo AUT: **https://www.saucedemo.com**  
  - Common demo creds: `standard_user` / `secret_sauce`  
- Produces:
  - **`Reports/execution_report.json`** (status, message, timestamps, screenshot path)  
  - **Screenshot** in `Reports/`

> Prefer UiPath or Power Automate Desktop? Reuse `out/testcases.json` and implement equivalent steps (`navigate`, `type`, `click`, `assert_contains`) in your tool.

### Part 3: Integration
**Requirement**
- Demonstrate how Part 1’s output **feeds** Part 2 (can be staged, not fully automated).

**This project**
```
[input/user_stories.json] → (LLM prompt) → [out/testcases.json]
                                  │
                (copy/point tests to this file)
                                  ↓
           [qa-pom/Resources/testcases.json] → dotnet test → Reports/
```

---

## What’s Inside

### Part 1 – GenAI
- `input/user_stories.json` — sample story  
- `genai/prompt.txt` — guardrailed prompt  
- `genai/generate_testcases.py` — LLM call + validation  
- `requirements.txt` — Python dep (`requests`)  
- **Output:** `out/testcases.json`

### Part 2 – Selenium POM (NUnit)
- `qa-pom/Core` — `DriverFactory`, `Waits`, `Reporter`  
- `qa-pom/Pages` — `BasePage`, `LoginPage`, `SecureAreaPage`  
- `qa-pom/Models` — schema classes for `testcases.json`  
- `qa-pom/Resources/testcases.json` — ready-to-run sample  
- `qa-pom/Tests/LoginTests.cs` — executes the smoke test  
- `qa-pom/qa-pom.csproj` — Selenium 4, NUnit, WaitHelpers, .NET 8

### Scripts
- `scripts/generate.(sh|bat)` — run the LLM generator  
- `scripts/test.(sh|bat)` — run the NUnit tests

---

## Quick Start

### Prerequisites
- Python 3.10+  
- .NET SDK 8.x  
- Google Chrome (Selenium Manager auto-fetches the driver)  
- **OpenAI API key** if you want to regenerate test cases

### Run It

#### Optional: regenerate test cases via LLM
```bash
cd qa-genai-rpa-pom
export OPENAI_API_KEY=sk-...   # PowerShell: $Env:OPENAI_API_KEY="sk-..."
pip install -r requirements.txt
python genai/generate_testcases.py   # -> out/testcases.json

# (Optionally copy to the NUnit project resources)
# macOS/Linux:
cp out/testcases.json qa-pom/Resources/testcases.json
# Windows:
copy out\testcases.json qa-pom\Resources\testcases.json
```

#### Execute Selenium POM tests
```bash
cd qa-genai-rpa-pom/qa-pom
dotnet test
```
Results:
- `Reports/execution_report.json`
- Screenshot in `Reports/`

---

## Demo AUT: Sauce Demo – Selectors & Expectations

- **URL:** `https://www.saucedemo.com`  
- **Selectors (typical):**
  - Username: `#user-name`
  - Password: `#password`
  - Login button: `#login-button`
  - Success indicator: URL contains `/inventory.html` or `span.title` text equals **Products**

**Sample test case (snippet)**
```json
{
  "test_suite_id": "TS-2025-09-10-001",
  "source_story_id": "US-001",
  "app_under_test": "https://www.saucedemo.com",
  "test_cases": [
    {
      "id": "TC-001",
      "title": "Login with valid credentials",
      "priority": "P1",
      "tags": ["login","smoke"],
      "gherkin": "Scenario: Login with valid credentials\n  Given I am on the Sauce Demo login page\n  When I enter a valid username and password\n  Then I should land on the inventory page and see Products",
      "steps": [
        {"action":"navigate","target":"https://www.saucedemo.com"},
        {"action":"type","target":"#user-name","value":"${username}"},
        {"action":"type","target":"#password","value":"${password}"},
        {"action":"click","target":"#login-button"},
        {"action":"assert_contains","target":"css=span.title","value":"Products"}
      ],
      "expected_results": ["URL contains /inventory.html","Title shows Products"]
    }
  ]
}
```

---

## LLM, Validation & Guardrails

- **LLM:** OpenAI by default (`OPENAI_MODEL=gpt-4o-mini`). Change endpoint/model in `genai/generate_testcases.py`.  
- **Key:** Provide `OPENAI_API_KEY` via environment variable (not committed).  
- **Validation:** Generator rejects empty `expected_results` and unsupported actions.  
- **Determinism:** `temperature=0.2`; strict JSON schema in the prompt.

---

## Switching Tools or Targets

- **UiPath / Power Automate Desktop / Playwright / Puppeteer:**  
  Use `out/testcases.json` as the input map; implement the same step primitives, keep report shape.
- **Switch AUT:**  
  Update `app_under_test`, selectors in `testcases.json`, and page locators in `qa-pom/Pages/*`.

---


## Project Layout
```
input/                   # user stories (input to LLM)
genai/                   # LLM prompt + generator script
out/                     # generated test cases (by script)
qa-pom/                  # Selenium POM NUnit project
  Core/                  # driver, waits, reporting
  Models/                # schema for testcases.json
  Pages/                 # page objects
  Resources/             # testcases.json used by tests
  Tests/                 # NUnit tests
scripts/                 # helper scripts
```

---

## Security & Tips
- Never commit API keys. Use env vars or CI secrets.
- For CI (GitHub Actions / Azure DevOps), set `OPENAI_API_KEY` as a secret and expose via `env:`.


