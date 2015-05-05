// MyExampleSingleton.cs
//
// Author(s): Clint Doriot
// Copyright 2014 Thrust Interactive
// See https://thrust.atlassian.net/wiki/display/THRUST/Thrust.Singleton for full documenation
//
// Example to test extending the Singleton class

using UnityEngine;
using Thrust.Singleton;

namespace Thrust.Examples.Singleton
{
    public class MyExampleSingleton : Singleton<MyExampleSingleton>
    {
    	// new properties
    	public string myNewProperty = "Hello World";

    	// if needing an awake function, be sure to override the existing one
    	// a similar override is necessary for OnApplicationQuit 
    	protected override void Awake()
    	{
    		// required as first step when overriding Awake()
    		base.Awake();
    		if (this != _instance) return;
    		
    		// TODO: my Awake() initialization routine
    	} 
    	
    	// new function
    	public void MyNewFunction ()
    	{
    		Debug.Log(myNewProperty);
    	}
    }
}