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
    internal class WeaponPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Enchantments,Graphics,Keywords,Names,ObjectBounds,Sounds,Stats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Weapon().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Destructible
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Destructible").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Destructible != null && Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                overrideObject.Destructible?.DeepCopyIn(foundContext.Record.Destructible);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Enchantments
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Enchantments").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ObjectEffect,originalObject.Record.ObjectEffect))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ObjectEffect,workingContext.Record.ObjectEffect)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectEffect != null && Compare.NotEqual(foundContext.Record.ObjectEffect,originalObject.Record.ObjectEffect))
                                overrideObject.ObjectEffect?.SetTo(foundContext.Record.ObjectEffect);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Graphics").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model)
                        || Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons)
                        || Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Icons,workingContext.Record.Icons)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.FirstPersonModel,workingContext.Record.FirstPersonModel)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null && Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                            if (foundContext.Record.Icons != null && Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons))
                                overrideObject.Icons?.DeepCopyIn(foundContext.Record.Icons);
                            if (foundContext.Record.FirstPersonModel != null && Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel))
                                overrideObject.FirstPersonModel?.SetTo(foundContext.Record.FirstPersonModel);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.HasTags("Keywords", out var FoundKeys))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && Compare.NotEqual(context.Record.Keywords,originalObject.Record.Keywords));
                    if (foundContext.Any())
                    {
                        Keywords NewKeywords = new(workingContext.Record.Keywords);
                        foreach (var context in foundContext)
                            NewKeywords.Add(context.Record.Keywords, originalObject.Record.Keywords);
                        foreach (var context in foundContext.Reverse())
                            NewKeywords.Remove(context.Record.Keywords, originalObject.Record.Keywords);
                        if (NewKeywords.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Keywords?.SetTo(NewKeywords.OverrideObject);
                        }
                    }
                }

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
                            if (foundContext.Record.Name != null && Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                overrideObject.Name?.Set(foundContext.Record.Name.TargetLanguage, foundContext.Record.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("ObjectBounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectBounds != null && Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                overrideObject.ObjectBounds?.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Sounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.AttackSound,originalObject.Record.AttackSound)
                        || Compare.NotEqual(foundContext.Record.AttackSound2D,originalObject.Record.AttackSound2D)
                        || Compare.NotEqual(foundContext.Record.AttackLoopSound,originalObject.Record.AttackLoopSound)
                        || Compare.NotEqual(foundContext.Record.AttackFailSound,originalObject.Record.AttackFailSound)
                        || Compare.NotEqual(foundContext.Record.IdleSound,originalObject.Record.IdleSound)
                        || Compare.NotEqual(foundContext.Record.EquipSound,originalObject.Record.EquipSound)
                        || Compare.NotEqual(foundContext.Record.UnequipSound,originalObject.Record.UnequipSound)
                        || Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound)
                        || Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound)
                        || Compare.NotEqual(foundContext.Record.DetectionSoundLevel,originalObject.Record.DetectionSoundLevel))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.AttackSound,workingContext.Record.AttackSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.AttackSound2D,workingContext.Record.AttackSound2D)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.AttackLoopSound,workingContext.Record.AttackLoopSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.AttackFailSound,workingContext.Record.AttackFailSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.IdleSound,workingContext.Record.IdleSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.EquipSound,workingContext.Record.EquipSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.UnequipSound,workingContext.Record.UnequipSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.PickUpSound,workingContext.Record.PickUpSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.PutDownSound,workingContext.Record.PutDownSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.DetectionSoundLevel,workingContext.Record.DetectionSoundLevel)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AttackSound != null && Compare.NotEqual(foundContext.Record.AttackSound,originalObject.Record.AttackSound))
                                overrideObject.AttackSound.SetTo(foundContext.Record.AttackSound);
                            if (foundContext.Record.AttackSound2D != null && Compare.NotEqual(foundContext.Record.AttackSound2D,originalObject.Record.AttackSound2D))
                                overrideObject.AttackSound2D.SetTo(foundContext.Record.AttackSound2D);
                            if (foundContext.Record.AttackLoopSound != null && Compare.NotEqual(foundContext.Record.AttackLoopSound,originalObject.Record.AttackLoopSound))
                                overrideObject.AttackLoopSound.SetTo(foundContext.Record.AttackLoopSound);
                            if (foundContext.Record.AttackFailSound != null && Compare.NotEqual(foundContext.Record.AttackFailSound,originalObject.Record.AttackFailSound))
                                overrideObject.AttackFailSound.SetTo(foundContext.Record.AttackFailSound);
                            if (foundContext.Record.IdleSound != null && Compare.NotEqual(foundContext.Record.IdleSound,originalObject.Record.IdleSound))
                                overrideObject.IdleSound.SetTo(foundContext.Record.IdleSound);
                            if (foundContext.Record.EquipSound != null && Compare.NotEqual(foundContext.Record.EquipSound,originalObject.Record.EquipSound))
                                overrideObject.EquipSound.SetTo(foundContext.Record.EquipSound);
                            if (foundContext.Record.UnequipSound != null && Compare.NotEqual(foundContext.Record.UnequipSound,originalObject.Record.UnequipSound))
                                overrideObject.UnequipSound.SetTo(foundContext.Record.UnequipSound);
                            if (foundContext.Record.PickUpSound != null && Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound))
                                overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                            if (foundContext.Record.PutDownSound != null && Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound))
                                overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
                            if (Compare.NotEqual(foundContext.Record.DetectionSoundLevel,originalObject.Record.DetectionSoundLevel))
                                overrideObject.DetectionSoundLevel = foundContext.Record.DetectionSoundLevel;
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Stats").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)
                        || Compare.NotEqual(foundContext.Record.Data,originalObject.Record.Data)
                        || Compare.NotEqual(foundContext.Record.Critical,originalObject.Record.Critical)
                        || Compare.NotEqual(foundContext.Record.BasicStats,originalObject.Record.BasicStats)
                        || Compare.NotEqual(foundContext.Record.ImpactDataSet,originalObject.Record.ImpactDataSet)
                        || Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,originalObject.Record.AlternateBlockMaterial)
                        || Compare.NotEqual(foundContext.Record.DetectionSoundLevel,originalObject.Record.DetectionSoundLevel))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.EditorID,workingContext.Record.EditorID)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Data,workingContext.Record.Data)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Critical,workingContext.Record.Critical)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.BasicStats,workingContext.Record.BasicStats)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.ImpactDataSet,workingContext.Record.ImpactDataSet)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,workingContext.Record.AlternateBlockMaterial)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.DetectionSoundLevel,workingContext.Record.DetectionSoundLevel)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.EditorID != null && Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID))
                                overrideObject.EditorID = foundContext.Record.EditorID;
                            if (foundContext.Record.Data != null && Compare.NotEqual(foundContext.Record.Data,originalObject.Record.Data))
                                overrideObject.Data?.DeepCopyIn(foundContext.Record.Data);
                            if (foundContext.Record.Critical != null && Compare.NotEqual(foundContext.Record.Critical,originalObject.Record.Critical))
                                overrideObject.Critical?.DeepCopyIn(foundContext.Record.Critical);
                            if (foundContext.Record.BasicStats != null && Compare.NotEqual(foundContext.Record.BasicStats,originalObject.Record.BasicStats))
                                overrideObject.BasicStats?.DeepCopyIn(foundContext.Record.BasicStats);
                            if (foundContext.Record.ImpactDataSet != null && Compare.NotEqual(foundContext.Record.ImpactDataSet,originalObject.Record.ImpactDataSet))
                                overrideObject.ImpactDataSet.SetTo(foundContext.Record.ImpactDataSet);
                            if (foundContext.Record.AlternateBlockMaterial != null && Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,originalObject.Record.AlternateBlockMaterial))
                                overrideObject.AlternateBlockMaterial.SetTo(foundContext.Record.AlternateBlockMaterial);
                            if (foundContext.Record.DetectionSoundLevel != null && Compare.NotEqual(foundContext.Record.DetectionSoundLevel,originalObject.Record.DetectionSoundLevel))
                                overrideObject.DetectionSoundLevel = foundContext.Record.DetectionSoundLevel;
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Text").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Description,workingContext.Record.Description)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Description != null && Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                                overrideObject.Description?.Set(foundContext.Record.Description.TargetLanguage, foundContext.Record.Description.String);
                        }
                        break;
                    }
                }
            
            }
        }
    }
}
