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
    internal class ArmorPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Enchantments,Graphics,Keywords,Names,ObjectBounds,Sounds,Stats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Armor().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IArmor, IArmorGetter>(workingContext.Record.FormKey).Last();
                bool[] mapped = new bool[20];
                Keywords NewKeywords = new(workingContext.Record.Keywords);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (Settings.TagList("Destructible").Contains(foundContext.ModKey) && !mapped[0])
                    {
                        if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
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
                                mapped[0] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Enchantments
                    //==============================================================================================================
                    if (Settings.TagList("Enchantments").Contains(foundContext.ModKey) && !mapped[1])
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectEffect,originalObject.Record.ObjectEffect))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[1] = true;
                            else 
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectEffect,workingContext.Record.ObjectEffect)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ObjectEffect,originalObject.Record.ObjectEffect))
                                        overrideObject.ObjectEffect.SetTo(foundContext.Record.ObjectEffect);
                                }
                                mapped[1] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Settings.TagList("Graphics").Contains(foundContext.ModKey) && !mapped[2])
                    {
                        if (Compare.NotEqual(foundContext.Record.WorldModel,originalObject.Record.WorldModel)
                            || Compare.NotEqual(foundContext.Record.Armature,originalObject.Record.Armature))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[2] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.WorldModel,workingContext.Record.WorldModel)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Armature,workingContext.Record.Armature)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.WorldModel,originalObject.Record.WorldModel))
                                        overrideObject.WorldModel = Utility.NewGender<ArmorModel>(foundContext.Record.WorldModel?.Male?.DeepCopy(), foundContext.Record.WorldModel?.Female?.DeepCopy());
                                    if (Compare.NotEqual(foundContext.Record.Armature,originalObject.Record.Armature))
                                        overrideObject.Armature.SetTo(foundContext.Record.Armature);
                                }
                                mapped[2] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Settings.TagList("Names").Contains(foundContext.ModKey) && !mapped[3])
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[3] = true;
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
                                mapped[3] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (Settings.TagList("Sounds").Contains(foundContext.ModKey) && !mapped[4])
                    {
                        if (Compare.NotEqual(foundContext.Record.PickUpSound,originalObject.Record.PickUpSound)
                            || Compare.NotEqual(foundContext.Record.PutDownSound,originalObject.Record.PutDownSound))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[4] = true;
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
                                mapped[4] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (Settings.TagList("Stats").Contains(foundContext.ModKey) && !mapped[5])
                    {
                        if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)
                            || Compare.NotEqual(foundContext.Record.Value,originalObject.Record.Value)
                            || Compare.NotEqual(foundContext.Record.Weight,originalObject.Record.Weight)
                            || Compare.NotEqual(foundContext.Record.ArmorRating,originalObject.Record.ArmorRating)
                            || Compare.NotEqual(foundContext.Record.BashImpactDataSet,originalObject.Record.BashImpactDataSet)
                            || Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,originalObject.Record.AlternateBlockMaterial))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[5] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.EditorID,workingContext.Record.EditorID)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Value,workingContext.Record.Value)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Weight,workingContext.Record.Weight)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.ArmorRating,workingContext.Record.ArmorRating)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.BashImpactDataSet,workingContext.Record.BashImpactDataSet)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,workingContext.Record.AlternateBlockMaterial)) Change = true;

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
                                    if (Compare.NotEqual(foundContext.Record.ArmorRating,originalObject.Record.ArmorRating))
                                        overrideObject.ArmorRating = foundContext.Record.ArmorRating;
                                    if (Compare.NotEqual(foundContext.Record.BashImpactDataSet,originalObject.Record.BashImpactDataSet))
                                        overrideObject.BashImpactDataSet.SetTo(foundContext.Record.BashImpactDataSet);
                                    if (Compare.NotEqual(foundContext.Record.AlternateBlockMaterial,originalObject.Record.AlternateBlockMaterial))
                                        overrideObject.AlternateBlockMaterial.SetTo(foundContext.Record.AlternateBlockMaterial);
                                    
                                }
                                mapped[5] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Text
                    //==============================================================================================================
                    if (Settings.TagList("Text").Contains(foundContext.ModKey) && !mapped[6])
                    {
                        if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[6] = true;
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Description,workingContext.Record.Description)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Description,originalObject.Record.Description))
                                        overrideObject.Description = Utility.NewString(foundContext.Record.Description);
                                }
                                mapped[6] = true;
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
