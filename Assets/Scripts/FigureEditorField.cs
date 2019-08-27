using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FigureEditorField
{
	[SerializeField] GameObject prefab;
	[SerializeField] float chanceToSelect = 100f;

	public GameObject GetPrefab ()
	{
		return prefab;
	}
	public float GetChanceToSelect ()
	{
		return chanceToSelect;
	}
}
