using Klak.TestTools;
using UnityEngine;

namespace NNCam {
    public class HumanMaskProvider : MonoBehaviour {
        public Texture MaskTexture => output;
        public Texture SourceTexture => source.Texture;

        [SerializeField] protected ResourceSet resource;
        [SerializeField] protected Shader shader;
        [SerializeField] protected Shader mirrorShader;

        protected SegementationFilter filter;

        [SerializeField] private RenderTexture output;
        [SerializeField] private bool mirror;

        protected ImageSource source;

        private Material material;
        private Material mirrorMat;

        protected virtual void Start() {
            source = GetComponent<ImageSource>();
            filter = new SegementationFilter(resource, 512, 384);
            material = new Material(shader);
            mirrorMat = new Material(mirrorShader);
        }

        protected virtual void Update() {
            filter.ProcessImage(source.Texture);

            Graphics.SetRenderTarget(output);

            material.SetTexture("_BodyPixTexture", filter.MaskTexture);
            material.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 3, 1);

            if (mirror) {
                var temp = RenderTexture.GetTemporary(output.descriptor);
                Graphics.Blit(output, temp, mirrorMat);
                Graphics.Blit(temp, output);
                temp.Release();
            }
        }

        protected virtual void OnDestroy() {
            filter?.Dispose();
            Destroy(material);
        }
    }
}