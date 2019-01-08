using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using Replay;

public class GameMgr : Singleton<GameMgr> {

    #region base member datas
    //state machine
    private StateMachine<GameState> m_sm = new StateMachine<GameState>(GameState.Prepare);
    #endregion

    #region playing game data members
    //singleton이지만 유니티 엔진 특성상 억지로 싱글턴화 시킨 것이기 때문에 멤버들을 static으로 선언할 필요는 없다.
    private List<GameObject> m_attackers = new List<GameObject>();
    private List<GameObject> m_defenders = new List<GameObject>();
    private List<int> m_sequence = new List<int>(); //공격 순서
    private Side m_winner = Side.None;              //승자
    #endregion

    #region replay members
    private bool m_isReplaying = false;             //현재 리플레이 재생 중인지의 여부
    private List<ReplayResult> m_replays = new List<ReplayResult>();
    #endregion


    #region get target functions
    private List<Hero> GetTargets(TargetType targetType, bool isAttacker) {
        List<Hero> ret = new List<Hero>();
        switch (targetType) {
            case TargetType.Single:
                ret.Add(this.GetOpponentRandom(isAttacker));
                break;

            case TargetType.Random:
                ret.Add(this.GetOpponentRandom(isAttacker));
                break;

            case TargetType.All:
                break;
        }

        return ret;
    }

    public Hero GetOpponentByIdx(bool isAttacker, int idx) {
        if (isAttacker) {
            foreach (GameObject o in this.m_attackers) {
                Hero hero = this.GetHeroScript(o);
                if (hero.GetIdx() == idx) {
                    return hero;
                }
            }
        } else {
            foreach (GameObject o in this.m_defenders) {
                Hero hero = this.GetHeroScript(o);
                if (hero.GetIdx() == idx) {
                    return hero;
                }
            }
        }

        return null;
    }

    public Hero GetOpponentRandom(bool isAttacker) {
        if (isAttacker) {
            int idx = Random.Range(0, this.m_defenders.Count - 1);
            GameObject o = this.m_defenders[idx];
            return this.GetHeroScript(o);
        } else {
            int idx = Random.Range(0, this.m_attackers.Count - 1);
            GameObject o = this.m_attackers[idx];
            return this.GetHeroScript(o);
        }
    }
    #endregion

    #region get object&component functions
    public GameObject GetHeroObject(int idx) {
        foreach (GameObject o in this.m_attackers) {
            Hero h = o.GetComponent<Hero>();
            if (h.GetIdx() == idx) {
                return o;
            }
        }

        foreach (GameObject o in this.m_defenders) {
            Hero h = o.GetComponent<Hero>();
            if (h.GetIdx() == idx) {
                return o;
            }
        }

        return null;
    }

    public Hero GetCurrentHeroScript() {
        int idx = this.m_sequence[0];
        return this.GetHeroScript(idx);
    }

    public Hero GetHeroScript(GameObject o) {
        return o.GetComponent<Hero>();
    }

    public Actor GetActorScript(GameObject o) {
        return o.GetComponent<Actor>();
    }

    public Hero GetHeroScript(int idx) {
        foreach (GameObject o in this.m_attackers) {
            Hero hero = this.GetHeroScript(o);
            if (hero.GetIdx() == idx) {
                return hero;
            }
        }

        foreach (GameObject o in this.m_defenders) {
            Hero hero = this.GetHeroScript(o);
            if (hero.GetIdx() == idx) {
                return hero;
            }
        }

        return null;
    }

    public Actor GetActorScript(int idx) {
        foreach (GameObject o in this.m_attackers) {
            Actor actor = this.GetActorScript(o);
            Hero hero = this.GetHeroScript(o);
            if (hero.GetIdx() == idx) {
                return actor;
            }
        }

        foreach (GameObject o in this.m_defenders) {
            Actor actor = this.GetActorScript(o);
            Hero hero = this.GetHeroScript(o);
            if (hero.GetIdx() == idx) {
                return actor;
            }
        }

        return null;
    }
    #endregion

