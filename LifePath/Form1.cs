using System;
using System.Collections.Generic;
using System.Data;
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
        private CActor m_player;

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
            if (String.IsNullOrEmpty(tbxFirstName.Text))
                tbxFirstName.Text = m_namegen.GetFirstName();
            if (String.IsNullOrEmpty(tbxLastName.Text))
                tbxLastName.Text = m_namegen.GetLastName();

            m_lifepath = m_lpgen.Generate(tbxFirstName.Text, tbxLastName.Text);

            m_player = new CActor();
            m_player.FirstName = m_lifepath.FirstName;
            m_player.LastName = m_lifepath.LastName;
            m_player.Lifepath = m_lifepath;

            getParentPath();
            getSiblingPaths(ref m_player);
            getFriendPaths(ref m_player);
            getEnemyPaths(ref m_player);
            getLoverPath(ref m_player);
            displayPathData();
        }

        private void getLoverPath(ref CActor me)
        {
            if (m_lifepath.Lover != null && !String.IsNullOrEmpty(m_lifepath.Lover.FirstName))
            {
                m_lifepath.Lover.Lifepath = m_lpgen.Generate(m_lifepath.Lover.FirstName, m_lifepath.Lover.LastName);
                m_lifepath.Lover.Lifepath.Lover = me;
                m_lifepath.Lover.Lifepath.RomanceStatus = m_lifepath.RomanceStatus;
            }
        }

        private void getEnemyPaths(ref CActor me)
        {
            for (int i = 0; i < m_lifepath.Enemies.Count; ++i)
            {
                CActor enemy = m_lifepath.Enemies[i];
                enemy.Lifepath = m_lpgen.Generate(enemy.FirstName, enemy.LastName);
                enemy.Lifepath.Enemies.Add(me);
                enemy.Lifepath.Enemies[enemy.Lifepath.Enemies.Count - 1].Relationship = enemy.Relationship;
                enemy.Lifepath.Enemies[enemy.Lifepath.Enemies.Count - 1].Origin = enemy.Origin;
                enemy.Lifepath.Enemies[enemy.Lifepath.Enemies.Count - 1].Status = enemy.Status;
                enemy.Lifepath.Enemies[enemy.Lifepath.Enemies.Count - 1].Reaction = enemy.Reaction;
            }
        }

        private void getFriendPaths(ref CActor me)
        {
            for (int i = 0; i < m_lifepath.Friends.Count; ++i)
            {
                CActor friend = m_lifepath.Friends[i];
                friend.Lifepath = m_lpgen.Generate(friend.FirstName, friend.LastName);
                friend.Lifepath.Friends.Add(me);
                friend.Lifepath.Enemies[friend.Lifepath.Enemies.Count - 1].Relationship = friend.Relationship;
                friend.Lifepath.Enemies[friend.Lifepath.Enemies.Count - 1].Origin = friend.Origin;
                friend.Lifepath.Enemies[friend.Lifepath.Enemies.Count - 1].Status = friend.Status;
                friend.Lifepath.Enemies[friend.Lifepath.Enemies.Count - 1].Reaction = friend.Reaction;
            }
        }

        private void getParentPath()
        {
            CNameGenerator namegen = new CNameGenerator(m_pathData);
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < m_lifepath.Parents.Count; ++i)
            {
                CActor parent = m_lifepath.Parents[i];
                int coin = rand.Next(0, 1);
                if (coin > 0)
                    parent.Lifepath = m_lpgen.Generate(parent.FirstName, namegen.GetLastName());
                else
                    parent.Lifepath = m_lpgen.Generate(parent.FirstName, parent.LastName);
            }
        }

        private void getSiblingPaths(ref CActor me)
        {
            for (int i = 0; i < m_lifepath.Siblings.Count; ++i)
            {
                CActor sibling = m_lifepath.Siblings[i];
                sibling.Lifepath = m_lpgen.Generate(sibling.FirstName, sibling.LastName);
                sibling.Lifepath.Siblings.Clear();
                sibling.Lifepath.Siblings.Add(me);
                foreach (CActor sib in m_lifepath.Siblings)
                {
                    if (!sib.FirstName.Equals(sibling.FirstName))
                        sibling.Lifepath.Siblings.Add(sib);
                }
            }
        }

        private void displayPathData()
        {
            lblParent1.Text = "";
            lblParent2.Text = "";
            lblParentStatus.Text = m_player.Lifepath.ParentStatus;
            if (m_lifepath.Parents.Count > 0)
                lblParent1.Text = m_player.Lifepath.Parents[0].ToString();
            if (m_lifepath.Parents.Count > 1)
                lblParent2.Text = m_player.Lifepath.Parents[1].ToString();
            lblFamilyStatus.Text = m_player.Lifepath.FamilyStatus;
            lbxSiblings.Items.Clear();
            foreach (CActor actor in m_player.Lifepath.Siblings)
                lbxSiblings.Items.Add(actor);
            lbxFriends.Items.Clear();
            foreach (CActor actor in m_player.Lifepath.Friends)
                lbxFriends.Items.Add(actor);
            lbxEnemies.Items.Clear();
            foreach (CActor actor in m_player.Lifepath.Enemies)
                lbxEnemies.Items.Add(actor);
            lblLoverName.Text = "";
            lblRelInfo.Text = "";
            lblRelationshipStatus.Text = m_player.Lifepath.RomanceStatus;
            if(!String.IsNullOrEmpty(m_player.Lifepath.Lover.FirstName))
            {
                lblLoverName.Text = m_player.Lifepath.Lover.Name;
                lblRelInfo.Text = m_player.Lifepath.Lover.Relationship;
            }
            tbxDisplaySelected.Text = "";
        }

        private void dumpPathData()
        {
            m_player.Save();
            foreach(CActor actor in m_lifepath.Parents)
                actor.Save(m_player);
            foreach (CActor actor in m_lifepath.Siblings)
                actor.Save(m_player);
            foreach(CActor actor in m_lifepath.Friends)
                actor.Save(m_player);
            foreach (CActor actor in m_lifepath.Enemies)
                actor.Save(m_player);
            if (!String.IsNullOrEmpty(m_lifepath.Lover.FirstName))
                m_player.Lifepath.Lover.Save(m_player);
        }

        private void btnFirstName_Click(object sender, EventArgs e)
        {
            tbxFirstName.Text = m_namegen.GetFirstName();
            updatePath();
        }

        private void btnLastName_Click(object sender, EventArgs e)
        {
            tbxLastName.Text = m_namegen.GetLastName();
            updatePath();
        }

        private void btnGenName_Click(object sender, EventArgs e)
        {
            KeyValuePair<String, String> name = m_namegen.Generate();
            tbxFirstName.Text = name.Key;
            tbxLastName.Text = name.Value;
            updatePath();
        }

        private void updatePath()
        {
            if(m_lifepath != null)
            {
                m_lifepath.FirstName = tbxFirstName.Text;
                m_lifepath.LastName = tbxLastName.Text;
                m_player.FirstName = m_lifepath.FirstName;
                m_player.LastName = m_lifepath.LastName;
                if(m_lifepath.Parents.Count > 0)
                {
                    foreach (CActor actor in m_lifepath.Parents)
                        actor.LastName = m_lifepath.LastName;
                }
                if (m_lifepath.Siblings.Count > 0)
                {
                    foreach (CActor actor in m_lifepath.Siblings)
                        actor.LastName = m_lifepath.LastName;
                }
                displayPathData();
            }
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
            {
                m_lpgen.RollParents(ref m_lifepath);
                getParentPath();
            }
            if (clicked == lblFamilyReroll)
                m_lpgen.RollFamilySituation(ref m_lifepath);
            if (clicked == lblSiblingReroll)
            {
                m_lpgen.RollSiblings(ref m_lifepath);
                getSiblingPaths(ref m_player);
            }
            if (clicked == lblFriendsReroll)
            {
                m_lpgen.RollFriends(ref m_lifepath);
                getFriendPaths(ref m_player);
            }
            if (clicked == lblEnemiesReroll)
            {
                m_lpgen.RollEnemies(ref m_lifepath);
                getEnemyPaths(ref m_player);
            }
            if (clicked == lblRelationshipReroll)
            {
                m_lpgen.RollRomance(ref m_lifepath);
                getLoverPath(ref m_player);
            }
            displayPathData();
        }

        private void lbxPeople_Leave(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            box.SelectedIndex = -1;
        }

        private void tbxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_lifepath == null)
                return;
            if (e.KeyCode != Keys.Return)
                return;
            TextBox box = (TextBox)sender;
            if (box == tbxFirstName)
            {
                m_lifepath.FirstName = tbxFirstName.Text;
                updatePath();
            }
            if (box == tbxLastName)
            {
                m_lifepath.LastName = tbxLastName.Text;
                updatePath();
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}
