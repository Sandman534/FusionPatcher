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
        public List<ModKey> Stats = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
    }

    internal class ArmorPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ArmorSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Stats,
                Settings.Graphics,
                Settings.Keywords
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Armor().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                if (Settings.Stats.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Stats.Contains(context.ModKey)
                            && (context.Record.Name != originalObject.Record.Name
                                || context.Record.Value != originalObject.Record.Value
                                || context.Record.Weight != originalObject.Record.Weight
                                || context.Record.ArmorRating != originalObject.Record.ArmorRating
                                || context.Record.BashImpactDataSet != originalObject.Record.BashImpactDataSet
                                || context.Record.AlternateBlockMaterial != originalObject.Record.AlternateBlockMaterial))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Record
                        overrideObject.Value = lastObject.Value;
                        overrideObject.Weight = lastObject.Weight;
                        overrideObject.ArmorRating = lastObject.ArmorRating;
                        if (lastObject.Name != null && overrideObject.Name != lastObject.Name)
                            overrideObject.Name = lastObject.Name.ToString();
                        if (lastObject.BashImpactDataSet != null && overrideObject.BashImpactDataSet != lastObject.BashImpactDataSet)
                            overrideObject.BashImpactDataSet.SetTo(lastObject.BashImpactDataSet);
                        if (lastObject.AlternateBlockMaterial != null && overrideObject.AlternateBlockMaterial != lastObject.AlternateBlockMaterial) 
                            overrideObject.AlternateBlockMaterial.SetTo(lastObject.AlternateBlockMaterial);
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                if (Settings.Graphics.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Graphics.Contains(context.ModKey)
                            && (context.Record.WorldModel != originalObject.Record.WorldModel
                                || context.Record.Armature != originalObject.Record.Armature))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Records
                        if (lastObject.WorldModel?.Male != null && overrideObject.WorldModel?.Male != lastObject.WorldModel.Male) 
                            overrideObject.WorldModel?.Male?.DeepCopyIn(lastObject.WorldModel.Male);
                        if (lastObject.WorldModel?.Female != null && overrideObject.WorldModel?.Female != lastObject.WorldModel.Female)
                            overrideObject.WorldModel?.Female?.DeepCopyIn(lastObject.WorldModel.Female);
                        if (lastObject.Armature.Count > 0)
                        {
                            overrideObject.Armature.Clear();
                            foreach (var armor in lastObject.Armature) 
                                overrideObject.Armature.Add(armor);
                        }
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey)
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
            }
        }
    }
}
