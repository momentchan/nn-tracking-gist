using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mj.gist.tracking.Kinect
{
    public class Tracker : MonoBehaviour
    {
        [SerializeField] private TrackerObject objectPrefab;
        public int PlayerId { get; private set; }
        private List<TrackerObject> trackObjects = new List<TrackerObject>();

        public bool IsActive => trackObjects.Where(o => o.IsActive).Count() != 0;

        public List<TrackerData> TrackerData => trackObjects.Select(o => o.Data).ToList();

        public void Setup(int playerId)
        {
            this.PlayerId = playerId;

            for (var i = 0; i < TrackingManager.Instance.TargetJointsCount; i++)
            {
                var o = Instantiate(objectPrefab, transform);
                o.Setup(PlayerId);
                trackObjects.Add(o);
            }
        }

        public void UpdateTracker(int jointId, int uniqueId, Vector3 pos)
        {
            trackObjects[jointId].UpdatePosition(uniqueId, pos);
        }
    }

    public enum JointType : int
    {
        Pelvis = 0,
        SpineNaval = 1,
        SpineChest = 2,
        Neck = 3,
        Head = 4,

        ClavicleLeft = 5,
        ShoulderLeft = 6,
        ElbowLeft = 7,
        WristLeft = 8,
        HandLeft = 9,

        ClavicleRight = 10,
        ShoulderRight = 11,
        ElbowRight = 12,
        WristRight = 13,
        HandRight = 14,

        HipLeft = 15,
        KneeLeft = 16,
        AnkleLeft = 17,
        FootLeft = 18,

        HipRight = 19,
        KneeRight = 20,
        AnkleRight = 21,
        FootRight = 22,

        Nose = 23,
        EyeLeft = 24,
        EarLeft = 25,
        EyeRight = 26,
        EarRight = 27,

        HandtipLeft = 28,
        ThumbLeft = 29,
        HandtipRight = 30,
        ThumbRight = 31,

        Count = 32
    }
}