using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class FACT
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Names, Tags.Relations_Remove, Tags.Relations_Add, Tags.Relations_Change);
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
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IFaction? overrideObject = null;
                MappedTags mapped = new();
                Relations NewRelation = new(wContext.Record.Relations);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Names, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Name,oContext.Record.Name)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Name,wContext.Record.Name,oContext.Record.Name)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Name = Utility.NewString(fContext.Record.Name);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Relation Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Relations_Add).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Relations,oContext.Record.Relations))
                            NewRelation.Add(fContext.Record.Relations);

                    if (Settings.TagList(Tags.Relations_Change).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Relations,oContext.Record.Relations))
                            NewRelation.Change(fContext.Record.Relations, oContext.Record.Relations);

                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Relation Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Relations_Remove).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Relations,oContext.Record.Relations))
                            NewRelation.Remove(fContext.Record.Relations, oContext.Record.Relations);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewRelation.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Relations.SetTo(NewRelation.OverrideObject);
                }
        
            }
        }
    }
}
