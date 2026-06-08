using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace Fusion
{
    internal class RACE
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.R_Body_F, Tags.R_Body_M, Tags.R_Body_Size_F, Tags.R_Body_Size_M, Tags.R_ChangeSpells,
                Tags.R_Description, Tags.R_Ears, Tags.R_Eyes, Tags.R_Hair, Tags.R_Head, Tags.R_Mouth, Tags.R_Relations_Add, Tags.R_Relations_Change, 
                Tags.R_Relations_Remove, Tags.R_Skills, Tags.R_Teeth, Tags.R_Voice_F, Tags.R_Voice_M, Tags.Keywords);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IRaceGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "NPC");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IRace, IRaceGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IRace? overrideObject = null;
                MappedTags mapped = new();
                Keywords NewKeywords = new(wContext.Record.Keywords);
                Flags<Npc.SkyrimMajorRecordFlag> NewFlags = new(wContext.Record.SkyrimMajorRecordFlags);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Female Body
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_Body_F, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.BodyData.Female,oContext.Record.BodyData.Female)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.BodyData.Female,wContext.Record.BodyData.Female,oContext.Record.BodyData.Female)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BodyData.Female?.DeepCopyIn(fContext.Record.BodyData.Female);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Male Body
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_Body_M, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.BodyData.Male,oContext.Record.BodyData.Male)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.BodyData.Male,wContext.Record.BodyData.Male,oContext.Record.BodyData.Male)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BodyData.Male?.DeepCopyIn(fContext.Record.BodyData.Male);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Change Spells
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_ChangeSpells, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ActorEffect,oContext.Record.ActorEffect)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.ActorEffect,wContext.Record.ActorEffect,oContext.Record.ActorEffect)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    var existing = new HashSet<FormKey>((overrideObject.ActorEffect ?? []).Select(x => x.FormKey));

                                    foreach (var effect in fContext.Record.ActorEffect!) {
                                        if (existing.Add(effect.FormKey))
                                            overrideObject.ActorEffect?.Add(effect);
                                    }
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Description
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_Description, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Description,oContext.Record.Description)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Description,wContext.Record.Description,oContext.Record.Description)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Description = Utility.NewStringNotNull(fContext.Record.Description);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Skills
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_Ears, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.SkillBoost0,oContext.Record.SkillBoost0)
                            || Compare.NotEqual(fContext.Record.SkillBoost1,oContext.Record.SkillBoost1)
                            || Compare.NotEqual(fContext.Record.SkillBoost2,oContext.Record.SkillBoost2)
                            || Compare.NotEqual(fContext.Record.SkillBoost3,oContext.Record.SkillBoost3)
                            || Compare.NotEqual(fContext.Record.SkillBoost4,oContext.Record.SkillBoost4)
                            || Compare.NotEqual(fContext.Record.SkillBoost5,oContext.Record.SkillBoost5)
                            || Compare.NotEqual(fContext.Record.SkillBoost6,oContext.Record.SkillBoost6)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost0,wContext.Record.SkillBoost0,oContext.Record.SkillBoost0)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost0?.DeepCopyIn(fContext.Record.SkillBoost0);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost1,wContext.Record.SkillBoost1,oContext.Record.SkillBoost1)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost1?.DeepCopyIn(fContext.Record.SkillBoost1);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost2,wContext.Record.SkillBoost2,oContext.Record.SkillBoost2)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost2?.DeepCopyIn(fContext.Record.SkillBoost2);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost3,wContext.Record.SkillBoost3,oContext.Record.SkillBoost3)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost3?.DeepCopyIn(fContext.Record.SkillBoost3);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost4,wContext.Record.SkillBoost4,oContext.Record.SkillBoost4)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost4?.DeepCopyIn(fContext.Record.SkillBoost4);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost5,wContext.Record.SkillBoost5,oContext.Record.SkillBoost5)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost5?.DeepCopyIn(fContext.Record.SkillBoost5);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkillBoost6,wContext.Record.SkillBoost6,oContext.Record.SkillBoost6)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkillBoost6?.DeepCopyIn(fContext.Record.SkillBoost6);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Female Voice
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_Voice_F, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Voices.Female,oContext.Record.Voices.Female)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.Voices.Female,wContext.Record.Voices.Female,oContext.Record.Voices.Female)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Voices.Female = fContext.Record.Voices.Female;
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Male Voice
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.R_Voice_M, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Voices.Male,oContext.Record.Voices.Male)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.Voices.Male,wContext.Record.Voices.Male,oContext.Record.Voices.Male)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Voices.Male = fContext.Record.Voices.Male;
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Keyword Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                            NewKeywords.Add(fContext.Record.Keywords, oContext.Record.Keywords);
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Keyword Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.Keywords).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Keywords,oContext.Record.Keywords))
                            NewKeywords.Remove(fContext.Record.Keywords, oContext.Record.Keywords);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewKeywords.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Keywords = NewKeywords.OverrideObject;
                }                
            }
        }
    }
}
