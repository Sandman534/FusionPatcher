using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class WEAP
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Destructible, Tags.Enchantments, Tags.Graphics, Tags.Keywords, Tags.Names, Tags.ObjectBounds, 
                Tags.Sound, Tags.Stats, Tags.Text);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IWeaponGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Weapon");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IWeapon? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Destructible, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Destructible,oContext.Record.Destructible)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Destructible,wContext.Record.Destructible,oContext.Record.Destructible)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Destructible = fContext.Record.Destructible?.DeepCopy();
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Enchantments
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Enchantments, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ObjectEffect,oContext.Record.ObjectEffect)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ObjectEffect,wContext.Record.ObjectEffect,oContext.Record.ObjectEffect)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ObjectEffect.SetTo(fContext.Record.ObjectEffect);
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
                            Compare.NotEqual(fContext.Record.Model,oContext.Record.Model)
                            || Compare.NotEqual(fContext.Record.Icons,oContext.Record.Icons)
                            || Compare.NotEqual(fContext.Record.FirstPersonModel,oContext.Record.FirstPersonModel)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.Model,wContext.Record.Model,oContext.Record.Model)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Model?.DeepCopyIn(fContext.Record.Model);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.Icons,wContext.Record.Icons,oContext.Record.Icons)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Icons?.DeepCopyIn(fContext.Record.Icons);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.FirstPersonModel,wContext.Record.FirstPersonModel,oContext.Record.FirstPersonModel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FirstPersonModel?.SetTo(fContext.Record.FirstPersonModel);
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
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.AttackSound,oContext.Record.AttackSound)
                            || Compare.NotEqual(fContext.Record.AttackSound2D,oContext.Record.AttackSound2D)
                            || Compare.NotEqual(fContext.Record.AttackLoopSound,oContext.Record.AttackLoopSound)
                            || Compare.NotEqual(fContext.Record.AttackFailSound,oContext.Record.AttackFailSound)
                            || Compare.NotEqual(fContext.Record.IdleSound,oContext.Record.IdleSound)
                            || Compare.NotEqual(fContext.Record.EquipSound,oContext.Record.EquipSound)
                            || Compare.NotEqual(fContext.Record.UnequipSound,oContext.Record.UnequipSound)
                            || Compare.NotEqual(fContext.Record.PickUpSound,oContext.Record.PickUpSound)
                            || Compare.NotEqual(fContext.Record.PutDownSound,oContext.Record.PutDownSound)
                            || Compare.NotEqual(fContext.Record.DetectionSoundLevel,oContext.Record.DetectionSoundLevel)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.AttackSound,wContext.Record.AttackSound,oContext.Record.AttackSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AttackSound.SetTo(fContext.Record.AttackSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.AttackSound2D,wContext.Record.AttackSound2D,oContext.Record.AttackSound2D)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AttackSound2D.SetTo(fContext.Record.AttackSound2D);
                                }

                                if (Utility.ShouldChange(fContext.Record.AttackLoopSound,wContext.Record.AttackLoopSound,oContext.Record.AttackLoopSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AttackLoopSound.SetTo(fContext.Record.AttackLoopSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.AttackFailSound,wContext.Record.AttackFailSound,oContext.Record.AttackFailSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AttackFailSound.SetTo(fContext.Record.AttackFailSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.IdleSound,wContext.Record.IdleSound,oContext.Record.IdleSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.IdleSound.SetTo(fContext.Record.IdleSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.EquipSound,wContext.Record.EquipSound,oContext.Record.EquipSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EquipSound.SetTo(fContext.Record.EquipSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.UnequipSound,wContext.Record.UnequipSound,oContext.Record.UnequipSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.UnequipSound.SetTo(fContext.Record.UnequipSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.PickUpSound,wContext.Record.PickUpSound,oContext.Record.PickUpSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.PickUpSound.SetTo(fContext.Record.PickUpSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.PutDownSound,wContext.Record.PutDownSound,oContext.Record.PutDownSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.PutDownSound.SetTo(fContext.Record.PutDownSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.DetectionSoundLevel,wContext.Record.DetectionSoundLevel,oContext.Record.DetectionSoundLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.DetectionSoundLevel = fContext.Record.DetectionSoundLevel;
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Stats, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.EditorID,oContext.Record.EditorID)
                            || Compare.NotEqual(fContext.Record.Data,oContext.Record.Data)
                            || Compare.NotEqual(fContext.Record.Critical,oContext.Record.Critical)
                            || Compare.NotEqual(fContext.Record.BasicStats,oContext.Record.BasicStats)
                            || Compare.NotEqual(fContext.Record.ImpactDataSet,oContext.Record.ImpactDataSet)
                            || Compare.NotEqual(fContext.Record.AlternateBlockMaterial,oContext.Record.AlternateBlockMaterial)
                            || Compare.NotEqual(fContext.Record.DetectionSoundLevel,oContext.Record.DetectionSoundLevel)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EditorID,wContext.Record.EditorID,oContext.Record.EditorID)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EditorID = fContext.Record.EditorID;
                                }

                                if (Utility.ShouldChange(fContext.Record.Data,wContext.Record.Data,oContext.Record.Data)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Data = fContext.Record.Data?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.Critical,wContext.Record.Critical,oContext.Record.Critical)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Critical = fContext.Record.Critical?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.BasicStats,wContext.Record.BasicStats,oContext.Record.BasicStats)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BasicStats = fContext.Record.BasicStats?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.ImpactDataSet,wContext.Record.ImpactDataSet,oContext.Record.ImpactDataSet)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImpactDataSet.SetTo(fContext.Record.ImpactDataSet);
                                }

                                if (Utility.ShouldChange(fContext.Record.AlternateBlockMaterial,wContext.Record.AlternateBlockMaterial,oContext.Record.AlternateBlockMaterial)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AlternateBlockMaterial.SetTo(fContext.Record.AlternateBlockMaterial);
                                }

                                if (Utility.ShouldChange(fContext.Record.DetectionSoundLevel,wContext.Record.DetectionSoundLevel,oContext.Record.DetectionSoundLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.DetectionSoundLevel = fContext.Record.DetectionSoundLevel;
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
                    if (Utility.TagCheck(Tags.Keywords, mapped, Settings, fContext))
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
                    if (Utility.TagCheck(Tags.Keywords, mapped, Settings, fContext))
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
