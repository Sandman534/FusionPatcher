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
    internal class ScrollPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Keywords,Names,ObjectBounds,Sounds,SpellStats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Scroll().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IScroll, IScrollGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IScroll, IScrollGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Destructible
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Destructible").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                overrideObject.Destructible = foundContext.Record.Destructible?.DeepCopy();
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Graphics").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.MenuDisplayObject,originalObject.Record.MenuDisplayObject))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.MenuDisplayObject,workingContext.Record.MenuDisplayObject)) Change = true;
                        
                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.MenuDisplayObject,originalObject.Record.MenuDisplayObject))
                                overrideObject.MenuDisplayObject.SetTo(foundContext.Record.MenuDisplayObject);
                        }
                        break;
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
                            addedRecord.Keywords = NewKeywords.OverrideObject;
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
                            if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("ObjectBounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Sounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound)
                        || Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.PickUpSound,workingContext.Record.PickUpSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.PutDownSound,workingContext.Record.PutDownSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound))
                                overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                            if (Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound)) 
                                overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // SpellStats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("SpellStats").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)
                        || Compare.NotEqual(foundContext.Record.BaseCost,originalObject.Record.BaseCost)
                        || Compare.NotEqual(foundContext.Record.Type,originalObject.Record.Type)
                        || Compare.NotEqual(foundContext.Record.Flags,originalObject.Record.Flags)
                        || Compare.NotEqual(foundContext.Record.CastDuration,originalObject.Record.CastDuration)
                        || Compare.NotEqual(foundContext.Record.CastType,originalObject.Record.CastType)
                        || Compare.NotEqual(foundContext.Record.ChargeTime,originalObject.Record.ChargeTime)
                        || Compare.NotEqual(foundContext.Record.HalfCostPerk,originalObject.Record.HalfCostPerk)
                        || Compare.NotEqual(foundContext.Record.Range,originalObject.Record.Range)
                        || Compare.NotEqual(foundContext.Record.TargetType,originalObject.Record.TargetType))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.BaseCost,originalObject.Record.BaseCost)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Type,originalObject.Record.Type)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Flags,originalObject.Record.Flags)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.CastDuration,originalObject.Record.CastDuration)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.CastType,originalObject.Record.CastType)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.ChargeTime,originalObject.Record.ChargeTime)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.HalfCostPerk,originalObject.Record.HalfCostPerk)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Range,originalObject.Record.Range)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.TargetType,originalObject.Record.TargetType)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID))
                                overrideObject.EditorID = foundContext.Record.EditorID;
                            if (Compare.NotEqual(foundContext.Record.BaseCost,originalObject.Record.BaseCost))
                                overrideObject.BaseCost = foundContext.Record.BaseCost;
                            if (Compare.NotEqual(foundContext.Record.Type,originalObject.Record.Type))
                                overrideObject.Type = foundContext.Record.Type;
                            if (Compare.NotEqual(foundContext.Record.Flags,originalObject.Record.Flags))
                                overrideObject.Flags = foundContext.Record.Flags;
                            if (Compare.NotEqual(foundContext.Record.CastDuration,originalObject.Record.CastDuration))
                                overrideObject.CastDuration = foundContext.Record.CastDuration;
                            if (Compare.NotEqual(foundContext.Record.CastType,originalObject.Record.CastType))
                                overrideObject.CastType = foundContext.Record.CastType;
                            if (Compare.NotEqual(foundContext.Record.ChargeTime,originalObject.Record.ChargeTime))
                                overrideObject.ChargeTime = foundContext.Record.ChargeTime;
                            if (Compare.NotEqual(foundContext.Record.HalfCostPerk,originalObject.Record.HalfCostPerk))
                                overrideObject.HalfCostPerk.SetTo(foundContext.Record.HalfCostPerk);
                            if (Compare.NotEqual(foundContext.Record.Range,originalObject.Record.Range))
                                overrideObject.Range = foundContext.Record.Range;
                            if (Compare.NotEqual(foundContext.Record.TargetType,originalObject.Record.TargetType))
                                overrideObject.TargetType = foundContext.Record.TargetType;   
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Text").Contains(context.ModKey)))
                {

                    if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Description,workingContext.Record.Description)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                                overrideObject.Description = Utility.NewString(foundContext.Record.Description);
                        }
                        break;
                    }
                }

            }
        }
    }
}
