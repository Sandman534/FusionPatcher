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
    internal class NPC_
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            Console.WriteLine("Processing NPC");
            HashSet<ModKey> workingModList = Settings.GetModList("Actors.ACBS,Actors.AIData,Actors.AIPackages,Actors.AIPackagesForceAdd,Actors.CombatStyle" +
                ",Actors.DeathItem,Actors.Factions,Actors.Perks.Add,Actors.Perks.Change,Actors.Perks.Remove,Actors.RecordFlags,Actors.Skeleton" + 
                ",Actors.Spells,Actors.SpellsForceAdd,Actors.Stats,Actors.Voice,NPC.AIPackageOverrides,NPC.AttackRace,NPC.Class,NPC.CrimeFaction" +
                ",NPC.DefaultOutfit,NPC.Race,NpcFacesForceFullImport,Invent.Add,Invent.Remove,Invent.Change,Keywords");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(workingContext.Record.FormKey).Last();
                MappedTags mapped = new MappedTags();
                Keywords NewKeywords = new(workingContext.Record.Keywords);
                Packages NewPackages = new(workingContext.Record.Packages);
                Factions NewFaction = new(workingContext.Record.Factions);
                Containers NewContainer = new(workingContext.Record.Items);
                Perks NewPerks = new(workingContext.Record.Perks);
                Flags<Npc.SkyrimMajorRecordFlag> NewFlags = new(workingContext.Record.SkyrimMajorRecordFlags);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // ACBS
                    //==============================================================================================================
                    if (mapped.NotMapped("Actors.ACBS") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Configuration,originalObject.Record.Configuration))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Configuration,workingContext.Record.Configuration)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Configuration,originalObject.Record.Configuration))
                                        overrideObject.Configuration.DeepCopyIn(foundContext.Record.Configuration);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // AI Data
                    //==============================================================================================================
                    if (mapped.NotMapped("Actors.AIData") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.AIData,originalObject.Record.AIData))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.AIData,workingContext.Record.AIData)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.AIData,originalObject.Record.AIData))
                                        overrideObject.AIData.DeepCopyIn(foundContext.Record.AIData);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // AI Package Overrride
                    //==============================================================================================================
                    if (mapped.NotMapped("NPC.AIPackageOverrides") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.SpectatorOverridePackageList,originalObject.Record.SpectatorOverridePackageList)
                            || Compare.NotEqual(foundContext.Record.ObserveDeadBodyOverridePackageList,originalObject.Record.ObserveDeadBodyOverridePackageList)
                            || Compare.NotEqual(foundContext.Record.GuardWarnOverridePackageList,originalObject.Record.GuardWarnOverridePackageList)
                            || Compare.NotEqual(foundContext.Record.CombatOverridePackageList,originalObject.Record.CombatOverridePackageList))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.SpectatorOverridePackageList,workingContext.Record.SpectatorOverridePackageList)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.ObserveDeadBodyOverridePackageList,workingContext.Record.ObserveDeadBodyOverridePackageList)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.GuardWarnOverridePackageList,workingContext.Record.GuardWarnOverridePackageList)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.CombatOverridePackageList,workingContext.Record.CombatOverridePackageList)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.SpectatorOverridePackageList,originalObject.Record.SpectatorOverridePackageList))
                                        overrideObject.SpectatorOverridePackageList.SetTo(foundContext.Record.SpectatorOverridePackageList);
                                    if (Compare.NotEqual(foundContext.Record.ObserveDeadBodyOverridePackageList,originalObject.Record.ObserveDeadBodyOverridePackageList))
                                        overrideObject.ObserveDeadBodyOverridePackageList.SetTo(foundContext.Record.ObserveDeadBodyOverridePackageList);
                                    if (Compare.NotEqual(foundContext.Record.GuardWarnOverridePackageList,originalObject.Record.GuardWarnOverridePackageList))
                                        overrideObject.GuardWarnOverridePackageList.SetTo(foundContext.Record.GuardWarnOverridePackageList);
                                    if (Compare.NotEqual(foundContext.Record.CombatOverridePackageList,originalObject.Record.CombatOverridePackageList))
                                        overrideObject.CombatOverridePackageList.SetTo(foundContext.Record.CombatOverridePackageList);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Attack Race
                    //==============================================================================================================
                    if (mapped.NotMapped("NPC.AttackRace") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.AttackRace,originalObject.Record.AttackRace))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.AttackRace,workingContext.Record.AttackRace)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.AttackRace,originalObject.Record.AttackRace))
                                        overrideObject.AttackRace.SetTo(foundContext.Record.AttackRace);
                                }
                                mapped[3]= true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Class
                    //==============================================================================================================
                    if (mapped.NotMapped("NPC.Class") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Class,originalObject.Record.Class))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Class,workingContext.Record.Class)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Class,originalObject.Record.Class))
                                        overrideObject.Class.SetTo(foundContext.Record.Class);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Combat Style
                    //==============================================================================================================
                    if (mapped.NotMapped("Actors.CombatStyle") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.CombatStyle,originalObject.Record.CombatStyle))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) 
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.CombatStyle,workingContext.Record.CombatStyle)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.CombatStyle,originalObject.Record.CombatStyle))
                                        overrideObject.CombatStyle.SetTo(foundContext.Record.CombatStyle);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Crime Faction
                    //==============================================================================================================
                    if (mapped.NotMapped("NPC.CrimeFaction") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.CrimeFaction,originalObject.Record.CrimeFaction))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.CrimeFaction,workingContext.Record.CrimeFaction)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.CrimeFaction,originalObject.Record.CrimeFaction))
                                        overrideObject.CrimeFaction.SetTo(foundContext.Record.CrimeFaction);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Death Items
                    //==============================================================================================================
                    if (mapped.NotMapped("Actors.DeathItem") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.DeathItem,originalObject.Record.DeathItem))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.DeathItem,workingContext.Record.DeathItem)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.DeathItem,originalObject.Record.DeathItem))
                                        overrideObject.DeathItem.SetTo(foundContext.Record.DeathItem);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Default Outfit
                    //==============================================================================================================
                    if (mapped.NotMapped("NPC.DefaultOutfit") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.DefaultOutfit,originalObject.Record.DefaultOutfit)
                            || Compare.NotEqual(foundContext.Record.Template,originalObject.Record.Template))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[8] =  true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.DefaultOutfit,workingContext.Record.DefaultOutfit)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Template,workingContext.Record.Template)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.DefaultOutfit,originalObject.Record.DefaultOutfit))
                                        overrideObject.DefaultOutfit.SetTo(foundContext.Record.DefaultOutfit);
                                    if (Compare.NotEqual(foundContext.Record.Template,originalObject.Record.Template))
                                        overrideObject.Template.SetTo(foundContext.Record.Template);
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
                    // NPC Face Import
                    //==============================================================================================================
                    if (mapped.NotMapped("NpcFacesForceFullImport") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
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
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
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
                                    if (Compare.NotEqual(foundContext.Record.HeadParts,originalObject.Record.HeadParts))
                                    {
                                        overrideObject.HeadParts.Clear();
                                        overrideObject.HeadParts.AddRange(foundContext.Record.HeadParts); 
                                    }
                                    if (Compare.NotEqual(foundContext.Record.HairColor,originalObject.Record.HairColor))
                                        overrideObject.HairColor.SetTo(foundContext.Record.HairColor);
                                    if (Compare.NotEqual(foundContext.Record.HeadTexture,originalObject.Record.HeadTexture))
                                        overrideObject.HeadTexture.SetTo(foundContext.Record.HeadTexture);
                                    if (Compare.NotEqual(foundContext.Record.TextureLighting,originalObject.Record.TextureLighting))
                                        overrideObject.TextureLighting = foundContext.Record.TextureLighting;
                                    if (Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                                        overrideObject.Weight = foundContext.Record.Weight;
                                    if (Compare.NotEqual(foundContext.Record.FaceMorph,originalObject.Record.FaceMorph))
                                        overrideObject.FaceMorph = foundContext.Record.FaceMorph?.DeepCopy();
                                    if (Compare.NotEqual(foundContext.Record.FaceParts,originalObject.Record.FaceParts))
                                        overrideObject.FaceParts = foundContext.Record.FaceParts?.DeepCopy();
                                    if(Compare.NotEqual(foundContext.Record.TintLayers,originalObject.Record.TintLayers))
                                    {
                                        overrideObject.TintLayers.Clear();
                                        foreach (var tint in foundContext.Record.TintLayers)
                                            overrideObject.TintLayers.Add(tint.DeepCopy()); 
                                    }
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Race
                    //==============================================================================================================
                    if (mapped.NotMapped("NPC.Race") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Race,originalObject.Record.Race))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Race,workingContext.Record.Race)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Race,originalObject.Record.Race))
                                        overrideObject.Race.SetTo(foundContext.Record.Race);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Voice
                    //==============================================================================================================
                    if (mapped.NotMapped("Actors.Voice") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Voice,originalObject.Record.Voice))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Voice,workingContext.Record.Voice)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Voice,originalObject.Record.Voice))
                                        overrideObject.Voice.SetTo(foundContext.Record.Voice);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }
                    
                    //==============================================================================================================
                    // AI Packages Adds
                    //==============================================================================================================
                    if (Settings.TagList("Actors.AIPackages").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Packages,originalObject.Record.Packages))
                            NewPackages.Add(foundContext.Record.Packages, originalObject.Record.Packages);

                    //==============================================================================================================
                    // Faction Adds
                    //==============================================================================================================
                    if (Settings.TagList("Actors.Factions").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Factions,originalObject.Record.Factions))
                            NewFaction.Add(foundContext.Record.Factions);

                    //==============================================================================================================
                    // Inventory Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList("Invent.Add").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Items,originalObject.Record.Items))
                            NewContainer.Add(foundContext.Record.Items);

                    if (Settings.TagList("Invent.Change").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Items,originalObject.Record.Items))
                            NewContainer.Change(foundContext.Record.Items, originalObject.Record.Items);

                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                            NewKeywords.Add(foundContext.Record.Keywords, originalObject.Record.Keywords);

                    //==============================================================================================================
                    // Record Flag Adds
                    //==============================================================================================================
                    if (Settings.TagList("C.RecordFlags").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.SkyrimMajorRecordFlags,originalObject.Record.SkyrimMajorRecordFlags))
                            NewFlags.Add(foundContext.Record.SkyrimMajorRecordFlags, originalObject.Record.SkyrimMajorRecordFlags);

                    //==============================================================================================================
                    // Perk Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList("Perks.Add").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Perks,originalObject.Record.Perks))
                            NewPerks.Add(foundContext.Record.Perks);

                    if (Settings.TagList("Perks.Change").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Perks,originalObject.Record.Perks))
                            NewPerks.Change(foundContext.Record.Perks, originalObject.Record.Perks); 

                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // AI Packages Removes
                    //==============================================================================================================
                    if (Settings.TagList("Actors.AIPackages").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Packages,originalObject.Record.Packages))
                            NewPackages.Remove(foundContext.Record.Packages, originalObject.Record.Packages);

                    //==============================================================================================================
                    // Faction Removes
                    //==============================================================================================================
                    if (Settings.TagList("Actors.Factions").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Factions,originalObject.Record.Factions))
                            NewFaction.Remove(foundContext.Record.Factions, originalObject.Record.Factions);

                    //==============================================================================================================
                    // Inventory Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList("Invent.Remove").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Items,originalObject.Record.Items))
                            NewContainer.Remove(foundContext.Record.Items, originalObject.Record.Items);

                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                            NewKeywords.Remove(foundContext.Record.Keywords, originalObject.Record.Keywords);

                    //==============================================================================================================
                    // Record Flag Removes
                    //==============================================================================================================
                    if (Settings.TagList("C.MiscFlags").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.SkyrimMajorRecordFlags,originalObject.Record.SkyrimMajorRecordFlags))
                            NewFlags.Remove(foundContext.Record.SkyrimMajorRecordFlags, originalObject.Record.SkyrimMajorRecordFlags);

                    //==============================================================================================================
                    // Perk Removes
                    //==============================================================================================================
                    if (Settings.TagList("Perks.Remove").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Perks,originalObject.Record.Perks))
                            NewPerks.Remove(foundContext.Record.Perks, originalObject.Record.Perks);
                }

                //==============================================================================================================
                // AI Package Force Adds
                //==============================================================================================================
                foreach(var foundContext in modContext)
                    if (Settings.TagList("Actors.AIPackagesForceAdd").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Packages,originalObject.Record.Packages))
                            NewPackages.Add(foundContext.Record.Packages, originalObject.Record.Packages);

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewPackages.Modified)
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Packages.SetTo(NewPackages.OverrideObject);
                }
                if (NewFaction.Modified)
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Factions.SetTo(NewFaction.OverrideObject);
                }
                if (NewContainer.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Items = NewContainer.OverrideObject;
                }
                if (NewKeywords.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                if (NewFlags.Modified)
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.SkyrimMajorRecordFlags = NewFlags.OverrideObject;
                }
                if (NewPerks.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Perks = NewPerks.OverrideObject;
                }
                
            }
        }
    }
}
