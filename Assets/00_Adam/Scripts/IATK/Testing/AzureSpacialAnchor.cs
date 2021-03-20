using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;
using Microsoft.Azure.SpatialAnchors;


#if WINDOWS_UWP || UNITY_WSA
using NativeAnchor = UnityEngine.XR.WSA.WorldAnchor;
#else
using NativeAnchor = UnityEngine.MonoBehaviour;
#endif

public class AzureSpacialAnchor : MonoBehaviour
{
    private const string accountKey = @"Sly3ON+5/tj6vWZvrYMRoV049AKdp8wjmbTeYs3iD8Y=";
    private const string accountDomain = @"southcentralus.mixedreality.azure.com";
    public GameObject originalObj;
    private readonly Queue<Action> dispatchQueue = new Queue<Action>();
    protected CloudSpatialAnchorSession cloudSpatialAnchorSession;
    protected CloudSpatialAnchor currentCloudAnchor;
    protected string cloudSpacialAnchorID = "";
    protected GameObject phyAnchor;
    protected bool createAnchor = false;
    protected float recommendedForCreate = 0;

    private void Start()
    {
        createAnchor = true;
        
        InitializeSession();
        
    }

    private void Update()
    {
        lock (dispatchQueue)
        {
            if (dispatchQueue.Count > 0)
            {
                dispatchQueue.Dequeue()();
            }
        }
    }

    protected void QueueOnUpdate(Action updateAction)
    {
        lock (dispatchQueue)
        {
            dispatchQueue.Enqueue(updateAction);
        }
    }

    //Can be used to clean up objects
    /*public void CleanupObjects()
    {
        if (sphere != null)
        {
            Destroy(sphere);
            sphere = null;
        }

        if (sphereMaterial != null)
        {
            Destroy(sphereMaterial);
            sphereMaterial = null;
        }

        currentCloudAnchor = null;
    }*/

    public void ResetSession(Action completionRoutine = null)
    {
        Debug.Log("ASA Info: Resetting the session.");

        if (cloudSpatialAnchorSession.GetActiveWatchers().Count > 0)
        {
            Debug.LogError("ASA Error: We are resetting the session with active watchers, which is unexpected.");
        }

        // CleanupObjects();

        this.cloudSpatialAnchorSession.Reset();

        lock (this.dispatchQueue)
        {
            this.dispatchQueue.Enqueue(() =>
            {
                if (cloudSpatialAnchorSession != null)
                {
                    cloudSpatialAnchorSession.Stop();
                    cloudSpatialAnchorSession.Dispose();
                    Debug.Log("ASA Info: Session was reset.");
                    completionRoutine?.Invoke();
                }
                else
                {
                    Debug.LogError("ASA Error: cloudSpatialAnchorSession was null, which is unexpected.");
                }
            });
        }
    }


    void InitializeSession()
    {
        Debug.Log("ASA Info: Initializing a CloudSpatialAnchorSession.");
        //cloudSpatialAnchorSession.Start();

        Debug.Log("Account key set.");
   
        cloudSpatialAnchorSession = new CloudSpatialAnchorSession();

        this.cloudSpatialAnchorSession.Configuration.AccountId = @"2632fff4-ebde-40d7-b9d0-47f809e2613a";
        this.cloudSpatialAnchorSession.Configuration.AccountKey = accountKey;
        this.cloudSpatialAnchorSession.Configuration.AccountDomain = accountDomain;

       this.cloudSpatialAnchorSession.Start();
        cloudSpatialAnchorSession.LogLevel = SessionLogLevel.All;

        cloudSpatialAnchorSession.Error += CloudSpatialAnchorSession_Error;
        cloudSpatialAnchorSession.OnLogDebug += CloudSpatialAnchorSession_OnLogDebug;
        cloudSpatialAnchorSession.SessionUpdated += CloudSpatialAnchorSession_SessionUpdated;
        cloudSpatialAnchorSession.AnchorLocated += CloudSpatialAnchorSession_AnchorLocated;
        cloudSpatialAnchorSession.LocateAnchorsCompleted += CloudSpatialAnchorSession_LocateAnchorsCompleted;
      

        Debug.Log("ASA Info: Session was initialized.");
    }
    private void CloudSpatialAnchorSession_Error(object sender, SessionErrorEventArgs args)
    {
        Debug.LogError("ASA Error: " + args.ErrorMessage);
    }

    private void CloudSpatialAnchorSession_OnLogDebug(object sender, OnLogDebugEventArgs args)
    {
        Debug.Log("ASA Log: " + args.Message);
        System.Diagnostics.Debug.WriteLine("ASA Log: " + args.Message);
    }

    private void CloudSpatialAnchorSession_SessionUpdated(object sender, SessionUpdatedEventArgs args)
    {
        Debug.Log("ASA Log: recommendedForCreate: " + args.Status.RecommendedForCreateProgress);
        recommendedForCreate = args.Status.RecommendedForCreateProgress;
    }
    private void CloudSpatialAnchorSession_AnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        switch (args.Status)
        {
            case LocateAnchorStatus.Located:
                Debug.Log("ASA Info: Anchor located! Identifier: " + args.Identifier);
                QueueOnUpdate(() =>
                {
                    
                    phyAnchor = GameObject.Instantiate(originalObj, Vector3.zero, Quaternion.identity) as GameObject;

                    phyAnchor.AddComponent<WorldAnchor>();
                    
                    phyAnchor.GetComponent<UnityEngine.XR.WSA.WorldAnchor>().SetNativeSpatialAnchorPtr(args.Anchor.LocalAnchor);

                    // Resets anchor 
                    cloudSpacialAnchorID = "";
                    createAnchor = false;
                });
                break;
            case LocateAnchorStatus.AlreadyTracked:
                Debug.Log("ASA Info: Anchor already tracked. Identifier: " + args.Identifier);
                break;
            case LocateAnchorStatus.NotLocated:
                Debug.Log("ASA Info: Anchor not located. Identifier: " + args.Identifier);
                break;
            case LocateAnchorStatus.NotLocatedAnchorDoesNotExist:
                Debug.LogError("ASA Error: Anchor not located does not exist. Identifier: " + args.Identifier);
                break;
        }
    }

    private void CloudSpatialAnchorSession_LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
    {
        Debug.Log("ASA Info: Locate anchors completed. Watcher identifier: " + args.Watcher.Identifier);
    }



}

