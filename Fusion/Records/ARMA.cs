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
    internal class ARMA
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList("Graphics,Sound");
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
                var workingContext = allContexts[0];
                var originalObject = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IArmorAddon? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach (var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (mapped.NotMapped("Graphics") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.WorldModel,originalObject.Record.WorldModel)
                            || Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.WorldModel,workingContext.Record.WorldModel)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.FirstPersonModel,workingContext.Record.FirstPersonModel)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.WorldModel,originalObject.Record.WorldModel)) 
                                        overrideObject.WorldModel = Utility.NewGender<Model>(foundContext.Record.WorldModel?.Male?.DeepCopy(), foundContext.Record.WorldModel?.Female?.DeepCopy());
                                    if (Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel)) 
                                        overrideObject.FirstPersonModel = Utility.NewGender<Model>(foundContext.Record.FirstPersonModel?.Male?.DeepCopy(), foundContext.Record.FirstPersonModel?.Female?.DeepCopy());
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (mapped.NotMapped("Sound") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.FootstepSound,originalObject.Record.FootstepSound))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.FootstepSound,workingContext.Record.FootstepSound)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.FootstepSound,originalObject.Record.FootstepSound))
                                        overrideObject.FootstepSound.SetTo(foundContext.Record.FootstepSound);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }
                }
                
            }
        }
    }
}
