using UnityEngine;

namespace CodeExtensions
{
    /*public static class LoggerExtensions
          {
              public static void Log(this Object loggerObject, string message)
              {
                  Debug.Log($"<color=yellow>{GetLoggerPrefix(loggerObject)}<color=white> {message}");
              }
              
              public static void LogError(this Object loggerObject, string message)
              {
                  Debug.LogError($"<color=red>{GetLoggerPrefix(loggerObject)}<color=white> {message}");
              }
      
              private static string GetLoggerPrefix(Object loggerObject)
              {
                  var typePart = $"[{loggerObject.GetType()}]";
      
                  if (loggerObject is not MonoBehaviour monoBehaviour)
                  {
                      return typePart;
                  }
      
                  var sceneName = monoBehaviour.gameObject.scene.name;
                  var objectName = monoBehaviour.name;
      
                  return $"{typePart}[{sceneName}][{objectName}]";
              }
          }*/
}