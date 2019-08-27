using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameField : MonoBehaviour
{
	[SerializeField] Transform figuresParent;
	[SerializeField] Transform renderBackgroundField;
	[SerializeField] int fieldWidth = 10;
	[SerializeField] int fieldHeight = 20;
	[SerializeField] float figureFallPeriod = 1f;
	[SerializeField] FigureEditorField[] figures;

	Transform trans;

	public static GameField gameField { get; private set; }


	private void Awake ()
	{
		gameField = this;
		trans = GetComponent<Transform> ();
		SetupGameField ();

		StartCoroutine (SpawnRoutine());
	}

	private void FigureUserControl (Figure figure)
	{
		Vector2Int input = Vector2Int.zero;

		if (Input.GetKeyDown (KeyCode.RightArrow))
			input.x += 1;
		if (Input.GetKeyDown (KeyCode.LeftArrow))
			input.x -= 1;


		figure.Move (input);
	}

	private IEnumerator SpawnRoutine ()
	{
		Figure figure;
		bool canPlace = true;

		while (canPlace) {

			for (int p = 0; p < fieldWidth; p++) {
				if (!IsPointValid (new Vector2Int (p, fieldHeight - 1), null)) {
					canPlace = false;
					break;
				}
			}

			if (!canPlace)
				break;

			figure = SpawnFigure (figures.GetRandomArrayElement().GetPrefab());


			while (figure.isActive) {
				FigureUserControl (figure);
				yield return null;
			}
		}
		yield return null;
	}

	private void SetupGameField ()
	{
		trans.position = new Vector3 (fieldWidth * 0.5f, fieldHeight * 0.5f);
		figuresParent.position = Vector3.zero;
		renderBackgroundField.localScale = new Vector3 (fieldWidth, fieldHeight, 1f);
	}

	public float GetFallPeriod ()
	{
		return figureFallPeriod;
	}

	public virtual bool IsPointValid (Vector2Int point, Figure ignoreFigure)
	{
		RectInt rect = new RectInt (0, 0, fieldWidth, Mathf.Max(fieldHeight * 2, int.MaxValue));

		if (!rect.Contains (point))
			return false;

		foreach (var figure in Figure.figuresCollection)
		{
			if (figure == ignoreFigure)
				continue;
			if (figure.ContainsWorldPoint (point))
				return false;
		}
		return true;
	}

	protected Figure SpawnFigure (GameObject prefab)
	{
		GameObject instance = Instantiate (prefab, figuresParent);
		instance.transform.localPosition = new Vector3 (fieldWidth * 0.5f, fieldHeight);
		return instance.GetComponent<Figure> ();
	}

	private void OnDrawGizmos ()
	{

		try {
			Gizmos.color = Color.green;
			for (int x = 0; x < fieldWidth; x++) {
				for (int y = 0; y < fieldHeight; y++) {
					bool draw = false;
					foreach (var figure in Figure.figuresCollection) {
						if (figure.ContainsWorldPoint (new Vector2Int (x, y)))
						{
							draw = true;
							break;
						}
					}
					if (draw)
						Gizmos.DrawCube (new Vector3(x + 0.5f, y + 0.5f, -1), Vector3.one);
				}
			}
		} catch {
			
		}
		for (int p = 0; p < fieldWidth; p++) {
			Vector2Int check = new Vector2Int (p, fieldHeight - 1);
			if (!IsPointValid (check, null)) {
				Gizmos.color = Color.red;
				Gizmos.DrawCube (new Vector3 (check.x + 0.5f, check.y + 0.5f), Vector3.one);
			}
		}
	}
}
