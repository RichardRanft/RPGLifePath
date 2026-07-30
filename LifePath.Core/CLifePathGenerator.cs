using System;
using System.Collections.Generic;
using System.Data;
using FluentBehaviourTree;
using LifePath.Core.Tables;

namespace LifePath.Core
{
    public class CLifePathGenerator
    {
        private Random m_rand;
        private Dictionary<string, WeightedTable> m_tables;
        private CNameGenerator m_namegen;

        public CLifePathGenerator(Dictionary<string, WeightedTable> tables, DataSet nameData = null)
        {
            m_rand = new Random(DateTime.Now.Millisecond);
            m_tables = tables;
            m_namegen = new CNameGenerator(nameData);
        }

        public CLifePath Generate(String firstname, String lastname)
        {
            CLifePath path = new CLifePath();
            path.FirstName = firstname;
            path.LastName = lastname;

            IBehaviourTreeNode tree = new BehaviourTreeBuilder()
                .Sequence("Lifepath")
                    .Do("ParentStatus", t => { RollParents(ref path); return BehaviourTreeStatus.Success; })
                    .Do("FamilySituation", t => { RollFamilySituation(ref path); return BehaviourTreeStatus.Success; })
                    .Do("FriendsAndEnemies", t => { RollFriends(ref path); RollEnemies(ref path); return BehaviourTreeStatus.Success; })
                    .Do("RomanticLife", t => { RollRomance(ref path); return BehaviourTreeStatus.Success; })
                .End()
                .Build();
            tree.Tick(default);

            return path;
        }

        public void RollParents(ref CLifePath path)
        {
            path.Parents.Clear();
            String parentStatus = m_tables["Parents"].Roll(m_rand);
            if (parentStatus == "@BothLiving")
            {
                path.ParentStatus = m_tables["BothLiving"].Roll(m_rand);
                for (int i = 0; i < 2; ++i)
                {
                    CActor parent = new CActor();
                    parent.FirstName = m_namegen.GetFirstName();
                    parent.LastName = path.LastName;
                    path.AddParent(parent);
                }
            }
            else
            {
                path.ParentStatus = m_tables["Other"].Roll(m_rand);
                if (path.ParentStatus.Contains("(s)"))
                {
                    int coin = m_rand.Next(2);
                    if (coin > 0)
                    {
                        CActor parent = new CActor();
                        parent.FirstName = m_namegen.GetFirstName();
                        parent.LastName = path.LastName;
                        path.AddParent(parent);
                    }
                }
            }
        }

        public void RollFamilySituation(ref CLifePath path)
        {
            path.FamilyStatus = "Normal";
            path.Siblings.Clear();
            String familySituation = m_tables["FamilyStanding"].Roll(m_rand);
            if (familySituation == "@Siblings")
            {
                String sibnum = m_tables["Siblings"].Roll(m_rand);
                if (sibnum != "0")
                {
                    int num = int.Parse(sibnum);
                    for (int i = 0; i < num; ++i)
                    {
                        CActor sibling = new CActor();
                        sibling.FirstName = m_namegen.GetFirstName();
                        sibling.LastName = path.LastName;
                        sibling.Relationship = m_tables["SiblingRel"].Roll(m_rand);
                        path.AddSibling(sibling);
                    }
                }
            }
            else
            {
                path.FamilyStatus = m_tables["FamilyMisfortune"].Roll(m_rand);
                path.LifeGoal = m_tables["LifeGoal"].Roll(m_rand);
            }
        }

        public void RollSiblings(ref CLifePath path)
        {
            path.Siblings.Clear();
            String sibnum = m_tables["Siblings"].Roll(m_rand);
            if (sibnum != "0")
            {
                int num = int.Parse(sibnum);
                for (int i = 0; i < num; ++i)
                {
                    CActor sibling = new CActor();
                    sibling.FirstName = m_namegen.GetFirstName();
                    sibling.LastName = path.LastName;
                    sibling.Relationship = m_tables["SiblingRel"].Roll(m_rand);
                    path.AddSibling(sibling);
                }
            }
        }

        public void RollFriends(ref CLifePath path)
        {
            path.Friends.Clear();
            int friends = 1;
            int roll = m_rand.Next(11);
            if (roll >= 5 && roll <= 8)
                friends = 2;
            if (roll >= 9)
                friends = 3;
            for (int i = 0; i < friends; ++i)
            {
                CActor friend = new CActor();
                friend.FirstName = m_namegen.GetFirstName();
                friend.LastName = m_namegen.GetLastName();
                friend.Relationship = m_tables["Friends"].Roll(m_rand);
                path.AddFriend(friend);
            }
        }

        public void RollEnemies(ref CLifePath path)
        {
            path.Enemies.Clear();
            int enemies = 1;
            int roll = m_rand.Next(11);
            if (roll >= 5 && roll <= 8)
                enemies = 2;
            if (roll >= 9)
                enemies = 3;
            for (int i = 0; i < enemies; ++i)
            {
                CActor enemy = new CActor();
                enemy.FirstName = m_namegen.GetFirstName();
                enemy.LastName = m_namegen.GetLastName();
                enemy.Relationship = m_tables["Enemies"].Roll(m_rand);
                enemy.Origin = m_tables["EnemyOrigin"].Roll(m_rand);
                enemy.Status = m_tables["EnemyStatus"].Roll(m_rand);
                enemy.Reaction = m_tables["EnemyReaction"].Roll(m_rand);
                path.AddEnemy(enemy);
            }
        }

        public void RollRomance(ref CLifePath path)
        {
            path.Lover = new CActor();
            String romance = m_tables["Romance"].Roll(m_rand);
            switch (romance)
            {
                case "@RelationshipStatus":
                    path.Lover.FirstName = m_namegen.GetFirstName();
                    path.Lover.LastName = m_namegen.GetLastName();
                    path.Lover.Relationship = m_tables["RelationshipStatus"].Roll(m_rand);
                    path.RomanceStatus = "In a relationship.";
                    break;
                case "@SingleStatus":
                    path.RomanceStatus = m_tables["SingleStatus"].Roll(m_rand);
                    break;
                case "@ReboundStatus":
                    path.RomanceStatus = m_tables["ReboundStatus"].Roll(m_rand);
                    getExStatus(ref path);
                    break;
            }
        }

        private void getExStatus(ref CLifePath path)
        {
            switch (path.RomanceStatus)
            {
                case "They died in a war":
                    break;
                case "They were killed in an accident":
                    break;
                default:
                    path.Lover.FirstName = m_namegen.GetFirstName();
                    path.Lover.LastName = m_namegen.GetLastName();
                    path.Lover.Relationship = m_tables["ExStatus"].Roll(m_rand);
                    break;
            }
        }
    }
}
