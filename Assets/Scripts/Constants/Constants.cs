﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Constants {

	public const string Controller = "Mouse";
	
	public const int PlayerStartHP = 200;
    public const float MoveTimeInSeconds = 20f;

    public const float SpecialMoveFillRequirement = 30f; //How much the maximum of the chosen 'special' is
    public const float SpecialMoveMultiplier = 2f; //How much the 'special' bar of a color fills up.
    public const int TurnPassHealAmount = 5;

    public const int gridSizeHorizontal = 8;
    public const int gridSizeVertical = 8;

    public const int AmountOfColors = 4;
}
