using System;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class LoggerInfo {
        private string file;
        private Common.LogLevelType logLevel;

        public LoggerInfo() {
        }

        public string File {
            get {
                return file;
            }
            set {
                file = value;
            }
        }

        [XmlElement("LogLevel")]
        public Common.LogLevelType LogLevel {
            get {
                return logLevel;
            }
            set {
                logLevel = value;
            }
        }
    }
}
