# RPGLifePath Port Plan

Source: `port_intent.txt`. Four goals, tackled in order since each depends on the
previous: (1) move to .NET 10, (2) split generation logic into its own library,
(3) add a standalone weighted-table abstraction loadable from XML or JSON,
(4) evaluate the Spiker `//depot/AI/BehaviorTree` (`FluentBehaviourTree`) code as a
way to make lifepath generation more flexible.

## Current state (baseline, verified by reading the repo)

- Single WinForms project, `LifePath.csproj`, old-style MSBuild, `TargetFrameworkVersion
  v4.5.2`, `OutputType WinExe`.
- `packages.config` is empty — no NuGet dependencies to migrate.
- Six source files, ~1580 lines total: `Program.cs`, `Form1.cs` (+Designer/resx) are UI;
  `CActor.cs`, `CLifePath.cs`, `CLifePathGenerator.cs`, `CNameGenerator.cs` are the
  generation logic.
- Generation logic already has one leaked UI dependency: `CNameGenerator.loadNames()`
  calls `MessageBox.Show` on error (`CNameGenerator.cs:120`) and references
  `System.Windows.Forms` at the top of the file even though the class does no UI work
  otherwise.
- Data-driven via `System.Data.DataSet`/`DataTable` loaded from `Tables\pathdata.xml`
  (roll tables) plus two CSV name files and a few `.txt` tables. `CLifePathGenerator`
  is a fixed, hardcoded call chain (`getParentStatus` → `getFamilySituation` →
  `getFriendsAndEnemies` → `getRomanticLife`/`getExStatus`), each step reading a
  specific named `DataTable` and mutating a `CLifePath` passed by `ref`. Adding,
  reordering, or branching a life stage today means editing this code.
- All file I/O (`CActor.Save`, `CLifePath.Save`, table loading) uses paths relative to
  the process working directory (`"Tables\\..."`), not the assembly location.

## Depot assessment: `spiker:1666//depot/AI/...`

Connected via `p4 -p spiker:1666` and inspected `//depot/AI/*`:

- `//depot/AI/BehaviorTree` — **relevant, recommended**. `FluentBehaviourTree`
  namespace, ~12 loose `.cs` files, no `.csproj` (vendor as source). Five node types
  (Action, Sequence, Selector, Inverter, Parallel), a fluent `BehaviourTreeBuilder`,
  and a `BehaviourTreeJsonLoader` that builds a tree from JSON via `Newtonsoft.Json`
  (only external dependency). Files use modern nullable-reference syntax (`string?`,
  `= null!`), so it already assumes a current C# compiler — no language-version work
  needed to bring it into a .NET 10 project. Full API confirmed via
  `DEVELOPER_GUIDE.md` (fetched in full) and the four core interface/type files
  (`IBehaviourTreeNode.cs`, `IParentBehaviourTreeNode.cs`, `BehaviourTreeStatus.cs`,
  `TimeData.cs`, `BehaviourTreeJsonLoader.cs`, `BehaviourTreeNodeJson.cs`).
- `//depot/AI/StateMachine` — simpler alternative (2 source files: `StateMachineDefinition.cs`,
  `StateMachineRunner.cs`). Not pursued: the intent explicitly names BehaviorTree, and
  lifepath generation is a linear-with-branches pipeline, which Sequence/Selector maps
  to more directly than a state machine's transition graph.
- `//depot/AI/CSGOAP`, `//depot/AI/NeuralNetwork`, `//depot/AI/SLM` — listed but not
  relevant (goal-oriented planning, log-analysis neural net, small-language-model
  tooling — no fit for a table-roll backstory generator). Not investigated further.

## Phase 1 — .NET 10 migration (complete)

Goal: same app, same behavior, running on .NET 10 with an SDK-style project, opened
via `LifePath.slnx` in Visual Studio 2026.

Done: `LifePath.csproj` converted to SDK-style (`net10.0-windows`, `UseWindowsForms`),
`packages.config`/`AssemblyInfo.cs` deleted, `LifePath.sln` replaced by `LifePath.slnx`,
`.claude/settings.json`'s build hook repointed at `LifePath.slnx`/VS2026 MSBuild.
`dotnet build` succeeds; app runs and generates lifepaths (verified while fixing the
`m_namegen` null-reference regression from commit f53c599).

1. Convert `LifePath\LifePath.csproj` to SDK-style:
   `<Project Sdk="Microsoft.NET.Sdk">`, `<TargetFramework>net10.0-windows</TargetFramework>`,
   `<UseWindowsForms>true</UseWindowsForms>`, `<OutputType>WinExe</OutputType>`.
   SDK-style glob-includes `.cs`/`.resx` by default, so the long explicit `<Compile>`/
   `<EmbeddedResource>` item list collapses to just the special cases (Tables content
   files with `CopyToOutputDirectory`, and `Properties\Settings.settings` if kept).
