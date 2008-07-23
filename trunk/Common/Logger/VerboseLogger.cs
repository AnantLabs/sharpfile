using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Common.Logger {
    internal class VerboseLogger : Logger {
        private const string currentNamespace = "Common.Logger";

        protected override string getMessage(Exception exception, string content) {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = null;
            MethodBase methodBase = null;
            string fullyQualifiedMethod = string.Empty;
            string methodNamespace = currentNamespace;
            int frameCount = 0;

            // Make sure that the method we retrieve is the method directly before the call to a Log method.
            while (currentNamespace.Equals(methodNamespace) &&
                frameCount < stackTrace.FrameCount) {
                stackFrame = stackTrace.GetFrame(frameCount);
                methodBase = stackFrame.GetMethod();

                methodNamespace = methodBase.ReflectedType.Namespace;

                fullyQualifiedMethod = string.Format("{0}.{1}",
                    methodBase.ReflectedType.FullName,
                    methodBase.Name);

                frameCount++;
            }

            ParameterInfo[] parameterInfos = methodBase.GetParameters();
            string[] parameterNames = Array.ConvertAll<ParameterInfo, string>(parameterInfos,
                delegate(ParameterInfo pi) {
                    return pi.ParameterType.Name + " " + pi.Name;
                });

            string exceptionMessage = getExceptionMessage(exception);

            content = string.Format("{0}: {1}({2}): {3}{4}",
                DateTime.Now,
                fullyQualifiedMethod,
                string.Join(", ", parameterNames),
                string.IsNullOrEmpty(exceptionMessage) ? string.Empty : exceptionMessage,
                content);

            return content;
        }
    }
}