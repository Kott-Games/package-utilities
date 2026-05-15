using System;

namespace Kott.Utilities.Progress
{
    public class SubProgress : IProgress<ProgressReport>
    {
        private readonly IProgress<ProgressReport> _progress;

        public ProgressReport Start;
        public float Scale;

        public SubProgress(IProgress<ProgressReport> progress)
        {
            _progress = progress;
        }

        public void Report(ProgressReport report)
        {
            report.Info = CombineInfo(report.Info);
            report.Progress = Start.Progress + report.Progress * Scale;
            _progress?.Report(report);
        }

        private string CombineInfo(string info)
        {
            if (string.IsNullOrEmpty(Start.Info)) return info;
            if (string.IsNullOrEmpty(info)) return Start.Info;
            return $"{Start.Info} - {info}";
        }
    }
}
