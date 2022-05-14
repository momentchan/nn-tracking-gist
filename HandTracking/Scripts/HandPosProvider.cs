using UnityEngine;

namespace mj.gist.tracking.hands {
    public class HandPosProvider : MonoBehaviour {
        [SerializeField] ResourceSet _resources = null;
        protected ImageSource source;

        PalmDetector _detector;

        ComputeBuffer _boxDrawArgs;
        ComputeBuffer _keyDrawArgs;

        public ComputeBuffer DetectionBuffer => _detector.DetectionBuffer;
        public ComputeBuffer BoxDrawArgs => _boxDrawArgs;
        public ComputeBuffer KeyDrawArgs => _keyDrawArgs;

        void Start() {
            source = GetComponent<ImageSource>();
            _detector = new PalmDetector(_resources);

            var cbType = ComputeBufferType.IndirectArguments;
            _boxDrawArgs = new ComputeBuffer(4, sizeof(uint), cbType);
            _keyDrawArgs = new ComputeBuffer(4, sizeof(uint), cbType);
            _boxDrawArgs.SetData(new[] { 6, 0, 0, 0 });
            _keyDrawArgs.SetData(new[] { 24, 0, 0, 0 });
        }

        void OnDestroy() {
            _detector.Dispose();
            _boxDrawArgs.Dispose();
            _keyDrawArgs.Dispose();
        }

        void LateUpdate() {
            _detector.ProcessImage(source.Texture);
            _detector.SetIndirectDrawCount(_boxDrawArgs);
            _detector.SetIndirectDrawCount(_keyDrawArgs);
        }
    }
}