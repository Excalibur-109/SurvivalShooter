using System;
using UnityEngine;

namespace Excalibur
{
    public enum Capture { Component, Entity }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class AutoCaptureAttribute : Attribute 
    {
        private string _name;
        private Capture _capture;
        private Type _component;

        public AutoCaptureAttribute(string name, Capture capture, Type component)
        {
            _name = name;
            _capture = capture;
            _component = component;
        }

        public string name => _name;
        public Capture capture => _capture;
        public Type component => _component;
    }
}
