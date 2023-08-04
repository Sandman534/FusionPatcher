using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Immutable;

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
                            if (foundContext.Record.WorldModel?.Male != null && Compare.NotEqual(foundContext.Record.WorldModel.Male,originalObject.Record.WorldModel?.Male)) 
                                overrideObject.WorldModel?.Male?.DeepCopyIn(foundContext.Record.WorldModel.Male);
                            if (foundContext.Record.WorldModel?.Female != null && Compare.NotEqual(foundContext.Record.WorldModel.Female,originalObject.Record.WorldModel?.Female))
                                overrideObject.WorldModel?.Female?.DeepCopyIn(foundContext.Record.WorldModel.Female);
                            if (foundContext.Record.FirstPersonModel?.Male != null && Compare.NotEqual(foundContext.Record.FirstPersonModel.Male,originalObject.Record.FirstPersonModel?.Male)) 
                                overrideObject.FirstPersonModel?.Male?.DeepCopyIn(foundContext.Record.FirstPersonModel.Male);
                            if (foundContext.Record.FirstPersonModel?.Female != null && Compare.NotEqual(foundContext.Record.FirstPersonModel.Female,originalObject.Record.FirstPersonModel?.Female)) 
                                overrideObject.FirstPersonModel?.Female?.DeepCopyIn(foundContext.Record.FirstPersonModel.Female);
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
                            if (foundContext.Record.FootstepSound != null && Compare.NotEqual(foundContext.Record.FootstepSound,originalObject.Record.FootstepSound))
                                overrideObject.FootstepSound.SetTo(foundContext.Record.FootstepSound);
                        }
                        break;
                    }
                }

            }
        }
    }
}
