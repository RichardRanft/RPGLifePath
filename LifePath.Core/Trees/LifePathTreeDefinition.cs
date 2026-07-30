using System.Collections.Generic;
using System.IO;
using FluentBehaviourTree;
using Newtonsoft.Json;

namespace LifePath.Core.Trees
{
    public static class LifePathTreeDefinition
    {
        public static BehaviourTreeNodeJson Default()
        {
            return new BehaviourTreeNodeJson
            {
                Type = "Sequence",
                Name = "Lifepath",
                Children = new List<BehaviourTreeNodeJson>
                {
                    new BehaviourTreeNodeJson { Type = "Action", Name = "ParentStatus", Action = "ParentStatus" },
                    new BehaviourTreeNodeJson { Type = "Action", Name = "FamilySituation", Action = "FamilySituation" },
                    new BehaviourTreeNodeJson { Type = "Action", Name = "FriendsAndEnemies", Action = "FriendsAndEnemies" },
                    new BehaviourTreeNodeJson { Type = "Action", Name = "RomanticLife", Action = "RomanticLife" },
                }
            };
        }

        public static void Save(BehaviourTreeNodeJson root, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(root, Formatting.Indented));
        }

        public static BehaviourTreeNodeJson Load(string path)
        {
            return JsonConvert.DeserializeObject<BehaviourTreeNodeJson>(File.ReadAllText(path));
        }
    }
}
