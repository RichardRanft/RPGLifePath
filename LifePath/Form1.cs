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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_lpgen = new CLifePathGenerator();
            m_lpgen.Load();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

        }
    }
}
