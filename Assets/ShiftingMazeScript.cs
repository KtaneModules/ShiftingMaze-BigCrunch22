using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class ShiftingMazeScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    public AudioClip[] SFX;
	public AudioSource Digger;

    public KMSelectable[] Steps;
    public GameObject[] Stepping;
    public GameObject[] Steppers;
    public TextMesh Seedling;
    public KMSelectable SendIt;

    private int[][] Copper = new int[3][]{
        new int[2] {0, 0},
        new int[2] {0, 0},
        new int[2] {0, 0}
    };

    private bool MovingAgain = false;

    string Kelp;

    // Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        for (int k = 0; k < 5; ++k)
        {
            int Movement = k;
            Steps[Movement].OnInteract += delegate
            {
                Moving(Movement);
                return false;
            };
        }

        for (int a = 0; a < 5; ++a)
        {
            int Pressing = a;
            Steps[Pressing].OnHighlight += delegate
            {
                Selected(Pressing);
            };
        }

        for (int b = 0; b < 5; ++b)
        {
            int Depressing = b;
            Steps[Depressing].OnHighlightEnded += delegate
            {
                Deselected(Depressing);
            };
        }

        SendIt.OnInteract += delegate () { Sender(); return false; };
    }

    void Start()
    {
        Coordinance();
        Random();
    }

    string[] Alfa = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" }; //Center
    string[] Bravo = { "E", "K", "L" }; //Top Left
    string[] Charlie = { "F", "L", "M" }; //Top Right
    string[] Delta = { "H", "K", "N" }; //Bottom Left
    string[] Echo = { "G", "M", "N" }; //Bottom Right
    string[] Foxtrot = { "D", "E", "F", "J", "K", "L", "M" }; //Top
    string[] Golf = { "B", "G", "H", "J", "K", "M", "N" }; //Bottom
    string[] Hotel = { "A", "F", "G", "I", "L", "M", "N" }; //Right
    string[] India = { "C", "E", "H", "I", "K", "L", "N" }; //Left

    void Selected(int Pressing)
    {
        Stepping[Pressing].SetActive(true);
    }

    void Deselected(int Depressing)
    {
        Stepping[Depressing].SetActive(false);
    }

    void Random()
    {
        if (Copper[0][0] == 0 && Copper[0][1] == 0)
        {
            Kelp = Bravo[UnityEngine.Random.Range(0, Bravo.Count())];
        }

        else if (Copper[0][0] == 0 && Copper[0][1] == 5)
        {
            Kelp = Charlie[UnityEngine.Random.Range(0, Charlie.Count())];
        }

        else if (Copper[0][0] == 5 && Copper[0][1] == 0)
        {
            Kelp = Delta[UnityEngine.Random.Range(0, Delta.Count())];
        }

        else if (Copper[0][0] == 5 && Copper[0][1] == 5)
        {
            Kelp = Echo[UnityEngine.Random.Range(0, Echo.Count())];
        }

        else if (Copper[0][0] == 0)
        {
            Kelp = Foxtrot[UnityEngine.Random.Range(0, Foxtrot.Count())];
        }

        else if (Copper[0][0] == 5)
        {
            Kelp = Golf[UnityEngine.Random.Range(0, Golf.Count())];
        }

        else if (Copper[0][1] == 5)
        {
            Kelp = Hotel[UnityEngine.Random.Range(0, Hotel.Count())];
        }

        else if (Copper[0][1] == 0)
        {
            Kelp = India[UnityEngine.Random.Range(0, India.Count())];
        }

        else
        {
            Kelp = Alfa[UnityEngine.Random.Range(0, Alfa.Count())];
        }

		if (Kelp == "A")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: East", moduleId);
		}
		
		else if (Kelp == "B")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: South", moduleId);
		}
		
		else if (Kelp == "C")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: West", moduleId);
		}
		
		else if (Kelp == "D")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: North", moduleId);
		}
		
		else if (Kelp == "E")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: North/West", moduleId);
		}
		
		else if (Kelp == "F")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: North/East", moduleId);
		}
		
		else if (Kelp == "G")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: South/East", moduleId);
		}
		
		else if (Kelp == "H")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: South/West", moduleId);
		}
		
		else if (Kelp == "I")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: East/West", moduleId);
		}
		
		else if (Kelp == "J")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: North/South", moduleId);
		}
		
		else if (Kelp == "K")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: South/West/North", moduleId);
		}
		
		else if (Kelp == "L")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: West/North/East", moduleId);
		}
		
		else if (Kelp == "M")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: North/East/South", moduleId);
		}
		
		else if (Kelp == "N")
		{
			Debug.LogFormat("[Shifting Maze #{0}] Current wall on your cell: East/South/West", moduleId);
		}
		
		else if (Kelp == "O")
		{
			Debug.LogFormat("[Swtiching Maze #{0}] Current wall on your cell: -", moduleId);
		}
    }

    void Sender()
    {
        if (MovingAgain == false)
        {
            StartCoroutine(ActualStep());
        }
    }

    void Moving(int Movement)
    {
        if (MovingAgain == false)
        {
            if (Movement == 0)
            {
				Debug.LogFormat("[Shifting Maze #{0}] You moved north.", moduleId);
                if (Copper[0][0] == 0 || Kelp == "D" || Kelp == "E" || Kelp == "F" || Kelp == "J" || Kelp == "K" || Kelp == "L" || Kelp == "M")
                {
                    StartCoroutine(Incorrect());
                }

                else
                {
                    Copper[0][0] -= 1;
                    for (int q = 0; q < 6; q++)
                    {
                        Steppers[q].SetActive(false);
                    }
                    StartCoroutine(Stepyard());
					Debug.LogFormat("[Shifting Maze #{2}] Your are currently on: {0}, {1}", Copper[0][0].ToString(), Copper[0][1].ToString(), moduleId);
					Random();
                }
            }

            if (Movement == 1)
            {
				Debug.LogFormat("[Shifting Maze #{0}] You moved south.", moduleId);
                if (Copper[0][0] == 5 || Kelp == "B" || Kelp == "G" || Kelp == "H" || Kelp == "J" || Kelp == "K" || Kelp == "M" || Kelp == "N")
                {
                    StartCoroutine(Incorrect());
                }

                else
                {
                    Copper[0][0] += 1;
                    for (int q = 0; q < 6; q++)
                    {
                        Steppers[q].SetActive(false);
                    }
					Debug.LogFormat("[Shifting Maze #{2}] Your are currently on: {0}, {1}", Copper[0][0].ToString(), Copper[0][1].ToString(), moduleId);
                    StartCoroutine(Stepyard());
					Random();
                }
            }

            if (Movement == 2)
            {
				Debug.LogFormat("[Shifting Maze #{0}] You moved east.", moduleId);
                if (Copper[0][1] == 5 || Kelp == "A" || Kelp == "F" || Kelp == "G" || Kelp == "I" || Kelp == "L" || Kelp == "M" || Kelp == "N")
                {
                    StartCoroutine(Incorrect());
                }

                else
                {
                    Copper[0][1] += 1;
                    for (int q = 0; q < 6; q++)
                    {
                        Steppers[q].SetActive(false);
                    }
					Debug.LogFormat("[Shifting Maze #{2}] Your are currently on: {0}, {1}", Copper[0][0].ToString(), Copper[0][1].ToString(), moduleId);
                    StartCoroutine(Stepyard());
					Random();
                }
            }

            if (Movement == 3)
            {
				Debug.LogFormat("[Shifting Maze #{0}] You moved west.", moduleId);
                if (Copper[0][1] == 0 || Kelp == "C" || Kelp == "E" || Kelp == "H" || Kelp == "I" || Kelp == "K" || Kelp == "L" || Kelp == "N")
                {
                    StartCoroutine(Incorrect());
                }

                else
                {
                    Copper[0][1] -= 1;
                    for (int q = 0; q < 6; q++)
                    {
                        Steppers[q].SetActive(false);
                    }
                    StartCoroutine(Stepyard());
					Debug.LogFormat("[Shifting Maze #{2}] Your are currently on: {0}, {1}", Copper[0][0].ToString(), Copper[0][1].ToString(), moduleId);
					Random();
                }
            }

            if (Movement == 4)
            {
                StartCoroutine(Dinger());
            }
        }
    }

    IEnumerator Dinger()
    {
        MovingAgain = true;
		UsingMic = true;
		Debug.LogFormat("[Shifting Maze #{0}] You used your sound frequency generator.", moduleId);
        for (int q = 0; q < 6; q++)
        {
            Steppers[q].SetActive(false);
        }
        if (Kelp == "A")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            Stepping[0].SetActive(false);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            Stepping[2].SetActive(false);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            Stepping[1].SetActive(false);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            Stepping[3].SetActive(false);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "B")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "C")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "D")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "E")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "F")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "G")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "H")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "I")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "J")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "K")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "L")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "M")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }

        else if (Kelp == "N")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[2].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }
		
		else if (Kelp == "O")
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
            Audio.PlaySoundAtTransform(SFX[1].name, transform);
            yield return new WaitForSecondsRealtime(0.4f);
        }
		
        for (int q = 0; q < 4; q++)
        {
            Steppers[q].SetActive(true);
        }
        Steppers[5].SetActive(true);
        MovingAgain = false;
		UsingMic = false;
		MicUsed = true;
    }

    IEnumerator Stepyard()
    {
        MovingAgain = true;
		TakingAStep = true;
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        yield return new WaitForSecondsRealtime(0.65f);
        for (int q = 0; q < 6; q++)
        {
            Steppers[q].SetActive(true);
        }
        MovingAgain = false;
		MicUsed = false;
		TakingAStep = false;
    }

    string[] Alphabreak = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "/" };
    void Coordinance()
    {
        Seedling.text = "SEED: ";
        for (int a = 0; a < 2; a++)
        {
            for (int b = 0; b < 2; b++)
            {
                for (int c = 0; c < 2; c++)
                {
                    Copper[2][c] = UnityEngine.Random.Range(0, Alphabreak.Count());
                    Seedling.text += Alphabreak[Copper[2][c]];
                }
                Copper[a][b] = ((Copper[2][0] * 64) + (Copper[2][1])) % 6;
            }
        }
		Debug.LogFormat("[Shifting Maze #{0}] {1}", moduleId, Seedling.text);
        Debug.LogFormat("[Shifting Maze #{2}] Your starting coordinance is: {0}, {1}", Copper[0][0].ToString(), Copper[0][1].ToString(), moduleId);
        Debug.LogFormat("[Shifting Maze #{2}] Your destination is: {0}, {1}", Copper[1][0].ToString(), Copper[1][1].ToString(), moduleId);
    }

    IEnumerator Incorrect()
    {
		Debug.LogFormat("[Shifting Maze #{0}] You slammed on a wall. The mazing is now moving.", moduleId);
        MovingAgain = true;
		MazeMoving = true;
        for (int q = 0; q < 6; q++)
        {
            Steppers[q].SetActive(false);
        }
        Audio.PlaySoundAtTransform(SFX[3].name, transform);
        yield return new WaitForSecondsRealtime(1.8f);
		Digger.clip = SFX[4];
		Digger.Play();
        while (Digger.isPlaying)
		{
            Seedling.text = "SEED: ";
            for (int f = 0; f < 8; f++)
            {
                int Seedlings = UnityEngine.Random.Range(0, Alphabreak.Count());
                Seedling.text += Alphabreak[Seedlings];
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
        Audio.PlaySoundAtTransform(SFX[5].name, transform);
        Module.HandleStrike();
		Debug.LogFormat("[Shifting Maze #{0}] A strike was given to you.", moduleId);
        Coordinance();
        Random();
        for (int q = 0; q < 6; q++)
        {
            Steppers[q].SetActive(true);
        }
        MovingAgain = false;
		MazeMoving = false;
		MicUsed = false;
    }

    IEnumerator ActualStep()
    {
		Debug.LogFormat("[Shifting Maze #{0}] You activated your current platform. The maze is now moving.", moduleId);
        MovingAgain = true;
		MazeMoving = true;
        for (int q = 0; q < 6; q++)
        {
            Steppers[q].SetActive(false);
        }
        Audio.PlaySoundAtTransform(SFX[6].name, transform);
        yield return new WaitForSecondsRealtime(.75f);
		Digger.clip = SFX[4];
		Digger.Play();
		while (Digger.isPlaying)
		{
            Seedling.text = "SEED: ";
            for (int f = 0; f < 8; f++)
            {
                int Seedlings = UnityEngine.Random.Range(0, Alphabreak.Count());
                Seedling.text += Alphabreak[Seedlings];
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
        if (Copper[0][0] == Copper[1][0] && Copper[0][1] == Copper[1][1])
        {
            Audio.PlaySoundAtTransform(SFX[5].name, transform);
            Module.HandlePass();
            ModuleSolved = true;
            Seedling.text = "";
            yield return new WaitForSecondsRealtime(1f);
            Audio.PlaySoundAtTransform(SFX[7].name, transform);
			Debug.LogFormat("[Shifting Maze #{0}] You stepped the correct platform. Module solved.", moduleId);
        }
        else
        {
            Audio.PlaySoundAtTransform(SFX[5].name, transform);
            Module.HandleStrike();
			Debug.LogFormat("[Shifting Maze #{0}] You stepped on an incorrect platform. A strike was given.", moduleId);
            Coordinance();
            Random();
            for (int q = 0; q < 6; q++)
            {
                Steppers[q].SetActive(true);
            }
            MovingAgain = false;
        }
		MazeMoving = false;
		MicUsed = false;
    }
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To move in the maze, use the command !{0} north/south/east/west or up/down/left/right | To use the sound frequency generator, use the command !{0} microphone | To activate your current platform, use the command !{0} set";
    #pragma warning restore 414
	
	bool UsingMic = false;
	bool MicUsed = false;
	bool TakingAStep = false;
	bool MazeMoving = false;
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(command, @"^\s*mic\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*microphone\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (TakingAStep == true)
			{
				yield return "sendtochaterror You are currently moving in the maze. The command was not processed.";
				yield break;
			}
			
			else if (MazeMoving == true)
			{
				yield return "sendtochaterror The maze is currently moving. The command was not processed.";
				yield break;
			}
			
			else if (MicUsed == true)
			{
				yield return "sendtochaterror You already used your sound frequency generator. The command was not processed.";
				yield break;
			}
			Steps[4].OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*up\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*north\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*u\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*n\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UsingMic == true)
			{
				yield return "sendtochaterror You are currently using your sound frequency generator. The command was not processed.";
				yield break;
			}
			
			else if (TakingAStep == true)
			{
				yield return "sendtochaterror You are currently moving in the maze. The command was not processed.";
				yield break;
			}
			
			else if (MazeMoving == true)
			{
				yield return "sendtochaterror The maze is currently moving. The command was not processed.";
				yield break;
			}
			yield return "strike";
			Steps[0].OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*down\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*south\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*d\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*s\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UsingMic == true)
			{
				yield return "sendtochaterror You are currently using your sound frequency generator. The command was not processed.";
				yield break;
			}
			
			else if (TakingAStep == true)
			{
				yield return "sendtochaterror You are currently moving in the maze. The command was not processed.";
				yield break;
			}
			
			else if (MazeMoving == true)
			{
				yield return "sendtochaterror The maze is currently moving. The command was not processed.";
				yield break;
			}
			yield return "strike";
			Steps[1].OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*east\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*r\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*e\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UsingMic == true)
			{
				yield return "sendtochaterror You are currently using your sound frequency generator. The command was not processed.";
				yield break;
			}
			
			else if (TakingAStep == true)
			{
				yield return "sendtochaterror You are currently moving in the maze. The command was not processed.";
				yield break;
			}
			
			else if (MazeMoving == true)
			{
				yield return "sendtochaterror The maze is currently moving. The command was not processed.";
				yield break;
			}
			yield return "strike";
			Steps[2].OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*west\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*l\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*w\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UsingMic == true)
			{
				yield return "sendtochaterror You are currently using your sound frequency generator. The command was not processed.";
				yield break;
			}
			
			else if (TakingAStep == true)
			{
				yield return "sendtochaterror You are currently moving in the maze. The command was not processed.";
				yield break;
			}
			
			else if (MazeMoving == true)
			{
				yield return "sendtochaterror The maze is currently moving. The command was not processed.";
				yield break;
			}
			yield return "strike";
			Steps[3].OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UsingMic == true)
			{
				yield return "sendtochaterror You are currently using your sound frequency generator. The command was not processed.";
				yield break;
			}
			
			else if (TakingAStep == true)
			{
				yield return "sendtochaterror You are currently moving in the maze. The command was not processed.";
				yield break;
			}
			
			else if (MazeMoving == true)
			{
				yield return "sendtochaterror The maze is currently moving. The command was not processed.";
				yield break;
			}
			yield return "solve";
			yield return "strike";
			SendIt.OnInteract();
		}
	}
}
