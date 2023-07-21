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
    public class BookSettings
    {
        public List<ModKey> Stats = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
    }

    internal class BookPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, BookSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Stats,
                Settings.Graphics,
                Settings.Keywords
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Book().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                if (Settings.Stats.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Stats.Contains(context.ModKey)
                            && (context.Record.Name != originalObject.Record.Name))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.Name != null && overrideObject.Name != lastObject.Name) 
                            overrideObject.Name = lastObject.Name.ToString();
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                if (Settings.Graphics.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Graphics.Contains(context.ModKey)
                            && (context.Record.Model != originalObject.Record.Model
                                || context.Record.InventoryArt != originalObject.Record.InventoryArt))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.Model != null && overrideObject.Model != lastObject.Model) 
                            overrideObject.Model?.DeepCopyIn(lastObject.Model);
                        if (lastObject.InventoryArt != null && overrideObject.InventoryArt != lastObject.InventoryArt) 
                            overrideObject.InventoryArt.SetTo(lastObject.InventoryArt);
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey)
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
            }
        }
    }
}
