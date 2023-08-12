using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MadmanSleepTrainer
{
    public class SkillLearnerCycle : CompBiosculpterPod_Cycle
    {
        private List<SkillDef> _skillDefs = new List<SkillDef>();
        private Dictionary<SkillDef, Texture2D> _skillIcons = new Dictionary<SkillDef, Texture2D>();
        private SkillDef _skillForCycle = SkillDefOf.Animals;
        private SkillLearnerCycle_Properties _properties;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _properties = (SkillLearnerCycle_Properties)this.props;
            _skillDefs.Add(SkillDefOf.Animals);
            _skillIcons.Add(SkillDefOf.Animals, Textures.TexturesByFilename.TryGetValue("MM_Animals"));

            _skillDefs.Add(SkillDefOf.Artistic);
            _skillIcons.Add(SkillDefOf.Artistic, Textures.TexturesByFilename.TryGetValue("MM_Artistic"));

            _skillDefs.Add(SkillDefOf.Construction);
            _skillIcons.Add(SkillDefOf.Construction, Textures.TexturesByFilename.TryGetValue("MM_Construction"));

            _skillDefs.Add(SkillDefOf.Cooking);
            _skillIcons.Add(SkillDefOf.Cooking, Textures.TexturesByFilename.TryGetValue("MM_Cooking"));

            _skillDefs.Add(SkillDefOf.Crafting);
            _skillIcons.Add(SkillDefOf.Crafting, Textures.TexturesByFilename.TryGetValue("MM_Crafting"));

            _skillDefs.Add(SkillDefOf.Intellectual);
            _skillIcons.Add(SkillDefOf.Intellectual, Textures.TexturesByFilename.TryGetValue("MM_Intellectual"));

            _skillDefs.Add(SkillDefOf.Medicine);
            _skillIcons.Add(SkillDefOf.Medicine, Textures.TexturesByFilename.TryGetValue("MM_Medicine"));

            _skillDefs.Add(SkillDefOf.Melee);
            _skillIcons.Add(SkillDefOf.Melee, Textures.TexturesByFilename.TryGetValue("MM_Melee"));

            _skillDefs.Add(SkillDefOf.Mining);
            _skillIcons.Add(SkillDefOf.Mining, Textures.TexturesByFilename.TryGetValue("MM_Mining"));

            _skillDefs.Add(SkillDefOf.Plants);
            _skillIcons.Add(SkillDefOf.Plants, Textures.TexturesByFilename.TryGetValue("MM_Plants"));

            _skillDefs.Add(SkillDefOf.Shooting);
            _skillIcons.Add(SkillDefOf.Shooting, Textures.TexturesByFilename.TryGetValue("MM_Crafting"));

            _skillDefs.Add(SkillDefOf.Social);
            _skillIcons.Add(SkillDefOf.Social, Textures.TexturesByFilename.TryGetValue("MM_Social"));
            SetCycleTime(_properties.cycleIntervalLength);


        }
        public override void CycleCompleted(Pawn pawn)
        {
            if (pawn.health == null)
            {
                return;
            }
            float facilityMult = 1.0f + (0.15f * this.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading.Count);
            pawn.skills.GetSkill(_skillForCycle).Learn(facilityMult * _properties.xpRewardPerDay * ((CompProperties_BiosculpterPod_BaseCycle)this.props).durationDays);
            pawn.skills.GetSkill(_skillForCycle).xpSinceMidnight = 0;
            return;
        }

        public void SetTargetSkill(SkillDef skill)
        {
            _skillForCycle = skill;
        }

        public void SetCycleTime(float ticks)
        {
            ((CompProperties_BiosculpterPod_BaseCycle)this.props).durationDays = ticks / 60000;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (this.parent.Faction == Faction.OfPlayer)
            {
                var chosenSkillDef = _skillForCycle;
                Command_Action chooseSkillButton = new Command_Action();
                chooseSkillButton.defaultLabel = "Cycle: " + chosenSkillDef.defName;
                chooseSkillButton.icon = _skillIcons.TryGetValue(_skillForCycle);
                chooseSkillButton.action = delegate ()
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (SkillDef skill in _skillDefs)
                    {
                        FloatMenuOption item = new FloatMenuOption(skill.defName, delegate ()
                        {
                            SetTargetSkill(skill);
                        }, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, skill), null, true, 0);
                        list.Add(item);
                        Find.WindowStack.Add(new FloatMenu(list));

                    }
                };
                Command_Action chooseLengthButton = new Command_Action();
                chooseLengthButton.defaultLabel = "Length: " + ((CompProperties_BiosculpterPod_BaseCycle)this.props).durationDays.ToString();
                chooseLengthButton.icon = Textures.TexturesByFilename.TryGetValue("MM_TimeSelect");
                chooseLengthButton.action = delegate ()
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    for (int i = 0; i < _properties.numDifferentCycleLengths; i++)
                    {
                        var duration = _properties.cycleIntervalLength * (i + 1);
                        list.Add(new FloatMenuOption(duration.ToStringTicksToDays(), delegate () { SetCycleTime((float)duration); }, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, this.parent.def), null, true, 0));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                };

                yield return chooseSkillButton;
                yield return chooseLengthButton;




            }
            yield break;
        }





    }

    public class SkillLearnerCycle_Properties : CompProperties_BiosculpterPod_BaseCycle
    {
        public int cycleIntervalLength;
        public int numDifferentCycleLengths;
        public float xpRewardPerDay;
        public SkillLearnerCycle_Properties()
        {
            this.compClass = typeof(SkillLearnerCycle);
        }
    }

}
