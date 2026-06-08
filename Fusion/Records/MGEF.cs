using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class MGEF
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.Keywords, Tags.Names, Tags.EffectStats, Tags.Sound, Tags.Text);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IMagicEffectGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Magic Effect");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IMagicEffect, IMagicEffectGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IMagicEffect? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Effect Stats
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.EffectStats, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags)
                            || Compare.NotEqual(fContext.Record.BaseCost,oContext.Record.BaseCost)
                            || Compare.NotEqual(fContext.Record.MagicSkill,oContext.Record.MagicSkill)
                            || Compare.NotEqual(fContext.Record.ResistValue,oContext.Record.ResistValue)
                            || Compare.NotEqual(fContext.Record.TaperWeight,oContext.Record.TaperWeight)
                            || Compare.NotEqual(fContext.Record.MinimumSkillLevel,oContext.Record.MinimumSkillLevel)
                            || Compare.NotEqual(fContext.Record.SpellmakingArea,oContext.Record.SpellmakingArea)
                            || Compare.NotEqual(fContext.Record.SpellmakingCastingTime,oContext.Record.SpellmakingCastingTime)
                            || Compare.NotEqual(fContext.Record.TaperCurve,oContext.Record.TaperCurve)
                            || Compare.NotEqual(fContext.Record.TaperDuration,oContext.Record.TaperDuration)
                            || Compare.NotEqual(fContext.Record.SecondActorValueWeight,oContext.Record.SecondActorValueWeight)
                            || Compare.NotEqual(fContext.Record.Archetype,oContext.Record.Archetype)
                            // Actor Value
                            || Compare.NotEqual(fContext.Record.CastType,oContext.Record.CastType)
                            || Compare.NotEqual(fContext.Record.TargetType,oContext.Record.TargetType)
                            || Compare.NotEqual(fContext.Record.SecondActorValue,oContext.Record.SecondActorValue)
                            || Compare.NotEqual(fContext.Record.SkillUsageMultiplier,oContext.Record.SkillUsageMultiplier)
                            || Compare.NotEqual(fContext.Record.EquipAbility,oContext.Record.EquipAbility)
                            || Compare.NotEqual(fContext.Record.PerkToApply,oContext.Record.PerkToApply)
                            || Compare.NotEqual(fContext.Record.ScriptEffectAIScore,oContext.Record.ScriptEffectAIScore)
                            || Compare.NotEqual(fContext.Record.ScriptEffectAIDelayTime,oContext.Record.ScriptEffectAIDelayTime)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Flags,wContext.Record.Flags,oContext.Record.Flags)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Flags = fContext.Record.Flags;
                                }

                                if (Utility.ShouldChange(fContext.Record.BaseCost,wContext.Record.BaseCost,oContext.Record.BaseCost)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BaseCost = fContext.Record.BaseCost;
                                }

                                if (Utility.ShouldChange(fContext.Record.BaseCost,wContext.Record.BaseCost,oContext.Record.BaseCost)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BaseCost = fContext.Record.BaseCost;
                                }

                                if (Utility.ShouldChange(fContext.Record.MagicSkill,wContext.Record.MagicSkill,oContext.Record.MagicSkill)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.MagicSkill = fContext.Record.MagicSkill;
                                }

                                if (Utility.ShouldChange(fContext.Record.ResistValue,wContext.Record.ResistValue,oContext.Record.ResistValue)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ResistValue = fContext.Record.ResistValue;
                                }

                                if (Utility.ShouldChange(fContext.Record.TaperWeight,wContext.Record.TaperWeight,oContext.Record.TaperWeight)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TaperWeight = fContext.Record.TaperWeight;
                                }

                                if (Utility.ShouldChange(fContext.Record.MinimumSkillLevel,wContext.Record.MinimumSkillLevel,oContext.Record.MinimumSkillLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.MinimumSkillLevel = fContext.Record.MinimumSkillLevel;
                                }

                                if (Utility.ShouldChange(fContext.Record.SpellmakingArea,wContext.Record.SpellmakingArea,oContext.Record.SpellmakingArea)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SpellmakingArea = fContext.Record.SpellmakingArea;
                                }

                                if (Utility.ShouldChange(fContext.Record.SpellmakingCastingTime,wContext.Record.SpellmakingCastingTime,oContext.Record.SpellmakingCastingTime)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SpellmakingCastingTime = fContext.Record.SpellmakingCastingTime;   
                                }

                                if (Utility.ShouldChange(fContext.Record.TaperCurve,wContext.Record.TaperCurve,oContext.Record.TaperCurve)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TaperCurve = fContext.Record.TaperCurve;   
                                }

                                if (Utility.ShouldChange(fContext.Record.TaperDuration,wContext.Record.TaperDuration,oContext.Record.TaperDuration)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TaperDuration = fContext.Record.TaperDuration;   
                                }

                                if (Utility.ShouldChange(fContext.Record.SecondActorValueWeight,wContext.Record.SecondActorValueWeight,oContext.Record.SecondActorValueWeight)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SecondActorValueWeight = fContext.Record.SecondActorValueWeight;   
                                }

                                if (Utility.ShouldChange(fContext.Record.Archetype,wContext.Record.Archetype,oContext.Record.Archetype)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Archetype = fContext.Record.Archetype.DeepCopy();   
                                }

                                if (Utility.ShouldChange(fContext.Record.CastType,wContext.Record.CastType,oContext.Record.CastType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CastType = fContext.Record.CastType;   
                                }

                                if (Utility.ShouldChange(fContext.Record.TargetType,wContext.Record.TargetType,oContext.Record.TargetType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TargetType = fContext.Record.TargetType;   
                                }

                                if (Utility.ShouldChange(fContext.Record.SecondActorValue,wContext.Record.SecondActorValue,oContext.Record.SecondActorValue)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SecondActorValue = fContext.Record.SecondActorValue;   
                                }

                                if (Utility.ShouldChange(fContext.Record.SkillUsageMultiplier,wContext.Record.SkillUsageMultiplier,oContext.Record.SkillUsageMultiplier)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillUsageMultiplier = fContext.Record.SkillUsageMultiplier;   
                                }

                                if (Utility.ShouldChange(fContext.Record.EquipAbility,wContext.Record.EquipAbility,oContext.Record.EquipAbility)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EquipAbility.SetTo(fContext.Record.EquipAbility);   
                                }

                                if (Utility.ShouldChange(fContext.Record.PerkToApply,wContext.Record.PerkToApply,oContext.Record.PerkToApply)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.PerkToApply.SetTo(fContext.Record.PerkToApply);   
                                }

                                if (Utility.ShouldChange(fContext.Record.ScriptEffectAIScore,wContext.Record.ScriptEffectAIScore,oContext.Record.ScriptEffectAIScore)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ScriptEffectAIScore = fContext.Record.ScriptEffectAIScore;   
                                }

                                if (Utility.ShouldChange(fContext.Record.ScriptEffectAIDelayTime,wContext.Record.ScriptEffectAIDelayTime,oContext.Record.ScriptEffectAIDelayTime)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ScriptEffectAIDelayTime = fContext.Record.ScriptEffectAIDelayTime;   
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.MenuDisplayObject,oContext.Record.MenuDisplayObject)
                            || Compare.NotEqual(fContext.Record.CastingLight,oContext.Record.CastingLight)
                            || Compare.NotEqual(fContext.Record.HitShader,oContext.Record.HitShader)
                            || Compare.NotEqual(fContext.Record.EnchantShader,oContext.Record.EnchantShader)
                            || Compare.NotEqual(fContext.Record.Projectile,oContext.Record.Projectile)
                            || Compare.NotEqual(fContext.Record.Explosion,oContext.Record.Explosion)
                            || Compare.NotEqual(fContext.Record.CastingArt,oContext.Record.CastingArt)
                            || Compare.NotEqual(fContext.Record.HitEffectArt,oContext.Record.HitEffectArt)
                            || Compare.NotEqual(fContext.Record.ImpactData,oContext.Record.ImpactData)
                            || Compare.NotEqual(fContext.Record.DualCastArt,oContext.Record.DualCastArt)
                            || Compare.NotEqual(fContext.Record.DualCastScale,oContext.Record.DualCastScale)
                            || Compare.NotEqual(fContext.Record.EnchantArt,oContext.Record.EnchantArt)
                            || Compare.NotEqual(fContext.Record.HitVisuals,oContext.Record.HitVisuals)
                            || Compare.NotEqual(fContext.Record.EnchantVisuals,oContext.Record.EnchantVisuals)
                            || Compare.NotEqual(fContext.Record.ImageSpaceModifier,oContext.Record.ImageSpaceModifier)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.MenuDisplayObject,wContext.Record.MenuDisplayObject,oContext.Record.MenuDisplayObject)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.MenuDisplayObject.SetTo(fContext.Record.MenuDisplayObject);
                                }

                                if (Utility.ShouldChange(fContext.Record.CastingLight,wContext.Record.CastingLight,oContext.Record.CastingLight)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CastingLight.SetTo(fContext.Record.CastingLight);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.HitShader,wContext.Record.HitShader,oContext.Record.HitShader)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HitShader.SetTo(fContext.Record.HitShader);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.EnchantShader,wContext.Record.EnchantShader,oContext.Record.EnchantShader)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnchantShader.SetTo(fContext.Record.EnchantShader);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Projectile,wContext.Record.Projectile,oContext.Record.Projectile)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Projectile.SetTo(fContext.Record.Projectile);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Explosion,wContext.Record.Explosion,oContext.Record.Explosion)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Explosion.SetTo(fContext.Record.Explosion);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.CastingArt,wContext.Record.CastingArt,oContext.Record.CastingArt)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CastingArt.SetTo(fContext.Record.CastingArt);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.HitEffectArt,wContext.Record.HitEffectArt,oContext.Record.HitEffectArt)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HitEffectArt.SetTo(fContext.Record.HitEffectArt);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.ImpactData,wContext.Record.ImpactData,oContext.Record.ImpactData)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImpactData.SetTo(fContext.Record.ImpactData);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.DualCastArt,wContext.Record.DualCastArt,oContext.Record.DualCastArt)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.DualCastArt.SetTo(fContext.Record.DualCastArt);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.DualCastScale,wContext.Record.DualCastScale,oContext.Record.DualCastScale)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.DualCastScale = fContext.Record.DualCastScale;
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.EnchantArt,wContext.Record.EnchantArt,oContext.Record.EnchantArt)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnchantArt.SetTo(fContext.Record.EnchantArt);
                                }

                                if (Utility.ShouldChange(fContext.Record.HitVisuals,wContext.Record.HitVisuals,oContext.Record.HitVisuals)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HitVisuals.SetTo(fContext.Record.HitVisuals);
                                }

                                if (Utility.ShouldChange(fContext.Record.EnchantVisuals,wContext.Record.EnchantVisuals,oContext.Record.EnchantVisuals)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnchantVisuals.SetTo(fContext.Record.EnchantVisuals);
                                }

                                if (Utility.ShouldChange(fContext.Record.ImageSpaceModifier,wContext.Record.ImageSpaceModifier,oContext.Record.ImageSpaceModifier)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImageSpaceModifier.SetTo(fContext.Record.ImageSpaceModifier);
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
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.CastingSoundLevel,oContext.Record.CastingSoundLevel)
                            || Compare.NotEqual(fContext.Record.Sounds,oContext.Record.Sounds)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.CastingSoundLevel,wContext.Record.CastingSoundLevel,oContext.Record.CastingSoundLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CastingSoundLevel = fContext.Record.CastingSoundLevel;
                                }

                                if (Utility.ShouldChange(fContext.Record.Sounds,wContext.Record.Sounds,oContext.Record.Sounds)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Sounds?.SetTo((fContext.Record.Sounds ?? []).Select(x => x.DeepCopy()).ToArray());
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
                                    overrideObject.Description = Utility.NewString(fContext.Record.Description);
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
