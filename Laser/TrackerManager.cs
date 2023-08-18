using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using mj.gist;
using Osc;
using UnityEngine;

namespace mj.gist.tracking.Laser
{
    public class TrackerManager : SingletonMonoBehaviour<TrackerManager>
    {
        public GraphicsBuffer TrackerBuffer { get; private set; }
        public TrackerData MouseData { get; private set; }

        [SerializeField] private int trackerNum = 20;
        [SerializeField] private float distanceThreshold = 0.1f;

        private static readonly float RANDOM_SMOOTH_FACTOR = 0.5f;
        private static readonly float ALIVE_DURATION = 1f;

        private TrackerData[] trackers;

        public void OnReceivePoint(OscPort.Capsule c)
        {
            try
            {
                var msg = c.message;
                var pos = new Vector2(Mathf.Clamp01((float)msg.data[0]), Mathf.Clamp01((float)msg.data[1]));
                var isMoving = (uint)Mathf.CeilToInt((float)msg.data[2]);
                UpdateTrackerData(pos, isMoving);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Start()
        {
            trackers = new TrackerData[trackerNum];
            TrackerBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, trackerNum, Marshal.SizeOf(typeof(TrackerData)));
        }


        private void Update()
        {
#if UNITY_EDITOR
            UpdateMouseTrackerData();
#endif
            UpdateTrackers();
        }
        private void UpdateMouseTrackerData()
        {
            var pos = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
            var data = new TrackerData
            {
                active = 1,
                isMoving = 1,
                pos = pos,
                dis = pos - MouseData.pos,
                dir = (pos - MouseData.pos).normalized,
                lastUpdateTime = Time.time
            };
            MouseData = data;
        }

        private void UpdateTrackers()
        {
            for (var i = 0; i < trackers.Length; i++)
            {
                var d = trackers[i];
                if (d.lastUpdateTime < Time.time - ALIVE_DURATION)
                {
                    d.active = 0;
                    d.activeRatio = Mathf.Clamp01(d.activeRatio - Time.deltaTime / ALIVE_DURATION);
                    trackers[i] = d;
                }
            }
            TrackerBuffer.SetData(trackers);
        }

        private void UpdateTrackerData(Vector2 pos, uint isMoving)
        {
            var actives = 0;
            var minId = -1;
            var minDist = 1e5;
            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var dist = (pos - tracker.pos).magnitude;
                if (dist < minDist) { minId = i; minDist = dist; }
                if (tracker.active == 1) { actives = i; }
            }

            if (minDist < distanceThreshold)
            {
                var d = GetUpdatedTracker(minId, pos, isMoving);
                trackers[minId] = d;
            }
            else
            {
                var newID = (actives + 1) % trackers.Length;
                var d = GetUpdatedTracker(newID, pos, isMoving);
                trackers[newID] = d;
            }
        }

        private TrackerData GetUpdatedTracker(int id, Vector2 pos, uint isMoving)
        {
            var prevPos = trackers[id].pos;
            var prevRatio = trackers[id].activeRatio;

            TrackerData d;
            d.active = 1;
            d.pos = Vector2.Lerp(prevPos, pos, RANDOM_SMOOTH_FACTOR);
            d.dis = d.pos - prevPos;
            d.dir = d.dis.normalized;
            d.isMoving = isMoving;
            d.lastUpdateTime = Time.time;
            d.activeRatio = Mathf.Clamp(prevRatio + Time.deltaTime / ALIVE_DURATION, 0, 1);
            return d;
        }

        private void OnDestroy()
        {
            TrackerBuffer.Dispose();
        }
    }
}