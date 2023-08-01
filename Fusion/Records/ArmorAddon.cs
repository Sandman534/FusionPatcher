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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Graphic").Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.WorldModel?.Equals(originalObject.Record.WorldModel) ?? false)
                        || (!foundContext.Record.FirstPersonModel?.Equals(originalObject.Record.FirstPersonModel) ?? false))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.WorldModel?.Equals(originalObject.Record.WorldModel) ?? false) Change = true;
                        if (!foundContext.Record.FirstPersonModel?.Equals(originalObject.Record.FirstPersonModel) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.WorldModel?.Male != null) overrideObject.WorldModel?.Male?.DeepCopyIn(foundContext.Record.WorldModel.Male);
                            if (foundContext.Record.WorldModel?.Female != null) overrideObject.WorldModel?.Female?.DeepCopyIn(foundContext.Record.WorldModel.Female);
                            if (foundContext.Record.FirstPersonModel?.Male != null) overrideObject.FirstPersonModel?.Male?.DeepCopyIn(foundContext.Record.FirstPersonModel.Male);
                            if (foundContext.Record.FirstPersonModel?.Female != null) overrideObject.FirstPersonModel?.Female?.DeepCopyIn(foundContext.Record.FirstPersonModel.Female);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Sounds").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.FootstepSound.Equals(originalObject.Record.FootstepSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.FootstepSound.Equals(workingContext.Record.FootstepSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.FootstepSound != null) overrideObject.FootstepSound.SetTo(foundContext.Record.FootstepSound);
                        }
                        break;
                    }
                }

            }
        }
    }
}
