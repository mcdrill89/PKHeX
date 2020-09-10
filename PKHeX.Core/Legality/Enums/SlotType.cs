﻿using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Wild Encounter data <see cref="EncounterSlot"/> Type
    /// </summary>
    /// <remarks>
    /// Different from <see cref="EncounterType"/>, this corresponds to the method that the <see cref="IEncounterable"/> may be encountered.</remarks>
    [Flags]
#pragma warning disable RCS1191 // Declare enum value as combination of names.
    public enum SlotType : byte
    {
        /// <summary>
        /// Default (un-assigned) encounter slot type.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Slot is encountered via Grass.
        /// </summary>
        Grass = 1,

        /// <summary>
        /// Slot is encountered via Surfing.
        /// </summary>
        Surf = 2,

        /// <summary>
        /// Slot is encountered via Old Rod (Fishing).
        /// </summary>
        Old_Rod = 3,

        /// <summary>
        /// Slot is encountered via Good Rod (Fishing).
        /// </summary>
        Good_Rod = 4,

        /// <summary>
        /// Slot is encountered via Super Rod (Fishing).
        /// </summary>
        Super_Rod = 5,

        /// <summary>
        /// Slot is encountered via Rock Smash.
        /// </summary>
        Rock_Smash = 6,

        /// <summary>
        /// Slot is encountered via Headbutt.
        /// </summary>
        Headbutt = 7,

        /// <summary>
        /// Slot is encountered via a Honey Tree.
        /// </summary>
        HoneyTree = 8,

        /// <summary>
        /// Slot is encountered via the Bug Catching Contest.
        /// </summary>
        BugContest = 9,

        HiddenGrotto = 10,
        GoPark = 11,
        FriendSafari = 12,
        Horde = 13,
        // Pokeradar = 14,
        SOS = 15,
        // always used as a modifier to another slot type

        Swarm = 1 << 7,
    }

    public static partial class Extensions
    {
        internal static bool IsFishingRodType(this SlotType t)
        {
            t &= (SlotType)0xF;
            return t == SlotType.Old_Rod || t == SlotType.Good_Rod || t == SlotType.Super_Rod;
        }

        internal static bool IsSweetScentType(this SlotType t)
        {
            return t switch
            {
                SlotType.Grass => true,
                SlotType.Surf => true,
                SlotType.BugContest => true,

                _ => false,
            };
        }

        public static Ball GetRequiredBallValueWild(this SlotType t, int gen, int loc)
        {
            return gen switch
            {
                3 when Locations.IsSafariZoneLocation3(loc) => Ball.Safari,
                4 when Locations.IsSafariZoneLocation4(loc) => Ball.Safari,
                4 when t == SlotType.BugContest => Ball.Sport,

                // Poké Pelago
                7 when loc == 30016 => Ball.Poke,

                _ => Ball.None,
            };
        }
    }
}
