using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MadmanSleepTrainer
{
    public class SleepTrainerCore : CompBiosculpterPod
    {
        private SleepTrainerCore_Properties _properties;
        private List<HediffDef> _failureHediffs = new List<HediffDef>();
        private System.Random _random;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _properties = (SleepTrainerCore_Properties)this.props;
            _random = new System.Random();
            foreach (HediffToApplyWithWeight possibleFailureResult in _properties.hediffsOnFailure)
            {
                for (int i = 0; i < possibleFailureResult.weight; i++)
                {
                    _failureHediffs.Add(possibleFailureResult.hediff);
                }
            }
        }

        public Pawn getOccupantExcludingLoading()
        {
            if (this.CurrentCycle == null)
            {
                return null;
            }
            if (this.GetDirectlyHeldThings().Count != 1)
            {
                return null;
            }
            return this.GetDirectlyHeldThings()[0] as Pawn;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.GetComp<CompPowerTrader>() == null)
            {
                return;
            }
            if (!this.parent.GetComp<CompPowerTrader>().PowerOn && this.getOccupantExcludingLoading() != null)
            {
                this.Occupant.health.AddHediff(_failureHediffs[_random.Next(_failureHediffs.Count)]);
                this.EjectContents(true, false, this.parent.Map);
            }
        }

  

   

        public override void PostDraw()
        {

            if (getOccupantExcludingLoading() == null)
            {

                Mesh mesh = this._properties.graphicData.Graphic.MeshAt(this.parent.Rotation);
                Vector3 drawPos = this.parent.DrawPos;
                drawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
                Graphics.DrawMesh(mesh, drawPos + this._properties.graphicData.drawOffset.RotatedBy(this.parent.Rotation), Quaternion.identity, this._properties.graphicData.Graphic.MatAt(this.parent.Rotation, null), 0);
            }
        }
    }




    public class SleepTrainerCore_Properties : CompProperties_BiosculpterPod
    {
        public List<HediffToApplyWithWeight> hediffsOnFailure;
        public GraphicData graphicData;
        public SleepTrainerCore_Properties()
        {
    
            this.compClass = typeof(SleepTrainerCore);
        }
    }


}

public class HediffToApplyWithWeight
{
    public HediffDef hediff;
    public int weight;
}
