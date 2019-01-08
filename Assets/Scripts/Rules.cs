using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity {
    public class Rules {
        public List<Hero> GetTargets(TargetType targetType, bool isAttacker) {
            List<Hero> ret = new List<Hero>();
            switch (targetType) {
                case TargetType.Single:
                    //ret.Add(this.GetOpponentRandom(isAttacker));
                    break;

                case TargetType.Random:
                    //ret.Add(this.GetOpponentRandom(isAttacker));
                    break;

                case TargetType.All:
                    break;
            }

            return ret;
        }

        /*
        public static bool CheckAllDead(bool isAttacker) {
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
        */
    }
}
