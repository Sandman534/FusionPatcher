using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace Fusion
{    
    internal class LVSP
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Relev, Tags.Delev, Tags.ObjectBounds);
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
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                ILeveledSpell? overrideObject = null;
                MappedTags mapped = new();
                Leveled NewList = new(wContext.Record.Entries);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.ObjectBounds, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ObjectBounds,oContext.Record.ObjectBounds)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ObjectBounds,wContext.Record.ObjectBounds,oContext.Record.ObjectBounds)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ObjectBounds.DeepCopyIn(fContext.Record.ObjectBounds);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Leveled List Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Relev).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Entries,oContext.Record.Entries))
                            NewList.Add(fContext.Record.Entries);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Leveled List Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Delev).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Entries,oContext.Record.Entries))
                            NewList.Remove(fContext.Record.Entries, oContext.Record.Entries);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewList.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Entries = NewList.OverrideSpellObject;
                }
            }
        }
    }
}
