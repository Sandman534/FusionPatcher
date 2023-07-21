using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Immutable;
using System.Linq;

namespace Fusion
{
    public class NPCSettings
    {
        public List<ModKey> Appearence = new();
        public List<ModKey> Combat = new();
        public List<ModKey> Inventory = new();
        public List<ModKey> Factions = new();
        public List<ModKey> Outfits = new();
        public List<ModKey> Packages = new();
        public List<ModKey> Scripts = new();
    }

    internal class NPCPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, NPCSettings Settings)
        {
            List<ModKey> modList = new List<ModKey>
            {
                Settings.Appearence,
                Settings.Combat,
                Settings.Inventory,
                Settings.Factions,
                Settings.Outfits,
                Settings.Packages,
                Settings.Scripts
            };
            HashSet<ModKey> workingModList = new HashSet<ModKey>(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var testContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey)).ToImmutableHashSet();
                if (testContext == null || testContext.Count <= 0) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey).ToImmutableHashSet().First();

                //==============================================================================================================
                // Appearence
                //==============================================================================================================
                if (Settings.Appearence.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Appearence.Contains(context.ModKey)
                            && (context.Record.HeadParts != originalObject.Record.HeadParts
                                || context.Record.HairColor != originalObject.Record.HairColor
                                || context.Record.HeadTexture != originalObject.Record.HeadTexture
                                || context.Record.TextureLighting != originalObject.Record.TextureLighting
                                || context.Record.FaceMorph != originalObject.Record.FaceMorph
                                || context.Record.FaceParts != originalObject.Record.FaceParts
                                || context.Record.TintLayers != originalObject.Record.TintLayers))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;

                        // Copy Record
                        if (overrideObject.HeadParts.Count > 0)
                        {
                            overrideObject.HeadParts.Clear();
                            overrideObject.HeadParts.AddRange(lastObject.HeadParts);
                        }
                        if (lastObject.HairColor != null && overrideObject.HairColor != lastObject.HairColor) 
                            overrideObject.HairColor.SetTo(lastObject.HairColor);
                        if (lastObject.HeadTexture != null && overrideObject.HeadTexture != lastObject.HeadTexture) 
                            overrideObject.HeadTexture.SetTo(lastObject.HeadTexture);
                        if (lastObject.TextureLighting != null && overrideObject.TextureLighting != lastObject.TextureLighting) 
                            overrideObject.TextureLighting = lastObject.TextureLighting;
                        if (lastObject.FaceMorph != null && overrideObject.FaceMorph != lastObject.FaceMorph) 
                            overrideObject.FaceMorph?.DeepCopyIn(lastObject.FaceMorph);
                        if (lastObject.FaceParts != null && overrideObject.FaceParts != lastObject.FaceParts) 
                            overrideObject.FaceParts?.DeepCopyIn(lastObject.FaceParts);
                        if (overrideObject.TintLayers?.Count > 0 && lastObject.TintLayers?.Count > 0)
                        {
                            overrideObject.TintLayers.Clear();
                            foreach (var tint in lastObject.TintLayers) overrideObject.TintLayers.Add(tint.DeepCopy());
                        }
                    }
                }

                //==============================================================================================================
                // Combat
                //==============================================================================================================
                if (Settings.Combat.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Combat.Contains(context.ModKey)
                            && (context.Record.PlayerSkills != originalObject.Record.PlayerSkills
                              || context.Record.CombatStyle != originalObject.Record.CombatStyle))
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
                            if (listObject.Items != null && listObject.Items.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Items != null && originalObject.Record.Items.Count > 0 && overrideObject.Items?.Count > 0)
                                    foreach (var rec in originalObject.Record.Items)
                                        if (!listObject.Items.Contains(rec) && overrideObject.Items.Contains(rec))
                                            overrideObject.Items.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.Items)
                                    if (overrideObject.Items != null && !overrideObject.Items.Contains(rec))
                                        overrideObject.Items.Add(rec.DeepCopy());
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Inventory
                //==============================================================================================================
                if (Settings.Inventory.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Inventory.Contains(context.ModKey)
                            && (context.Record.Items != originalObject.Record.Items))
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
                            if (listObject.Items != null && listObject.Items.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Items != null && originalObject.Record.Items.Count > 0 && overrideObject.Items?.Count > 0)
                                    foreach (var rec in originalObject.Record.Items)
                                        if (!listObject.Items.Contains(rec) && overrideObject.Items.Contains(rec))
                                            overrideObject.Items.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.Items)
                                    if (overrideObject.Items != null && !overrideObject.Items.Contains(rec))
                                        overrideObject.Items.Add(rec.DeepCopy());
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Factions
                //==============================================================================================================
                if (Settings.Factions.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Factions.Contains(context.ModKey)
                            && (context.Record.Factions != originalObject.Record.Factions
                                || context.Record.CrimeFaction != originalObject.Record.CrimeFaction))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;
                        if (lastObject.CrimeFaction != null && overrideObject.CrimeFaction != lastObject.CrimeFaction)
                            overrideObject.CrimeFaction.SetTo(lastObject.CrimeFaction);

                        // Add All Records
                        foreach (var context in foundContext)
                        {
                            var listObject = context.Record;
                            if (listObject.Factions != null && listObject.Factions.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Factions != null && originalObject.Record.Factions.Count > 0 && overrideObject.Factions?.Count > 0)
                                    foreach (var rec in originalObject.Record.Factions)
                                        if (!listObject.Factions.Contains(rec) && overrideObject.Factions.Contains(rec))
                                            overrideObject.Factions.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.Factions)
                                    if (overrideObject.Factions != null && !overrideObject.Factions.Contains(rec))
                                        overrideObject.Factions.Add(rec.DeepCopy());
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Outfit
                //==============================================================================================================
                if (Settings.Outfits.Count > 0)
                {
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Appearence.Contains(context.ModKey)
                            && (context.Record.DefaultOutfit != originalObject.Record.DefaultOutfit
                                || context.Record.SleepingOutfit != originalObject.Record.SleepingOutfit))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var lastObject = foundContext.Last().Record;
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);

                        // Copy Record
                        if (lastObject.DefaultOutfit != null && overrideObject.DefaultOutfit != lastObject.DefaultOutfit) 
                            overrideObject.DefaultOutfit.SetTo(lastObject.DefaultOutfit);
                        if (lastObject.SleepingOutfit != null && overrideObject.SleepingOutfit != lastObject.SleepingOutfit) 
                            overrideObject.SleepingOutfit.SetTo(lastObject.SleepingOutfit);
                    }
                }

                //==============================================================================================================
                // Packages
                //==============================================================================================================
                if (Settings.Packages.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Packages.Contains(context.ModKey)
                            && (context.Record.Packages != originalObject.Record.Packages
                                || context.Record.DefaultPackageList != originalObject.Record.DefaultPackageList))
                        .ToImmutableHashSet();

                    // If we found records to modify
                    if (foundContext.Count > 0 && foundContext.Last().ModKey != workingContext.ModKey)
                    {
                        // Last and New Record
                        var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                        var lastObject = foundContext.Last().Record;
                        if (lastObject.DefaultPackageList != null && overrideObject.DefaultPackageList != lastObject.DefaultPackageList)
                            overrideObject.DefaultPackageList.SetTo(lastObject.DefaultPackageList);

                        // Add All Records
                        foreach (var context in foundContext)
                        {
                            var listObject = context.Record;
                            if (listObject.Packages != null && listObject.Packages.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.Packages != null && originalObject.Record.Packages.Count > 0 && overrideObject.Packages?.Count > 0)
                                    foreach (var rec in originalObject.Record.Packages)
                                        if (!listObject.Packages.Contains(rec) && overrideObject.Packages.Contains(rec))
                                            overrideObject.Packages.Remove(rec);

                                // Add Items
                                foreach (var rec in listObject.Packages)
                                    if (overrideObject.Packages != null && !overrideObject.Packages.Contains(rec))
                                        overrideObject.Packages.Add(rec);
                            }
                        }
                    }
                }

                //==============================================================================================================
                // Scripts
                //==============================================================================================================
                if (Settings.Scripts.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey)
                        .Where(context => Settings.Scripts.Contains(context.ModKey)
                            && (context.Record.VirtualMachineAdapter != originalObject.Record.VirtualMachineAdapter))
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
                            if (listObject.VirtualMachineAdapter != null && listObject.VirtualMachineAdapter.Scripts.Count > 0)
                            {
                                // Remove Items
                                if (originalObject.Record.VirtualMachineAdapter != null && originalObject.Record.VirtualMachineAdapter.Scripts.Count > 0 && overrideObject.VirtualMachineAdapter?.Scripts?.Count > 0)
                                    foreach (var rec in originalObject.Record.VirtualMachineAdapter.Scripts)
                                        if (!listObject.VirtualMachineAdapter.Scripts.Contains(rec) && overrideObject.VirtualMachineAdapter.Scripts.Contains(rec)) overrideObject.VirtualMachineAdapter.Scripts.Remove(rec.DeepCopy());

                                // Add Items
                                foreach (var rec in listObject.VirtualMachineAdapter.Scripts)
                                    if (overrideObject.VirtualMachineAdapter?.Scripts != null && !overrideObject.VirtualMachineAdapter.Scripts.Contains(rec)) overrideObject.VirtualMachineAdapter.Scripts.Add(rec.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
    }
}
