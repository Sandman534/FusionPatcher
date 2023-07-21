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
    public class ContainerSettings
    {
        public List<ModKey> Graphics = new();
        public List<ModKey> Inventory = new();
    }

    internal class ContainerPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ContainerSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Graphics,
                Settings.Inventory
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Container().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Appearence
                //==============================================================================================================
                if (Settings.Graphics.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Graphics.Contains(context.ModKey)
                            && (context.Record.Model != originalObject.Record.Model))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Record
                        if (lastObject.Model != null && overrideObject.Model != lastObject.Model)
                            overrideObject.Model?.DeepCopyIn(lastObject.Model);
                    }
                }

                //==============================================================================================================
                // Inventory
                //==============================================================================================================
                if (Settings.Inventory.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Inventory.Contains(context.ModKey)
                            && (context.Record.Items != originalObject.Record.Items))
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
                            if (listObject.Items != null && listObject.Items.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Items != null && originalObject.Record.Items.Count > 0 && overrideObject.Items?.Count > 0)
                                    foreach (var rec in originalObject.Record.Items)
                                        if (!listObject.Items.Contains(rec) && overrideObject.Items.Contains(rec))
                                            overrideObject.Items.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.Items)
                                    if (overrideObject.Items != null && !overrideObject.Items.Contains(rec))
                                        overrideObject.Items.Add(rec.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
    }
}
