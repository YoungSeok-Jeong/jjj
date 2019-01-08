using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entity {
    [System.Serializable]
    public class Buff {
        private BuffType m_type;
        private int m_value = 0;
        private int m_turn;

        public Buff(BuffType type, int turn, int value) {
            this.m_type = type;
            this.m_turn = turn;
            this.m_value = value;
        }

        public BuffType GetBuffType() {
            return this.m_type;
        }

        public int GetTurn() {
            return this.m_turn;
        }

        public void Merge(int turn, int value) {
            switch (this.m_type) {
                case BuffType.Defense:
                    this.m_value += value;
                    break;
                case BuffType.Poison:
                    this.m_turn += turn;
                    break;
            }
        }

        public int ApplyHeroPower(int power, List<BuffType> affectedBuff) {
            return power;
        }

        public int ApplyDecDamage(int orgDamage, List<BuffType> affectedBuff) {
            int retDamage = orgDamage;
            switch (this.m_type) {
                case BuffType.Defense:
                    //데미지 감소
                    retDamage = Math.Max(0, retDamage - this.m_value);
                    //밸류값 차감
                    this.m_value = Math.Max(0, this.m_value - retDamage);
                    affectedBuff.Add(this.m_type);
                    break;
            }

            return retDamage;
        }

        public int ApplyDotDamage(int maxHp, List<BuffType> affectedBuff) {
            switch (this.m_type) {
                case BuffType.Poison:
                    affectedBuff.Add(this.m_type);
                    return (int)(maxHp * 0.1);
                default:
                    return 0;
            }
        }

        public int ApplyDef(int orgDef, List<BuffType> affectedBuff) {
            int retDef = orgDef;

            return retDef;
        }

        public int ApplyEndTurnDamage(int orgDamage, int hp, List<BuffType> affectedBuff) {
            int retDamage = orgDamage;
            switch (this.m_type) {
                case BuffType.Poison:
                    retDamage += (int)(hp * BuffDefinition.PoisonDamageRate);
                    affectedBuff.Add(BuffType.Poison);
                    break;
            }

            return retDamage;
        }

        public bool Update() {
            this.m_turn--;
            if (this.m_turn <= 0) {
                return true;
            } else {
                return false;
            }
        }

        public static Buff Create(BuffType type, int turn, int value) {
            return new Buff(type, turn, value);
        }

        public static Buff Create(BuffType type, int turn) {
            return new Buff(type, turn, 0);
        }

        public override string ToString() {
            return this.m_type.ToString();
        }
    }

    public enum BuffType {
        Defense, Poison
    }
}
