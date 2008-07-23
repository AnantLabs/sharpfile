using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common {
    public class Templater {
        private const string _property = "property";

        private object obj;
        private Type type;
        private Regex regex = new Regex(@"(\{(?<property>[^}]+)\})",
                RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public Templater(object obj) {
            this.obj = obj;
            this.type = obj.GetType();
        }

        public Templater(object obj, string regex, RegexOptions options) {
            this.obj = obj;
            this.type = obj.GetType();

            this.regex = new Regex(regex, options);
        }

        public string Generate(string template) {
            PropertyInfo propertyInfo;
            string propertyName;
            string propertyValue;
            string returnValue = template;

            foreach (Match match in this.regex.Matches(template)) {
                propertyName = match.Groups[_property].Value;
                propertyInfo = this.type.GetProperty(propertyName);

                if (propertyInfo != null) {
                    propertyValue = propertyInfo.GetValue(obj, null).ToString();
                    returnValue = returnValue.Replace(match.Value, propertyValue);
                }
            }

            return returnValue;
        }
    }
}