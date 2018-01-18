using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//http://catlikecoding.com/unity/tutorials/object-pools/
public class SpellPool : MonoBehaviour {

	PooledSpell pspell;
	List<PooledSpell> spell_list = new List<PooledSpell>();

	public static SpellPool GetPool(PooledSpell pooledspell)
	{
		GameObject obj;
		SpellPool pool;
		if (Application.isEditor)
		{
			obj = GameObject.Find(pooledspell.name + " Pool");
			if (obj)
			{
				pool = obj.GetComponent<SpellPool>();
				if (pool)
				{
					return (pool);
				}
			}
		}
		obj = new GameObject(pooledspell.name + " Pool"); //creates our pool objects
		pool = obj.AddComponent<SpellPool>();
		pool.pspell = pooledspell;
		return (pool);
	}

	public PooledSpell GetSpell()
	{
		PooledSpell spell;
		int i = spell_list.Count - 1;
		if (i >= 0)
		{
			spell = spell_list[i];
			spell_list.RemoveAt(i);
			spell.gameObject.SetActive(true);
		}
		else
		{
			spell = Instantiate<PooledSpell>(pspell);
			spell.transform.SetParent(transform, false);
			spell.pool = this;
		}
		return (spell);
	}
	public void Add(PooledSpell spell)
	{
		spell.gameObject.SetActive(false);
		spell_list.Add(spell);
	}
}
