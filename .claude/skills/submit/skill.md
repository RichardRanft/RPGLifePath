---
name: submit
description: Runs the build-verify-checkout-submit loop for Perforce changes — clean build, full test suite, checkout only changed files, group into one logically-scoped changelist, submit with a descriptive message.
---

1. Run a clean build and the full test suite. Stop and report if either fails — do not proceed
   to checkout or submit.
2. `p4 edit` only the files that actually changed; do not check out files outside the scope of
   this task.
3. Group related changes into one changelist. Never mix feature code, docs, and config changes
   in the same changelist — split into separate changelists by logical concern.
4. Preserve each changed file's existing line-ending style; verify no unintended CRLF/LF
   conversion crept into the diff before submitting. For any Lua file touched, verify it is
   UTF-8 without a BOM (a BOM breaks the Lua interpreter) and strip one if present.
5. Ask the user if there's a Jira issue to reference; if given, include it in the changelist
   description, but don't block the submit if there isn't one.
6. Show `p4 describe -S <cl>` for each changelist and STOP for explicit user approval before
   submitting. Never run `p4 submit` without that go-ahead.
7. Submit each approved changelist with a descriptive message summarizing the change and its
   motivation.
8. Report the changelist number(s) and confirm build/test status explicitly.
