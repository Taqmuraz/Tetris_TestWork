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
	protected Transform trans { get; private set; }
	public FigureSegmentsControl figureSegmentControl { get; private set; }

	private void Awake ()
	{
		trans = GetComponent<Transform> ();
		if (sprite)
			texture = sprite.texture;

		GameField.gameField.OnLineCreated += OnLineCreated;

		figureSegmentControl = CreateSegmentControl ();

		OnActivate ();

		figures.Add (this);
	}

	protected virtual FigureSegmentsControl CreateSegmentControl ()
	{
		return new FigureSegmentsControl (this, segmentPrefab);
	}

	private void Start ()
	{
		StartCoroutine (FallRoutine ());
	}

	protected virtual IEnumerator FallRoutine ()
	{
		WaitForSeconds wait = new WaitForSeconds (GameField.gameField.GetFallPeriod());

		while (CanMoveAtDirection (Vector2Int.down))
		{

			Move (Vector2Int.down);

			yield return wait;
		}

		OnDisactivate ();
		yield return null;
	}

	public Vector2Int FromFigureToWorldSpace (Vector2Int figure)
	{
		return figure + trans.position.ToVector2IntByFloor ();
	}
	public Vector2Int FromWorldToFigureSpace (Vector2Int world)
	{
		return world - trans.position.ToVector2IntByFloor ();
	}

	protected virtual void OnActivate ()
	{
		
	}


	protected virtual void OnDisactivate ()
	{
		GlobalFigure.GetGlobalFigure ().Concat (this);
		Destroy (gameObject);
	}

	protected virtual void OnLineCreated (params int[] levels)
	{
		foreach (var i in levels) {
			figureSegmentControl.RemoveLine (i);
		}
	}

	protected bool CanMoveAtDirection (Vector2Int direction) 
	{
		var segments = figureSegmentControl.GetSegmentsArray ();

		foreach (var segment in segments) {
			if (!segment)
				continue;
			if (!GameField.gameField.IsPointValid (segment.position.ToVector2IntByFloor () + direction, this))
				return false;
		}
		return true;
	}

	protected virtual void OnDestroy ()
	{
		figures.Remove (this);
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
		if (!texture)
			return Vector2Int.zero;
		return new Vector2Int (texture.width / 2, texture.height / 2);
	}
	private Vector2Int Calc_devisionTo2 ()
	{
		if (!texture)
			return Vector2Int.zero;
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
