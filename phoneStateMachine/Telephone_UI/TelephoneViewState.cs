﻿using Capiche.Common;

namespace Telephone_UI
{

    /// <summary>
    /// Class defining a view state of an UI
    /// </summary>
    public class TelephoneViewState : IViewState
    {
        // Public members
        public string Name { get; private set; }
        public bool Bell { get; private set; }
        public bool Line { get; private set; }
        public bool ReceiverHungUp { get; private set; }
        public bool IsDefaultViewState { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bell"></param>
        /// <param name="line"></param>
        /// <param name="receiverHungUp"></param>
        /// <param name="isDefaultViewState"></param>
        public TelephoneViewState(string name, bool bell, bool line, bool receiverHungUp,  bool isDefaultViewState=false)
        {
            Name = name;
            Bell = bell;
            Line = line;
            ReceiverHungUp = receiverHungUp;
            IsDefaultViewState = isDefaultViewState;
        }
    }
}

// =======================================================================
// Disclaimer - Building State Machines in .NET
// =======================================================================
// 
//   THIS CODE IS EDUCATIONAL AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//   ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// 
//    Copyright (C) 2013 Wechsler Consulting GmbH & Co. KG
// 
//    Alexander Wechsler, Enterprise Architect
//    Microsoft Regional Director Germany | eMVP
//    Wechsler Consulting GmbH & Co. KG
// 
// =======================================================================
