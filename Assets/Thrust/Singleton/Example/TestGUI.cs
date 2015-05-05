// TestGUI.cs
//
// Author(s): Clint Doriot
// Copyright 2014 Thrust Interactive
// See https://thrust.atlassian.net/wiki/display/THRUST/Thrust.Singleton for full documenation
//
// Unit test case for MyExampleSingleton.cs

using UnityEngine;
using System.Collections;

namespace Thrust.Examples.Singleton
{
    public class TestGUI : MonoBehaviour
    {
        void OnGUI()
        {
            GUI.Box(new Rect(0,0,200,25), MyExampleSingleton.instance.myNewProperty);

            if (GUI.Button(new Rect(0,50,200,25), "Test Singleton"))
                MyExampleSingleton.instance.MyNewFunction();
        }
    }
}
