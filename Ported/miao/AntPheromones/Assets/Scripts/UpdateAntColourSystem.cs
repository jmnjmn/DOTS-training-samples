﻿using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class UpdateAntColourSystem : JobComponentSystem
    {
        private struct Job : IJobForEach<Brightness, ResourceCarrier, ColourDisplay>
        {
            public Color SearchColour;
            public Color CarryColour;
            
            public void Execute([ReadOnly] ref Brightness brightness, [ReadOnly] ref ResourceCarrier carrier, [WriteOnly] ref ColourDisplay colourDisplay)
            {
                Color colourToDisplay = carrier.IsCarrying ? this.SearchColour : this.CarryColour;
                colourDisplay.Value += (colourToDisplay * brightness.Value - colourDisplay.Value) * 0.05f;
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                SearchColour = new Color(48f, 54f, 90f), CarryColour = new Color(184f, 181f, 101f)
            }.Schedule(this, inputDeps);
        }
    }
}