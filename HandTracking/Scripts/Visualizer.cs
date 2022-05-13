using UnityEngine;
using UnityEngine.UI;
using MediaPipe.BlazePalm;
using Klak.TestTools;

namespace MediaPipe {

    public class Visualizer : MonoBehaviour {
        [SerializeField] RawImage _previewUI = null;
        [SerializeField] Shader _shader = null;

        private HandPosProvider provider;
        private ImageSource source;
        private Material _material;

        void Start() {
            provider = GetComponent<HandPosProvider>();
            source = GetComponent<ImageSource>();

            _material = new Material(_shader);
            _previewUI.texture = source.Texture;
        }

        void OnDestroy() {
            Destroy(_material);
        }

        void OnRenderObject() {
            // Detection buffer
            _material.SetBuffer("_Detections", provider.DetectionBuffer);

            // Copy the detection count into the indirect draw args.
            var ratio = new Vector2(_previewUI.rectTransform.sizeDelta.x / Screen.width, _previewUI.rectTransform.sizeDelta.y / Screen.height);
            _material.SetVector("_CanvasRatio", ratio);
            _material.SetVector("_CanvasOffset", new Vector2(1 - ratio.x, 1 - ratio.y));

            // Bounding box
            _material.SetPass(0);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Triangles, provider.BoxDrawArgs, 0);

            // Key points
            _material.SetPass(1);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Lines, provider.KeyDrawArgs, 0);
        }
    }
}
