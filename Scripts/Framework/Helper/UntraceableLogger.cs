using UnityEngine;

namespace Sapiscow
{
	public static class UntraceableLogger
	{
		public static void Log(object message)
		{
#if UNITY_EDITOR
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}", message);
#endif
		}

		public static void LogWarning(object message)
		{
#if UNITY_EDITOR
			Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "{0}", message);
#endif
		}

		public static void LogError(object message)
		{
#if UNITY_EDITOR
			Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "{0}", message);
#endif
		}

		public static void LogFormat(string format, params object[] args)
		{
#if UNITY_EDITOR
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, format, args);
#endif
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
#if UNITY_EDITOR
			Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, format, args);
#endif
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
#if UNITY_EDITOR
			Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, format, args);
#endif
		}
	}
}