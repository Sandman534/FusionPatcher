using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
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

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(workingContext.Record.FormKey).Last();
                bool[] mapped = new bool[20];
                Containers NewContainer = new(workingContext.Record.Items);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (Settings.TagList("Destructible").Contains(foundContext.ModKey) && !mapped[0])
                    {
                        if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
                            else 
                            {
                                if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                        overrideObject.Destructible = foundContext.Record.Destructible?.DeepCopy();
                                }
                                mapped[0] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Settings.TagList("Graphics").Contains(foundContext.ModKey) && !mapped[1])
                    {
                        if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[1] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                                
                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                        overrideObject.Model = foundContext.Record.Model?.DeepCopy();
                                }
                                mapped[1] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Settings.TagList("Names").Contains(foundContext.ModKey) && !mapped[2])
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[2] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                        overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                                }
                                mapped[2] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (Settings.TagList("ObjectBounds").Contains(foundContext.ModKey) && !mapped[3])
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[3] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (foundContext.Record.ObjectBounds != null && Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                        overrideObject.ObjectBounds = foundContext.Record.ObjectBounds.DeepCopy();
                                }
                                mapped[3] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Scripts
                    //==============================================================================================================
                    if (Settings.TagList("Scripts").Contains(foundContext.ModKey) && !mapped[4])
                    {
                        if (Compare.NotEqual(foundContext.Record.VirtualMachineAdapter,originalObject.Record.VirtualMachineAdapter))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[4] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.VirtualMachineAdapter,workingContext.Record.VirtualMachineAdapter)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.VirtualMachineAdapter,originalObject.Record.VirtualMachineAdapter))
                                        overrideObject.VirtualMachineAdapter = foundContext.Record.VirtualMachineAdapter?.DeepCopy();
                                }
                                mapped[4] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (Settings.TagList("Sounds").Contains(foundContext.ModKey) && !mapped[5])
                    {
                        if (Compare.NotEqual(foundContext.Record.OpenSound,originalObject.Record.OpenSound)
                            || Compare.NotEqual(foundContext.Record.CloseSound,originalObject.Record.CloseSound))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[5] = true;
                            else
                            {
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
                                mapped[5] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Inventory Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList("Invent.Add").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Items,originalObject.Record.Items))
                            NewContainer.Add(foundContext.Record.Items);
                    if (Settings.TagList("Invent.Change").Contains(foundContext.ModKey))
                        if(Compare.NotEqual(foundContext.Record.Items,originalObject.Record.Items))
                            NewContainer.Change(foundContext.Record.Items, originalObject.Record.Items);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Inventory Removes
                    //==============================================================================================================
                    if (Settings.TagList("Invent.Remove").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Items,originalObject.Record.Items))
                            NewContainer.Remove(foundContext.Record.Items, originalObject.Record.Items);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewContainer.Modified)
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Items = NewContainer.OverrideObject;
                }
                
            }
        }
    }
}
