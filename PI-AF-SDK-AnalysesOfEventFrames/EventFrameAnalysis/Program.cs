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
using System.Timers;
using System.Collections.Generic;
using OSIsoft.AF;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Asset;
using LimitCalculatorSDK;

namespace EventFrameAnalysis
{
    class Program
    {
        static Timer refreshTimer = new Timer(1000);
        static object cookie;
        static ElapsedEventHandler elapsedEH;
        static EventHandler<AFChangedEventArgs> changedEH;

        static AFDatabase afdatabase;
        static List<LimitCalculation> calculations;

        public static void WaitForQuit()
        {
            do
            {
                Console.Write("Enter Q to exit the program: ");
            } while (Console.ReadLine() != "Q");
        }

        static void Main(string[] args)
        {
            PISystems pisystems = new PISystems();
            PISystem sys = pisystems.DefaultPISystem;

            afdatabase = sys.Databases[Properties.Settings.Default.AFDatabase];
            List<CalculationPreference> calculationPreferences;

            calculationPreferences = new List<CalculationPreference> { };
            PISystem pisystem = afdatabase.PISystem;
            AFDatabase configuration = pisystem.Databases["Configuration"];
            AFElements preferences = configuration.Elements["LimitCalculator"].Elements;
            foreach (AFElement preference in preferences)
            {
                string JSON = (string)preference.Attributes["configuration"].GetValue().Value;
                calculationPreferences.Add(CalculationPreference.CalculationPreferenceFromJSON(JSON));
            }

            calculations = new List<LimitCalculation> { };

            foreach (CalculationPreference pref in calculationPreferences)
            {
                calculations.Add(new LimitCalculation(pref));
            }

            // Initialize the cookie (bookmark)
            afdatabase.FindChangedItems(false, int.MaxValue, null, out cookie);

            // Initialize the timer, used to refresh the database
            elapsedEH = new System.Timers.ElapsedEventHandler(OnElapsed);
            refreshTimer.Elapsed += elapsedEH;

            // Set the function to be triggered once a change is detected
            changedEH = new EventHandler<AFChangedEventArgs>(OnChanged);
            afdatabase.Changed += changedEH;
            refreshTimer.Start();

            WaitForQuit();

            afdatabase.Changed -= changedEH;
            refreshTimer.Elapsed -= elapsedEH;
            refreshTimer.Stop();
        }

        internal static void OnChanged(object sender, AFChangedEventArgs e)
        {
            // Find changes since the last refresh
            List<AFChangeInfo> changes = new List<AFChangeInfo>();
            changes.AddRange(afdatabase.FindChangedItems(true, int.MaxValue, cookie, out cookie));
            AFChangeInfo.Refresh(afdatabase.PISystem, changes);

            foreach (AFChangeInfo info in changes.FindAll(i => i.Identity == AFIdentity.EventFrame))
            {
                AFEventFrame lastestEventFrame = (AFEventFrame)info.FindObject(afdatabase.PISystem, true);
                
                foreach (LimitCalculation calculation in calculations)
                {
                    calculation.performAction(lastestEventFrame, info.Action);
                }
            }
        }

        internal static void OnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Refreshing Database will cause any external changes to be seen which will result in the triggering of the OnChanged event handler
            lock (afdatabase)
            {
                afdatabase.Refresh();
            }
            refreshTimer.Start();
        }
    }
}