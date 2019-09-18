using System;

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
        public CLifePath Lifepath = null;
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

        public String GetDescription()
        {
            String descText = String.Format("Name         : {0}", Name);
            if (!String.IsNullOrEmpty(Relationship))
            {
                descText += Environment.NewLine;
                descText += String.Format("Relationship : {0}", Relationship);
            }
            if (!String.IsNullOrEmpty(Origin))
            {
                descText += Environment.NewLine;
                descText += String.Format("Origin       : {0}", Origin);
            }
            if (!String.IsNullOrEmpty(Status))
            {
                descText += Environment.NewLine;
                descText += String.Format("Status       : {0}", Status);
            }
            if (!String.IsNullOrEmpty(Reaction))
            {
                descText += Environment.NewLine;
                descText += String.Format("Reaction     : {0}", Reaction);
            }

            return descText;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
