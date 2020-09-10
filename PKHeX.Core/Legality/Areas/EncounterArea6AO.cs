﻿using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.ORAS"/> encounter area
    /// </summary>
    public sealed class EncounterArea6AO : EncounterArea
    {
        public static EncounterArea6AO[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea6AO[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea6AO(input[i], game);
            return result;
        }

        private EncounterArea6AO(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];

            Slots = ReadSlots(data);
        }

        private EncounterSlot6AO[] ReadSlots(byte[] data)
        {
            const int size = 4;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot6AO[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);
                ushort SpecForm = BitConverter.ToUInt16(data, offset);
                int species = SpecForm & 0x3FF;
                int form = SpecForm >> 11;
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot6AO(this, species, form, min, max);
            }

            return slots;
        }

        private const int FluteBoostMin = 4; // White Flute decreases levels.
        private const int FluteBoostMax = 4; // Black Flute increases levels.
        private const int DexNavBoost = 30; // Maximum DexNav chain

        private const int RandomForm = 31;

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    var boostMax = Type != SlotType.Rock_Smash ? DexNavBoost : FluteBoostMax;
                    const int boostMin = FluteBoostMin;
                    if (!slot.IsLevelWithinRange(pkm.Met_Level, boostMin, boostMax))
                        break;

                    if (slot.Form != evo.Form && slot.Form != RandomForm)
                        break;

                    var clone = (EncounterSlot6AO)slot.Clone();
                    MarkSlotDetails(pkm, clone, evo);
                    yield return clone;
                    break;
                }
            }
        }

        private static void MarkSlotDetails(PKM pkm, EncounterSlot6AO slot, EvoCriteria evo)
        {
            if (slot.LevelMin > evo.MinLevel)
                slot.WhiteFlute = true;
            if (slot.LevelMax + 1 <= evo.MinLevel && evo.MinLevel <= slot.LevelMax + FluteBoostMax)
                slot.BlackFlute = true;

            if (!slot.CanDexNav)
                return;

            if (slot.LevelMax != evo.MinLevel)
                slot.DexNav = true;
            if (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4)
                slot.DexNav = true;
        }
    }
}
