using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class ENCH
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.EnchantmentStats, Tags.Names, Tags.ObjectBounds);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IObjectEffectGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Enchantment");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IObjectEffect, IObjectEffectGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IObjectEffect? overrideObject = null;
                MappedTags mapped = new();
                
                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Enchantment Stats
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.EnchantmentStats, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.EnchantmentCost,oContext.Record.EnchantmentCost)
                            || Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags)
                            || Compare.NotEqual(fContext.Record.CastType,oContext.Record.CastType)
                            || Compare.NotEqual(fContext.Record.EnchantmentAmount,oContext.Record.EnchantmentAmount)
                            || Compare.NotEqual(fContext.Record.TargetType,oContext.Record.TargetType)
                            || Compare.NotEqual(fContext.Record.EnchantType,oContext.Record.EnchantType)
                            || Compare.NotEqual(fContext.Record.ChargeTime,oContext.Record.ChargeTime)
                            || Compare.NotEqual(fContext.Record.BaseEnchantment,oContext.Record.BaseEnchantment)
                            || Compare.NotEqual(fContext.Record.WornRestrictions,oContext.Record.WornRestrictions)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EnchantmentCost,wContext.Record.EnchantmentCost,oContext.Record.EnchantmentCost)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnchantmentCost = fContext.Record.EnchantmentCost;
                                }

                                if (Utility.ShouldChange(fContext.Record.Flags,wContext.Record.Flags,oContext.Record.Flags)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Flags = fContext.Record.Flags;
                                }

                                if (Utility.ShouldChange(fContext.Record.CastType,wContext.Record.CastType,oContext.Record.CastType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CastType = fContext.Record.CastType;
                                }

                                if (Utility.ShouldChange(fContext.Record.EnchantmentAmount,wContext.Record.EnchantmentAmount,oContext.Record.EnchantmentAmount)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnchantmentAmount = fContext.Record.EnchantmentAmount;
                                }

                                if (Utility.ShouldChange(fContext.Record.TargetType,wContext.Record.TargetType,oContext.Record.TargetType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TargetType = fContext.Record.TargetType;
                                }

                                if (Utility.ShouldChange(fContext.Record.EnchantType,wContext.Record.EnchantType,oContext.Record.EnchantType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnchantType = fContext.Record.EnchantType;
                                }

                                if (Utility.ShouldChange(fContext.Record.ChargeTime,wContext.Record.ChargeTime,oContext.Record.ChargeTime)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ChargeTime = fContext.Record.ChargeTime;
                                }

                                if (Utility.ShouldChange(fContext.Record.BaseEnchantment,wContext.Record.BaseEnchantment,oContext.Record.BaseEnchantment)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BaseEnchantment.SetTo(fContext.Record.BaseEnchantment);
                                }

                                if (Utility.ShouldChange(fContext.Record.WornRestrictions,wContext.Record.WornRestrictions,oContext.Record.WornRestrictions)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WornRestrictions.SetTo(fContext.Record.WornRestrictions);
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
                }
            }
        }
    }
}