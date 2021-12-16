using Unity.Advertisement.IosSupport.Components;
using UnityEngine;
using System;
using System.Collections;
#if UNITY_IOS
using UnityEngine.iOS;
using UnityEngine.SceneManagement;

namespace Unity.Advertisement.IosSupport.Samples
{

    /// <summary>
    /// This component will trigger the context screen to appear when the scene starts,
    /// if the user hasn't already responded to the iOS tracking dialog.
    /// </summary>
    public class ContextScreenManager : MonoBehaviour
    {

        Version currentVersion;
        Version ios14;
        /// <summary>
        /// The prefab that will be instantiated by this component.
        /// The prefab has to have an ContextScreenView component on its root GameObject.
        /// </summary>
        public ContextScreenView contextScreenPrefab;

        void Start()
        {
#if UNITY_IOS
            // check with iOS to see if the user has accepted or declined tracking
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            currentVersion = new Version(Device.systemVersion);
            ios14 = new Version("14.5");

            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && currentVersion >= ios14)
            {
                var contextScreen = Instantiate(contextScreenPrefab).GetComponent<ContextScreenView>();

                // after the Continue button is pressed, and the tracking request
                // has been sent, automatically destroy the popup to conserve memory
                contextScreen.sentTrackingAuthorizationRequest += () => Destroy(contextScreen.gameObject);
            }


#else
            Debug.Log("Unity iOS Support: App Tracking Transparency status not checked, because the platform is not iOS.");
#endif
            StartCoroutine(loadNextScene());
        }


        IEnumerator loadNextScene()
        {
#if UNITY_IOS && !UNITY_EDITOR
           
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
          
            while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && currentVersion >= ios14)
            {
                status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
                yield return null;
            }
#endif
            SceneManager.LoadScene("main menu");
            yield return null;
        }


    }
}
#endif