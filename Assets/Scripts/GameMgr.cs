using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;

public class GameMgr : Singleton<GameMgr> {
    //singleton이지만 유니티 엔진 특성상 억지로 싱글턴화 시킨 것이기 때문에 멤버들을 static으로 선언할 필요는 없다.
    private List<GameObject> m_attackers = new List<GameObject>();
    private List<GameObject> m_defenders = new List<GameObject>();
    private readonly List<int> m_sequence = new List<int>();

    private bool isUpdate = false;
    private bool doneSim = false;

    public GameObject GetHeroObject(int idx) {
        foreach(GameObject o in this.m_attackers) {
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

    public void InitSequence() {
        //attacker, defender 순서로 sequence에 세팅
        foreach (GameObject o in this.m_attackers) {
            Hero h = o.GetComponent<Hero>();
            this.m_sequence.Add(h.GetIdx());
        }

        foreach (GameObject o in this.m_defenders) {
            Hero h = o.GetComponent<Hero>();
            this.m_sequence.Add(h.GetIdx());
        }
    }

    public HeroAction SelectCard(int i) {
        GameObject card = GameObject.Find("card_" + i);
        HeroAction c = card.GetComponent<HeroAction>();

        return c;
    }

    public void StartGame() {
        //게임 시작!

        this.isUpdate = true;
        this.doneSim = false;
        //업데이트가 true면 시뮬레이팅
        //GameMgr에서 시뮬레이팅을 돌리고, 연출이 필요한 hero들만 따로 추려서 모아주자
        //연출이 필요한 hero들의 업데이트가 끝나면 넥스트 스텝으로 넘어가는 방.

    }

    public Hero GetHeroScript(GameObject o) {
        return o.GetComponent<Hero>();
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

    public void Simulate() {
        foreach (int idx in this.m_sequence) {
            GameObject heroObject = this.GetHeroObject(idx);
            Hero heroScript = this.GetHeroScript(heroObject);

            //카드를 임의로 선택
            Card c = heroObject.GetComponent<Card>();
            string type = c.SelectCard(0);

            //선택된 카드의 정보를 불러와서 타겟 설정
            CardInfo selectedCard = CardList.Instance().GetCardInfo(type);
            List<Hero> targets = new List<Hero>();
            switch (selectedCard.target) {
                case TargetType.Single:
                    targets.Add(this.GetOpponentRandom(heroScript.IsAttacker()));
                    break;
                
                case TargetType.Random:
                    targets.Add(this.GetOpponentRandom(heroScript.IsAttacker()));
                    break;

                case TargetType.All:
                    break;
            }


            //타겟을 상대로 히어로 액션을 수행
            heroScript.Action(selectedCard, targets);

            //액션 결과의 플레이 데이터를 저장
        }

        //업데이트된 캐릭터 액션들을 하나씩 플레이
        //플레이는 Update에서 매프레임 해주어야 하지 않을까?
        //하나의 플레이가 다 끝나면 다음 것 플레이 하는 식으로
    }
    public void UpdateHero() {

    }
    // Use this for initialization
    void Start () {
        GameMgr.Instance().Log();
        
        GameObject parent = GameObject.Find("objects");

        //attacker 생성
        GameObject obj = Resources.Load("Char/hero") as GameObject;
        GameObject attacker = Instantiate(obj, parent.transform);
        attacker.name = "attacker";
        attacker.transform.position = new Vector3(-4.0f, 0.19f, 0);
        Hero attackerHero = this.GetHeroScript(attacker);
        attackerHero.SetAttacker(true);

        //임의 카드 추가
        Card attackerDeck = attacker.GetComponent<Card>();
        attackerDeck.Add("attack");
        attackerDeck.Add("defense");

        //defender 생성
        GameObject defender = Instantiate(obj, parent.transform);
        defender.name = "defender";
        defender.transform.position = new Vector3(4.0f, 0.19f, 0);
        defender.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        Hero defenderHero = this.GetHeroScript(attacker);
        defenderHero.SetAttacker(false);

        //임의 카드 추가
        Card defenderDeck = attacker.GetComponent<Card>();
        defenderDeck.Add("attack");
        defenderDeck.Add("defense");

        this.m_attackers.Add(attacker);
        this.m_defenders.Add(defender);


        this.InitSequence();
        /*
        HeroAction c = this.SelectCard(5);
        string type = c.GetAction();
        int value_1 = c.GetValue_1();
        int value_2 = c.GetValue_2();
        Debug.LogFormat("type={0}, value_1={1}, value_2={2}", type, value_1, value_2);
        */
    }

	// Update is called once per frame
	void Update () {

	}

    public void Log() {
        Debug.Log("oh....");
    }
}
