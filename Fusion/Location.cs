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
    public class LocationSettings
    {
        public List<ModKey> Keywords = new();
        public List<ModKey> Names = new();
        public List<ModKey> Sounds = new();
        public List<ModKey> Stats = new();
    }

    internal class LocationPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, LocationSettings Settings)
        {
            List<ModKey> modList = new() { 
                Settings.Keywords, Settings.Names, Settings.Sounds, Settings.Stats };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Location().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.Keywords.Contains(context.ModKey) && ((!context.Record.Keywords?.Equals(originalObject.Record.Keywords) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<IFormLinkGetter<IKeywordGetter>> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<ILocation, ILocationGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Keywords != null)
                            foreach (var rec in patchRecord.Record.Keywords)
                                overrideObject.Add(rec);
                        else if (workingContext.Record.Keywords != null)
                            foreach (var rec in workingContext.Record.Keywords)
                                overrideObject.Add(rec);

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Keywords != null && context.Record.Keywords.Count > 0)
                                foreach (var rec in context.Record.Keywords)
                                    if (originalObject.Record.Keywords != null && !originalObject.Record.Keywords.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec);
                                        Change = true;
                                    }
                        }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Keywords != null && context.Record.Keywords.Count > 0)
                                if (originalObject.Record.Keywords != null && originalObject.Record.Keywords.Count > 0 && originalObject.Record.Keywords?.Count > 0)
                                    foreach (var rec in originalObject.Record.Keywords)
                                        if (!context.Record.Keywords.Contains(rec) && overrideObject.Contains(rec))
                                        {
                                            overrideObject.Remove(rec);
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Keywords?.SetTo(overrideObject);
                        }
                    }
                }

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
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Sounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Music.Equals(originalObject.Record.Music))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Music.Equals(workingContext.Record.Music)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Music != null) overrideObject.Music.SetTo(foundContext.Record.Music);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Stats.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ParentLocation.Equals(originalObject.Record.ParentLocation)
                        || !foundContext.Record.WorldLocationMarkerRef.Equals(originalObject.Record.WorldLocationMarkerRef)
                        || !foundContext.Record.WorldLocationRadius.Equals(originalObject.Record.WorldLocationRadius))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ParentLocation.Equals(originalObject.Record.ParentLocation)) Change = true;
                        if (!foundContext.Record.WorldLocationMarkerRef.Equals(originalObject.Record.WorldLocationMarkerRef))  Change = true;
                        if (!foundContext.Record.WorldLocationRadius.Equals(originalObject.Record.WorldLocationRadius))  Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.EditorID != null) overrideObject.EditorID = foundContext.Record.EditorID;
                            if (foundContext.Record.ParentLocation != null) overrideObject.ParentLocation.SetTo(foundContext.Record.ParentLocation);
                            if (foundContext.Record.WorldLocationMarkerRef != null) overrideObject.WorldLocationMarkerRef.SetTo(foundContext.Record.WorldLocationMarkerRef);
                            if (foundContext.Record.WorldLocationRadius != null) overrideObject.WorldLocationRadius = foundContext.Record.WorldLocationRadius;
                        }
                        break;
                    }
                }

            }
        }
    }
}
