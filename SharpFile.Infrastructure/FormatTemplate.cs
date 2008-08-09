using System;
using System.Xml.Serialization;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class FormatTemplate {
        private string template;
        private FullyQualifiedMethod fullyQualifiedMethod;
        private FullyQualifiedMethod.AlterMethod alterMethod;

        public FormatTemplate() {
        }

        public FormatTemplate(string template) {
            this.template = template;
        }

        [XmlAttribute("Template")]
        public string Template {
            get {
                return template;
            }
            set {
                template = value;
            }
        }

        public FullyQualifiedMethod FullyQualifiedMethod {
            get {
                return fullyQualifiedMethod;
            }
            set {
                fullyQualifiedMethod = value;
            }
        }

        [XmlIgnore]
        public FullyQualifiedMethod.AlterMethod MethodDelegate {
            get {
                if (alterMethod == null && fullyQualifiedMethod != null) {
                    try {
                        alterMethod = Reflection.CreateDelegate<FullyQualifiedMethod.AlterMethod>(
                            fullyQualifiedMethod.FullyQualifiedType.Assembly,
                            fullyQualifiedMethod.FullyQualifiedType.Type,
                            fullyQualifiedMethod.Name);
                    } catch (Exception ex) {
                        string message = "Creating the AlterMethod, {0}, for the {1} FormatTemplate failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                fullyQualifiedMethod.FullyQualifiedType.Type, template);

                    }
                }

                return alterMethod;
            }
        }
    }
}