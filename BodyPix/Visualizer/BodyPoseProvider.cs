using UnityEngine;

namespace mj.gist.tracking.bodyPix {
    public class BodyPoseProvider : MonoBehaviour {
        [SerializeField] private ResourceSet resources = null;
        [SerializeField] private Vector2Int resolution = new Vector2Int(512, 384);

        public GraphicsBuffer KeypointBuffer => detector.KeypointBuffer;
        public RenderTexture MaskTexture => detector.MaskTexture;
        public float Aspect => (float)resolution.x / resolution.y;

        private ImageSource source = null;
        private BodyDetector detector;

        void Start() {
            source = GetComponent<ImageSource>();
            detector = new BodyDetector(resources, resolution.x, resolution.y);
        }

        void LateUpdate() {
            detector.ProcessImage(source.Texture);
        }

        void OnDestroy() {
            detector.Dispose();
        }
    }
}