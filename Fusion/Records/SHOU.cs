using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class SHOU
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.Names, Tags.Text);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IShoutGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Shout");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IShout, IShoutGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IShout? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.MenuDisplayObject,oContext.Record.MenuDisplayObject)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.MenuDisplayObject,wContext.Record.MenuDisplayObject,oContext.Record.MenuDisplayObject)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.MenuDisplayObject.SetTo(fContext.Record.MenuDisplayObject);
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
                    // Text
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Text, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Description,oContext.Record.Description)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Description,wContext.Record.Description,oContext.Record.Description)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Description = Utility.NewString(fContext.Record.Description);
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
