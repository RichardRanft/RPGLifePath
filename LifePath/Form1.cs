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
            if (String.IsNullOrEmpty(tbxFirstName.Text) || String.IsNullOrEmpty(tbxLastName.Text))
                return;
            m_lifepath = m_lpgen.Generate(tbxFirstName.Text, tbxLastName.Text);
            displayPathData();
        }

        private void displayPathData()
        {
            lblParent1.Text = "";
            lblParent2.Text = "";
            lblParentStatus.Text = m_lifepath.ParentStatus;
            if (m_lifepath.Parents.Count > 0)
                lblParent1.Text = m_lifepath.Parents[0].ToString();
            if (m_lifepath.Parents.Count > 1)
                lblParent2.Text = m_lifepath.Parents[1].ToString();
            lblFamilyStatus.Text = m_lifepath.FamilyStatus;
            lbxSiblings.Items.Clear();
            foreach (CActor actor in m_lifepath.Siblings)
                lbxSiblings.Items.Add(actor);
            lbxFriends.Items.Clear();
            foreach (CActor actor in m_lifepath.Friends)
                lbxFriends.Items.Add(actor);
            lbxEnemies.Items.Clear();
            foreach (CActor actor in m_lifepath.Enemies)
                lbxEnemies.Items.Add(actor);
            lblLoverName.Text = "";
            lblRelInfo.Text = "";
            lblRelationshipStatus.Text = m_lifepath.RomanceStatus;
            if(!String.IsNullOrEmpty(m_lifepath.Lover.FirstName))
            {
                lblLoverName.Text = m_lifepath.Lover.Name;
                lblRelInfo.Text = m_lifepath.Lover.Relationship;
            }
            tbxDisplaySelected.Text = "";
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
            if (m_lifepath != null)
                dumpPathData();
        }

        private void lbxSiblings_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if(box.SelectedItem != null)
            {
                CActor actor = (CActor)box.SelectedItem;
                tbxDisplaySelected.Text = actor.GetDescription();
            }
        }

        private void lblItemReroll_Click(object sender, EventArgs e)
        {
            if (m_lifepath == null)
                return;
            Label clicked = (Label)sender;
            if(!clicked.Name.Contains("Reroll"))
            {
                switch(clicked.Name)
                {
                    case "lblParent1":
                        m_lifepath.Parents[0].FirstName = m_namegen.GetFirstName();
                        break;
                    case "lblParent2":
                        m_lifepath.Parents[1].FirstName = m_namegen.GetFirstName();
                        break;
                    case "lblLoverName":
                        m_lifepath.Lover.FirstName = m_namegen.GetFirstName();
                        m_lifepath.Lover.LastName = m_namegen.GetLastName();
                        break;
                }
            }
            if (clicked == lblParentReroll)
                m_lpgen.RollParents(ref m_lifepath);
            if (clicked == lblFamilyReroll)
                m_lpgen.RollFamilySituation(ref m_lifepath);
            if (clicked == lblSiblingReroll)
                m_lpgen.RollSiblings(ref m_lifepath);
            if (clicked == lblFriendsReroll)
                m_lpgen.RollFriends(ref m_lifepath);
            if (clicked == lblEnemiesReroll)
                m_lpgen.RollEnemies(ref m_lifepath);
            if (clicked == lblRelationshipReroll)
                m_lpgen.RollRomance(ref m_lifepath);
            displayPathData();
        }

        private void lbxPeople_Leave(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            box.SelectedIndex = -1;
        }
    }
}
