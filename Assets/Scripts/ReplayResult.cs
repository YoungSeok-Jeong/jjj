using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Replay {
    public class ReplayResult {

        private int m_target;
        private string m_action;
        private int m_value;

        public int GetTarget() {
            return this.m_target;
        }

        public string GetAction() {
            return this.m_action;
        }

        public int GetValue() {
            return this.m_value;
        }

        public ReplayResult(int target, string action, int value) {
            this.m_target = target;
            this.m_action = action;
            this.m_value = value;
        }

    }
}