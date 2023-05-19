using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFramework.UGUI_Extension {

	public interface IScrollRectDataSource {
		int GetItemCount ();
		void PrepareItemAtIndex (PoolingScrollRect scroll, GameObject item, int itemIndex);
	}

	[RequireComponent (typeof (ScrollRect))]
	public class PoolingScrollRect : MonoBehaviour, IDragHandler, IEndDragHandler {

		private const float SNAP_POINTER_SPEED_THRESHOLD = 500f;

		[System.Serializable]
		public class ScrollSnapEvent : UnityEvent<int> { }

		public enum LayoutDirection {
			Vertical,
			Horizontal
		}

		public IScrollRectDataSource DataSource;

		[SerializeField]
		private GameObject _itemPrefab = default;

		[SerializeField]
		private float _itemPadding = 20;

		[SerializeField]
		private float _contentMargin = 40;

		[SerializeField]
		private LayoutDirection _layoutDirection = LayoutDirection.Vertical;

		[SerializeField]
		private bool _reloadDataOnStart = true;

		[SerializeField]
		private float _scrollNormalizedSpeed = 7.5f;

		[SerializeField]
		private bool _snapCenterItemWhenStop = false;

		public ScrollSnapEvent onSnapItemChanged;

		public int SnapItemIndex { get; private set; }

		public RectTransform Content {
			get { return _scrollRect.content; }
		}

		private ScrollRect _scrollRect;
		private RectTransform _contentRect;

		private int _totalItemCount;
		private Vector2 _itemSize = Vector2.zero;

		private Stack<GameObject> _pool = new Stack<GameObject> ();
		private Dictionary<int, GameObject> _visibleItems = new Dictionary<int, GameObject> ();
		private RangeInt _visibleRange = new RangeInt ();

		private bool _started;
		private Vector2 _scrollRectSize;
		private Vector2 _contentSize;

		/********************************************************
        注意水平滚动和垂直滚动的区别：
        水平滚动 ContentRect Position x 为负数。
        垂直滚动 ContentRect Position y 为正数。
        而且两者绝对值相等。
        ********************************************************/
		private Vector2 _contentRectPosition;
		private Vector2 _firstItemCenterPosition;
		private float _firstItemEdgePosition;
		private float _itemStepOffset;
		private int _scrollingIndex;

		delegate float NumberConvertDelegate (float offset);
		private NumberConvertDelegate _positionConverter;
		private NumberConvertDelegate _deltaOffsetConverter;

		private float _lerpNormalizedTarget;
		private bool _lerping;
		private DeltaSmoother _velocitySmoother;

		public void ReloadData () {
			if (DataSource == null) {
				Debug.LogError ("[PoolingScrollRect] Try to load data but DataSource is null!");
				return;
			}

			_totalItemCount = DataSource.GetItemCount ();

			//  The RectTransform is not correct if Start() is not run yet! Ignore reloading data.
			if (!_started) {
				return;
			}

			_contentSize = _scrollRectSize;
			_itemStepOffset = _itemSize[_scrollingIndex] + _itemPadding;

			if (_totalItemCount > 0) {
				var contentLength = _totalItemCount * _itemStepOffset - _itemPadding + _contentMargin + _contentMargin;
				_contentSize[_scrollingIndex] = Mathf.Max (contentLength, _contentSize[_scrollingIndex]);
			}
			var axis = (_layoutDirection == LayoutDirection.Vertical) ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal;
			_contentRect.SetSizeWithCurrentAnchors (axis, _contentSize[_scrollingIndex]);
			_firstItemCenterPosition = Vector2.zero;
			_firstItemCenterPosition[_scrollingIndex] = _positionConverter.Invoke (_contentSize[_scrollingIndex] * 0.5f - _contentMargin - _itemSize[_scrollingIndex] * 0.5f);
			_firstItemEdgePosition = _positionConverter.Invoke (_contentSize[_scrollingIndex] * 0.5f - _contentMargin);
			//Debug.Log("First item edge: " + _firstItemEdgePosition);
			//Debug.Log("Content size: " + _contentSize);

			ClearItems ();
			LayoutItems ();
		}

		public void ScrollToItemAtIndex (int index, bool animated = true) {

			if (_started) {
				SnapItemIndex = Mathf.Clamp (index, 0, _totalItemCount - 1);
			} else {
				SnapItemIndex = index;
				return;
			}

			float target;
			if (_snapCenterItemWhenStop) {
				target = CalculateNormalizedPositionToCenterItem (SnapItemIndex);
			} else {
				target = CalculateNormalizedPositionToShowItemAtHead (SnapItemIndex);
			}
			if (animated) {
				_lerping = true;
				_lerpNormalizedTarget = target;
			} else {
				if (_scrollRect.vertical) {
					_scrollRect.verticalNormalizedPosition = target;
				} else if (_scrollRect.horizontal) {
					_scrollRect.horizontalNormalizedPosition = target;
				}
			}
			onSnapItemChanged.Invoke (SnapItemIndex);
		}

		public GameObject GetVisibleItem (int index) {
			if (_visibleItems.ContainsKey (index)) {
				return _visibleItems[index];
			}
			return null;
		}

		public void OnDrag (PointerEventData eventData) {
			if (!_snapCenterItemWhenStop) {
				return;
			}

			if (_velocitySmoother == null) {
				_velocitySmoother = new DeltaSmoother ();
				_velocitySmoother.Create ();
			}
			_velocitySmoother.Handle (eventData.delta, Time.deltaTime);

		}

		public void OnEndDrag (PointerEventData eventData) {
			if (!_snapCenterItemWhenStop) {
				return;
			}

			//  根据滑动的速度判断是否翻页
			var snapIndex = CalculateViewPortCenterNearestIndex ();
			_velocitySmoother.Handle (eventData.delta, Time.deltaTime);
			var velocity = _velocitySmoother.CalculateSmoothVelocity ();
			_velocitySmoother.Reset();
			if (Mathf.Abs (velocity[_scrollingIndex]) > SNAP_POINTER_SPEED_THRESHOLD) {
				if (velocity[_scrollingIndex] > SNAP_POINTER_SPEED_THRESHOLD) {
					snapIndex = Mathf.Max (snapIndex - 1, 0);
				} else {
					snapIndex = Mathf.Min (snapIndex + 1, _totalItemCount - 1);
				}
			}
			ScrollToItemAtIndex (snapIndex, true);
		}

		#region -- Layout Items --

		/// <summary>
		/// 获取复用池中的 GameObject。非 active 状态，但不要调用 SetActive()，这个类会自动管理该状态。
		/// </summary>
		/// <returns>The pooled item.</returns>
		private GameObject DequeuePooledObject () {
			if (_pool.Count > 0) {
				return _pool.Pop ();
			}
			return null;
		}

		private void ClearItems () {
			foreach (var item in _visibleItems) {
				var go = item.Value;
				go.SetActive (false);
				_pool.Push (go);
			}
			_visibleItems.Clear ();
			_visibleRange.start = 0;
			_visibleRange.length = 0;
		}

		private void LayoutItems () {
			if (_totalItemCount > 0) {
				_contentRectPosition = _contentRect.anchoredPosition;
				float viewPortPositionMin = CalculateViewPortPositionMin ();
				float viewPortPositionMax = CalculateViewPortPositionMax ();

				int minIndex = Mathf.Max (CalculateItemIndex (viewPortPositionMin), 0);
				int maxIndex = Mathf.Min (CalculateItemIndex (viewPortPositionMax), _totalItemCount - 1);
				//Debug.Log("View port min position: " + viewPortPositionMin + "; max position: " + viewPortPositionMax);
				//Debug.Log("Min index: " + minIndex + "; Max: " + maxIndex);

				// Recycle invisible items.
				int visibleStart = _visibleRange.start;
				int visibleEnd = _visibleRange.end;
				for (int i = visibleStart; i < visibleEnd; ++i) {
					if (i < minIndex || i > maxIndex) {
						// Out of new visible range. Recycle.
						RecycleItem (i);
					}
				}

				// Present new visible cells.
				for (int i = minIndex; i <= maxIndex; ++i) {
					if (i < visibleStart || i >= visibleEnd) {
						// Become visible now.
						PresentItem (i);
					}
				}

				// Update visible range.
				_visibleRange.start = minIndex;
				_visibleRange.length = maxIndex - minIndex + 1;
			}
		}

		private void RecycleItem (int index) {
			GameObject go = null;
			if (_visibleItems.TryGetValue (index, out go)) {
				go.SetActive (false);
				_pool.Push (go);
				_visibleItems.Remove (index);
			} else {
				Debug.LogError ("[PoolingScrollRect] Failed to recycle item at index: " + index);
			}
		}

		private void PresentItem (int index) {
			GameObject item = DequeuePooledObject ();
			if (item == null) {
				item = Instantiate (_itemPrefab, _contentRect);
			}

			DataSource.PrepareItemAtIndex (this, item, index);

			Vector2 pos = Vector2.zero;
			var rectTransform = item.GetComponent<RectTransform> ();
			if (rectTransform != null) {
				pos[_scrollingIndex] = CalculateItemPosition (index);
				rectTransform.anchoredPosition = pos;
			}
			item.SetActive (true);
			_visibleItems.Add (index, item);
		}

		private void ScrollDidChanged (Vector2 scrollMove) {
			LayoutItems ();
		}

		private float CalculateViewPortPositionMin () {
			return _positionConverter.Invoke (_contentSize[_scrollingIndex] * 0.5f - _positionConverter (_contentRectPosition[_scrollingIndex]));
		}

		private float CalculateViewPortPositionMax () {
			return _positionConverter.Invoke (_contentSize[_scrollingIndex] * 0.5f - _scrollRectSize[_scrollingIndex] - _positionConverter (_contentRectPosition[_scrollingIndex]));
		}

		private float CalculateItemPosition (int index) {
			return _firstItemCenterPosition[_scrollingIndex] + _deltaOffsetConverter (_itemStepOffset * index);
		}

		private int CalculateItemIndex (float position) {
			var offset = _deltaOffsetConverter (position - _firstItemEdgePosition);
			var itemNumber = offset / _itemStepOffset;
			return Mathf.FloorToInt (itemNumber);
		}

		#endregion // Layout Items

		#region -- Offset Calculate --

		private static float ConvertVerticalPosition (float offset) {
			return offset;
		}

		private static float ConvertVerticalDeltaOffset (float delta) {
			return -delta;
		}

		private static float ConvertHorizontalPosition (float offset) {
			return -offset;
		}

		private static float ConvertHorizontalDeltaOffset (float delta) {
			return delta;
		}

		#endregion // Offset Calculate

		#region -- Snap --

		private int CalculateViewPortCenterNearestIndex () {
			var normalizedPosition = _scrollRect.normalizedPosition[_scrollingIndex];
			var index = Mathf.FloorToInt (normalizedPosition / (1f / (_totalItemCount - 1)));
			if (index < 0) {
				return 0;
			} else if (index >= _totalItemCount - 1) {
				return _totalItemCount - 1;
			}
			var pos = CalculateNormalizedPositionToCenterItem (index);
			var nextPos = CalculateNormalizedPositionToCenterItem (index + 1);
			if (Mathf.Abs (pos - normalizedPosition) < Mathf.Abs (nextPos - normalizedPosition)) {
				return index;
			} else {
				return index + 1;
			}
		}

		private float CalculateNormalizedPositionToCenterItem (int index) {
			var pos = 1f / (_totalItemCount - 1) * index;
			if (_layoutDirection == LayoutDirection.Vertical) {
				pos = 1f - pos;
			}
			return pos;
		}

		private float CalculateNormalizedPositionToShowItemAtHead (int index) {
			var itemPosition = CalculateItemPosition (index) - _deltaOffsetConverter (_itemSize[_scrollingIndex] * 0.5f + _itemPadding);
			if (_layoutDirection == LayoutDirection.Horizontal) {
				// Normalized position starts from left, range is (0, 1)
				var itemOffset = itemPosition - (-_contentSize[_scrollingIndex] * 0.5f);
				return Mathf.Clamp01 (itemOffset / (_contentSize[_scrollingIndex] - _scrollRectSize[_scrollingIndex]));
			} else {
				// Normalized position starts from bottom, range is (0, 1)
				var itemOffset = itemPosition - (_contentSize[_scrollingIndex] * -0.5f + _scrollRectSize[_scrollingIndex]);
				return Mathf.Clamp01 (itemOffset / (_contentSize[_scrollingIndex] - _scrollRectSize[_scrollingIndex]));
			}
		}

		#endregion // Snap

		#region -- MonoBehaviour --

		private void Awake () {
			_scrollRect = GetComponent<ScrollRect> ();
			_scrollRect.onValueChanged.AddListener (ScrollDidChanged);
			_contentRect = _scrollRect.content;

			if (_layoutDirection == LayoutDirection.Vertical) {
				_scrollingIndex = 1;
				_positionConverter = ConvertVerticalPosition;
				_deltaOffsetConverter = ConvertVerticalDeltaOffset;
			} else {
				_scrollingIndex = 0;
				_positionConverter = ConvertHorizontalPosition;
				_deltaOffsetConverter = ConvertHorizontalDeltaOffset;
			}
		}

		private void Start () {
			_started = true;

			var scrollRectTransform = _scrollRect.GetComponent<RectTransform> ();
			_scrollRectSize = scrollRectTransform.rect.size;
			//Debug.Log("Scroll view size: " + _scrollRectSize);

			var itemRect = _itemPrefab.GetComponent<RectTransform> ();
			_itemSize.x = itemRect.rect.width;
			_itemSize.y = itemRect.rect.height;

			if (_reloadDataOnStart) {
				ReloadData ();
			}
			if (SnapItemIndex != 0) {
				ScrollToItemAtIndex (SnapItemIndex, false);
			}
		}

		private void OnDestroy () {
			_pool.Clear ();
			if (_scrollRect != null) {
				_scrollRect.onValueChanged.RemoveListener (ScrollDidChanged);
			}
		}

		private void Update () {
			if (_lerping) {
				if (_scrollRect.vertical) {
					_scrollRect.verticalNormalizedPosition = Mathf.Lerp (_scrollRect.verticalNormalizedPosition, _lerpNormalizedTarget, _scrollNormalizedSpeed * Time.deltaTime);
					if (Mathf.Abs (_scrollRect.verticalNormalizedPosition - _lerpNormalizedTarget) < 0.001f) {
						_scrollRect.verticalNormalizedPosition = _lerpNormalizedTarget;
						_lerping = false;
					}
				} else if (_scrollRect.horizontal) {
					_scrollRect.horizontalNormalizedPosition = Mathf.Lerp (_scrollRect.horizontalNormalizedPosition, _lerpNormalizedTarget, _scrollNormalizedSpeed * Time.deltaTime);
					if (Mathf.Abs (_scrollRect.horizontalNormalizedPosition - _lerpNormalizedTarget) < 0.001f) {
						_scrollRect.horizontalNormalizedPosition = _lerpNormalizedTarget;
						_lerping = false;
					}
				}
			}
		}

#if UNITY_EDITOR

		private void OnValidate () {
			var scrollRect = GetComponent<ScrollRect> ();
			var content = scrollRect.content;
			if (content == null) {
				Debug.LogError ("[PoolingScrollRect] The content field of ScrollRect is null! Please set Content of ScrollRect in Inspector.");
			} else {
				if (_layoutDirection == LayoutDirection.Vertical) {
					content.anchorMin = new Vector2 (0f, 1f);
					content.anchorMax = new Vector2 (1f, 1f);
					content.pivot = new Vector2 (0.5f, 1f);
					content.offsetMin = new Vector2 (0f, -scrollRect.GetComponent<RectTransform> ().rect.height);
					content.offsetMax = new Vector2 (0f, 0f);
				} else if (_layoutDirection == LayoutDirection.Horizontal) {
					content.anchorMin = new Vector2 (0f, 0f);
					content.anchorMax = new Vector2 (0f, 1f);
					content.pivot = new Vector2 (0f, 0.5f);
					content.offsetMin = new Vector2 (0f, 0f);
					content.offsetMax = new Vector2 (scrollRect.GetComponent<RectTransform> ().rect.width, 0f);
				}
			}

			if (_itemPrefab == null) {
				Debug.LogError ("[PoolingScrollRect] The item prefab is null!");
			} else {
				var itemRect = _itemPrefab.GetComponent<RectTransform> ();
				if (itemRect == null) {
					Debug.LogError ("[PoolingScrollRect] The item prefab does not contain component RectTransform!");
				} else {
					var center = new Vector2 (0.5f, 0.5f);
					if (Vector2.Distance (center, itemRect.anchorMin) > 0.1f || Vector2.Distance (center, itemRect.anchorMax) > 0.1f || Vector2.Distance (center, itemRect.pivot) > 0.1f) {
						itemRect.anchorMin = center;
						itemRect.anchorMax = center;
						itemRect.pivot = center;
						Debug.LogWarningFormat ("[PoolingScrollRect]-{0} The item prefab RectTransform must anchor center! Already changed!", gameObject.name);
					}
				}
			}

			if (_layoutDirection == LayoutDirection.Vertical) {
				scrollRect.vertical = true;
				scrollRect.horizontal = false;
			} else {
				scrollRect.vertical = false;
				scrollRect.horizontal = true;
			}
		}

#endif

		#endregion // MonoBehaviour

	}

	public class DeltaSmoother {

		// 历史数量
		private static int HISTORY_COUNT = 5;

		// 当前记录的历史数
		private int _currentCount = 0;

		// 历史（速度的过往记录）
		private Vector2[] _history;
		private float[] _timeHistory;

		// ------------------------------------------------------------------------ //

		public void Create () {
			_history = new Vector2[HISTORY_COUNT];
			_timeHistory = new float[HISTORY_COUNT];
		}

		// 处理速度（返回平滑处理后的速度）
		public void Handle (Vector2 delta, float time) {
			// 将最新的速度追加到历史中
			int index = _currentCount % HISTORY_COUNT;

			_history[index] = delta;
			_timeHistory[index] = time;

			_currentCount++;
		}

		public Vector2 CalculateSmoothVelocity () {
			// 将过去的几帧速度进行平均化，平滑处理

			int smoothCount = Mathf.Min (HISTORY_COUNT, _currentCount);

			Vector2 totalDelta = Vector2.zero;
			float totalTime = 0f;

			for (int i = 0; i < smoothCount; i++) {

				totalDelta += this._history[i];
				totalTime += this._timeHistory[i];
			}

			return totalDelta / totalTime;
		}

		// 重置（清空历史）
		public void Reset () {
			_currentCount = 0;
		}
	}
}