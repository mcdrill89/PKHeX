﻿namespace PKHeX.Core
{
    internal sealed class EncounterStatic5N : EncounterStatic5
    {
        public readonly uint PID;
        public override Shiny Shiny { get; set; } = Shiny.FixedValue;
        public const bool NSparkle = true;

        internal EncounterStatic5N(uint pid) => PID = pid;

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pk.PersonalInfo);
            int nature = (int)Nature;
            int ability = Ability;

            pk.PID = PID;
            pk.Gender = gender;
            SetIVs(pk);

            pk.Nature = nature;
            pk.RefreshAbility(ability >> 1);
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (PID != pkm.PID)
                return false;
            return base.IsMatch(pkm, evo);
        }

        internal void SetNPokemonData(PK5 pk5, int lang)
        {
            pk5.IV_HP = pk5.IV_ATK = pk5.IV_DEF = pk5.IV_SPA = pk5.IV_SPD = pk5.IV_SPE = 30;
            pk5.NPokémon = NSparkle;
            pk5.OT_Name = Legal.GetG5OT_NSparkle(lang);
            pk5.TID = 00002;
            pk5.SID = 00000;
        }
    }
}