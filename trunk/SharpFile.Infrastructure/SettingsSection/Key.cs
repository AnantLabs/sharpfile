using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public struct Key {
        private Keys primaryKey;
        private List<Keys> modifierKeys;
        Keys modifiers;

        public Key(List<Keys> modifierKeys, Keys primaryKey) {
            this.modifierKeys = modifierKeys;
            this.primaryKey = primaryKey;
            this.modifiers = 0;
        }

        /// <summary>
        /// Primary key.
        /// </summary>
        public Keys PrimaryKey {
            get {
                return primaryKey;
            }
            set {
                primaryKey = value;
            }
        }

        /// <summary>
        /// Modifier Keys.
        /// </summary>
        [XmlArray("ModifierKeys")]
        [XmlArrayItem("Key")]
        public List<Keys> ModifierKeys {
            get {
                return modifierKeys;
            }
            set {
                modifierKeys = value;
            }
        }

        /// <summary>
        /// Lazy-loaded modifiers OR-ed together.
        /// </summary>
        [XmlIgnore]
        public Keys Modifiers {
            get {
                if (modifiers == 0) {
                    if (ModifierKeys != null) {
                        foreach (Keys k in ModifierKeys) {
                            modifiers |= k;
                        }
                    }
                }

                return modifiers;
            }
        }
    }
}