# BookReader

As part of my Data Structures and Algorithms class, I measured the performance of different implementations of a dictionary in C#.

CODE SUMMARY:

The efficiency of storing the number of instances each word in a book is compared across the following classes:
- SortedList
- SortedDictionary (binary tree)
- Dictionary (hash table)

To actually create an index, chaining a list of positions can be used.


RESULTS:

On my computer, it took about 1.36 seconds for the hash table, 33.5 seconds for the binary tree and 38.8 seconds for the sorted list to count words in a novel of 52,071 words. A trial was taken 15 times to obtain these averages. For fun, an unsorted list was implemented. It took a whopping 788.4 seconds.

