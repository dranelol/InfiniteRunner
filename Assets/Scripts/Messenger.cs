// Simple C# / Unity Messaging System
// Author: Matt Wallace
// Last Updated: 10.20.14
// Adapted from:
// Rod Hyde's "CSharpMessenger"
// Magnus Wolffelt's "CSharpMessenger Extended"
// Ilya Suzdalnitski's "Advanced C# Messenger"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


#region Example Usage

/*
void OnPropCollected( PropType propType ) 
{
    if (propType == PropType.Life)
    {
        livesAmount++;
    }
}

void Start() 
{
	Messenger.AddListener< Prop >( "prop collected", OnPropCollected );
}

Messenger.RemoveListener< Prop > ( "prop collected", OnPropCollected );

public void OnTriggerEnter(Collider _collider) 
{
	Messenger.Broadcast< PropType > ( "prop collected", _collider.gameObject.GetComponent<Prop>().propType );
}

*/

#endregion

//public delegate void Callback(params object[] args);

// Callback functions; one for each function type
public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

public class BroadcastException : Exception
{
    public BroadcastException(string msg)
        : base(msg)
    {
    }
}

public class ListenerException : Exception
{
    public ListenerException(string msg)
        : base(msg)
    {
    }
}

static internal class Messenger
{
    // instantiates a messengerhelper object as a singleton
    static private MessengerHelper messengerHelper = (new GameObject("MessengerHelper")).AddComponent<MessengerHelper>();

    static public Dictionary<string, Delegate> events = new Dictionary<string, Delegate>();

    static public List<string> permanentMessages = new List<string>();

    static public void MakePermanent(string eventName)
    {
        permanentMessages.Add(eventName);
    }

    static public void Cleanup()
    {
        List<string> messagesToRemove = new List<string>();

        foreach (KeyValuePair<string, Delegate> pair in events)
        {
            bool wasFound = false;

            foreach (string message in permanentMessages)
            {
                if (pair.Key == message)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                messagesToRemove.Add(pair.Key);
        }

        foreach (string message in messagesToRemove)
        {
            events.Remove(message);
        }
    }

    #region adding a listener
    static void preAddListener(string eventType, Delegate listenerToAdd)
    {
        // if we havent added an event for this type yet, add a null k,v pair in the dictionary
        if (events.ContainsKey(eventType) == false)
        {
            events.Add(eventType, null);
        }

        // grab pre-existing event; null is returned if we just added the event
        Delegate del = events[eventType];

        // check if the event even contained anything
        if (listenerToAdd == null)
        {
            throw new ListenerException("You tried to add an empty event");
        }

        // make sure the listener to be added, and the current listeners to that event, are of the same type
        if (del != null && del.GetType() != listenerToAdd.GetType())
        {
            throw new ListenerException(
                "Attempting to add a listener with a type inconsistant with the current listeners for type "
                + eventType + ". Current type is " + del.GetType().Name + ", trying to add " + listenerToAdd.GetType().Name);
        }
    }



    static public void AddListener(string eventType, Callback listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback)events[eventType] + listenerToAdd;
    }

    static public void AddListener<T>(string eventType, Callback<T> listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback<T>)events[eventType] + listenerToAdd;
    }

    static public void AddListener<T, U>(string eventType, Callback<T, U> listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback<T, U>)events[eventType] + listenerToAdd;
    }

    static public void AddListener<T, U, V>(string eventType, Callback<T, U, V> listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback<T, U, V>)events[eventType] + listenerToAdd;
    }
    #endregion

    #region removing a listener

    static void preRemoveListener(string eventType, Delegate listenerToRemove)
    {
        // check to see if this event even exists
        if (events.ContainsKey(eventType) == true)
        {
            // grab pre-existing event; null is returned if said event doesnt exist
            Delegate del = events[eventType];

            // check if any events of this type already exist (may be redundant, but whatever)
            if (del == null)
            {
                throw new ListenerException("Attempting to remove a listener for " + eventType + " but it doesn't exist");
            }

            // check for event type mismatch
            else if (del.GetType() != listenerToRemove.GetType())
            {
                throw new ListenerException(
                    "Attempting to remove a listener with a type inconsistant with the current listeners for type "
                    + eventType + ". Current type is " + del.GetType().Name + ", trying to add " + listenerToRemove.GetType().Name);
            }
        }


        else
        {
            throw new ListenerException("Attempting to remove listener for type: " + eventType + " but the messenger doesn't know about this event type.");
        }
    }

    static void postRemoveListener(string eventType)
    {
        if (events[eventType] == null)
        {
            events.Remove(eventType);
        }
    }


    static public void RemoveListener(string eventType, Callback listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    static public void RemoveListener<T>(string eventType, Callback<T> listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback<T>)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    static public void RemoveListener<T, U>(string eventType, Callback<T, U> listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback<T, U>)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    static public void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback<T, U, V>)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    #endregion

    #region broadcasting


    static public void Broadcast(string eventType)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + "found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback callback = (Callback)del;

            if (callback != null)
            {
                callback();
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    static public void Broadcast<T>(string eventType, T arg0)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + " found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback<T> callback = (Callback<T>)del;

            if (callback != null)
            {
                callback(arg0);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    static public void Broadcast<T, U>(string eventType, T arg0, U arg1)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + "found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback<T, U> callback = (Callback<T, U>)del;

            if (callback != null)
            {
                callback(arg0, arg1);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    static public void Broadcast<T, U, V>(string eventType, T arg0, U arg1, V arg2)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + "found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback<T, U, V> callback = (Callback<T, U, V>)del;

            if (callback != null)
            {
                callback(arg0, arg1, arg2);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    #endregion


}

//This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
public sealed class MessengerHelper : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //Clean up eventTable every time a new level loads.
    public void OnLevelWasLoaded(int unused)
    {
        Messenger.Cleanup();
    }
}


