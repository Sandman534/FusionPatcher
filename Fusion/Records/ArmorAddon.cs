using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Fusion
{
    internal class ArmorAddonPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Graphics,Sounds");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.ArmorAddon().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IArmorAddon, IArmorAddonGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IArmorAddon, IArmorAddonGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Graphic").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.WorldModel,originalObject.Record.WorldModel)
                        || Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.WorldModel,workingContext.Record.WorldModel)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.FirstPersonModel,workingContext.Record.FirstPersonModel)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.WorldModel,originalObject.Record.WorldModel)) 
                                overrideObject.WorldModel = Utility.NewGender<Model>(foundContext.Record.WorldModel?.Male?.DeepCopy(), foundContext.Record.WorldModel?.Female?.DeepCopy());
                            if (Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel)) 
                                overrideObject.FirstPersonModel = Utility.NewGender<Model>(foundContext.Record.FirstPersonModel?.Male?.DeepCopy(), foundContext.Record.FirstPersonModel?.Female?.DeepCopy());
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Sounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.FootstepSound,originalObject.Record.FootstepSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.FootstepSound,workingContext.Record.FootstepSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.FootstepSound,originalObject.Record.FootstepSound))
                                overrideObject.FootstepSound.SetTo(foundContext.Record.FootstepSound);
                        }
                        break;
                    }
                }

            }
        }
    }
}
