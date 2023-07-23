using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Osc;
using PrefsGUI;
using PrefsGUI.RapidGUI;
using UnityEngine;

namespace mj.gist.tracking
{
    public class TrackingManager : SingletonMonoBehaviour<TrackingManager>, IGUIUser
    {
        public static readonly float IDLE_TIME = 2f;
        public int TargetJointsCount => targetJoints.Count;
        public GraphicsBuffer TrackerBuffer => trackerBuffer;
        public int TotalTrackerObjectsCount => maxTrackers * targetJoints.Count;

        [SerializeField] private InteractiveVolume volume;
        [SerializeField] private Tracker tracker;
        [SerializeField] private List<JointType> targetJoints = new List<JointType>();

        private List<Tracker> trackers = new List<Tracker>();

        private GraphicsBuffer trackerBuffer;
        private List<TrackerData> trackerData = new List<TrackerData>();

        public Transform VolumeTrans => volume.transform;

        public float DebugSize => debugSize;
        public Vector3 InteractiveRange => interactiveRange;
        public Vector2 RangeRemapX => rangeRemapX;
        public Vector2 RangeRemapY => rangeRemapY;
        public Vector2 RangeRemapZ => rangeRemapZ;

        #region GUI
        public bool DebugMode => trackerDebug;
        private PrefsInt maxTrackers = new PrefsInt("MaxTrackers", 20);
        private PrefsBool trackerDebug = new PrefsBool("TrackerDebug", false);
        private PrefsFloat debugSize = new PrefsFloat("DebugSize", 1);
        private PrefsVector3 interactiveRange = new PrefsVector3("InteractiveRange(m)", new Vector3(2f, 2f, 3f));
        private PrefsVector2 rangeRemapX = new PrefsVector2("RangeRemapX (from, to)", new Vector2(0, 1f));
        private PrefsVector2 rangeRemapY = new PrefsVector2("RangeRemapY (from, to)", new Vector2(0, 1f));
        private PrefsVector2 rangeRemapZ = new PrefsVector2("RangeRemapZ (from, to)", new Vector2(0, 1f));

        public string GetName() => "Tracker";

        public void ShowGUI()
        {
            maxTrackers.DoGUI();
            trackerDebug.DoGUI();
            debugSize.DoGUI();
            interactiveRange.DoGUI();
            rangeRemapX.DoGUI();
            rangeRemapY.DoGUI();
            rangeRemapZ.DoGUI();
        }

        public void SetupGUI() { }
        #endregion

        private void Start()
        {
            for (var i = 0; i < maxTrackers; i++)
            {
                var tracker = Instantiate(this.tracker, transform);
                tracker.Setup(i);
                trackers.Add(tracker);
            }

            trackerBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, TotalTrackerObjectsCount, Marshal.SizeOf(typeof(TrackerData)));
        }

        private void Update()
        {
            UpdateBuffer();
        }

        private void UpdateBuffer()
        {
            trackerData.Clear();
            trackers.ForEach(t => trackerData.AddRange(t.TrackerData));
            trackerBuffer.SetData(trackerData.ToArray());
        }

        public void OscMessageReceived(OscPort.Capsule e)
        {
            var uniqueId = (int)e.message.data[0];
            var playerId = (int)e.message.data[1];
            var type = (int)e.message.data[2];
            if (!targetJoints.Contains((JointType)type)) return;

            var jointId = (int)e.message.data[3];

            var x = (float)e.message.data[4];
            var y = (float)e.message.data[5];
            var z = (float)e.message.data[6];
            var pos = new Vector3(x, y, -z);

            //Debug.Log($"{uniqueId} {playerId} {type} {pos}");

            var tracker = trackers[playerId];
            tracker.UpdateTracker(jointId, uniqueId, pos);
        }
    }
}