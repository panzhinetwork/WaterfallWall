using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.UGUI_Extension
{
	public class EmptyRect : Graphic, ICanvasRaycastFilter
	{
		protected override void OnPopulateMesh (VertexHelper vh)
		{
			// Do not draw.
			vh.Clear ();
		}

		public virtual bool IsRaycastLocationValid (Vector2 sp, Camera eventCamera)
		{
			return RectTransformUtility.RectangleContainsScreenPoint (rectTransform, sp, eventCamera);
		}
	}
}