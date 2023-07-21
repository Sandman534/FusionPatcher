using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Immutable;


namespace Fusion
{
    public class PerkSettings
    {
        public List<ModKey> Description = new();
        public List<ModKey> Conditions = new();
    }

    internal class PerkPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, PerkSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Description,
                Settings.Conditions
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Perk().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IPerk, IPerkGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IPerk, IPerkGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Description
                //==============================================================================================================
                if (Settings.Description.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<IPerk, IPerkGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Description.Contains(context.ModKey)
                            && (context.Record.Description != originalObject.Record.Description))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        overrideObject.Description.Set(lastObject.Description.TargetLanguage, lastObject.Description.String);
                    }
                }

                //==============================================================================================================
                // Conditions
                //==============================================================================================================
                if (Settings.Conditions.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IPerk, IPerkGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Conditions.Contains(context.ModKey)
                            && (context.Record.Conditions != originalObject.Record.Conditions))
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
                            if (listObject.Conditions != null && listObject.Conditions.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Conditions != null && originalObject.Record.Conditions.Count > 0 && overrideObject.Conditions?.Count > 0)
                                    foreach (var rec in originalObject.Record.Conditions)
                                        if (!listObject.Conditions.Contains(rec) && overrideObject.Conditions.Contains(rec))
                                            overrideObject.Conditions.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.Conditions)
                                    if (overrideObject.Conditions != null && !overrideObject.Conditions.Contains(rec))
                                        overrideObject.Conditions.Add(rec.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
    }
}
