# RPGLifePath

A WinForms tool that rolls a random character backstory (family, friends, enemies,
romance) from weighted lifepath tables, in the style of Cyberpunk/Mekton-family RPGs.

## Projects

- `LifePath` — the WinForms UI (`Form1`), plus the `Tables\` data files it ships with.
- `LifePath.Core` — generation logic (`CLifePathGenerator`, `CLifePath`, `CActor`,
  `CNameGenerator`) and the `LifePath.Core.Tables` weighted-table abstraction
  (`WeightedTable`, loadable from `pathdata.xml` or the equivalent `pathdata.json`).
- `LifePath.BehaviorTree` — a vendored copy of `FluentBehaviourTree`, used by
  `CLifePathGenerator.Generate()` to run the parent/family/friends-and-enemies/romance
  pipeline as a behavior tree instead of a hardcoded call chain.

See `port_plan.md` for the history and design decisions behind this structure.

## Build & run

Requires the .NET 10 SDK.

```
dotnet build LifePath.slnx
dotnet run --project LifePath
```

## Data license

All data except the name tables are copyright R. Talsorian Games and extracted from
the Mekton II rulebook. The MIT license applies only to software and documentation -
I do not own the lifepath table data. R.Talsorian has advised that the data can be
used provided that I do not charge for this tool and that their copyright is
displayed prominently.
