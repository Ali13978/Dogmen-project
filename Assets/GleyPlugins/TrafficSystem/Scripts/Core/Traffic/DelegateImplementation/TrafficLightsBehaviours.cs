using System.Collections.Generic;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class TrafficLightsBehaviours
    {
        public static void DefaultBehaviour(TrafficLightsColor currentRoadColor, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects, string name)
        {
            switch (currentRoadColor)
            {
                case TrafficLightsColor.Red:
                    SetLight(true, redLightObjects, name);
                    SetLight(false, yellowLightObjects, name);
                    SetLight(false, greenLightObjects, name);
                    break;
                case TrafficLightsColor.Yellow:
                    SetLight(false, redLightObjects, name);
                    SetLight(true, yellowLightObjects, name);
                    SetLight(true, greenLightObjects, name);
                    break;
                case TrafficLightsColor.Green:
                    SetLight(false, redLightObjects, name);
                    SetLight(false, yellowLightObjects, name);
                    SetLight(true, greenLightObjects, name);
                    break;
            }
        }

        /// <summary>
        /// Set traffic lights color
        /// </summary>
        private static void SetLight(bool active, List<GameObject> lightObjects, string name)
        {
            for (int j = 0; j < lightObjects.Count; j++)
            {
                if (lightObjects[j] != null)
                {
                    if (lightObjects[j].activeSelf != active)
                    {
                        lightObjects[j].SetActive(active);
                    }
                }
                else
                {
                    Debug.LogWarning("Intersection " + name + " has null red light objects");
                }
            }
        }
    }
}
