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
    internal class AmmunitionPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Keywords,Names,ObjectBounds,Sounds,Stats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Ammunition().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IAmmunition, IAmmunitionGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IAmmunition, IAmmunitionGetter>(workingContext.Record.FormKey).Last();

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
                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model)
                        || Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Icons,workingContext.Record.Icons)) Change = true;
                        
                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                overrideObject.Model = foundContext.Record.Model?.DeepCopy();
                            if (Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons))
                                overrideObject.Icons = foundContext.Record.Icons?.DeepCopy();
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
                            if (foundContext.Record.ObjectBounds != null && Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
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
                // Stats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Stats").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)
                        || Compare.NotEqual(foundContext.Record.Damage,originalObject.Record.Damage)
                        || Compare.NotEqual(foundContext.Record.Value,originalObject.Record.Value)
                        || Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.EditorID,workingContext.Record.EditorID)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Damage,workingContext.Record.Damage)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Value,workingContext.Record.Value)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.Weight,workingContext.Record.Weight)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID))
                                overrideObject.EditorID = foundContext.Record.EditorID;
                            if (Compare.NotEqual(foundContext.Record.Damage,originalObject.Record.Damage))
                                overrideObject.Damage = foundContext.Record.Damage;
                            if (Compare.NotEqual(foundContext.Record.Value,originalObject.Record.Value))
                                overrideObject.Value = foundContext.Record.Value;
                            if (Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                                overrideObject.Weight = foundContext.Record.Weight;
                            
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Text").Contains(context.ModKey)))
                {

                    if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description)
                        || Compare.NotEqual(foundContext.Record.ShortName,originalObject.Record.ShortName))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Description,workingContext.Record.Description)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.ShortName,workingContext.Record.ShortName)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                                overrideObject.Description = Utility.NewString(foundContext.Record.Description);
                            if (Compare.NotEqual(foundContext.Record.ShortName,originalObject.Record.ShortName))
                                overrideObject.ShortName = foundContext.Record.ShortName;
                        }
                        break;
                    }
                }
            
            }
        }
    }
}