    #region state funcions
    public void StartGlobalTurn() {
        //공격자의 턴 준비
        foreach (GameObject heroObject in this.m_attackers) {
            Hero heroScript = this.GetHeroScript(heroObject);
            if (heroScript.IsAlive()) {
                heroScript.StartTurn();
            }
        }
        //방어자의 턴 준비
        foreach (GameObject heroObject in this.m_defenders) {
            Hero heroScript = this.GetHeroScript(heroObject);
            if (heroScript.IsAlive()) {
                heroScript.StartTurn();
            }
        }
        //공격 순서 설정
        this.InitSequence();

        //스테이트 전환
        this.m_sm.ChangeState(GameState.PrepareTurn);
    }

    /*
     * 이번 순서 히어로의 턴을 준비
     */
    public void PrepareTurn() {
        Hero heroScript = this.GetCurrentHeroScript();
        if (heroScript.IsAlive()) {
            heroScript.DamageFromBuff(this.m_replays);
            heroScript.StartMyTurn();
            if (this.m_replays.Count > 0) {
                this.m_sm.ChangeState(GameState.Replay);
            } else {
                this.m_sm.ChangeState(GameState.Decide);
            }
        } else {
            this.m_sm.ChangeState(GameState.NextHero);
        }

    }

    public void Decide() {
        Hero heroScript = this.GetCurrentHeroScript();
        if (heroScript.EnableToUseCard()) {
            heroScript.SelectCard();
            this.m_sm.ChangeState(GameState.Simulate);
        } else {
            this.m_sm.ChangeState(GameState.NextHero);
        }
    }

    public void Simulate() {
        //첫 순서의 영웅을 불러온다
        Hero heroScript = this.GetCurrentHeroScript();

        //해당 영웅이 선택한 카드의 정보를 불러와서 타겟 설정
        Card selectedCard = heroScript.GetSelectedCard();
        CardInfo selectedCardInfo = CardList.Instance().GetCardInfo(selectedCard.GetCardType());

        List<Hero> targets = this.GetTargets(selectedCardInfo.target, heroScript.IsAttacker());

        //카드 사용
        heroScript.UseCard();

        //타겟을 상대로 히어로 액션을 수행
        heroScript.Action(selectedCardInfo, targets, this.m_replays);

        //리플레이 할 것이 있다면 Replay로 스테이트 전환
        this.m_sm.ChangeState(GameState.Replay);
    }

    public void Replay() {
        bool isDone = true;
        foreach (ReplayResult replay in this.m_replays) {
            if (!replay.IsPlaying()) {
                int idx = replay.GetOwner();
                Actor actorScript = this.GetActorScript(idx);
                this.m_isReplaying = true;
                actorScript.Performance(replay);
                replay.SetPlaying();
            }

            if (!replay.IsDonePlaying()) {
                isDone = false;
            }
        }

        if (isDone) {
            if (this.CheckAllDead(false)) {
                this.m_winner = Side.Attacker;
                this.m_sm.ChangeState(GameState.Finish);
            } else if (this.CheckAllDead(true)) {
                this.m_winner = Side.Defender;
                this.m_sm.ChangeState(GameState.Finish);
            } else {
                this.m_sm.ChangeState(GameState.Decide);
            }
        }
    }

    public void NextHero() {
        Hero heroScript = this.GetCurrentHeroScript();
        heroScript.EndMyTurn();
        this.m_sequence.RemoveAt(0);
        if (this.m_sequence.Count > 0) {
            this.m_sm.ChangeState(GameState.PrepareTurn);
        } else {
            this.m_sm.ChangeState(GameState.EndGlobalTurn);
        }
    }

    public void EndGlobalTurn() {
        foreach (GameObject o in this.m_attackers) {
            Hero heroScript = this.GetHeroScript(o);
            heroScript.EndTurn();
        }

        foreach (GameObject o in this.m_defenders) {
            Hero heroScript = this.GetHeroScript(o);
            heroScript.EndTurn();
        }

        this.m_sm.ChangeState(GameState.StartGlobalTurn);
    }

