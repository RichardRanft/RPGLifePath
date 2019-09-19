﻿using System;
using System.Data;

namespace LifePath
{
    class CLifePathGenerator
    {
        private Random m_rand;
        private DataSet m_pathData;
        private CNameGenerator m_namegen;

        public CLifePathGenerator()
        {
            m_rand = new Random(DateTime.Now.Millisecond);
            CNameGenerator namegen = new CNameGenerator(m_pathData);
        }

        public CLifePathGenerator(DataSet set)
        {
            m_rand = new Random(DateTime.Now.Millisecond);
            CNameGenerator namegen = new CNameGenerator(m_pathData);
            m_pathData = set;
        }

        public CLifePath Generate(String firstname, String lastname)
        {
            CLifePath path = new CLifePath();
            path.FirstName = firstname;
            path.LastName = lastname;
            getParentStatus(ref path);
            return path;
        }

        private void getParentStatus(ref CLifePath path)
        {
            String parentStatus = getResult(m_pathData.Tables["Parents"]);
            if (parentStatus == "@BothLiving")
            {
                path.ParentStatus = getResult(m_pathData.Tables["BothLiving"]);
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
                path.ParentStatus = getResult(m_pathData.Tables["Other"]);
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
            getFamilySituation(ref path);
        }

        public void RollParents(ref CLifePath path)
        {
            path.Parents.Clear();
            String parentStatus = getResult(m_pathData.Tables["Parents"]);
            if (parentStatus == "@BothLiving")
            {
                path.ParentStatus = getResult(m_pathData.Tables["BothLiving"]);
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
                path.ParentStatus = getResult(m_pathData.Tables["Other"]);
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

        private void getFamilySituation(ref CLifePath path)
        {
            String familySituation = getResult(m_pathData.Tables["FamilyStanding"]);
            if (familySituation == "@Siblings")
            {
                String sibnum = getResult(m_pathData.Tables["Siblings"]);
                if (sibnum != "0")
                {
                    int num = int.Parse(sibnum);
                    for (int i = 0; i < num; ++i)
                    {
                        CActor sibling = new CActor();
                        sibling.FirstName = m_namegen.GetFirstName();
                        sibling.LastName = path.LastName;
                        sibling.Relationship = getResult(m_pathData.Tables["SiblingRel"]);
                        path.AddSibling(sibling);
                    }
                }
            }
            else
            {
                path.FamilyStatus = getResult(m_pathData.Tables["FamilyMisfortune"]);
                path.LifeGoal = getResult(m_pathData.Tables["LifeGoal"]);
            }
            getFriendsAndEnemies(ref path);
        }

        public void RollFamilySituation(ref CLifePath path)
        {
            path.FamilyStatus = "Normal";
            path.Siblings.Clear();
            String familySituation = getResult(m_pathData.Tables["FamilyStanding"]);
            if (familySituation == "@Siblings")
            {
                String sibnum = getResult(m_pathData.Tables["Siblings"]);
                if (sibnum != "0")
                {
                    int num = int.Parse(sibnum);
                    for (int i = 0; i < num; ++i)
                    {
                        CActor sibling = new CActor();
                        sibling.FirstName = m_namegen.GetFirstName();
                        sibling.LastName = path.LastName;
                        sibling.Relationship = getResult(m_pathData.Tables["SiblingRel"]);
                        path.AddSibling(sibling);
                    }
                }
            }
            else
            {
                path.FamilyStatus = getResult(m_pathData.Tables["FamilyMisfortune"]);
                path.LifeGoal = getResult(m_pathData.Tables["LifeGoal"]);
            }
        }

        public void RollSiblings(ref CLifePath path)
        {
            path.Siblings.Clear();
            String sibnum = getResult(m_pathData.Tables["Siblings"]);
            if (sibnum != "0")
            {
                int num = int.Parse(sibnum);
                for (int i = 0; i < num; ++i)
                {
                    CActor sibling = new CActor();
                    sibling.FirstName = m_namegen.GetFirstName();
                    sibling.LastName = path.LastName;
                    sibling.Relationship = getResult(m_pathData.Tables["SiblingRel"]);
                    path.AddSibling(sibling);
                }
            }
        }

        private void getFriendsAndEnemies(ref CLifePath path)
        {
            RollFriends(ref path);
            RollEnemies(ref path);
            getRomanticLife(ref path);
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
                friend.Relationship = getResult(m_pathData.Tables["Friends"]);
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
                enemy.Relationship = getResult(m_pathData.Tables["Enemies"]);
                enemy.Origin = getResult(m_pathData.Tables["EnemyOrigin"]);
                enemy.Status = getResult(m_pathData.Tables["EnemyStatus"]);
                enemy.Reaction = getResult(m_pathData.Tables["EnemyReaction"]);
                path.AddEnemy(enemy);
            }
        }

        private void getRomanticLife(ref CLifePath path)
        {
            String romance = getResult(m_pathData.Tables["Romance"]);
            switch (romance)
            {
                case "@RelationshipStatus":
                    path.Lover.FirstName = m_namegen.GetFirstName();
                    path.Lover.LastName = m_namegen.GetLastName();
                    path.Lover.Relationship = getResult(m_pathData.Tables["RelationshipStatus"]);
                    path.RomanceStatus = "In a relationship.";
                    break;
                case "@SingleStatus":
                    path.RomanceStatus = getResult(m_pathData.Tables["SingleStatus"]);
                    break;
                case "@ReboundStatus":
                    path.RomanceStatus = getResult(m_pathData.Tables["ReboundStatus"]);
                    getExStatus(ref path);
                    break;
            }
        }

        public void RollRomance(ref CLifePath path)
        {
            path.Lover = new CActor();
            String romance = getResult(m_pathData.Tables["Romance"]);
            switch (romance)
            {
                case "@RelationshipStatus":
                    path.Lover.FirstName = m_namegen.GetFirstName();
                    path.Lover.LastName = m_namegen.GetLastName();
                    path.Lover.Relationship = getResult(m_pathData.Tables["RelationshipStatus"]);
                    path.RomanceStatus = "In a relationship.";
                    break;
                case "@SingleStatus":
                    path.RomanceStatus = getResult(m_pathData.Tables["SingleStatus"]);
                    break;
                case "@ReboundStatus":
                    path.RomanceStatus = getResult(m_pathData.Tables["ReboundStatus"]);
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
                    path.Lover.Relationship = getResult(m_pathData.Tables["ExStatus"]);
                    break;
            }
        }

        private string getResult(DataTable table)
        {
            string result = "";
            Tuple<int, int> range = getRange(table);
            int roll = m_rand.Next(range.Item1, range.Item2 + 1);
            int low = 0;
            int high = 0;
            foreach (DataRow row in table.Rows)
            {
                String l = row["rlow"].ToString();
                String h = row["rhigh"].ToString();
                String r = row["result"].ToString();
                if (String.IsNullOrEmpty(h))
                {
                    low = int.Parse(l);
                    if (roll == low)
                    {
                        if (r == "#")
                            result = roll.ToString();
                        else
                            result = r;
                        break;
                    }
                }
                else
                {
                    low = int.Parse(l);
                    high = int.Parse(h);
                    if (roll >= low && roll <= high)
                    {
                        if (r == "#")
                            result = roll.ToString();
                        else
                            result = r;
                        break;
                    }
                }
            }

            return result;
        }

        private Tuple<int, int> getRange(DataTable table)
        {
            Tuple<int, int> range = new Tuple<int, int>(0, 0);
            int lval = 1;
            int hval = 1;
            int low = 0;
            int high = 0;
            foreach (DataRow row in table.Rows)
            {
                String l = row["rlow"].ToString();
                String h = row["rhigh"].ToString();
                if (!String.IsNullOrEmpty(l))
                {
                    low = int.Parse(l);
                    if (low < lval)
                    {
                        lval = low;
                    }
                }
                if (!String.IsNullOrEmpty(h))
                {
                    high = int.Parse(h);
                    if (hval < high)
                    {
                        hval = high;
                    }
                }
            }
            range = new Tuple<int, int>(lval, hval);
            return range;
        }
    }
}
