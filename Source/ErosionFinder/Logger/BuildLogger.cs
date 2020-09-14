using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System;

namespace ErosionFinder.Logger
{
    internal class BuildLogger : Microsoft.Build.Framework.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger logger;

        public BuildLogger(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory?.CreateLogger<BuildLogger>()
                ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

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
            logger.LogTrace($"{GetLogPrefixMessage()}Initializing MSBuild logger");

            eventSource.TargetStarted += (sender, evt) =>
                logger.LogDebug($"{GetLogPrefixMessage()}{evt.TargetName} started");

            eventSource.TargetFinished += (sender, evt) =>
                logger.LogDebug($"{GetLogPrefixMessage()}{evt.TargetName} finished");

            eventSource.WarningRaised += (sender, evt) =>
            {
                var warningLogMessage = $"Warning at file '{evt.File}', from project '{evt.ProjectFile}'; ";

                warningLogMessage += $"error associated with lines {evt.LineNumber}-{evt.EndLineNumber}; Sender: {evt.SenderName}. ";
                warningLogMessage += $"Code error: {evt.Code}; message: '{evt.Message}'";

                logger.LogWarning($"{GetLogPrefixMessage()}{warningLogMessage}");
            };

            eventSource.ErrorRaised += (sender, evt) =>
            {
                var errorLogMessage = $"Error at file '{evt.File}', from project '{evt.ProjectFile}'; ";
            
                errorLogMessage += $"error associated with lines {evt.LineNumber}-{evt.EndLineNumber}; Sender: {evt.SenderName}. ";
                errorLogMessage += $"Code error: {evt.Code}; message: '{evt.Message}'";

                logger.LogCritical($"{GetLogPrefixMessage()}{errorLogMessage}");
            };

            eventSource.MessageRaised += (sender, evt) =>
            {
                var logMessage = $"{GetLogPrefixMessage()}{evt.Message}";

                switch (evt.Importance)
                {
                    case MessageImportance.High:
                        logger.LogInformation(logMessage);
                        break;
                    case MessageImportance.Normal:
                        logger.LogDebug(logMessage);
                        break;
                    default:
                        logger.LogTrace(logMessage);
                        break;
                }
            };
        }

        public void Shutdown() 
        {
            logger.LogTrace($"{GetLogPrefixMessage()}Shutting down MSBuild logger");
        }

        private string GetLogPrefixMessage() => $"[{nameof(BuildLogger)}] - ";
    }
}