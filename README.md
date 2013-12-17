SortVis
=======

A visualization of various sorting algorithms.

To implement a new sort algorithm, derive from `SorterBase` and use `CompareInArray` and `CompareNum` to compare
elements or numbers. This way, your compares are counted. To modify the array during sorting, use one of
`Shift`, `Write` and `Swap`, depending on your needs. That way writes to the array are counted, too (swap counts
twice). If you interally do writes to a temporary array, increment the property `Writes` accordingly.

Additionally, these three writeing functions synchronize the sorter threads and trigger drawing of the arrays.

The run time information is derived from the assembly, a sorter is put in.

For starters take a look at the popular—but bad—teaching example `BubbleSort`.