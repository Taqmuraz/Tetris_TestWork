using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Figure : MonoBehaviour
{
	public static IEnumerable<Figure> figuresCollection
	{
		get { return figures; }
	}
	private static readonly List<Figure> figures = new List<Figure>();

	[SerializeField] Sprite sprite;
	private Texture2D texture;
	private Transform trans;

	public bool isActive { get; private set; }

	private void Awake ()
	{
		trans = GetComponent<Transform> ();
		GetComponent<SpriteRenderer> ().sprite = sprite;
		texture = sprite.texture;

		isActive = true;

		figures.Add (this);
	}

	private void Start ()
	{
		StartCoroutine (FallRoutine ());
	}

	private IEnumerator FallRoutine ()
	{
		WaitForSeconds wait = new WaitForSeconds (GameField.gameField.GetFallPeriod());

		while (CanMoveAtDirection(Vector2Int.down))
		{
			Move (Vector2Int.down);

			yield return wait;
		}

		isActive = false;
	}

	public Vector2Int FromFigureToWorldSpace (Vector2Int figure)
	{
		return figure + trans.position.ToVector2IntByFloor ();
	}
	public Vector2Int FromWorldToFigureSpace (Vector2Int world)
	{
		return world - trans.position.ToVector2IntByFloor ();
	}

	protected bool CanMoveAtDirection (Vector2Int direction) 
	{
		Vector2Int devisionTo2;
		Vector2Int figureCenterOffset = GetCenterOffset (out devisionTo2);

		for (int x = -figureCenterOffset.x - devisionTo2.x; x < figureCenterOffset.x; x++) 
		{
			for (int y = -figureCenterOffset.y - devisionTo2.y; y < figureCenterOffset.y; y++) 
			{
				
				Vector2Int texturePos = new Vector2Int (x, y) + figureCenterOffset;
				if (texture.GetPixel (texturePos.x, texturePos.y).IsTransparent ())
					continue;
					
				if (!GameField.gameField.IsPointValid (FromFigureToWorldSpace (new Vector2Int (x, y) + direction), this))
					return false;
			}
		}
		
		return true;
	}

	public bool Move (Vector2Int direction)
	{
		if (CanMoveAtDirection (direction))
		{
			trans.position += direction.ToVector3Float ();
			return true;
		}
		return false;
	}

	protected Vector2Int GetCenterOffset (out Vector2Int devisionTo2)
	{
		Vector2Int figureCenterOffset = new Vector2Int (texture.width / 2, texture.height / 2);
		devisionTo2 = new Vector2Int (texture.width & 1, texture.height & 1);
		return figureCenterOffset + devisionTo2;
	}

	public bool ContainsWorldPoint (Vector2Int point)
	{
		Vector2Int devision;
		Vector2Int centerOffset = GetCenterOffset (out devision);

		point = FromWorldToFigureSpace (point);
		point += centerOffset;

		if (new Rect (0, 0, texture.width, texture.height).Contains (point))
			return !texture.GetPixel (point.x, point.y).IsTransparent();
		
		return false;
	}
}
