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
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Destructible").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                overrideObject.Destructible = foundContext.Record.Destructible?.DeepCopy();
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Graphics").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                        
                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                overrideObject.Model = foundContext.Record.Model?.DeepCopy();
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Inventory
                //==============================================================================================================
                if (Settings.HasTags("Invent.Add,Invent.Change,Invent.Remove"))
                {
                    Containers NewContainer = new(workingContext.Record.Items);
                    if (Settings.HasTags("Invent.Add", out var InventAdd))
                    {
                        var InventAddContext = modContext.Where(context => InventAdd.Contains(context.ModKey) && Compare.NotEqual(context.Record.Items,originalObject.Record.Items));
                        if (InventAddContext.Any())
                            foreach (var context in InventAddContext)
                                NewContainer.Add(context.Record.Items);
                    }

                    if (Settings.HasTags("Invent.Change", out var InventChange))
                    {
                        // Get the last overriding context of our element
                        var foundContext = modContext.Where(context => InventChange.Contains(context.ModKey) && Compare.NotEqual(context.Record.Items,originalObject.Record.Items));
                        if (foundContext.Any())
                            foreach (var context in foundContext.Reverse())
                                NewContainer.Change(context.Record.Items, originalObject.Record.Items);
                    }

                    if (Settings.HasTags("Invent.Remove", out var InventRemove))
                    {
                        var foundContext = modContext.Where(context => InventRemove.Contains(context.ModKey) && Compare.NotEqual(context.Record.Items,originalObject.Record.Items));
                        if (foundContext.Any())
                            foreach (var context in foundContext.Reverse())
                                NewContainer.Remove(context.Record.Items, originalObject.Record.Items);
                    }

                    if (NewContainer.Modified) {
                        var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                        addedRecord.Items = NewContainer.OverrideObject;
                    }
                }

                //==============================================================================================================
                // Names
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Names").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                        }
                        break;
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
                            if (foundContext.Record.ObjectBounds != null && Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                overrideObject.ObjectBounds = foundContext.Record.ObjectBounds.DeepCopy();
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Scripts
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Scripts").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.VirtualMachineAdapter,originalObject.Record.VirtualMachineAdapter))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.VirtualMachineAdapter,workingContext.Record.VirtualMachineAdapter)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.VirtualMachineAdapter,originalObject.Record.VirtualMachineAdapter))
                                overrideObject.VirtualMachineAdapter = foundContext.Record.VirtualMachineAdapter?.DeepCopy();
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Sounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.OpenSound,originalObject.Record.OpenSound)
                        || Compare.NotEqual(foundContext.Record.CloseSound,originalObject.Record.CloseSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.OpenSound,workingContext.Record.OpenSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.CloseSound,workingContext.Record.CloseSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.OpenSound,originalObject.Record.OpenSound))
                                overrideObject.OpenSound.SetTo(foundContext.Record.OpenSound);
                            if (Compare.NotEqual(foundContext.Record.CloseSound,originalObject.Record.CloseSound)) 
                                overrideObject.CloseSound.SetTo(foundContext.Record.CloseSound);
                        }
                        break;
                    }
                }

            }
        }
    }
}
