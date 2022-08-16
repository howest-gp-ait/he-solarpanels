using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIT.SolarPanels.Server.Core.Entities
{
    public class SolarStation
    {
        private List<SolarPanel> solarPanels;
        public string StationId { get; set; }
        public IEnumerable<SolarPanel> SolarPanels 
        { 
            get
            {
                return solarPanels.AsReadOnly();
            }
        }
        public int Suncondition { get; set; }
        public double StationPerformance
        {
            get
            {
                double stationPerformance = 0;
                foreach(SolarPanel solarPanel in SolarPanels)
                {
                    stationPerformance += 1.0 * (solarPanel.MaxPowerPerSquareMeter * (1.0* solarPanel.SurfaceInSquareCentimeters / 10000) * (1.0* Suncondition/100))/1000;
                }
                return stationPerformance;
            }
        }
        public SolarStation(string stationId, int suncondition)
        {
            solarPanels = new List<SolarPanel>();
            StationId = stationId;
            Suncondition = suncondition;
        }
        public void AddSolarPanel(string panelId, int surfaceInSquareCentimeters, int maxPowerPerSquareMeter)
        {
            solarPanels.Add(new SolarPanel(panelId, surfaceInSquareCentimeters, maxPowerPerSquareMeter));
        }
        public override string ToString()
        {
            return $"{StationId}";
        }

    }
}
