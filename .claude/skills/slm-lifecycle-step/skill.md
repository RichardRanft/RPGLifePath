---
name: slm-lifecycle-step
description: Runs one step of the SLM model lifecycle (corpus relabeling, retraining, benchmarking, or production deployment) with explicit confirmation gates for destructive or external actions.
---

1. Identify which lifecycle step is being requested: relabel, retrain, benchmark, or deploy.
2. For relabel: show a diff summary of corpus changes and wait for confirmation before applying.
3. For retrain: run the CLI/Bash training command and report completion status.
4. For benchmark: compare new results against the current production model's metrics in a
   side-by-side table. Refuse to proceed if accuracy is below 94%. Explicitly check for the
   known recurring bug classes: AM/PM case-sensitivity in timestamp handling, and
   test-vs-live filter-field mismatches (e.g. `name` vs `displayName`).
5. For deploy: only proceed if the new model measurably outperforms production and clears the
   94% accuracy bar, and require explicit user confirmation before any production SQL action.
6. Append a dated entry to the model-lifecycle history log after any step that changes state.