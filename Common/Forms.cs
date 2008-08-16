using System;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace Common {
    public static class Forms {
        private delegate bool propertyInfoValidator<T>(PropertyInfo propertyInfo, Control control);
        private delegate void eventInfoAttacher(EventInfo eventInfo, Control control);

        /// <summary>
        /// Get the specified property in any child controls for the current control.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <returns>The value of the property that was found.</returns>
        public static T GetPropertyInChild<T>(Control control, string property) {
            T value = default(T);

            bool foundProperty = findControlInChild<T>(control, property,
                // Make sure that the property can be read.
                delegate(PropertyInfo propertyInfo, Control childControl) {
                    if (propertyInfo.CanRead) {
                        value = (T)propertyInfo.GetValue(childControl, null);
                        return true;
                    }

                    return false;
                });

            if (foundProperty) {
                return value;
            } else {
                return default(T);
            }
        }

        /// <summary>
        /// Get the specified property in any parent control of the current control.
        /// Note: Must pass in the parent control of the current control, overwise there will be a stack overflow.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <returns>The value of the property found.</returns>
        public static T GetPropertyInParent<T>(Control control, string property) {
            PropertyInfo info = control.GetType().GetProperty(property, typeof(T));

            // Make sure the property is valid and can be read.
            if ((info != null) && info.CanRead) {
                T local = (T)info.GetValue(control, null);

                if (local != null) {
                    return local;
                }
            }

            // If there is no parent to be found, throw an exception.
            if (control.Parent == null) {
                throw new Exception("The property, " + property + ", was not found.");
            }

            // Recursively search for the property in the current control's parent.
            return GetPropertyInParent<T>(control.Parent, property);
        }

        /// <summary>
        /// Set the specified property in the first child of the current control that is found.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <param name="value">The value to set on the property.</param>
        public static void SetPropertyInChild<T>(Control control, string property, T value) {
            if (value != null) {
                findControlInChild<T>(control, property,
                    delegate(PropertyInfo propertyInfo, Control childControl) {
                        // Make sure the property can be set.
                        if (propertyInfo.CanWrite) {
                            if (childControl.InvokeRequired) {
                                childControl.Invoke((MethodInvoker)delegate {
                                    propertyInfo.SetValue(childControl, value, null);
                                });
                            } else {
                                propertyInfo.SetValue(childControl, value, null);
                            }

                            return true;
                        }

                        return false;
                    });
            }
        }

        /// <summary>
        /// Set the specified property in the first parent of the current control that is found.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <param name="value">The value to set on the property.</param>
        public static void SetPropertyInParent<T>(Control control, string property, T value) {
            if (value != null) {
                PropertyInfo info = control.GetType().GetProperty(property, typeof(T));

                // Make sure the property is valid and can be read.
                if ((info != null) && info.CanWrite) {
                    if (control.InvokeRequired) {
                        control.Invoke((MethodInvoker)delegate {
                            info.SetValue(control, value, null);
                        });
                    } else {
                        info.SetValue(control, value, null);
                    }

                    return;
                }

                // If there is no parent to be found, throw an exception.
                if (control.Parent == null) {
                    throw new Exception("The property, " + property + ", was not found.");
                }

                // Recursively search for the property in the current control's parent.
                SetPropertyInParent<T>(control.Parent, property, value);
            }
        }

        /// <summary>
        /// Set the specified property in the first parent of the current control that is found.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <param name="value">The value to set on the property.</param>
        public static void AddEventHandlerInChildren(Control control, string eventName, Delegate handler) {
            findEventInChildren(control, eventName,
                // Make sure that the property can be read.
                delegate(EventInfo eventInfo, Control childControl) {
                    eventInfo.AddEventHandler(childControl, handler);
                });
        }

        /// <summary>
        /// Paints the background color red to show an error in a control.
        /// </summary>
        /// <param name="form">Form that the control is placed on.</param>
        /// <param name="control">Control to change the background color of.</param>
        public static void ShowErrorInControl(Control control) {
            // Paint the textbox red to show there was an error.
            using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
                backgroundWorker.DoWork += delegate {
                    Color originalBackColor = Color.White;

                    control.Invoke((MethodInvoker)delegate {
                        originalBackColor = control.BackColor;
                        control.BackColor = Color.Red;
                    });

                    Thread.Sleep(500);

                    control.Invoke((MethodInvoker)delegate {
                        control.BackColor = originalBackColor;
                    });
                };

                backgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Recursively searches for a property in the control's children.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <returns>A container object with the control and property information.</returns>
        private static bool findControlInChild<T>(Control control, string property, propertyInfoValidator<T> validator) {
            foreach (Control child in control.Controls) {
                PropertyInfo propertyInfo = child.GetType().GetProperty(property, typeof(T));

                if (propertyInfo != null) {
                    bool foundProperty = validator(propertyInfo, child);

                    if (foundProperty) {
                        return true;
                    }
                }

                return findControlInChild<T>(child, property, validator);
            }

            throw new Exception("The property, " + property + ", was not found.");
        }

        /// <summary>
        /// Recursively searches for a property in the control's children.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="control">Control to begin searching at.</param>
        /// <param name="property">Property to search for.</param>
        /// <returns>A container object with the control and property information.</returns>
        private static void findEventInChildren(Control control, string eventName, eventInfoAttacher attacher) {
            foreach (Control child in control.Controls) {
                EventInfo info = child.GetType().GetEvent(eventName);

                if (info != null) {
                    attacher(info, child);
                }

                findEventInChildren(child, eventName, attacher);
            }
        }
    }
}