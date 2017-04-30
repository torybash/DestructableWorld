using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Map : MonoBehaviour {


	[Serializable]
	public enum DebugLevel
	{
		NONE	= 0,
		SIMPLE	= 1,
		ALL	= 2,
	}

	private bool DebugSimple { get { return debugLevel >= DebugLevel.SIMPLE; } }
	private bool DebugAll { get { return debugLevel >= DebugLevel.ALL; } }

	[SerializeField] private MeshRenderer rend;

	[SerializeField] private int xSize = 1920;
	[SerializeField] private int ySize = 1080;

	[SerializeField] private float brushSize = 8f;

	[SerializeField] private Texture2D testTex;

	[SerializeField] private DebugLevel debugLevel = DebugLevel.SIMPLE;

	private Texture2D _tex;
	private Material _rendMat;

	public int XSize { get { return xSize; } }
	public int YSize { get { return ySize; } }



	public Texture2D Tex { get { return _tex; } }


	private Color _alphaColor = new Color(0,0,0,0);

	void Awake()
	{
		Phys.SetMap(this);
	}

	void Start () {
		rend.transform.localScale = new Vector3(xSize, ySize, 1f) / Phys.PIX_PER_UNIT;
		rend.transform.position = new Vector3(xSize, ySize, 1f) / Phys.PIX_PER_UNIT / 2f;

		_tex = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);
		_tex.filterMode = FilterMode.Point;
		_rendMat = rend.material;

		_tex.SetPixels32(testTex.GetPixels32());
		_tex.Apply();

		_rendMat.mainTexture = _tex;

		Events.I.MapLoaded.Invoke(this);
	}
	
	void Update () {
		if (Input.GetMouseButton(0))
			Painting(0);
		if (Input.GetMouseButton(1))
			Painting(1);
		
	}

	private void Painting(int type){
		var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) * Phys.PIX_PER_UNIT;

		Debug.Log("MousePos: "+ mousePos);

		for (int x = (int)(mousePos.x - brushSize); x < (int)(mousePos.x + brushSize + 1f); x++) {
			for (int y = (int)(mousePos.y - brushSize); y < (int)(mousePos.y + brushSize + 1f); y++) {
				_tex.SetPixel(x, y, type == 0 ? Color.red : _alphaColor);
			}
		}

		_tex.Apply();

		_rendMat.mainTexture = _tex;
	}

	public bool IsAnyTilesFull(Pos2D[] pos2Ds){
		for (int i = 0; i < pos2Ds.Length; i++) {
			var pos2D = pos2Ds[i];
			if (IsTileFull(pos2D.x, pos2D.y)) return true;
		}
		return false;
	}

	public bool IsTileFull(int x, int y)
	{
		return _tex.GetPixel(x, y).a > 0;
	}

	public bool IsTileFull(Pos2D pos)
	{
		//Debug.Log("IsTileFull - pos: " + pos + " = " + (_tex.GetPixel(pos.x, pos.y).a > 0));
		return _tex.GetPixel(pos.x, pos.y).a > 0;
	}
	
}
