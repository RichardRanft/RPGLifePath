using System;
using System.Collections.Generic;
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
            return success;
        }
    }
}
