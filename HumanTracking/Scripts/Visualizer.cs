using UnityEngine;
using UnityEngine.UI;

namespace NNCam {
    public class Visualizer : MonoBehaviour {
        [SerializeField] private RawImage previewUI;
        [SerializeField] private Type type;

        private HumanMaskProvider provider;

        private void Start() {
            provider = GetComponent<HumanMaskProvider>(); 
        }

        void Update() {
            switch (type) {
                case Type.Mask:
                    previewUI.enabled = true;
                    previewUI.texture = provider.MaskTexture;
                    break;
                case Type.Source:
                    previewUI.enabled = true;
                    previewUI.texture = provider.SourceTexture;
                    break;
                case Type.None:
                default:
                    previewUI.enabled = false;
                    break;
            }
        }
        enum Type { Mask, Source, None }
    }
}