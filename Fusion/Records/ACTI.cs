using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class ACTI
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Destructible, Tags.Graphics, Tags.Keywords, Tags.Names, Tags.ObjectBounds, Tags.Sound, Tags.Text);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IActivatorGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Activator");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IActivator, IActivatorGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IActivator? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);
                
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
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ActivationSound,oContext.Record.ActivationSound)
                            || Compare.NotEqual(fContext.Record.LoopingSound,oContext.Record.LoopingSound)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ActivationSound,wContext.Record.ActivationSound,oContext.Record.ActivationSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ActivationSound.SetTo(fContext.Record.ActivationSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.LoopingSound,wContext.Record.LoopingSound,oContext.Record.LoopingSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.LoopingSound.SetTo(fContext.Record.LoopingSound);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Text
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Text, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ActivateTextOverride,oContext.Record.ActivateTextOverride)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ActivateTextOverride,wContext.Record.ActivateTextOverride,oContext.Record.ActivateTextOverride)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ActivateTextOverride = Utility.NewString(fContext.Record.ActivateTextOverride);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                                NewKeywords.Add(fContext.Record.Keywords, oContext.Record.Keywords);
                        
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                            NewKeywords.Remove(fContext.Record.Keywords, oContext.Record.Keywords);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewKeywords.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                
            }
        }
    }
}
