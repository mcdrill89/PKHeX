using System.Collections.Generic;

namespace PKHeX.Core
{
    internal sealed class EncounterSlot3Swarm : EncounterSlot3, IMoveset
    {
        public IReadOnlyList<int> Moves { get; }

        public EncounterSlot3Swarm(EncounterArea3 area, int species, int min, int max, int slot,
            IReadOnlyList<int> moves) : base(area, species, 0, min, max, slot, 0, 0, 0, 0) => Moves = moves;

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = Moves;
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }
}
