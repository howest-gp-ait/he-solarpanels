using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIT.SolarPanels.Server.Core.Entities;

namespace AIT.SolarPanels.Server.Core.Services
{
    public class SolarPanelService
    {
        private List<SolarStation> solarStations;
        public IEnumerable <SolarStation> SolarStations 
        { 
            get
            {
                return solarStations.AsReadOnly();
            }
            
        }
        public double TotalPerformance
        {
            get
            {
                double totalPerformance = 0;
                foreach(SolarStation solarStation in solarStations)
                {
                    totalPerformance += solarStation.StationPerformance;
                }
                return totalPerformance;
            }
        }

        public SolarPanelService()
        {
            solarStations = new List<SolarStation>();
        }
        public bool? ProcessMessage(string message, out string stationId)
        {
            try
            {
                message = message.Replace("#@@#", "");
                string[] stationParts = message.Split("-+-");
                stationId = stationParts[0];
                foreach (SolarStation solarStation in SolarStations)
                {
                    if (solarStation.StationId == stationId)
                    {
                        solarStations.Remove(solarStation);
                        break;
                    }
                }
                if (stationParts.Length == 2)
                {
                    return null;
                }
                else
                {

                    int suncondition = int.Parse(stationParts[1]);
                    string panels = stationParts[2];

                    SolarStation newSolarStation = new SolarStation(stationId, suncondition);
                    solarStations.Add(newSolarStation);
                    string[] panelParts = panels.Split("-@-");
                    foreach (string panelPart in panelParts)
                    {
                        if (panelPart == "") continue;
                        string[] panelDetails = panelPart.Split("|");
                        string panelId = panelDetails[0];
                        int surfaceInSquareCentimeters = int.Parse(panelDetails[1]);
                        int maxPowerPerSquareMeter = int.Parse(panelDetails[2]);
                        newSolarStation.AddSolarPanel(panelId, surfaceInSquareCentimeters, maxPowerPerSquareMeter);
                    }
                    return true;
                }
            }
            catch
            {
                stationId = null;
                return false;
            }
        }
    }
}
