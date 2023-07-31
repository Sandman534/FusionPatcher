using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace Fusion
{

    public class NPCSettings
    {
        public List<ModKey> ACBS = new();
        public List<ModKey> AIData = new();
        public List<ModKey> AIPackages = new();
        public List<ModKey> AIPackagesForceAdd = new();
        public List<ModKey> AIPackagesOverrides = new();
        public List<ModKey> AttackRace = new();
        public List<ModKey> Class = new();
        public List<ModKey> CombatStyle = new();
        public List<ModKey> CrimeFaction = new();
        public List<ModKey> DeathItem = new();
        public List<ModKey> DefaultOutfit = new();
        public List<ModKey> Factions = new ();
        public List<ModKey> InventAdd = new();
        public List<ModKey> InventChange = new();
        public List<ModKey> InventRemove = new();
        public List<ModKey> Keywords = new();
        public List<ModKey> Names = new();
        public List<ModKey> NpcFacesForceFullImport = new();
        public List<ModKey> PerksAdd = new ();
        public List<ModKey> PerksChange = new ();
        public List<ModKey> PerksRemove = new ();
        public List<ModKey> Race = new();
        public List<ModKey> RecordFlags = new();
        public List<ModKey> Stats = new();
        public List<ModKey> Voice = new();   
    }

    internal class NPCPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, NPCSettings Settings)
        {
           List<ModKey> modList = new()
          {
                Settings.ACBS,Settings.AIData,Settings.AIPackages,Settings.AIPackagesForceAdd,Settings.AIPackagesOverrides,Settings.AttackRace,
                Settings.Class, Settings.CombatStyle, Settings.CrimeFaction, Settings.DeathItem, Settings.DefaultOutfit, Settings.Factions,
                Settings.InventAdd,Settings.InventChange, Settings.InventRemove,Settings.Names, Settings.NpcFacesForceFullImport, Settings.PerksAdd,
                Settings.PerksChange, Settings.PerksRemove,Settings.Race, Settings.RecordFlags, Settings.Voice, Settings.Keywords
            };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // ACBS
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.ACBS.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Configuration.Equals(originalObject.Record.Configuration))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Configuration.Equals(workingContext.Record.Configuration)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Configuration != null) overrideObject.Configuration.DeepCopyIn(foundContext.Record.Configuration);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // AI Data
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.AIData.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.AIData.Equals(originalObject.Record.AIData))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.AIData.Equals(workingContext.Record.AIData)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AIData != null) overrideObject.AIData.DeepCopyIn(foundContext.Record.AIData);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // AI Packages
                //==============================================================================================================
                if (Settings.AIPackages.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.AIPackages.Contains(context.ModKey) && ((!context.Record.Packages?.Equals(originalObject.Record.Packages) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<IFormLinkGetter<IPackageGetter>> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Packages != null)
                            foreach (var rec in patchRecord.Record.Packages)
                                overrideObject.Add(rec);
                        else if (workingContext.Record.Packages != null)
                            foreach (var rec in workingContext.Record.Packages)
                                overrideObject.Add(rec);

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Packages != null && context.Record.Packages.Count > 0)
                                foreach (var rec in context.Record.Packages)
                                    if (originalObject.Record.Packages != null && !originalObject.Record.Packages.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec);
                                        Change = true;
                                    }
                        }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Packages != null && context.Record.Packages.Count > 0)
                                if (originalObject.Record.Packages != null && originalObject.Record.Packages.Count > 0 && originalObject.Record.Packages?.Count > 0)
                                    foreach (var rec in originalObject.Record.Packages)
                                        if (!context.Record.Packages.Contains(rec) && overrideObject.Contains(rec))
                                        {
                                            overrideObject.Remove(rec);
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Packages?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // AI Packages Force Add
                //==============================================================================================================
                if (Settings.AIPackagesForceAdd.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.AIPackagesForceAdd.Contains(context.ModKey) && ((!context.Record.Packages?.Equals(originalObject.Record.Packages) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<IFormLinkGetter<IPackageGetter>> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Packages != null)
                            foreach (var rec in patchRecord.Record.Packages)
                                overrideObject.Add(rec);
                        else if (workingContext.Record.Packages != null)
                            foreach (var rec in workingContext.Record.Packages)
                                overrideObject.Add(rec);

                        // Add Records
                        bool bChangeMade = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Packages != null && context.Record.Packages.Count > 0)
                                foreach (var rec in context.Record.Packages)
                                    if (originalObject.Record.Packages != null && !originalObject.Record.Packages.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec);
                                        bChangeMade = true;
                                    }
                        }

                        // If changes were made, override and write back
                        if (bChangeMade)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Packages?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // AI Package Overrride
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.AIPackagesOverrides.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.SpectatorOverridePackageList.Equals(originalObject.Record.SpectatorOverridePackageList)
                        || !foundContext.Record.ObserveDeadBodyOverridePackageList.Equals(originalObject.Record.ObserveDeadBodyOverridePackageList)
                        || !foundContext.Record.GuardWarnOverridePackageList.Equals(originalObject.Record.GuardWarnOverridePackageList)
                        || !foundContext.Record.CombatOverridePackageList.Equals(originalObject.Record.CombatOverridePackageList))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.SpectatorOverridePackageList.Equals(workingContext.Record.SpectatorOverridePackageList)) Change = true;
                        if (!foundContext.Record.ObserveDeadBodyOverridePackageList.Equals(workingContext.Record.ObserveDeadBodyOverridePackageList)) Change = true;
                        if (!foundContext.Record.GuardWarnOverridePackageList.Equals(workingContext.Record.GuardWarnOverridePackageList)) Change = true;
                        if (!foundContext.Record.CombatOverridePackageList.Equals(workingContext.Record.CombatOverridePackageList)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.SpectatorOverridePackageList != null) overrideObject.SpectatorOverridePackageList.SetTo(foundContext.Record.SpectatorOverridePackageList);
                            if (foundContext.Record.ObserveDeadBodyOverridePackageList != null) overrideObject.ObserveDeadBodyOverridePackageList.SetTo(foundContext.Record.ObserveDeadBodyOverridePackageList);
                            if (foundContext.Record.GuardWarnOverridePackageList != null) overrideObject.GuardWarnOverridePackageList.SetTo(foundContext.Record.GuardWarnOverridePackageList);
                            if (foundContext.Record.CombatOverridePackageList != null) overrideObject.CombatOverridePackageList.SetTo(foundContext.Record.CombatOverridePackageList);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Attack Race
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.AttackRace.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.AttackRace.Equals(originalObject.Record.AttackRace))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.AttackRace.Equals(workingContext.Record.AttackRace)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AttackRace != null) overrideObject.AttackRace.SetTo(foundContext.Record.AttackRace);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Class
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Class.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Class.Equals(originalObject.Record.Class))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Class.Equals(workingContext.Record.Class)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Class != null) overrideObject.Class.SetTo(foundContext.Record.Class);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Combat Style
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.CombatStyle.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.CombatStyle.Equals(originalObject.Record.CombatStyle))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.CombatStyle.Equals(workingContext.Record.CombatStyle)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.CombatStyle != null) overrideObject.CombatStyle.SetTo(foundContext.Record.CombatStyle);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Crime Faction
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.CrimeFaction.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.CrimeFaction.Equals(originalObject.Record.CrimeFaction))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.CrimeFaction.Equals(workingContext.Record.CrimeFaction)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.CrimeFaction != null) overrideObject.CrimeFaction.SetTo(foundContext.Record.CrimeFaction);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Death Items
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.DeathItem.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.DeathItem.Equals(originalObject.Record.DeathItem))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.DeathItem.Equals(workingContext.Record.DeathItem)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.DeathItem != null) overrideObject.DeathItem.SetTo(foundContext.Record.DeathItem);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Default Outfit
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.DefaultOutfit.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.DefaultOutfit.Equals(originalObject.Record.DefaultOutfit))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.DefaultOutfit.Equals(workingContext.Record.DefaultOutfit)) Change = true;
                        if (!foundContext.Record.Template.Equals(workingContext.Record.Template)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.DefaultOutfit != null) overrideObject.DefaultOutfit.SetTo(foundContext.Record.DefaultOutfit);
                            if (foundContext.Record.Template != null) overrideObject.Template.SetTo(foundContext.Record.Template);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Factions
                //==============================================================================================================
                if (Settings.Factions.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.Factions.Contains(context.ModKey) && ((!context.Record.Factions?.Equals(originalObject.Record.Factions) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<RankPlacement> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Factions != null)
                            foreach (var rec in patchRecord.Record.Factions)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Factions != null)
                            foreach (var rec in workingContext.Record.Factions)
                                overrideObject.Add(rec.DeepCopy());

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Factions != null && context.Record.Factions.Count > 0)
                                foreach (var rec in context.Record.Factions)
                                    if (originalObject.Record.Factions != null && !originalObject.Record.Factions.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec.DeepCopy());
                                        Change = true;
                                    }
                        }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Factions != null && context.Record.Factions.Count > 0)
                                if (originalObject.Record.Factions != null && originalObject.Record.Factions.Count > 0 && originalObject.Record.Factions?.Count > 0)
                                    foreach (var rec in originalObject.Record.Factions)
                                        if (!context.Record.Factions.Contains(rec) && overrideObject.Contains(rec))
                                        {
                                            overrideObject.Remove(rec.DeepCopy());
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Factions?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Add
                //==============================================================================================================
                if (Settings.InventAdd.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.InventAdd.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<ContainerEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Items != null)
                            foreach (var rec in patchRecord.Record.Items)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Items != null)
                            foreach (var rec in workingContext.Record.Items)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext)
                            if (context.Record.Items != null && context.Record.Items.Count > 0)
                                foreach (var rec in context.Record.Items)
                                    if (!overrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                                    {
                                        overrideObject.Add(rec.DeepCopy());
                                        Change = true;
                                    }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Remove
                //==============================================================================================================
                if (Settings.InventRemove.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.InventRemove.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<ContainerEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Items != null)
                            foreach (var rec in patchRecord.Record.Items)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Items != null)
                            foreach (var rec in workingContext.Record.Items)
                                overrideObject.Add(rec.DeepCopy());

                        // Add All Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Items != null && context.Record.Items.Count > 0)
                                // Remove Items
                                if (originalObject.Record.Items != null && originalObject.Record.Items.Count > 0)
                                    foreach (var rec in originalObject.Record.Items)
                                        if (!context.Record.Items.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                                        {
                                            var oFoundRec = overrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                                            if (oFoundRec.Any())
                                            {
                                                overrideObject.Remove(oFoundRec.First());
                                                Change = true;
                                            }
                                        }
                        }

                        // If changes were made, override and write bac
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Change
                //==============================================================================================================
                if (Settings.InventChange.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.InventChange.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<ContainerEntry> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Items != null)
                            foreach (var rec in patchRecord.Record.Items)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Items != null)
                            foreach (var rec in workingContext.Record.Items)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Items != null && originalObject.Record.Items != null)
                                foreach (var rec in context.Record.Items)
                                    if (!originalObject.Record.Items.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey && x.Item.Count == rec.Item.Count).Any())
                                    {
                                        var oFoundRec = overrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                                        if (oFoundRec.First() != null && oFoundRec.First().Item.Count != rec.Item.Count)
                                        {
                                            overrideObject.Remove(oFoundRec.First());
                                            overrideObject.Add(rec.DeepCopy());
                                            Change = true;
                                        }
                                    }
                            }

                        // If changes were made, override and write bac
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(overrideObject);
                        }
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
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<IFormLinkGetter<IKeywordGetter>> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Keywords != null)
                            foreach (var rec in patchRecord.Record.Keywords)
                                overrideObject.Add(rec);
                        else if (workingContext.Record.Keywords != null)
                            foreach (var rec in workingContext.Record.Keywords)
                                overrideObject.Add(rec);

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Keywords != null && context.Record.Keywords.Count > 0)
                                foreach (var rec in context.Record.Keywords)
                                    if (originalObject.Record.Keywords != null && !originalObject.Record.Keywords.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec);
                                        Change = true;
                                    }
                        }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Keywords != null && context.Record.Keywords.Count > 0)
                                if (originalObject.Record.Keywords != null && originalObject.Record.Keywords.Count > 0 && originalObject.Record.Keywords?.Count > 0)
                                    foreach (var rec in originalObject.Record.Keywords)
                                        if (!context.Record.Keywords.Contains(rec) && overrideObject.Contains(rec))
                                        {
                                            overrideObject.Remove(rec);
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Keywords?.SetTo(overrideObject);
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
                // NPC Face Import
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.NpcFacesForceFullImport.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.HeadParts.Equals(originalObject.Record.HeadParts)
                        || !foundContext.Record.HairColor.Equals(originalObject.Record.HairColor)
                        || !foundContext.Record.HeadTexture.Equals(originalObject.Record.HeadTexture)
                        || !foundContext.Record.TextureLighting.Equals(originalObject.Record.TextureLighting)
                        || (!foundContext.Record.FaceMorph?.Equals(originalObject.Record.FaceMorph) ?? false)
                        || (!foundContext.Record.FaceParts?.Equals(originalObject.Record.FaceParts) ?? false)
                        || !foundContext.Record.TintLayers.Equals(originalObject.Record.TintLayers)
                        || !foundContext.Record.Weight.Equals(originalObject.Record.Weight))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.HeadParts.Equals(originalObject.Record.HeadParts)) Change = true;
                        if (!foundContext.Record.HairColor.Equals(originalObject.Record.HairColor)) Change = true;
                        if (!foundContext.Record.HeadTexture.Equals(originalObject.Record.HeadTexture)) Change = true;
                        if (!foundContext.Record.TextureLighting.Equals(originalObject.Record.TextureLighting)) Change = true;
                        if (!foundContext.Record.FaceMorph?.Equals(originalObject.Record.FaceMorph) ?? false) Change = true;
                        if (!foundContext.Record.FaceParts?.Equals(originalObject.Record.FaceParts) ?? false) Change = true;
                        if (!foundContext.Record.TintLayers.Equals(originalObject.Record.TintLayers)) Change = true;
                        if (!foundContext.Record.Weight.Equals(originalObject.Record.Weight)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (overrideObject.HeadParts.Count > 0) {
                                overrideObject.HeadParts.Clear();
                                overrideObject.HeadParts.AddRange(foundContext.Record.HeadParts); }
                            if (foundContext.Record.HairColor != null) overrideObject.HairColor.SetTo(foundContext.Record.HairColor);
                            if (foundContext.Record.HeadTexture != null) overrideObject.HeadTexture.SetTo(foundContext.Record.HeadTexture);
                            if (foundContext.Record.TextureLighting != null) overrideObject.TextureLighting = foundContext.Record.TextureLighting;
                            if (foundContext.Record.Weight != overrideObject.Weight) overrideObject.Weight = foundContext.Record.Weight;
                            if (foundContext.Record.FaceMorph != null) overrideObject.FaceMorph?.DeepCopyIn(foundContext.Record.FaceMorph);
                            if (foundContext.Record.FaceParts != null) overrideObject.FaceParts?.DeepCopyIn(foundContext.Record.FaceParts);
                            if (foundContext.Record.FaceParts != null) overrideObject.FaceParts?.DeepCopyIn(foundContext.Record.FaceParts);
                            if (overrideObject.TintLayers?.Count > 0 && foundContext.Record.TintLayers?.Count > 0) {
                                overrideObject.TintLayers.Clear();
                                foreach (var tint in foundContext.Record.TintLayers) overrideObject.TintLayers.Add(tint.DeepCopy()); }
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Perk Add
                //==============================================================================================================
                if (Settings.PerksAdd.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.PerksAdd.Contains(context.ModKey) && ((!context.Record.Perks?.Equals(originalObject.Record.Perks) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<PerkPlacement> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Perks != null)
                            foreach (var rec in patchRecord.Record.Perks)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Perks != null)
                            foreach (var rec in workingContext.Record.Perks)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Perks != null && context.Record.Perks.Count > 0)
                                foreach (var rec in context.Record.Perks)
                                    if (!overrideObject.Where(x => x.Equals(rec)).Any())
                                    {
                                        overrideObject.Add(rec.DeepCopy());
                                        Change = true;
                                    }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Perks?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Perk Remove
                //==============================================================================================================
                if (Settings.PerksRemove.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.PerksRemove.Contains(context.ModKey) && ((!context.Record.Perks?.Equals(originalObject.Record.Perks) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<PerkPlacement> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Perks != null)
                            foreach (var rec in patchRecord.Record.Perks)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Perks != null)
                            foreach (var rec in workingContext.Record.Perks)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Perks != null && context.Record.Perks.Count > 0)
                                if (originalObject.Record.Perks != null && originalObject.Record.Perks.Count > 0)
                                    foreach (var rec in originalObject.Record.Perks)
                                        if (!context.Record.Perks.Where(x => x.Equals(rec)).Any())
                                        {
                                            var oFoundRec = overrideObject.Where(x => x.Equals(rec));
                                            if (oFoundRec.Any())
                                            {
                                                overrideObject.Remove(oFoundRec.First());
                                                Change = true;
                                            }
                                        }
                        }

                        // If changes were made, override and write bac
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Perks?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Perk Change
                //==============================================================================================================
                if (Settings.PerksChange.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.PerksChange.Contains(context.ModKey) && ((!context.Record.Perks?.Equals(originalObject.Record.Perks) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<PerkPlacement> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Perks != null)
                            foreach (var rec in patchRecord.Record.Perks)
                                overrideObject.Add(rec.DeepCopy());
                        else if (workingContext.Record.Perks != null)
                            foreach (var rec in workingContext.Record.Perks)
                                overrideObject.Add(rec.DeepCopy());

                        // Copy Records
                        bool Change = false;
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Perks != null && context.Record.Perks.Count > 0)
                                foreach (var rec in context.Record.Perks)
                                    if (!(originalObject.Record.Perks?.Contains(rec) ?? false))
                                    {
                                        var oFoundRec = overrideObject.Where(x => x.Equals(rec));
                                        if (oFoundRec.Any() && (originalObject.Record.Perks?.Contains(oFoundRec.First()) ?? true))
                                        {
                                            overrideObject.Remove(oFoundRec.First());
                                            overrideObject.Add(rec.DeepCopy());
                                            Change = true;
                                        }
                                    }
                        }

                        // If changes were made, override and write bac
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Perks?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Race
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Race.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Race.Equals(originalObject.Record.Race))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Race.Equals(workingContext.Record.Race)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Race != null) overrideObject.Race.SetTo(foundContext.Record.Race);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Record Flags
                //==============================================================================================================
                if (Settings.RecordFlags.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.RecordFlags.Contains(context.ModKey) && (!context.Record.SkyrimMajorRecordFlags.Equals(originalObject.Record.SkyrimMajorRecordFlags)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        Npc.SkyrimMajorRecordFlag overrideObject;
                        if (state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord)) overrideObject = patchRecord.Record.SkyrimMajorRecordFlags;
                        else overrideObject = workingContext.Record.SkyrimMajorRecordFlags;

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                            foreach (var rec in Enums<Npc.SkyrimMajorRecordFlag>.Values)
                                if (context.Record.SkyrimMajorRecordFlags.HasFlag(rec) && !originalObject.Record.SkyrimMajorRecordFlags.HasFlag(rec) && !overrideObject.HasFlag(rec))
                                {
                                    overrideObject |= rec;
                                    Change = true;
                                }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                            foreach (var rec in Enums<Npc.SkyrimMajorRecordFlag>.Values)
                                if (!context.Record.SkyrimMajorRecordFlags.HasFlag(rec) && originalObject.Record.SkyrimMajorRecordFlags.HasFlag(rec) && overrideObject.HasFlag(rec))
                                {
                                    overrideObject &= ~rec;
                                    Change = true;
                                }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            foreach (var rec in Enums<Npc.SkyrimMajorRecordFlag>.Values)
                                if (overrideObject.HasFlag(rec) && !addedRecord.SkyrimMajorRecordFlags.HasFlag(rec)) addedRecord.SkyrimMajorRecordFlags |= rec;
                                else if (!overrideObject.HasFlag(rec) && addedRecord.SkyrimMajorRecordFlags.HasFlag(rec)) addedRecord.SkyrimMajorRecordFlags &= ~rec;
                        }
                    }
                }

                //==============================================================================================================
                // Voice
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Voice.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Voice.Equals(originalObject.Record.Voice))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Voice.Equals(workingContext.Record.Voice)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Voice != null) overrideObject.Voice.SetTo(foundContext.Record.Voice);
                        }
                        break;
                    }
                }

            }
        }
    }
}
