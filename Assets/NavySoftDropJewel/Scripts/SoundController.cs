using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    [SerializeField]
    private AudioSource audioSound;
    [SerializeField]
    private AudioSource BGM;
 
    [SerializeField]
    private Sprite soundOn;
    [SerializeField]
    private Sprite soundOff;
    [SerializeField]
    private Sprite musicOn;
    [SerializeField]
    private Sprite musicOff;
 
    [SerializeField]
    private List<AudioClip> ClipComboSound;
    [SerializeField]
    private Image[] music;
    [SerializeField]
    private Image [] sound;
    [SerializeField]
    private AudioClip button;
    [SerializeField]
    private AudioClip clipGameOver;
    [SerializeField]
    private AudioClip clipExplosion;
    [SerializeField]
    private AudioClip clipBlockBroken;
    [SerializeField]
    private AudioClip clipBlockPlace;
    [SerializeField]
    private AudioClip clipBlockMiss;
    private void Awake()
    {
        instance = this;
        DataInGame.GetData();
    }

	private void Start()
	{
		audioSound.volume = DataInGame.IsSoundSave;
        bool muteSound = (DataInGame.IsSoundSave > 0) ? true : false;
        bool muteMusic = (DataInGame.IsMusicSave > 0) ? true : false;
      
        SoundPower(DataInGame.IsSoundSave);
        MusicPower(DataInGame.IsMusicSave);

        for (int i = 0; i < music.Length; i++)
        {
            music[i].sprite = (muteMusic) ? musicOn : musicOff;
            sound[i].sprite = (muteSound) ? soundOn : soundOff;
        }
 

    }

    public void OnEventMusic()
    {
        DataInGame.IsMusicSave = (DataInGame.IsMusicSave > 0) ? 0 : 1;
        bool mute = (DataInGame.IsMusicSave > 0) ? true : false;
        MusicPower(DataInGame.IsMusicSave);


        for (int i = 0; i < music.Length; i++)
        {
            music[i].sprite = (mute) ? musicOn : musicOff;
          
        }


    }


    public void OnEventSound( )
    {

        DataInGame.IsSoundSave = (DataInGame.IsSoundSave > 0) ? 0 : 1;
        bool mute = (DataInGame.IsSoundSave > 0) ? true : false;
        SoundPower(DataInGame.IsSoundSave);
        for (int i = 0; i < sound.Length; i++)
        {
            sound[i].sprite = (mute) ? soundOn : soundOff;

        }
    
       

    }

    public void ExplosionSound()
    {
        audioSound.PlayOneShot(clipExplosion);
    }
    public void GameOverSound()
    {
        audioSound.PlayOneShot(clipGameOver);
    }
    public void BlockBroken()
    {
        audioSound.PlayOneShot(clipBlockBroken);
    }
    public void BlockMiss()
    {
        audioSound.PlayOneShot(clipBlockMiss);
    }
    public void BlockPlace()
    {
        audioSound.PlayOneShot(clipBlockPlace);
    }
    public void SoundPower(float power)
	{
		audioSound.volume = power;
		DataInGame.IsSoundSave = power;
		PlayerPrefs.SetFloat("IsSoundSave", power);
	}

    public void MusicPower(float  power)
    {
        this.BGM.volume = power;
        DataInGame.IsMusicSave = power;
        PlayerPrefs.SetFloat("IsMusicSave", power);
    }

    public void OnEventButtonSound()
	{
		audioSound.PlayOneShot(button);
	}

	 

	public void ComboSound(int num)
	{
		audioSound.PlayOneShot(ClipComboSound[num]);
	}

	public void SoundStop()
	{
		if (audioSound.isPlaying)
		{
			audioSound.Stop();
		}
	}

	public void SoundPlay()
	{
		audioSound.Play();
	}
}
