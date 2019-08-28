using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComplexFigure : Figure
{
	public static ComplexFigure CreateFreeFigure (Transform[] segments)
	{
		ComplexFigure figure = Instantiate (Resources.Load<GameObject> ("Prefabs/ComplexFigure")).GetComponent<ComplexFigure> ();
		Debug.Log ("Created!");

		(figure.figureSegmentControl as GlobalFigureSegmentControl).ConcatSegments (segments);

		return figure;
	}
	protected override FigureSegmentsControl CreateSegmentControl ()
	{
		return new GlobalFigureSegmentControl (this, null);
	}
	public void Concat (Figure figure)
	{
		(figureSegmentControl as GlobalFigureSegmentControl).ConcatSegments (figure.figureSegmentControl);
	}
	protected override IEnumerator FallRoutine ()
	{
		WaitForSeconds wait = new WaitForSeconds (GameField.gameField.GetFallPeriod());
		yield return wait;

		while (figureSegmentControl.GetSegmentsArray ().Where (s => s != null).FirstOrDefault () && CanMoveAtDirection (Vector2Int.down))
		{
			Move (Vector2Int.down);
			yield return wait;
		}

		OnDisactivate ();
		yield return null;
	}
	protected override void OnActivate ()
	{
		base.OnActivate ();
		trans.SetParent(GameField.gameField.GetFiguresParent());
		trans.localPosition = Vector3.zero;
	}
	private void OnDrawGizmos ()
	{
		var segments = figureSegmentControl.GetSegmentsArray ();

		foreach (var segment in segments) {
			if (!segment)
				continue;
			if (!GameField.gameField.IsPointValid (segment.position.ToVector2IntByFloor () + Vector2Int.down, this))
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.green;
			Gizmos.DrawCube (segment.position, Vector3.one);
		}
	}
}
