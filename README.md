# BookReader

As an experiment, I decided to read a set of freely available books as slow and as fast as possible.

The goal of this project is to show how much better or worse performance is by implementing a dictionary in a naive way (unordered list!), as a SortedList, a SortedDictionary (binary tree with better insertion/removal time than a SortedList), and Dictionary (hash table). For the naive lists without a key-value pair, I made the WordCount class.

The code will index any text file.


RESULTS:

As one would expect, the dictionaries were tested in increasing order of efficiency. It took about 1.36 seconds for the hash table implementation and 788.35 seconds for the naive one to read a book with 52071 words. A trial was taken 15 times to obtain this average.
