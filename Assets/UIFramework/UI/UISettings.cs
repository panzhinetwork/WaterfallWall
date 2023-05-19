using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework {

    [CreateAssetMenu (fileName = "UISettings", menuName = "UIFramework/UI Settings")]
    public class UISettings : ScriptableObject {

        [SerializeField]
        private UIPack _packPrefab = default;

        [SerializeField]
        private List<GameObject> _viewsToRegister = default;

        [SerializeField]
        private bool _deactivateViewGOs = true;

        public UIPack CreatePackInstance (bool createAndRegisterView = true) {
            var pack = Instantiate (_packPrefab);

            if (createAndRegisterView) {
                foreach (var prefab in _viewsToRegister) {
                    var go = Instantiate (prefab);
                    var view = go.GetComponent<IUIView> ();

                    if (view != null) {
                        pack.RegisterView (view);
                        if (_deactivateViewGOs && go.activeSelf) {
                            go.SetActive (false);
                        }
                    } else {
                        Debug.LogError ("[UISettings] View doesn't contain a component of IUIView! Skipping " + go.name);
                    }
                }
            }

            return pack;
        }

        private void OnValidate () {
            List<GameObject> toRemove = new List<GameObject> ();
            for (int i = 0; i < _viewsToRegister.Count; i++) {
                var view = _viewsToRegister[i].GetComponent<IUIView> ();
                if (view == null) {
                    toRemove.Add (_viewsToRegister[i]);
                }
            }

            if (toRemove.Count > 0) {
                Debug.LogError ("[UISettings] Some view prefabs to register do not contain IUIView component! Removing.");
                foreach (var obj in toRemove) {
                    Debug.LogError ("[UISettings] Remove " + obj.name + " from " + name);
                    _viewsToRegister.Remove (obj);
                }
            }
        }
    }

}