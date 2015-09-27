/*  

The goal of this code is to demonstrate the performance differences
in several different implementations of dictionaries. Each dictionary
serves the purpose of reading in a series of words from a list and
storing the number of occurences of each word.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Collections;

/* WordCount class is used throughout this project.
   It offers the ability to store a string and an integer.
   These two member variables become the key and value respectively
   in the list implementation of a Dictionary. */


/* Supports the IComparable and IEquatable interfaces for use of sorting
   and determining if two WordCount objects are equal. */

class WordCount : IComparable<WordCount>, IEquatable<WordCount>
{
    public string word;
    public int count;

    public WordCount()
    {
        // Should never be defaulted
        Console.WriteLine("Logical Coding Error: WordCount without parameters");
    }
    
    /* Defaulted at 1 for simplicity since new words are only made
       when they are seen the first time. */
    public WordCount(string newWord)
    {
        word = newWord;
        count = 1;
    }
    
    public WordCount(string theWord, int value)
    {
        word = theWord;
        count = value;
    }

    /* Goal is to sort by Count, so return
       the integer comparison of the two counts. */
    public int CompareTo(WordCount other)
    {
        return this.count.CompareTo(other.count);
    }

    /* Goal will be to only compare keys, so return
       the string comparison of the two words. */
    public bool Equals(WordCount other)
    {
        return (this.word.Equals(other.word));
    }
}



class Program
{
    static void Main()
    {
        // Done to ensure emperical timing is accurate.
        Process instance = Process.GetCurrentProcess();
        IntPtr value = new IntPtr(1);
        instance.ProcessorAffinity = value;

        // Load in all of the words of each file into a large list.
        try
        {
            string directoryContainingBooks = @"C:\Users\Andy\code\BookReader\books";
            
            DirectoryInfo dirInfo = new DirectoryInfo(directoryContainingBooks);
            
            if (!dirInfo.Exists)
            {
                throw new Exception("Directory " + directoryContainingBooks + " does not exist.");
            }
            
            List<string> words = new List<string>();
            words.Capacity = 4500000;
            
            foreach (FileInfo fileInfo in dirInfo.GetFiles("*.txt"))
            {
                StreamReader sr = new StreamReader(fileInfo.FullName);
                Tokenize(sr.ReadToEnd(), words);
                sr.Close();
            }
            
            Console.WriteLine("Finished reading {0} words.", words.Count);

            /* The Measure method is used for timing word-reading.
               It is a straight-forward method that simply makes something happen
               multiple times and then prints to the console the time required on
               average for this task.
            
               It has the following three parameters:
               The first one is the type of dictionary implementation represented as
               an integer. The second parameter is the list of words to be stored
               into a dictionary. The third is the number of times the storing process
               should be run.

               Each dictionary implementation will be run 15 times to get an average. */
                                  
            Measure(4, words, 15);
            Measure(1, words, 15);
            Measure(2, words, 15);
            Measure(3, words, 15);

            /* The Produce method produces actual output by calling different implementations
               in a similar fashion to Measure. The difference is that these implementatons
               also include the sorting of each dictionary to receive the final output of
               the 20 most common words. */

            // Each dictionary implementation will be run 1 time to get the output it produces.
            Actual(1, words, 1);
            Actual(2, words, 1);
            Actual(3, words, 1);
            Actual(4, words, 1);
        }

        catch (Exception ex)
        {
            Console.Error.WriteLine("Caught unhandled exception: " + ex);
        }

    }

    /* A brief summary of this method was provided above. 
       Comments throughout the method help explain what is going on. */

