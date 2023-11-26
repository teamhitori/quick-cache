using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class LogPosition : ILogPosition
{
    private ulong _position;

    public LogPosition(ulong seed = 0)
    {
        _position = seed;
    }

    public ulong? GetNewPosition()
    {
        var value = Interlocked.Increment(ref _position);

        return value == 0 ? null : value;
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _position, 0);
    }
}
