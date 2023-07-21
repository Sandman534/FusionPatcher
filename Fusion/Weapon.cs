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
        public List<ModKey> Stats = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
        public List<ModKey> Sounds = new();
    }

    internal class WeaponPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, WeaponSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Stats,
                Settings.Graphics,
                Settings.Keywords,
                Settings.Sounds
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Weapon().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                if (Settings.Stats.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Stats.Contains(context.ModKey)
                            && (context.Record.Name != originalObject.Record.Name
                                || context.Record.Data != originalObject.Record.Data
                                || context.Record.Critical != originalObject.Record.Critical
                                || context.Record.BasicStats != originalObject.Record.BasicStats
                                || context.Record.ImpactDataSet != originalObject.Record.ImpactDataSet
                                || context.Record.AlternateBlockMaterial != originalObject.Record.AlternateBlockMaterial
                                || context.Record.DetectionSoundLevel != originalObject.Record.DetectionSoundLevel))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.Name != null && overrideObject.Name != lastObject.Name)
                            overrideObject.Name = lastObject.Name.ToString();
                        if (lastObject.Data != null && overrideObject.Data != lastObject.Data) 
                            overrideObject.Data?.DeepCopyIn(lastObject.Data);
                        if (lastObject.Critical != null && overrideObject.Critical != lastObject.Critical) 
                            overrideObject.Critical?.DeepCopyIn(lastObject.Critical);
                        if (lastObject.BasicStats != null && overrideObject.BasicStats != lastObject.BasicStats) 
                            overrideObject.BasicStats?.DeepCopyIn(lastObject.BasicStats);
                        if (lastObject.ImpactDataSet != null && overrideObject.ImpactDataSet != lastObject.ImpactDataSet) 
                            overrideObject.ImpactDataSet.SetTo(lastObject.ImpactDataSet);
                        if (lastObject.AlternateBlockMaterial != null && overrideObject.AlternateBlockMaterial != lastObject.AlternateBlockMaterial) 
                            overrideObject.AlternateBlockMaterial.SetTo(lastObject.AlternateBlockMaterial);
                        if (lastObject.DetectionSoundLevel != null && overrideObject.DetectionSoundLevel != lastObject.DetectionSoundLevel) 
                            overrideObject.DetectionSoundLevel = lastObject.DetectionSoundLevel;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                if (Settings.Graphics.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Graphics.Contains(context.ModKey)
                            && (context.Record.Model != originalObject.Record.Model
                                || context.Record.FirstPersonModel != originalObject.Record.FirstPersonModel))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.Model != null && overrideObject.Model != lastObject.Model) 
                            overrideObject.Model?.DeepCopyIn(lastObject.Model);
                        if (lastObject.FirstPersonModel != null && overrideObject.FirstPersonModel != lastObject.FirstPersonModel) 
                            overrideObject.FirstPersonModel.SetTo(lastObject.FirstPersonModel);
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Keywords.Contains(context.ModKey) && context.Record.Keywords != originalObject.Record.Keywords)
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
                            if (listObject.Keywords != null && listObject.Keywords.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Keywords != null && originalObject.Record.Keywords.Count > 0 && overrideObject.Keywords?.Count > 0)
                                    foreach (var rec in originalObject.Record.Keywords)
                                        if (!listObject.Keywords.Contains(rec) && overrideObject.Keywords.Contains(rec))
                                            overrideObject.Keywords.Remove(rec);

                                // Add Items
                                foreach (var rec in listObject.Keywords)
                                    if (overrideObject.Keywords != null && !overrideObject.Keywords.Contains(rec))
                                        overrideObject.Keywords.Add(rec);
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                if (Settings.Sounds.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IWeapon, IWeaponGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Graphics.Contains(context.ModKey)
                            && (context.Record.AttackSound != originalObject.Record.AttackSound
                                || context.Record.AttackSound2D != originalObject.Record.AttackSound2D
                                || context.Record.AttackLoopSound != originalObject.Record.AttackLoopSound
                                || context.Record.AttackFailSound != originalObject.Record.AttackFailSound
                                || context.Record.IdleSound != originalObject.Record.IdleSound
                                || context.Record.EquipSound != originalObject.Record.EquipSound
                                || context.Record.UnequipSound != originalObject.Record.UnequipSound))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.AttackSound != null && overrideObject.AttackSound != lastObject.AttackSound) 
                            overrideObject.AttackSound.SetTo(lastObject.AttackSound);
                        if (lastObject.AttackSound2D != null && overrideObject.AttackSound2D != lastObject.AttackSound2D) 
                            overrideObject.AttackSound2D.SetTo(lastObject.AttackSound2D);
                        if (lastObject.AttackLoopSound != null && overrideObject.AttackLoopSound != lastObject.AttackLoopSound) 
                            overrideObject.AttackLoopSound.SetTo(lastObject.AttackLoopSound);
                        if (lastObject.AttackFailSound != null && overrideObject.AttackFailSound != lastObject.AttackFailSound) 
                            overrideObject.AttackFailSound.SetTo(lastObject.AttackFailSound);
                        if (lastObject.IdleSound != null && overrideObject.IdleSound != lastObject.IdleSound) 
                            overrideObject.IdleSound.SetTo(lastObject.IdleSound);
                        if (lastObject.EquipSound != null && overrideObject.EquipSound != lastObject.EquipSound) 
                            overrideObject.EquipSound.SetTo(lastObject.EquipSound);
                        if (lastObject.UnequipSound != null && overrideObject.UnequipSound != lastObject.UnequipSound) 
                            overrideObject.UnequipSound.SetTo(lastObject.UnequipSound);
                    }
                }
            }
        }
    }
}
