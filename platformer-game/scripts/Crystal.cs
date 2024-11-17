using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Crystal : MonoBehaviour
{
    ParticleSystem partSys;
    Animator anim;

    public AudioSource crystalHitSound;

    public int roomNr;
    float lightIntensityDefault;

    private void Start()
    {
        partSys = transform.Find("Particle System").GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();

        lightIntensityDefault = transform.Find("Light 2D").GetComponent<Light2D>().intensity;
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > .99f && anim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
            RespawnCrystal();

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            transform.Find("Light 2D").gameObject.SetActive(true);
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                transform.Find("Light 2D").GetComponent<Light2D>().intensity = lightIntensityDefault * (1 - 2 * anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            else
                transform.Find("Light 2D").GetComponent<Light2D>().intensity = lightIntensityDefault * (2 * anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 1);
        }
    }

    public void HitCrystal()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        //transform.Find("KillCollider").GetComponent<BoxCollider2D>().enabled = false;
        partSys.Play();
        anim.SetTrigger("TrReload");
        crystalHitSound.Play();
    }

   public void RespawnCrystal()
   {
        GetComponent<BoxCollider2D>().enabled = true;
        //transform.Find("KillCollider").GetComponent<BoxCollider2D>().enabled = true;
        anim.Play("Idle", -1, 0);
   }
}
