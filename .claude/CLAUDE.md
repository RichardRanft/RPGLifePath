## Diagnosis Policy
Control-flow correctness (loop exits, state transitions, scheduling logic) is treated as a
hard gate. Before proposing or making any fix, invoke the `root-cause-analyst` subagent to
establish the root cause with evidence.

## Scope
Work only within the project or file named in the request. Confirm the file list before editing.
