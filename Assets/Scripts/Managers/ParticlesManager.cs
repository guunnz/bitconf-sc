using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Particles
{
    public ParticleType Type;
    public ParticleSystem Particle;
}

public enum ParticleType
{
    CameraTurbo = 1,
    CameraBoss = 2,
}

public class ParticlesManager : MonoBehaviour
{
    public List<Particles> ParticleList;

    static public ParticlesManager DO;


    private void Start()
    {
        DO = this;
    }

    public void Play(ParticleType type)
    {
        ParticleList.Single(x => x.Type == type).Particle.Play();
    }

    public void Stop(ParticleType type)
    {
        ParticleList.Single(x => x.Type == type).Particle.Stop();
    }
}
