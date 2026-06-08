using Mutagen.Bethesda;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Immutable;
using System.Drawing;
using DynamicData.Diagnostics;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using System.Diagnostics.CodeAnalysis;

public static class Tags
{
    // Actor
    public const string Actors_ACBS = "Actors.ACBS";
    public const string Actors_AIData = "Actors.AIData";
    public const string Actors_AIPackages = "Actors.AIPackages";
    public const string Actors_AIPackagesForceAdd = "Actors.AIPackagesForceAdd";
    public const string Actors_CombatStyle = "Actors.CombatStyle";
    public const string Actors_DeathItem = "Actors.DeathItem";
    public const string Actors_Factions = "Actors.Factions";
    public const string Actors_Perks_Add = "Actors.Perks.Add";
    public const string Actors_Perks_Change = "Actors.Perks.Change";
    public const string Actors_Perks_Remove = "Actors.Perks.Remove";
    public const string Actors_RecordFlags = "Actors.RecordFlags";
    public const string Actors_Skeleton = "Actors.Skeleton";
    public const string Actors_Spells = "Actors.Spells";
    public const string Actors_SpellsForceAdd = "Actors.SpellsForceAdd";
    public const string Actors_Stats = "Actors.Stats";
    public const string Actors_Voice = "Actors.Voice";

    // NPC
    public const string NPC_AIPackageOverrides = "NPC.AIPackageOverrides";
    public const string NPC_AttackRace = "NPC.AttackRace";
    public const string NPC_Class = "NPC.Class";
    public const string NPC_CrimeFaction = "NPC.CrimeFaction";
    public const string NPC_DefaultOutfit = "NPC.DefaultOutfit";
    public const string NPC_Race = "NPC.Race";
    public const string NpcFacesForceFullImport = "NpcFacesForceFullImport";

    // Cell
    public const string C_Acoustic = "C.Acoustic";
    public const string C_Climate = "C.Climate";
    public const string C_Encounter = "C.Encounter";
    public const string C_ImageSpace = "C.ImageSpace";
    public const string C_Light = "C.Light";
    public const string C_LockList = "C.LockList";
    public const string C_Location = "C.Location";
    public const string C_MiscFlags = "C.MiscFlags";
    public const string C_Music = "C.Music";
    public const string C_Name = "C.Name";
    public const string C_Owner = "C.Owner";
    public const string C_RecordFlags = "C.RecordFlags";
    public const string C_Regions = "C.Regions";
    public const string C_SkyLighting = "C.SkyLighting";
    public const string C_Water = "C.Water";

    // Forms
    public const string Destructible = "Destructible";
    public const string EffectStats = "EffectStats";
    public const string Enchantments = "Enchantments";
    public const string EnchantmentStats = "EnchantmentStats";
    public const string Graphics = "Graphics";
    public const string Keywords = "Keywords";
    public const string Names = "Names";
    public const string ObjectBounds = "ObjectBounds";
    public const string Scripts = "Scripts";
    public const string Sound = "Sound";
    public const string SpellStats = "SpellStats";
    public const string Stats = "Stats";
    public const string Text = "Text";

    // Inventory
    public const string Invent_Add = "Invent.Add";
    public const string Invent_Change = "Invent.Change";
    public const string Invent_Remove = "Invent.Remove";

    // Leveled Lists
    public const string Delev = "Delev";
    public const string Relev = "Relev";
    public const string Deflst = "Deflst";

    // Outfits
    public const string Outfits_Add = "Outfits.Add";
    public const string Outfits_Remove = "Outfits.Remove";

    // Race
    public const string R_AddSpells = "R.AddSpells";
    public const string R_Body_F = "R.Body-F";
    public const string R_Body_M = "R.Body-M";
    public const string R_Body_Size_F = "R.Body-Size-F";
    public const string R_Body_Size_M = "R.Body-Size-M";
    public const string R_ChangeSpells = "R.ChangeSpells";
    public const string R_Description = "R.Description";
    public const string R_Ears = "R.Ears";
    public const string R_Eyes = "R.Eyes";
    public const string R_Hair = "R.Hair";
    public const string R_Head = "R.Head";
    public const string R_Mouth = "R.Mouth";
    public const string R_Relations_Add = "R.Relations.Add";
    public const string R_Relations_Change = "R.Relations.Change";
    public const string R_Relations_Remove = "R.Relations.Remove";
    public const string R_Skills = "R.Skills";
    public const string R_Teeth = "R.Teeth";
    public const string R_Voice_F = "R.Voice-F";
    public const string R_Voice_M = "R.Voice-M";

    // References
    public const string F_Base = "F.Base";
    public const string F_EnableParent = "F.EnableParent";
    public const string F_LocationReference = "F.LocationReference";

    // Relationships
    public const string Relations_Add = "Relations.Add";
    public const string Relations_Change = "Relations.Change";
    public const string Relations_Remove = "Relations.Remove";
}

public class MappedTags 
{
    Dictionary<string, bool> keywordList = new Dictionary<string, bool>();
    string tag = "";

    public MappedTags()
    {
        foreach (var field in typeof(Tags).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) {
            if (field.IsLiteral && field.FieldType == typeof(string)) {
                keywordList[(string)field.GetRawConstantValue()!] = false;
            }
        }
    }

    public string GetTag()
    {
        return tag;
    }

    public void SetTag(string index) 
    { 
        tag = index; 
    }

    public bool NotMapped() 
    { 
        return !keywordList[tag]; 
    }

    public void SetMapped()
    {
        keywordList[tag] = true;
    }
}

public static class Compare
{
    public static bool NotEqual(IModelGetter? Record1, IModelGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Filename Test
        if (Record1?.File.DataRelativePath != Record2?.File.DataRelativePath)
            return true;    

        // Alternate Texture Test
        if (NullTest(Record1?.AlternateTextures,Record2?.AlternateTextures, out bool Result2)) return Result2;
        bool bTexture = false;
        if (Record1?.AlternateTextures != null && Record2?.AlternateTextures != null)
        {
            foreach (var tex in Record1.AlternateTextures)
                if (!Record2.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                    bTexture = true;

            foreach (var tex in Record2.AlternateTextures)
                if (!Record1.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                    bTexture = true;

            return bTexture;
        }

        return false;
    }

    public static bool NotEqual(IIconsGetter? Record1, IIconsGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.LargeIconFilename.DataRelativePath != Record2?.LargeIconFilename.DataRelativePath)
            return true;
        if (Record1?.SmallIconFilename?.DataRelativePath != Record2?.SmallIconFilename?.DataRelativePath)
            return true;

        return false;
    }

    public static bool NotEqual(IDestructibleGetter? Record1, IDestructibleGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.Data?.Health != Record2?.Data?.Health)
            return true;
        if (Record1?.Data?.DESTCount != Record2?.Data?.DESTCount)
            return true;
        if (Record1?.Data?.VATSTargetable != Record2?.Data?.VATSTargetable)
            return true;

        return false;
    }
    
