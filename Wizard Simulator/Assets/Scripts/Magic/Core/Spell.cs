using UnityEngine;

/*
 * [Spell Class]
 * 
 * Spells are magical building blocks.  All spells share some common
 * traits, such as magnitude, duration, permanence, and cost.
 * 
 * Spells take the form of either a shape or an effect.  Effects can
 * be freely attached to shapes, while shapes may be chained
 * together with some limitations.
 * 
 * Uniques is a reserved special category.  Perhaps special specific combinations
 * or unique words can trigger these un-combinable spells.
 * 
 */

public abstract class Spell : WizSimBehavior {
    // Distinguishes the two major categories of spell types
	public enum SpellType
	{
        None,
		Shape,
		Effect,
        Unique
	};

    // Enum value denoting whether this spell is a shape or an effect.  
	public SpellType spellType = SpellType.None;

    // Scalar value for this spell's strength.  Base value of 1.0f
    public float magnitude = 1.0f;

    // Scalar value for the default duration of this spell, given in seconds to live.
    public float defaultDuration = 0.0f;
    public float duration = 0.0f;

    // Boolean value denoting whether we've dispelled this or not
    protected bool dispelled;

    // Boolean value denoting whether this spell has infinite life (most likely because it is tied to a shape but sometimes we can )
    public bool permanent = false;

    // Sets spell magnitude
	public virtual void SetMagnitude (float scale)
	{
		magnitude = scale;
	}

    // Increases spell magnitude
	public virtual void IncreaseMagnitude (float amount)
	{
		magnitude += amount;
	}

    // Resets the duration
    public void Refresh()
    {
        duration = defaultDuration;
    }

    private void Start()
    {
        Refresh();
        SpellStart();
    }

    // Start for derived spell types
    public abstract void SpellStart();

    // Update for derived spell types
    public abstract void SpellUpdate();

    // This is our cleanup/death function
    public abstract void Dispel();

    // Update is called once per frame
    public void Update()
	{
        // If spell is not permanent, subtract from its time to live.
        if (!permanent)
        {
            duration -= this.timeScale * Time.deltaTime;
            if (duration <= 0.0f)
            {
                dispelled = true;
                Dispel();
            }
        }
        // Update our spell
        SpellUpdate();
	}
}
