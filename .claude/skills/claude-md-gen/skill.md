---
name: claude-md-gen
description: Generates or updates a single CLAUDE.md file for a project by exploring its actual source, documenting architecture and build commands concisely, and flagging security issues like plaintext credentials. For a large multi-project codebase/monorepo, use claude-md-gen-l instead.
---

1. Read the existing CLAUDE.md if one exists.
2. Explore the solution's actual source (Read/Grep/Glob) to confirm architecture, build commands,
   and conventions — do not rely on assumptions from similar past projects.
3. Write or update CLAUDE.md concisely — no over-explaining.
4. Run a quick scan for plaintext credentials, hardcoded secrets, or unescaped SQL and flag
   any findings at the top of the doc under a "Known Issues" heading.
5. Report a short summary of what changed; do not print the full doc contents in chat.