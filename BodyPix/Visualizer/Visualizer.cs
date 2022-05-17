using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace mj.gist.tracking.bodyPix {
    public class Visualizer : MonoBehaviour {
        [SerializeField] private RawImage rawUI = null;
        [SerializeField] private RawImage maskUI = null;
        [SerializeField] private Shader shader;
        [SerializeField] private bool drawSkeleton = false;

        private Material material;
        private RenderTexture mask;
        private BodyPoseProvider provider;
        private ImageSource source = null;

        void Start() {
            provider = GetComponent<BodyPoseProvider>();
            source = GetComponent<ImageSource>();

            var reso = source.OutputResolution;
            mask = new RenderTexture(reso.x, reso.y, 0);

            material = new Material(shader);
        }
        private void LateUpdate() {
            rawUI.texture = source.Texture;
            maskUI.texture = mask;
            Graphics.Blit(provider.MaskTexture, mask, material, 0);
        }

        protected void OnCameraRender(ScriptableRenderContext context, Camera[] cameras) {
            if (!drawSkeleton) return;

            material.SetBuffer("_Keypoints", provider.KeypointBuffer);
            material.SetFloat("_Aspect", provider.Aspect);

            material.SetPass(1);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, Body.KeypointCount);

            material.SetPass(2);
            Graphics.DrawProceduralNow(MeshTopology.Lines, 2, 12);
        }

        private void OnEnable() {
            if (GraphicsSettings.renderPipelineAsset != null)
                RenderPipelineManager.endFrameRendering += OnCameraRender;
        }

        private void OnDisable() {
            if (GraphicsSettings.renderPipelineAsset != null)
                RenderPipelineManager.endFrameRendering -= OnCameraRender;
        }

        private void OnDestroy() {
            Destroy(material);
            Destroy(mask);
        }
    }
}