    public static bool NotEqual(ITranslatedStringGetter? Record1, ITranslatedStringGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.String != Record2?.String)
            return true;

        return false;
    }

    public static bool NotEqual(IObjectBoundsGetter? Record1, IObjectBoundsGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.First.X != Record2?.First.X
            || Record1?.First.Y != Record2?.First.Y
            || Record1?.First.Z != Record2?.First.Z)
            return true;

        if (Record1?.Second.X != Record2?.Second.X
            || Record1?.Second.Y != Record2?.Second.Y
            || Record1?.Second.Z != Record2?.Second.Z)
            return true;

        return false;
    }

    public static bool NotEqual(IAIDataGetter? Record1, IAIDataGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.Aggression != Record2?.Aggression
            || Record1?.Confidence != Record2?.Confidence
            || Record1?.EnergyLevel != Record2?.EnergyLevel
            || Record1?.Responsibility != Record2?.Responsibility
            || Record1?.Mood != Record2?.Mood
            || Record1?.Assistance != Record2?.Assistance)
            return true;

        if (Record1?.AggroRadiusBehavior != Record2?.AggroRadiusBehavior
            || Record1?.Warn != Record2?.Warn
            || Record1?.WarnOrAttack != Record2?.WarnOrAttack
            || Record1?.Attack != Record2?.Attack)
            return true;

        return false;
    }

    public static bool NotEqual(INpcConfigurationGetter? Record1, INpcConfigurationGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Values
        if (Record1?.MagickaOffset != Record2?.MagickaOffset
            || Record1?.StaminaOffset != Record2?.StaminaOffset
            || Record1?.Level != Record2?.Level
            || Record1?.CalcMinLevel != Record2?.CalcMinLevel
            || Record1?.CalcMaxLevel != Record2?.CalcMaxLevel
            || Record1?.SpeedMultiplier != Record2?.SpeedMultiplier
            || Record1?.DispositionBase != Record2?.DispositionBase
            || Record1?.TemplateFlags != Record2?.TemplateFlags
            || Record1?.HealthOffset != Record2?.HealthOffset
            || Record1?.BleedoutOverride != Record2?.BleedoutOverride)
            return true;

        // Record Flags
        return NotEqual(Record1?.Flags, Record2?.Flags);
    }

    public static bool NotEqual(IEnableParentGetter? Record1, IEnableParentGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1, Record2, out bool Result)) return Result;

        // Values
        if (Record1?.Reference != Record2?.Reference)
            return true;

        // Record Flags
        return NotEqual(Record1?.Flags, Record2?.Flags);
    }

    public static bool NotEqual(ICellLightingGetter? Record1, ICellLightingGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Colors
        if (Record1?.AmbientColor.ToArgb() != Record2?.AmbientColor.ToArgb()
            || Record1?.DirectionalColor.ToArgb() != Record2?.DirectionalColor.ToArgb()
            || Record1?.FogNearColor.ToArgb() != Record2?.FogNearColor.ToArgb()
            || Record1?.FogFarColor.ToArgb() != Record2?.FogFarColor.ToArgb())
            return true;

        // Ambient Colors
        if (Record1?.AmbientColors.Specular.ToArgb() != Record2?.AmbientColors.Specular.ToArgb())
            return true;
        if (Record1?.AmbientColors.Scale != Record2?.AmbientColors.Scale)
            return true;
        if (Record1?.AmbientColors.DirectionalXPlus.ToArgb() != Record2?.AmbientColors.DirectionalXPlus.ToArgb()
            || Record1?.AmbientColors.DirectionalXMinus.ToArgb() != Record2?.AmbientColors.DirectionalXMinus.ToArgb()
            || Record1?.AmbientColors.DirectionalYPlus.ToArgb() != Record2?.AmbientColors.DirectionalYPlus.ToArgb()
            || Record1?.AmbientColors.DirectionalYMinus.ToArgb() != Record2?.AmbientColors.DirectionalYMinus.ToArgb()
            || Record1?.AmbientColors.DirectionalZPlus.ToArgb() != Record2?.AmbientColors.DirectionalZPlus.ToArgb()
            || Record1?.AmbientColors.DirectionalZMinus.ToArgb() != Record2?.AmbientColors.DirectionalZMinus.ToArgb())
            return true;

        // Values
        if (Record1?.FogNear != Record2?.FogNear
            || Record1?.FogFar != Record2?.FogFar
            || Record1?.FogClipDistance != Record2?.FogClipDistance
            || Record1?.FogPower != Record2?.FogPower
            || Record1?.FogMax != Record2?.FogMax)
            return true;
        if (Record1?.DirectionalRotationXY != Record2?.DirectionalRotationXY 
            || Record1?.DirectionalRotationZ != Record2?.DirectionalRotationZ 
            || Record1?.DirectionalFade != Record2?.DirectionalFade)
            return true;
        if (Record1?.LightFadeBegin != Record2?.LightFadeBegin
            || Record1?.LightFadeEnd != Record2?.LightFadeEnd)
            return true;

        // Inherits
        return NotEqual(Record1?.Inherits, Record2?.Inherits);
    }

    public static bool NotEqual(IWeaponDataGetter? Record1, IWeaponDataGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Values
        if (Record1?.Speed != Record2?.Speed
            || Record1?.Reach != Record2?.Reach
            || Record1?.SightFOV != Record2?.SightFOV
            || Record1?.BaseVATStoHitChance != Record2?.BaseVATStoHitChance
            || Record1?.NumProjectiles != Record2?.NumProjectiles
            || Record1?.EmbeddedWeaponAV != Record2?.EmbeddedWeaponAV
            || Record1?.RangeMin != Record2?.RangeMin
            || Record1?.RangeMax != Record2?.RangeMax
            || Record1?.AnimationAttackMult != Record2?.AnimationAttackMult
            || Record1?.RumbleLeftMotorStrength != Record2?.RumbleLeftMotorStrength
            || Record1?.RumbleRightMotorStrength != Record2?.RumbleRightMotorStrength
            || Record1?.RumbleDuration != Record2?.RumbleDuration
            || Record1?.Stagger != Record2?.Stagger)
            return true;

        // Record Flags
        if (NotEqual(Record1?.AnimationType, Record2?.AnimationType))
            return true;
        if (NotEqual(Record1?.AttackAnimation, Record2?.AttackAnimation))
            return true;
        if (NotEqual(Record1?.OnHit, Record2?.OnHit))
            return true;
        if (NotEqual(Record1?.Skill, Record2?.Skill))
            return true;
        if (NotEqual(Record1?.Resist, Record2?.Resist))
            return true;

        return false;
    }

    public static bool NotEqual(IWeaponBasicStatsGetter? Record1, IWeaponBasicStatsGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Values
        if (Record1?.Value != Record2?.Value
            || Record1?.Weight != Record2?.Weight
            || Record1?.Damage != Record2?.Damage)
            return true;

        // Record Flags
        return false;
    }

    public static bool NotEqual(ICriticalDataGetter? Record1, ICriticalDataGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Values
        if (Record1?.Damage != Record2?.Damage
            || Record1?.PercentMult != Record2?.PercentMult
            || Record1?.Effect.FormKey != Record2?.Effect.FormKey)
            return true;

        // Record Flags
        return NotEqual(Record1?.Flags, Record2?.Flags);
    }

    public static bool NotEqual(INpcFaceMorphGetter? Record1, INpcFaceMorphGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Values
        if (Record1?.NoseLongVsShort != Record2?.NoseLongVsShort
            || Record1?.NoseUpVsDown != Record2?.NoseUpVsDown
            || Record1?.JawUpVsDown != Record2?.JawUpVsDown
            || Record1?.JawForwardVsBack != Record2?.JawForwardVsBack
            || Record1?.JawNarrowVsWide != Record2?.JawNarrowVsWide
            || Record1?.CheeksUpVsDown != Record2?.CheeksUpVsDown
            || Record1?.CheeksForwardVsBack != Record2?.CheeksForwardVsBack
            || Record1?.EyesForwardVsBack != Record2?.EyesForwardVsBack
            || Record1?.EyesInVsOut != Record2?.EyesInVsOut
            || Record1?.EyesUpVsDown != Record2?.EyesUpVsDown
            || Record1?.BrowsForwardVsBack != Record2?.BrowsForwardVsBack
            || Record1?.BrowsInVsOut != Record2?.BrowsInVsOut
            || Record1?.BrowsUpVsDown != Record2?.BrowsUpVsDown
            || Record1?.LipsInVsOut != Record2?.LipsInVsOut
            || Record1?.LipsUpVsDown != Record2?.LipsUpVsDown
            || Record1?.ChinNarrowVsWide != Record2?.ChinNarrowVsWide
            || Record1?.CheeksUpVsDown != Record2?.CheeksUpVsDown
            || Record1?.ChinUnderbiteVsOverbite != Record2?.ChinUnderbiteVsOverbite)
            return true;

        // Record Flags
        return false;
    }

    public static bool NotEqual(INpcFacePartsGetter? Record1, INpcFacePartsGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Values
        if (Record1?.Nose != Record2?.Nose
            || Record1?.Eyes != Record2?.Eyes
            || Record1?.Mouth != Record2?.Mouth)
            return true;

        // Record Flags
        return false;
    }

    public static bool NotEqual(IVirtualMachineAdapterGetter? Record1, IVirtualMachineAdapterGetter? Record2)
    {
        // // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach (var script in Record1.Scripts)
            {
                if (!Record2.Scripts.Where(x => x.Name == script.Name).Any())
                    return true;
                else
                    foreach (var prop in script.Properties)
                        if (!Record2.Scripts.Where(x => x.Name == script.Name && x.Properties.Equals(prop)).Any())
                            return true;
            }

            foreach (var script in Record2.Scripts)
            {
                if (!Record1.Scripts.Where(x => x.Name == script.Name).Any())
                    return true;
                else
                    foreach (var prop in script.Properties)
                        if (!Record1.Scripts.Where(x => x.Name == script.Name && x.Properties.Equals(prop)).Any())
                            return true;
            }
        }

        return false;
    }

    public static bool NotEqual(IReadOnlyList<ITintLayerGetter>? Record1, IReadOnlyList<ITintLayerGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Index == item.Index && x.Preset == item.Preset && x.InterpolationValue == item.InterpolationValue && x.Color?.ToArgb() == item.Color?.ToArgb()).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Index == item.Index && x.Preset == item.Preset && x.InterpolationValue == item.InterpolationValue && x.Color?.ToArgb() == item.Color?.ToArgb()).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<IContainerEntryGetter>? Record1, IReadOnlyList<IContainerEntryGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Item.Item.FormKey == item.Item.Item.FormKey && x.Item.Count == item.Item.Count).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Item.Item.FormKey == item.Item.Item.FormKey && x.Item.Count == item.Item.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<IPerkPlacementGetter>? Record1, IReadOnlyList<IPerkPlacementGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Perk.FormKey == item.Perk.FormKey && x.Rank == item.Rank).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Perk.FormKey == item.Perk.FormKey && x.Rank == item.Rank).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<IRelationGetter>? Record1, IReadOnlyList<IRelationGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Target.FormKey == item.Target.FormKey && x.Modifier == item.Modifier).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Target.FormKey == item.Target.FormKey && x.Modifier == item.Modifier).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<IRankPlacementGetter>? Record1, IReadOnlyList<IRankPlacementGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Faction.FormKey == item.Faction.FormKey && x.Rank == item.Rank).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Faction.FormKey == item.Faction.FormKey && x.Rank == item.Rank).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<ILeveledItemEntryGetter>? Record1, IReadOnlyList<ILeveledItemEntryGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<ILeveledNpcEntryGetter>? Record1, IReadOnlyList<ILeveledNpcEntryGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<ILeveledSpellEntryGetter>? Record1, IReadOnlyList<ILeveledSpellEntryGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IGenderedItemGetter<IArmorModelGetter?>? Record1, IGenderedItemGetter<IArmorModelGetter?>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (NotEqual(Record1?.Male?.Model,Record2?.Male?.Model))
            return true;

        if (NotEqual(Record1?.Female?.Model,Record2?.Female?.Model))
            return true;

        return false;
    }

    public static bool NotEqual(IGenderedItemGetter<IModelGetter?>? Record1, IGenderedItemGetter<IModelGetter?>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (NotEqual(Record1?.Male,Record2?.Male))
            return true;

        if (NotEqual(Record1?.Female,Record2?.Female))
            return true;

        return false;
    }

    public static bool NotEqual(IReadOnlyList<IFormLinkGetter<ISkyrimMajorRecordGetter>>? Record1, IReadOnlyList<IFormLinkGetter<ISkyrimMajorRecordGetter>>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            foreach(var item in Record1)
                if (!Record2.Where(x => x.FormKey == item.FormKey).Any())
                    return true;

            foreach(var item in Record2)
                if (!Record1.Where(x => x.FormKey == item.FormKey).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IFormLinkNullableGetter<ISkyrimMajorRecordGetter>? Record1, IFormLinkNullableGetter<ISkyrimMajorRecordGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null && Record1.FormKey != Record2.FormKey)
            return true;

        return false;
    }

    public static bool NotEqual(IFormLinkGetter<ISkyrimMajorRecordGetter>? Record1, IFormLinkGetter<ISkyrimMajorRecordGetter>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null && Record1.FormKey != Record2.FormKey)
            return true;

        return false;
    }

    public static bool NotEqual<TEnum>(TEnum Record1, TEnum Record2) where TEnum : struct, System.Enum
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        foreach (var flag in Enums<TEnum>.Values)
        {
            if(Record1.HasFlag(flag) && !Record2.HasFlag(flag)) return true;
            if(!Record1.HasFlag(flag) && Record2.HasFlag(flag)) return true;
        }

        return false;
    }

    public static bool NotEqual<TEnum>(TEnum? Record1, TEnum? Record2) where TEnum : struct, System.Enum
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        foreach (var flag in Enums<TEnum>.Values)
        {
            if(Record1.GetValueOrDefault().HasFlag(flag) && !Record2.GetValueOrDefault().HasFlag(flag)) return true;
            if(!Record1.GetValueOrDefault().HasFlag(flag) && Record2.GetValueOrDefault().HasFlag(flag)) return true;
        }

        return false;
    }

    public static bool NotEqual(Color? Record1, Color? Record2)
    {
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        else if (Record1 != null && Record2 != null && Record1.Value.ToArgb() != Record2.Value.ToArgb())
            return true;
        else
            return false;
    }

    public static bool NotEqual(string? Record1, string? Record2)
    {
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        else if (Record1 != null && Record2 != null && Record1 != Record2)
            return true;
        else
            return false;
    }

    public static bool NotEqual(float? Record1, float? Record2)
    {
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        else if (Record1 != null && Record2 != null && Record1 != Record2)
            return true;
        else
            return false;
    }

    public static bool NotEqual(uint? Record1, uint? Record2)
    {
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        else if (Record1 != null && Record2 != null && Record1 != Record2)
            return true;
        else
            return false;
    }

    public static bool NotEqual(object? Record1, object? Record2)
    {
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        else if (Record1 != null && !Record1.Equals(Record2))
            return true;
        else
            return false;
    }

    public static bool NullTest(object? Obj1, object? Obj2, out bool value)
    {
        // Null Test
        if (Obj1 == null && Obj2 == null)
        {
            value = false;
            return true;
        }
        else if ((Obj1 != null && Obj2 == null) || (Obj1 == null && Obj2 != null))
        {
            value = true;
            return true;
        }
        else
        {
            value = true;
            return false;      
        }  
    }

}

