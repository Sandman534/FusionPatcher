using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Immutable;

namespace Fusion;

public class Keywords
{
    public ExtendedList<IFormLinkGetter<IKeywordGetter>> OverrideObject {get; set;}
    public bool Modified;

    public Keywords(IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? PatchRecord,IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec);
        else if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec);        
    }
    public void Add(IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? FoundContext, IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (OriginalObject != null && !OriginalObject.Contains(rec) && !OverrideObject.Contains(rec))
                {
                    OverrideObject.Add(rec);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? FoundContext, IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            if (OriginalObject != null)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Contains(rec) && OverrideObject.Contains(rec))
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

    public Regions(IReadOnlyList<IFormLinkGetter<IRegionGetter>>? PatchRecord,IReadOnlyList<IFormLinkGetter<IRegionGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec);
        else if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec);        
    }
    public void Add(IReadOnlyList<IFormLinkGetter<IRegionGetter>>? FoundContext, IReadOnlyList<IFormLinkGetter<IRegionGetter>>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (OriginalObject != null && !OriginalObject.Contains(rec) && !OverrideObject.Contains(rec))
                {
                    OverrideObject.Add(rec);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IRegionGetter>>? FoundContext, IReadOnlyList<IFormLinkGetter<IRegionGetter>>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            if (OriginalObject != null)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Contains(rec) && OverrideObject.Contains(rec))
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

    public Packages(IReadOnlyList<IFormLinkGetter<IPackageGetter>>? PatchRecord,IReadOnlyList<IFormLinkGetter<IPackageGetter>>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec);
        else if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec);        
    }
    public void Add(IReadOnlyList<IFormLinkGetter<IPackageGetter>>? FoundContext, IReadOnlyList<IFormLinkGetter<IPackageGetter>>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (OriginalObject != null && !OriginalObject.Contains(rec) && !OverrideObject.Contains(rec))
                {
                    OverrideObject.Add(rec);
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IFormLinkGetter<IPackageGetter>>? FoundContext, IReadOnlyList<IFormLinkGetter<IPackageGetter>>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            if (OriginalObject != null)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Contains(rec) && OverrideObject.Contains(rec))
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

    public Factions(IReadOnlyList<IRankPlacementGetter>? PatchRecord,IReadOnlyList<IRankPlacementGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec.DeepCopy());
        else if (WorkingRecord != null)
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
                    if (oFoundRec.First() != null && oFoundRec.First().Rank != rec.Rank)
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

    public Relations(IReadOnlyList<IRelationGetter>? PatchRecord,IReadOnlyList<IRelationGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec.DeepCopy());
        else if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());        
    }
    public void Add(IReadOnlyList<IRelationGetter>? FoundContext)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (!OverrideObject.Where(x => x.Target.FormKey == rec.Target.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IRelationGetter>? FoundContext, IReadOnlyList<IRelationGetter>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Where(x => x.Target.FormKey == rec.Target.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Target.FormKey == rec.Target.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IRelationGetter>? FoundContext, IReadOnlyList<IRelationGetter>? OriginalObject)
    {
        if (FoundContext != null && OriginalObject != null)
            foreach (var rec in FoundContext)
                if (!OriginalObject.Where(x => x.Target.FormKey == rec.Target.FormKey && x.Modifier == rec.Modifier).Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Target.FormKey == rec.Target.FormKey);
                    if (oFoundRec.First() != null && oFoundRec.First().Modifier != rec.Modifier)
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(rec.DeepCopy());
                        Modified = true;
                    }
                }
    }

}

public class Perks
{
    public ExtendedList<PerkPlacement> OverrideObject {get; set;}
    public bool Modified;

    public Perks(IReadOnlyList<IPerkPlacementGetter>? PatchRecord,IReadOnlyList<IPerkPlacementGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec.DeepCopy());
        else if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());        
    }
    public void Add(IReadOnlyList<IPerkPlacementGetter>? FoundContext)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (!OverrideObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IPerkPlacementGetter>? FoundContext, IReadOnlyList<IPerkPlacementGetter>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Where(x => x.Perk.FormKey == rec.Perk.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IPerkPlacementGetter>? FoundContext, IReadOnlyList<IPerkPlacementGetter>? OriginalObject)
    {
        if (FoundContext != null && OriginalObject != null)
            foreach (var rec in FoundContext)
                if (!OriginalObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey && x.Rank == rec.Rank).Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Perk.FormKey == rec.Perk.FormKey);
                    if (oFoundRec.First() != null && oFoundRec.First().Rank != rec.Rank)
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(rec.DeepCopy());
                        Modified = true;
                    }
                }
    }

}

public class Containers
{
    public ExtendedList<ContainerEntry> OverrideObject {get; set;}
    public bool Modified;

    public Containers(IReadOnlyList<IContainerEntryGetter>? PatchRecord,IReadOnlyList<IContainerEntryGetter>? WorkingRecord)
    {
        OverrideObject = new();
        Modified = false;

        if (PatchRecord != null)
            foreach (var rec in PatchRecord) 
                OverrideObject.Add(rec.DeepCopy());
        else if (WorkingRecord != null)
            foreach (var rec in WorkingRecord) 
                OverrideObject.Add(rec.DeepCopy());        
    }
    public void Add(IReadOnlyList<IContainerEntryGetter>? FoundContext)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            foreach (var rec in FoundContext)
                if (!OverrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                {
                    OverrideObject.Add(rec.DeepCopy());
                    Modified = true;
                }
    }
    
    public void Remove(IReadOnlyList<IContainerEntryGetter>? FoundContext, IReadOnlyList<IContainerEntryGetter>? OriginalObject)
    {
        if (FoundContext != null && FoundContext.Count > 0)
            // Remove Items
            if (OriginalObject != null && OriginalObject.Count > 0)
                foreach (var rec in OriginalObject)
                    if (!FoundContext.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey).Any())
                    {
                        var oFoundRec = OverrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                        if (oFoundRec.Any())
                        {
                            OverrideObject.Remove(oFoundRec.First());
                            Modified = true;
                        }
                    }
    }

    public void Change(IReadOnlyList<IContainerEntryGetter>? FoundContext, IReadOnlyList<IContainerEntryGetter>? OriginalObject)
    {
        if (FoundContext != null && OriginalObject != null)
            foreach (var rec in FoundContext)
                if (!OriginalObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey && x.Item.Count == rec.Item.Count).Any())
                {
                    var oFoundRec = OverrideObject.Where(x => x.Item.Item.FormKey == rec.Item.Item.FormKey);
                    if (oFoundRec.First() != null && oFoundRec.First().Item.Count != rec.Item.Count)
                    {
                        OverrideObject.Remove(oFoundRec.First());
                        OverrideObject.Add(rec.DeepCopy());
                        Modified = true;
                    }
                }
    }

}