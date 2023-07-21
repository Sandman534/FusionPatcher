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
    public class FactionSettings
    {
        public List<ModKey> Flags = new();
        public List<ModKey> Relations = new();
    }

    internal class FactionPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, FactionSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Flags,
                Settings.Relations
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Faction().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Flags
                //==============================================================================================================
                if (Settings.Flags.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey)
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
                        foreach (var flag in Enum.GetValues<Faction.FactionFlag>())
                        {
                            if (lastObject.Flags.HasFlag(flag))
                                overrideObject.Flags |= flag;
                        }
                    }
                }

                //==============================================================================================================
                // Relations
                //==============================================================================================================
                if (Settings.Relations.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Relations.Contains(context.ModKey)
                            && (context.Record.Relations != originalObject.Record.Relations))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);

                        // Add All Records
                        foreach (var context in foundContext)
                        {
                            var listObject = context.Record;
                            if (listObject.Relations != null && listObject.Relations.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Relations != null && originalObject.Record.Relations.Count > 0 && overrideObject.Relations?.Count > 0)
                                    foreach (var rec in originalObject.Record.Relations)
                                        if (!listObject.Relations.Contains(rec) && overrideObject.Relations.Contains(rec))
                                            overrideObject.Relations.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.Relations)
                                    if (overrideObject.Relations != null && !overrideObject.Relations.Contains(rec))
                                        overrideObject.Relations.Add(rec.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
    }
}
