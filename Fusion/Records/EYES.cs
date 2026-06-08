using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
namespace Fusion
{
    internal class EYES
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Names, Tags.Stats);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IEyesGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Eyes");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IEyes, IEyesGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IEyes? overrideObject = null;
                MappedTags mapped = new();
                
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
                                    overrideObject.Name = Utility.NewStringNotNull(fContext.Record.Name);
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
                            Compare.NotEqual(fContext.Record.EditorID,oContext.Record.EditorID)
                            || Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EditorID,wContext.Record.EditorID,oContext.Record.EditorID)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EditorID = fContext.Record.EditorID;
                                }

                                if (Utility.ShouldChange(fContext.Record.Flags,wContext.Record.Flags,oContext.Record.Flags)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Flags = fContext.Record.Flags;
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