2. Drop `packages.config` (empty, no packages) and `App.config`'s binding-redirect
   boilerplate (`AutoGenerateBindingRedirects` isn't needed without NuGet packages).
3. Convert `LifePath.sln` → `LifePath.slnx` and target Visual Studio 2026, so
   double-clicking in Explorer opens the right IDE. Verified on this machine before
   writing this step:
   - Both VS2022 (17.14.37516.0, `...\Microsoft Visual Studio\2022\Community`) and
     VS2026 (18.8.12023.21, `...\Microsoft Visual Studio\18\Community` — confirmed via
     `vswhere -all` as `displayName: "Visual Studio Community 2026"`) are installed
     side by side. `vswhere -latest` resolves to the 2026 install, and the `.NET 10`
     SDK (`10.0.302`) needed for Phase 1's `net10.0-windows` TFM is already present.
   - `.sln` and `.slnx` are **both** registered to the same shared
     `VSLauncher.exe` (confirmed via the `HKEY_CLASSES_ROOT\.sln`/`.slnx` ProgID →
     `shell\open\command` registry keys) — there is no per-extension "this file type
     opens devenv X" association to change. VSLauncher picks an installed version at
     launch time (newest-compatible by default), so renaming the file alone doesn't
     *guarantee* 2026 opens; it's the likely-but-not-certain outcome once the file no
     longer carries the old `# Visual Studio 14` / `VisualStudioVersion = 14.0...`
     header (`LifePath.sln`'s current first lines) that can pin an upgrade prompt to
     an older toolset. Also, VS2022 17.14 already understands `.slnx` too, so format
     alone isn't the deciding factor.
   - Steps: run `dotnet solution LifePath.sln migrate` (confirmed available in the
     installed .NET 10 SDK — command is `dotnet solution <file> migrate`, not
     `dotnet sln migrate`) to generate `LifePath.slnx`; `git mv`/delete the old `.sln`;
     update any reference to `LifePath.sln` (README, scripts, this plan) to
     `LifePath.slnx`. `.slnx` is a flat XML project list with no VS-version header, so
     the stale "VS14" pin goes away with the conversion.
   - To make the IDE choice deterministic rather than "probably picks newest
     installed," open `LifePath.slnx` once via VS2026's `devenv.exe` directly
     (`"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe" LifePath.slnx`)
     as part of this phase's manual verification pass — this is a one-time, per-machine
     Explorer/VSLauncher affinity action, not something encoded in the committed
     `.slnx`/`.csproj` files, so it isn't part of the diff and needs to be re-done on
     any other dev machine that has both versions installed.
   - **Don't forget**: `.claude/settings.json`'s `PostToolUse` build hook currently runs
     `MSBuild.exe LifePath.sln` against the VS2022 path. It will start failing the
     moment `LifePath.sln` is deleted. Update it in the same step to build
     `LifePath.slnx` (confirm `MSBuild.exe` on this SDK/VS version actually builds
     `.slnx` directly — fall back to `dotnet build LifePath.slnx` if not) — and point
     it at the VS2026 MSBuild path
     (`...\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe`) to
     match the retarget.
4. `Properties\AssemblyInfo.cs` conflicts with SDK auto-generated assembly info by
   default — either delete it and move the handful of attributes into
   `<PropertyGroup>` in the csproj, or set `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>`
   to keep the file as-is. Prefer deleting it (less to maintain).
5. Build with `dotnet build`, run the app, and manually exercise: load, generate,
   each reroll button, save. This project has no test suite, so this manual pass is
   the verification gate for this phase.
6. Delete `obj\`/`bin\` old-style build output (`LifePath\obj\Debug\...` currently
   tracked in the working tree per the file listing) so it doesn't collide with the
   SDK-style build layout.

Risk: none of the current code has been checked for TFM-specific behavior changes
(e.g. `DataSet.ReadXml` schema inference) — Phase 1 build+manual-run is the check.

## Phase 2 — isolate generation logic into a library (complete)

Goal: `CActor`, `CLifePath`, `CLifePathGenerator`, `CNameGenerator` move to a new
class library that the WinForms app references; the app project keeps only
`Program.cs`, `Form1.cs`/Designer/resx.

Done: added `LifePath.Core\LifePath.Core.csproj` (SDK-style, `net10.0`, no WinForms),
moved the four class files in via `git mv`, renamed their namespace to `LifePath.Core`
and made the classes `public` (required once they cross an assembly boundary — the
original `internal` default made them invisible to `LifePath.csproj`, caught by the
first build attempt). Replaced `CNameGenerator`'s `MessageBox.Show`-on-load-failure
with a thrown `InvalidOperationException`, dropping the `System.Windows.Forms`
reference from the library. `LifePath.csproj` got a `<ProjectReference>` to
`LifePath.Core.csproj`; `Form1.cs` got a `using LifePath.Core;`. `LifePath.slnx`
lists both projects. `dotnet build LifePath.slnx` succeeds; `LifePath.exe` launches
and stays running with `Tables\pathdata.xml` loaded (process-liveness check — no
Windows GUI automation tool is available in this environment, so the interactive
generate/reroll/save pass from Phase 1 step 5 still needs a manual click-through by
the user).

1. Add `LifePath.Core\LifePath.Core.csproj` (SDK-style, `net10.0`, no WinForms
   dependency) to the solution.
2. Move the four class files into it and rename the namespace to `LifePath.Core`
   (decided — matches the project name, one rename pass done up front rather than
   left inconsistent for later consumers).
3. Remove the leaked UI dependency: `CNameGenerator.cs` currently does
   `using System.Windows.Forms;` and calls `MessageBox.Show` on a load failure
   (line 120). Replace with a thrown exception (the existing `catch` already has
   `ex.Message`/`ex.InnerException` assembled into `msg` — wrap and rethrow, or let
   it propagate) and let `Form1` decide how to surface it to the user. This is the
   only non-mechanical change in this phase.
4. Table/name-file loading currently hardcodes `"Tables\\..."` relative to the
   working directory (`CNameGenerator.loadNames`, `Form1.Form1_Load`). Keep the
   relative-path contract as-is for this phase (don't scope-creep into a path-
   injection redesign) — just confirm the Core library doesn't assume a working
   directory different from the app's (it doesn't; all `File.Exists`/`StreamReader`
   calls stay relative, unchanged).
5. `LifePath.csproj` adds a `<ProjectReference>` to `LifePath.Core.csproj`; `Tables\`
   content files stay with the WinForms app project (they're runtime data next to
   the .exe, not library code).
6. Rebuild, rerun the same manual pass as Phase 1 step 5 to confirm no behavior
   changed from the split.

## Phase 3 — weighted-table abstraction (XML + JSON) (complete)

Goal: pull the roll-table lookup that's currently inlined in `CLifePathGenerator`
(`getResult(DataTable)` / `getRange(DataTable)`, `CLifePathGenerator.cs:278-350`) out
into a standalone, reusable type that can be loaded from either the existing XML
format or an equivalent JSON format.

**Verified structure of `Tables\pathdata.xml`** (read directly — the file is 5.3MB /
283,419 lines, too large for a single read, so inspected in slices and via targeted
`grep`):

- Root element `<LifePath>` contains flat, repeated per-row elements — no nesting,
  no `xs:schema` block; `DataSet.ReadXml` is inferring the schema from repetition
  (element name → table name, child elements → columns).
- Two element kinds:
  - **Name lookups** (not weighted): `<First_Names><name>Aaron</name></First_Names>`
    and `<Last_Names><name>...</name></Last_Names>`, one element per name, no
    `rlow`/`rhigh`/`result`. **5,493** `First_Names` rows and **88,799** `Last_Names`
    rows — this pair accounts for the overwhelming majority of the file's size and
    line count. These already have a second representation too
    (`Tables\CSV_Database_of_First_Names.csv` / `..._Last_Names.csv`, read directly
    by `CNameGenerator` when no `DataSet` is supplied) — pre-existing duplication,
    out of scope here, noted for awareness only.
  - **Weighted roll tables** (the ones in scope): rows shaped
    `<TableName><rlow>N</rlow><rhigh>M</rhigh><result>text</result></TableName>`.
    `rhigh` is present-but-empty (`<rhigh />`) for a single-value row (matches
    `getResult`'s `String.IsNullOrEmpty(h)` branch, e.g. the `FamilyMisfortune` rows
    for roll 9 and 10). `result` of literal `#` is a sentinel meaning "use the roll
    number itself as the result" (e.g. `Siblings` rows 1-7 → `#`, so a roll of 4
    means "4 siblings").
- Confirmed all **18** weighted tables referenced in `CLifePathGenerator.cs` are
  present and row counts are small — this is a tiny dataset, not a scaling concern:
  `Parents`(2), `BothLiving`(3), `Other`(10), `FamilyStanding`(2), `Siblings`(2),
  `SiblingRel`(5), `FamilyMisfortune`(6), `LifeGoal`(5), `Friends`(10), `Enemies`(8),
  `EnemyOrigin`(10), `EnemyStatus`(3), `EnemyReaction`(5), `Romance`(3),
  `RelationshipStatus`(9), `SingleStatus`(5), `ReboundStatus`(10), `ExStatus`(10) —
  108 rows total across all 18 tables (sum of the per-table counts above; also
  confirmed independently via `grep -c "<rlow>"` against `pathdata.xml`).

**Design:**

1. New type in a `LifePath.Core.Tables` sub-namespace, e.g. `WeightedTable`: an
   ordered list of `(int Low, int High, string Result)` entries plus the two
   behaviors currently duplicated inline as `getResult`/`getRange` —
   `Roll(Random rand)` (pick a value in the table's full range, find the matching
   row, resolve `#` to the roll number) and the range computation itself (min of all
   `Low`, max of all `High`) computed once at load time instead of rescanned per roll.
2. `WeightedTableXmlLoader` — parses the existing `pathdata.xml` shape directly via
   `System.Xml.Linq` (`XDocument`/`XElement`), grouping repeated same-named elements
   into one `WeightedTable` per distinct tag, skipping `First_Names`/`Last_Names`
   (handled separately, unchanged). This is a **new, narrower parser**, not a reuse
   of `DataSet.ReadXml` — deliberately, so the weighted-table path stops depending on
   `System.Data.DataSet`'s schema-inference behavior for this data. `CNameGenerator`
   keeps using `DataSet`/CSV for name tables, unchanged (decided — dropping
   `System.Data.DataSet` from the project entirely is a separate, larger decision not
   bundled into this phase).
3. `WeightedTableJsonLoader` — new format, a single combined JSON document mirroring
   the same grouping (decided — matches the single-`pathdata.xml`-file model, one
   load call, easiest to diff against the XML for equivalence checks), e.g.:
   ```json
   {
     "FamilyStanding": [
       { "Low": 1, "High": 6, "Result": "@Siblings" },
       { "Low": 7, "High": 10, "Result": "@FamilyMisfortune" }
     ],
     "Siblings": [
       { "Low": 1, "High": 7, "Result": "#" },
       { "Low": 8, "High": 10, "Result": "0" }
     ]
   }
   ```
   `High` omitted/null for single-value rows instead of XML's empty-element
   convention. Only the 18 weighted tables belong in this file — name lists stay out
   (they're not weighted-roll data, and 94K name rows don't belong in a hand-editable
   JSON config). Adds the `Newtonsoft.Json` package reference to `LifePath.Core` —
   this is the first phase that needs it, since Phase 4's JSON loader is deferred
   (Decision 5).
4. Refactor `CLifePathGenerator` to hold a `Dictionary<string, WeightedTable>`
   (built by whichever loader ran) instead of reaching into `m_pathData.Tables["X"]`
   directly; every call site (`getResult(m_pathData.Tables["Parents"])` etc.)
   becomes `m_tables["Parents"].Roll(m_rand)`. Mechanical, one-to-one replacement —
   no behavior change.
5. Verify: for every one of the 18 tables, load via both loaders from equivalent
   XML/JSON content and assert identical `(Low, High, Result)` rows and identical
   computed range; then roll each table many times with a seeded `Random` before and
   after the `CLifePathGenerator` refactor and diff results, same equivalence-check
   approach as Phase 4 step 3.

This phase is independent of Phase 4's tree work but feeds it: once life stages are
`ActionNode`s, each one's body is exactly "roll one or more `WeightedTable`s and
apply the result to the context" — a small, mechanical action instead of the current
30-40 line per-stage methods.

**Done, with empirical validation (throwaway spike, not committed — see below):**

- `LifePath.Core.Tables.WeightedTable` holds an ordered `IReadOnlyList<WeightedTableRow>`
  (`Low`, nullable `High`, `Result`) plus `RangeLow`/`RangeHigh` computed once in the
  constructor and a `Roll(Random)` method. The range computation deliberately
  reproduces the original `getRange` quirk rather than "fixing" it: `RangeLow` is the
  min of *all* rows' `Low` (since `rlow` is always present), but `RangeHigh` is the max
  of only the rows that *have* an explicit `rhigh` — rows with `rhigh` empty (single-value
  rows) don't extend the upper bound. Confirmed via `FamilyMisfortune` in the real data:
  rows for 9 and 10 have empty `rhigh`, rows 1-8 have explicit `rhigh` maxing at 8, so
  `RangeHigh` is 8 and rolls of 9/10 are structurally unreachable — a pre-existing dead
  branch in the original code, now preserved exactly rather than silently fixed.
- `WeightedTableXmlLoader` groups `pathdata.xml`'s root-level elements by tag name via
  `XDocument`/`XElement`, treating a group as a weighted table only if its elements have
  an `<rlow>` child (this is what naturally excludes `First_Names`/`Last_Names`, which
  have a `<name>` child instead — no hardcoded table-name list needed).
- `WeightedTableJsonLoader` deserializes the single-combined-document shape via
  `Newtonsoft.Json` (added as a `LifePath.Core` package reference).
- `LifePath\Tables\pathdata.json` generated from the live `pathdata.xml` content (18
  weighted tables, matching the design shape) and committed as source — not yet wired
  into the running app as a runtime option (decision: stay on the XML path for now,
  matching the "no behavior change" scope of this phase); it exists as a real,
  loader-verified artifact rather than just a design sketch.
- `CLifePathGenerator` now takes `(Dictionary<string, WeightedTable> tables, DataSet
  nameData = null)` instead of a bare `DataSet`; every `getResult(m_pathData.Tables["X"])`
  call site became `m_tables["X"].Roll(m_rand)`. `nameData` is still a `DataSet` — passed
  straight through to build the internal `CNameGenerator`, unchanged, per Decision 2.
  `Form1.Form1_Load` now also calls `WeightedTableXmlLoader.Load("Tables\\pathdata.xml")`
  and passes the result alongside the existing `DataSet` load.
- **Validation performed** (throwaway console spike under a scratch dir, `LifePath.Core`
  referenced via `ProjectReference`, deleted after the run — not part of the repo):
  1. Loaded all 18 known tables from the real `pathdata.xml` via `WeightedTableXmlLoader`;
     confirmed `First_Names`/`Last_Names` are correctly excluded (no `<rlow>` child).
  2. Serialized those 18 tables to the JSON shape (this *is* how `pathdata.json` was
     produced) and reloaded via `WeightedTableJsonLoader`; diffed every row's
     `(Low, High, Result)` and both loaders' computed `(RangeLow, RangeHigh)` per table —
     **all 18 tables identical**.
  3. Re-implemented the original `DataSet`-based `getResult`/`getRange` verbatim (copied
     from the pre-refactor source) against the same `pathdata.xml` loaded into a
     `DataSet`, then for each of the 18 tables ran **100,000 rolls** with two `Random`
     instances seeded identically (old algorithm vs. `WeightedTable.Roll`), calling each
     exactly once per iteration so the underlying RNG sequences track in lockstep —
     **zero mismatches across all 18 tables × 100,000 iterations**.
- Rebuilt (`dotnet build LifePath.slnx` — 0 warnings, 0 errors) and manually confirmed
  `LifePath.exe` still launches and stays running with the new `WeightedTableXmlLoader`
  call in `Form1_Load` (process-liveness check, same caveat as Phase 2: no Windows GUI
  automation tool available in this environment for a full generate/reroll/save
  click-through).

### Post-Phase-3 bug fix: single-value rows never reachable in 7 of 18 tables

User-reported: generated Friends always showed relationship "Grew up with you" and
Enemies always "Enemy agent". Root-caused with the `root-cause-analyst` subagent
(project diagnosis policy) before any fix, confirmed independently by reading the full
row data for all 18 tables and cross-checking `git log`/`git blame` on
`pathdata.xml:282880-283420` — the data is unmodified since commit `7253550b` (2019),
so this predates the .NET port entirely; Phase 3 faithfully carried the bug forward
rather than introducing it.

**Root cause:** `WeightedTable`'s range computation (and the pre-port `getRange` it
replicated) only extended `RangeHigh` from rows that had an explicit `<rhigh>`. A row
with `<rhigh />` empty (a "single-value" row, meant to match when `roll == Low`) never
contributed to `RangeHigh` at all — not even its own `Low`. For a table made entirely
of single-value rows, `RangeHigh` stays at its initial value of 1 forever, so every
roll is forced to 1 and the table always returns its first row.

