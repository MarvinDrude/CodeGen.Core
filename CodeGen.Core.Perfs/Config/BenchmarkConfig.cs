using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace CodeGen.Core.Perfs.Config;

public sealed class BenchmarkConfig : ManualConfig
{
   public BenchmarkConfig()
   {
      SummaryStyle = BenchmarkDotNet.Reports.SummaryStyle.Default
         .WithRatioStyle(RatioStyle.Trend);
   }
}