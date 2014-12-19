﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ContractConfigurator
{
    class LoggingUtil
    {

        public enum LogLevel {
            VERBOSE = 0,
            DEBUG = 1,
            INFO = 2,
            WARNING = 3,
            ERROR = 4
        }

        public static LogLevel logLevel {set; get;}

        private static Dictionary<Type, LogLevel> specificLogLevels = new Dictionary<Type, LogLevel>();

        /*
         * Loads debugging configurations.
         */
        public static void LoadDebuggingConfig()
        {
            Debug.Log("[INFO] CC_LoggingUtil: Loading DebuggingConfig node.");
            // Don't know why .GetConfigNode("CC_DEBUGGING") returns null, using .GetConfigNodes("CC_DEBUGGING") works fine.
            ConfigNode[] debuggingConfigs = GameDatabase.Instance.GetConfigNodes("CC_DEBUGGING");

            if (debuggingConfigs.Length > 0)
            {
                try
                {
                    // Fetch config
                    ConfigNode debuggingConfig = debuggingConfigs[0];

                    // Set LogLevel
                    if (debuggingConfig.HasValue("logLevel"))
                    {
                        LoggingUtil.logLevel = (LoggingUtil.LogLevel)Enum.Parse(typeof(LoggingUtil.LogLevel), debuggingConfig.GetValue("logLevel"));
                        LoggingUtil.LogInfo(typeof(LoggingUtil), "Set LogLevel = " + LoggingUtil.logLevel);
                    }

                    // Fetch specific loglevels for given types
                    foreach (ConfigNode levelExceptionNode in debuggingConfig.GetNodes("ADD_LOGLEVEL_EXCEPTION"))
                    {
                        if (levelExceptionNode.HasValue("type") && levelExceptionNode.HasValue("logLevel"))
                        {
                            // Fetch full type name
                            string typeName = levelExceptionNode.GetValue("type");
                            if (typeName.StartsWith("ContractConfigurator.") == false)
                            {
                                typeName = "ContractConfigurator." + typeName;
                            }

                            Type type = Type.GetType(typeName);

                            if (type != null)
                            {
                                LoggingUtil.LogLevel logLevel = (LoggingUtil.LogLevel)Enum.Parse(typeof(LoggingUtil.LogLevel), levelExceptionNode.GetValue("logLevel"));
                                LoggingUtil.AddSpecificLogLevel(type, logLevel);
                            }
                            else
                            {
                                Debug.LogWarning("[WARNING] CC_LoggingUtil: Couldn't find Type with name: '" + typeName + "'");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("[WARNING] CC_LoggingUtil: Couldn't load specific LogLevel node, type or logLevel not given!");
                        }
                    }

                    LoggingUtil.LogInfo(typeof(LoggingUtil), "DebugingConfig loaded!");
                }
                catch (Exception e)
                {
                    LoggingUtil.ClearSpecificLogLevel();
                    LoggingUtil.logLevel = LoggingUtil.LogLevel.INFO;

                    LoggingUtil.LogWarning(typeof(LoggingUtil), "Debugging Config failed to load! Message: '" + e.Message + "' Set LogLevel to INFO and cleaned specific LogLevels");
                }
            }
            else
            {
                LoggingUtil.logLevel = LoggingUtil.LogLevel.INFO;
                LoggingUtil.LogWarning(typeof(LoggingUtil), "No debugging config found! LogLevel set to INFO");
            }
        }

        public static void LogVerbose(Type type, string message) {
            LoggingUtil.Log(LogLevel.VERBOSE, type, message);
        }

        public static void LogDebug(Type type, string message) {
            LoggingUtil.Log(LogLevel.DEBUG, type, message);
        }

        public static void LogInfo(Type type, string message) {
            LoggingUtil.Log(LogLevel.INFO, type, message);
        }
        
        public static void LogWarning(Type type, string message) {
            LoggingUtil.Log(LogLevel.WARNING, type, message);
        }

        public static void LogError(Type type, string message) {
            LoggingUtil.Log(LogLevel.ERROR, type, message);
        }

        public static void Log(LogLevel logLevel, Type type, string message)
        {
            LogLevel logLevelCheckAgainst = LoggingUtil.logLevel;
            if (specificLogLevels.ContainsKey(type))
            {
                logLevelCheckAgainst = specificLogLevels[type]; 
            }

            if (logLevel >= logLevelCheckAgainst)
            {                
                message = "CC_" + type.Name + ": " + message;

                if (logLevel <= LogLevel.INFO) {
                    Debug.Log("[" + logLevel + "] " + message);
                }
                else if (logLevel == LogLevel.WARNING) {
                    Debug.LogWarning(message);
                }
                else if (logLevel == LogLevel.ERROR) {
                    Debug.LogError(message);
                }
            }
        }

        private static void AddSpecificLogLevel(Type type, LogLevel logLevel)
        {
            specificLogLevels.Add(type, logLevel);
        }

        private static void ClearSpecificLogLevel()
        {
            specificLogLevels.Clear();
        }
    }
}