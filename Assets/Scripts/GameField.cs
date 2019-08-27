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

			yield return null;

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
		Material material = renderBackgroundField.GetComponent<Renderer> ().material;
		material.SetFloat ("_SizeX", fieldWidth);
		material.SetFloat ("_SizeY", fieldHeight);
	}

	public float GetFallPeriod ()
	{
		return figureFallPeriod;
	}

	public virtual bool IsOutOfBounds (Vector2Int point)
	{
		return !GetFieldRect().Contains(point);
	}
	public RectInt GetFieldRect ()
	{
		return new RectInt (0, 0, fieldWidth, Mathf.Max (fieldHeight * 2, int.MaxValue));
	}

	public virtual bool IsPointValid (Vector2Int point, Figure ignoreFigure)
	{
		if (IsOutOfBounds (point))
			return false;

		return !ContainsAnyFigure (point, ignoreFigure);
	}
	protected bool ContainsAnyFigure (Vector2Int point, Figure ignoreFigure)
	{
		foreach (var figure in Figure.figuresCollection)
		{
			if (figure == ignoreFigure)
				continue;
			if (figure.ContainsWorldPoint (point))
				return true;
		}
		return false;
	}

	protected Figure SpawnFigure (GameObject prefab)
	{
		GameObject instance = Instantiate (prefab, figuresParent);
		instance.transform.localPosition = new Vector3 (fieldWidth * 0.5f, fieldHeight);
		return instance.GetComponent<Figure> ();
	}
}
