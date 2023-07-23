using System.Collections;
using System.Collections.Generic;
using mj.gist;
using UnityEngine;

namespace mj.tracking {
    public class TrackerManager : SingletonMonoBehaviour<TrackerManager> {
        private TrackerData[] trackers;

        private void Start() {
            trackers = new TrackerData[20];
        }

        private void UpdateRandomTracker(Vector2 pos, uint isMoving) {
            //var actives = 0;
            //var minId = -1;
            //var minDist = 1e5;
            //for (var i = 0; i < trackers.Length; i++) {
            //    var tracker = trackers[i];
            //    var dist = (pos - tracker.pos).magnitude;
            //    if (dist < minDist) { minId = i; minDist = dist; }
            //    if (tracker.active == 1) { actives = i; }
            //}

            //if (minDist < DistanceThreshold) {
            //    var d = GetUpdatedTracker(minId, pos, isMoving, ref randomTrackers, RANDOM_SMOOTH_FACTOR);
            //    randomTrackers[minId] = d;
            //}
            //else {
            //    var newID = (actives + 1) % randomTrackers.Length;
            //    var d = GetUpdatedTracker(newID, pos, isMoving, ref randomTrackers);
            //    randomTrackers[newID] = d;
            //}
        }
        //private TrackerData GetUpdatedTracker(int id, Vector2 pos, uint isMoving, ref TrackerData[] trackers, float smooth = 1) {
        //    var prevPos = trackers[id].pos;
        //    var prevRatio = trackers[id].activeRatio;

        //    TrackerData d;
        //    d.active = 1;
        //    d.pos = Vector2.Lerp(prevPos, pos, smooth);
        //    d.dis = d.pos - prevPos;
        //    d.dir = d.dis.normalized;
        //    d.isMoving = isMoving;
        //    d.lastUpdateTime = Time.time;
        //    //d.activeRatio = Mathf.Clamp(prevRatio + Time.deltaTime / ALIVE_DURATION, 0, 1);

        //    return d;
        //}

    }
}