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
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Actors.ACBS").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Configuration,originalObject.Record.Configuration))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Configuration,workingContext.Record.Configuration)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Configuration != null && Compare.NotEqual(foundContext.Record.Configuration,originalObject.Record.Configuration))
                                overrideObject.Configuration.DeepCopyIn(foundContext.Record.Configuration);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // AI Data
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Actors.AIData").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.AIData,originalObject.Record.AIData))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.AIData,workingContext.Record.AIData)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AIData != null && Compare.NotEqual(foundContext.Record.AIData,originalObject.Record.AIData))
                                overrideObject.AIData.DeepCopyIn(foundContext.Record.AIData);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // AI Packages
                //==============================================================================================================
                if (Settings.HasTags("Actors.AIPackages,Actors.AIPackagesForceAdd"))
                {
                    Packages NewPackages = new(workingContext.Record.Packages);
                    if (Settings.HasTags("Actors.AIPackages", out var AIPackages))
                    {
                        // Get the last overriding context of our element
                        var foundContext = modContext.Where(context => AIPackages.Contains(context.ModKey) && Compare.NotEqual(context.Record.Packages,originalObject.Record.Packages));
                        if (foundContext.Any())
                        {
                            foreach (var context in foundContext)
                                NewPackages.Add(context.Record.Packages, originalObject.Record.Packages);
                            foreach (var context in foundContext.Reverse())
                                NewPackages.Remove(context.Record.Packages, originalObject.Record.Packages);
                        }
                    }

                    if (Settings.HasTags("Actors.AIPackagesForceAdd", out var AIPackagesAdd))
                    {
                        // Get the last overriding context of our element
                        var foundContext = modContext.Where(context => AIPackagesAdd.Contains(context.ModKey) && Compare.NotEqual(context.Record.Packages,originalObject.Record.Packages));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewPackages.Add(context.Record.Packages, originalObject.Record.Packages);
                    }

                    if (NewPackages.Modified) {
                        var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                        addedRecord.Packages?.SetTo(NewPackages.OverrideObject);
                    }
                }

                //==============================================================================================================
                // AI Package Overrride
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NPC.AIPackageOverrides").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.SpectatorOverridePackageList,originalObject.Record.SpectatorOverridePackageList)
                        || Compare.NotEqual(foundContext.Record.ObserveDeadBodyOverridePackageList,originalObject.Record.ObserveDeadBodyOverridePackageList)
                        || Compare.NotEqual(foundContext.Record.GuardWarnOverridePackageList,originalObject.Record.GuardWarnOverridePackageList)
                        || Compare.NotEqual(foundContext.Record.CombatOverridePackageList,originalObject.Record.CombatOverridePackageList))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.SpectatorOverridePackageList,workingContext.Record.SpectatorOverridePackageList)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.ObserveDeadBodyOverridePackageList,workingContext.Record.ObserveDeadBodyOverridePackageList)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.GuardWarnOverridePackageList,workingContext.Record.GuardWarnOverridePackageList)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.CombatOverridePackageList,workingContext.Record.CombatOverridePackageList)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.SpectatorOverridePackageList != null && Compare.NotEqual(foundContext.Record.SpectatorOverridePackageList,originalObject.Record.SpectatorOverridePackageList))
                                overrideObject.SpectatorOverridePackageList.SetTo(foundContext.Record.SpectatorOverridePackageList);
                            if (foundContext.Record.ObserveDeadBodyOverridePackageList != null && Compare.NotEqual(foundContext.Record.ObserveDeadBodyOverridePackageList,originalObject.Record.ObserveDeadBodyOverridePackageList))
                                overrideObject.ObserveDeadBodyOverridePackageList.SetTo(foundContext.Record.ObserveDeadBodyOverridePackageList);
                            if (foundContext.Record.GuardWarnOverridePackageList != null && Compare.NotEqual(foundContext.Record.GuardWarnOverridePackageList,originalObject.Record.GuardWarnOverridePackageList))
                                overrideObject.GuardWarnOverridePackageList.SetTo(foundContext.Record.GuardWarnOverridePackageList);
                            if (foundContext.Record.CombatOverridePackageList != null && Compare.NotEqual(foundContext.Record.CombatOverridePackageList,originalObject.Record.CombatOverridePackageList))
                                overrideObject.CombatOverridePackageList.SetTo(foundContext.Record.CombatOverridePackageList);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Attack Race
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NPC.AttackRace").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.AttackRace,originalObject.Record.AttackRace))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.AttackRace,workingContext.Record.AttackRace)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AttackRace != null && Compare.NotEqual(foundContext.Record.AttackRace,originalObject.Record.AttackRace))
                                overrideObject.AttackRace?.SetTo(foundContext.Record.AttackRace);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Class
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NPC.Class").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Class,originalObject.Record.Class))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Class,workingContext.Record.Class)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Class != null && Compare.NotEqual(foundContext.Record.Class,originalObject.Record.Class))
                                overrideObject.Class?.SetTo(foundContext.Record.Class);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Combat Style
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Actors.CombatStyle").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.CombatStyle,originalObject.Record.CombatStyle))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.CombatStyle,workingContext.Record.CombatStyle)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.CombatStyle != null && Compare.NotEqual(foundContext.Record.CombatStyle,originalObject.Record.CombatStyle))
                                overrideObject.CombatStyle?.SetTo(foundContext.Record.CombatStyle);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Crime Faction
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NPC.CrimeFaction").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.CrimeFaction,originalObject.Record.CrimeFaction))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.CrimeFaction,workingContext.Record.CrimeFaction)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.CrimeFaction != null && Compare.NotEqual(foundContext.Record.CrimeFaction,originalObject.Record.CrimeFaction))
                                overrideObject.CrimeFaction?.SetTo(foundContext.Record.CrimeFaction);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Death Items
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Actors.DeathItem").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.DeathItem,originalObject.Record.DeathItem))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.DeathItem,workingContext.Record.DeathItem)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.DeathItem != null && Compare.NotEqual(foundContext.Record.DeathItem,originalObject.Record.DeathItem))
                                overrideObject.DeathItem?.SetTo(foundContext.Record.DeathItem);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Default Outfit
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NPC.DefaultOutfit").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.DefaultOutfit,originalObject.Record.DefaultOutfit)
                        || Compare.NotEqual(foundContext.Record.Template,originalObject.Record.Template))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.DefaultOutfit,workingContext.Record.DefaultOutfit)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Template,workingContext.Record.Template)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.DefaultOutfit != null && Compare.NotEqual(foundContext.Record.DefaultOutfit,originalObject.Record.DefaultOutfit))
                                overrideObject.DefaultOutfit?.SetTo(foundContext.Record.DefaultOutfit);
                            if (foundContext.Record.Template != null && Compare.NotEqual(foundContext.Record.Template,originalObject.Record.Template))
                                overrideObject.Template?.SetTo(foundContext.Record.Template);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Factions
                //==============================================================================================================
                if (Settings.HasTags("Actors.Factions", out var Factions))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Factions.Contains(context.ModKey) && Compare.NotEqual(context.Record.Factions,originalObject.Record.Factions));
                    if (foundContext.Any())
                    {
                        Factions NewFaction = new(workingContext.Record.Factions);
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
                // Inventory
                //==============================================================================================================
                if (Settings.HasTags("Invent.Add,Invent.Change,Invent.Remove"))
                {
                    Containers NewContainer = new(workingContext.Record.Items);
                    if (Settings.HasTags("Invent.Add", out var InventAdd))
                    {
                        var InventAddContext = modContext.Where(context => InventAdd.Contains(context.ModKey) && Compare.NotEqual(context.Record.Items,originalObject.Record.Items));
                        if (InventAddContext.Any())
                            foreach (var context in InventAddContext)
                                NewContainer.Add(context.Record.Items);
                    }

                    if (Settings.HasTags("Invent.Change", out var InventChange))
                    {
                        // Get the last overriding context of our element
                        var foundContext = modContext.Where(context => InventChange.Contains(context.ModKey) && Compare.NotEqual(context.Record.Items,originalObject.Record.Items));
                        if (foundContext.Any())
                            foreach (var context in foundContext.Reverse())
                                NewContainer.Change(context.Record.Items, originalObject.Record.Items);
                    }

                    if (Settings.HasTags("Invent.Remove", out var InventRemove))
                    {
                        var foundContext = modContext.Where(context => InventRemove.Contains(context.ModKey) && Compare.NotEqual(context.Record.Items,originalObject.Record.Items));
                        if (foundContext.Any())
                            foreach (var context in foundContext.Reverse())
                                NewContainer.Remove(context.Record.Items, originalObject.Record.Items);
                    }

                    if (NewContainer.Modified) {
                        var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                        addedRecord.Items?.SetTo(NewContainer.OverrideObject);
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
                // NPC Face Import
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NpcFacesForceFullImport").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.HeadParts,originalObject.Record.HeadParts)
                        || Compare.NotEqual(foundContext.Record.HairColor,originalObject.Record.HairColor)
                        || Compare.NotEqual(foundContext.Record.HeadTexture,originalObject.Record.HeadTexture)
                        || Compare.NotEqual(foundContext.Record.TextureLighting,originalObject.Record.TextureLighting)
                        || Compare.NotEqual(foundContext.Record.FaceMorph,originalObject.Record.FaceMorph)
                        || Compare.NotEqual(foundContext.Record.FaceParts,originalObject.Record.FaceParts)
                        || Compare.NotEqual(foundContext.Record.TintLayers,originalObject.Record.TintLayers)
                        || Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.HeadParts,workingContext.Record.HeadParts)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.HairColor,workingContext.Record.HairColor)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.HeadTexture,workingContext.Record.HeadTexture)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.TextureLighting,workingContext.Record.TextureLighting)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.FaceMorph,workingContext.Record.FaceMorph)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.FaceParts,workingContext.Record.FaceParts)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.TintLayers,workingContext.Record.TintLayers)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Weight,workingContext.Record.Weight)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (overrideObject.HeadParts.Count > 0 && Compare.NotEqual(foundContext.Record.HeadParts,originalObject.Record.HeadParts))
                            {
                                overrideObject.HeadParts.Clear();
                                overrideObject.HeadParts.AddRange(foundContext.Record.HeadParts); 
                            }
                            if (foundContext.Record.HairColor != null && Compare.NotEqual(foundContext.Record.HairColor,originalObject.Record.HairColor))
                                overrideObject.HairColor.SetTo(foundContext.Record.HairColor);
                            if (foundContext.Record.HeadTexture != null && Compare.NotEqual(foundContext.Record.HeadTexture,originalObject.Record.HeadTexture))
                                overrideObject.HeadTexture.SetTo(foundContext.Record.HeadTexture);
                            if (Compare.NotEqual(foundContext.Record.TextureLighting,originalObject.Record.TextureLighting))
                                overrideObject.TextureLighting = foundContext.Record.TextureLighting;
                            if (Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                                overrideObject.Weight = foundContext.Record.Weight;
                            if (foundContext.Record.FaceMorph != null && Compare.NotEqual(foundContext.Record.FaceMorph,originalObject.Record.FaceMorph))
                                overrideObject.FaceMorph?.DeepCopyIn(foundContext.Record.FaceMorph);
                            if (foundContext.Record.FaceParts != null && Compare.NotEqual(foundContext.Record.FaceParts,originalObject.Record.FaceParts))
                                overrideObject.FaceParts?.DeepCopyIn(foundContext.Record.FaceParts);
                            if (overrideObject.TintLayers?.Count > 0 && Compare.NotEqual(foundContext.Record.TintLayers,originalObject.Record.TintLayers))
                            {
                                overrideObject.TintLayers.Clear();
                                foreach (var tint in foundContext.Record.TintLayers)
                                    overrideObject.TintLayers.Add(tint.DeepCopy()); 
                            }
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Perks
                //==============================================================================================================
                if (Settings.HasTags("Perks.Add,Perks.Remove,Perks.Change"))
                {
                    Perks NewPerks = new(workingContext.Record.Perks);
                    if (Settings.HasTags("Perks.Add", out var PerksAdd))
                    {
                        var foundContext = modContext.Where(context => PerksAdd.Contains(context.ModKey) && Compare.NotEqual(context.Record.Perks,originalObject.Record.Perks));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewPerks.Add(context.Record.Perks);
                    }

                    if (Settings.HasTags("Perks.Change", out var PerksChange))
                    {
                        var foundContext = modContext.Where(context => PerksChange.Contains(context.ModKey) && Compare.NotEqual(context.Record.Perks,originalObject.Record.Perks));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewPerks.Change(context.Record.Perks, originalObject.Record.Perks);
                    }

                    if (Settings.HasTags("Perks.Remove", out var PerksRemove))
                    {
                        var foundContext = modContext.Where(context => PerksRemove.Contains(context.ModKey) && Compare.NotEqual(context.Record.Perks,originalObject.Record.Perks));
                        if (foundContext.Any())
                            foreach (var context in foundContext)
                                NewPerks.Remove(context.Record.Perks, originalObject.Record.Perks);
                    }

                    if (NewPerks.Modified) {
                        var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                        addedRecord.Perks?.SetTo(NewPerks.OverrideObject);
                    }
                }

                //==============================================================================================================
                // Race
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("NPC.Race").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Race,originalObject.Record.Race))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Race,workingContext.Record.Race)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Race != null && Compare.NotEqual(foundContext.Record.Race,originalObject.Record.Race))
                                overrideObject.Race?.SetTo(foundContext.Record.Race);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Record Flags
                //==============================================================================================================
                if (Settings.HasTags("Actors.RecordFlags", out var RecordFlags))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => RecordFlags.Contains(context.ModKey) && (!context.Record.SkyrimMajorRecordFlags.Equals(originalObject.Record.SkyrimMajorRecordFlags)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        Npc.SkyrimMajorRecordFlag overrideObject = workingContext.Record.SkyrimMajorRecordFlags;

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
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Actors.Voice").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Voice,originalObject.Record.Voice))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Voice,workingContext.Record.Voice)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Voice != null && Compare.NotEqual(foundContext.Record.Voice,originalObject.Record.Voice))
                                overrideObject.Voice?.SetTo(foundContext.Record.Voice);
                        }
                        break;
                    }
                }

            }
        }
    }
}
