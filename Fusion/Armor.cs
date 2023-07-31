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
    public class ArmorSettings
    {
        public List<ModKey> Destructible = new();
        public List<ModKey> Enchantments = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
        public List<ModKey> Names = new();
        public List<ModKey> Stats = new();
        public List<ModKey> Sounds = new();
        public List<ModKey> Text = new();
    }
    internal class ArmorPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ArmorSettings Settings)
        {
            List<ModKey> modList = new() {
                Settings.Destructible, Settings.Enchantments, Settings.Graphics, Settings.Keywords, Settings.Names, Settings.Stats, Settings.Sounds, Settings.Text };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Armor().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey).Last();

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
                    if ((!foundContext.Record.WorldModel?.Equals(originalObject.Record.WorldModel) ?? false)
                        || !foundContext.Record.Armature.Equals(originalObject.Record.Armature))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.WorldModel?.Equals(originalObject.Record.WorldModel) ?? false) Change = true;
                        if (!foundContext.Record.Armature.Equals(originalObject.Record.Armature)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.WorldModel?.Male != null) overrideObject.WorldModel?.Male?.DeepCopyIn(foundContext.Record.WorldModel.Male);
                            if (foundContext.Record.WorldModel?.Female != null) overrideObject.WorldModel?.Female?.DeepCopyIn(foundContext.Record.WorldModel.Female);
                            if (foundContext.Record.Armature.Count > 0) {
                                overrideObject.Armature.Clear();
                                foreach (var armor in foundContext.Record.Armature) overrideObject.Armature.Add(armor); }
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
                        state.LinkCache.TryResolveContext<IArmor, IArmorGetter>(workingContext.Record.FormKey, out var patchRecord);
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
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Sounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.PickUpSound.Equals(originalObject.Record.PickUpSound)
                        || !foundContext.Record.PutDownSound.Equals(originalObject.Record.PutDownSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.PickUpSound.Equals(workingContext.Record.PickUpSound)) Change = true;
                        if (!foundContext.Record.PutDownSound.Equals(workingContext.Record.PutDownSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.PickUpSound != null) overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                            if (foundContext.Record.PutDownSound != null) overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
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
                        || !foundContext.Record.Value.Equals(originalObject.Record.Value)
                        || !foundContext.Record.Weight.Equals(originalObject.Record.Weight)
                        || !foundContext.Record.ArmorRating.Equals(originalObject.Record.ArmorRating)
                        || !foundContext.Record.BashImpactDataSet.Equals(originalObject.Record.BashImpactDataSet)
                        || !foundContext.Record.AlternateBlockMaterial.Equals(originalObject.Record.AlternateBlockMaterial))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.EditorID?.Equals(originalObject.Record.EditorID) ?? false) Change = true;
                        if (!foundContext.Record.Value.Equals(originalObject.Record.Value)) Change = true;
                        if (!foundContext.Record.Weight.Equals(originalObject.Record.Weight)) Change = true;
                        if (!foundContext.Record.ArmorRating.Equals(originalObject.Record.ArmorRating)) Change = true;
                        if (!foundContext.Record.BashImpactDataSet.Equals(originalObject.Record.BashImpactDataSet)) Change = true;
                        if (!foundContext.Record.AlternateBlockMaterial.Equals(originalObject.Record.AlternateBlockMaterial)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.EditorID != null) overrideObject.EditorID = foundContext.Record.EditorID;
                            if (overrideObject.Value != foundContext.Record.Value) overrideObject.Value = foundContext.Record.Value;
                            if (overrideObject.Weight != foundContext.Record.Weight) overrideObject.Weight = foundContext.Record.Weight;
                            if (overrideObject.ArmorRating != foundContext.Record.ArmorRating) overrideObject.ArmorRating = foundContext.Record.ArmorRating;
                            if (foundContext.Record.BashImpactDataSet != null) overrideObject.BashImpactDataSet.SetTo(foundContext.Record.BashImpactDataSet);
                            if (foundContext.Record.AlternateBlockMaterial != null) overrideObject.AlternateBlockMaterial.SetTo(foundContext.Record.AlternateBlockMaterial);
                            
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
