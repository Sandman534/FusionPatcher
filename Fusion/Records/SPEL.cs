using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class SPEL
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.Keywords, Tags.Names, Tags.ObjectBounds, Tags.SpellStats, Tags.Text);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<ISpellGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Spell");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<ISpell, ISpellGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                ISpell? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.MenuDisplayObject,oContext.Record.MenuDisplayObject)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.MenuDisplayObject,wContext.Record.MenuDisplayObject,oContext.Record.MenuDisplayObject)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.MenuDisplayObject.SetTo(fContext.Record.MenuDisplayObject);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

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
                    // SpellStats
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.SpellStats, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.EditorID,oContext.Record.EditorID)
                            || Compare.NotEqual(fContext.Record.BaseCost,oContext.Record.BaseCost)
                            || Compare.NotEqual(fContext.Record.Type,oContext.Record.Type)
                            || Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags)
                            || Compare.NotEqual(fContext.Record.CastDuration,oContext.Record.CastDuration)
                            || Compare.NotEqual(fContext.Record.HalfCostPerk,oContext.Record.HalfCostPerk)
                            || Compare.NotEqual(fContext.Record.ChargeTime,oContext.Record.ChargeTime)
                            || Compare.NotEqual(fContext.Record.Range,oContext.Record.Range)
                            || Compare.NotEqual(fContext.Record.TargetType,oContext.Record.TargetType)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EditorID,wContext.Record.EditorID,oContext.Record.EditorID)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EditorID = fContext.Record.EditorID;
                                }

                                if (Utility.ShouldChange(fContext.Record.BaseCost,wContext.Record.BaseCost,oContext.Record.BaseCost)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BaseCost = fContext.Record.BaseCost;
                                }

                                if (Utility.ShouldChange(fContext.Record.Type,wContext.Record.Type,oContext.Record.Type)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Type = fContext.Record.Type;
                                }

                                if (Utility.ShouldChange(fContext.Record.Flags,wContext.Record.Flags,oContext.Record.Flags)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Flags = fContext.Record.Flags;
                                }

                                if (Utility.ShouldChange(fContext.Record.CastDuration,wContext.Record.CastDuration,oContext.Record.CastDuration)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CastDuration = fContext.Record.CastDuration;
                                }

                                if (Utility.ShouldChange(fContext.Record.HalfCostPerk,wContext.Record.HalfCostPerk,oContext.Record.HalfCostPerk)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HalfCostPerk.SetTo(fContext.Record.HalfCostPerk);
                                }

                                if (Utility.ShouldChange(fContext.Record.ChargeTime,wContext.Record.ChargeTime,oContext.Record.ChargeTime)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ChargeTime = fContext.Record.ChargeTime;
                                }

                                if (Utility.ShouldChange(fContext.Record.Range,wContext.Record.Range,oContext.Record.Range)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Range = fContext.Record.Range;
                                }

                                if (Utility.ShouldChange(fContext.Record.TargetType,wContext.Record.TargetType,oContext.Record.TargetType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TargetType = fContext.Record.TargetType;   
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Text
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Text, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Description,oContext.Record.Description)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Description,wContext.Record.Description,oContext.Record.Description)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Description = Utility.NewStringNotNull(fContext.Record.Description);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }
                
                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                                NewKeywords.Add(fContext.Record.Keywords, oContext.Record.Keywords);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                            NewKeywords.Remove(fContext.Record.Keywords, oContext.Record.Keywords);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewKeywords.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                

            }
        }
    }
}
