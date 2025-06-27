using PaintIn3D;
using UnityEngine;
using UnityEngine.UI;

public class ImagePicker : MonoBehaviour
{
	private StickerHandler _stickerHandler;
	public Sprite _stickerImg;
	public Texture _StickertTexture;
    
	void Start()
	{
		_stickerHandler = StickerHandler.Instance;
	}

	private void ApplySticker()
	{
		if (_stickerHandler)
		{
			_stickerHandler.CreateSticker();
			_stickerHandler.currentSticker.GetComponent<Image>().sprite = _stickerImg;
			_stickerHandler.currentSticker.GetComponent<StickerDragger>().decal.GetComponent<CwPaintDecal>().Texture =_StickertTexture;
			if (SubTabController.Instance)
			{
				SubTabController.Instance.CloseTab();
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
				Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize,false );
				
				if( texture == null )
				{
					Debug.Log( "Couldn't load texture from " + path );
					return;
				}

				
				_StickertTexture = texture;
				var bytes = texture.EncodeToPNG();
				Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
				_stickerImg = sprite;

				ApplySticker();
			}
		} );

		Debug.Log( "Permission result: " + permission );
	}

}
