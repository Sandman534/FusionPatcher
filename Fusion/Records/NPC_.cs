using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class NPC_
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Actors_ACBS, Tags.Actors_AIData, Tags.Actors_AIPackages, Tags.Actors_AIPackagesForceAdd,
                Tags.Actors_CombatStyle, Tags.Actors_DeathItem, Tags.Actors_Factions, Tags.Actors_Perks_Add, Tags.Actors_Perks_Change, Tags.Actors_Perks_Remove,
                Tags.Actors_RecordFlags, Tags.Actors_Skeleton, Tags.Actors_Spells, Tags.Actors_SpellsForceAdd, Tags.Actors_Stats, Tags.Actors_Voice,
                Tags.NPC_AIPackageOverrides, Tags.NPC_AttackRace, Tags.NPC_CrimeFaction, Tags.NPC_DefaultOutfit, Tags.NPC_Race, Tags.NpcFacesForceFullImport,
                Tags.Invent_Add, Tags.Invent_Change, Tags.Invent_Remove, Tags.Keywords, Tags.Names);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<INpcGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "NPC");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                INpc? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);
                Packages NewPackages = new(wContext.Record.Packages);
                Factions NewFaction = new(wContext.Record.Factions);
                Containers NewContainer = new(wContext.Record.Items);
                Perks NewPerks = new(wContext.Record.Perks);
                Flags<Npc.SkyrimMajorRecordFlag> NewFlags = new(wContext.Record.SkyrimMajorRecordFlags);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // ACBS
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Actors_ACBS, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Configuration,oContext.Record.Configuration)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Configuration,wContext.Record.Configuration,oContext.Record.Configuration)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Configuration.DeepCopyIn(fContext.Record.Configuration);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // AI Data
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Actors_AIData, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.AIData,oContext.Record.AIData)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.AIData,wContext.Record.AIData,oContext.Record.AIData)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AIData.DeepCopyIn(fContext.Record.AIData);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // AI Package Overrride
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NPC_AIPackageOverrides, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.SpectatorOverridePackageList,oContext.Record.SpectatorOverridePackageList)
                            || Compare.NotEqual(fContext.Record.ObserveDeadBodyOverridePackageList,oContext.Record.ObserveDeadBodyOverridePackageList)
                            || Compare.NotEqual(fContext.Record.GuardWarnOverridePackageList,oContext.Record.GuardWarnOverridePackageList)
                            || Compare.NotEqual(fContext.Record.CombatOverridePackageList,oContext.Record.CombatOverridePackageList)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.SpectatorOverridePackageList,wContext.Record.SpectatorOverridePackageList,oContext.Record.SpectatorOverridePackageList)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SpectatorOverridePackageList.SetTo(fContext.Record.SpectatorOverridePackageList);
                                }

                                if (Utility.ShouldChange(fContext.Record.ObserveDeadBodyOverridePackageList,wContext.Record.ObserveDeadBodyOverridePackageList,oContext.Record.ObserveDeadBodyOverridePackageList)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ObserveDeadBodyOverridePackageList.SetTo(fContext.Record.ObserveDeadBodyOverridePackageList);
                                }

                                if (Utility.ShouldChange(fContext.Record.GuardWarnOverridePackageList,wContext.Record.GuardWarnOverridePackageList,oContext.Record.GuardWarnOverridePackageList)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.GuardWarnOverridePackageList.SetTo(fContext.Record.GuardWarnOverridePackageList);
                                }

                                if (Utility.ShouldChange(fContext.Record.CombatOverridePackageList,wContext.Record.CombatOverridePackageList,oContext.Record.CombatOverridePackageList)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CombatOverridePackageList.SetTo(fContext.Record.CombatOverridePackageList);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Attack Race
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NPC_AttackRace, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.AttackRace,oContext.Record.AttackRace)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.AttackRace,wContext.Record.AttackRace,oContext.Record.AttackRace)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AttackRace.SetTo(fContext.Record.AttackRace);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Class
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NPC_Class, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Class,oContext.Record.Class)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Class,wContext.Record.Class,oContext.Record.Class)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Class.SetTo(fContext.Record.Class);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Combat Style
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Actors_CombatStyle, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.CombatStyle,oContext.Record.CombatStyle)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.CombatStyle,wContext.Record.CombatStyle,oContext.Record.CombatStyle)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CombatStyle.SetTo(fContext.Record.CombatStyle);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Crime Faction
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NPC_CrimeFaction, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.CrimeFaction,oContext.Record.CrimeFaction)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.CrimeFaction,wContext.Record.CrimeFaction,oContext.Record.CrimeFaction)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CrimeFaction.SetTo(fContext.Record.CrimeFaction);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Death Items
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Actors_DeathItem, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.DeathItem,oContext.Record.DeathItem)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.DeathItem,wContext.Record.DeathItem,oContext.Record.DeathItem)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.DeathItem.SetTo(fContext.Record.DeathItem);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Default Outfit
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NPC_DefaultOutfit, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.DefaultOutfit,oContext.Record.DefaultOutfit)
                            || Compare.NotEqual(fContext.Record.Template,oContext.Record.Template)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.DefaultOutfit,wContext.Record.DefaultOutfit,oContext.Record.DefaultOutfit)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.DefaultOutfit.SetTo(fContext.Record.DefaultOutfit);
                                }

                                if (Utility.ShouldChange(fContext.Record.Template,wContext.Record.Template,oContext.Record.Template)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Template.SetTo(fContext.Record.Template);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Names, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Name,oContext.Record.Name)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Name,wContext.Record.Name,oContext.Record.Name)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Name = Utility.NewString(fContext.Record.Name);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // NPC Face Import
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NpcFacesForceFullImport, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.HeadParts,oContext.Record.HeadParts)
                            || Compare.NotEqual(fContext.Record.HairColor,oContext.Record.HairColor)
                            || Compare.NotEqual(fContext.Record.HeadTexture,oContext.Record.HeadTexture)
                            || Compare.NotEqual(fContext.Record.TextureLighting,oContext.Record.TextureLighting)
                            || Compare.NotEqual(fContext.Record.Weight,oContext.Record.Weight)
                            || Compare.NotEqual(fContext.Record.FaceMorph,oContext.Record.FaceMorph)
                            || Compare.NotEqual(fContext.Record.FaceParts,oContext.Record.FaceParts)
                            || Compare.NotEqual(fContext.Record.TintLayers,oContext.Record.TintLayers)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.HeadParts,wContext.Record.HeadParts,oContext.Record.HeadParts)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HeadParts.Clear();
                                    overrideObject.HeadParts.AddRange(fContext.Record.HeadParts); 
                                }

                                if (Utility.ShouldChange(fContext.Record.HairColor,wContext.Record.HairColor,oContext.Record.HairColor)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HairColor.SetTo(fContext.Record.HairColor);
                                }

                                if (Utility.ShouldChange(fContext.Record.HeadTexture,wContext.Record.HeadTexture,oContext.Record.HeadTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HeadTexture.SetTo(fContext.Record.HeadTexture);
                                }

                                if (Utility.ShouldChange(fContext.Record.TextureLighting,wContext.Record.TextureLighting,oContext.Record.TextureLighting)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.TextureLighting = fContext.Record.TextureLighting;
                                }

                                if (Utility.ShouldChange(fContext.Record.Weight,wContext.Record.Weight,oContext.Record.Weight)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Weight = fContext.Record.Weight;
                                }

                                if (Utility.ShouldChange(fContext.Record.FaceMorph,wContext.Record.FaceMorph,oContext.Record.FaceMorph)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FaceMorph = fContext.Record.FaceMorph?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.FaceParts,wContext.Record.FaceParts,oContext.Record.FaceParts)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FaceParts = fContext.Record.FaceParts?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.TintLayers,wContext.Record.TintLayers,oContext.Record.TintLayers)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                        overrideObject.TintLayers.Clear();
                                        foreach (var tint in fContext.Record.TintLayers)
                                            overrideObject.TintLayers.Add(tint.DeepCopy());
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Race
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.NPC_Race, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Race,oContext.Record.Race)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Race,wContext.Record.Race,oContext.Record.Race)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Race.SetTo(fContext.Record.Race);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Voice
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Actors_Voice, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Voice,oContext.Record.Voice)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Voice,wContext.Record.Voice,oContext.Record.Voice)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Voice.SetTo(fContext.Record.Voice);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }
                    
                    //==============================================================================================================
                    // AI Packages Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_AIPackages).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Packages,oContext.Record.Packages))
                            NewPackages.Add(fContext.Record.Packages, oContext.Record.Packages);

                    //==============================================================================================================
                    // Faction Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_Factions).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Factions,oContext.Record.Factions))
                            NewFaction.Add(fContext.Record.Factions);

                    //==============================================================================================================
                    // Inventory Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Invent_Add).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewContainer.Add(fContext.Record.Items);

                    if (Settings.TagList(Tags.Invent_Change).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewContainer.Change(fContext.Record.Items, oContext.Record.Items);

                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                            NewKeywords.Add(fContext.Record.Keywords, oContext.Record.Keywords);

                    //==============================================================================================================
                    // Record Flag Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_RecordFlags).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.SkyrimMajorRecordFlags,oContext.Record.SkyrimMajorRecordFlags))
                            NewFlags.Add(fContext.Record.SkyrimMajorRecordFlags, oContext.Record.SkyrimMajorRecordFlags);

                    //==============================================================================================================
                    // Perk Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_Perks_Add).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Perks,oContext.Record.Perks))
                            NewPerks.Add(fContext.Record.Perks);

                    if (Settings.TagList(Tags.Actors_Perks_Change).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Perks,oContext.Record.Perks))
                            NewPerks.Change(fContext.Record.Perks, oContext.Record.Perks); 

                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // AI Packages Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_AIPackages).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Packages,oContext.Record.Packages))
                            NewPackages.Remove(fContext.Record.Packages, oContext.Record.Packages);

                    //==============================================================================================================
                    // Faction Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_Factions).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Factions,oContext.Record.Factions))
                            NewFaction.Remove(fContext.Record.Factions, oContext.Record.Factions);

                    //==============================================================================================================
                    // Inventory Adds/Changes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Invent_Remove).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Items,oContext.Record.Items))
                            NewContainer.Remove(fContext.Record.Items, oContext.Record.Items);

                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                            NewKeywords.Remove(fContext.Record.Keywords, oContext.Record.Keywords);

                    //==============================================================================================================
                    // Record Flag Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_RecordFlags).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.SkyrimMajorRecordFlags,oContext.Record.SkyrimMajorRecordFlags))
                            NewFlags.Remove(fContext.Record.SkyrimMajorRecordFlags, oContext.Record.SkyrimMajorRecordFlags);

                    //==============================================================================================================
                    // Perk Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Actors_Perks_Remove).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Perks,oContext.Record.Perks))
                            NewPerks.Remove(fContext.Record.Perks, oContext.Record.Perks);
                }

                //==============================================================================================================
                // AI Package Force Adds
                //==============================================================================================================
                foreach(var fContext in modContext)
                    if (Settings.TagList(Tags.Actors_AIPackagesForceAdd).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Packages,oContext.Record.Packages))
                            NewPackages.Add(fContext.Record.Packages, oContext.Record.Packages);

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewPackages.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Packages.SetTo(NewPackages.OverrideObject);
                }
                if (NewFaction.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Factions.SetTo(NewFaction.OverrideObject);
                }
                if (NewContainer.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Items = NewContainer.OverrideObject;
                }
                if (NewKeywords.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                if (NewFlags.Modified)
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.SkyrimMajorRecordFlags = NewFlags.OverrideObject;
                }
                if (NewPerks.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Perks = NewPerks.OverrideObject;
                }
                
            }
        }
    }
}
