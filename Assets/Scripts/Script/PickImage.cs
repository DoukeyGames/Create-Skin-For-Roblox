using System.Collections;
using System.Collections.Generic;
using PaintIn3D;
using UnityEngine;
using UnityEngine.UI;

public class PickImage : MonoBehaviour
{
	private StickerHandler _stickerHandler;
	public Sprite _stickerImg;
	public Texture _StickertTexture;
    
	// Start is called before the first frame update
	void Start()
	{
		_stickerHandler = StickerHandler.Instance;
	}

	public void setSticker()
	{
		if (_stickerHandler)
		{
			_stickerHandler.CreateSticker();
			_stickerHandler.currentSticker.GetComponent<Image>().sprite = _stickerImg;
			_stickerHandler.currentSticker.GetComponent<UIDragAndSnap>().decal.GetComponent<CwPaintDecal>().Texture =_StickertTexture;
			if (SubMenuController.Instance)
			{
				SubMenuController.Instance.closeCategory();
			}
		}
	}


public void ChooseImage()
{
	PickImageFromGallery( 2048 );
}

private void PickImageFromGallery( int maxSize )
{
	NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
	{
		Debug.Log( "Image path: " + path );
		if( path != null )
		{
			// Create Texture from selected image
			Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize,false );
			
			if( texture == null )
			{
				Debug.Log( "Couldn't load texture from " + path );
				return;
			}

			// // Assign texture to a temporary quad and destroy it after 5 seconds
			// GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
			// quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
			// quad.transform.forward = Camera.main.transform.forward;
			// quad.transform.localScale = new Vector3( 1f, texture.height / (float) texture.width, 1f );

			// Material material = quad.GetComponent<Renderer>().material;
			// if( !material.shader.isSupported ) // happens when Standard shader is not included in the build
			// 	material.shader = Shader.Find( "Legacy Shaders/Diffuse" );
			//
			// material.mainTexture = texture;
			
			_StickertTexture = texture;
			var bytes = texture.EncodeToPNG();
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
			_stickerImg = sprite;

			setSticker();
			//Destroy( quad, 5f );

			// If a procedural texture is not destroyed manually, 
			// it will only be freed after a scene change
			//Destroy( texture, 5f );
		}
	} );

	Debug.Log( "Permission result: " + permission );
}

}
