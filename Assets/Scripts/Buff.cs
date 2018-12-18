using System;
namespace Entity {
    [System.Serializable]
    public class Buff {
        private string m_name;
        private int m_turn;

        public Buff(string name, int turn) {
            this.m_name = name;
            this.m_turn = turn;
        }

        public string GetName() {
            return this.m_name;
        }

        public int GetTurn() {
            return this.m_turn;
        }
    }

    public enum BuffType {
        Guard
        , Poison
    }
}
