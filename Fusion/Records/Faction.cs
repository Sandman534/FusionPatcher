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
    internal class FactionPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Names,Relations.Remove,Relations.Add,Relations.Change");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Faction().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IFaction, IFactionGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Names
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Names").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Relations
                //==============================================================================================================
                if (Settings.HasTags("Relations.Add,Relations.Change,Relations.Remove"))
                {
                    Relations NewRelation = new(workingContext.Record.Relations);
                    if (Settings.HasTags("Relations.Add", out var RelationsAdd))
                    {
                        var foundContext = modContext.Where(context => RelationsAdd.Contains(context.ModKey) && Compare.NotEqual(context.Record.Relations,originalObject.Record.Relations));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewRelation.Add(context.Record.Relations);
                    }

                    if (Settings.HasTags("Relations.Change", out var RelationsChange))
                    {
                        var foundContext = modContext.Where(context => RelationsChange.Contains(context.ModKey) && Compare.NotEqual(context.Record.Relations,originalObject.Record.Relations));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewRelation.Change(context.Record.Relations, originalObject.Record.Relations);
                    }

                    if (Settings.HasTags("Relations.Remove", out var RelationsRemove))
                    {
                        var foundContext = modContext.Where(context => RelationsRemove.Contains(context.ModKey) && Compare.NotEqual(context.Record.Relations,originalObject.Record.Relations));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewRelation.Remove(context.Record.Relations, originalObject.Record.Relations);
                            
                    }

                    if (NewRelation.Modified) {
                        var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                        addedRecord.Relations.SetTo(NewRelation.OverrideObject);
                    }
                }

            }
        }
    }
}
