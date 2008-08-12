using System;

namespace SharpFile.Infrastructure.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute {
        private string author;
        private string description;
        private string version;

        public PluginAttribute() {
        }

        public PluginAttribute(string author, string description) {
            this.author = author;
            this.description = description;
        }

        /// <summary>
        /// Author of the plugin.
        /// </summary>
        public string Author {
            get {
                return author;
            }
            set {
                author = value;
            }
        }

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description {
            get {
                return description;
            }
            set {
                description = value;
            }
        }

        /// <summary>
        /// Version of the plugin. Will default to the version of the assembly if not specified.
        /// </summary>
        public string Version {
            get {
                return version;
            }
            set {
                version = value;
            }
        }
    }
}