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
    internal class ActivatorPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Keywords,Names,ObjectBounds,Sounds,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Activator().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IActivator, IActivatorGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IActivator, IActivatorGetter>(workingContext.Record.FormKey).Last();
                bool[] mapped = new bool[20];
                Keywords NewKeywords = new(workingContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if(Settings.TagList("Destructible").Contains(foundContext.ModKey) && !mapped[0])
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
                                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                        overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                                }
                                mapped[3] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (Settings.TagList("Sounds").Contains(foundContext.ModKey) && !mapped[4])
                    {
                        if (Compare.NotEqual(foundContext.Record.ActivationSound,originalObject.Record.ActivationSound)
                            || Compare.NotEqual(foundContext.Record.LoopingSound,originalObject.Record.LoopingSound))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[4] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ActivationSound,workingContext.Record.ActivationSound)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.LoopingSound,workingContext.Record.LoopingSound)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ActivationSound,originalObject.Record.ActivationSound))
                                        overrideObject.ActivationSound.SetTo(foundContext.Record.ActivationSound);
                                    if (Compare.NotEqual(foundContext.Record.LoopingSound,originalObject.Record.LoopingSound)) 
                                        overrideObject.LoopingSound.SetTo(foundContext.Record.LoopingSound);
                                }
                                mapped[4] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Text
                    //==============================================================================================================
                    if (Settings.TagList("Text").Contains(foundContext.ModKey) && !mapped[5])
                    {
                        if (Compare.NotEqual(foundContext.Record.ActivateTextOverride,originalObject.Record.ActivateTextOverride))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[5] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ActivateTextOverride,workingContext.Record.ActivateTextOverride)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ActivateTextOverride,originalObject.Record.ActivateTextOverride))
                                        overrideObject.ActivateTextOverride = Utility.NewString(foundContext.Record.ActivateTextOverride);
                                }
                                mapped[5] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                                NewKeywords.Add(foundContext.Record.Keywords, originalObject.Record.Keywords);
                        
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                            NewKeywords.Remove(foundContext.Record.Keywords, originalObject.Record.Keywords);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewKeywords.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                
            }
        }
    }
}
