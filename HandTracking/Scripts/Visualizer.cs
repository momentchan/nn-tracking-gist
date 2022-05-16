using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace mj.gist.tracking.hands {

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
        }
        
        private void LateUpdate() {
            _previewUI.texture = source.Texture;
        }

        protected void OnCameraRender(ScriptableRenderContext context, Camera[] cameras) {
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

        private void OnEnable() {
            if (GraphicsSettings.renderPipelineAsset != null)
                RenderPipelineManager.endFrameRendering += OnCameraRender;
        }

        private void OnDisable() {
            if (GraphicsSettings.renderPipelineAsset != null)
                RenderPipelineManager.endFrameRendering -= OnCameraRender;
        }

        void OnDestroy() {
            Destroy(_material);
        }
    }
}
