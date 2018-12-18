using System;
using System.Collections.Generic;

namespace Entity {
    [System.Serializable]
    public class Stat {
        private int m_hp;

        public Stat(int hp) {
            this.m_hp = hp;
        }

        public int SetDamage(int damage) {
            int cur = this.m_hp;
            this.m_hp = Math.Max(this.m_hp - damage, 0);

            return this.m_hp;
        }
    }
}
