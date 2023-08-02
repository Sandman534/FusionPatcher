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
    internal class ContainerPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Invent.Remove,Invent.Add,Invent.Change,Keywords,Names" +
                ",ObjectBounds,Sounds,Scripts,Sounds");
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Destructible").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Graphics").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Model?.Equals(workingContext.Record.Model) ?? false) Change = true;

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
                if (Settings.TagCount("Invent.Add", out var FoundKeys) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IContainer, IContainerGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Containers NewContainer = new(patchRecord?.Record.Items, workingContext.Record.Items);
                        foreach (var context in foundContext)
                            NewContainer.Add(context.Record.Items);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Remove
                //==============================================================================================================
                if (Settings.TagCount("Invent.Remove", out var FoundKeys1) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys1.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IContainer, IContainerGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Containers NewContainer = new(patchRecord?.Record.Items, workingContext.Record.Items);
                        foreach (var context in foundContext.Reverse())
                            NewContainer.Remove(context.Record.Items, originalObject.Record.Items);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Change
                //==============================================================================================================
                if (Settings.TagCount("Invent.Change", out var FoundKeys2) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys2.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IContainer, IContainerGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Containers NewContainer = new(patchRecord?.Record.Items, workingContext.Record.Items);
                        foreach (var context in foundContext.Reverse())
                            NewContainer.Change(context.Record.Items, originalObject.Record.Items);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

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

                //==============================================================================================================
                // Scripts
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Scripts").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.VirtualMachineAdapter?.Equals(originalObject.Record.VirtualMachineAdapter) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.VirtualMachineAdapter?.Equals(workingContext.Record.VirtualMachineAdapter) ?? false) Change = true;

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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Sounds").Contains(context.ModKey)))
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
