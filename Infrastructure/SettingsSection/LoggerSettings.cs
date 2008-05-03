using System;
using System.Xml.Serialization;
using Common.Logger;

namespace SharpFile.Infrastructure.SettingsSection {
    public class Logger {
        private string file = "log.txt";
        private LogLevelType logLevel = LogLevelType.ErrorsOnly;

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