public class Utility
{
    public static HashSet<FormKey> GetAffectedFormKeys<T>(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, HashSet<ModKey> workingModList) where T : class, IMajorRecordGetter
    {
        HashSet<FormKey> affectedFormKeys = [];

        foreach (var mod in state.LoadOrder.PriorityOrder) {
            if (!workingModList.Contains(mod.ModKey) || mod.Mod is null)
                continue;

            foreach (var record in mod.Mod.EnumerateMajorRecords<T>()) {
                affectedFormKeys.Add(record.FormKey);
            }
        }
        
        return affectedFormKeys;
    }

    public static void RecordCountMessage(int iRecordCount, string sRecordType)
    {
        if (iRecordCount > 0)
            Console.WriteLine("Processing {0} {1} Records", iRecordCount, sRecordType);
    }

    public static bool TagCheck(string record, MappedTags mapped, Fusion.SettingsUtility Settings, IModContext found)
    {
        mapped.SetTag(record);
        return mapped.NotMapped() && Settings.TagList(record).Contains(found.ModKey);
    }

    public static bool CheckContext(IModContext found, IModContext working, IModContext original)
    {
        return found.ModKey != working.ModKey && found.ModKey != original.ModKey;
    }

    public static bool ShouldChange<T>(T FoundRecord, T WorkingRecord, T OriginalRecord)
    {
        return Compare.NotEqual(FoundRecord,WorkingRecord) && Compare.NotEqual(FoundRecord,OriginalRecord);
    }

