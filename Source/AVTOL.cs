using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;


namespace AVTOL
{
    public class AVTOL:PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Pitch Neutral%"), 
        UI_FloatRange(minValue = 0f, maxValue = 100f, stepIncrement = 1f)]
        public float pitchNeutral = 100f;
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Pitch Range%"), 
        UI_FloatRange(minValue = -100f, maxValue = 100f, stepIncrement = 1f)]
        public float pitchRange = 0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Phase Angle"),
        UI_FloatRange(minValue = 0f, maxValue = 180f, stepIncrement = 5f)]
        public float phaseAngle = 90f;        
        
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Max Angle"), 
        UI_FloatRange(minValue = 0f, maxValue = 180f, stepIncrement = 5f)]
        public float maxAngle = 90f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Step Angle"),
        UI_FloatRange(minValue = -90f, maxValue = 90f, stepIncrement = 0.5f)]
        public float stepAngle = 22.5f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Vertical Cutoff"), 
        UI_FloatRange(minValue = 1f, maxValue = 100f, stepIncrement = 1f)]
        public float verticalspeed = 100f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "VTOL setting:"),
        UI_Toggle(disabledText = "Off", enabledText = "On")]
        public bool showMenu = true;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "VTOL system:"),
        UI_Toggle(disabledText = "Off", enabledText = "Engaged")]
        public bool isEngaged = false;

        [KSPAction("Toggle VTOL system")]
        public void toggleSystem(KSPActionParam param)
        {
            isEngaged = !isEngaged;
            print("VTOL Control System is " + (isEngaged ? "Engaged" : "Off"));
            if(!isEngaged)
            {
                SetThrustPercentage(100f);
            }
        }
        [KSPAction("Increase Phase Angle")]
        public void increasePhaseAngle(KSPActionParam param)
        {
            phaseAngle += stepAngle;
            if (phaseAngle > maxAngle)
                phaseAngle = maxAngle;
        }
        [KSPAction("Decrease Phase Angle")]
        public void DecreasePhaseAngle(KSPActionParam param)
        {
            phaseAngle -= stepAngle;
            if (phaseAngle < 0f)
                phaseAngle = 0f;
        }
        private ModuleEngines engine;
        private ModuleEnginesFX engineFX;
        public override void OnStart(StartState state)
        {
            if (state == StartState.Editor)
                return;
            showMenu = false;
            if (part.Modules.Contains("ModuleEngines"))
                engine = (ModuleEngines)part.Modules["ModuleEngines"];
            if (part.Modules.Contains("ModuleEnginesFX"))
                engineFX = (ModuleEnginesFX)part.Modules["ModuleEnginesFX"];
        }

        public void FixedUpdate()
        {
            this.Fields["pitchNeutral"].guiActive = showMenu;
            this.Fields["pitchNeutral"].guiActiveEditor = showMenu;
            this.Fields["pitchRange"].guiActive = showMenu;
            this.Fields["pitchRange"].guiActiveEditor = showMenu;
            this.Fields["phaseAngle"].guiActiveEditor = showMenu;
            this.Fields["phaseAngle"].guiActive = showMenu;
            this.Fields["maxAngle"].guiActive = showMenu;
            this.Fields["maxAngle"].guiActiveEditor = showMenu;
            this.Fields["stepAngle"].guiActive = showMenu;
            this.Fields["stepAngle"].guiActiveEditor = showMenu; 

            this.Fields["verticalspeed"].guiActive = showMenu;
            this.Fields["verticalspeed"].guiActiveEditor = showMenu;

            if (HighLogic.LoadedSceneIsEditor)
            {
                
                return;
            }
            if(HighLogic.LoadedSceneIsFlight && isEngaged)
            {

                float T = pitchNeutral;
                T *= (1f - (float)vessel.verticalSpeed / verticalspeed);
                T *= Mathf.Sin(phaseAngle / 57.259f);
                T = Mathf.Clamp(T, 0f, 100f);
                T += pitchRange * vessel.ctrlState.pitch;
                SetThrustPercentage(T);
                
            }
            
        }


        void SetThrustPercentage(float T)
        {

            T = Mathf.Clamp(T, 0f, 100f);
            if(engine!=null)
            {
                engine.thrustPercentage = T;
                return;
            }
            if (engineFX != null)
            {
                engineFX.thrustPercentage = T;
                return;
            }
        }
    }
}
