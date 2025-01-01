using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Bubble : MonoBehaviour
{
    public ParticleSystem particles;

    public List<ParticleSystem.Particle> colliders = new List<ParticleSystem.Particle>();

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        int count = particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, colliders, out var data);

        if(count > 0)
        {
            for(int i = 0; i < count; i ++)
            {
                var p = colliders[i];
                p.remainingLifetime = 1f;
                p.velocity.Set(p.velocity.x, 0, p.velocity.y);
                colliders[i] = p;
            }

            particles.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, colliders);
        }
    }
}
