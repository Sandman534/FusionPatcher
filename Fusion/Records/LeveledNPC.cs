using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Strings;
using Noggog;

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
                // Leveled List
                //==============================================================================================================
                if (Settings.HasTags("Relev,Delev"))
                {
                    // Create list and fill it with Last Record or Patch Record
                    ExtendedList<LeveledNpcEntry> overrideObject = new();
                    if (workingContext.Record.Entries != null)
                        foreach (var rec in workingContext.Record.Entries)
                            overrideObject.Add(rec.DeepCopy());

                    bool Change = false;
                    if (Settings.HasTags("Relev", out var Relev))
                    {
                        // Get the last overriding context of our element
                        var foundContext = modContext.Where(context => Relev.Contains(context.ModKey) && Compare.NotEqual(context.Record.Entries,originalObject.Record.Entries));
                        if (foundContext.Any())
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
                    }

                    if (Settings.HasTags("Delev", out var Delev))
                    {
                        // Get the last overriding context of our element
                        var foundContext = modContext.Where(context => Delev.Contains(context.ModKey) && Compare.NotEqual(context.Record.Entries,originalObject.Record.Entries));
                        if (foundContext.Any())
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
                    }

                    // If changes were made, override and write back
                    if (Change)
                    {
                        var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                        addedRecord.Entries = overrideObject;
                    }
                }

                //==============================================================================================================
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("ObjectBounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

            }
        }
    }
}
