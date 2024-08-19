using EFT.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZonePlacementTool.Helpers
{
    internal class Utils
    {
        public static string GetClosestExfilName(List<ExfiltrationPoint> exfils)
        {
            if (exfils.Count == 0) return "no exfils found";

            ExfiltrationPoint selectedPoint = exfils[0];
            foreach (var exfil in exfils)
            {
                if (Vector3.Distance(selectedPoint.transform.position, Plugin.Player.Transform.position) > Vector3.Distance(exfil.transform.position, Plugin.Player.Transform.position))
                {
                    selectedPoint = exfil;
                }
            }

            return selectedPoint.Settings.Name;
        }
    }
}
