﻿using System.Collections.Generic;
using static PKHeX.Core.BatchEditing;

namespace PKHeX.Core
{
    public static class BatchFilters
    {
        public static readonly List<IComplexFilter> FilterMods = new()
        {
            new ComplexFilter(PROP_LEGAL,
                (pkm, cmd) => new LegalityAnalysis(pkm).Valid == cmd.Evaluator,
                (info, cmd) => info.Legality.Valid == cmd.Evaluator),

            new ComplexFilter(PROP_TYPENAME,
                (pkm, cmd) => (pkm.GetType().Name == cmd.PropertyValue) == cmd.Evaluator,
                (info, cmd) => (info.Entity.GetType().Name == cmd.PropertyValue) == cmd.Evaluator),
        };

        public static readonly List<IComplexFilterMeta> FilterMeta = new()
        {
            new MetaFilter(IdentifierContains,
                (obj, cmd) => obj is SlotCache s && s.Identify().Contains(cmd.PropertyValue) == cmd.Evaluator),

            new MetaFilter(nameof(SlotInfoBox.Box),
                (obj, cmd) => obj is SlotCache { Source: SlotInfoBox b } && (b.Box.ToString() == cmd.PropertyValue) == cmd.Evaluator),

            new MetaFilter(nameof(ISlotInfo.Slot),
                (obj, cmd) => obj is SlotCache s && (s.Source.Slot.ToString() == cmd.PropertyValue) == cmd.Evaluator),
        };
    }
}