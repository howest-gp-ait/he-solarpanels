using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIT.SolarPanels.Server.Core.Entities
{
    public class SolarPanel
    {
        public string PanelId { get; set; }
        public int SurfaceInSquareCentimeters { get; set; }
        public int MaxPowerPerSquareMeter { get; set; }

        public SolarPanel(string panelId, int surfaceInSquareCentimeters, int maxPowerPerSquareMeter)
        {
            PanelId = panelId;
            SurfaceInSquareCentimeters = surfaceInSquareCentimeters;
            MaxPowerPerSquareMeter = maxPowerPerSquareMeter;
        }
        public override string ToString()
        {
            return $"{SurfaceInSquareCentimeters} cm2 - {MaxPowerPerSquareMeter} W/m²";
        }
    }
}
