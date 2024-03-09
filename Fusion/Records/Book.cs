using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;

namespace Fusion
{
    internal class BOOK
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            Console.WriteLine("Processing Book");
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Keywords,Names,ObjectBounds,Sounds,Stats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Book().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey).Last();
                MappedTags mapped = new MappedTags();
                Keywords NewKeywords = new(workingContext.Record.Keywords);
                
                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (mapped.NotMapped("Destructible") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                        overrideObject.Destructible = foundContext.Record.Destructible?.DeepCopy();
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (mapped.NotMapped("Graphics") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model)
                            || Compare.NotEqual(foundContext.Record.InventoryArt,originalObject.Record.InventoryArt)
                            || Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.InventoryArt,workingContext.Record.InventoryArt)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Icons,workingContext.Record.Icons)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                        overrideObject.Model = foundContext.Record.Model?.DeepCopy();
                                    if (Compare.NotEqual(foundContext.Record.InventoryArt,originalObject.Record.InventoryArt))
                                        overrideObject.InventoryArt.SetTo(foundContext.Record.InventoryArt);
                                    if (Compare.NotEqual(foundContext.Record.Icons,originalObject.Record.Icons))
                                        overrideObject.Icons = foundContext.Record.Icons?.DeepCopy();
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
                    // Object Bounds
                    //==============================================================================================================
                    if (mapped.NotMapped("ObjectBounds") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                        overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (mapped.NotMapped("Sounds") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound)
                            || Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
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
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (mapped.NotMapped("Stats") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)
                            || Compare.NotEqual(foundContext.Record.Value,originalObject.Record.Value)
                            || Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.EditorID,workingContext.Record.EditorID)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Value,workingContext.Record.Value)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Weight,workingContext.Record.Weight)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID))
                                        overrideObject.EditorID = foundContext.Record.EditorID;
                                    if (Compare.NotEqual(foundContext.Record.Value,originalObject.Record.Value))
                                        overrideObject.Value = foundContext.Record.Value;
                                    if (Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight))
                                        overrideObject.Weight = foundContext.Record.Weight;
                                    
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Text
                    //==============================================================================================================
                    if (mapped.NotMapped("Text") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {

                        if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description)
                            || Compare.NotEqual(foundContext.Record.BookText,originalObject.Record.BookText))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Description,workingContext.Record.Description)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.BookText,workingContext.Record.BookText)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                                        overrideObject.Description = Utility.NewString(foundContext.Record.Description);
                                    if (Compare.NotEqual(foundContext.Record.BookText,originalObject.Record.BookText))
                                        overrideObject.BookText = Utility.NewStringNotNull(foundContext.Record.BookText);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }
                
                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                                NewKeywords.Add(foundContext.Record.Keywords, originalObject.Record.Keywords);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList("Keywords").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Keywords,originalObject.Record.Keywords))
                            NewKeywords.Remove(foundContext.Record.Keywords, originalObject.Record.Keywords);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewKeywords.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }
                
            }
        }
    }
}