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

using System;
using System.Collections.Generic;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using LimitCalculatorSDK;
using System.Threading.Tasks;

namespace EventFrameAnalysis
{
    class Program
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void WaitForQuit()
        {
            do
            {
                Console.Write("Enter Q to exit the program: ");
            } while (Console.ReadLine() != "Q");
        }

        static void Main(string[] args)
        {
            logger.Info("The application has started.");

            PISystem pisystem = new PISystems().DefaultPISystem; ;
            AFDatabase configuration = pisystem.Databases["Configuration"];
            AFElements preferences = configuration.Elements["LimitCalculator"].Elements;
            logger.Info($"Will process {preferences.Count} preferences");
            List<DatabaseMonitoring> monitoredDB = new List<DatabaseMonitoring> { };
            Parallel.ForEach(preferences, (preference) =>
                {
                    string JSON = (string)preference.Attributes["configuration"].GetValue().Value;
                    LimitCalculation calc = new LimitCalculation(CalculationPreference.CalculationPreferenceFromJSON(JSON));
                    monitoredDB.Add(new DatabaseMonitoring(calc));
                });

            WaitForQuit();
            
            foreach(DatabaseMonitoring db in monitoredDB)
            {
                db.quit();
            }
        }
    }
}