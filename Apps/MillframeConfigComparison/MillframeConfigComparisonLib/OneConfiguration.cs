using System;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillframeConfigComparisonLib
{
    public class OneConfiguration
    {
        public Configuration ThisWebConfig = null;
        public Configuration ThisAppConfig = null;

        public OneConfiguration(string Path, bool DoWebConfig, bool DoAppConfig=false)
        {
            Extract(Path, DoWebConfig, DoAppConfig);
        }

        public bool Extract(string PathArg, bool DoWebConfig, bool DoAppConfig = false)
        {
            if (!Directory.Exists(PathArg))
            {
                return false;
            }

            if (DoWebConfig)
            {
                ExeConfigurationFileMap ConfigFileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(PathArg, @"Web.config") };
                ThisWebConfig = ConfigurationManager.OpenMappedExeConfiguration(ConfigFileMap,ConfigurationUserLevel.None);

                Trace.WriteLine("appSettings section:");
                foreach (string key in ThisWebConfig.AppSettings.Settings.AllKeys)
                {
                    // TODO instead of trace outout, use the settings to compare
                    Trace.WriteLine(key + " => " + ThisWebConfig.AppSettings.Settings[key].Value);
                }

                Trace.WriteLine("Connection strings section:");
                foreach (ConnectionStringSettings Setting in ThisWebConfig.ConnectionStrings.ConnectionStrings)
                {
                    // TODO instead of trace outout, use the settings to compare
                    Trace.WriteLine(Setting.Name + " => " + Setting.ConnectionString);
                }
            }

            if (DoAppConfig)
            {
                ExeConfigurationFileMap ConfigFileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(PathArg, @"App.config") };
                ThisAppConfig = ConfigurationManager.OpenMappedExeConfiguration(ConfigFileMap, ConfigurationUserLevel.None);

                foreach (string key in ThisAppConfig.AppSettings.Settings.AllKeys)
                {
                    Trace.WriteLine(key + " => " + ThisAppConfig.AppSettings.Settings[key].Value);
                }

                Trace.WriteLine("Connection strings section:");
                foreach (ConnectionStringSettings Setting in ThisAppConfig.ConnectionStrings.ConnectionStrings)
                {
                    // TODO instead of trace outout, use the settings to compare
                    Trace.WriteLine(Setting.Name + " => " + Setting.ConnectionString);
                }
            }

            return true;
        }
    }
}
