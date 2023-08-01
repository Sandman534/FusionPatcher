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
    internal class FactionPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Names,Relations.Remove,Relations.Add,Relations.Change");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Faction().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Names
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Names").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Name?.Equals(originalObject.Record.Name) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Name?.Equals(workingContext.Record.Name) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Name != null) overrideObject.Name?.Set(foundContext.Record.Name.TargetLanguage, foundContext.Record.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Relations Add
                //==============================================================================================================
                if (Settings.TagCount("Invent.Add", out var FoundKeys) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && ((!context.Record.Relations?.Equals(originalObject.Record.Relations) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IFaction, IFactionGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Relations NewContainer = new(patchRecord?.Record.Relations, workingContext.Record.Relations);
                        foreach (var context in foundContext)
                            NewContainer.Add(context.Record.Relations);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Relations?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Relations Remove
                //==============================================================================================================
                if (Settings.TagCount("Invent.Add", out var FoundKeys1) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys1.Contains(context.ModKey) && ((!context.Record.Relations?.Equals(originalObject.Record.Relations) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IFaction, IFactionGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Relations NewContainer = new(patchRecord?.Record.Relations, workingContext.Record.Relations);
                        foreach (var context in foundContext)
                            NewContainer.Remove(context.Record.Relations, originalObject.Record.Relations);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Relations?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Relations Change
                //==============================================================================================================
                if (Settings.TagCount("Invent.Add", out var FoundKeys2) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys2.Contains(context.ModKey) && ((!context.Record.Relations?.Equals(originalObject.Record.Relations) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IFaction, IFactionGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Relations NewContainer = new(patchRecord?.Record.Relations, workingContext.Record.Relations);
                        foreach (var context in foundContext)
                            NewContainer.Change(context.Record.Relations, originalObject.Record.Relations);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Relations?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

            }
        }
    }
}
