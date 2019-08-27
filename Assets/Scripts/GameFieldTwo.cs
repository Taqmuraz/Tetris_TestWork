using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldTwo : GameField
{
	public override bool IsOutOfBounds (Vector2Int point)
	{
		return point.y < 0;
	}
}