    public static bool ShouldChangeNull<T>([NotNullWhen(true)] T? FoundRecord, T? WorkingRecord, T? OriginalRecord)
    {
        return FoundRecord != null && Compare.NotEqual(FoundRecord,WorkingRecord) && Compare.NotEqual(FoundRecord,OriginalRecord);
    }   
    public static GenderedItem<T?>? NewGender<T>(T? Male, T? Female)
    {
        if (Male == null && Female == null) return null;
        else return new GenderedItem<T?>(Male, Female);
    }

    public static TranslatedString? NewString(ITranslatedStringGetter? text)
    {
        if (text == null) return null;
        else return new TranslatedString(text.TargetLanguage, text.String);
    }

    public static TranslatedString NewStringNotNull(ITranslatedStringGetter text)
    {
        return new TranslatedString(text.TargetLanguage, text.String);
    }
}

public class Keywords
{
    public ExtendedList<IFormLinkGetter<IKeywordGetter>> OverrideObject {get; set;}
    public bool Modified;
    
    public Keywords(IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec);        
    }
    
    public void Add(IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (OriginalObject != null && !OriginalObject.Contains(rec) && !OverrideObject.Contains(rec))
                {
                    OverrideObject.Add(rec);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            if (OriginalObject != null)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Contains(rec) && OverrideObject.Contains(rec))
                    {
                        OverrideObject.Remove(rec);
                        Modified = true;
                    }
    }

}

public class Flags<TEnum> where TEnum : struct, System.Enum
{
    public TEnum OverrideObject {get; set;}
    public bool Modified;

    public Flags(TEnum WorkingRecord)
    {
        OverrideObject = WorkingRecord;
        Modified = false;     
    }
    
    public void Add(TEnum FoundObject, TEnum OriginalObject)
    {
        foreach (dynamic rec in Enums<TEnum>.Values)
            if (FoundObject.HasFlag(rec) && !OriginalObject.HasFlag(rec) && !OverrideObject.HasFlag(rec))
            {
                OverrideObject |= rec;
                Modified = true;
            }
    }
    
    public void Remove(TEnum FoundObject, TEnum OriginalObject)
    {
        foreach (dynamic rec in Enums<TEnum>.Values)
            if (!FoundObject.HasFlag(rec) && OriginalObject.HasFlag(rec) && OverrideObject.HasFlag(rec))
            {
                OverrideObject &= ~rec;
                Modified = true;
            }
    }
}

public class Regions
{
    public ExtendedList<IFormLinkGetter<IRegionGetter>> OverrideObject {get; set;}
    public bool Modified;

    public Regions(IReadOnlyList<IFormLinkGetter<IRegionGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec);        
    }
    
    public void Add(IReadOnlyList<IFormLinkGetter<IRegionGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IRegionGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (OriginalObject != null && !OriginalObject.Contains(rec) && !OverrideObject.Contains(rec))
                {
                    OverrideObject.Add(rec);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IRegionGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IRegionGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            if (OriginalObject != null)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Contains(rec) && OverrideObject.Contains(rec))
                    {
                        OverrideObject.Remove(rec);
                        Modified = true;
                    }
    }

}

public class Packages
{
    public ExtendedList<IFormLinkGetter<IPackageGetter>> OverrideObject {get; set;}
    public bool Modified;

    public Packages(IReadOnlyList<IFormLinkGetter<IPackageGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec);        
    }
    
    public void Add(IReadOnlyList<IFormLinkGetter<IPackageGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IPackageGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (OriginalObject != null && !OriginalObject.Contains(rec) && !OverrideObject.Contains(rec))
                {
                    OverrideObject.Add(rec);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IPackageGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IPackageGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            if (OriginalObject != null)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Contains(rec) && OverrideObject.Contains(rec))
                    {
                        OverrideObject.Remove(rec);
                        Modified = true;
                    }
    }

}

