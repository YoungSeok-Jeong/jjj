using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using Replay;
using System;
using Spine.Unity;
using Random = UnityEngine.Random;
using System.Text;

public class Hero : MonoBehaviour {
    public int m_idx;
    private bool m_isAttacker;
    private int m_maxHp = 100;
    private int m_hp = 100;
    private Stat m_stat;
    private int m_maxEnergy = 1;
    private int m_energy = 1;
    private int m_selectedCard = -1;
    private bool m_initTurn = false;

    [SerializeField]
    private List<Card> m_cardList = new List<Card>();
    private List<Buff> m_buffList = new List<Buff>();

    [SerializeField]
    private bool m_isPlayable = false;

    public void Awake() {
        this.m_idx = IdxDispenser.GetIdx();
        this.m_hp = this.m_maxHp;
        this.m_stat = new Stat(this.m_maxHp);
    }

    public void Start() {
    }

    public int GetHp() {
        return this.m_hp;
    }

    public int GetMaxHp() {
        return this.m_maxHp;
    }

    public void AddCard(string type) {
        this.m_cardList.Add(Card.Create(type));
    }

    private int DecideCard() {
        int max = this.m_cardList.Count;
        return Random.Range(0, max);
    }

    public Card SelectCard() {
        this.m_selectedCard = this.DecideCard();
        return this.m_cardList[this.m_selectedCard];
    }

    public Card UseCard() {
        Card c = this.m_cardList[this.m_selectedCard];
        string cardType = c.GetCardType();
        CardInfo info = CardList.Instance().GetCardInfo(cardType);
        int oldEnergy = this.m_energy;
        this.m_energy -= info.energy;
        this.m_selectedCard = -1;
        JLog.Log(JLog.LogCategory.Card, "{0}/{1} / energy {2}/{3} -> {4}/{3}"
        , this.GetIdx(), cardType, oldEnergy, this.m_maxEnergy, this.m_energy);

        return c;
    }

    public Card GetCard(int idx) {
        return this.m_cardList[idx];
    }

    public Card GetSelectedCard() {
        return this.m_cardList[this.m_selectedCard];
    }

    public void StartTurn() {

    }

    public bool IsInitTurn() {
        return this.m_initTurn;
    }

    public void StartMyTurn() {
        if (!this.m_initTurn) {
            this.m_energy = this.m_maxEnergy;
            this.UpdateBuff();
            m_initTurn = true;
        }
    }

    /*
     * 매 턴 도트데미지가 들어가는 디버프들로 데미지 계산
     */
    public void DamageFromBuff(List<ReplayResult> replays) {
        int damage = 0;
        List<BuffType> affectedBuff = new List<BuffType>();

        foreach (Buff buff in this.m_buffList) {
            damage += buff.ApplyDotDamage(this.m_maxHp, affectedBuff);
        }

        if (damage > 0) {
            this.m_hp -= damage;
            replays.Add(ReplayResult.Create(this.GetIdx(), 0.0f, ReplayAction.Damaged
                        , damage, affectedBuff, !this.IsAlive()));
            JLog.Log(JLog.LogCategory.Action, "{0}/debuff damaged!={1}/target={2}/affectedBuff = {3}"
            , this.GetIdx(), damage, this.ToString(), JLog.ToString<BuffType>(affectedBuff));
        }
    }

    public void EndMyTurn() {
        this.m_initTurn = false;
    }

    public void EndTurn() {

    }

    public void AddBuff(BuffType buffType, int turn, int value) {
        bool exist = false;
        foreach (Buff buff in this.m_buffList) {
            if (buff.GetBuffType().Equals(buffType)) {
                buff.Merge(turn, value);
                exist = true;
                break;
            }
        }

        if (!exist) {
            this.m_buffList.Add(Buff.Create(buffType, turn, value));
        }
    }

    public void UpdateBuff() {
        List<int> removeList = new List<int>();
        int cnt = this.m_buffList.Count;
        for (int i = 0; i < cnt; i++) {
            Buff buff = this.m_buffList[i];
            if (buff.Update()) {
                removeList.Add(i);
            }
        }

        cnt = removeList.Count;
        for (int i = cnt - 1; i >= 0; i--) {
            int idx = removeList[i];
            this.m_buffList.RemoveAt(idx);
        }
    }

