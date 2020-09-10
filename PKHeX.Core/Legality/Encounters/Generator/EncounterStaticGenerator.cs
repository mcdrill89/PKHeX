﻿using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;
using static PKHeX.Core.Encounters1;
using static PKHeX.Core.Encounters2;
using static PKHeX.Core.Encounters3;
using static PKHeX.Core.Encounters4;
using static PKHeX.Core.Encounters5;
using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters7;
using static PKHeX.Core.Encounters7b;
using static PKHeX.Core.Encounters8;

namespace PKHeX.Core
{
    public static class EncounterStaticGenerator
    {
        public static IEnumerable<EncounterStatic> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var encounters = GetStaticEncounters(pkm, chain, gameSource);
            if (ParseSettings.AllowGBCartEra)
                return encounters;
            return encounters.Where(e => !GameVersion.GBCartEraOnly.Contains(e.Version));
        }

        public static IEnumerable<EncounterStatic> GetValidStaticEncounter(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            var poss = GetPossible(pkm, chain, gameSource: gameSource);

            // Back Check against pkm
            return GetMatchingStaticEncounters(pkm, poss, chain);
        }

        private static IEnumerable<EncounterStatic> GetMatchingStaticEncounters(PKM pkm, IEnumerable<EncounterStatic> poss, IReadOnlyList<DexLevel> evos)
        {
            // check for petty rejection scenarios that will be flagged by other legality checks
            var deferred = new List<EncounterStatic>();
            foreach (EncounterStatic e in poss)
            {
                foreach (var dl in evos)
                {
                    if (!e.IsMatch(pkm, dl))
                        continue;

                    if (e.IsMatchDeferred(pkm))
                        deferred.Add(e);
                    else
                        yield return e;
                    break;
                }
            }
            foreach (var e in deferred)
                yield return e;
        }

        private static IEnumerable<EncounterStatic> GetStaticEncounters(PKM pkm, IReadOnlyList<DexLevel> dl, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var table = GetEncounterStaticTable(pkm, gameSource);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }

        internal static EncounterStatic7 GetVCStaticTransferEncounter(PKM pkm, IEncounterable enc)
        {
            var species = pkm.Species;
            var met = pkm.Met_Level;
            if (pkm.VC1)
                return EncounterStatic7.GetVC1(species > MaxSpeciesID_1 ? enc.Species : species, met);
            if (pkm.VC2)
                return EncounterStatic7.GetVC2(species > MaxSpeciesID_2 ? enc.Species : species, met);

            // Should never reach here.
            throw new ArgumentException(nameof(pkm.Version));
        }

        internal static EncounterStatic? GetStaticLocation(PKM pkm, int species = -1)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                    return EncounterStatic7.GetVC1(species, pkm.Met_Level);
                case 2:
                    return EncounterStatic7.GetVC2(species, pkm.Met_Level);
                default:
                    var dl = EvolutionChain.GetValidPreEvolutions(pkm, maxLevel: 100, skipChecks: true);
                    return GetPossible(pkm, dl).FirstOrDefault();
            }
        }

        // Generation Specific Fetching
        private static IEnumerable<EncounterStatic> GetEncounterStaticTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GameVersion.RBY:
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.YW:
                    return StaticRBY;

                case GameVersion.GSC:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    return GetEncounterStaticTableGSC(pkm);

                case GameVersion.R: return StaticR;
                case GameVersion.S: return StaticS;
                case GameVersion.E: return StaticE;
                case GameVersion.FR: return StaticFR;
                case GameVersion.LG: return StaticLG;
                case GameVersion.CXD: return Encounter_CXD;

                case GameVersion.D: return StaticD;
                case GameVersion.P: return StaticP;
                case GameVersion.Pt: return StaticPt;
                case GameVersion.HG: return StaticHG;
                case GameVersion.SS: return StaticSS;

                case GameVersion.B: return StaticB;
                case GameVersion.W: return StaticW;
                case GameVersion.B2: return StaticB2;
                case GameVersion.W2: return StaticW2;

                case GameVersion.X: return StaticX;
                case GameVersion.Y: return StaticY;
                case GameVersion.AS: return StaticA;
                case GameVersion.OR: return StaticO;

                case GameVersion.SN: return StaticSN;
                case GameVersion.MN: return StaticMN;
                case GameVersion.US: return StaticUS;
                case GameVersion.UM: return StaticUM;

                case GameVersion.GP: return StaticGP;
                case GameVersion.GE: return StaticGE;

                case GameVersion.SW: return StaticSW;
                case GameVersion.SH: return StaticSH;

                default: return Enumerable.Empty<EncounterStatic>();
            }
        }

        private static IEnumerable<EncounterStatic> GetEncounterStaticTableGSC(PKM pkm)
        {
            if (!ParseSettings.AllowGen2Crystal(pkm))
                return StaticGS;
            if (pkm.Format != 2)
                return StaticGSC;

            if (pkm.HasOriginalMetLocation)
                return StaticC;
            return StaticGSC;
        }
    }
}
