using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace LifePath
{
    class CLifePathGenerator
    {
        private Random m_rand;
        private DataSet m_pathData;

        public CLifePathGenerator()
        {
            m_rand = new Random(DateTime.Now.Millisecond);
        }

        public CLifePathGenerator(DataSet set)
        {
            m_rand = new Random(DateTime.Now.Millisecond);
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

        public CLifePath Generate()
        {
            CLifePath path = new CLifePath();
            getParentStatus(ref path);
            return path;
        }

        private void getParentStatus(ref CLifePath path)
        {
            String parentStatus = getResult(m_pathData.Tables["Parents"]);
            if (parentStatus == "@BothLiving")
            {
                path.ParentStatus = getResult(m_pathData.Tables["BothLiving"]);
                CNameGenerator namegen = new CNameGenerator(m_pathData);
                for (int i = 0; i < 2; ++i)
                {
                    CActor parent = new CActor();
                    parent.FirstName = namegen.GetFirstName();
                    parent.LastName = path.LastName;
                    path.AddParent(parent);
                }
            }
            else
            {
                path.ParentStatus = getResult(m_pathData.Tables["Other"]);
                if(path.ParentStatus.Contains("(s)"))
                {
                    int coin = m_rand.Next(2);
                    if(coin > 0)
                    {
                        CNameGenerator namegen = new CNameGenerator(m_pathData);
                        CActor parent = new CActor();
                        parent.FirstName = namegen.GetFirstName();
                        parent.LastName = path.LastName;
                        path.AddParent(parent);
                    }
                }
            }
            getFamilySituation(ref path);
        }

        private void getFamilySituation(ref CLifePath path)
        {
            String familySituation = getResult(m_pathData.Tables["FamilyStanding"]);
            if(familySituation == "@Siblings")
            {
                String sibnum = getResult(m_pathData.Tables["Siblings"]);
                if(sibnum != "0")
                {
                    CNameGenerator namegen = new CNameGenerator(m_pathData);
                    int num = int.Parse(sibnum);
                    for(int i = 0; i < num; ++i)
                    {
                        CActor sibling = new CActor();
                        sibling.FirstName = namegen.GetFirstName();
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

        private void getFriendsAndEnemies(ref CLifePath path)
        {
            int friends = 1;
            int roll = m_rand.Next(11);
            if (roll >= 5 && roll <= 8)
                friends = 2;
            if (roll >= 9)
                friends = 3;
            CNameGenerator namegen = new CNameGenerator(m_pathData);
            for (int i = 0; i < friends; ++i)
            {
                CActor friend = new CActor();
                friend.FirstName = namegen.GetFirstName();
                friend.LastName = namegen.GetLastName();
                friend.Relationship = getResult(m_pathData.Tables["Friends"]);
                path.AddFriend(friend);
            }
            int enemies = 1;
            roll = m_rand.Next(11);
            if (roll >= 5 && roll <= 8)
                enemies = 2;
            if (roll >= 9)
                enemies = 3;
            for (int i = 0; i < enemies; ++i)
            {
                CActor enemy = new CActor();
                enemy.FirstName = namegen.GetFirstName();
                enemy.LastName = namegen.GetLastName();
                enemy.Relationship = getResult(m_pathData.Tables["Enemies"]);
                enemy.Origin = getResult(m_pathData.Tables["EnemyOrigin"]);
                enemy.Status = getResult(m_pathData.Tables["EnemyStatus"]);
                enemy.Reaction = getResult(m_pathData.Tables["EnemyReaction"]);
                path.AddEnemy(enemy);
            }
            getRomanticLife(ref path);
        }

        private void getRomanticLife(ref CLifePath path)
        {
            String romance = getResult(m_pathData.Tables["Romance"]);
            CNameGenerator namegen = new CNameGenerator(m_pathData);
            switch(romance)
            {
                case "@RelationshipStatus":
                    path.Lover.FirstName = namegen.GetFirstName();
                    path.Lover.LastName = namegen.GetLastName();
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
            switch(path.RomanceStatus)
            {
                case "They died in a war":
                    break;
                case "They were killed in an accident":
                    break;
                default:
                    CNameGenerator namegen = new CNameGenerator(m_pathData);
                    path.Lover.FirstName = namegen.GetFirstName();
                    path.Lover.LastName = namegen.GetLastName();
                    path.Lover.Relationship = getResult(m_pathData.Tables["ExStatus"]);
                    break;
            }
        }

        private string getResult(DataTable table)
        {
            string result = "";

            int roll = m_rand.Next(1, 11);
            int low = 0;
            int high = 0;
            foreach(DataRow row in table.Rows)
            {
                String l = row["rlow"].ToString();
                String h = row["rhigh"].ToString();
                String r = row["result"].ToString();
                if(String.IsNullOrEmpty(h))
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
    }
}
