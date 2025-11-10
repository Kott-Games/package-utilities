namespace Kott.Utilities.Progress
{
    public struct ProgressReport
    {
        public float Progress;
        public string Info;

        public ProgressReport(float progress, string info)
        {
            Progress = progress;
            Info = info;
        }
    }
}
