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
    internal class PlacedNPCPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("F.Base,F.EnableParent,F.LocationReference");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.PlacedNpc().WinningContextOverrides(state.LinkCache))
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IPlacedNpc, IPlacedNpcGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IPlacedNpc, IPlacedNpcGetter>(workingContext.Record.FormKey).Last();
                bool[] mapped = new bool[20];

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Base
                    //==============================================================================================================
                    if (Settings.TagList("F.Base").Contains(foundContext.ModKey) && !mapped[0])
                    {
                        if (Compare.NotEqual(foundContext.Record.Base,originalObject.Record.Base))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Base, workingContext.Record.Base)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Base, originalObject.Record.Base))
                                        overrideObject.Base.FormKey = foundContext.Record.Base.FormKey;
                                }
                                mapped[0] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Enable Parent
                    //==============================================================================================================
                    if (Settings.TagList("F.EnableParent").Contains(foundContext.ModKey) && !mapped[1])
                    {
                        if (Compare.NotEqual(foundContext.Record.EnableParent, originalObject.Record.EnableParent))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.EnableParent, workingContext.Record.EnableParent)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.EnableParent, originalObject.Record.EnableParent))
                                        overrideObject.EnableParent = foundContext.Record.EnableParent?.DeepCopy();
                                }
                                mapped[1] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Location Reference
                    //==============================================================================================================
                    if (Settings.TagList("F.LocationReference").Contains(foundContext.ModKey) && !mapped[2])
                    {
                        if (Compare.NotEqual(foundContext.Record.LocationReference, originalObject.Record.LocationReference))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.LocationReference, workingContext.Record.LocationReference)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.EnableParent, originalObject.Record.EnableParent))
                                        overrideObject.LocationReference.SetTo(foundContext.Record.LocationReference);
                                }
                                mapped[2] = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
