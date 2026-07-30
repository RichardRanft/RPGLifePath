---
name: deploy
description: Deploys a build to TEST then PROD with log/config-safe robocopy, pre-flight backup, and post-deploy verification. Use for any production deployment step.
---

1. Confirm the build is clean (build + full test suite) before touching any deploy path.
2. Identify the deploy command. If it uses robocopy/xcopy, confirm the exclusion list covers
   logs/, config overrides, and any caches — never mirror over them. Show the exact command
   (including exclusions) before running it.
3. Run a dry-run/what-if pass first if the tool supports one (e.g. robocopy `/L`), report the
   file-level diff it would make, and wait for confirmation before the real run.
4. Deploy to the TEST instance/tables first. Verify there (tail the log, query the DB, or hit
   the endpoint) and show the evidence — state explicitly which environment (TEST) it came from.
5. Only after TEST verification passes, ask for explicit confirmation before promoting to PROD.
6. After the PROD deploy, verify again against PROD (not by re-showing TEST evidence) and report
   what was checked and what it returned.
7. If any step is destructive, irreversible, or would overwrite existing production state, stop
   and ask rather than proceeding.
