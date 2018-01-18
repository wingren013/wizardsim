using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPscript : WizSimBehavior
{

	public enum Element
	{
		Fire,
		Ice,
		Electric,
		Light
	};

	public Enemy_Base enemy;
	private float maxhp;
	private float currenthp;

	// Use this for initialization
	void Start () {
		maxhp = enemy.maxhp;
		currenthp = maxhp;
		enemy = gameObject.GetComponent<Enemy_Base>();
	}
	
	// Update is called once per frame
	void Update () {
		if (currenthp <= 0)
			enemy.Die();
	}

	public float GetHP()
	{
		return (currenthp);
	}

	public void SetHP(float amount)
	{
		currenthp = amount;
	}

	public float GetMaxHP()
	{
		return (maxhp);
	}

	public void SetMaxHP(float amount)
	{
		maxhp = amount;
	}

	public void doDamage(float amount, Element e)
	{
		if (!enemy)
		{
			Debug.Log("HP Script " + this + " not attached to enemy");
			damageInanimate(amount);
			return;
		}
		switch (e)
		{
			case Element.Fire:
				currenthp -= amount * enemy.fireres;
				break;
			case Element.Ice:
				currenthp -= amount * enemy.coldres;
				break;
			case Element.Electric:
				currenthp -= amount * enemy.elecres;
				break;
			case Element.Light:
				currenthp -= amount * enemy.lightres;
				break;
			default:
				Debug.Log("Untyped Damage!");
				currenthp -= amount;
				break;
		}
		if (currenthp > maxhp)
			currenthp = maxhp;
	}

	public void damageInanimate(float amount)
	{
		currenthp -= amount;
		if (currenthp > maxhp)
			maxhp = currenthp;
	}
}
