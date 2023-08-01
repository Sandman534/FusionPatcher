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
    internal class LeveledNPCPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Relev,Delev,ObjectBounds");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.LeveledNpc().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<ILeveledNpc, ILeveledNpcGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<ILeveledNpc, ILeveledNpcGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Relev
                //==============================================================================================================
                if (Settings.TagCount("Relev", out var FoundKeys) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && ((!context.Record.Entries?.Equals(originalObject.Record.Entries) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<LeveledNpcEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<ILeveledNpc, ILeveledNpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Entries != null)
                            foreach (var rec in patchRecord.Record.Entries)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Entries != null)
                            foreach (var rec in workingContext.Record.Entries)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Entries != null && context.Record.Entries.Count > 0)
                                foreach (var rec in context.Record.Entries)
                                    if (rec.Data != null && rec.Data.Reference != null)
                                        if (!overrideObject.Where(x => x.Data != null && x.Data.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                                        {
                                            overrideObject.Add(rec.DeepCopy());
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Entries?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Delev
                //==============================================================================================================
                if (Settings.TagCount("Delev", out var FoundKeys1) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys1.Contains(context.ModKey) && ((!context.Record.Entries?.Equals(originalObject.Record.Entries) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<LeveledNpcEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<ILeveledNpc, ILeveledNpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Entries != null)
                            foreach (var rec in patchRecord.Record.Entries)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Entries != null)
                            foreach (var rec in workingContext.Record.Entries)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Entries != null && context.Record.Entries.Count > 0)
                                if (originalObject.Record.Entries != null && originalObject.Record.Entries.Count > 0)
                                    foreach (var rec in originalObject.Record.Entries)
                                        if (rec.Data != null && rec.Data.Reference != null)
                                            if (!context.Record.Entries.Where(x => x.Data != null && x.Data.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                                            {
                                                var oFoundRec = overrideObject.Where(x => x.Data != null && x.Data.Reference.FormKey == rec.Data.Reference.FormKey);
                                                if (oFoundRec.Any())
                                                {
                                                    overrideObject.Remove(oFoundRec.First());
                                                    Change = true;
                                                }
                                            }
                        }

                        // If changes were made, override and write bac
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Entries?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("ObjectBounds").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ObjectBounds?.Equals(originalObject.Record.ObjectBounds) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ObjectBounds?.Equals(workingContext.Record.ObjectBounds) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectBounds != null) overrideObject.ObjectBounds?.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

            }
        }
    }
}
