using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Thinktecture.IdentityServer.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            // Get the default initial configuration for DiagnosticMonitor.
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();

            // Filter the logs so that only error-level logs are transferred to persistent storage.
            config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = config.Logs.ScheduledTransferLogLevelFilter =
                config.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

            // Schedule a transfer period of 30 minutes.
            config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = config.Logs.ScheduledTransferPeriod = config.WindowsEventLog.ScheduledTransferPeriod =
                config.Directories.ScheduledTransferPeriod = config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            /*
            // Specify a buffer quota.
            config.DiagnosticInfrastructureLogs.BufferQuotaInMB = config.Logs.BufferQuotaInMB = config.WindowsEventLog.BufferQuotaInMB =
                config.Directories.BufferQuotaInMB = config.PerformanceCounters.BufferQuotaInMB = 512;

            // Set an overall quota of 8GB maximum size.
            config.OverallQuotaInMB = 8192;
            */

            // WindowsEventLog data buffer being added to the configuration, which is defined to collect event data from the System and Application channel
            config.WindowsEventLog.DataSources.Add("System!*");
            config.WindowsEventLog.DataSources.Add("Application!*");

            // Use 30 seconds for the perf counter sample rate.
            TimeSpan perfSampleRate = TimeSpan.FromSeconds(30D);

            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Memory\Available Bytes",
                SampleRate = perfSampleRate
            });

            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = perfSampleRate
            });

            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Applications Running",
                SampleRate = perfSampleRate
            });



            // Start the DiagnosticMonitor using the diagnosticConfig and our connection string.
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);

            return base.OnStart();
        }

    }
}