using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class ARMA
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.Sound);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IArmorAddonGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Armor Addon");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IArmorAddon, IArmorAddonGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IArmorAddon? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach (var fContext in modContext)
                {
                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.WorldModel,oContext.Record.WorldModel)
                            || Compare.NotEqual(fContext.Record.FirstPersonModel,oContext.Record.FirstPersonModel)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.WorldModel,wContext.Record.WorldModel,oContext.Record.WorldModel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WorldModel = Utility.NewGender<Model>(fContext.Record.WorldModel?.Male?.DeepCopy(), fContext.Record.WorldModel?.Female?.DeepCopy());
                                }

                                if (Utility.ShouldChange(fContext.Record.FirstPersonModel,wContext.Record.FirstPersonModel,oContext.Record.FirstPersonModel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FirstPersonModel = Utility.NewGender<Model>(fContext.Record.FirstPersonModel?.Male?.DeepCopy(), fContext.Record.FirstPersonModel?.Female?.DeepCopy());
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
                            Compare.NotEqual(fContext.Record.FootstepSound,oContext.Record.FootstepSound)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.FootstepSound,wContext.Record.FootstepSound,oContext.Record.FootstepSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FootstepSound.SetTo(fContext.Record.FootstepSound);
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
