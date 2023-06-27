﻿using HarmonyLib;
using RotationAnarchyEvolved;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

[HarmonyPatch]
public class BuilderOnDisablePatch
{

    // Get protected method and make it public so we can patch
    static MethodBase TargetMethod() => AccessTools.Method(typeof(Builder), "OnDisable");

    [HarmonyPrefix]
    public static void PreOnDisable()
    {
        RAE.MainController.SetBuilder(null);
    }
}