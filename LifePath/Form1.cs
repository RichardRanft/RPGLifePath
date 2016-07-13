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
using BasicSettings;

namespace LifePath
{
    public partial class Form1 : Form
    {
        private CLifePathGenerator m_lpgen;
        private CNameGenerator m_namegen;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_lpgen = new CLifePathGenerator();
            m_lpgen.Load();
            m_namegen = new CNameGenerator();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            m_lpgen.Generate();
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
    }
}
