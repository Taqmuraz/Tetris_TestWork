using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Figure
{
	public class FigureSegmentsControl : NullBool
	{
		Transform[,] segments;
		Figure figure;

		public FigureSegmentsControl (Figure figure, GameObject segmentPrefab)
		{
			this.figure = figure;
			InitializeSegments(segmentPrefab);
		}
		private void InitializeSegments (GameObject segmentPrefab)
		{
			segments = new Transform[figure.texture.width, figure.texture.height];

			for (int y = 0; y < segments.GetLength(1); y++) {
				for (int x = 0; x < segments.GetLength(0); x++) {

					if (figure.texture.GetPixel (x, y).IsTransparent ())
						continue;

					GameObject instance = Instantiate (segmentPrefab, figure.trans);
					Transform trans = instance.transform;
					SegmentPosition (trans, x, y);
					segments [x, y] = trans;
				}
			}
		}
		public void PlaceSegments ()
		{
			for (int y = 0; y < segments.GetLength (1); y++) {
				for (int x = 0; x < segments.GetLength (0); x++) {
					if (segments [x, y] == null)
						continue;
					SegmentPosition (segments[x, y], x, y);
				}
			}
		}
		private void SegmentPosition (Transform trans, int x, int y)
		{
			Vector2Int pos = new Vector2Int (x, y) - figure.GetCenterOffset ();

			RectInt gameRect = GameField.gameField.GetFieldRect ();

			Vector2Int worldPos = figure.FromFigureToWorldSpace (pos);


			if (!gameRect.Contains (worldPos)) {
				worldPos = ClampWorldPoint (worldPos, gameRect.size);

				trans.position = worldPos.ToVector3Float () + new Vector3 (0.5f, 0.5f, 0f);
			} else
				trans.localPosition = pos.ToVector3Float () + new Vector3 (0.5f, 0.5f, 0f);
		}

		private Vector2Int ClampWorldPoint (Vector2Int worldPos, Vector2Int worldSize)
		{
			if (worldPos.x <= 0)
				worldPos.x = worldSize.x + worldPos.x;
			if (worldPos.x >= worldSize.x)
				worldPos.x = worldPos.x % worldSize.x;
			
			return worldPos;
		}

		public bool ContainsGlobalPoint (Vector2Int point)
		{
			point = ClampWorldPoint (point, GameField.gameField.GetFieldRect().size);
			foreach (var segment in segments) {
				if (segment && segment.position.ToVector2IntByFloor () == point)
					return true;
			}
			return false;
		}
	}
}
