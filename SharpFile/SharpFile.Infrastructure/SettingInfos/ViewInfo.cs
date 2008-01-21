using System.Collections;
using System.Xml.Serialization;
using Common;
using System;
using Common.Logger;

namespace SharpFile.Infrastructure {
    public class ViewInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private FullyQualifiedType comparerType;
        private IViewComparer comparer;

        [XmlAttribute("Name")]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }

        public FullyQualifiedType ComparerType {
            get {
                return comparerType;
            }
            set {
                comparerType = value;
            }
        }

        [XmlIgnore]
        public IViewComparer Comparer {
            get {
                if (comparer == null) {
                    if (comparerType != null) {
                        try {
                            comparer = Reflection.InstantiateObject<IViewComparer>(
                                comparerType.Assembly,
                                comparerType.Type);
                        } catch (MissingMethodException ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Comparer, {0}, could not be instantiated (is it an abstract class?).",
                                comparerType.Type);
                        } catch (TypeLoadException ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Comparer, {0}, can not be instantiated.",
                                comparerType.Type);
                        }
                    }
                }

                return comparer;
            }
        }
    }
}