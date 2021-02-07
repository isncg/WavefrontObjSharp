using System;
using System.Collections;
using System.Threading;
public static class Time
{
    public static readonly FrameStatics frameStatics = new FrameStatics();
    public static readonly FpsFilter fpsFilter = new FpsFilter();
    public static float DeltaTime => (float)frameStatics.deltaTicks / TimeSpan.TicksPerSecond;
    public static float Fps => TimeSpan.TicksPerSecond / (float)frameStatics.deltaTicks;
    public static float FilteredFps => fpsFilter.Filter(frameStatics.deltaTicks);//TimeSpan.TicksPerSecond / (float)frameStatics.deltaTicks;
    public static double RunningTime => frameStatics.curTicks / (double)TimeSpan.TicksPerSecond;
}

public class FrameStatics
{
    public long curTicks = 0;
    public long deltaTicks = 0;
    private long begin = 0;
    public void Update()
    {
        if (begin <= 0)
        {
            begin = DateTime.Now.Ticks;
            deltaTicks = 0;
            curTicks = 0;
        }
        else
        {
            var curTicks = DateTime.Now.Ticks - begin;
            deltaTicks = curTicks - this.curTicks;
            this.curTicks = curTicks;
        }
    }
}

public class FpsFilter
{
    public float lastFps = 0;
    private long ticksSum = 0;
    private int sumCount = 0;
    public float Filter(long ticks)
    {
        ticksSum += ticks;
        sumCount++;
        if (ticksSum >= TimeSpan.TicksPerSecond >> 2)
        {
            lastFps = (TimeSpan.TicksPerSecond * sumCount) / (float)ticksSum;
            ticksSum = 0;
            sumCount = 0;
        }
        return lastFps;
    }
}