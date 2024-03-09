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
    internal class WEAP
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            Console.WriteLine("Processing Weapon");
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Enchantments,Graphics,Keywords,Names,ObjectBounds,Sound,Stats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Weapon().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey).Last();
                MappedTags mapped = new MappedTags();
                Keywords NewKeywords = new(workingContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (mapped.NotMapped("Destructible") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                        overrideObject.Destructible = foundContext.Record.Destructible?.DeepCopy();
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Enchantments
                    //==============================================================================================================
                    if (mapped.NotMapped("Enchantments") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectEffect,originalObject.Record.ObjectEffect))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectEffect,workingContext.Record.ObjectEffect)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ObjectEffect,originalObject.Record.ObjectEffect))
                                        overrideObject.ObjectEffect.SetTo(foundContext.Record.ObjectEffect);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (mapped.NotMapped("Graphics") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model)
                            || Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons)
                            || Compare.NotEqual(foundContext.Record.FirstPersonModel,originalObject.Record.FirstPersonModel))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else {
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
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (mapped.NotMapped("Names") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                        overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (mapped.NotMapped("ObjectBounds") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
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
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (mapped.NotMapped("Sound") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
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
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
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
                                    if (Compare.NotEqual(foundContext.Record.AttackSound,originalObject.Record.AttackSound))
                                        overrideObject.AttackSound.SetTo(foundContext.Record.AttackSound);
                                    if (Compare.NotEqual(foundContext.Record.AttackSound2D,originalObject.Record.AttackSound2D))
                                        overrideObject.AttackSound2D.SetTo(foundContext.Record.AttackSound2D);
                                    if (Compare.NotEqual(foundContext.Record.AttackLoopSound,originalObject.Record.AttackLoopSound))
                                        overrideObject.AttackLoopSound.SetTo(foundContext.Record.AttackLoopSound);
                                    if (Compare.NotEqual(foundContext.Record.AttackFailSound,originalObject.Record.AttackFailSound))
                                        overrideObject.AttackFailSound.SetTo(foundContext.Record.AttackFailSound);
                                    if (Compare.NotEqual(foundContext.Record.IdleSound,originalObject.Record.IdleSound))
                                        overrideObject.IdleSound.SetTo(foundContext.Record.IdleSound);
                                    if (Compare.NotEqual(foundContext.Record.EquipSound,originalObject.Record.EquipSound))
                                        overrideObject.EquipSound.SetTo(foundContext.Record.EquipSound);
                                    if (Compare.NotEqual(foundContext.Record.UnequipSound,originalObject.Record.UnequipSound))
                                        overrideObject.UnequipSound.SetTo(foundContext.Record.UnequipSound);
                                    if (Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound))
                                        overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                                    if (Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound))
                                        overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
                                    if (Compare.NotEqual(foundContext.Record.DetectionSoundLevel,originalObject.Record.DetectionSoundLevel))
                                        overrideObject.DetectionSoundLevel = foundContext.Record.DetectionSoundLevel;
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (mapped.NotMapped("Stats") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
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
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
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
                                    if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID))
                                        overrideObject.EditorID = foundContext.Record.EditorID;
                                    if (Compare.NotEqual(foundContext.Record.Data,originalObject.Record.Data))
                                        overrideObject.Data = foundContext.Record.Data?.DeepCopy();
                                    if (Compare.NotEqual(foundContext.Record.Critical,originalObject.Record.Critical))
                                        overrideObject.Critical = foundContext.Record.Critical?.DeepCopy();
                                    if (Compare.NotEqual(foundContext.Record.BasicStats,originalObject.Record.BasicStats))
                                        overrideObject.BasicStats = foundContext.Record.BasicStats?.DeepCopy();
                                    if (Compare.NotEqual(foundContext.Record.ImpactDataSet,originalObject.Record.ImpactDataSet))
                                        overrideObject.ImpactDataSet.SetTo(foundContext.Record.ImpactDataSet);
                                    if (Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,originalObject.Record.AlternateBlockMaterial))
                                        overrideObject.AlternateBlockMaterial.SetTo(foundContext.Record.AlternateBlockMaterial);
                                    if (Compare.NotEqual(foundContext.Record.DetectionSoundLevel,originalObject.Record.DetectionSoundLevel))
                                        overrideObject.DetectionSoundLevel = foundContext.Record.DetectionSoundLevel;
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Text
                    //==============================================================================================================
                    if (mapped.NotMapped("Text") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Description,workingContext.Record.Description)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                                        overrideObject.Description = Utility.NewString(foundContext.Record.Description);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                                NewKeywords.Add(foundContext.Record.Keywords, originalObject.Record.Keywords);
                }    

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                            NewKeywords.Remove(foundContext.Record.Keywords, originalObject.Record.Keywords);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewKeywords.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                

            }
        }
    }
}
