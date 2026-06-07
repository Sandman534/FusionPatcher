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
    internal class FACT
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList("Names,Relations.Remove,Relations.Add,Relations.Change");
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IFactionGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Faction");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var workingContext = allContexts[0];
                var originalObject = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IFaction? overrideObject = null;
                MappedTags mapped = new MappedTags();
                Relations NewRelation = new(workingContext.Record.Relations);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (mapped.NotMapped("Names") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    overrideObject ??= workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                        overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Relation Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList("Relations.Add").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Relations,originalObject.Record.Relations))
                            NewRelation.Add(foundContext.Record.Relations);

                    if (Settings.TagList("Relations.Change").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Relations,originalObject.Record.Relations))
                            NewRelation.Change(foundContext.Record.Relations, originalObject.Record.Relations);

                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Relation Removes
                    //==============================================================================================================
                    if (Settings.TagList("Relations.Remove").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Relations,originalObject.Record.Relations))
                            NewRelation.Remove(foundContext.Record.Relations, originalObject.Record.Relations);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewRelation.Modified)
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Relations.SetTo(NewRelation.OverrideObject);
                }
        
            }
        }
    }
}
