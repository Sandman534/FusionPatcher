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
    internal class LocationPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Keywords,Names,Sounds,Stats");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Location().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(workingContext.Record.FormKey).Last();
                bool[] mapped = new bool[20];
                Keywords NewKeywords = new(workingContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Settings.TagList("Names").Contains(foundContext.ModKey) && !mapped[0])
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
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
                                mapped[0] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (Settings.TagList("Sounds").Contains(foundContext.ModKey) && !mapped[1])
                    {
                        if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[1] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Music,workingContext.Record.Music)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                                        overrideObject.Music.SetTo(foundContext.Record.Music);
                                }
                                mapped[1] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (Settings.TagList("Stats").Contains(foundContext.ModKey) && !mapped[2])
                    {
                        if (Compare.NotEqual(foundContext.Record.ParentLocation,originalObject.Record.ParentLocation)
                            || Compare.NotEqual(foundContext.Record.WorldLocationMarkerRef,originalObject.Record.WorldLocationMarkerRef)
                            || Compare.NotEqual(foundContext.Record.WorldLocationRadius,originalObject.Record.WorldLocationRadius))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) 
                                mapped[2] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ParentLocation,workingContext.Record.ParentLocation)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.WorldLocationMarkerRef,workingContext.Record.WorldLocationMarkerRef)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.WorldLocationRadius,workingContext.Record.WorldLocationRadius)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ParentLocation,originalObject.Record.ParentLocation))
                                        overrideObject.ParentLocation.SetTo(foundContext.Record.ParentLocation);
                                    if (Compare.NotEqual(foundContext.Record.WorldLocationMarkerRef,originalObject.Record.WorldLocationMarkerRef))
                                        overrideObject.WorldLocationMarkerRef.SetTo(foundContext.Record.WorldLocationMarkerRef);
                                    if (Compare.NotEqual(foundContext.Record.WorldLocationRadius,originalObject.Record.WorldLocationRadius))
                                        overrideObject.WorldLocationRadius = foundContext.Record.WorldLocationRadius;
                                    
                                }
                                mapped[2] = true;
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
