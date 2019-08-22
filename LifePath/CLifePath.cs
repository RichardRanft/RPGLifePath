using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePath
{
    class CLifePath
    {
        private List<CActor> m_parents;
        private List<CActor> m_siblings;
        private List<CActor> m_friends;
        private List<CActor> m_enemies;

        private String m_firstName;
        private String m_lastName;

        public String FamilyStatus = "Normal";
        public String LifeGoal = "";
        public String RomanceStatus = "";
        public CActor Lover;

        public String Name
        {
            get { return String.Format("{0} {1}", m_firstName, m_lastName); }
            set { updateName(value); }
        }

        public String FirstName
        {
            get { return m_firstName; }
            set { m_firstName = value; }
        }

        public String LastName
        {
            get { return m_lastName; }
            set { m_lastName = value; }
        }

        public String ParentStatus = "";
        public List<CActor> Parents { get { return m_parents; } }
        public List<CActor> Siblings { get { return m_siblings; } }
        public List<CActor> Friends { get { return m_friends; } }
        public List<CActor> Enemies { get { return m_enemies; } }

        public CLifePath()
        {
            m_parents = new List<CActor>();
            m_siblings = new List<CActor>();
            m_friends = new List<CActor>();
            m_enemies = new List<CActor>();
            Lover = new CActor();
        }

        public void AddParent(CActor actor)
        {
            m_parents.Add(actor);
        }

        public void AddSibling(CActor actor)
        {
            m_siblings.Add(actor);
        }

        public void AddFriend(CActor actor)
        {
            m_friends.Add(actor);
        }

        public void AddEnemy(CActor actor)
        {
            m_enemies.Add(actor);
        }

        private void updateName(string value)
        {
            String[] parts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            m_firstName = parts[0];
            if (parts.Length > 1)
                m_lastName = parts[1];
        }
    }
}
