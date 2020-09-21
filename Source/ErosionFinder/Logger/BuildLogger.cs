using Microsoft.Build.Framework;
using System;
using System.Diagnostics;

namespace ErosionFinder.Logger
{
    internal class BuildLogger : Microsoft.Build.Framework.ILogger
    {
        public BuildLogger() { }

        public LoggerVerbosity Verbosity 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public string Parameters
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Initialize(IEventSource eventSource)
        {
            Trace.WriteLine($"{GetTracePrefixMessage()}Initializing MSBuild recorder");

            eventSource.TargetStarted += (sender, evt) =>
                Trace.WriteLine($"{GetTracePrefixMessage()}{evt.TargetName} started");

            eventSource.TargetFinished += (sender, evt) =>
                Trace.WriteLine($"{GetTracePrefixMessage()}{evt.TargetName} finished");

            eventSource.WarningRaised += (sender, evt) =>
            {
                var warningLogMessage = $"Warning at file '{evt.File}', from project '{evt.ProjectFile}'; ";

                warningLogMessage += $"error associated with lines {evt.LineNumber}-{evt.EndLineNumber}; Sender: {evt.SenderName}. ";
                warningLogMessage += $"Code error: {evt.Code}; message: '{evt.Message}'";

                Trace.TraceWarning($"{GetTracePrefixMessage()}{warningLogMessage}");
            };

            eventSource.ErrorRaised += (sender, evt) =>
            {
                var errorLogMessage = $"Error at file '{evt.File}', from project '{evt.ProjectFile}'; ";
            
                errorLogMessage += $"error associated with lines {evt.LineNumber}-{evt.EndLineNumber}; Sender: {evt.SenderName}. ";
                errorLogMessage += $"Code error: {evt.Code}; message: '{evt.Message}'";

                Trace.Fail($"{GetTracePrefixMessage()}{errorLogMessage}");
            };

            eventSource.MessageRaised += (sender, evt) =>
            {
                var logMessage = $"{GetTracePrefixMessage()}{evt.Message}";

                switch (evt.Importance)
                {
                    case MessageImportance.High:
                    case MessageImportance.Normal:
                        Trace.TraceInformation(logMessage);
                        break;
                    default:
                        Trace.WriteLine(logMessage);
                        break;
                }
            };
        }

        public void Shutdown() 
        {
            Trace.WriteLine($"{GetTracePrefixMessage()}Shutting down MSBuild recorder");
        }

        private string GetTracePrefixMessage() 
            => $"[MSBuild solution compiling events] - ";
    }
}