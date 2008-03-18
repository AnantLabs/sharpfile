using System;
using System.Xml.Serialization;
using Common.Logger;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class LoggerInfo {
        private string file;
        private LogLevelType logLevel;

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
        public LogLevelType LogLevel {
            get {
                return logLevel;
            }
            set {
                logLevel = value;
            }
        }
    }
}