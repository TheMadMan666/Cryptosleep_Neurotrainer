using RimWorld;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using Verse;

namespace MadmanSleepTrainer
{
    public class ApplierCycle : CompBiosculpterPod_Cycle
    {
        private List<SkillDef> _skillDefs = new List<SkillDef>();
        private Dictionary<SkillDef, Texture2D> _skillIcons = new Dictionary<SkillDef, Texture2D>();
        private SkillDef _skillForCycle = SkillDefOf.Animals;
        private ApplierCycle_Properties _properties;
        private SubCycle _currentSubcycle;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _properties = (ApplierCycle_Properties)this.props;
            _currentSubcycle = _properties.subCycles.First();
            SetCycleTime(_currentSubcycle.cycleLength);
        }
        public override void CycleCompleted(Pawn pawn)
        {
     
            if (pawn.health == null)
            {
                return;
            }
            if(_currentSubcycle.traitToApplyOnCycleComplete != null) {
                pawn.story.traits.GainTrait(new Trait(_currentSubcycle.traitToApplyOnCycleComplete));

            }
            if (_currentSubcycle.traitToApplyOnCycleComplete != null)
            {
                pawn.health.AddHediff(_currentSubcycle.hediffToApplyOnCycleComplete, pawn.health.hediffSet.GetBrain());
            }
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
                Command_Action chooseSubcycleButton = new Command_Action();
                chooseSubcycleButton.defaultLabel = "Cycle: " + _currentSubcycle.subCycleName;
                chooseSubcycleButton.icon = Textures.TexturesByFilename.TryGetValue(_currentSubcycle.iconFile);
                chooseSubcycleButton.action = delegate ()
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (SubCycle subCycle in _properties.subCycles)
                    {
                        FloatMenuOption item = new FloatMenuOption(subCycle.subCycleName, delegate ()
                        {
                            _currentSubcycle = subCycle;
                            SetCycleTime(subCycle.cycleLength);
                        }, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, this.parent.def), null, true, 0);
                        list.Add(item);
                        Find.WindowStack.Add(new FloatMenu(list));

                    }
                };

                yield return chooseSubcycleButton;
            }
            yield break;
        }

    }

    public class SubCycle
    {
        public int cycleLength;
        public string subCycleName;
        public string iconFile;
        public HediffDef hediffToApplyOnCycleComplete;
        public TraitDef traitToApplyOnCycleComplete;
    }


    public class ApplierCycle_Properties : CompProperties_BiosculpterPod_BaseCycle
    {
        public List<SubCycle> subCycles;
        public ApplierCycle_Properties()
        {
            this.compClass = typeof(ApplierCycle);
        }
    }

}
