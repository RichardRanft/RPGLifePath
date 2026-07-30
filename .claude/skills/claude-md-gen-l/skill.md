---
name: claude-md-gen-l
description: Generates a lean root CLAUDE.md index plus nested per-subsystem CLAUDE.md files for large multi-project codebases/monorepos, so subsystem detail lazy-loads only when Claude works in that subsystem. Use instead of claude-md-gen when the repo has multiple distinct projects/packages.
---

1. Read the existing root CLAUDE.md and any existing nested CLAUDE.md files first.
2. Detect subsystem boundaries by scanning for project/package markers (`.csproj`/`.sln`,
   `pyproject.toml`/`setup.py`, `package.json`, `go.mod`, or a clearly separate top-level `src/*`
   module) — do not assume boundaries from directory names alone.
3. Apply a threshold: only give a candidate subsystem its own nested CLAUDE.md if it's
   non-trivial (its own build target, or more than a handful of source files). Trivial ones get
   a one-line entry in the root index instead of a separate file.
4. For each qualifying subsystem, explore its actual source (Read/Grep/Glob) — do not infer its
   architecture from naming or from sibling subsystems — and write/update
   `<subsystem-dir>/CLAUDE.md` with: purpose, architecture, its own build/test commands, and
   known gotchas. Keep each one concise; subsystem-specific detail belongs only here.
5. Never use `@path` import syntax in any of these files. `@import` loads eagerly at session
   start regardless of relevance — nested CLAUDE.md must stay plain so it only loads when Claude
   reads a file inside that subsystem's directory.
6. Write/update the root CLAUDE.md as a lean index: one line per subsystem (name, one-sentence
   purpose, path), plus only conventions that are genuinely global (cross-subsystem build
   orchestration, VCS/deployment rules, communication style). Do not restate subsystem detail
   here.
7. Run a scan for plaintext credentials, hardcoded secrets, or unescaped SQL across the whole
   tree and flag findings under a "Known Issues" heading in the root file, tagged with which
   subsystem each came from.
8. Report a short summary: which subsystems got their own CLAUDE.md, which were folded into the
   root index, and what changed. Do not print full file contents in chat.