**Affected tables** (verified by reading every row in `pathdata.xml:282880-283420`):
`Other` (10 rows, all single-value → always row 1), `Friends` (10 rows, all
single-value → always "Grew up with you"), `Enemies` (row 1 ranged 1-3, rows 2-8
single-value → range collapsed to 1-3, always "Enemy agent"), `EnemyOrigin` (10 rows,
all single-value → always row 1), `ReboundStatus` (10 rows, all single-value → always
row 1), `ExStatus` (10 rows, all single-value → always row 1), and `FamilyMisfortune`
(rows 1-8 ranged, rows 9-10 single-value → reachable range capped at 8, rows 9/10 dead
— this is the same quirk noted and *deliberately preserved* in the Phase 3 record
above; it turned out to be a bug, not an intentional design choice, so that earlier
call was wrong and is superseded by this fix). The other 11 tables were already
correctly formed (either fully ranged, or a mix where the last row's explicit `rhigh`
happened to reach 10) and are unaffected.

**Fix:** `LifePath.Core/Tables/WeightedTable.cs` constructor — a row's contribution to
`RangeHigh` is now `row.High ?? row.Low` instead of being skipped when `High` is null.
This is a code-only fix (no `pathdata.xml`/`pathdata.json` edits — the bug was in how
the range was computed, not in the data itself), so it applies uniformly to both the
XML and JSON loading paths.

