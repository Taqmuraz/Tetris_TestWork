using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class Figure
{
	public class GlobalFigureSegmentControl : FigureSegmentsControl
	{
		private readonly List<Transform> segmentsList = new List<Transform> ();

		public GlobalFigureSegmentControl (Figure figure, GameObject prefab, params FigureSegmentsControl[] concatControls) : base (figure, null)
		{
			if (concatControls != null)
				ConcatSegments(concatControls);
			InitializeSegments(prefab);
		}
		protected override void InitializeSegments (GameObject segmentPrefab)
		{
			foreach (var segment in segmentsList) {
				if (segment)
					segment.SetParent (figure.trans);
			}
		}
		public void ConcatSegments (params FigureSegmentsControl[] concatArray)
		{
			concatArray = concatArray.Where (c => c != null).ToArray ();
			foreach (var concatElement in concatArray) {
				segmentsList.AddRange (concatElement.GetSegmentsArray());
			}
			ClearNullLinks ();
			InitializeSegments (null);
			PlaceSegments ();
		}
		public void ConcatSegments (params Transform[] concatArray)
		{
			concatArray = concatArray.Where (t => t != null).ToArray ();

			segmentsList.AddRange (concatArray);

			ClearNullLinks ();
			InitializeSegments (null);
			PlaceSegments ();
		}
		public override IEnumerable<Transform> GetSegmentsArray ()
		{
			return segmentsList;
		}

		protected void ClearNullLinks ()
		{
			segmentsList.RemoveAll (t => t == null);
		}

		public override void PlaceSegments ()
		{
			return;
			foreach (var segment in segmentsList) {
				if (!segment)
					continue;
				Vector2Int pos = segment.localPosition.ToVector2IntByFloor () + figure.GetCenterOffset ();
				SegmentPosition (segment, pos.x, pos.y);
			}
		}
		public override bool HasSegment (int x, int y)
		{
			Vector2 searchPos = ClampWorldPoint(new Vector2Int (x, y) - figure.GetCenterOffset (), GameField.gameField.GetFieldRect().size);
			foreach (var segment in segmentsList) {
				if (!segment)
					continue;
				if (segment.localPosition.ToVector2IntByFloor () == searchPos)
					return true;
			}
			return false;
		}
		public override bool ContainsGlobalPoint (Vector2Int point)
		{
			point = ClampWorldPoint (point, GameField.gameField.GetFieldRect().size);
			foreach (var segment in segmentsList) {
				if (segment && segment.position.ToVector2IntByFloor () == point)
					return true;
			}
			return false;
		}
		public override void RemoveLine (int level)
		{
			foreach (var segment in segmentsList) {
				if (segment && segment.position.ToVector2IntByFloor ().y == level)
					DestroySegment (segment);
			}
			ClearNullLinks ();
		}
		public override void UnownSegment (Transform segment)
		{
			segmentsList.Remove (segment);
		}
		public override void DestroySegment (Transform segment)
		{
			if (!segmentsList.Where (t => t != null).FirstOrDefault ())
				Destroy (figure);
			base.DestroySegment (segment);
		}

	}
}