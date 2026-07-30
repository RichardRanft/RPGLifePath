using System.Collections.Generic;

namespace FluentBehaviourTree
{
    /// <summary>
    /// JSON shape for a behaviour tree definition. Type is one of
    /// "Action" | "Sequence" | "Selector" | "Inverter" | "Parallel".
    /// Action nodes are resolved by name against a caller-supplied registry,
    /// since JSON can't encode the executable delegate itself.
    /// </summary>
    public class BehaviourTreeNodeJson
    {
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Action { get; set; }
        public int NumRequiredToFail { get; set; }
        public int NumRequiredToSucceed { get; set; }
        public List<BehaviourTreeNodeJson> Children { get; set; } = new();
    }
}
