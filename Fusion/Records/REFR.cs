using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class REFR
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.F_Base, Tags.F_EnableParent, Tags.F_LocationReference);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IPlacedObjectGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Object Reference");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IPlacedObject, IPlacedObjectGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IPlacedObject? overrideObject = null;
                MappedTags mapped = new();
                

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Base
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.F_Base, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Base,oContext.Record.Base)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Base,wContext.Record.Base,oContext.Record.Base)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Base.FormKey = fContext.Record.Base.FormKey;
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Enable Parent
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.F_EnableParent, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.EnableParent,oContext.Record.EnableParent)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EnableParent,wContext.Record.EnableParent,oContext.Record.EnableParent)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnableParent = fContext.Record.EnableParent?.DeepCopy();
                                }
                            }
                            mapped.SetMapped();
                        }

                    }

                    //==============================================================================================================
                    // Location Reference
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.F_LocationReference, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.LocationReference,oContext.Record.LocationReference)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.LocationReference,wContext.Record.LocationReference,oContext.Record.LocationReference)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.LocationReference.SetTo(fContext.Record.LocationReference);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }
                }
            }
        }
    }
}
