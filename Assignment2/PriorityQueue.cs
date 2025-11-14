namespace PartA
{
    public class PriorityQueue
    {
        private int[,] H; // Heap matrix
        private int numRows; // Number of rows
        private int numCols; // Number of columns
        private int size; // Current number of elements in the heap
        // 1 mark
        // Initialize an m x n Heap matrix
        public PriorityQueue(int m, int n)
        {
            numRows = m;
            numCols = n;
            H = new int[m, n];
            size = 0; // Initialize size to 0

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    H[i, j] = int.MinValue; // Initialize all elements to minimum value
                }
            }
        }

        // Calculates the (row, col) position in the matrix for inserting the next element
        public (int row, int col) IndexToPos(int size)
        {
            int row = size / numCols; // how many elements / how many columns gives us row number
            int col = size % numCols; // how many columns / how many elements gives us col number

            return (row, col);
        }

        public void BubbleUp(int row, int col)
        {
            // track the best row and column
            int bestRow = row;
            int bestCol = col;

            while (true)
            {
                if (row - 1 >= 0 && H[row - 1, col] > H[row, col]) // check if the element above is bigger
                {
                    bestRow = row - 1;
                    bestCol = col;
                }

                if (col - 1 >= 0 && H[bestRow, bestCol - 1] > H[bestRow, bestCol]) // check if the element to the left is bigger
                {
                    bestRow = row;
                    bestCol = col - 1;
                }

                if (row == bestRow && col == bestCol)
                {
                    break;
                }

                // swap the elements
                int temp = H[row, col];
                H[row, col] = H[bestRow, bestCol];
                H[bestRow, bestCol] = temp;

                // update the row and column to continue the bubble up process
                row = bestRow;
                col = bestCol;
            
            }
        }

        // 6 marks
        // Insert an entry into the priority queue
        public void Insert(int item)
        {
            // get the position to insert the element
            var (row, col) = IndexToPos(size);
            H[row, col] = item;
            // bubble up the element to maintain the heap property
            BubbleUp(row, col);

            size++;
            Console.WriteLine($"[Checkpoint] Inserted {item}:");
            Print();
        }

        // 6 marks
        // Remove the item with the highest priority (6 marks)
        public void Remove()
        {
            if (size == 0)
            {
                Console.WriteLine("Priority Queue is empty. Cannot remove element.");
                return;
            }

            // Replace the root of the heap with the last element
            var (lastRow, lastCol) = IndexToPos(size - 1);

            H[0, 0] = H[lastRow, lastCol];
            H[lastRow, lastCol] = int.MinValue; // Mark the last position as empty
            size--;

            // Bubble down the new root element to maintain the heap property
            BubbleDown(0, 0);

            Console.WriteLine($"[Checkpoint] Removed element:");
            Print();
        }

        // BubbleDown ensures the min-heap property is restored by moving the element at (row, col)
        // down the heap to its proper position, swapping with the smallest child as necessary.
        public void BubbleDown(int row, int col)
        {
            while (true)
            {
                int bestRow = row;
                int bestCol = col;

                // Check the element below (down)
                if (row + 1 < numRows && H[row + 1, col] != int.MinValue && H[row + 1, col] < H[bestRow, bestCol])
                {
                    bestRow = row + 1;
                    bestCol = col;
                }
                // Check the element to the right (right)
                if (col + 1 < numCols && H[row, col + 1] != int.MinValue && H[row, col + 1] < H[bestRow, bestCol])
                {
                    bestRow = row;
                    bestCol = col + 1;
                }
                // If no swap needed, exit loop
                if (bestRow == row && bestCol == col)
                    break;

                // Swap the current element with the best (smallest) child
                int temp = H[row, col];
                H[row, col] = H[bestRow, bestCol];
                H[bestRow, bestCol] = temp;

                // Move to the best child
                row = bestRow;
                col = bestCol;
            }
        }

        // 1 mark
        // Return the item with the highest priority (1 mark)
        public int Front()
        {
            return H[0, 0];
        }
        // 4 marks
        // Return true if the item is found in the priority queue; false otherwise (4 marks)
        public bool Found(int item)
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (H[i, j] == item)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        // 2 marks
        // Return the number of items in the priority queue (2 marks)
        public int Size()
        {
            return size;
        }
        // 2 marks
        // Output the Heap matrix (2 marks)
        public void Print()
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (H[i, j] == int.MinValue)
                    {
                        Console.Write("∞\t");
                    }
                    else
                    {
                        Console.Write(H[i, j] + "\t");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}