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
            Console.WriteLine("Processing Faction");
            HashSet<ModKey> workingModList = Settings.GetModList("Names,Relations.Remove,Relations.Add,Relations.Change");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Faction().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Last();
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
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
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
