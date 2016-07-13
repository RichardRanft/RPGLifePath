using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSettings;
using System.IO;
using System.Windows.Forms;

namespace LifePath
{
    class CLifePathGenerator
    {
        private Random m_rand;
        private CSettings m_parents;
        private CSettings m_family;
        private CSettings m_friends;
        private CSettings m_romance;
        private Dictionary<String, StringDictionary> m_tables;

        private String m_parentStatus;
        
        public CLifePathGenerator()
        {
            m_rand = new Random(DateTime.Now.Millisecond);
        }

        public bool Load()
        {
            bool success = true;
            if (File.Exists("Tables\\parentStatus.txt"))
            {
                m_parents = new CSettings("Tables\\parentStatus.txt");
                if (!m_parents.LoadSettings())
                {
                    MessageBox.Show("Unable to load parent status tables.");
                    success = false;
                }
            }
            if (File.Exists("Tables\\familyBackground.txt"))
            {
                m_family = new CSettings("Tables\\familyBackground.txt");
                if (!m_family.LoadSettings())
                {
                    MessageBox.Show("Unable to load family background tables.");
                    success = false;
                }
            }
            if (File.Exists("Tables\\friendsAndEnemies.txt"))
            {
                m_friends = new CSettings("Tables\\friendsAndEnemies.txt");
                if (!m_friends.LoadSettings())
                {
                    MessageBox.Show("Unable to load friends and enemies tables.");
                    success = false;
                }
            }
            if (File.Exists("Tables\\romanticLife.txt"))
            {
                m_romance = new CSettings("Tables\\romanticLife.txt");
                if (!m_romance.LoadSettings())
                {
                    MessageBox.Show("Unable to load romance tables.");
                    success = false;
                }
            }
            extractTables();
            return success;
        }

        private void extractTables()
        {
            m_tables = new Dictionary<string, StringDictionary>();
            try
            {
                foreach (KeyValuePair<String, StringDictionary> entry in m_parents.Attributes)
                {
                    if(entry.Key.CompareTo("[Default]") != 0)
                        m_tables.Add(entry.Key, entry.Value);
                }
                foreach (KeyValuePair<String, StringDictionary> entry in m_family.Attributes)
                {
                    if (entry.Key.CompareTo("[Default]") != 0)
                        m_tables.Add(entry.Key, entry.Value);
                }
                foreach (KeyValuePair<String, StringDictionary> entry in m_friends.Attributes)
                {
                    if (entry.Key.CompareTo("[Default]") != 0)
                        m_tables.Add(entry.Key, entry.Value);
                }
                foreach (KeyValuePair<String, StringDictionary> entry in m_romance.Attributes)
                {
                    if (entry.Key.CompareTo("[Default]") != 0)
                        m_tables.Add(entry.Key, entry.Value);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += Environment.NewLine + ex.InnerException.Message;
                MessageBox.Show(msg, "Error Loading Tables");
            }
        }

        public CLifePath Generate()
        {
            CLifePath path = new CLifePath();
            StringDictionary currentTable = m_tables["[Parents]"];
            String result = getResult(currentTable);
            String next = "";
            currentTable = m_tables["[" + result.Replace("@", "") + "]"];
            if(result.ToLower().Contains("other"))
            {
                // how many surviving parents?
                result = getResult(currentTable);
                next = getNextTable(currentTable);
                currentTable = m_tables["[" + next.Replace("@", "") + "]"];

                int living = m_rand.Next(100);
                if (living < 25)
                    path.ParentStatus = "Both are dead";
                else
                {
                    path.ParentStatus = "One is dead";
                    CActor parent = new CActor();
                    parent.Age = m_rand.Next(36, 60).ToString();
                    parent.Status = getResult(currentTable);
                    path.Parents.Add(parent);
                }
            }
            else
            {
                // generate both parents
                result = getResult(currentTable);
                next = getNextTable(currentTable);
                currentTable = m_tables["[" + next.Replace("@", "") + "]"];

                path.ParentStatus = "Both are living";
                CActor parent = new CActor();
                parent.Age = m_rand.Next(36, 60).ToString();
                CActor parent2 = new CActor();
                parent2.Age = m_rand.Next(36, 60).ToString();
                switch (result)
                {
                    case "You get along well with both parents":
                        parent.Attitude = "Good";
                        parent2.Attitude = "Good";
                        break;
                    case "You get along well with one but not the other":
                        int coin = m_rand.Next(1);
                        if(coin == 0)
                        {
                            parent.Attitude = "Bad";
                            parent2.Attitude = "Good";
                        }
                        else
                        {
                            parent.Attitude = "Good";
                            parent2.Attitude = "Bad";
                        }
                        break;
                    default:
                        parent.Attitude = "Bad";
                        parent2.Attitude = "Bad";
                        break;
                }

                path.Parents.Add(parent);
                path.Parents.Add(parent2);
            }
            result = getResult(currentTable);
            next = getNextTable(currentTable);
            currentTable = m_tables["[" + next.Replace("@", "") + "]"];

            return path;
        }

        private string getResult(StringDictionary table)
        {
            string result = "";

            int roll = m_rand.Next(1, 11);
            int low = 0;
            int high = 0;
            foreach(string range in table.Keys)
            {
                if (range.ToLower().CompareTo("next") == 0)
                    continue;
                if (range.Contains('-'))
                {
                    string[] parts = range.Split('-');
                    low = int.Parse(parts[0]);
                    high = int.Parse(parts[1]);
                }
                else
                    low = high = int.Parse(range);
                if(roll >= low && roll <= high)
                {
                    result = table[range];
                    break;
                }
            }

            return result;
        }

        private string getNextTable(StringDictionary table)
        {
            if (table.ContainsKey("NEXT"))
                return table["NEXT"];
            else
                return "";
        }
    }
}
