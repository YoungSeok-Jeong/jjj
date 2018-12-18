using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Entity;

public class CardList : Singleton<CardList> {
    public List<CardInfo> cards = new List<CardInfo>();

    public CardInfo GetCardInfo(string name) {
        foreach (CardInfo c in this.cards) {
            if (c.name.Equals(name)) {
                return c;
            }
        }

        return null;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    [ContextMenu("AddCard")]
    public void AddCard() {
        this.cards.Add(new CardInfo());
    }
}