**Validation** (throwaway spike, not committed): loaded the real `pathdata.xml`,
computed both the old and new range formula for all 18 tables — 11 unchanged, 7
changed (the list above, exactly). For the 11 unchanged-range tables, ran 100,000
seeded rolls old-formula vs. new-formula side by side: **zero differences**, confirming
no regression. For the 7 changed tables, ran 200,000 rolls under each formula and
counted distinct results reached: **old formula reached only 1 distinct result** for
`Other`/`Friends`/`Enemies`/`EnemyOrigin`/`ReboundStatus`/`ExStatus` and 4 of 6 for
`FamilyMisfortune`; **new formula reached all rows' distinct results in every one of
the 7 tables**. Rebuilt (0 warnings, 0 errors) and manually confirmed `LifePath.exe`
still launches and stays running.

## Phase 4 — evaluate FluentBehaviourTree for generation flexibility (complete, all steps)

Goal: make the fixed `getX → getY → getZ` call chain in `CLifePathGenerator` into a
composable tree, so new life stages, branching, or reordering can eventually be done
via tree structure instead of editing generator code. Scoped as an *evaluation with a
working spike*, not a mandated rewrite — confirm the shape below with a spike before
committing to it (see `spike-validate` skill for the empirical-validation approach
referenced in working-style guidance). Scope for this pass is the builder-based tree
only (steps 1-3, Decision 5) — JSON-driven loading (step 4) is a deferred follow-up.

