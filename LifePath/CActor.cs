using System;
using System.IO;

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

        public void Save(CActor main = null)
        {
            String player = "";
            if (main != null)
            {
                String folder = String.Format("{0}_{1}", main.FirstName, main.LastName);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                player = folder + "\\";
            }
            String outfile = String.Format("{0}{1}_{2}.txt", player, FirstName, LastName);
            using (StreamWriter sw = new StreamWriter(outfile))
            {
                sw.WriteLine("--------");
                sw.WriteLine(Name);
                sw.WriteLine("--------");
                sw.WriteLine(" - Family -");
                sw.WriteLine("Parents: {0}", Lifepath.ParentStatus);
                foreach (CActor actor in Lifepath.Parents)
                {
                    sw.WriteLine("Name : {0}", actor.Name);
                    sw.WriteLine("-");
                }
                if (Lifepath.Siblings.Count > 0)
                {
                    sw.WriteLine("Siblings:");
                    foreach (CActor actor in Lifepath.Siblings)
                    {
                        sw.WriteLine("Name         : {0}", actor.Name);
                        sw.WriteLine("Relationship : {0}", actor.Relationship);
                        sw.WriteLine("-");
                    }
                }
                sw.WriteLine("Family Status: {0}", Lifepath.FamilyStatus);
                if (Lifepath.FamilyStatus != "Normal")
                    sw.WriteLine("Life Goal: {0}", Lifepath.LifeGoal);
                sw.WriteLine("--------");
                sw.WriteLine(" - Friends and Enemies -");
                sw.WriteLine("Friends:");
                foreach (CActor actor in Lifepath.Friends)
                {
                    sw.WriteLine("Name         : {0}", actor.Name);
                    sw.WriteLine("Relationship : {0}", actor.Relationship);
                    sw.WriteLine("-");
                }
                sw.WriteLine("Enemies:");
                foreach (CActor actor in Lifepath.Enemies)
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
                sw.WriteLine("Status       : {0}", Lifepath.RomanceStatus);
                if (!String.IsNullOrEmpty(Lifepath.Lover.FirstName))
                {
                    sw.WriteLine("Name         : {0}", Lifepath.Lover.Name);
                    sw.WriteLine("Relationship : {0}", Lifepath.Lover.Relationship);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
