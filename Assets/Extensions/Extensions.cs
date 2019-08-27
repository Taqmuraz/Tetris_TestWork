using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
	public static bool IsTransparent (this Color color)
	{
		return color.a < 0.5f;
	}
	public static Vector2Int ToVector2IntByFloor (this Vector3 vector3)
	{
		return new Vector2Int ((int)vector3.x, (int)vector3.y);
	}
	public static Vector2Int ToVector2IntByCeil (this Vector3 vector3)
	{
		return new Vector2Int (Mathf.CeilToInt (vector3.x), Mathf.CeilToInt (vector3.y));
	}
	public static Vector3 ToVector3Float (this Vector2Int vector2int)
	{
		return new Vector3 (vector2int.x, vector2int.y);
	}
	public static T GetRandomArrayElement<T> (this T[] array)
	{
		return array [Random.Range (0, array.Length)];
	}
}

