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
    public class QuestSettings
    {
        public List<ModKey> Names = new();
        public List<ModKey> Text = new();
    }

    internal class QuestPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, QuestSettings Settings)
        {
            List<ModKey> modList = new() { 
                Settings.Names, Settings.Text };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Quest().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IQuest, IQuestGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IQuest, IQuestGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Names
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Names.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Name?.Equals(originalObject.Record.Name) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Name?.Equals(workingContext.Record.Name) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Name != null) overrideObject.Name?.Set(foundContext.Record.Name.TargetLanguage, foundContext.Record.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Text.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Description?.Equals(originalObject.Record.Description) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Description?.Equals(workingContext.Record.Description) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Description != null) overrideObject.Description?.Set(foundContext.Record.Description.TargetLanguage, foundContext.Record.Description.String);
                        }
                        break;
                    }
                }

            }
        }
    }
}
