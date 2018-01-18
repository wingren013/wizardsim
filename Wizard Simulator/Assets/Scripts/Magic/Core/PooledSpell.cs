using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledSpell : WizSimBehavior {

	public SpellPool pool;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[System.NonSerialized]
	SpellPool prefabpool;

	public T GetFromPool<T>() where T : PooledSpell
	{
		if (!prefabpool)
			prefabpool = SpellPool.GetPool(this);
		return (T)prefabpool.GetSpell();
	}

	public void Die()
	{
		if (pool)
			pool.Add(this);
		else
			Destroy(gameObject);
	}
}
