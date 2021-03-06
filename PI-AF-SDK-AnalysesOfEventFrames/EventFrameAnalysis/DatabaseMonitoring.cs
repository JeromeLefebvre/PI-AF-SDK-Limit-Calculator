﻿#region Copyright
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

using OSIsoft.AF;
using OSIsoft.AF.EventFrame;
using System;
using System.Collections.Generic;
using System.Timers;

namespace EventFrameAnalysis
{
    class DatabaseMonitoring
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Timer refreshTimer = new Timer(1000);
        private object cookie;

        private AFDatabase afdatabase;
        private LimitCalculation calculation;

        public DatabaseMonitoring(LimitCalculation calculation)
        {
            this.calculation = calculation;
            afdatabase = calculation.afdatabase;
            logger.Info($"Monitoring the database: {afdatabase}");
            // Initialize the cookie (bookmark)
            afdatabase.FindChangedItems(false, int.MaxValue, null, out cookie);

            refreshTimer.Elapsed += new ElapsedEventHandler(OnElapsed);
            afdatabase.Changed += new EventHandler<AFChangedEventArgs>(OnChanged);
            refreshTimer.Start();
        }

        internal void OnChanged(object sender, AFChangedEventArgs e)
        {
            List<AFChangeInfo> changes = new List<AFChangeInfo>();
            changes.AddRange(afdatabase.FindChangedItems(true, int.MaxValue, cookie, out cookie));
            AFChangeInfo.Refresh(afdatabase.PISystem, changes);

            foreach (AFChangeInfo info in changes.FindAll(change => change.Identity == AFIdentity.EventFrame))
            {
                if (info.Action != AFChangeInfoAction.Removed) { 
                    AFEventFrame lastestEventFrame = (AFEventFrame)info.FindObject(afdatabase.PISystem, true);
                    logger.Debug($"A new event to verify: {lastestEventFrame.Name}");
                    calculation.performAction(lastestEventFrame, info.Action);
                }
            }
        }

        internal void OnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Refreshing Database will cause any external changes to be seen which will result in the triggering of the OnChanged event handler
            lock (afdatabase)
            {
                // possible to hit  a timeout here
                afdatabase.Refresh();
            }
            refreshTimer.Start();
        }
    }
}