public class Factions
{
    public ExtendedList<RankPlacement> OverrideObject {get; set;}
    public bool Modified;

    public Factions(IReadOnlyList<IRankPlacementGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());        
    }
   
   public void Add(IReadOnlyList<IRankPlacementGetter>? FoundContext)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (!OverrideObject.Where(x => x.Faction.FormKey == rec.Faction.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IRankPlacementGetter>? FoundContext, IReadOnlyList<IRankPlacementGetter>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Where(x => x.Faction.FormKey == rec.Faction.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Faction.FormKey == rec.Faction.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IRankPlacementGetter>? FoundContext, IReadOnlyList<IRankPlacementGetter>? OriginalObject)
    {
        if (FoundContext != null && OriginalObject != null)
            foreach (var rec in FoundContext)
                if (!OriginalObject.Where(x => x.Faction.FormKey == rec.Faction.FormKey && x.Rank == rec.Rank).Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Faction.FormKey == rec.Faction.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(rec.DeepCopy());
                        Modified = true;
                    }
                }
    }

}

public class Relations
{
    public ExtendedList<Relation> OverrideObject {get; set;}
    public bool Modified;

    public Relations(IReadOnlyList<IRelationGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());        
    }
    
    public void Add(IReadOnlyList<IRelationGetter>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (!OverrideObject.Where(x => x.Target.FormKey == rec.Target.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IRelationGetter>? FoundObject, IReadOnlyList<IRelationGetter>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Where(x => x.Target.FormKey == rec.Target.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Target.FormKey == rec.Target.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IRelationGetter>? FoundObject, IReadOnlyList<IRelationGetter>? OriginalObject)
    {
        if (FoundObject != null && OriginalObject != null)
            foreach (var rec in OriginalObject)
            {
                var oFoundOrg = FoundObject.Where(x => x.Target.FormKey == rec.Target.FormKey && x.Modifier != rec.Modifier);
                if (oFoundOrg.Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Target.FormKey == rec.Target.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(oFoundOrg.First().DeepCopy());
                        Modified = true;
                    }
                }
            }
    }

}

public class Perks
{
    public ExtendedList<PerkPlacement> OverrideObject {get; set;}
    public bool Modified;

    public Perks(IReadOnlyList<IPerkPlacementGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());        
    }
   
    public void Add(IReadOnlyList<IPerkPlacementGetter>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (!OverrideObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IPerkPlacementGetter>? FoundObject, IReadOnlyList<IPerkPlacementGetter>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IPerkPlacementGetter>? FoundObject, IReadOnlyList<IPerkPlacementGetter>? OriginalObject)
    {
        if (FoundObject != null && OriginalObject != null)
            foreach (var rec in OriginalObject)
            {
                var oFoundOrg = FoundObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey && x.Rank != rec.Rank);
                if (oFoundOrg.Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(oFoundOrg.First().DeepCopy());
                        Modified = true;
                    }
                }
            }
    }

}

public class Containers
{
    public ExtendedList<ContainerEntry> OverrideObject {get; set;}
    public bool Modified;

    public Containers(IReadOnlyList<IContainerEntryGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());
    }
    
    public void Add(IReadOnlyList<IContainerEntryGetter>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (!OverrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IContainerEntryGetter>? FoundObject, IReadOnlyList<IContainerEntryGetter>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IContainerEntryGetter>? FoundObject, IReadOnlyList<IContainerEntryGetter>? OriginalObject)
    {
        if (FoundObject != null && OriginalObject != null)
            foreach (var rec in OriginalObject)
            {
                var oFoundOrg = FoundObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey && x.Item.Count != rec.Item.Count);
                if (oFoundOrg.Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(oFoundOrg.First().DeepCopy());
                        Modified = true;
                    }
                }
            }
    }
}

public class Leveled
{
    public ExtendedList<LeveledItemEntry> OverrideItemObject {get; set;}
    public ExtendedList<LeveledNpcEntry> OverrideNpcObject {get; set;}
    public ExtendedList<LeveledSpellEntry> OverrideSpellObject {get; set;}
    public bool Modified;

    public Leveled(IReadOnlyList<ILeveledItemEntryGetter>? WorkingRecord)
    {
        OverrideItemObject = new();
        OverrideNpcObject = new();
        OverrideSpellObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideItemObject.Add(rec.DeepCopy());        
    }

    public Leveled(IReadOnlyList<ILeveledNpcEntryGetter>? WorkingRecord)
    {
        OverrideItemObject = new();
        OverrideNpcObject = new();
        OverrideSpellObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideNpcObject.Add(rec.DeepCopy());        
    }

    public Leveled(IReadOnlyList<ILeveledSpellEntryGetter>? WorkingRecord)
    {
        OverrideItemObject = new();
        OverrideNpcObject = new();
        OverrideSpellObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideSpellObject.Add(rec.DeepCopy());        
    }

