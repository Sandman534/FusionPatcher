using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Immutable;

namespace Fusion
{
    public class CellSettings
    {
        public List<ModKey> Flags = new();
        public List<ModKey> Lighting = new();
        public List<ModKey> Location = new();
        public List<ModKey> Music = new();
    }

    internal class CellPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, CellSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Flags,
                Settings.Lighting,
                Settings.Location,
                Settings.Music
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Cell().WinningContextOverrides(state.LinkCache))
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Flags
                //==============================================================================================================
                if (Settings.Flags.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Flags.Contains(context.ModKey)
                            && (context.Record.Flags != originalObject.Record.Flags))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        foreach (var flag in Enum.GetValues<Cell.Flag>())
                        {
                            if (lastObject.Flags.HasFlag(flag))
                                overrideObject.Flags |= flag;
                        }
                    }
                }

                //==============================================================================================================
                // Lighting
                //==============================================================================================================
                if (Settings.Lighting.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Lighting.Contains(context.ModKey)
                            && (context.Record.Lighting != originalObject.Record.Lighting
                                || context.Record.LightingTemplate != originalObject.Record.LightingTemplate
                                || context.Record.ImageSpace != originalObject.Record.ImageSpace))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Records
                        if (lastObject.Lighting != null && overrideObject.Lighting != lastObject.Lighting)
                            overrideObject.Lighting?.DeepCopyIn(lastObject.Lighting);
                        if (lastObject.LightingTemplate != null && overrideObject.LightingTemplate != lastObject.LightingTemplate)
                            overrideObject.LightingTemplate.SetTo(lastObject.LightingTemplate);
                        if (lastObject.ImageSpace != null && overrideObject.ImageSpace != lastObject.ImageSpace)
                            overrideObject.ImageSpace.SetTo(lastObject.ImageSpace);
                    }
                }

                var workingRecord = workingContext.GetOrAddAsOverride(state.PatchMod);

                //==============================================================================================================
                // Location
                //==============================================================================================================
                if (Settings.Location.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Location.Contains(context.ModKey)
                            && (context.Record.Location != originalObject.Record.Location
                                || context.Record.Regions != originalObject.Record.Regions))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;
                        if (lastObject.Location != null && overrideObject.Location != lastObject.Location)
                            overrideObject.Location.SetTo(lastObject.Location);

                        // Add All Records
                        foreach (var context in foundContext)
                        {
                            var listObject = context.Record;
                            if (listObject.Regions != null && listObject.Regions.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Regions != null && originalObject.Record.Regions.Count > 0 && overrideObject.Regions?.Count > 0)
                                    foreach (var rec in originalObject.Record.Regions)
                                        if (!listObject.Regions.Contains(rec) && overrideObject.Regions.Contains(rec))
                                            overrideObject.Regions.Remove(rec);

                                // Add Items
                                foreach (var rec in listObject.Regions)
                                    if (overrideObject.Regions != null && !overrideObject.Regions.Contains(rec))
                                        overrideObject.Regions.Add(rec);
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Music
                //==============================================================================================================
                if (Settings.Music.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Music.Contains(context.ModKey)
                            && (context.Record.Music != originalObject.Record.Music))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.Music != null && overrideObject.Music != lastObject.Music)
                            overrideObject.Music.SetTo(lastObject.Music);
                    }
                }
            }
        }
    }
}
