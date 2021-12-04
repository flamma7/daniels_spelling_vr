using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class knowledge_tracing : MonoBehaviour
{
    private string[] words = { "air", "day", "down", "find", "first", "long", "look", "make", "number", "one", "party", "people", "see", "two", "up", "water", "way", "words", "word" };
    private List<int> working_memory_ind; 
    private float[] knowledge;
    private float initial_knowledge = 0.01f;
    private float prob_transition = 0.15f;
    private float prob_slip = 0.2f;
    private float num_words_to_add = 5;
    private float learned_thresh = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the working memory to the first 5 words
        working_memory_ind = new List<int> { 0, 1, 2, 3, 4 };

        knowledge = new float[words.Length];
        for( int i = 0; i < words.Length; i++)
        {
            knowledge[i] = initial_knowledge;
        }

        //PrintProbabilities();
        string word = GetWord();
        Debug.Log(word);
        AddResponse(word, false);
        PrintProbabilities();
        AddResponse(word, true);
        PrintProbabilities();
        AddResponse(word, true);
        PrintProbabilities();
    }

    void PrintProbabilities()
    {
        for(int i = 0; i < working_memory_ind.Count; i++)
        {
            float val = knowledge[i];
            Debug.Log(words[i] + " : " + val.ToString());
        }
    }

    void AddResponse(string word, bool is_correct)
    {
        int ind = Array.IndexOf(words, word);
        if (ind == -1)
        {
            Debug.Log("ERROR! '" + word + "' is not a recognized word!");
            return;
        }

        float prob_guess = 1.0f / working_memory_ind.Count;
        float prior = knowledge[ind];
        float likelihood;
        if (is_correct)
        {
            likelihood = (prior * (1 - prob_slip)) / (prior * (1 - prob_slip) + (1 - prior) * prob_guess);
        } else
        {
            likelihood = (prior * prob_slip) / (prior * prob_slip + (1 - prior) * (1 - prob_guess));
        }
        float posterior = likelihood + (1 - likelihood) * prob_transition;

        // Update Knowledge
        knowledge[ind] = posterior;
    }

     string GetWord()
    {
        // Check if we're above the learned threshold for all words -> increase working memory size
        // Get selection probabilities
        // Select word
        bool all_learned = true;
        foreach(int index in working_memory_ind)
        {
            if( knowledge[index] < learned_thresh)
            {
                all_learned = false;
                break;
            }
        }

        // Extend working memory
        if( all_learned)
        {
            int last_index = working_memory_ind[working_memory_ind.Count - 1];
            int next_index = last_index + 1;
            for( int i=0; i < num_words_to_add && i+next_index < words.Length; i++)
            {
                working_memory_ind.Add(next_index + i);
            }
        }
        // Get selection probabilities
        //float[] selection_probs = new float[working_memory_ind.Count];
        //float sum = 0;
        //for( int i =0; i < working_memory_ind.Count; i++)
        //{
        //    selection_probs[i] = 1.0f / knowledge[i];
        //    sum += selection_probs[i];
        //}

        // For now just choose a random index, no question sequencing b/c there's no numpy.random.choice() easy equivalent in C#
        System.Random random = new System.Random();
        int sel = random.Next(0, working_memory_ind.Count);
        return words[sel];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
