using System.Collections.Generic;
using UnityEngine;

public static class GlobalEventSystem {

    // for debugging purposes
    public static Dictionary<string, (HashSet<string>, System.Type)> GetMapperStatus() {
        var status = new Dictionary<string, (HashSet<string>, System.Type)>();
        foreach (var pair in mapper)
        {
            var actionStatusSet = new HashSet<string>();
            foreach (var action in pair.Value.Item1)
            {
                var actionStatus = "NULL/Destroyed";
                // Debug.Log(action.GetType().GetProperty("Target").GetValue(action, null));
                var actionOrigin = (MonoBehaviour)(action.GetType().GetProperty("Target").GetValue(action, null));
                if (actionOrigin != null) {
                    var methodName = action.GetType().GetProperty("Method").GetValue(action, null);
                    actionStatus = actionOrigin.name + "->"  + actionOrigin.GetType().Name + "->" + methodName;
                }

                actionStatusSet.Add(actionStatus);
            }

            status.Add(pair.Key, (actionStatusSet, pair.Value.Item2));
        }

        return status;
    }

    // object of hashset must be System.Action that has a monobehaviour target
    private static Dictionary<string, (HashSet<object>, System.Type typeID)> mapper 
        = new Dictionary<string, (HashSet<object>, System.Type typeID)>();


    // add listener component to gameobject and register into the mapper
    public static void AddListener<T>(string eventKey, System.Action<T> action) {
        // action's target must be a monobehavior
        if (action.Target is MonoBehaviour == false) {
            Debug.LogError("Action's target object must be monobehavior.");
            return;
        }

        // create a new event if it doesnt exist
        if (!mapper.ContainsKey(eventKey)) {
            var typeID = typeof(T);
            mapper.Add(eventKey, (new HashSet<object>(), typeID));
        }

        // check if action's type matches registered type
        var registeredType = mapper[eventKey].Item2;
        if (typeof(T) != registeredType) {
            Debug.LogError("Incorrect action type. Send a " + registeredType.Name + "!");
            return;
        }

        mapper[eventKey].Item1.Add((object)action);
    }

    public static void Message<T>(string eventKey, T msg) {
        if (!mapper.ContainsKey(eventKey)) {
            return;
        }

        // check if type matches event
        var registeredType = mapper[eventKey].Item2;
        if (typeof(T) != registeredType) {
            Debug.LogError("Incorrect message type. Send a " + registeredType.Name + "!");
            return;
        }

        var actions = mapper[eventKey].Item1;
        var deadActions = new HashSet<object>();
        foreach (var action in actions) {
            var castedAction = (System.Action<T>)action;
            var actionOrigin = (MonoBehaviour)(castedAction.Target);

            // should be null if monobehaviour was destroyed
            if (actionOrigin == null) {
                Debug.Log("removed an action");
                deadActions.Add(action);
            } else {
                castedAction.Invoke(msg);
            }
        }

        // remove all dead actions
        foreach (var deadAction in deadActions) {
            actions.Remove(deadAction);
        }

        // remove event if all listeners have been removed
        if (actions.Count == 0) {
            mapper.Remove(eventKey);
        }

        return;
    }
}