// PersistentSingleton.cs
//
// Author(s): Clint Doriot
// Copyright Thrust Interactive 2014
// See https://thrust.atlassian.net/wiki/display/THRUST/Thrust.Singleton for full documenation
//
// Extends basic Singleton functionality for classes that should stay loaded across all Unity scenes

using UnityEngine;

namespace Thrust.Singleton
{
    public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            // Call Singleton Awake function to ensure only one instance is created
            // If this is a duplicate instance, don't initialize
            base.Awake();
            if (this != _instance)
                return;

            // Make object persistent between scenes
            Object.DontDestroyOnLoad(this.gameObject);
        }
    }
}
