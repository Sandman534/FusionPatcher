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
    internal class REFR
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList("F.Base,F.EnableParent,F.LocationReference");
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
                var workingContext = allContexts[0];
                var originalObject = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IPlacedObject? overrideObject = null;
                MappedTags mapped = new MappedTags();
                

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
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
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
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
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
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.LocationReference, originalObject.Record.LocationReference))
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
