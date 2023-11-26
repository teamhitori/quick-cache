internal interface ILogPosition
{
    ulong? GetNewPosition();
    void Reset();
}