    public bool IsAlive() {
        return this.m_hp > 0;
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

    public int GetHeroPower(int power, List<BuffType> affectedBuff) {
        int newPower = power;
        foreach (Buff buff in this.m_buffList) {
            newPower = buff.ApplyHeroPower(newPower, affectedBuff);
        }

        return newPower;
    }

    public int GetDamageToThisHero(int damage, List<BuffType> affectedBuff) {
        int newDamage = damage;
        foreach (Buff buff in this.m_buffList) {
            newDamage = buff.ApplyDecDamage(newDamage, affectedBuff);
        }

        return newDamage;
    }

    public int GetDefToThisHero(int def, List<BuffType> affectedBuff) {
        int newDef = def;
        foreach (Buff buff in this.m_buffList) {
            newDef = buff.ApplyDef(newDef, affectedBuff);
        }

        return newDef;
    }

    public void Action(CardInfo cardInfo, List<Hero> targets, List<ReplayResult> replays) {
        CardType type = cardInfo.type;
        int value = cardInfo.value;
        int energy = cardInfo.energy;
        //먼저 히어로 공격에 영향을 끼치는 것을 가지고 공격력을 구하자
        List<BuffType> affectedBuff = new List<BuffType>();

        foreach (Hero targetHero in targets) {
            List<BuffType> targetAffectedBuff = new List<BuffType>();
            switch (type) {
                case CardType.Attack:
                    //타겟의 버프를 분석해서 value(데미지)를 업데이트
                    int power = this.GetHeroPower(value, affectedBuff);
                    int oldHp = targetHero.GetHp();
                    int damage = targetHero.GetDamageToThisHero(power, targetAffectedBuff);
                    int remainHp = targetHero.SetDamage(damage);

                    //공격자의 리플레이 정보 입력
                    replays.Add(ReplayResult.Create(this.m_idx, ReplayAction.Attack, power, affectedBuff));
                    //방어자의 리플레이 정보 입력
                    replays.Add(ReplayResult.Create(targetHero.GetIdx(), 0.9f, ReplayAction.Damaged
                        , damage, targetAffectedBuff, !targetHero.IsAlive()));
                    JLog.Log(JLog.LogCategory.Action, "{0}/action={1}/target={2}/affected buff={3}"
                        , this.GetIdx(), type, targetHero.ToString(), JLog.ToString<BuffType>(targetAffectedBuff));

                    break;

                case CardType.Defense:
                    int def = targetHero.GetDefToThisHero(value, targetAffectedBuff);
                    this.AddBuff(BuffType.Defense, 1, def);

                    //리플레이 정보 입력
                    replays.Add(ReplayResult.Create(this.m_idx, ReplayAction.Defense, def, affectedBuff));
                    JLog.Log(JLog.LogCategory.Action, "{0}/{1}/def={2}/target={3}", this.GetIdx(), type, def, this.ToString());
                    break;

                case CardType.Poison:
                    targetHero.AddBuff(BuffType.Poison, value, 0);
                    //리플레이 정보 입력
                    replays.Add(ReplayResult.Create(this.m_idx, ReplayAction.Skill, 0, affectedBuff));
                    JLog.Log(JLog.LogCategory.Action, "{0}/{1}/alive turn={2},target={3}", this.GetIdx(), type, value, targetHero.ToString());
                    break;
            }
        }

        this.m_energy -= energy;
    }

    public int SetDamage(int damage) {
        int cur = this.m_hp;
        this.m_hp = Math.Max(this.m_hp - damage, 0);
        return this.m_hp;
    }

    public bool EnableToUseCard() {
        return this.m_energy > 0;
    }

    public override string ToString() {
        StringBuilder str = new StringBuilder();
        str.Append("[");
        str.Append(string.Format("idx={0}", this.GetIdx())); 
        str.Append(string.Format(",hp={0}/{1}", this.m_hp, this.m_maxHp));
        str.Append(string.Format(",buff={0}", JLog.ToString<Buff>(this.m_buffList)));
        str.Append("]");
        return str.ToString();
    }
}