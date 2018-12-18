using System;
using UnityEngine;


[System.Serializable]
public class HeroAction : MonoBehaviour {
    public string m_action = "attack";
    public int m_value_1 = 0;
    public int m_value_2 = 0;

    public HeroAction() {

    }
    public HeroAction(string action) {
        this.m_action = action;
    }

    public HeroAction(string action, int value_1) {
        this.m_action = action;
        this.m_value_1 = value_1;
    }

    public HeroAction(string action, int value_1, int value_2) {
        this.m_action = action;
        this.m_value_1 = value_1;
        this.m_value_2 = value_2;
    }

    public string GetAction() {
        return this.m_action;
    }

    public int GetValue_1() {
        return this.m_value_1;
    }

    public int GetValue_2() {
        return this.m_value_2;
    }
}
