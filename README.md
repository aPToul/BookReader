# BookReader

As part of my Algorithms and  Data Structures class, I measured the performance of different implementations of a dictionary in C#.

CODE SUMMARY:

The efficiency of storing the number of instances each word in a book is compared across the following classes:
- SortedList
- SortedDictionary (binary tree)
- Dictionary (hash table)

A trial is taken 15 times to obtain an average.

RESULTS:

On average, it took about 1.36 seconds for the hash table, 33.5 seconds for the binary tree, and 38.8 seconds for the sorted list to count the words in a novel with 52,071 words. For fun, an unsorted list was used. It took a whopping 788.4 seconds.

Note that technically this is not indexing a book, but that word is sometimes used. To actually index the book, one could use chaining to add a list of positions for each word found.

