SortVis
=======

A visualization of various sorting algorithms.

Coding
------

To implement a new sort algorithm, derive from `SorterBase` and use `CompareInArray` and `CompareNum` to compare
elements or numbers. This way, your compares are counted. To modify the array during sorting, use one of
`Shift`, `Write` and `Swap`, depending on your needs. That way writes to the array are counted, too (swap counts
twice). If you internally do writes to a temporary array, increment the property `Writes` accordingly.

Additionally, these three writing functions synchronize the sorter threads and trigger drawing of the arrays.

The run time information is derived from the assembly, a sorter is put in.

For starters take a look at the popular—but bad—teaching example `BubbleSort`.

Plugin mechanism
----------------

When adding a new sorter it will be
discovered automatically. On the other hand it will not be loaded if it is defective.