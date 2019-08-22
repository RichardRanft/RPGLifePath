using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace LifePath
{
    class CNameGenerator
    {
        private List<String> m_first;
        private List<String> m_last;
        private Random m_rnd;
        private bool m_loaded = false;
        private DataSet m_pathData;

        public CNameGenerator()
        {
            m_rnd = new Random(DateTime.Now.Millisecond);
        }

        public CNameGenerator(DataSet set)
        {
            m_rnd = new Random(DateTime.Now.Millisecond);
            m_pathData = set;
        }

        private void loadNames()
        {
            try
            {
                if (m_pathData == null)
                {
                    if (m_first == null)
                    {
                        m_first = new List<string>();
                        String names = "";
                        if (!File.Exists("Tables\\database_of_first_names.csv"))
                        {
                            using (StreamReader sr = new StreamReader("Tables\\csv_database_of_first_names.csv"))
                            {
                                names = sr.ReadToEnd();
                            }
                            if (names.Contains(Environment.NewLine))
                            {
                                names = convertEOLtoComma(names);
                                using (StreamWriter sw = new StreamWriter("Tables\\database_of_first_names.csv"))
                                {
                                    sw.Write(names);
                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr = new StreamReader("Tables\\database_of_first_names.csv"))
                            {
                                names = sr.ReadToEnd();
                            }
                        }
                        String[] parts = names.Split(',');
                        foreach (String part in parts)
                            m_first.Add(part);
                    }
                    if (m_last == null)
                    {
                        String names = "";
                        if (!File.Exists("Tables\\database_of_last_names.csv"))
                        {
                            m_last = new List<string>();
                            using (StreamReader sr = new StreamReader("Tables\\csv_database_of_last_names.csv"))
                            {
                                names = sr.ReadToEnd();
                            }
                            if (names.Contains(Environment.NewLine))
                            {
                                names = convertEOLtoComma(names);
                                using (StreamWriter sw = new StreamWriter("Tables\\database_of_last_names.csv"))
                                {
                                    sw.Write(names);
                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr = new StreamReader("Tables\\database_of_last_names.csv"))
                            {
                                names = sr.ReadToEnd();
                            }
                        }
                        String[] parts = names.Split(',');
                        foreach (String part in parts)
                            m_last.Add(part);
                    }
                    m_loaded = true;
                }
                else
                {
                    m_first = new List<String>();
                    foreach (DataRow row in m_pathData.Tables["First_Names"].Rows)
                    {
                        m_first.Add(row["name"].ToString());
                    }
                    m_last = new List<String>();
                    foreach (DataRow row in m_pathData.Tables["Last_Names"].Rows)
                    {
                        m_last.Add(row["name"].ToString());
                    }
                    m_loaded = true;
                }
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                    msg += Environment.NewLine + ex.InnerException.Message;

                MessageBox.Show(msg, "Error Loading Name Database");
            }
        }

        public KeyValuePair<string, string> Generate()
        {
            if (!m_loaded)
                loadNames();
            
            String first = m_first[m_rnd.Next(0, m_first.Count - 1)].ToLower();
            String last = m_last[m_rnd.Next(0, m_last.Count - 1)].ToLower();
            first = capitalize(first);
            last = capitalize(last);
            KeyValuePair<string, string> name = new KeyValuePair<string, string>(first, last);

            return name;
        }

        public KeyValuePair<string, string> Generate(string last)
        {
            if (!m_loaded)
                loadNames();

            String first = m_first[m_rnd.Next(0, m_first.Count - 1)].ToLower();
            first = capitalize(first);
            last = capitalize(last);
            KeyValuePair<string, string> name = new KeyValuePair<string, string>(first, last);

            return name;
        }

        public String GetFirstName()
        {
            if (!m_loaded)
                loadNames();

            String first = m_first[m_rnd.Next(0, m_first.Count - 1)].ToLower();
            first = capitalize(first);
            return first;
        }

        public String GetLastName()
        {
            if (!m_loaded)
                loadNames();

            String last = m_last[m_rnd.Next(0, m_last.Count - 1)].ToLower();
            last = capitalize(last);
            return last;
        }

        private String convertEOLtoComma(String text)
        {
            return text.Replace(Environment.NewLine, ",");
        }

        private String capitalize(String text)
        {
            String first = text[0].ToString();
            text = text.Remove(0, 1);
            first = first.ToUpper();
            text = text.Insert(0, first);
            return text;
        }
    }
}