    public void Finish() {
        if (this.m_sm.IsFirstFrame()) {
            UI_Game.SetActive("Button_ok", true);
            if (this.m_winner == Side.Attacker) {
                UI_Game.SetButtonText("Button_ok", "attacker win!");
            } else {
                UI_Game.SetButtonText("Button_ok", "defender win!");
            }
        }
    }
    #endregion

    #region playing game functions
    public void InitSequence() {
        //attacker, defender 순서로 sequence에 세팅
        foreach (GameObject o in this.m_attackers) {
            Hero h = o.GetComponent<Hero>();
            if (h.IsAlive()) {
                this.m_sequence.Add(h.GetIdx());
            }
        }

        foreach (GameObject o in this.m_defenders) {
            Hero h = o.GetComponent<Hero>();
            if (h.IsAlive()) {
                this.m_sequence.Add(h.GetIdx());
            }
        }
    }

    public void ReplayDone() {
        this.m_isReplaying = false;
        this.m_replays.RemoveAt(0);
    }

    public bool CheckAllDead(bool isAttacker) {
        if (isAttacker) {
            foreach (GameObject o in this.m_attackers) {
                Hero hero = this.GetHeroScript(o);
                if (hero.IsAlive()) {
                    return false;
                }
            }

            return true;

        } else {
            foreach (GameObject o in this.m_defenders) {
                Hero hero = this.GetHeroScript(o);
                if (hero.IsAlive()) {
                    return false;
                }
            }

            return true;
        }
    }
    #endregion

    #region unity message functions
    void Start() {
        UI_Game.SetActive("Button_ok", false);

        GameObject parentAttackers = GameObject.Find("attackers");
        GameObject parentDefenders = GameObject.Find("defenders");

        //attacker 생성
        GameObject obj = Resources.Load("Char/hero") as GameObject;
        GameObject attacker = Instantiate(obj, parentAttackers.transform);
        attacker.name = "attacker";
        attacker.transform.localPosition = new Vector3(0.0f, 0.19f, -4.0f);
        Hero attackerHero = this.GetHeroScript(attacker);
        attackerHero.SetAttacker(true);

        //임의 카드 추가
        attackerHero.AddCard("attack");
        attackerHero.AddCard("attack");
        attackerHero.AddCard("attack");
        attackerHero.AddCard("defense");
        attackerHero.AddCard("poison");
        attackerHero.AddCard("poison");

        //defender 생성
        GameObject defender = Instantiate(obj, parentDefenders.transform);
        defender.name = "defender";
        defender.transform.localPosition = new Vector3(0.0f, 0.19f, -4.0f);
        Hero defenderHero = this.GetHeroScript(defender);
        defenderHero.SetAttacker(false);

        //임의 카드 추가
        //defenderHero.AddCard("attack_2");
        defenderHero.AddCard("attack");
        defenderHero.AddCard("defense");

        this.m_attackers.Add(attacker);
        this.m_defenders.Add(defender);
    }

    // Update is called once per frame
    void Update() {
        //JLog.Simple("{0}", this.m_sm.GetState().ToString());
        switch (this.m_sm.GetState()) {
            case GameState.Prepare:
                this.m_sm.ChangeState(GameState.StartGlobalTurn);
                break;
            case GameState.StartGlobalTurn:
                this.StartGlobalTurn();
                break;
            case GameState.PrepareTurn:
                this.PrepareTurn();
                break;
            case GameState.Decide:
                this.Decide();
                break;
            case GameState.Simulate:
                this.Simulate();
                break;
            case GameState.Replay:
                this.Replay();
                break;
            case GameState.NextHero:
                this.NextHero();
                break;
            case GameState.EndGlobalTurn:
                this.EndGlobalTurn();
                break;
            case GameState.Finish:
                this.Finish();
                break;
        }

        this.m_sm.Update();
        //JLog.Simple("***** finish! winner = {0}", this.m_winner);
    }
    #endregion

}

namespace Entity {
    public enum GameState {
        Prepare, StartGlobalTurn, PrepareTurn, Decide, Simulate, Replay
            , NextHero, EndGlobalTurn, Finish
    }

    public enum Side {
        None, Attacker, Defender
    }
}