1. Vendor the BehaviorTree source from `//depot/AI/BehaviorTree/BehaviorTree/*.cs`
   into a new, standalone `LifePath.BehaviorTree` project referenced by
   `LifePath.Core` (Decision 4 — keeps the generic tree library decoupled from
   lifepath-specific code, reusable by other Spiker projects later without dragging
   lifepath logic along). No `Newtonsoft.Json` reference needed here — `LifePath.Core`
   already picked it up in Phase 3 for `WeightedTableJsonLoader`, and this phase's
   own JSON loader (step 4) is deferred anyway.
2. Model the current fixed pipeline as a `SequenceNode` tree first, as a mechanical,
   behavior-preserving refactor:
   `Sequence("Lifepath") → [ParentStatus, FamilySituation, FriendsAndEnemies, RomanticLife]`,
   each stage an `ActionNode` whose delegate is today's `getParentStatus`/
   `getFamilySituation`/etc. body, closing over a shared mutable context (today's
   `ref CLifePath path` becomes a captured local). All nodes should return `Success`
   (there's no failure/retry concept in the current logic) so tree semantics don't
   change generator output.
3. Validate equivalence: generate N lifepaths with the same seeded `Random` on both
   the old direct-call code and the new tree-based code, diff the resulting
   `CLifePath` objects field-by-field. This is the concrete spike — don't consider
   Phase 4 done on code review alone.
4. **Deferred follow-up, not this pass:** evaluate `BehaviourTreeJsonLoader` for
   making the tree *data-driven* (tree shape defined in a JSON file instead of
   hardcoded `BehaviourTreeBuilder` calls). This is where the actual flexibility
   payoff is — e.g. adding a new optional life stage, or branching family outcomes
   with a `Selector`, becomes a JSON edit — but it lands after the builder-based tree
   is proven, not alongside it.
5. Whether `Selector`/`Parallel` nodes should eventually replace any of the
   `WeightedTable.Roll` logic from Phase 3, or the tree should only ever govern
   *which stages run* leaving table-roll weighting untouched, is a question for the
   JSON-driven follow-up, not this pass — the BT library has no concept of weighted
   random rolls, so `WeightedTable` stays as-is regardless of how that's resolved.

**Done, with empirical validation (throwaway spike, not committed — see below):**

- Vendored all 12 `//depot/AI/BehaviorTree/BehaviorTree/*.cs` files (via `p4 print`,
  not a workspace sync, so no client mapping needed) into a new
  `LifePath.BehaviorTree` project (`net10.0`, `FluentBehaviourTree` namespace
  preserved as-is rather than renamed, since it's vendored third-party-style code, not
  lifepath-specific). **Correction to this phase's original plan text above:** step 1
  said no `Newtonsoft.Json` reference would be needed since the JSON loader (step 4)
  is deferred — that's wrong. `BehaviourTreeJsonLoader.cs` is vendored source that
  *compiles* as part of the project regardless of whether it's called this phase, and
  it has a hard `using Newtonsoft.Json;` — so `LifePath.BehaviorTree.csproj` needs the
  package reference too, discovered by the first build attempt (`CS0246` on
  `JsonConvert`). Also enabled `<Nullable>enable</Nullable>` for this project (the
  files use `?` nullable-reference syntax, e.g. `BehaviourTreeBuilder.cs`'s
  `IBehaviourTreeNode? curNode`) to clear `CS8632` warnings rather than leave them.
  `LifePath.Core` references `LifePath.BehaviorTree`; `LifePath.slnx` lists all three
  projects now.
- `CLifePathGenerator.Generate()` now builds and ticks
  `Sequence("Lifepath") → [ParentStatus, FamilySituation, FriendsAndEnemies, RomanticLife]`
  exactly as specified, each leaf an `ActionNode` delegate that always returns
  `BehaviourTreeStatus.Success`. The mechanical part turned out simpler than the plan
  assumed: the four leaf bodies aren't new code — they're the *existing public*
  `RollParents`/`RollFamilySituation`/`RollFriends`+`RollEnemies`/`RollRomance`
  methods (the ones the UI's individual reroll buttons already call), reused as-is.
  Each of those already resets its own state at the top (`path.Parents.Clear()`,
  `path.Siblings.Clear()`, `path.Lover = new CActor()`, etc.) as a no-op on a
  freshly-constructed `CLifePath`, and their bodies were already byte-for-byte
  identical to the old private chain methods' bodies (confirmed by direct comparison
  before writing any code) — so no new leaf-body code was needed at all. The four
  private chain methods (`getParentStatus`, `getFamilySituation`,
  `getFriendsAndEnemies`, `getRomanticLife`) became dead once `Generate()` stopped
  calling them and were deleted; `getExStatus` stays (still used by `RollRomance`).
- **Validation performed** (throwaway console spike under a scratch dir, referencing
  `LifePath.Core` and `LifePath.BehaviorTree` via `ProjectReference`, deleted after
  the run — not part of the repo): reimplemented the *old* pre-Phase-4 hardcoded call
  chain as free functions (copied from the pre-edit source) and the *new*
  tree-orchestrated version as free functions calling the vendored
  `BehaviourTreeBuilder`, both operating on the same real `pathdata.xml`-loaded
  `WeightedTable` dictionary and a shared deterministic name stub (`"N" +
  rand.Next(1_000_000)`, used in place of `CNameGenerator` — which seeds its own
  internal `Random` from `DateTime.Now.Millisecond` and so can't be made
  reproducible across two independent instances; irrelevant to what Phase 4 changes,
  since name draws consume a separate RNG stream from the table-roll `Random` in both
  the old and new code and were never touched by this refactor). Ran **20,000**
  generations, each with two `Random` instances seeded identically (old vs. new),
  dumped every field of the resulting `CLifePath` (parent/sibling/friend/enemy counts
  and each actor's name/relationship/origin/status/reaction, parent/family/romance
  status strings, lover fields) to a string and compared — **zero mismatches across
  all 20,000 generations**.
- Rebuilt (`dotnet build LifePath.slnx` — 0 warnings, 0 errors) and manually
  confirmed `LifePath.exe` still launches and stays running (same process-liveness
  caveat as prior phases — no Windows GUI automation tool available here for a full
  click-through).
### Step 4 follow-up, now done: JSON-driven tree definition

Goal: make the tree shape itself data (loadable/saveable JSON), not just a builder
call graph, so adding/reordering a life stage becomes a JSON edit — the actual
flexibility payoff step 4 was deferred for.

- New `LifePath.Core.Trees.LifePathTreeDefinition` (static class): `Default()`
  returns the canonical `BehaviourTreeNodeJson` — `Sequence("Lifepath")` with the
  same four `Action` children (`ParentStatus`, `FamilySituation`,
  `FriendsAndEnemies`, `RomanticLife`) the hardcoded builder tree had. `Save(node,
  path)` writes it via `JsonConvert.SerializeObject(node, Formatting.Indented)`;
  `Load(path)` reads it back via `JsonConvert.DeserializeObject<BehaviourTreeNodeJson>`
  — both trivial, since `BehaviourTreeNodeJson` (vendored) is already a plain mutable
  DTO with no custom serialization needed.
- `CLifePathGenerator`'s hardcoded `BuildTree()`/`BehaviourTreeBuilder` fluent chain
  is gone, **replaced** (not kept alongside) by
  `BehaviourTreeJsonLoader.LoadFromNode(treeDefinition, ResolveAction)` — the
  constructor takes an optional `BehaviourTreeNodeJson treeDefinition = null`,
  falling back to `LifePathTreeDefinition.Default()`. `ResolveAction(string)` maps
  the four action names to the same `RollParents`/`RollFamilySituation`/
  `RollFriends`+`RollEnemies`/`RollRomance` leaf bodies as before (unchanged
  method bodies — only how the tree wiring reaches them changed). This
  consolidates on a single tree-construction mechanism (JSON-node-driven, whether
  the node came from a file or the in-code default) instead of maintaining both a
  fluent-builder path and a JSON path for the same tree.
- `LifePath\Trees\lifepath.json` — new committed content file (`CopyToOutputDirectory`
  in `LifePath.csproj`), generated once via `LifePathTreeDefinition.Save`, same
  precedent as `Tables\pathdata.json` in Phase 3. `Form1.Form1_Load` loads it if
  present; if absent (e.g. a future clean checkout without the file, or a user who
  deletes it to reset), bootstraps it by saving `LifePathTreeDefinition.Default()`
  to that path first — so both load and save are real, exercised code paths, not
  just available-but-unused API surface.
- **Validation performed** (throwaway console spike, referencing `LifePath.Core`
  and `LifePath.BehaviorTree` via `ProjectReference`, deleted after the run — not
  part of the repo):
  1. Round-trip fidelity: `Save(Default())` → `Load()` structurally deep-equal to
     the original (`Type`/`Name`/`Action`/`NumRequiredToFail`/
     `NumRequiredToSucceed`/`Children` recursively) — **pass**.
  2. The actual committed `Trees\lifepath.json` loads back structurally identical
     to `LifePathTreeDefinition.Default()` — **pass**.
  3. Equivalence, same method as the Phase 4 step 1-3 spike above: reimplemented
     the *original pre-Phase-4* hardcoded call chain as free functions, and a
     *new* JSON-tree-driven version using `BehaviourTreeJsonLoader.LoadFromNode`
     against the actual committed `Trees\lifepath.json` content, both sharing the
     same real `pathdata.xml`-loaded `WeightedTable`s and a deterministic name
     stub. Ran **20,000** generations with identically-seeded `Random` instances
     (old vs. new) and diffed every field of the resulting `CLifePath` — **zero
     mismatches across all 20,000 generations**, confirming the JSON-driven tree
     is behaviorally identical to both the code it replaced in this pass and the
     original pre-Phase-4 chain.
- Rebuilt (0 warnings, 0 errors) and manually confirmed `LifePath.exe` still
  launches with the `Trees\lifepath.json`-driven tree (same process-liveness
  caveat as prior phases).

## Post-Phase-4 cleanup pass (`/simplify`)

Four review agents (reuse/simplification/efficiency/altitude) reviewed the full
Phase 2-4 diff (`966d14a...cd301a9`). All four independently converged on the same
two real findings; a third, minor one came from the simplification pass only.
Applied all three, each verified empirically before/alongside the fix:

- **`Form1.Form1_Load` parsed `pathdata.xml` twice** — once via `DataSet.ReadXml`
  (for `CNameGenerator`), once via `WeightedTableXmlLoader.Load` (for
  `WeightedTable`s), each a separate disk read + full parse of the same 5.3MB file.
  Fixed by parsing once with `XDocument.Load`, feeding it to
  `WeightedTableXmlLoader.LoadFrom(doc)` directly, and building the `DataSet` from
  the same in-memory document via `doc.CreateReader()` → `DataSet.ReadXml(XmlReader)`
  instead of re-opening the file. Verified: app still launches and loads correctly.
- **`CLifePathGenerator.Generate()` rebuilt the behavior tree from scratch on every
  call** — a new `BehaviourTreeBuilder` graph and four closures allocated per
  generation, even though the tree shape never varies. Fixed by building the tree
  once in the constructor (`BuildTree()`, stored in `m_tree`) with leaf closures
  reading a `m_currentPath` instance field instead of capturing a per-call local;
  `Generate()` now just sets `m_currentPath` and ticks the cached tree. **Not
  applied:** the simplification agent's alternative of dropping the tree and calling
  `RollParents`/etc. directly — that would undo Phase 4's actual point (driving
  generation through the behavior tree) rather than just removing waste, so it was
  skipped as changing intended behavior/architecture, not just performance. Verified
  empirically (throwaway spike, deleted after): 5,000 `Generate()` calls on one
  `CLifePathGenerator` instance, confirmed varied output (13 distinct `ParentStatus`
  values, all three `Friends`-count buckets seen) and zero empty/null core fields —
  no cross-call state bleed from reusing the cached tree.
