using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class GameMaster : MonoBehaviour  {
    public static bool gamePaused { get; private set; }
    public static readonly string waterTag = "EnergySource";
    public static ShipController currentShip;
    public static GameMaster currentSession;

    private Action sceneClosingEvent;

    private void Awake()
    {
        currentSession = this;
    }

    public void SubscribeToSceneClosingEvent(Action a)
    {
        sceneClosingEvent += a;
    }
    public void UnsubscribeFromSceneClosingEvent(Action a)
    {
        if (sceneClosingEvent != null) sceneClosingEvent -= a;
    }
}
