using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.Core.Tokens;

namespace Fusion
{
    internal class ACHR
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            Console.WriteLine("Processing Actor Reference");
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
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Base
                    //==============================================================================================================
                    if (mapped.NotMapped("F.Base") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Base,originalObject.Record.Base))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
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
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Enable Parent
                    //==============================================================================================================
                    if (mapped.NotMapped("F.EnableParent") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.EnableParent, originalObject.Record.EnableParent))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
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
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Location Reference
                    //==============================================================================================================
                    if (mapped.NotMapped("F.LocationReference") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.LocationReference, originalObject.Record.LocationReference))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
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
                                mapped.SetMapped();
                            }
                        }
                    }
                }
            }
        }
    }
}
