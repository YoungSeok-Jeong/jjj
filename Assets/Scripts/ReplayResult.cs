using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Entity;
using System.Text;

namespace Replay {
    public class ReplayResult {

        #region members
        private int m_owner;            //리플레이를 할 히어로의 idx
        private float m_startTime;      //언제 재생을 시작할 지
        private ReplayAction m_action;  //어떤 카드를 사용한 것인지
        private int m_value;            //공격이나 방어에 밸류 값
        private bool m_isDead;          //사망 여부
        private List<BuffType> m_affectedBuff = new List<BuffType>();
        private bool m_isPlaying = false;
        private bool m_donePlaying = false;
        #endregion

        #region getter functions
        public int GetOwner() {
            return this.m_owner;
        }

        public float GetStartTime() {
            return this.m_startTime;
        }

        public ReplayAction GetAction() {
            return this.m_action;
        }

        public int GetValue() {
            return this.m_value;
        }

        public List<BuffType> GetAffectedBuff() {
            return this.m_affectedBuff;
        }
        #endregion

        #region base functions
        public void SetPlaying() {
            this.m_isPlaying = true;
        }

        public bool IsPlaying() {
            return this.m_isPlaying;
        }

        public void DonePlaying() {
            this.m_donePlaying = true;
        }

        public bool IsDonePlaying() {
            return this.m_donePlaying;
        }

        public bool isDead() {
            return this.m_isDead;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("owner=" + this.m_owner + " / action=" + this.m_action.ToString());
            sb.Append(" / value=" + this.m_value + " / buff=" + JLog.ToString<BuffType>(this.m_affectedBuff));
            return sb.ToString();
        }

        private ReplayResult(int owner, float startTime, ReplayAction action, int value, List<BuffType> affectedBuff, bool isDead) {
            this.m_owner = owner;
            this.m_startTime = startTime;
            this.m_action = action;
            this.m_value = value;
            this.m_isDead = isDead;
            if (affectedBuff != null) {
                foreach (BuffType buff in affectedBuff) {
                    this.m_affectedBuff.Add(buff);
                }
            }
        }
        #endregion

        #region create functions
        public static ReplayResult Create(int owner, float startTime, ReplayAction action, int value, List<BuffType> affectedBuff, bool isDead) {
            return new ReplayResult(owner, startTime, action, value, affectedBuff, isDead);
        }

        public static ReplayResult Create(int owner, float startTime, ReplayAction action, int value, List<BuffType> affectedBuff) {
            return new ReplayResult(owner, startTime, action, value, affectedBuff, false);
        }

        public static ReplayResult Create(int owner, ReplayAction action, int value, List<BuffType> affectedBuff) {
            return new ReplayResult(owner, 0.0f, action, value, affectedBuff, false);
        }

        public static ReplayResult Create(int owner, ReplayAction action, int value) {
            return new ReplayResult(owner, 0.0f, action, value, null, false);
        }

        public static ReplayResult Create(int owner, ReplayAction action) {
            return new ReplayResult(owner, 0.0f, action, 0, null, false);
        }
        #endregion

    }

    public enum ReplayAction {
        Attack, Defense, Damaged, Skill
    }
}