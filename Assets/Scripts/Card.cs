using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entity {
    [System.Serializable]
    public class Card {
        [SerializeField]
        private int m_lv = 1;

        [SerializeField]
        private string m_type = "attack";


        public int GetLv() {
            return this.m_lv;
        }

        public string GetCardType() {
            return this.m_type;
        }

        private Card(int lv, string type) {
            this.m_lv = lv;
            this.m_type = type;
        }

        public static Card Create(int lv, string type) {
            return new Card(lv, type);
        }

        public static Card Create(string type) {
            return new Card(1, type);
        }
    }

}