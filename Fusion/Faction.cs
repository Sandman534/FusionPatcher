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
        public List<ModKey> Names = new();
        public List<ModKey> RelationsRemove = new();
        public List<ModKey> RelationsAdd = new();
        public List<ModKey> RelationsChange = new();
    }

    internal class FactionPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, FactionSettings Settings)
        {
            List<ModKey> modList = new() { 
                Settings.Names, Settings.RelationsRemove, Settings.RelationsAdd, Settings.RelationsChange };
            HashSet<ModKey> workingModList = new(modList);

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
                foreach(var foundContext in modContext.Where(context => Settings.Names.Contains(context.ModKey)))
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
                if (Settings.RelationsAdd.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.RelationsAdd.Contains(context.ModKey) && ((!context.Record.Relations?.Equals(originalObject.Record.Relations) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<Relation> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IFaction, IFactionGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Relations != null)
                            foreach (var rec in patchRecord.Record.Relations)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Relations != null)
                            foreach (var rec in workingContext.Record.Relations)
                                overrideObject.Add(rec.DeepCopy());

                        // Add All Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Relations != null && context.Record.Relations.Count > 0)
                                foreach (var rec in context.Record.Relations)
                                    if (!overrideObject.Where(x => x.Equals(rec)).Any())
                                    {
                                        overrideObject.Add(rec.DeepCopy());
                                        Change = true;
                                    }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Relations?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Relations Remove
                //==============================================================================================================
                if (Settings.RelationsRemove.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.RelationsRemove.Contains(context.ModKey) && ((!context.Record.Relations?.Equals(originalObject.Record.Relations) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<Relation> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IFaction, IFactionGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Relations != null)
                            foreach (var rec in patchRecord.Record.Relations)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Relations != null)
                            foreach (var rec in workingContext.Record.Relations)
                                overrideObject.Add(rec.DeepCopy());

                        // Add All Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Relations != null && context.Record.Relations.Count > 0)
                                if (originalObject.Record.Relations != null && originalObject.Record.Relations.Count > 0)
                                    foreach (var rec in originalObject.Record.Relations)
                                        if (!context.Record.Relations.Where(x => x.Equals(rec)).Any())
                                        {
                                            var oFoundRec = overrideObject.Where(x => x.Equals(rec));
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
                            addedRecord.Relations.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Relations Change
                //==============================================================================================================
                if (Settings.RelationsChange.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.RelationsChange.Contains(context.ModKey) && ((!context.Record.Relations?.Equals(originalObject.Record.Relations) ?? false)));

                    // If we found records to modify
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<Relation> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IFaction, IFactionGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Relations != null)
                            foreach (var rec in patchRecord.Record.Relations)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Relations != null)
                            foreach (var rec in workingContext.Record.Relations)
                                overrideObject.Add(rec.DeepCopy());

                        // Add All Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Relations != null && context.Record.Relations.Count > 0)
                                foreach (var rec in context.Record.Relations)
                                    if (!(originalObject.Record.Relations?.Contains(rec) ?? false))
                                    {
                                        var oFoundRec = overrideObject.Where(x => x.Equals(rec));
                                        if (oFoundRec.Any() && (originalObject.Record.Relations?.Contains(oFoundRec.First()) ?? true))
                                        {
                                            overrideObject.Remove(oFoundRec.First());
                                            overrideObject.Add(rec.DeepCopy());
                                            Change = true;
                                        }
                                    }
                        }

                        // If changes were made, override and write bac
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Relations?.SetTo(overrideObject);
                        }
                    }
                }

            }
        }
    }
}
