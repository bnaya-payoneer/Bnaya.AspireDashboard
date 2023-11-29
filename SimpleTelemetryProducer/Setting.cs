using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTelemetryProducer;

internal static class Setting
{
    private static string DT_API_URL = ""; // TODO: Provide your SaaS/Managed URL here
    private static string DT_API_TOKEN = ""; // TODO: Provide the OpenTelemetry-scoped access token here

    private const string activitySource = "Dynatrace.Bnaya.Sample"; // TODO: Provide a descriptive name for your application here
    public static readonly ActivitySource MyActivitySource = new ActivitySource(activitySource);
    private static ILoggerFactory loggerFactoryOT;
}