    public void Add(IReadOnlyList<ILeveledItemEntryGetter>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (rec.Data?.Reference != null && !OverrideItemObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                {
                    OverrideItemObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<ILeveledItemEntryGetter>? FoundObject, IReadOnlyList<ILeveledItemEntryGetter>? OriginalObject)
    {
        if (FoundObject != null && OriginalObject != null)
            foreach (var rec in OriginalObject)
                if (rec.Data?.Reference != null && !FoundObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                {
                    var oFoundRec = OverrideItemObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideItemObject.Remove(oFoundRec.First());
                        Modified = true;
                    }
                }
    }

    public void Add(IReadOnlyList<ILeveledNpcEntryGetter>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (rec.Data?.Reference != null && !OverrideNpcObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                {
                    OverrideNpcObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<ILeveledNpcEntryGetter>? FoundObject, IReadOnlyList<ILeveledNpcEntryGetter>? OriginalObject)
    {
        if (FoundObject != null && OriginalObject != null)
            foreach (var rec in OriginalObject)
                if (rec.Data?.Reference != null && !FoundObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                {
                    var oFoundRec = OverrideNpcObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideNpcObject.Remove(oFoundRec.First());
                        Modified = true;
                    }
                }
    }

    public void Add(IReadOnlyList<ILeveledSpellEntryGetter>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (rec.Data?.Reference != null && !OverrideSpellObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                {
                    OverrideSpellObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<ILeveledSpellEntryGetter>? FoundObject, IReadOnlyList<ILeveledSpellEntryGetter>? OriginalObject)
    {
        if (FoundObject != null && OriginalObject != null)
            foreach (var rec in OriginalObject)
                if (rec.Data?.Reference != null && !FoundObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey).Any())
                {
                    var oFoundRec = OverrideSpellObject.Where(x => x.Data?.Reference.FormKey == rec.Data.Reference.FormKey);
                    if (oFoundRec.Any())
                    {
                        OverrideSpellObject.Remove(oFoundRec.First());
                        Modified = true;
                    }
                }
    }

}

public class Outfits
{
    public ExtendedList<IFormLinkGetter<IOutfitTarget>> OverrideObject {get; set;}
    public bool Modified;

    public Outfits(IReadOnlyList<IFormLinkGetter<IOutfitTargetGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.FormKey);        
    }
   
    public void Add(IReadOnlyList<IFormLinkGetter<IOutfitTargetGetter>>? FoundObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            foreach (var rec in FoundObject)
                if (!OverrideObject.Where(x => x.FormKey == rec.FormKey).Any())
                {
                    OverrideObject.Add(rec.FormKey);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IOutfitTargetGetter>>? FoundObject, IReadOnlyList<IFormLinkGetter<IOutfitTargetGetter>>? OriginalObject)
    {
        if (FoundObject != null && FoundObject.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundObject.Where(x => x.FormKey == rec.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.FormKey == rec.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }
}

public class EffectShaders
{
    public static bool DATACheck(IEffectShaderGetter lhs, IEffectShaderGetter rhs)
    {
        if (Compare.NotEqual(lhs.MembraneSourceBlendMode, rhs.MembraneSourceBlendMode)) return false;
        if (Compare.NotEqual(lhs.MembraneBlendOperation, rhs.MembraneBlendOperation)) return false;
        if (Compare.NotEqual(lhs.MembraneZTest, rhs.MembraneZTest)) return false;
        if (Compare.NotEqual(lhs.FillColorKey1, rhs.FillColorKey1)) return false;
        if (Compare.NotEqual(lhs.FillAlphaFadeInTime, rhs.FillAlphaFadeInTime)) return false;
        if (Compare.NotEqual(lhs.FillFullAlphaTime, rhs.FillFullAlphaTime)) return false;
        if (Compare.NotEqual(lhs.FillFadeOutTime, rhs.FillFadeOutTime)) return false;
        if (Compare.NotEqual(lhs.FillPersistentAlphaRatio, rhs.FillPersistentAlphaRatio)) return false;
        if (Compare.NotEqual(lhs.FillAlphaPulseAmplitude, rhs.FillAlphaPulseAmplitude)) return false;
        if (Compare.NotEqual(lhs.FillAlphaPulseFrequency, rhs.FillAlphaPulseFrequency)) return false;
        if (Compare.NotEqual(lhs.FillTextureAnimationSpeedU, rhs.FillTextureAnimationSpeedU)) return false;
        if (Compare.NotEqual(lhs.FillTextureAnimationSpeedV, rhs.FillTextureAnimationSpeedV)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectFallOff, rhs.EdgeEffectFallOff)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectColor, rhs.EdgeEffectColor)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectAlphaFadeInTime, rhs.EdgeEffectAlphaFadeInTime)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectFullAlphaTime, rhs.EdgeEffectFullAlphaTime)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectAlphaFadeOutTime, rhs.EdgeEffectAlphaFadeOutTime)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectPersistentAlphaRatio, rhs.EdgeEffectPersistentAlphaRatio)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectAlphaPulseAmplitude, rhs.EdgeEffectAlphaPulseAmplitude)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectAlphaPulseFrequency, rhs.EdgeEffectAlphaPulseFrequency)) return false;
        if (Compare.NotEqual(lhs.FillFullAlphaRatio, rhs.FillFullAlphaRatio)) return false;
        if (Compare.NotEqual(lhs.EdgeEffectFullAlphaRatio, rhs.EdgeEffectFullAlphaRatio)) return false;
        if (Compare.NotEqual(lhs.MembraneDestBlendMode, rhs.MembraneDestBlendMode)) return false;
        if (Compare.NotEqual(lhs.ParticleSourceBlendMode, rhs.ParticleSourceBlendMode)) return false;
        if (Compare.NotEqual(lhs.ParticleBlendOperation, rhs.ParticleBlendOperation)) return false;
        if (Compare.NotEqual(lhs.ParticleZTest, rhs.ParticleZTest)) return false;
        if (Compare.NotEqual(lhs.ParticleDestBlendMode, rhs.ParticleDestBlendMode)) return false;
        if (Compare.NotEqual(lhs.ParticleBirthRampUpTime, rhs.ParticleBirthRampUpTime)) return false;
        if (Compare.NotEqual(lhs.ParticleFullBirthTime, rhs.ParticleFullBirthTime)) return false;
        if (Compare.NotEqual(lhs.ParticleBirthRampDownTime, rhs.ParticleBirthRampDownTime)) return false;
        if (Compare.NotEqual(lhs.ParticleFullBirthRatio, rhs.ParticleFullBirthRatio)) return false;
        if (Compare.NotEqual(lhs.ParticlePeristentCount, rhs.ParticlePeristentCount)) return false;
        if (Compare.NotEqual(lhs.ParticleLifetime, rhs.ParticleLifetime)) return false;
        if (Compare.NotEqual(lhs.ParticleLifetimePlusMinus, rhs.ParticleLifetimePlusMinus)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialSpeedAlongNormal, rhs.ParticleInitialSpeedAlongNormal)) return false;
        if (Compare.NotEqual(lhs.ParticleAccelerationAlongNormal, rhs.ParticleAccelerationAlongNormal)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialVelocity1, rhs.ParticleInitialVelocity1)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialVelocity2, rhs.ParticleInitialVelocity2)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialVelocity3, rhs.ParticleInitialVelocity3)) return false;
        if (Compare.NotEqual(lhs.ParticleAcceleration1, rhs.ParticleAcceleration1)) return false;
        if (Compare.NotEqual(lhs.ParticleAcceleration2, rhs.ParticleAcceleration2)) return false;
        if (Compare.NotEqual(lhs.ParticleAcceleration3, rhs.ParticleAcceleration3)) return false;
        if (Compare.NotEqual(lhs.ParticleScaleKey1, rhs.ParticleScaleKey1)) return false;
        if (Compare.NotEqual(lhs.ParticleScaleKey2, rhs.ParticleScaleKey2)) return false;
        if (Compare.NotEqual(lhs.ParticleScaleKey1Time, rhs.ParticleScaleKey1Time)) return false;
        if (Compare.NotEqual(lhs.ParticleScaleKey2Time, rhs.ParticleScaleKey2Time)) return false;
        if (Compare.NotEqual(lhs.ColorKey1, rhs.ColorKey1)) return false;
        if (Compare.NotEqual(lhs.ColorKey2, rhs.ColorKey2)) return false;
        if (Compare.NotEqual(lhs.ColorKey3, rhs.ColorKey3)) return false;
        if (Compare.NotEqual(lhs.ColorKey1Alpha, rhs.ColorKey1Alpha)) return false;
        if (Compare.NotEqual(lhs.ColorKey2Alpha, rhs.ColorKey2Alpha)) return false;
        if (Compare.NotEqual(lhs.ColorKey3Alpha, rhs.ColorKey3Alpha)) return false;
        if (Compare.NotEqual(lhs.ColorKey1Time, rhs.ColorKey1Time)) return false;
        if (Compare.NotEqual(lhs.ColorKey2Time, rhs.ColorKey2Time)) return false;
        if (Compare.NotEqual(lhs.ColorKey3Time, rhs.ColorKey3Time)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialSpeedAlongNormalPlusMinus, rhs.ParticleInitialSpeedAlongNormalPlusMinus)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialRotationDegree, rhs.ParticleInitialRotationDegree)) return false;
        if (Compare.NotEqual(lhs.ParticleInitialRotationDegreePlusMinus, rhs.ParticleInitialRotationDegreePlusMinus)) return false;
        if (Compare.NotEqual(lhs.ParticleRotationSpeedDegreePerSec, rhs.ParticleRotationSpeedDegreePerSec)) return false;
        if (Compare.NotEqual(lhs.ParticleRotationSpeedDegreePerSecPlusMinus, rhs.ParticleRotationSpeedDegreePerSecPlusMinus)) return false;
        if (Compare.NotEqual(lhs.AddonModels, rhs.AddonModels)) return false;
        if (Compare.NotEqual(lhs.HolesStartTime, rhs.HolesStartTime)) return false;
        if (Compare.NotEqual(lhs.HolesEndTime, rhs.HolesEndTime)) return false;
        if (Compare.NotEqual(lhs.HolesStartValue, rhs.HolesStartValue)) return false;
        if (Compare.NotEqual(lhs.HolesEndValue, rhs.HolesEndValue)) return false;
        if (Compare.NotEqual(lhs.EdgeWidth, rhs.EdgeWidth)) return false;
        if (Compare.NotEqual(lhs.EdgeColor, rhs.EdgeColor)) return false;
        if (Compare.NotEqual(lhs.ExplosionWindSpeed, rhs.ExplosionWindSpeed)) return false;
        if (Compare.NotEqual(lhs.TextureCountU, rhs.TextureCountU)) return false;
        if (Compare.NotEqual(lhs.TextureCountV, rhs.TextureCountV)) return false;
        if (Compare.NotEqual(lhs.AddonModelsFadeInTime, rhs.AddonModelsFadeInTime)) return false;
        if (Compare.NotEqual(lhs.AddonModelsFadeOutTime, rhs.AddonModelsFadeOutTime)) return false;
        if (Compare.NotEqual(lhs.AddonModelsScaleStart, rhs.AddonModelsScaleStart)) return false;
        if (Compare.NotEqual(lhs.AddonModelsScaleEnd, rhs.AddonModelsScaleEnd)) return false;
        if (Compare.NotEqual(lhs.AddonModelsScaleInTime, rhs.AddonModelsScaleInTime)) return false;
        if (Compare.NotEqual(lhs.AddonModelsScaleOutTime, rhs.AddonModelsScaleOutTime)) return false;
        if (Compare.NotEqual(lhs.FillColorKey2, rhs.FillColorKey2)) return false;
        if (Compare.NotEqual(lhs.FillColorKey3, rhs.FillColorKey3)) return false;
        if (Compare.NotEqual(lhs.FillColorKey1Scale, rhs.FillColorKey1Scale)) return false;
        if (Compare.NotEqual(lhs.FillColorKey2Scale, rhs.FillColorKey2Scale)) return false;
        if (Compare.NotEqual(lhs.FillColorKey3Scale, rhs.FillColorKey3Scale)) return false;
        if (Compare.NotEqual(lhs.FillColorKey1Time, rhs.FillColorKey1Time)) return false;
        if (Compare.NotEqual(lhs.FillColorKey2Time, rhs.FillColorKey2Time)) return false;
        if (Compare.NotEqual(lhs.FillColorKey3Time, rhs.FillColorKey3Time)) return false;
        if (Compare.NotEqual(lhs.ColorScale, rhs.ColorScale)) return false;
        if (Compare.NotEqual(lhs.BirthPositionOffset, rhs.BirthPositionOffset)) return false;
        if (Compare.NotEqual(lhs.BirthPositionOffsetRangePlusMinus, rhs.BirthPositionOffsetRangePlusMinus)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedStartFrame, rhs.ParticleAnimatedStartFrame)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedStartFrameVariation, rhs.ParticleAnimatedStartFrameVariation)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedEndFrame, rhs.ParticleAnimatedEndFrame)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedLoopStartFrame, rhs.ParticleAnimatedLoopStartFrame)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedLoopStartVariation, rhs.ParticleAnimatedLoopStartVariation)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedFrameCount, rhs.ParticleAnimatedFrameCount)) return false;
        if (Compare.NotEqual(lhs.ParticleAnimatedFrameCountVariation, rhs.ParticleAnimatedFrameCountVariation)) return false;
        if (Compare.NotEqual(lhs.FillTextureScaleU, rhs.FillTextureScaleU)) return false;
        if (Compare.NotEqual(lhs.FillTextureScaleV, rhs.FillTextureScaleV)) return false;
        if (Compare.NotEqual(lhs.SceneGraphEmitDepthLimit, rhs.SceneGraphEmitDepthLimit)) return false;
        if (Compare.NotEqual(lhs.DATADataTypeState, rhs.DATADataTypeState)) return false;
        return true;
    }

    public static void DATADeepCopy(IEffectShader lhs, IEffectShaderGetter rhs)
    {
        lhs.MembraneSourceBlendMode = rhs.MembraneSourceBlendMode;
        lhs.MembraneBlendOperation = rhs.MembraneBlendOperation;
        lhs.MembraneZTest = rhs.MembraneZTest;
        lhs.FillColorKey1 = rhs.FillColorKey1;
        lhs.FillAlphaFadeInTime = rhs.FillAlphaFadeInTime;
        lhs.FillFullAlphaTime = rhs.FillFullAlphaTime;
        lhs.FillFadeOutTime = rhs.FillFadeOutTime;
        lhs.FillPersistentAlphaRatio = rhs.FillPersistentAlphaRatio;
        lhs.FillAlphaPulseAmplitude = rhs.FillAlphaPulseAmplitude;
        lhs.FillAlphaPulseFrequency = rhs.FillAlphaPulseFrequency;
        lhs.FillTextureAnimationSpeedU = rhs.FillTextureAnimationSpeedU;
        lhs.FillTextureAnimationSpeedV = rhs.FillTextureAnimationSpeedV;
        lhs.EdgeEffectFallOff = rhs.EdgeEffectFallOff;
        lhs.EdgeEffectColor = rhs.EdgeEffectColor;
        lhs.EdgeEffectAlphaFadeInTime = rhs.EdgeEffectAlphaFadeInTime;
        lhs.EdgeEffectFullAlphaTime = rhs.EdgeEffectFullAlphaTime;
        lhs.EdgeEffectAlphaFadeOutTime = rhs.EdgeEffectAlphaFadeOutTime;
        lhs.EdgeEffectPersistentAlphaRatio = rhs.EdgeEffectPersistentAlphaRatio;
        lhs.EdgeEffectAlphaPulseAmplitude = rhs.EdgeEffectAlphaPulseAmplitude;
        lhs.EdgeEffectAlphaPulseFrequency = rhs.EdgeEffectAlphaPulseFrequency;
        lhs.FillFullAlphaRatio = rhs.FillFullAlphaRatio;
        lhs.EdgeEffectFullAlphaRatio = rhs.EdgeEffectFullAlphaRatio;
        lhs.MembraneDestBlendMode = rhs.MembraneDestBlendMode;
        lhs.ParticleSourceBlendMode = rhs.ParticleSourceBlendMode;
        lhs.ParticleBlendOperation = rhs.ParticleBlendOperation;
        lhs.ParticleZTest = rhs.ParticleZTest;
        lhs.ParticleDestBlendMode = rhs.ParticleDestBlendMode;
        lhs.ParticleBirthRampUpTime = rhs.ParticleBirthRampUpTime;
        lhs.ParticleFullBirthTime = rhs.ParticleFullBirthTime;
        lhs.ParticleBirthRampDownTime = rhs.ParticleBirthRampDownTime;
        lhs.ParticleFullBirthRatio = rhs.ParticleFullBirthRatio;
        lhs.ParticlePeristentCount = rhs.ParticlePeristentCount;
        lhs.ParticleLifetime = rhs.ParticleLifetime;
        lhs.ParticleLifetimePlusMinus = rhs.ParticleLifetimePlusMinus;
        lhs.ParticleInitialSpeedAlongNormal = rhs.ParticleInitialSpeedAlongNormal;
        lhs.ParticleAccelerationAlongNormal = rhs.ParticleAccelerationAlongNormal;
        lhs.ParticleInitialVelocity1 = rhs.ParticleInitialVelocity1;
        lhs.ParticleInitialVelocity2 = rhs.ParticleInitialVelocity2;
        lhs.ParticleInitialVelocity3 = rhs.ParticleInitialVelocity3;
        lhs.ParticleAcceleration1 = rhs.ParticleAcceleration1;
        lhs.ParticleAcceleration2 = rhs.ParticleAcceleration2;
        lhs.ParticleAcceleration3 = rhs.ParticleAcceleration3;
        lhs.ParticleScaleKey1 = rhs.ParticleScaleKey1;
        lhs.ParticleScaleKey2 = rhs.ParticleScaleKey2;
        lhs.ParticleScaleKey1Time = rhs.ParticleScaleKey1Time;
        lhs.ParticleScaleKey2Time = rhs.ParticleScaleKey2Time;
        lhs.ColorKey1 = rhs.ColorKey1;
        lhs.ColorKey2 = rhs.ColorKey2;
        lhs.ColorKey3 = rhs.ColorKey3;
        lhs.ColorKey1Alpha = rhs.ColorKey1Alpha;
        lhs.ColorKey2Alpha = rhs.ColorKey2Alpha;
        lhs.ColorKey3Alpha = rhs.ColorKey3Alpha;
        lhs.ColorKey1Time = rhs.ColorKey1Time;
        lhs.ColorKey2Time = rhs.ColorKey2Time;
        lhs.ColorKey3Time = rhs.ColorKey3Time;
        lhs.ParticleInitialSpeedAlongNormalPlusMinus = rhs.ParticleInitialSpeedAlongNormalPlusMinus;
        lhs.ParticleInitialRotationDegree = rhs.ParticleInitialRotationDegree;
        lhs.ParticleInitialRotationDegreePlusMinus = rhs.ParticleInitialRotationDegreePlusMinus;
        lhs.ParticleRotationSpeedDegreePerSec = rhs.ParticleRotationSpeedDegreePerSec;
        lhs.ParticleRotationSpeedDegreePerSecPlusMinus = rhs.ParticleRotationSpeedDegreePerSecPlusMinus;
        lhs.AddonModels.SetTo(rhs.AddonModels);
        lhs.HolesStartTime = rhs.HolesStartTime;
        lhs.HolesEndTime = rhs.HolesEndTime;
        lhs.HolesStartValue = rhs.HolesStartValue;
        lhs.HolesEndValue = rhs.HolesEndValue;
        lhs.EdgeWidth = rhs.EdgeWidth;
        lhs.EdgeColor = rhs.EdgeColor;
        lhs.ExplosionWindSpeed = rhs.ExplosionWindSpeed;
        lhs.TextureCountU = rhs.TextureCountU;
        lhs.TextureCountV = rhs.TextureCountV;
        lhs.AddonModelsFadeInTime = rhs.AddonModelsFadeInTime;
        lhs.AddonModelsFadeOutTime = rhs.AddonModelsFadeOutTime;
        lhs.AddonModelsScaleStart = rhs.AddonModelsScaleStart;
        lhs.AddonModelsScaleEnd = rhs.AddonModelsScaleEnd;
        lhs.AddonModelsScaleInTime = rhs.AddonModelsScaleInTime;
        lhs.AddonModelsScaleOutTime = rhs.AddonModelsScaleOutTime;
        lhs.FillColorKey2 = rhs.FillColorKey2;
        lhs.FillColorKey3 = rhs.FillColorKey3;
        lhs.FillColorKey1Scale = rhs.FillColorKey1Scale;
        lhs.FillColorKey2Scale = rhs.FillColorKey2Scale;
        lhs.FillColorKey3Scale = rhs.FillColorKey3Scale;
        lhs.FillColorKey1Time = rhs.FillColorKey1Time;
        lhs.FillColorKey2Time = rhs.FillColorKey2Time;
        lhs.FillColorKey3Time = rhs.FillColorKey3Time;
        lhs.ColorScale = rhs.ColorScale;
        lhs.BirthPositionOffset = rhs.BirthPositionOffset;
        lhs.BirthPositionOffsetRangePlusMinus = rhs.BirthPositionOffsetRangePlusMinus;
        lhs.ParticleAnimatedStartFrame = rhs.ParticleAnimatedStartFrame;
        lhs.ParticleAnimatedStartFrameVariation = rhs.ParticleAnimatedStartFrameVariation;
        lhs.ParticleAnimatedEndFrame = rhs.ParticleAnimatedEndFrame;
        lhs.ParticleAnimatedLoopStartFrame = rhs.ParticleAnimatedLoopStartFrame;
        lhs.ParticleAnimatedLoopStartVariation = rhs.ParticleAnimatedLoopStartVariation;
        lhs.ParticleAnimatedFrameCount = rhs.ParticleAnimatedFrameCount;
        lhs.ParticleAnimatedFrameCountVariation = rhs.ParticleAnimatedFrameCountVariation;
        lhs.FillTextureScaleU = rhs.FillTextureScaleU;
        lhs.FillTextureScaleV = rhs.FillTextureScaleV;
        lhs.SceneGraphEmitDepthLimit = rhs.SceneGraphEmitDepthLimit;
        lhs.DATADataTypeState = rhs.DATADataTypeState;
    }
}