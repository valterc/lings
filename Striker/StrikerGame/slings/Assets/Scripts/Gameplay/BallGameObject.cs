using UnityEngine;
using System.Collections;
using LiNGS.Common.GameLogic;
using System;

public class BallGameObject : MonoBehaviour, INetworkedObject 
{
	
	[NetworkedField]
	public String NPCName;
	
	[NetworkedField]
	public int PositionX;
	
	[NetworkedField(Simulated = true)]
	public int PositionY;

	[NetworkedField]
	public int TextureColor;
	
	private int lastTextureColor;
	private Color textureColor;

	public static BallGameObject Instantiate(string name, GameObject gameObject)
	{
		GameObject go = (GameObject)GameObject.Instantiate(gameObject);
		go.name = name;
		BallGameObject t = go.GetComponent<BallGameObject>();
		return t;
	}

	void Start () 
	{
	
	}
	
	
	void Update () 
	{
		if (TextureColor != lastTextureColor) 
		{
			lastTextureColor = TextureColor;
			textureColor = new Color32(r: (byte)(TextureColor >> 16), g: (byte)(TextureColor >> 8), b: (byte)(TextureColor), a: 255);
			guiTexture.color = textureColor;
		}
		
		guiTexture.pixelInset = new Rect(PositionX, PositionY, guiTexture.pixelInset.width, guiTexture.pixelInset.height);
	}
	
	
}
