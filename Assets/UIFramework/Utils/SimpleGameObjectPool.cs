using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework {

    /// <summary>
    /// Simple but fast game object pool.
    /// 因为没有用泛型，在一千数量的条件下，性能比用泛型的快一倍。建议搭配 GetComponet() 使用。
    /// </summary>
    public class SimpleGameObjectPool {

        public GameObject prefab;
        public bool allowGrow = true;

        // 数组会比 List 更快，但用 List 足够快了。
        private List<GameObject> _items;
        private GameObject _firstItem;
        private Transform _parent;

        /// <summary>
        /// 用 prefab 初始化 pool。
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        public SimpleGameObjectPool(GameObject prefab, Transform parent = null, int initSize = 0) {
            _items = new List<GameObject>();
            this.prefab = prefab;
            _parent = parent;

            for (int i=0; i<initSize; i++) {
                var item = CreateItem();
                item.SetActive(false);
            }
        }

        /// <summary>
        /// 获取复用池中的 GameObject. 还需要另外调用 GameObject.SetActive()。
        /// </summary>
        /// <returns>The pooled object.</returns>
        public GameObject GetPooledObject() {
            if (_firstItem != null) {
                var item = _firstItem;
                _firstItem = null;
                return item;
            }

            int count = _items.Count;
            for (int i=0; i<count; i++) {
                var item = _items[i];
                if (!item.activeInHierarchy) {
                    return item;
                }
            }

            if (allowGrow) {
                return CreateItem();
            }

            return null;
        }

        /// <summary>
        /// 归还 GameObject 到复用池中。
        /// </summary>
        /// <param name="element">Element.</param>
        public void ReturnObjectToPool(GameObject element) {
            element.SetActive(false);

            if (_firstItem == null) {
                _firstItem = element;
            }
        }

        /// <summary>
        /// 清空所有的 GameObject。
        /// </summary>
        public void Clear() {
            int count = _items.Count;
            for (int i=0; i<count; i++) {
                var item = _items[i];
                if (item != null) {
                    GameObject.Destroy(item);
                }
            }
            _items.Clear();

            if (_firstItem != null) {
                GameObject.Destroy(_firstItem);
                _firstItem = null;
            }
        }

        /// <summary>
        /// 把所有的 GameObject 归还入池。
        /// </summary>
        public void ResetAllObjectsToPool() {
            int count = _items.Count;
            for (int i=0; i<count; i++) {
                _items[i].SetActive(false);
            }
        }

        private GameObject CreateItem() {
            var item = GameObject.Instantiate(prefab, _parent);
            _items.Add(item);
            return item;
        }

    }

}