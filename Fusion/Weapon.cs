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
    public class WeaponSettings
    {
        public List<ModKey> Destructible = new();
        public List<ModKey> Enchantments = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
        public List<ModKey> Names = new();
        public List<ModKey> ObjectBounds = new();
        public List<ModKey> Stats = new();
        public List<ModKey> Sounds = new();
        public List<ModKey> Text = new();
    }

    internal class WeaponPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, WeaponSettings Settings)
        {
            List<ModKey> modList = new() {
                Settings.Destructible, Settings.Enchantments, Settings.Graphics, Settings.Keywords, Settings.Names, Settings.ObjectBounds,
                Settings.Stats, Settings.Sounds, Settings.Text };
            HashSet<ModKey> workingModList = new(modList);

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
                foreach(var foundContext in modContext.Where(context => Settings.Destructible.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Destructible?.Equals(originalObject.Record.Destructible) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Destructible?.Equals(workingContext.Record.Destructible) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Destructible != null) overrideObject.Destructible?.DeepCopyIn(foundContext.Record.Destructible);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Enchantments
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Enchantments.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ObjectEffect?.Equals(originalObject.Record.ObjectEffect) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ObjectEffect?.Equals(workingContext.Record.ObjectEffect) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectEffect != null) overrideObject.ObjectEffect?.SetTo(foundContext.Record.ObjectEffect);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Graphics.Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false)
                        || !foundContext.Record.FirstPersonModel.Equals(originalObject.Record.FirstPersonModel)
                        || (!foundContext.Record.Icons?.Equals(originalObject.Record.Icons) ?? false))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false) Change = true;
                        if (!foundContext.Record.FirstPersonModel.Equals(originalObject.Record.FirstPersonModel)) Change = true;
                        if (!foundContext.Record.Icons?.Equals(originalObject.Record.Icons) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null) overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                            if (foundContext.Record.FirstPersonModel != null) overrideObject.FirstPersonModel.SetTo(foundContext.Record.FirstPersonModel);
                            if (foundContext.Record.Icons != null) overrideObject.Icons?.DeepCopyIn(foundContext.Record.Icons);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.Keywords.Contains(context.ModKey) && ((!context.Record.Keywords?.Equals(originalObject.Record.Keywords) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IWeapon, IWeaponGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Keywords NewKeywords = new(patchRecord?.Record.Keywords, workingContext.Record.Keywords);
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
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.ObjectBounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ObjectBounds?.Equals(originalObject.Record.ObjectBounds) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ObjectBounds?.Equals(workingContext.Record.ObjectBounds) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectBounds != null) overrideObject.ObjectBounds?.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Sounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.AttackSound.Equals(originalObject.Record.AttackSound)
                        || !foundContext.Record.AttackSound2D.Equals(originalObject.Record.AttackSound2D)
                        || !foundContext.Record.AttackLoopSound.Equals(originalObject.Record.AttackLoopSound)
                        || !foundContext.Record.AttackFailSound.Equals(originalObject.Record.AttackFailSound)
                        || !foundContext.Record.IdleSound.Equals(originalObject.Record.IdleSound)
                        || !foundContext.Record.EquipSound.Equals(originalObject.Record.EquipSound)
                        || !foundContext.Record.UnequipSound.Equals(originalObject.Record.UnequipSound)
                        || !foundContext.Record.PickUpSound.Equals(originalObject.Record.PickUpSound)
                        || !foundContext.Record.PutDownSound.Equals(originalObject.Record.PutDownSound)
                        || !foundContext.Record.DetectionSoundLevel.Equals(originalObject.Record.DetectionSoundLevel))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.AttackSound.Equals(originalObject.Record.AttackSound)) Change = true;
                        if (!foundContext.Record.AttackSound2D.Equals(originalObject.Record.AttackSound2D)) Change = true;
                        if (!foundContext.Record.AttackLoopSound.Equals(originalObject.Record.AttackLoopSound)) Change = true;
                        if (!foundContext.Record.AttackFailSound.Equals(originalObject.Record.AttackFailSound)) Change = true;
                        if (!foundContext.Record.IdleSound.Equals(originalObject.Record.IdleSound)) Change = true;
                        if (!foundContext.Record.EquipSound.Equals(originalObject.Record.EquipSound)) Change = true;
                        if (!foundContext.Record.UnequipSound.Equals(originalObject.Record.UnequipSound)) Change = true;
                        if (!foundContext.Record.PickUpSound.Equals(originalObject.Record.PickUpSound)) Change = true;
                        if (!foundContext.Record.PutDownSound.Equals(originalObject.Record.PutDownSound)) Change = true;
                        if (!foundContext.Record.DetectionSoundLevel.Equals(originalObject.Record.DetectionSoundLevel)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AttackSound != null) overrideObject.AttackSound.SetTo(foundContext.Record.AttackSound);
                            if (foundContext.Record.AttackSound2D != null) overrideObject.AttackSound2D.SetTo(foundContext.Record.AttackSound2D);
                            if (foundContext.Record.AttackLoopSound != null) overrideObject.AttackLoopSound.SetTo(foundContext.Record.AttackLoopSound);
                            if (foundContext.Record.AttackFailSound != null) overrideObject.AttackFailSound.SetTo(foundContext.Record.AttackFailSound);
                            if (foundContext.Record.IdleSound != null) overrideObject.IdleSound.SetTo(foundContext.Record.IdleSound);
                            if (foundContext.Record.EquipSound != null) overrideObject.EquipSound.SetTo(foundContext.Record.EquipSound);
                            if (foundContext.Record.UnequipSound != null) overrideObject.UnequipSound.SetTo(foundContext.Record.UnequipSound);
                            if (foundContext.Record.PickUpSound != null) overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                            if (foundContext.Record.PutDownSound != null) overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
                            if (foundContext.Record.DetectionSoundLevel != null) overrideObject.DetectionSoundLevel = foundContext.Record.DetectionSoundLevel;
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Stats.Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.EditorID?.Equals(originalObject.Record.EditorID) ?? false)
                        || (!foundContext.Record.Data?.Equals(originalObject.Record.Data) ?? false)
                        || (!foundContext.Record.Critical?.Equals(originalObject.Record.Critical) ?? false)
                        || (!foundContext.Record.BasicStats?.Equals(originalObject.Record.BasicStats) ?? false)
                        || !foundContext.Record.ImpactDataSet.Equals(originalObject.Record.ImpactDataSet)
                        || !foundContext.Record.AlternateBlockMaterial.Equals(originalObject.Record.AlternateBlockMaterial)
                        || !foundContext.Record.DetectionSoundLevel.Equals(originalObject.Record.DetectionSoundLevel))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.EditorID?.Equals(originalObject.Record.EditorID) ?? false) Change = true;
                        if (!foundContext.Record.Data?.Equals(originalObject.Record.Data) ?? false) Change = true;
                        if (!foundContext.Record.Critical?.Equals(originalObject.Record.Critical) ?? false) Change = true;
                        if (!foundContext.Record.BasicStats?.Equals(originalObject.Record.BasicStats) ?? false) Change = true;
                        if (!foundContext.Record.ImpactDataSet.Equals(originalObject.Record.ImpactDataSet)) Change = true;
                        if (!foundContext.Record.AlternateBlockMaterial.Equals(originalObject.Record.AlternateBlockMaterial)) Change = true;
                        if (!foundContext.Record.DetectionSoundLevel.Equals(originalObject.Record.DetectionSoundLevel)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.EditorID != null) overrideObject.EditorID = foundContext.Record.EditorID;
                            if (foundContext.Record.Data != null) overrideObject.Data?.DeepCopyIn(foundContext.Record.Data);
                            if (foundContext.Record.Critical != null) overrideObject.Critical?.DeepCopyIn(foundContext.Record.Critical);
                            if (foundContext.Record.BasicStats != null) overrideObject.BasicStats?.DeepCopyIn(foundContext.Record.BasicStats);
                            if (foundContext.Record.ImpactDataSet != null) overrideObject.ImpactDataSet.SetTo(foundContext.Record.ImpactDataSet);
                            if (foundContext.Record.AlternateBlockMaterial != null) overrideObject.AlternateBlockMaterial.SetTo(foundContext.Record.AlternateBlockMaterial);
                            if (foundContext.Record.DetectionSoundLevel != null) overrideObject.DetectionSoundLevel = foundContext.Record.DetectionSoundLevel;
                            
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
