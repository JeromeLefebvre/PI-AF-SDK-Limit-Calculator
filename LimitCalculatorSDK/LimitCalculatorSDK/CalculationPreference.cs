#region Copyright
//  Copyright 2016 OSIsoft, LLC
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
#endregion

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
