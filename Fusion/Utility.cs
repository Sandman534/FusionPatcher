using Mutagen.Bethesda;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Immutable;
using System.Drawing;
namespace Fusion;

public static class Compare
{
    public static bool NotEqual(IModelGetter? Record1, IModelGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Filename Test
        if (Record1?.File.RawPath != Record2?.File.RawPath)
            return true;    

        // Alternate Texture Test
        if (Record1?.AlternateTextures != null && Record2?.AlternateTextures != null)
        {
            bool bTexture = false;
            if (Record1.AlternateTextures.Count == Record1.AlternateTextures.Count)
                foreach (var tex in Record1.AlternateTextures)
                    if (!Record2.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                        bTexture = true;

            return bTexture;
        }

        return false;
    }

    public static bool NotEqual(IIconsGetter? Record1, IIconsGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.LargeIconFilename.RawPath != Record2?.LargeIconFilename.RawPath)
            return true;
        if (Record1?.SmallIconFilename?.RawPath != Record2?.SmallIconFilename?.RawPath)
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

    public static bool NotEqual(ICellLightingGetter? Record1, ICellLightingGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        // Colors
        if (Record1 != null && Record2 != null)
            if (Record1.AmbientColor.ToArgb() != Record2.AmbientColor.ToArgb()
                || Record1.DirectionalColor.ToArgb() != Record2.DirectionalColor.ToArgb()
                || Record1.FogNearColor.ToArgb() != Record2.FogNearColor.ToArgb()
                || Record1.FogFarColor.ToArgb() != Record2.FogFarColor.ToArgb())
                return true;

        // Ambient Colors
        if (Record1 != null && Record2 != null)
        {
            if (Record1.AmbientColors.Specular.ToArgb() != Record2.AmbientColors.Specular.ToArgb())
                return true;
            if (Record1.AmbientColors.Scale != Record2.AmbientColors.Scale)
                return true;
            if (Record1.AmbientColors.DirectionalXPlus.ToArgb() != Record2.AmbientColors.DirectionalXPlus.ToArgb()
                || Record1.AmbientColors.DirectionalXMinus.ToArgb() != Record2.AmbientColors.DirectionalXMinus.ToArgb()
                || Record1.AmbientColors.DirectionalYPlus.ToArgb() != Record2.AmbientColors.DirectionalYPlus.ToArgb()
                || Record1.AmbientColors.DirectionalYMinus.ToArgb() != Record2.AmbientColors.DirectionalYMinus.ToArgb()
                || Record1.AmbientColors.DirectionalZPlus.ToArgb() != Record2.AmbientColors.DirectionalZPlus.ToArgb()
                || Record1.AmbientColors.DirectionalZMinus.ToArgb() != Record2.AmbientColors.DirectionalZMinus.ToArgb())
                return true;
        }

        // Values
        if (Record1 != null && Record2 != null)
        {
            if (Record1.FogNear != Record2.FogNear || Record1.FogFar != Record2.FogFar || Record1.FogClipDistance != Record2.FogClipDistance || Record1.FogPower != Record2.FogPower || Record1.FogMax != Record2.FogMax)
                return true;
            if (Record1.DirectionalRotationXY != Record2.DirectionalRotationXY || Record1.DirectionalRotationZ != Record2.DirectionalRotationZ || Record1.DirectionalFade != Record2.DirectionalFade)
                return true;
            if (Record1.LightFadeBegin != Record2.LightFadeBegin || Record1.LightFadeEnd != Record2.LightFadeEnd)
                return true;
        }

        // Inherits
        return NotEqual(Record1?.Inherits, Record2?.Inherits);
    }

    public static bool NotEqual(IArmorModelGetter? Record1, IArmorModelGetter? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1 != null && Record2 != null)
        {
            if (Record1.Model?.File.RawPath != Record2.Model?.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Record1.Model?.AlternateTextures != null && Record2.Model?.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Record1.Model.AlternateTextures.Count == Record2.Model.AlternateTextures.Count)
                    foreach (var tex in Record1.Model.AlternateTextures)
                        if (!Record2.Model.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

        return false;
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

        if (Record1?.Male != null && Record2?.Male != null)
        {
            if (Record1.Male.Model?.File.RawPath != Record2.Male.Model?.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Record1.Male.Model?.AlternateTextures != null && Record2.Male.Model?.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Record1.Male.Model.AlternateTextures.Count == Record2.Male.Model.AlternateTextures.Count)
                    foreach (var tex in Record1.Male.Model.AlternateTextures)
                        if (!Record2.Male.Model.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

        if (Record1?.Female != null && Record2?.Female != null)
        {
            if (Record1.Female.Model?.File.RawPath != Record2.Female.Model?.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Record1.Female.Model?.AlternateTextures != null && Record2.Female.Model?.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Record1.Female.Model.AlternateTextures.Count == Record2.Female.Model.AlternateTextures.Count)
                    foreach (var tex in Record1.Female.Model.AlternateTextures)
                        if (!Record2.Female.Model.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

        return false;
    }

    public static bool NotEqual(IGenderedItemGetter<IModelGetter?>? Record1, IGenderedItemGetter<IModelGetter?>? Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1?.Male != null && Record2?.Male != null)
        {
            if (Record1.Male.File.RawPath != Record2.Male.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Record1.Male.AlternateTextures != null && Record2.Male.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Record1.Male.AlternateTextures.Count == Record2.Male.AlternateTextures.Count)
                    foreach (var tex in Record1.Male.AlternateTextures)
                        if (!Record2.Male.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

        if (Record1?.Female != null && Record2?.Female != null)
        {
            if (Record1.Female.File.RawPath != Record2.Female.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Record1.Female.AlternateTextures != null && Record2.Female.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Record1.Female.AlternateTextures.Count == Record2.Female.AlternateTextures.Count)
                    foreach (var tex in Record1.Female.AlternateTextures)
                        if (!Record2.Female.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

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

    public static bool NotEqual(IFormLinkNullableGetter<ISkyrimMajorRecordGetter> Record1, IFormLinkNullableGetter<ISkyrimMajorRecordGetter> Record2)
    {
        // Null Test
        if (NullTest(Record1,Record2, out bool Result)) return Result;

        if (Record1.FormKey != Record2.FormKey)
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
        else if ((Obj1 != null && Obj1 == null) || (Obj1 == null && Obj2 != null))
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