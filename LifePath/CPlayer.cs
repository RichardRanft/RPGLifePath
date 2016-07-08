using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePath
{
    class CPlayer : CActor
    {
        private List<CActor> m_parents;
        private List<CActor> m_siblings;
        private List<CActor> m_friends;
        private List<CActor> m_enemies;

        public CPlayer() : base()
        {
            m_parents = new List<CActor>();
            m_siblings = new List<CActor>();
            m_friends = new List<CActor>();
            m_enemies = new List<CActor>();
        }
    }
}
