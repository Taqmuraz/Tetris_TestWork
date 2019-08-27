using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Figure : MonoBehaviour
{

	public static IEnumerable<Figure> figuresCollection
	{
		get { return figures; }
	}
	private static readonly List<Figure> figures = new List<Figure>();

	[SerializeField] Sprite sprite;
	[SerializeField] GameObject segmentPrefab;
	private Texture2D texture;
	private Transform trans;
	private FigureSegmentsControl figureSegmentControl;

	public bool isActive { get; private set; }

	private void Awake ()
	{
		trans = GetComponent<Transform> ();
		texture = sprite.texture;

		isActive = true;

		figureSegmentControl = new FigureSegmentsControl (this, segmentPrefab);

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
			figureSegmentControl.PlaceSegments ();
			return true;
		}
		return false;
	}

	private Vector2Int Calc_figureCenter ()
	{
		return new Vector2Int (texture.width / 2, texture.height / 2);
	}
	private Vector2Int Calc_devisionTo2 ()
	{
		return new Vector2Int (texture.width & 1, texture.height & 1);
	}


	protected Vector2Int GetCenterOffset (out Vector2Int devisionTo2)
	{
		return Calc_figureCenter () + (devisionTo2 = Calc_devisionTo2 ());
	}
	protected Vector2Int GetCenterOffset ()
	{
		return Calc_figureCenter () + Calc_devisionTo2 ();
	}

	public bool ContainsWorldPoint (Vector2Int point)
	{
		return figureSegmentControl.ContainsGlobalPoint (point);
	}
}
