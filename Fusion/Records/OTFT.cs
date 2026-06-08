using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class OTFT
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Outfits_Add, Tags.Outfits_Remove);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IOutfitGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Outfit");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IOutfit, IOutfitGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                MappedTags mapped = new();
                Outfits NewOutfits = new(wContext.Record.Items);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Outfit Add
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Outfits_Add).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewOutfits.Add(fContext.Record.Items);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Outfit Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Outfits_Remove).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewOutfits.Remove(fContext.Record.Items, oContext.Record.Items);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewOutfits.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Items?.SetTo(NewOutfits.OverrideObject);
                }                
            }
        }
    }
}
