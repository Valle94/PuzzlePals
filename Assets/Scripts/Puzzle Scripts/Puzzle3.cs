using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle3 : MonoBehaviour
{
    [Header("Puzzle Params")]
    [SerializeField] int sequenceLength = 5;

    [Header("Puzzle Objects")]
    [SerializeField] Button[] buttons;

    private List<int> initialSequence = new List<int>();
    private List<int> userSequence = new List<int>();


    private enum State { Beginning, Playing, Waiting, Finished }

    private State currentState = State.Beginning;

    void Start()
    {
        Invoke("PlaySequence2", 0.0f);
    }

    void Update()
    {

    }

    void GenerateSequence()
    {
        initialSequence.Clear();

        for (int i = 0; i < sequenceLength; i++)
        {
            initialSequence.Add(Random.Range(1, 5));
        }
        Debug.Log($"Initial Sequence: {string.Join(", ", initialSequence)}");
    }

    public async void BeginPuzzle()
    {
        GenerateSequence();

        for (int i = initialSequence.Count - 2; i <= initialSequence.Count; i++)
        {
            Debug.Log($"Printing Sequence Length {i}");
            StartCoroutine(PlaySequence(initialSequence, i));
            StartCoroutine(GetPlayerResponse(i));
        }
    }

    public void AddToSequence(int index)
    {
        if (buttons[index].IsPressed())
        {

        }
    }

    IEnumerator PlaySequence(List<int> initialSeuquence, int length)
    {
        currentState = State.Playing;
        for (int i = 0; i < length; i++)
        {
            Debug.Log(initialSequence[i]);
            yield return new WaitForSeconds(1.0f);
        }
    }

    async Task PlaySequence2(List<int> initialSeuquence, int length)
    {
        for (int i = 0; i < length; i++)
        {
            Debug.Log(initialSequence[i]);
            await Task.Delay(1000);
        }
    }

    IEnumerator GetPlayerResponse(int length)
    {
        Debug.Log("Waiting for player response...");
        yield return new WaitUntil(() => userSequence.Count == length); // use this to wait for player 2 input
    }
}