- **`WeightedTableJsonLoader` had a private `Row` DTO** duplicating
  `WeightedTableRow`'s three fields just to satisfy JSON deserialization, then
  mapped one-to-one into the real (immutable, constructor-only) type. Removed it —
  Newtonsoft.Json can deserialize directly into a type with only a matching
  constructor (case-insensitive parameter-name binding against JSON properties),
  so `JsonConvert.DeserializeObject<Dictionary<string, List<WeightedTableRow>>>`
  works without a mutable shim. Verified empirically (throwaway spike, deleted
  after) rather than trusted on inspection: re-loaded `pathdata.json` with the
  simplified loader and diffed all 18 tables' rows/ranges against the XML loader —
  identical — plus a 2,000-roll spread check on `Friends` confirming all 10 rows
  still reachable (the nullable `High` field still round-trips correctly).

Rebuilt after all three fixes (0 warnings, 0 errors) and manually confirmed
`LifePath.exe` still launches and runs.

## Decisions

Resolved with the user; reflected in the phase steps above.

| # | Question | Decision |
|---|---|---|
| 1 | Phase 2 namespace | Rename to `LifePath.Core` |
| 2 | Phase 3: drop `DataSet` for name tables too? | No — keep `DataSet`/CSV for names, scope stays weighted-tables-only |
| 3 | Phase 3 JSON layout | Single combined JSON file for all 18 tables |
| 4 | Phase 4 project layout | Standalone `LifePath.BehaviorTree` project |
| 5 | Phase 4 scope | Builder-based tree only (steps 1-3); JSON-driven loading deferred to a follow-up |

## Future / out of scope for this plan

- Dropping `System.Data.DataSet` from `CNameGenerator`'s name-table loading (Decision 2).
- `BehaviourTreeJsonLoader`-based, JSON-driven tree definitions (Decision 5, Phase 4
  step 4) — land the builder-based tree first, revisit as a separate pass.
- Whether tree `Selector`/`Parallel` nodes should ever replace `WeightedTable.Roll`
  weighting itself (Phase 4 step 5) — depends on the JSON-driven follow-up above.
