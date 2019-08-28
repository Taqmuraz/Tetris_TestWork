using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public sealed class GlobalFigure : ComplexFigure
{
	public static GlobalFigure globalFigureInstance { get; private set; }

	public static GlobalFigure GetGlobalFigure ()
	{
		if (globalFigureInstance)
			return globalFigureInstance;
		return Instantiate (Resources.Load<GameObject> ("Prefabs/GlobalFigure")).GetComponent<GlobalFigure> ();
	}

	protected override void OnActivate ()
	{
		base.OnActivate ();
		globalFigureInstance = this;
	}
	protected override void OnDisactivate ()
	{
	}
	protected override void OnLineCreated (params int[] levels)
	{
		base.OnLineCreated (levels);

		int level = levels.Max ();
		Transform[] segments = (figureSegmentControl as GlobalFigureSegmentControl).GetSegmentsArray ().Where (s => s != null && s.position.ToVector2IntByFloor().y > level).ToArray();
		foreach (var segment in segments) {
			(figureSegmentControl as GlobalFigureSegmentControl).UnownSegment (segment);
		}

		ComplexFigure.CreateFreeFigure (segments);
	}
	protected override IEnumerator FallRoutine ()
	{
		yield return null;
	}
}
