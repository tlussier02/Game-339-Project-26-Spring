using UnityEngine;

namespace Game.Runtime
{
    public interface IGameLogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    public sealed class NullGameLogger : IGameLogger
    {
        public static readonly IGameLogger Instance = new NullGameLogger();

        private NullGameLogger()
        {
        }

        public void Log(string message)
        {
        }

        public void LogWarning(string message)
        {
        }

        public void LogError(string message)
        {
        }
    }

    public sealed class UnityDebugGameLogger : IGameLogger
    {
        private readonly string _prefix;
        private readonly Object _context;

        public UnityDebugGameLogger(string category, Object context = null)
        {
            _prefix = string.IsNullOrWhiteSpace(category) ? "[Game]" : "[" + category + "]";
            _context = context;
        }

        public void Log(string message)
        {
            if (_context != null)
            {
                Debug.Log(Format(message), _context);
                return;
            }

            Debug.Log(Format(message));
        }

        public void LogWarning(string message)
        {
            if (_context != null)
            {
                Debug.LogWarning(Format(message), _context);
                return;
            }

            Debug.LogWarning(Format(message));
        }

        public void LogError(string message)
        {
            if (_context != null)
            {
                Debug.LogError(Format(message), _context);
                return;
            }

            Debug.LogError(Format(message));
        }

        private string Format(string message)
        {
            return _prefix + " " + message;
        }
    }
}
