using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DamageTextAnimation : MonoBehaviour
    {
        public bool alive = true;

    private void Start()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 2.0f, 0);
    }

    // Update is called once per frame
    void Update()
        {
            if (!alive)
                Destroy(this.gameObject);
        }

        public void SetDamageText(int amount)
        {
            GetComponent<TMPro.TextMeshPro>().text = amount.ToString();
        }
    }