using System;

namespace AshenCitadel.Heroes
{
    public enum HeroRole { Vanguard, Destroyer, Support }

    [Serializable]
    public class HeroState
    {
        public string id;
        public string nameFa;
        public HeroRole role;
        public int level = 1;
        public int stars = 1;
        public float hp = 300;
        public float atk = 40;
        public float def = 30;
        public string skillFa;
        public bool owned;
    }
}
