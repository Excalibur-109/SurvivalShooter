using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Excalibur
{
    [CreateAssetMenu(fileName = CP.ProjectSettings, menuName = "Excalibur/Project Settings")]
    public sealed class ProjectSettings : ScriptableObject
    {
        [SerializeField] bool _enableABLoad;

        public string
            configurationPath;

        public bool EnableABLoad => _enableABLoad;
    }
}


