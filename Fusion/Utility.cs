using Mutagen.Bethesda;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Immutable;

namespace Fusion;

public static class Compare
{
    public static bool NotEqual(IModelGetter? Model1, IModelGetter? Model2)
    {
        // Null Test
        if (NullTest(Model1,Model2, out bool Result)) return Result;

        // Filename Test
        if (Model1?.File.RawPath != Model2?.File.RawPath)
            return true;    

        // Alternate Texture Test
        if (Model1?.AlternateTextures != null && Model2?.AlternateTextures != null)
        {
            bool bTexture = false;
            if (Model1.AlternateTextures.Count == Model1.AlternateTextures.Count)
                foreach (var tex in Model1.AlternateTextures)
                    if (!Model2.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                        bTexture = true;

            return bTexture;
        }

        return false;
    }

    public static bool NotEqual(IIconsGetter? Icon1, IIconsGetter? Icon2)
    {
        // Null Test
        if (NullTest(Icon1,Icon2, out bool Result)) return Result;

        if (Icon1?.LargeIconFilename.RawPath != Icon2?.LargeIconFilename.RawPath)
            return true;
        if (Icon1?.SmallIconFilename?.RawPath != Icon2?.SmallIconFilename?.RawPath)
            return true;

        return false;
    }

    public static bool NotEqual(IDestructibleGetter? Dest1, IDestructibleGetter? Dest2)
    {
        // Null Test
        if (NullTest(Dest1,Dest2, out bool Result)) return Result;

        if (Dest1?.Data?.Health != Dest2?.Data?.Health)
            return true;
        if (Dest1?.Data?.DESTCount != Dest2?.Data?.DESTCount)
            return true;
        if (Dest1?.Data?.VATSTargetable != Dest2?.Data?.VATSTargetable)
            return true;

        return false;
    }
    
    public static bool NotEqual(ITranslatedStringGetter? Name1, ITranslatedStringGetter? Name2)
    {
        // Null Test
        if (NullTest(Name1,Name2, out bool Result)) return Result;

        if (Name1?.String != Name2?.String)
            return true;

        return false;
    }

    public static bool NotEqual(IObjectBoundsGetter? Obj1, IObjectBoundsGetter? Obj2)
    {
        // Null Test
        if (NullTest(Obj1,Obj2, out bool Result)) return Result;

        if (Obj1?.First.X != Obj2?.First.X
            || Obj1?.First.Y != Obj2?.First.Y
            || Obj1?.First.Z != Obj2?.First.Z)
            return true;

        if (Obj1?.Second.X != Obj2?.Second.X
            || Obj1?.Second.Y != Obj2?.Second.Y
            || Obj1?.Second.Z != Obj2?.Second.Z)
            return true;

        return false;
    }

    public static bool NotEqual(IAIDataGetter? AI1, IAIDataGetter? AI2)
    {
        // Null Test
        if (NullTest(AI1,AI2, out bool Result)) return Result;

        if (AI1?.Aggression != AI2?.Aggression
            || AI1?.Confidence != AI2?.Confidence
            || AI1?.EnergyLevel != AI2?.EnergyLevel
            || AI1?.Responsibility != AI2?.Responsibility
            || AI1?.Mood != AI2?.Mood
            || AI1?.Assistance != AI2?.Assistance)
            return true;

        if (AI1?.AggroRadiusBehavior != AI2?.AggroRadiusBehavior
            || AI1?.Warn != AI2?.Warn
            || AI1?.WarnOrAttack != AI2?.WarnOrAttack
            || AI1?.Attack != AI2?.Attack)
            return true;

        return false;
    }

    public static bool NotEqual(INpcConfigurationGetter? Con1, INpcConfigurationGetter? Con2)
    {
        // Null Test
        if (NullTest(Con1,Con2, out bool Result)) return Result;

        if (Con1?.MagickaOffset != Con2?.MagickaOffset
            || Con1?.StaminaOffset != Con2?.StaminaOffset
            || Con1?.Level != Con2?.Level
            || Con1?.CalcMinLevel != Con2?.CalcMinLevel
            || Con1?.CalcMaxLevel != Con2?.CalcMaxLevel
            || Con1?.SpeedMultiplier != Con2?.SpeedMultiplier
            || Con1?.DispositionBase != Con2?.DispositionBase
            || Con1?.TemplateFlags != Con2?.TemplateFlags
            || Con1?.HealthOffset != Con2?.HealthOffset
            || Con1?.BleedoutOverride != Con2?.BleedoutOverride)
            return true;

        foreach (var flag in Enums<NpcConfiguration.Flag>.Values)
        {
            if(Con1 != null && Con2 != null && Con1.Flags.HasFlag(flag) && !Con2.Flags.HasFlag(flag)) return true;
            if(Con1 != null && Con2 != null && !Con1.Flags.HasFlag(flag) && Con2.Flags.HasFlag(flag)) return true;
        }

        return false;
    }

    public static bool NotEqual(ICellLightingGetter? Light1, ICellLightingGetter? Light2)
    {
        // Null Test
        if (NullTest(Light1,Light2, out bool Result)) return Result;

        // Colors
        if (Light1 != null && Light2 != null)
        {
            if (Light1.AmbientColor.R != Light2.AmbientColor.R || Light1.AmbientColor.G != Light2.AmbientColor.G || Light1.AmbientColor.B != Light2.AmbientColor.B)
                return true;
            if (Light1.DirectionalColor.R != Light2.DirectionalColor.R || Light1.DirectionalColor.G != Light2.DirectionalColor.G || Light1.DirectionalColor.B != Light2.DirectionalColor.B)
                return true;
            if (Light1.FogNearColor.R != Light2.FogNearColor.R || Light1.FogNearColor.G != Light2.FogNearColor.G || Light1.FogNearColor.B != Light2.FogNearColor.B)
                return true;
            if (Light1.FogFarColor.R != Light2.FogFarColor.R || Light1.FogFarColor.G != Light2.FogFarColor.G || Light1.FogFarColor.B != Light2.FogFarColor.B)
                return true;
        }

        // Ambient Colors
        if (Light1 != null && Light2 != null)
        {
            if (Light1.AmbientColors.Specular.R != Light2.AmbientColors.Specular.R || Light1.AmbientColors.Specular.G != Light2.AmbientColors.Specular.G || Light1.AmbientColors.Specular.B != Light2.AmbientColors.Specular.B)
                return true;
            if (Light1.AmbientColors.Scale != Light2.AmbientColors.Scale)
                return true;
            if (Light1.AmbientColors.DirectionalXPlus.R != Light2.AmbientColors.DirectionalXPlus.R || Light1.AmbientColors.DirectionalXPlus.G != Light2.AmbientColors.DirectionalXPlus.G || Light1.AmbientColors.DirectionalXPlus.B != Light2.AmbientColors.DirectionalXPlus.B)
                return true;
            if (Light1.AmbientColors.DirectionalXMinus.R != Light2.AmbientColors.DirectionalXMinus.R || Light1.AmbientColors.DirectionalXMinus.G != Light2.AmbientColors.DirectionalXMinus.G || Light1.AmbientColors.DirectionalXMinus.B != Light2.AmbientColors.DirectionalXMinus.B)
                return true;
            if (Light1.AmbientColors.DirectionalYPlus.R != Light2.AmbientColors.DirectionalYPlus.R || Light1.AmbientColors.DirectionalYPlus.G != Light2.AmbientColors.DirectionalYPlus.G || Light1.AmbientColors.DirectionalYPlus.B != Light2.AmbientColors.DirectionalYPlus.B)
                return true;
            if (Light1.AmbientColors.DirectionalYMinus.R != Light2.AmbientColors.DirectionalYMinus.R || Light1.AmbientColors.DirectionalYMinus.G != Light2.AmbientColors.DirectionalYMinus.G || Light1.AmbientColors.DirectionalYMinus.B != Light2.AmbientColors.DirectionalYMinus.B)
                return true;
            if (Light1.AmbientColors.DirectionalZPlus.R != Light2.AmbientColors.DirectionalZPlus.R || Light1.AmbientColors.DirectionalZPlus.G != Light2.AmbientColors.DirectionalZPlus.G || Light1.AmbientColors.DirectionalZPlus.B != Light2.AmbientColors.DirectionalZPlus.B)
                return true;
            if (Light1.AmbientColors.DirectionalZMinus.R != Light2.AmbientColors.DirectionalZMinus.R || Light1.AmbientColors.DirectionalZMinus.G != Light2.AmbientColors.DirectionalZMinus.G || Light1.AmbientColors.DirectionalZMinus.B != Light2.AmbientColors.DirectionalZMinus.B)
                return true;
        }

        // Values
        if (Light1 != null && Light2 != null)
        {
            if (Light1.FogNear != Light2.FogNear || Light1.FogFar != Light2.FogFar || Light1.FogClipDistance != Light2.FogClipDistance || Light1.FogPower != Light2.FogPower || Light1.FogMax != Light2.FogMax)
                return true;
            if (Light1.DirectionalRotationXY != Light2.DirectionalRotationXY || Light1.DirectionalRotationZ != Light2.DirectionalRotationZ || Light1.DirectionalFade != Light2.DirectionalFade)
                return true;
            if (Light1.LightFadeBegin != Light2.LightFadeBegin || Light1.LightFadeEnd != Light2.LightFadeEnd)
                return true;
        }

        // Inherits
        if (Light1 != null && Light2 != null)
            foreach (var flag in Enums<CellLighting.Inherit>.Values)
            {
                if(Light1.Inherits.HasFlag(flag) && !Light2.Inherits.HasFlag(flag)) return true;
                if(!Light1.Inherits.HasFlag(flag) && Light2.Inherits.HasFlag(flag)) return true;
            }

        return false;
    }

    public static bool NotEqual(Cell.MajorFlag Flag1, Cell.MajorFlag Flag2)
    {
        // Null Test
        if (NullTest(Flag1,Flag2, out bool Result)) return Result;

        foreach (var flag in Enums<Cell.MajorFlag>.Values)
        {
            if(Flag1.HasFlag(flag) && !Flag2.HasFlag(flag)) return true;
            if(!Flag1.HasFlag(flag) && Flag2.HasFlag(flag)) return true;
        }

        return false;
    }

    public static bool NotEqual(Cell.Flag Flag1, Cell.Flag Flag2)
    {
        // Null Test
        if (NullTest(Flag1,Flag2, out bool Result)) return Result;

        foreach (var flag in Enums<Cell.Flag>.Values)
        {
            if(Flag1.HasFlag(flag) && !Flag2.HasFlag(flag)) return true;
            if(!Flag1.HasFlag(flag) && Flag2.HasFlag(flag)) return true;
        }

        return false;
    }

    public static bool NotEqual(IReadOnlyList<IContainerEntryGetter>? List1, IReadOnlyList<IContainerEntryGetter>? List2)
    {
        // Null Test
        if (NullTest(List1,List2, out bool Result)) return Result;

        if (List1 != null && List2 != null)
        {
            foreach(var item in List1)
                if (!List2.Where(x => x.Item.Item.FormKey == item.Item.Item.FormKey && x.Item.Count == item.Item.Count).Any())
                    return true;

            foreach(var item in List2)
                if (!List1.Where(x => x.Item.Item.FormKey == item.Item.Item.FormKey && x.Item.Count == item.Item.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<IPerkPlacementGetter>? List1, IReadOnlyList<IPerkPlacementGetter>? List2)
    {
        // Null Test
        if (NullTest(List1,List2, out bool Result)) return Result;

        if (List1 != null && List2 != null)
        {
            foreach(var item in List1)
                if (!List2.Where(x => x.Perk.FormKey == item.Perk.FormKey && x.Rank == item.Rank).Any())
                    return true;

            foreach(var item in List2)
                if (!List1.Where(x => x.Perk.FormKey == item.Perk.FormKey && x.Rank == item.Rank).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<ILeveledItemEntryGetter>? List1, IReadOnlyList<ILeveledItemEntryGetter>? List2)
    {
        // Null Test
        if (NullTest(List1,List2, out bool Result)) return Result;

        if (List1 != null && List2 != null)
        {
            foreach(var item in List1)
                if (!List2.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;

            foreach(var item in List2)
                if (!List1.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<ILeveledNpcEntryGetter>? List1, IReadOnlyList<ILeveledNpcEntryGetter>? List2)
    {
        // Null Test
        if (NullTest(List1,List2, out bool Result)) return Result;

        if (List1 != null && List2 != null)
        {
            foreach(var item in List1)
                if (!List2.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;

            foreach(var item in List2)
                if (!List1.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<ILeveledSpellEntryGetter>? List1, IReadOnlyList<ILeveledSpellEntryGetter>? List2)
    {
        // Null Test
        if (NullTest(List1,List2, out bool Result)) return Result;

        if (List1 != null && List2 != null)
        {
            foreach(var item in List1)
                if (!List2.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;

            foreach(var item in List2)
                if (!List1.Where(x => x.Data?.Reference.FormKey == item.Data?.Reference.FormKey && x.Data?.Count == item.Data?.Count).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IReadOnlyList<IFormLinkGetter<ISkyrimMajorRecordGetter>>? List1, IReadOnlyList<IFormLinkGetter<ISkyrimMajorRecordGetter>>? List2)
    {
        // Null Test
        if (NullTest(List1,List2, out bool Result)) return Result;

        if (List1 != null && List2 != null)
        {
            foreach(var item in List1)
                if (!List2.Where(x => x.FormKey == item.FormKey).Any())
                    return true;

            foreach(var item in List2)
                if (!List1.Where(x => x.FormKey == item.FormKey).Any())
                    return true;
        }
        return false;
    }

    public static bool NotEqual(IFormLinkNullableGetter<ISkyrimMajorRecordGetter> Link1, IFormLinkNullableGetter<ISkyrimMajorRecordGetter> Link2)
    {
        // Null Test
        if (NullTest(Link1,Link2, out bool Result)) return Result;

        if (Link1.FormKey != Link2.FormKey)
            return true;

        return false;
    }

    public static bool NotEqual(IFormLinkGetter<ISkyrimMajorRecordGetter>? Link1, IFormLinkGetter<ISkyrimMajorRecordGetter>? Link2)
    {
        // Null Test
        if (NullTest(Link1,Link2, out bool Result)) return Result;

        if (Link1 != null && Link2 != null && Link1.FormKey != Link2.FormKey)
            return true;

        return false;
    }

    public static bool NotEqual(IGenderedItemGetter<IArmorModelGetter?>? Link1, IGenderedItemGetter<IArmorModelGetter?>? Link2)
    {
        // Null Test
        if (NullTest(Link1,Link2, out bool Result)) return Result;

        if (Link1?.Male != null && Link2?.Male != null)
        {
            if (Link1.Male.Model?.File.RawPath != Link2.Male.Model?.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Link1.Male.Model?.AlternateTextures != null && Link2.Male.Model?.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Link1.Male.Model.AlternateTextures.Count == Link2.Male.Model.AlternateTextures.Count)
                    foreach (var tex in Link1.Male.Model.AlternateTextures)
                        if (!Link2.Male.Model.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

        if (Link1?.Female != null && Link2?.Female != null)
        {
            if (Link1.Female.Model?.File.RawPath != Link2.Female.Model?.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Link1.Female.Model?.AlternateTextures != null && Link2.Female.Model?.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Link1.Female.Model.AlternateTextures.Count == Link2.Female.Model.AlternateTextures.Count)
                    foreach (var tex in Link1.Female.Model.AlternateTextures)
                        if (!Link2.Female.Model.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

        return false;
    }

    public static bool NotEqual(IArmorModelGetter? Link1, IArmorModelGetter? Link2)
    {
        if (Link1 != null && Link2 != null)
        {
            if (Link1.Model?.File.RawPath != Link2.Model?.File.RawPath)
                return true;

            // Alternate Texture Test
            if (Link1.Model?.AlternateTextures != null && Link2.Model?.AlternateTextures != null)
            {
                bool bTexture = false;
                if (Link1.Model.AlternateTextures.Count == Link2.Model.AlternateTextures.Count)
                    foreach (var tex in Link1.Model.AlternateTextures)
                        if (!Link2.Model.AlternateTextures.Where(x => x.NewTexture == tex.NewTexture).Any())
                            bTexture = true;

                return bTexture;
            }            
        }

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

    public static bool NullTest(object? O1, object? O2, out bool value)
    {
        // Null Test
        if (O1 == null && O2 == null)
        {
            value = false;
            return true;
        }
        else if ((O1 != null && O2 == null) || (O1 == null && O2 != null))
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
                    if (oFoundRec.Any() && oFoundRec.First().Rank != rec.Rank)
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
                    if (oFoundRec.Any() && oFoundRec.First().Modifier != rec.Modifier)
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
                    if (oFoundRec.Any() && oFoundRec.First().Rank != rec.Rank)
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
                    if (oFoundRec.Any() && oFoundRec.First().Item.Count != rec.Item.Count)
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(oFoundOrg.First().DeepCopy());
                        Modified = true;
                    }
                }
            }
    }
}