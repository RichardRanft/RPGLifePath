using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Builds an IBehaviourTreeNode tree from a JSON definition (see BehaviourTreeNodeJson),
    /// so a plugin's tree can be edited without recompiling. Action leaf nodes are resolved
    /// by name against the given registry.
    /// </summary>
    public static class BehaviourTreeJsonLoader
    {
        public static IBehaviourTreeNode LoadFromFile(string path, IReadOnlyDictionary<string, Func<TimeData, BehaviourTreeStatus>> actions)
        {
            return LoadFromJson(File.ReadAllText(path), actions);
        }

        public static IBehaviourTreeNode LoadFromJson(string json, IReadOnlyDictionary<string, Func<TimeData, BehaviourTreeStatus>> actions)
        {
            var root = JsonConvert.DeserializeObject<BehaviourTreeNodeJson>(json)
                ?? throw new InvalidOperationException("Behaviour tree JSON was empty or invalid.");
            return BuildNode(root, actionName =>
                actions.TryGetValue(actionName, out var fn)
                    ? fn
                    : throw new InvalidOperationException($"No registered action named '{actionName}'."));
        }

        /// <summary>
        /// Builds a tree from an already-parsed node (e.g. one converted from a Lua table) instead
        /// of JSON text, resolving each Action leaf's function on demand via
        /// <paramref name="actionResolver"/> rather than a fixed dictionary - lets the caller bind
        /// the resolved function to a specific context (e.g. a behaviour tree instance name) at
        /// build time.
        /// </summary>
        public static IBehaviourTreeNode LoadFromNode(BehaviourTreeNodeJson node, Func<string, Func<TimeData, BehaviourTreeStatus>> actionResolver)
        {
            return BuildNode(node, actionResolver);
        }

        private static IBehaviourTreeNode BuildNode(BehaviourTreeNodeJson node, Func<string, Func<TimeData, BehaviourTreeStatus>> actionResolver)
        {
            switch (node.Type)
            {
                case "Action":
                    if (node.Action == null)
                        throw new InvalidOperationException($"Action node '{node.Name}' is missing its Action name.");
                    return new ActionNode(node.Name, actionResolver(node.Action));

                case "Sequence":
                    return BuildChildren(new SequenceNode(node.Name), node, actionResolver);

                case "Selector":
                    return BuildChildren(new SelectorNode(node.Name), node, actionResolver);

                case "Inverter":
                    return BuildChildren(new InverterNode(node.Name), node, actionResolver);

                case "Parallel":
                    return BuildChildren(new ParallelNode(node.Name, node.NumRequiredToFail, node.NumRequiredToSucceed), node, actionResolver);

                default:
                    throw new InvalidOperationException($"Unknown behaviour tree node type '{node.Type}' for node '{node.Name}'.");
            }
        }

        private static IBehaviourTreeNode BuildChildren(IParentBehaviourTreeNode parent, BehaviourTreeNodeJson node, Func<string, Func<TimeData, BehaviourTreeStatus>> actionResolver)
        {
            foreach (var child in node.Children)
                parent.AddChild(BuildNode(child, actionResolver));
            return parent;
        }
    }
}
