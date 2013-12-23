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

For starters take a look at the popular—but bad—teaching example `BubbleSort`.

Plug-in mechanism
----------------

When adding a new sorter it will be
discovered automatically. On the other hand it will not be loaded if it is defective.

Big-O run time
--------------

The run time information is taken from the `LambdaAttribute` on the assembly, a sorter is put in.
So watch where you put the code.

Running
-------

Select a number generator and the length of the array that should be sorted. Then select which ones should
participate by checking the first column and click “Start all”. You can also run only a single sorter by
double-clicking its row.

An (un)check all functionality is available when clicking on the “Run” table header.

To run the sorters without animation—thus much faster and more suitable for a benchmark—click the start button
holding shift. To run a single sorter without animation hold Alt when double-clicking.