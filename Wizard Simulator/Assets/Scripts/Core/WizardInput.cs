using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;


    public class WizardInput : WizSimBehavior
    {
        public Hand l_hand;
        public Hand r_hand;
        public Animator l_hand_animator;
        public Animator r_hand_animator;

        public GameObject loadedSpell;
        public SpellGenerator generator;
        public GameObject model;
        public GameObject spellgun;
        public magicCharge charger;
        public GameObject dummy;
        public GameObject cameraparent;
        public SpeechHandler handler;
        private float charge = 0.0f;
        private float chargetime = 0.0f;
        private bool charged = false;
        public float mouseSensitivity = 5.0f;
        public float pitch = 1.0f;
        // Use this for initialization

        void Start()
        {
            generator = GameObject.Find("Spell Generator").GetComponent<SpellGenerator>();
        }


    //[ClientCallback]
    void Update()
    {
        if (r_hand.controller != null)
        {

            if(r_hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                r_hand_animator.SetBool("Squeeze", true);
            if(r_hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
                r_hand_animator.SetBool("Squeeze", false);
            if (r_hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                r_hand_animator.SetBool("TriggerHeld", true);
            if (r_hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                r_hand_animator.SetBool("TriggerHeld", false);

            if (r_hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && loadedSpell != null)
                charger.Charge();
            if (r_hand.controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) && loadedSpell != null)
            {
                if (charge < chargetime && charged == false)
                {
                    charger.IncreasePitch();
                    charge += 1.0f * Time.deltaTime;
                }
                if (charge >= chargetime && charged == false)
                {
                    charged = true;
                    charger.ChargeComplete();
                }
            }
            if (r_hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && loadedSpell != null)
            {
                if (charge >= chargetime)
                {
                    FireSpell(spellgun.transform.position, transform.rotation);
                }
                else
                {
                    Debug.Log("MISFIRE");
                }
                charger.Stop();
                charged = false;
                charge = 0.0f;
            }
        }

        if (l_hand.controller != null)
        {
            if (l_hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                l_hand_animator.SetBool("Squeeze", true);
            if (l_hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
                l_hand_animator.SetBool("Squeeze", false);
            if (l_hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                handler.StartListening();
                charger.StartListen();
                l_hand_animator.SetBool("Listening", true);
            }
            if (l_hand.controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                charger.Listen();
            if (l_hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                l_hand_animator.SetBool("Listening", false);
                handler.StopListening();
                charger.StopListen();
                loadedSpell = generator.MakeSpell();
                if (loadedSpell != null)
                {
                    charger.GetComponent<magicCharge>().SetChargeTime(loadedSpell.GetComponent<SpellShape>().GetCastTime());
                    chargetime = loadedSpell.GetComponent<SpellShape>().GetCastTime();
                }
            }
            
        }
    }



void FireSpell(Vector3 pos, Quaternion rot)
{

GameObject spell = Instantiate(loadedSpell, pos, rot) as GameObject;

SpellShape.SpellShapeType type = spell.GetComponent<SpellShape>().GetShapeType();
Vector3 distance = Vector3.zero;

switch (type)
{
case SpellShape.SpellShapeType.Ray:
    distance = Vector3.zero;
    spell.transform.rotation = spellgun.transform.rotation;
    spell.transform.parent = spellgun.transform;
    break;

case SpellShape.SpellShapeType.Missile:
    distance = spell.GetComponent<MissileShape>().getRadius() * spellgun.transform.TransformDirection(new Vector3(0, 0, 1));
    spell.GetComponent<Rigidbody>().velocity = spellgun.transform.TransformDirection(new Vector3(0, 0, 10.0f));
    break;
}

spell.transform.localPosition += distance;




//spell.GetComponent<Rigidbody>().velocity = transform.TransformDirection (new Vector3 (0, 0, 10.0f));
spell.name = "Spell";
spell.SetActive(true);
}

}
