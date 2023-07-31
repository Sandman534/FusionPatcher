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
        public List<ModKey> Destructible = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> InventRemove = new();
        public List<ModKey> InventAdd = new();
        public List<ModKey> InventChange = new();
        public List<ModKey> Names = new();
        public List<ModKey> ObjectBounds = new();
        public List<ModKey> Scripts = new();
        public List<ModKey> Sounds = new();
    }

    internal class ContainerPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ContainerSettings Settings)
        {
            List<ModKey> modList = new() {
                Settings.Destructible, Settings.Graphics, Settings.InventRemove, Settings.InventAdd, Settings.InventChange, Settings.Names,
                Settings.ObjectBounds, Settings.Scripts, Settings.Sounds };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Container().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Destructible
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Destructible.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Destructible?.Equals(originalObject.Record.Destructible) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Destructible?.Equals(workingContext.Record.Destructible) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Destructible != null) overrideObject.Destructible?.DeepCopyIn(foundContext.Record.Destructible);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Graphics.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null) overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Invent Add
                //==============================================================================================================
                if (Settings.InventAdd.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.InventAdd.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<ContainerEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IContainer, IContainerGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Items != null)
                            foreach (var rec in patchRecord.Record.Items)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Items != null)
                            foreach (var rec in workingContext.Record.Items)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext)
                            if (context.Record.Items != null && context.Record.Items.Count > 0)
                                foreach (var rec in context.Record.Items)
                                    if (!overrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                                    {
                                        overrideObject.Add(rec.DeepCopy());
                                        Change = true;
                                    }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Remove
                //==============================================================================================================
                if (Settings.InventRemove.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.InventRemove.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<ContainerEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IContainer, IContainerGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Items != null)
                            foreach (var rec in patchRecord.Record.Items)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Items != null)
                            foreach (var rec in workingContext.Record.Items)
                                overrideObject.Add(rec.DeepCopy());

                        // Add All Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Items != null && context.Record.Items.Count > 0)
                                // Remove Items
                                if (originalObject.Record.Items != null && originalObject.Record.Items.Count > 0)
                                    foreach (var rec in originalObject.Record.Items)
                                        if (!context.Record.Items.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                                        {
                                            var oFoundRec = overrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
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
                            addedRecord.Items?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Change
                //==============================================================================================================
                if (Settings.InventChange.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.InventChange.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<ContainerEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IContainer, IContainerGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Items != null)
                            foreach (var rec in patchRecord.Record.Items)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Items != null)
                            foreach (var rec in workingContext.Record.Items)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Items != null && originalObject.Record.Items != null)
                                foreach (var rec in context.Record.Items)
                                    if (!originalObject.Record.Items.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey && x.Item.Count == rec.Item.Count).Any())
                                    {
                                        var oFoundRec = overrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                                        if (oFoundRec.First() != null && oFoundRec.First().Item.Count != rec.Item.Count)
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
                            addedRecord.Items?.SetTo(overrideObject);
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
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.ObjectBounds.Contains(context.ModKey)))
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

                //==============================================================================================================
                // Scripts
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Scripts.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.VirtualMachineAdapter?.Equals(originalObject.Record.VirtualMachineAdapter) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.VirtualMachineAdapter?.Equals(originalObject.Record.VirtualMachineAdapter) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.VirtualMachineAdapter != null) overrideObject.VirtualMachineAdapter?.DeepCopyIn(foundContext.Record.VirtualMachineAdapter);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Sounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.OpenSound.Equals(originalObject.Record.OpenSound)
                        || !foundContext.Record.CloseSound.Equals(originalObject.Record.CloseSound)
                    )
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.OpenSound.Equals(workingContext.Record.OpenSound)) Change = true;
                        if (!foundContext.Record.CloseSound.Equals(workingContext.Record.CloseSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.OpenSound != null) overrideObject.OpenSound.SetTo(foundContext.Record.OpenSound);
                            if (foundContext.Record.CloseSound != null) overrideObject.CloseSound.SetTo(foundContext.Record.CloseSound);
                        }
                        break;
                    }
                }

            }
        }
    }
}
