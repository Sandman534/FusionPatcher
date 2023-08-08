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
    internal class LeveledNPCPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Relev,Delev,ObjectBounds");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.LeveledNpc().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<ILeveledNpc, ILeveledNpcGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<ILeveledNpc, ILeveledNpcGetter>(workingContext.Record.FormKey).Last();
                bool[] mapped = new bool[20];
                Leveled NewList = new(workingContext.Record.Entries);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (Settings.TagList("ObjectBounds").Contains(foundContext.ModKey) && !mapped[0])
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                        overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                                }
                                mapped[0] = true;
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
                    addedRecord.Entries = NewList.OverrideNpcObject;
                }

            }
        }
    }
}
