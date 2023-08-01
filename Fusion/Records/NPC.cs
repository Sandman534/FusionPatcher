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
    internal class NPCPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Actors.ACBS,Actors.AIData,Actors.AIPackages,Actors.AIPackagesForceAdd,Actors.CombatStyle" +
                ",Actors.DeathItem,Actors.Factions,Actors.Perks.Add,Actors.Perks.Change,Actors.Perks.Remove,Actors.RecordFlags,Actors.Skeleton" + 
                ",Actors.Spells,Actors.SpellsForceAdd,Actors.Stats,Actors.Voice,NPC.AIPackageOverrides,NPC.AttackRace,NPC.Class,NPC.CrimeFaction" +
                ",NPC.DefaultOutfit,NPC.Race,NpcFacesForceFullImport,Invent.Add,Invent.Remove,Invent.Change,Keywords");
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Actors.ACBS").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Actors.AIData").Contains(context.ModKey)))
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
                if (Settings.TagCount("Actors.AIPackages", out var FoundKeys) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && ((!context.Record.Packages?.Equals(originalObject.Record.Packages) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Packages NewPackages = new(patchRecord?.Record.Packages, workingContext.Record.Packages);
                        foreach (var context in foundContext)
                            NewPackages.Add(context.Record.Packages, originalObject.Record.Packages);
                        foreach (var context in foundContext.Reverse())
                            NewPackages.Remove(context.Record.Packages, originalObject.Record.Packages);
                        if (NewPackages.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Packages?.SetTo(NewPackages.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // AI Packages Force Add
                //==============================================================================================================
                if (Settings.TagCount("Actors.AIPackagesForceAdd", out var FoundKeys1) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys1.Contains(context.ModKey) && ((!context.Record.Packages?.Equals(originalObject.Record.Packages) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Packages NewPackages = new(patchRecord?.Record.Packages, workingContext.Record.Packages);
                        foreach (var context in foundContext)
                            NewPackages.Add(context.Record.Packages, originalObject.Record.Packages);
                        if (NewPackages.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Packages?.SetTo(NewPackages.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // AI Package Overrride
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NPC.AIPackageOverrides").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NPC.AttackRace").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NPC.Class").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Actors.CombatStyle").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NPC.CrimeFaction").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Actors.DeathItem").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NPC.DefaultOutfit").Contains(context.ModKey)))
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
                if (Settings.TagCount("Actors.Factions", out var FoundKeys2) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys2.Contains(context.ModKey) && ((!context.Record.Factions?.Equals(originalObject.Record.Factions) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Factions NewFaction = new(patchRecord?.Record.Factions, workingContext.Record.Factions);
                        foreach (var context in foundContext)
                            NewFaction.Add(context.Record.Factions);
                        foreach (var context in foundContext.Reverse())
                            NewFaction.Remove(context.Record.Factions, originalObject.Record.Factions);    
                        if (NewFaction.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Factions?.SetTo(NewFaction.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Add
                //==============================================================================================================
                if (Settings.TagCount("Invent.Add", out var FoundKeys3) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys3.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Containers NewContainer = new(patchRecord?.Record.Items, workingContext.Record.Items);
                        foreach (var context in foundContext)
                            NewContainer.Add(context.Record.Items);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Remove
                //==============================================================================================================
                if (Settings.TagCount("Invent.Remove", out var FoundKeys4) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys4.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Containers NewContainer = new(patchRecord?.Record.Items, workingContext.Record.Items);
                        foreach (var context in foundContext.Reverse())
                            NewContainer.Remove(context.Record.Items, originalObject.Record.Items);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Invent Change
                //==============================================================================================================
                if (Settings.TagCount("Invent.Change", out var FoundKeys5) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys5.Contains(context.ModKey) && ((!context.Record.Items?.Equals(originalObject.Record.Items) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Containers NewContainer = new(patchRecord?.Record.Items, workingContext.Record.Items);
                        foreach (var context in foundContext.Reverse())
                            NewContainer.Change(context.Record.Items, originalObject.Record.Items);
                        if (NewContainer.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Items?.SetTo(NewContainer.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.TagCount("Keywords", out var FoundKeys6) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys6.Contains(context.ModKey) && ((!context.Record.Keywords?.Equals(originalObject.Record.Keywords) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Names").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NpcFacesForceFullImport").Contains(context.ModKey)))
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
                if (Settings.TagCount("Perks.Add", out var FoundKeys7) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys7.Contains(context.ModKey) && ((!context.Record.Perks?.Equals(originalObject.Record.Perks) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Perks NewPerks = new(patchRecord?.Record.Perks, workingContext.Record.Perks);
                        foreach (var context in foundContext)
                            NewPerks.Add(context.Record.Perks);
                        if (NewPerks.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Perks?.SetTo(NewPerks.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Perk Remove
                //==============================================================================================================
                if (Settings.TagCount("Perks.Remove", out var FoundKeys8) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys8.Contains(context.ModKey) && ((!context.Record.Perks?.Equals(originalObject.Record.Perks) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Perks NewPerks = new(patchRecord?.Record.Perks, workingContext.Record.Perks);
                        foreach (var context in foundContext)
                            NewPerks.Remove(context.Record.Perks, originalObject.Record.Perks);
                        if (NewPerks.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Perks?.SetTo(NewPerks.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Perk Change
                //==============================================================================================================
                if (Settings.TagCount("Perks.Change", out var FoundKeys9) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys9.Contains(context.ModKey) && ((!context.Record.Perks?.Equals(originalObject.Record.Perks) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<INpc, INpcGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Perks NewPerks = new(patchRecord?.Record.Perks, workingContext.Record.Perks);
                        foreach (var context in foundContext)
                            NewPerks.Change(context.Record.Perks, originalObject.Record.Perks);
                        if (NewPerks.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Perks?.SetTo(NewPerks.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Race
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("NPC.Race").Contains(context.ModKey)))
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
                if (Settings.TagCount("Actors.RecordFlags", out var FoundKeys10) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys10.Contains(context.ModKey) && (!context.Record.SkyrimMajorRecordFlags.Equals(originalObject.Record.SkyrimMajorRecordFlags)));
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Actors.Voice").Contains(context.ModKey)))
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
