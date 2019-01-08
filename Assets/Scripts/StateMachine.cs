using System;
using UnityEngine;
namespace Entity {
    public class StateMachine<T> {
        private T m_state;
        private T m_nextState;
        private int m_frame = 0;
        private float m_timer = 0;

        public StateMachine(T initState) {
            this.m_state = initState;
            this.m_nextState = initState;
        }

        public void Update() {
            if (!this.m_state.Equals(this.m_nextState)) {
                this.m_state = this.m_nextState;
                this.m_frame = 0;
                this.m_timer = 0.0f;
            } else {
                this.m_frame ++;
                this.m_timer += Time.deltaTime;
            }
        }

        public void ChangeState(T state) {
            this.m_nextState = state;
        }

        public bool IsFirstFrame() {
            return this.m_frame == 0;
        }

        public T GetState() {
            return this.m_state;
        }
    }
}
