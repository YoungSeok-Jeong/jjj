using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;

public class Hero : MonoBehaviour {
    private int m_idx;
    private bool m_isAttacker;
    private int m_hp = 100;
    private Stat m_stat;

    [SerializeField]
    private bool m_isPlayable = false;

    public void Awake() {
        this.m_idx = IdxDispenser.GetIdx();
        this.m_stat = new Stat(this.m_hp);
    }

    public void Start() {

    }

    public bool IsPlayable() {
        return this.m_isPlayable;
    }

    public void SetAttacker(bool isAttacker) {
        this.m_isAttacker = isAttacker;
    }

    public bool IsAttacker() {
        return this.m_isAttacker;
    }

    public int GetIdx() {
        return this.m_idx;
    }

    public void Action(CardInfo cardInfo, List<Hero> targets) {
        string type = cardInfo.type;
        int value = cardInfo.value;

        foreach (Hero targetHero in targets) {
            switch (type) {
                case "attack":
                    int remainHp = targetHero.SetDamage(value);
                    break;
                case "defense":

                    break;
            }
        }
    }

    public int SetDamage(int damage) {
        return this.m_stat.SetDamage(damage);
    }
}