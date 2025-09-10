import json, os, datetime, sys, re
import requests

MODEL = os.getenv("OPENAI_MODEL", "gpt-4o-mini")
API_KEY = os.getenv("OPENAI_API_KEY")
API_URL = os.getenv("OPENAI_API_URL", "https://api.openai.com/v1/chat/completions")

def chat(prompt: str) -> str:
    if not API_KEY:
        print("Missing OPENAI_API_KEY", file=sys.stderr)
        sys.exit(1)
    headers = {"Authorization": f"Bearer {API_KEY}", "Content-Type": "application/json"}
    payload = {
        "model": MODEL,
        "temperature": 0.2,
        "messages": [
            {"role": "system", "content": "You produce STRICT JSON with the requested schema. No commentary."},
            {"role": "user", "content": prompt}
        ]
    }
    resp = requests.post(API_URL, headers=headers, json=payload, timeout=60)
    resp.raise_for_status()
    content = resp.json()["choices"][0]["message"]["content"]
    content = re.sub(r"^```json|```$", "", content.strip(), flags=re.MULTILINE)
    content = content.strip("` \n")
    return content

def validate(suite: dict):
    assert "test_cases" in suite and isinstance(suite["test_cases"], list) and suite["test_cases"], "No test cases found"
    for tc in suite["test_cases"]:
        assert tc.get("expected_results"), f"{tc.get('id','<no-id>')} missing expected_results"
        for step in tc.get("steps", []):
            assert step["action"] in {"navigate","type","click","assert_contains"}, f"Invalid action {step['action']}"

def main():
    with open("genai/prompt.txt", "r", encoding="utf-8") as f:
        base_prompt = f.read()
    with open("input/user_stories.json", "r", encoding="utf-8") as f:
        stories = json.load(f)

    today = datetime.date.today().isoformat()
    final = None
    for us in stories["user_stories"]:
        prompt = base_prompt + "\n\nUSER STORY JSON:\n" + json.dumps(us, indent=2)
        raw = chat(prompt)
        suite = json.loads(raw)
        suite.setdefault("test_suite_id", f"TS-{today}-001")
        suite.setdefault("source_story_id", us.get("id","US-UNKNOWN"))
        validate(suite)
        final = suite

    os.makedirs("out", exist_ok=True)
    with open("out/testcases.json", "w", encoding="utf-8") as f:
        json.dump(final, f, indent=2)
    print("Wrote out/testcases.json")

if __name__ == "__main__":
    main()
