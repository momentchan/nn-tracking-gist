using UnityEngine;

namespace mj.gist.tracking
{
    [ExecuteInEditMode]
    public class InteractiveVolume : MonoBehaviour
    {
        private TrackingManager manager => TrackingManager.Instance;

        private Vector3 bottomLeft => transform.position - transform.right * transform.localScale.x * 0.5f;
        private Vector3 topRight => bottomLeft + Vector3.Scale(new Vector3(1, 1, -1), transform.localScale);

        void Update()
        {
            transform.localScale = manager.InteractiveRange;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var s = new Vector3(Mathf.Lerp(bottomLeft.x, topRight.x, manager.RangeRemapX.x),
                                Mathf.Lerp(bottomLeft.y, topRight.y, manager.RangeRemapY.x),
                                Mathf.Lerp(bottomLeft.z, topRight.z, manager.RangeRemapZ.x));
            var e = new Vector3(Mathf.Lerp(bottomLeft.x, topRight.x, manager.RangeRemapX.y),
                                Mathf.Lerp(bottomLeft.y, topRight.y, manager.RangeRemapY.y),
                                Mathf.Lerp(bottomLeft.z, topRight.z, manager.RangeRemapZ.y));

            DrawBox(bottomLeft, topRight);
            DrawBox(s, e);
        }

        private void DrawBox(Vector3 p1, Vector3 p2)
        {
            var c = (p1 + p2) * 0.5f;
            var s = (p2 - p1);
            s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
            Gizmos.DrawWireCube(c, s);
        }
#endif
    }
}