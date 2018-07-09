using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {

    public AudioClip Confirm;
    public AudioClip Cancel;
    public AudioClip Cursor;
    public AudioClip Attack;
    public AudioClip Transition;
    public AudioClip Victory;
    public AudioClip BattleBGM;
    public AudioClip PreGameBGM;

    void Start ()
    {
        AudioListener.volume = 5f;
    }
	
	void Update () {
	
	}

    public void PlayConfirm()
    {
        AudioSource.PlayClipAtPoint(Confirm, transform.position);
    }

    public void PlayCancel()
    {
        AudioSource.PlayClipAtPoint(Cancel, transform.position);
    }

    public void PlayCursor()
    {
        AudioSource.PlayClipAtPoint(Cursor, transform.position, 0.2f);
    }

    public void PlayAttack()
    {
        AudioSource.PlayClipAtPoint(Attack, transform.position);
    }

    public void PlayTransiton()
    {
        AudioSource.PlayClipAtPoint(Transition, transform.position);
    }

    public void PlayVictory()
    {
        AudioSource.PlayClipAtPoint(Victory, transform.position);
    }

    public void PlayBattleBGM()
    {
        StartCoroutine(ReallyPlayBattleBGM());
    }

    public void PlayPreGameBGM()
    {
        GetComponent<AudioSource>().Play();
    }

    private IEnumerator ReallyPlayBattleBGM()
    {
        yield return new WaitForSeconds(3f);
        GetComponent<AudioSource>().clip = BattleBGM;
        GetComponent<AudioSource>().Play();
    }

    public void RunFadeOut()
    {
        StartCoroutine( FadeOutBGM() );
    }

    private IEnumerator FadeOutBGM()
    {
        while (GetComponent<AudioSource>().volume > 0)
        {
            GetComponent<AudioSource>().volume -= 0.05f * Time.deltaTime;
            yield return null;
        }

        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().volume = 0.08f;

        if (GameManager.instance.GameComplete)
            PlayVictory();
    }
}
