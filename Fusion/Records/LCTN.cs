using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class LCTN
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Keywords, Tags.Names, Tags.Sound, Tags.Stats);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<ILocationGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Location");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<ILocation, ILocationGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                ILocation? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
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
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Music,oContext.Record.Music)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Music,wContext.Record.Music,oContext.Record.Music)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Music.SetTo(fContext.Record.Music);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Stats, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ParentLocation,oContext.Record.ParentLocation)
                            || Compare.NotEqual(fContext.Record.WorldLocationMarkerRef,oContext.Record.WorldLocationMarkerRef)
                            || Compare.NotEqual(fContext.Record.WorldLocationRadius,oContext.Record.WorldLocationRadius)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ParentLocation,wContext.Record.ParentLocation,oContext.Record.ParentLocation)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ParentLocation.SetTo(fContext.Record.ParentLocation);
                                }

                                if (Utility.ShouldChange(fContext.Record.WorldLocationMarkerRef,wContext.Record.WorldLocationMarkerRef,oContext.Record.WorldLocationMarkerRef)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WorldLocationMarkerRef.SetTo(fContext.Record.WorldLocationMarkerRef);
                                }

                                if (Utility.ShouldChange(fContext.Record.WorldLocationRadius,wContext.Record.WorldLocationRadius,oContext.Record.WorldLocationRadius)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WorldLocationRadius = fContext.Record.WorldLocationRadius;
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
