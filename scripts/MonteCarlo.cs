using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class MonteCarlo : Resource
{   
    public int progress = 0;
    public double estimatePi(int interval)
    {
        return shots(interval) * 4.0 / interval;
    }

    public bool shoot()
    {
        double rand_x = Random.Shared.NextDouble(), rand_y = Random.Shared.NextDouble();
        return rand_x * rand_x + rand_y * rand_y <= 1.0;
    }

    public virtual int shots(int interval)
    {
        int hits = 0;

        for (progress = 0; progress < interval; progress++)
        {
            if (shoot()) hits++;
        }

        return hits;
    }
}
