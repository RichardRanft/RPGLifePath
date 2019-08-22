using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LifePath
{
    public partial class Form1 : Form
    {
        private CLifePathGenerator m_lpgen;
        private CNameGenerator m_namegen;
        private DataSet m_pathData;
        private CLifePath m_lifepath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("Tables\\pathdata.xml"))
            {
                try
                {
                    m_pathData = new DataSet();
                    m_pathData.ReadXml("Tables\\pathdata.xml");
                    m_lpgen = new CLifePathGenerator(m_pathData);
                    m_namegen = new CNameGenerator(m_pathData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to Load Data");
                    Application.Exit();
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            m_lifepath = m_lpgen.Generate(tbxFirstName.Text, tbxLastName.Text);
            displayPathData();
        }

        private void displayPathData()
        {
            
        }

        private void dumpPathData()
        {
            String outfile = String.Format("{0}_{1}.txt", m_lifepath.FirstName, m_lifepath.LastName);
            using (StreamWriter sw = new StreamWriter(outfile))
            {
                sw.WriteLine("--------");
                sw.WriteLine(m_lifepath.Name);
                sw.WriteLine("--------");
                sw.WriteLine(" - Family -");
                sw.WriteLine("Parents: {0}", m_lifepath.ParentStatus);
                foreach(CActor actor in m_lifepath.Parents)
                {
                    sw.WriteLine("Name : {0}", actor.Name);
                    sw.WriteLine("-");
                }
                if (m_lifepath.Siblings.Count > 0)
                {
                    sw.WriteLine("Siblings:");
                    foreach (CActor actor in m_lifepath.Siblings)
                    {
                        sw.WriteLine("Name         : {0}", actor.Name);
                        sw.WriteLine("Relationship : {0}", actor.Relationship);
                        sw.WriteLine("-");
                    }
                }
                sw.WriteLine("Family Status: {0}", m_lifepath.FamilyStatus);
                if(m_lifepath.FamilyStatus != "Normal")
                    sw.WriteLine("Life Goal: {0}", m_lifepath.LifeGoal);
                sw.WriteLine("--------");
                sw.WriteLine(" - Friends and Enemies -");
                sw.WriteLine("Friends:");
                foreach(CActor actor in m_lifepath.Friends)
                {
                    sw.WriteLine("Name         : {0}", actor.Name);
                    sw.WriteLine("Relationship : {0}", actor.Relationship);
                    sw.WriteLine("-");
                }
                sw.WriteLine("Enemies:");
                foreach (CActor actor in m_lifepath.Enemies)
                {
                    sw.WriteLine("Name         : {0}", actor.Name);
                    sw.WriteLine("Relationship : {0}", actor.Relationship);
                    sw.WriteLine("Origin       : {0}", actor.Origin);
                    sw.WriteLine("Status       : {0}", actor.Status);
                    sw.WriteLine("Reaction     : {0}", actor.Reaction);
                    sw.WriteLine("-");
                }
                sw.WriteLine("--------");
                sw.WriteLine(" - Romance -");
                sw.WriteLine("Status       : {0}", m_lifepath.RomanceStatus);
                if(!String.IsNullOrEmpty(m_lifepath.Lover.FirstName))
                {
                    sw.WriteLine("Name         : {0}", m_lifepath.Lover.Name);
                    sw.WriteLine("Relationship : {0}", m_lifepath.Lover.Relationship);
                }
            }
        }

        private void btnFirstName_Click(object sender, EventArgs e)
        {
            tbxFirstName.Text = m_namegen.GetFirstName();
        }

        private void btnLastName_Click(object sender, EventArgs e)
        {
            tbxLastName.Text = m_namegen.GetLastName();
        }

        private void btnGenName_Click(object sender, EventArgs e)
        {
            KeyValuePair<String, String> name = m_namegen.Generate();
            tbxFirstName.Text = name.Key;
            tbxLastName.Text = name.Value;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dumpPathData();
        }
    }
}
