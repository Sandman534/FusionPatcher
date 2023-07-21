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
        public List<ModKey> Conditions = new();
    }

    internal class QuestPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, QuestSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Conditions
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Quest().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IQuest, IQuestGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IQuest, IQuestGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Conditions
                //==============================================================================================================
                if (Settings.Conditions.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IQuest, IQuestGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Conditions.Contains(context.ModKey)
                            && (context.Record.DialogConditions != originalObject.Record.DialogConditions))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);

                        // Add All Records
                        foreach (var context in foundContext)
                        {
                            var listObject = context.Record;
                            if (listObject.DialogConditions != null && listObject.DialogConditions.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.DialogConditions != null && originalObject.Record.DialogConditions.Count > 0 && overrideObject.DialogConditions?.Count > 0)
                                    foreach (var rec in originalObject.Record.DialogConditions)
                                        if (!listObject.DialogConditions.Contains(rec) && overrideObject.DialogConditions.Contains(rec))
                                            overrideObject.DialogConditions.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.DialogConditions)
                                    if (overrideObject.DialogConditions != null && !overrideObject.DialogConditions.Contains(rec))
                                        overrideObject.DialogConditions.Add(rec.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
    }
}
