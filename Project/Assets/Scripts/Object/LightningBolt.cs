/*
	This script is placed in public domain. The author takes no responsibility for any possible harm.
	Contributed by Jonathan Czeck
*/
using UnityEngine;
using System.Collections;

public class LightningBolt : MonoBehaviour
{
	public Transform m_Target;
	public int m_Zigs = 100;
	public float m_Speed = 1f;
	public float m_Scale = 1f;
	public Light m_StartLight;
	public Light m_EndLight;
	
	Perlin m_Noise;
	float m_OneOverZigs;
	
	private Particle[] m_Particles;
	
	void Start()
	{
		m_OneOverZigs = 1f / (float)m_Zigs;
		particleEmitter.emit = false;

		particleEmitter.Emit(m_Zigs);
		m_Particles = particleEmitter.particles;
	}
	
	void Update ()
	{
		if (m_Noise == null)
			m_Noise = new Perlin();
			
		float timex = Time.time * m_Speed * 0.1365143f;
		float timey = Time.time * m_Speed * 1.21688f;
		float timez = Time.time * m_Speed * 2.5564f;
		
		for (int i=0; i < m_Particles.Length; i++)
		{
			Vector3 position = Vector3.Lerp(transform.position, m_Target.position, m_OneOverZigs * (float)i);
			Vector3 offset = new Vector3(m_Noise.Noise(timex + position.x, timex + position.y, timex + position.z),
										m_Noise.Noise(timey + position.x, timey + position.y, timey + position.z),
										m_Noise.Noise(timez + position.x, timez + position.y, timez + position.z));
			position += (offset * m_Scale * ((float)i * m_OneOverZigs));
			
			m_Particles[i].position = position;
			m_Particles[i].color = Color.white;
			m_Particles[i].energy = 1f;
		}
		
		particleEmitter.particles = m_Particles;
		
		if (particleEmitter.particleCount >= 30)
		{
			if (m_StartLight)
				m_StartLight.transform.position = m_Particles[0].position;
			if (m_EndLight)
				m_EndLight.transform.position = m_Particles[m_Particles.Length - 29].position;
		}
	}	
}