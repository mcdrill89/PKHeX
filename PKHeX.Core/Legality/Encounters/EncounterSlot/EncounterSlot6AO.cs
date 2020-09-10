namespace PKHeX.Core
{
    public sealed class EncounterSlot6AO : EncounterSlot
    {
        public override int Generation => 6;

        public EncounterSlot6AO(EncounterArea6AO area, int species, int form, int min, int max) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
        }

        public bool Pressure { get; set; }
        public bool DexNav { get; set; }
        public bool WhiteFlute { get; set; }
        public bool BlackFlute { get; set; }

        public bool CanDexNav => Area.Type != SlotType.Rock_Smash;

        protected override void SetFormatSpecificData(PKM pk)
        {
            var pk6 = (PK6)pk;
            if (CanDexNav)
            {
                var baseSpec = EvoBase.GetBaseSpecies(pk);
                var eggMoves = MoveEgg.GetEggMoves(pk, baseSpec.Species, baseSpec.Form, Version);
                if (eggMoves.Length > 0)
                    pk6.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
            }
            pk6.SetRandomMemory6();
        }

        public override string GetConditionString(out bool valid)
        {
            valid = true;
            if (WhiteFlute) // Decreased Level Encounters
                return Pressure ? LegalityCheckStrings.LEncConditionWhiteLead : LegalityCheckStrings.LEncConditionWhite;
            if (BlackFlute) // Increased Level Encounters
                return Pressure ? LegalityCheckStrings.LEncConditionBlackLead : LegalityCheckStrings.LEncConditionBlack;
            if (DexNav)
                return LegalityCheckStrings.LEncConditionDexNav;

            return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
        }
    }
}