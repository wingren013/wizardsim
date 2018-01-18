using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SpellGenerator : MonoBehaviour {

    //The type of word

	private enum WordType
	{
		Shape,
		Magnitude,
        Elemental,
		Effect,
		Origin,
        Propagate
	};

    //Struct for defining the properties associated with spellword strings.  
    //Used this as an aid to parsing

    private class SpellWord
    {
        public WordType type;
        public string   resource;
        public bool     unlocked;
        public float    cost;
        public float    magnitude; 
        public int      maxElementals;
        public int      maxEffects;
        public SpellShape.CastOrigin origin;

        public SpellWord(WordType type, string resource, float cost, bool unlocked)
        {
            this.type = type;
            this.resource = resource;
            this.unlocked = unlocked;
            this.cost = cost;
            maxElementals = 1;
            maxEffects = 2;
        }

        public SpellWord(WordType type, float magnitude, bool unlocked)
        {
            this.type = type;
            this.unlocked = unlocked;
            this.magnitude = magnitude;
        }

        public SpellWord(WordType type, SpellShape.CastOrigin origin, bool unlocked)
        {
            this.type = type;
            this.origin = origin;
            this.unlocked = unlocked;
        }
    }

    //Dictionary containing spell definitions
    private Dictionary<string, SpellWord> _spellDictionary;

    //The maximum number of words allowed by the generator.
	private int _wordMax = 10;

    //String for the current spell
    private string _spellString;

    //Error counters
    private int _grammarErrors = 0;
    private int _magicalErrors = 0;

    //Scalar value to determine success
    private float _success = 1.0f;

    //Store our spell prefabs here for convenience

    private GameObject _missile;
    private GameObject _ray;
    private GameObject _bubble;

    //Our player wand

    [SerializeField]
    private GameObject _playerWand;
    [SerializeField]
    private GameObject _confirmEffect;
    [SerializeField]
    private GameObject _errorEffect;


    //Boolean value for whether the player is generating this spell or not.
    private bool _isplayer = true;

	// Use this for initialization
	void Start () {
		_spellString = "";
        //Set our spell definitions
        DefineSpells();

        //Load up our spell prefabs as inactive objects
        _missile = Instantiate(Resources.Load("Spells/Missile") as GameObject, this.transform);
        _missile.SetActive(false);
        _ray = Instantiate(Resources.Load("Spells/Ray") as GameObject, this.transform);
        _ray.SetActive(false);
        _bubble = Instantiate(Resources.Load("Spells/Bubble") as GameObject, this.transform);
        _bubble.SetActive(false);
    }

    //We'll use this to create a shape rather than calling it directly, since we might end up using
    //object pools if things get slow.

    GameObject SpawnInactiveShape(string shapeName)
    {
        GameObject newShape;
        SpellWord spellWord = _spellDictionary[shapeName];
        if (spellWord.type == WordType.Shape)
        {
            switch(spellWord.resource)
            {
                case "Missile":
                    newShape = Instantiate(_missile);
                    break;
                case "Ray":
                    newShape = Instantiate(_ray);
                    break;
                case "Bubble":
                    newShape = Instantiate(_bubble);
                    break;
                default:
                    Debug.Log("This shape's functionality is not complete!");
                    newShape = null;
                    break;
            }
            newShape.SetActive(false);
            return newShape;
        }
        Debug.Log(shapeName + " does not correspond with any known spell shapes.");
        newShape = null;
        return null;
    }

    //Defines all of the spell word relationships - this is the default player loadout, no spell words active
    public void DefineSpells()
    {
        _spellDictionary = new Dictionary<string, SpellWord>()
        {
            { "missile", new SpellWord(WordType.Shape, "Missile", .5f, false)},
            { "beam", new SpellWord(WordType.Shape, "Ray", .5f, false)},
            { "bubble", new SpellWord(WordType.Shape, "Bubble", .5f, false)},

            { "freezing", new SpellWord(WordType.Elemental, "FreezingEffect", .5f, false)},
            { "burning", new SpellWord(WordType.Elemental, "BurningEffect", .5f, false)},
            { "shocking", new SpellWord(WordType.Elemental, "ShockingEffect", .5f, false)},
            { "shining", new SpellWord(WordType.Elemental, "ShiningEffect", .5f, false)},

            { "levitating",new SpellWord(WordType.Effect, "LevitatingEffect", .5f, false)},
            { "magnetic",new SpellWord(WordType.Effect, "MagneticEffect", .5f, false)},
            { "time",new SpellWord(WordType.Effect, "TimeEffect", .5f, false)},
            { "scale",new SpellWord(WordType.Effect, "ScaleEffect", .5f, false)},

            { "top",new SpellWord(WordType.Origin, SpellShape.CastOrigin.Top, false)},
            { "bottom",new SpellWord(WordType.Origin, SpellShape.CastOrigin.Bottom, false)},
            { "left",new SpellWord(WordType.Origin, SpellShape.CastOrigin.Left, false)},
            { "right",new SpellWord(WordType.Origin, SpellShape.CastOrigin.Right, false)},
            { "front",new SpellWord(WordType.Origin, SpellShape.CastOrigin.Front, false)},
            { "back",new SpellWord(WordType.Origin, SpellShape.CastOrigin.Back, false)},

            { "greater",new SpellWord(WordType.Magnitude, 2.0f, false)},
            { "major",   new SpellWord(WordType.Magnitude, 3.0f, false)},

            { "lesser",new SpellWord(WordType.Magnitude, 0.5f, false)},
            { "minor",   new SpellWord(WordType.Magnitude, 0.05f, false)},

             { "impart",   new SpellWord(WordType.Propagate, 0.25f, false)},
        };
    }

    public int SpeakSpell(string spell){
        //reset our errors
        _grammarErrors = 0;
        _spellString = spell;

        //Split the spell string into words
        string[] spellWords = spell.Split(' ');

        //Verify that the spell is legal
        foreach (string word in spellWords)
        {
            if (wordUnlocked(word))
                _grammarErrors += 1;
        }
        
        //Accumulate errors if we are over the word limit
        if(spellWords.Length > _wordMax)
            _grammarErrors += _wordMax - spellWords.Length;

        //If we had errors.  Let the player know

        if(_playerWand != null)
        {
            if(_grammarErrors > 0)
            {

            }
            else
            {
                Instantiate(_confirmEffect, _playerWand.transform);
            }
        }

        return _grammarErrors;
	}

	public bool wordUnlocked(string word)
	{
        return _spellDictionary[word].unlocked;
	}
			
	public GameObject MakeSpell()
	{
        //Don't parse if we have errors.   We can change this behavior later,
        //but for now, just return an empty spell

        if (_grammarErrors > 0)
        {
            return null;
        }

        //Last shape/effect encountered
        GameObject lastShape = null;
        SpellEffect lastEffect = null;

        //Keep track of effects/elementals on a single shape
        int elementalCount = 0;
        int effectCount = 0;

        //Reset magical errors
        _magicalErrors = 0;

        //Current word and last word for the loop
        SpellWord currentWord;
        SpellWord lastWord = null;

        //First cut the complete string into words
        if (_spellString == "")
            return null;
        string[] words = _spellString.Split(' ');

        //Iterate backwards from the last word
        for (int i = words.Length - 1 ; i >= 0; i--)
        {
            currentWord = _spellDictionary[words[i]];
            //Debug.Log("[SpellGenerator]: Parsing word " + words[i] + "\"");

            //Check the type of the word
            switch (currentWord.type)
            {
                //If the word is a shape...
                case WordType.Shape:
                    //Spawn a new inactive shape and reset counters
                    GameObject currentShape;
                    elementalCount = 0;
                    effectCount = 0;
                    currentShape = SpawnInactiveShape(words[i]);

                    //If the last word was an origin, set this shape's cast origin to a non-default value.
                    if (lastWord != null)
                    {
                        if (lastWord.type == WordType.Origin)
                            currentShape.GetComponent<SpellShape>().SetCastOrigin(lastWord.origin);
                    }
                    
                    //If we have previous encountered a shape word, chain it
                    if (lastShape != null)
                        currentShape.GetComponent<SpellShape>().ChainSpell(lastShape);
                    //Set current to last for the next iteration
                    lastShape = currentShape;

                    
                    break;
                //If the word is an effect or an elemental 
                case WordType.Effect:
                case WordType.Elemental:
                    //Check to see if this effect is already attached to the current spell
                    if (lastShape != null)
                    {
                        if(currentWord.type == WordType.Elemental)
                            elementalCount++;
                        if (currentWord.type == WordType.Effect)
                            effectCount++;
                        if (elementalCount > currentWord.maxElementals)
                        {
                            Debug.Log("[SpellGenerator]: Elementals over the limit!");
                            _magicalErrors++;
                        }
                        else if (effectCount > currentWord.maxEffects)
                        {
                            Debug.Log("[SpellGenerator]: Effects over the limit!");
                            _magicalErrors++;
                        }
                        System.Type effectType = System.Type.GetType(currentWord.resource);
                        if (lastShape.GetComponent(effectType))
                        {
                            Debug.Log("[SpellGenerator]: Redundant effect ignored.");
                            _magicalErrors++;
                        }
                        else
                        {
                            Debug.Log("adding effect");
                            lastEffect = lastShape.AddComponent(effectType) as SpellEffect;
                        }
                    }
                    else
                    {
                        Debug.Log("[SpellGenerator]: Shapeless effect - aborting.");
                        _magicalErrors++;
                    }
                    break;
                //If the word is a magnitude modifier, set the last effect/shape's magnitude accordingly
                case WordType.Magnitude:
                    if (lastWord != null)
                    {
                        if (lastWord.type == WordType.Shape)
                            lastShape.GetComponent<SpellShape>().SetMagnitude(currentWord.magnitude);
                        else if (lastWord.type == WordType.Effect)
                            lastEffect.SetMagnitude(currentWord.magnitude);
                        else
                        {
                            //Return null spell here because we broke the rules.
                            Debug.Log("[SpellGenerator]: Invalid use of magnitude modifier.");
                            _magicalErrors++;
                        }
                    }
                    break;
                case WordType.Propagate:
                    Debug.Log("recognized impart");
                    if(lastWord.type != WordType.Shape && lastEffect != null)
                    {
                        Debug.Log("setting impart");
                        lastEffect.SetPropagate(true, 1);
                    }
                    break;
            }
            //Set last word to this word before the loop iterates
            lastWord = currentWord;
        }
        //Return the last shape.  This should contain the complete spell.
        
        //to-do: If there are a ton of magical errors, implement a bungled spell function and call it like so
        //   lastShape = BungledSpell(_magicalErrors);
        return lastShape;
	}

	void addPropogateEffect(string name)
	{
		

	}

	void addEffect(string name)
	{

	}
}