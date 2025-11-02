class MonitorState
{
    private DateTime _lastCheckTime = DateTime.Now;

    public int CheckCount { get; private set; }
    public int AcceptCount { get; private set; }
    
    public double ElapsedSeconds => (DateTime.Now - _lastCheckTime).TotalSeconds;

    public void IncrementCheck()
    {
        CheckCount++;
    }

    public void IncrementAcceptCount()
    {
        AcceptCount++;
    }

    public void ResetCheckState()
    {
        _lastCheckTime = DateTime.Now;
        CheckCount = 0;
    }
}