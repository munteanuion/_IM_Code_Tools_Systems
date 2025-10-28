#if FMOD
using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ThisProject.__Project.Scripts.Helpers
{
    [CreateAssetMenu(fileName = "FMODEventsManager", menuName = "ScriptableObjects/FMODEventsManager", order = 1)]
    public class FMODEventsManager : ScriptableObject
    {
        #region Singleton Instance

        private static FMODEventsManager _instance;

        public static FMODEventsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<FMODEventsManager>("FMODEventsSystem");

                    if (_instance == null)
                    {
                        Debug.LogError("FMODEventsSystem not found. Please create one in the Resources folder.");
                    }
                }
                return _instance;
            } 
        }

        #endregion
    
    
        
        #region Static Methods For Parameters and Play Stop Events
        
        public static void SetGlobalParameter(string parameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }
    
        public static void SetGlobalParameter(string parameterName, string value)
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameterName, value);
        }
        
        
        
        public static void SetParameterEvent(string eventName, string parameterName, float value, bool setForAllEventInstances = false)
        {
            RuntimeManager.StudioSystem.getEvent(eventName,out EventDescription eventDescription);
            eventDescription.getInstanceList(out EventInstance[] eventInstances);
        
            if (setForAllEventInstances)
            {
                foreach (var item in eventInstances)
                    item.setParameterByName(parameterName, value);
            }
            else eventInstances[0].setParameterByName(parameterName, value); 
        }
    
        public static void SetParameterEvent(string eventName, string parameterName, string value, bool setForAllEventInstances = false)
        {
            RuntimeManager.StudioSystem.getEvent(eventName,out EventDescription eventDescription);
            eventDescription.getInstanceList(out EventInstance[] eventInstances);
        
            if (setForAllEventInstances)
            {
                foreach (var item in eventInstances)
                    item.setParameterByNameWithLabel(parameterName, value);
            }
            else eventInstances[0].setParameterByNameWithLabel(parameterName, value); 
        }

    
        
        
    
        public static void StopEvent(string eventPath, bool stopAllEventInstances = false) => StopEventMain(eventPath, stopAllEventInstances);
        public static void StopEvent(GUID eventPath, bool stopAllEventInstances = false) => StopEventMain(eventPath, stopAllEventInstances);
        public static void StopEvent(EventInstance eventInstance) => eventInstance.StopRelease();
        
        private static void StopEventMain(object eventPath, bool stopAllEventInstances = false)
        {
            EventDescription eventDescription = default;
            
            if (eventPath is string)
                RuntimeManager.StudioSystem.getEvent((string)eventPath,out eventDescription);
            else
                RuntimeManager.StudioSystem.getEventByID((GUID)eventPath,out eventDescription);
            
            eventDescription.getInstanceList(out EventInstance[] eventInstances);
        
            
            if (stopAllEventInstances)
            {
                foreach (var item in eventInstances)
                {
                    item.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    item.release();
                }
            }
            else
            {
                eventInstances[0].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                eventInstances[0].release();
            }
        }

        
        
        public static EventInstance PlayEventOne(GUID eventPath) => PlayEventOneMain(eventPath);
        public static EventInstance PlayEventOne(string eventPath) => PlayEventOneMain(eventPath);
        
        private static EventInstance PlayEventOneMain(object eventPath)
        {
            EventInstance eventInstance;

            if (eventPath is string)
                eventInstance = RuntimeManager.CreateInstance((string)eventPath);
            else
                eventInstance = RuntimeManager.CreateInstance((GUID)eventPath);
            
            eventInstance.start();
            eventInstance.release(); // Releasing the event instance since it's a one-shot

            return eventInstance;
        }

        
        
        public static EventInstance Play3DEvent(string eventPath, Transform attachedTransform, bool autoRelease = true)
            => Play3DEventMain(eventPath,attachedTransform,autoRelease);
        public static EventInstance Play3DEvent(GUID eventPath, Transform attachedTransform, bool autoRelease = true)
            => Play3DEventMain(eventPath,attachedTransform,autoRelease);
        
        private static EventInstance Play3DEventMain(object eventPath, Transform attachedTransform, bool autoRelease = true)
        {
            EventInstance eventInstance;

            if (eventPath is string)
                eventInstance = RuntimeManager.CreateInstance((string)eventPath);
            else
                eventInstance = RuntimeManager.CreateInstance((GUID)eventPath);

            RuntimeManager.AttachInstanceToGameObject(eventInstance, attachedTransform /*, attachedTransform.GetComponent<Rigidbody>()*/);
            eventInstance.start();

            if (autoRelease)
                eventInstance.release();
            
            return eventInstance;
        }
        
        
        #endregion

    
/*public static async void InitializeMusic()
        {
            // Wait until all banks are loaded
            await UniTask.WaitWhile(() => !RuntimeManager.HaveAllBanksLoaded);
            
            PlayMusicPermanentEvent(Event.Music_M_Music);
            //SetGlobalParameter(FMODParameters.Scene,FMODParameters.Scene_All_Labels.MainMenu);
        }*/


        #region Music Persistent Event

        //for local variables and persistent events
        private static FMOD.Studio.EventInstance _musicEventInstance;
        private static bool _musicEventInstantiated;

        public static void PlayMusicPermanentEvent(string eventPath)
        {
            if (_musicEventInstantiated) return;

            var eventInstance = RuntimeManager.CreateInstance(eventPath);
            _musicEventInstance = eventInstance;
            eventInstance.start();
            eventInstance.release();
            _musicEventInstantiated = true;
        }

        public static void SetParameterValueMusicEvent(string parameterName, float value)
        {
            _musicEventInstance.setParameterByName(parameterName, value);
        }

        #endregion
    }
}



public static class ExtensionsFMOD
{
    public static void StopRelease(this EventInstance eventInstance)
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
        }
    }
}
#endif
