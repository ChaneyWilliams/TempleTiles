using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;


public class LivesCounter : MonoBehaviour
{
    public static LivesCounter instance;
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;

    private Dictionary<string, List<AudioClip>> soundDictionary;

    public int lives = 3;
    public int currentLevelIndex = 0;
    public float globalVolume = 1.0f;
    public float red = 0.0f;
    public float green = 0.0f;
    public float blue = 0.0f;

    AudioSource audioSource;


    
    private const string LivesKey = "SavedLives";
    private const string LevelKey = "SavedLevelIndex";
    private const string GlobalVolumeKey = "GlobalVolume";
    private const string RedKey = "RedTint";
    private const string GreenKey = "GreenTint";
    private const string BlueKey = "BlueTint";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved values, using defaults for a new game.
        lives = PlayerPrefs.GetInt(LivesKey, 3);
        currentLevelIndex = PlayerPrefs.GetInt(LevelKey, 0);
        globalVolume = PlayerPrefs.GetFloat(GlobalVolumeKey, 1.0f);
        red = PlayerPrefs.GetFloat(RedKey, 0.0f);
        green = PlayerPrefs.GetFloat(GreenKey, 0.0f);
        blue = PlayerPrefs.GetFloat(BlueKey, 0.0f);

        InitializeDictionary();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetLives(int newLives)
    {
        if (newLives > 99 || newLives < 0)
            return;

        lives = newLives;
        SaveProgress();
    }

    public void SubLives()
    {
        lives = Mathf.Max(0, lives - 1);
        if (lives == 0)
        {
            SceneManager.LoadScene(0); 
        }
        SaveProgress();
    }

    public int GetLives()
    {
        return lives;
    }

    public void SetGlobalVolume(float newVolume)
    {
        globalVolume = Mathf.Clamp01(newVolume);
        PlayerPrefs.SetFloat(GlobalVolumeKey, globalVolume);
        PlayerPrefs.Save();
    }
    public void SetGlobalTint(Color color)
    {
        red = color.r;
        green = color.g;
        blue = color.b;
        Debug.Log($"Saving: {red}, {green}, {blue}");
        PlayerPrefs.SetFloat(RedKey, red);
        PlayerPrefs.SetFloat(GreenKey, green);
        PlayerPrefs.SetFloat(BlueKey, blue);
        PlayerPrefs.Save();
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(LivesKey, lives);
        PlayerPrefs.SetInt(LevelKey, SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
    }

    public void SetCurrentLevelIndex(int newIndex)
    {
        currentLevelIndex = newIndex;
        SaveProgress();
    }

    void OnApplicationQuit()
    {
        SaveProgress();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveProgress();
    }

    

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach(SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            soundDictionary[soundEffectGroup.name] = soundEffectGroup.audioClip;
        }
    }

    public void PlayRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            
            List<AudioClip> audioClips = soundDictionary[name];
            if (audioClips.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, audioClips.Count);
                AudioClip selectedClip = audioClips[randomIndex];
                audioSource.volume = 1.0f;
                audioSource.PlayOneShot(selectedClip);
                Debug.Log("Playing sound: " + name);
            }
            else
            {
                Debug.Log("Sound group is empty: " + name);
            }
            return;

        }
        Debug.Log("Sound not found: " + name);
    }

    public void PlayNoise()
    {
        PlayRandomClip("PlayerMove");
    }

    [System.Serializable]
    public struct SoundEffectGroup
    {
        public string name;
        public List<AudioClip> audioClip;
    }
}