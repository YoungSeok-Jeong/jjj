using System;
namespace Entity {
    [System.Serializable]
    public class CardInfo {
        public string name;
        public CardType type;
        public int value;
        public int energy;
        public TargetType target;
    }

    public enum CardType {
        Attack,
        Defense,
        Poison
    }
    public enum TargetType {
        Single
        , All
        , Random
    }
}
