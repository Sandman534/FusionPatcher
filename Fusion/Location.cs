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
        public List<ModKey> Sounds = new();
        public List<ModKey> Stats = new();
    }

    internal class LocationPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, LocationSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Keywords,
                Settings.Sounds,
                Settings.Stats
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Location().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Keywords.Contains(context.ModKey) && context.Record.Keywords != originalObject.Record.Keywords)
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
                            if (listObject.Keywords != null && listObject.Keywords.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Keywords != null && originalObject.Record.Keywords.Count > 0 && overrideObject.Keywords?.Count > 0)
                                    foreach (var rec in originalObject.Record.Keywords)
                                        if (!listObject.Keywords.Contains(rec) && overrideObject.Keywords.Contains(rec))
                                            overrideObject.Keywords.Remove(rec);

                                // Add Items
                                foreach (var rec in listObject.Keywords)
                                    if (overrideObject.Keywords != null && !overrideObject.Keywords.Contains(rec))
                                        overrideObject.Keywords.Add(rec);
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                if (Settings.Sounds.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Sounds.Contains(context.ModKey)
                            && (context.Record.Music != originalObject.Record.Music))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Record
                        if (lastObject.Music != null && overrideObject.Music != lastObject.Music) 
                            overrideObject.Music.SetTo(lastObject.Music);
                    }
                }

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                if (Settings.Stats.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Stats.Contains(context.ModKey)
                            && (context.Record.Name != originalObject.Record.Name
                                || context.Record.ParentLocation != originalObject.Record.ParentLocation
                                || context.Record.WorldLocationMarkerRef != originalObject.Record.WorldLocationMarkerRef
                                || context.Record.WorldLocationRadius != originalObject.Record.WorldLocationRadius))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Record
                        if (lastObject.Name != null && overrideObject.Name != lastObject.Name)
                            overrideObject.Name = lastObject.Name.ToString();
                        if (lastObject.ParentLocation != null && overrideObject.ParentLocation != lastObject.ParentLocation)
                            overrideObject.ParentLocation.SetTo(lastObject.ParentLocation);
                        if (lastObject.WorldLocationMarkerRef != null && overrideObject.WorldLocationMarkerRef != lastObject.WorldLocationMarkerRef)
                            overrideObject.WorldLocationMarkerRef.SetTo(lastObject.WorldLocationMarkerRef);
                        if (lastObject.WorldLocationRadius != null && overrideObject.WorldLocationRadius != lastObject.WorldLocationRadius)
                            overrideObject.WorldLocationRadius = lastObject.WorldLocationRadius;
                    }
                }
            }
        }
    }
}
