using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePath
{
    class CActor
    {
        public String Relationship = "";
        public String Status = "";
        public String Reaction = "";
        public String Origin = "";
        public String Location = "";
        public String Age = "";
        private String m_firstName;
        private String m_lastName;

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

        public CActor() { }

        private void updateName(string value)
        {
            String[] parts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            m_firstName = parts[0];
            if (parts.Length > 1)
                m_lastName = parts[1];
        }
    }
}
