using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Root.Directional
{
	public class DirectionalSpritesImporter : AssetPostprocessor
	{
		private void OnPreprocessTexture ()
		{
			TextureImporter textureImporter  = (TextureImporter)assetImporter;

			textureImporter.isReadable = true;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
			textureImporter.wrapMode = TextureWrapMode.Clamp;
		}
	}
}