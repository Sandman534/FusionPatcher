using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class CONT
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Destructible, Tags.Graphics, Tags.Invent_Remove, Tags.Invent_Add, Tags.Invent_Change,
                Tags.Names,Tags.ObjectBounds, Tags.Scripts, Tags.Sound);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IContainerGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Container");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IContainer, IContainerGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IContainer? overrideObject = null;
                MappedTags mapped = new();
                Containers NewContainer = new(wContext.Record.Items);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Destructible, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Destructible,oContext.Record.Destructible)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Destructible,wContext.Record.Destructible,oContext.Record.Destructible)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Destructible = fContext.Record.Destructible?.DeepCopy();
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Model,oContext.Record.Model)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Model,wContext.Record.Model,oContext.Record.Model)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Model = fContext.Record.Model?.DeepCopy();
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Names, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Name,oContext.Record.Name)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Name,wContext.Record.Name,oContext.Record.Name)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Name = Utility.NewString(fContext.Record.Name);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.ObjectBounds, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ObjectBounds,oContext.Record.ObjectBounds)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ObjectBounds,wContext.Record.ObjectBounds,oContext.Record.ObjectBounds)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ObjectBounds.DeepCopyIn(fContext.Record.ObjectBounds);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Scripts
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Scripts, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.VirtualMachineAdapter,oContext.Record.VirtualMachineAdapter)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.VirtualMachineAdapter,wContext.Record.VirtualMachineAdapter,oContext.Record.VirtualMachineAdapter)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.VirtualMachineAdapter = fContext.Record.VirtualMachineAdapter?.DeepCopy();
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.OpenSound,oContext.Record.OpenSound)
                            || Compare.NotEqual(fContext.Record.CloseSound,oContext.Record.CloseSound)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.OpenSound,wContext.Record.OpenSound,oContext.Record.OpenSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.OpenSound.SetTo(fContext.Record.OpenSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.CloseSound,wContext.Record.CloseSound,oContext.Record.CloseSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CloseSound.SetTo(fContext.Record.CloseSound);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Inventory Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Invent_Add).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewContainer.Add(fContext.Record.Items);
                    if (Settings.TagList(Tags.Invent_Change).Contains(fContext.ModKey))
                        if(Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewContainer.Change(fContext.Record.Items, oContext.Record.Items);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Inventory Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Invent_Remove).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewContainer.Remove(fContext.Record.Items, oContext.Record.Items);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewContainer.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Items = NewContainer.OverrideObject;
                }
                
            }
        }
    }
}
