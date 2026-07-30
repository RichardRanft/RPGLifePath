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

## Phase 4 — evaluate FluentBehaviourTree for generation flexibility

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
