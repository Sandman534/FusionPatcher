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
    internal class LVSP
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList("Relev,Delev,ObjectBounds");
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<ILeveledSpellGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Leveled Spell");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<ILeveledSpell, ILeveledSpellGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var workingContext = allContexts[0];
                var originalObject = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                ILeveledSpell? overrideObject = null;
                MappedTags mapped = new MappedTags();
                Leveled NewList = new(workingContext.Record.Entries);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (mapped.NotMapped("ObjectBounds") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                        overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Leveled List Adds
                    //==============================================================================================================
                    if (Settings.TagList("Relev").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Entries,originalObject.Record.Entries))
                            NewList.Add(foundContext.Record.Entries);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Leveled List Removes
                    //==============================================================================================================
                    if (Settings.TagList("Delev").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Entries,originalObject.Record.Entries))
                            NewList.Remove(foundContext.Record.Entries, originalObject.Record.Entries);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewList.Modified)
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Entries = NewList.OverrideSpellObject;
                }
            }
        }
    }
}
