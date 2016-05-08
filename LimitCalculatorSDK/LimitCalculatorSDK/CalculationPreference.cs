using System.Collections.Generic;
using System.Linq;

using OSIsoft.AF.Asset;
using Newtonsoft.Json;

namespace LimitCalculatorSDK
{
    public class CalculationPreference
    {
        public string sensorPath { get; set; }
        public string eventFrameQuery { get; set; }
        public Dictionary<string, string> calculationsToPerform = new Dictionary<string, string> { };
        private static Dictionary<string, AFAttributeTrait> reverse = AFAttributeTrait.AllLimits.ToDictionary(p => p.Name, p => p);

        public CalculationPreference(string sensorPath, string eventFrameQuery, Dictionary<string, string> calculationsToPerform)
        {
            this.sensorPath = sensorPath;
            this.eventFrameQuery = eventFrameQuery;
            this.calculationsToPerform = calculationsToPerform;
        }

        public static CalculationPreference CalculationPreferenceFromJSON(string json)
        {
            return JsonConvert.DeserializeObject<CalculationPreference>(json);
        }

        public Dictionary<AFAttributeTrait, string> getTraitDictionary()
        {
            return calculationsToPerform.ToDictionary(p => reverse[p.Key], p => p.Value);
        }

        public string JSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