    static public void Measure(int method, List<string> list, int minSamples)
    {
        Stopwatch watch = new Stopwatch();
        int numSamples = 0;

        // Outputs to the Console which method is being timed.
        Console.WriteLine("Dictionary " + method + ":");

        // Start timing.
        watch.Start();

        /* If the number of times a task is done is less
            than the minimum, Measure will continue to call it in 
            this while loop. */

        while (numSamples < minSamples)
        {
            // Straight-forward way of calling the implementation that is desired.

            if (method == 1)
            {
                Read1(list);
            }
            else if (method == 2)
            {
                Read2(list);
            }
            else if (method == 3)
            {
                Read3(list);
            }
            else if (method == 4)
            {
                Read4(list);
            }

            ++numSamples;
        }

        watch.Stop();

        // Print out the time taken in seconds.
        Console.WriteLine("Time taken:" + (((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) / numSamples));
    }



    /*   
       This method uses an unsorted list to store all the
       words and counts. Both the word and count is stored in
       a WordCount object, as discussed above. */
    
    static void Read1(List<string> list)
    {
        List<WordCount> countWords = new List<WordCount>();

        /* Grab words from the master list in order.
           One-by-one, see if countWords has the word in it.
           If not, add it to the list.
           If so, increment count where it was found.
           Repeat.*/

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];

            if (countWords.Count == 0)
            {
                WordCount newWord = new WordCount(wordToCheck);
                countWords.Add(newWord);
            }

            else
            {
                /* IndexOf uses the Equals method in the WordCount class to
                   search for (in a linear fashion) an index given a WordCount.
                   The Equals method works quite simply. It compares the word
                   of the given WordCount to the word of any other WordCount
                   via the already implemented string comparison method. */

                int index = countWords.IndexOf(new WordCount(wordToCheck));
                if (index == -1)
                {
                    WordCount newWord = new WordCount(wordToCheck);
                    countWords.Add(newWord);
                }
                else
                {
                    countWords[index].count = countWords[index].count + 1;
                }
            }

        }
        Console.WriteLine("Found " + countWords.Count + " words.");
    }

    // This method uses a sorted list of type <string, int> to store all the words and counts.

    static void Read2(List<string> list)
    {
        SortedList<string, int> countWords = new SortedList<string, int>();

        /* Grab words from the master list in order.
           One-by-one, try to increment the count integer
           If countWords has the word in it, this will work.
           If not, this will fail and be caught as an exception.
           Add the word to the list in this case.
           Repeat. */

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];
            if (countWords.Count == 0)
            {
                countWords.Add(wordToCheck, 1);
            }
            else
            {
                bool contains = countWords.ContainsKey(wordToCheck);
                if (contains)
                {
                    countWords[wordToCheck] = countWords[wordToCheck] + 1;
                }
                else
                {
                    countWords.Add(wordToCheck, 1);
                }
            }
        }
        Console.WriteLine("Found " + countWords.Count + " words.");      
    }

    /* This method uses a sorted dictionary of type <string, int> to store all 
       the words and counts. The method works much like Read2, so
       clarification has been omitted. */

    static void Read3(List<string> list)
    {
        SortedDictionary<string, int> countWords = new SortedDictionary<string, int>();

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];
            if (countWords.Count == 0)
            {
                countWords.Add(wordToCheck, 1);
            }
            else
            {
                bool contains = countWords.ContainsKey(wordToCheck);
                if (contains)
                {
                    countWords[wordToCheck] = countWords[wordToCheck] + 1;
                }
                else
                {
                    countWords.Add(wordToCheck, 1);
                }
            }
        }
        Console.WriteLine("Found " + countWords.Count + " words.");  
    }

    /* This method uses a dictionary of type <string, int> to store all 
       the words and counts. This method works much like Read2, so
       clarification has been omitted. */

    static void Read4(List<string> list)
    {

        Dictionary<string, int> countWords = new Dictionary<string, int>();

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];
            if (countWords.Count == 0)
            {
                countWords.Add(wordToCheck, 1);
            }
            else
            {
                bool contains = countWords.ContainsKey(wordToCheck);
                if (contains)
                {
                    countWords[wordToCheck] = countWords[wordToCheck] + 1;
                }
                else
                {
                    countWords.Add(wordToCheck, 1);
                }
            }
        }
        Console.WriteLine("Found " + countWords.Count + " words.");  
    }

    

    /* A brief summary of this method has been provided above.
       Essentially, this method works like Measure, but calls
       different methods which do the full work of finding the
       top twenty words. It also times the methods for extra
       data that is used in the analysis of this project.  */

    static public void Actual(int method, List<string> list, int minSamples)
    {
        Stopwatch watch = new Stopwatch();
        int numSamples = 0;

        Console.WriteLine("Dictionary " + method + ":");

        watch.Start();

        // Works just as Measure does.
        while (numSamples < minSamples)
        {
            if (method == 1)
            {
                Dictionary1(list);
            }
            else if (method == 2)
            {
                Dictionary2(list);
            }
            else if (method == 3)
            {
                Dictionary3(list);
            }
            else if (method == 4)
            {
                Dictionary4(list);
            }

            ++numSamples;
        }

        watch.Stop();

        Console.WriteLine("Time taken:" + (((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) / numSamples));
    }

    /* The following four Dictionary implementations work just like
       their Read counterparts. The only difference is that at the end 
       of each method, the dictionaries are converted into lists
       which are thereby sorted by count. From there, grabbing the
       top twenty occurences is easy, which is done in each method. */

    static void Dictionary1(List<string> list)
    {
        List<WordCount> countWords = new List<WordCount>();
        countWords.Capacity = 80000;

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];

            if (countWords.Count == 0)
            {
                WordCount newWord = new WordCount(wordToCheck);
                countWords.Add(newWord);
            }

            else
            {
                int index = countWords.IndexOf(new WordCount(wordToCheck));
                if (index == -1)
                {
                    WordCount newWord = new WordCount(wordToCheck);
                    countWords.Add(newWord);
                }
                else
                {
                    countWords[index].count = countWords[index].count + 1;
                }
            }

        }

        Console.WriteLine("Found " + countWords.Count + " words.");

        /* Sort the method, and output the last 20 elements.
           Sorting is done via the IComparable interface implemented
           in WordCount. Essentially, the method in the WordCount class
           tells the sorter in the list of WordCount to treat WordCounts
           with higher occurences or counts as something that belongs
           later in the list. This is much like how the int class tells
           the sorter of a list of ints that the number 100 comes later
           than the number 15. */

        countWords.Sort();

        for (int z = countWords.Count - 1; z > countWords.Count - 21; --z)
        {
            Console.WriteLine(countWords[z].word + "\t" + countWords[z].count);
        }

    }

    static void Dictionary2(List<string> list)
    {
        SortedList<string, int> countWords = new SortedList<string, int>();

        countWords.Capacity = 80000;

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];
            if (countWords.Count == 0)
            {
                countWords.Add(wordToCheck, 1);
            }
            else
            {
                bool contains = countWords.ContainsKey(wordToCheck);
                if (contains)
                {
                    countWords[wordToCheck] = countWords[wordToCheck] + 1;
                }
                else
                {
                    countWords.Add(wordToCheck, 1);
                }
            }
        }

        List<WordCount> countWords2 = new List<WordCount>();
        countWords2.Capacity = 80000;

        /* This is done in Dictionary 3 and 4 as well.
           To sort the dictionary, it is first
           copied into a list and then this list is sorted
           and used as in Dictionary 1. */

        foreach (KeyValuePair<string, int> pair in countWords)
        {
            countWords2.Add(new WordCount(pair.Key, pair.Value));
        }

        Console.WriteLine("Found " + countWords2.Count + " words.");

        countWords2.Sort();

        for (int z = countWords2.Count - 1; z > countWords2.Count - 21; --z)
        {
            Console.WriteLine(countWords2[z].word + "\t" + countWords2[z].count);
        }

    }

    static void Dictionary3(List<string> list)
    {
        SortedDictionary<string, int> countWords = new SortedDictionary<string, int>();

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];
            if (countWords.Count == 0)
            {
                countWords.Add(wordToCheck, 1);
            }
            else
            {
                bool contains = countWords.ContainsKey(wordToCheck);
                if (contains)
                {
                    countWords[wordToCheck] = countWords[wordToCheck] + 1;
                }
                else
                {
                    countWords.Add(wordToCheck, 1);
                }
            }
        }

        List<WordCount> countWords2 = new List<WordCount>();
        countWords2.Capacity = 80000;

        foreach (KeyValuePair<string, int> pair in countWords)
        {
            countWords2.Add(new WordCount(pair.Key, pair.Value));
        }

        Console.WriteLine("Found " + countWords2.Count + " words.");

        countWords2.Sort();

        for (int z = countWords2.Count - 1; z > countWords2.Count - 21; --z)
        {
            Console.WriteLine(countWords2[z].word + "\t" + countWords2[z].count);
        }

    }

    static void Dictionary4(List<string> list)
    {

        Dictionary<string, int> countWords = new Dictionary<string, int>();

        for (int i = 0; i < list.Count; ++i)
        {
            string wordToCheck = list[i];
            if (countWords.Count == 0)
            {
                countWords.Add(wordToCheck, 1);
            }
            else
            {
                bool contains = countWords.ContainsKey(wordToCheck);
                if (contains)
                {
                    countWords[wordToCheck] = countWords[wordToCheck] + 1;
                }
                else
                {
                    countWords.Add(wordToCheck, 1);
                }
            }
        }

        List<WordCount> countWords2 = new List<WordCount>();
        countWords2.Capacity = 80000;

        foreach (KeyValuePair<string, int> pair in countWords)
        {
            countWords2.Add(new WordCount(pair.Key, pair.Value));
        }

        Console.WriteLine("Found " + countWords2.Count + " words.");

        countWords2.Sort();

        for (int z = countWords2.Count - 1; z > countWords2.Count - 21; --z)
        {
            Console.WriteLine(countWords2[z].word + "\t" + countWords2[z].count);
        }

    }


    // This function takes a string and breaks it up into words and adds them to the given list.
    static public void Tokenize(string text, List<string> words)
    {
        text = text.ToLower();

        int start = 0;

        int i;
        for (i = 0; i < text.Length; ++i)
        {
            char c = text[i];
            if ( c < 'a' || c > 'z' )
            {
                if (start != i)
                {
                    string token = text.Substring(start, i - start);
                    words.Add(token);
                }
                start = i + 1;
            }
        }

        if (start != i)
        {
            words.Add(text.Substring(start, i - start));
        }
    }

}