using System;

// Point class implements IComparable<Point>, this class acts as TKey in the Hash Table
public class Point : IComparable<Point>
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    // CompareTo method
    // "Assume that points are ordered first on the x-coordinate and secondarily on the y-coordinate."
    public int CompareTo(Point? other)
    {
        // Return 1 if the other point is null (by convention, non-null > null)
        if (other == null) return 1;

        // First, compare X coordinates
        int xComparison = this.X.CompareTo(other.X);
        if (xComparison != 0)
        {
            return xComparison;
        }

        // If X coordinates are equal, compare Y coordinates
        return this.Y.CompareTo(other.Y);
    }

    // Override GetHashCode
    // This ensures points with the same X and Y go to the same bucket.
    public override int GetHashCode()
    {
        // A common technique for combining two integers into a hash code:
        // Multiply the first by a prime number and add the second.
        unchecked // Allow overflow, which is fine for hashing
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }
    }

    // Override Equals
    // This ensures the Hash Table knows when two points are actually identical.
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Point other = (Point)obj;
        return this.X == other.X && this.Y == other.Y;
    }

    // Optional: Useful for outputting results later
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

// Use Generics <TKey, TValue> so this table works with Point or any other comparable object.
// Constraint: TKey must implement IComparable (so we can sort them).
public class HashTable<TKey, TValue> where TKey : IComparable<TKey>
{
    // 1. The Internal Node Class
    private class Node
    {
        public TKey? Key;
        public TValue? Value;
        public Node? Next;

        // Constructor for data nodes
        // Allow next Node to be nullable
        public Node(TKey key, TValue value, Node? next) 
        {
            this.Key = key;
            this.Value = value;
            this.Next = next;
        }

        // Constructor for Header nodes (dummy data)
        public Node()
        {
            this.Next = null; // Next is nullable, so assigning null is valid
            // Key and Value will be default (null/0); both are nullable and safe for header nodes.
        }
    }

    // 2. The Hash Table Data
    private Node[] buckets;
    private int numBuckets;

    // Constructor
    public HashTable(int size)
    {
        this.numBuckets = size;
        buckets = new Node[numBuckets];
        InitializeBuckets();
    }

    // Helper method to create the header nodes
    private void InitializeBuckets()
    {
        for (int i = 0; i < numBuckets; i++)
        {
            // Requirement: Each bucket has a header node.
            buckets[i] = new Node(); 
        }
    }

    // Requirement: MakeEmpty
    // Since we use header nodes, "Empty" means every bucket 
    // contains exactly one node (the header) pointing to nothing.
    public void MakeEmpty()
    {
        InitializeBuckets();
    }

    // Helper: Hash Function
    // Maps a key to an index in the array
    private int GetIndex(TKey key)
    {
        // Math.Abs is crucial because GetHashCode() can return negative numbers
        return Math.Abs(key.GetHashCode()) % numBuckets;
    }

    public void Insert(TKey key, TValue value)
    {
        int index = GetIndex(key);
        Node prev = buckets[index]; // Start at the header node

        // Traverse as long as the NEXT node exists AND is smaller than our new key
        while (prev.Next != null && prev.Next.Key != null && prev.Next.Key.CompareTo(key) < 0)
        {
            prev = prev.Next;
        }

        // CRITICAL CHECK:
        // If we stopped, it's either because we hit the end, 
        // OR the next key is >= our new key.
        
        // Check for Duplicate: If the next key is EQUAL, we can't insert.
        if (prev.Next != null && prev.Next.Key != null && prev.Next.Key.CompareTo(key) == 0)
        {
            throw new ArgumentException("Key already exists in the Hash Table.");
        }

        // Insert the new node between 'prev' and 'prev.Next'
        Node newNode = new Node(key, value, prev.Next);
        prev.Next = newNode;
    }

    public TValue Retrieve(TKey key)
    {
        int index = GetIndex(key);
        Node prev = buckets[index];

        // Walk down the list...
        while (prev.Next != null && prev.Next.Key != null && prev.Next.Key.CompareTo(key) < 0)
        {
            prev = prev.Next;
        }

        // If we found it (Next key equals Search key)
        if (prev.Next != null && prev.Next.Key != null && prev.Next.Key.CompareTo(key) == 0)
        {
            // value shouldn't be null when retrieving
            if (prev.Next.Value is null) 
                throw new Exception("Found node's Value is null (corrupt list or inserted null)");
            return prev.Next.Value;
        }

        // If we get here, it means we either hit the end, or we hit a key BIGGER than ours.
        // In either case, the key is definitely not in the list.
        throw new Exception("Key not found.");
    }

    public void Remove(TKey key)
    {
        int index = GetIndex(key);
        Node prev = buckets[index];

        // Traverse to find the node just BEFORE the one we want to delete
        while (prev.Next != null && prev.Next.Key != null && prev.Next.Key.CompareTo(key) < 0)
        {
            prev = prev.Next;
        }

        // If the next node matches the key, remove it
        if (prev.Next != null && prev.Next.Key != null && prev.Next.Key.CompareTo(key) == 0)
        {
            prev.Next = prev.Next.Next; // Unlink the node
        }
        else
        {
            throw new Exception("Key not found, cannot remove.");
        }
    }

    public void Output()
{
    // Step 1: Collect all nodes from the table
    List<Node> allNodes = new List<Node>();

    for (int i = 0; i < numBuckets; i++)
    {
        Node? current = buckets[i].Next; // Skip the header node (may be null)
        while (current != null)
        {
            allNodes.Add(current);
            current = current.Next;
        }
    }

    // Step 2: Sort the list usinga a "Bubble Sort" loop
    for (int i = 0; i < allNodes.Count; i++)
        {
            for (int j = 0; j < allNodes.Count - 1; j++)
            {
                // We compare the current item (j) with the neighbor to its right (j + 1).
                // We use the CompareTo method we wrote in the Point class.
                
                // If result > 0, it means Item J is larger than Item J+1.
                // This is the WRONG order, so we must swap them.
                // We know allNodes contains only data nodes, so Key is never null here.
                if (allNodes[j].Key!.CompareTo(allNodes[j + 1].Key!) > 0)
                {
                    // Swap Logic:
                    Node temp = allNodes[j];       // 1. Save the current node
                    allNodes[j] = allNodes[j + 1]; // 2. Move the neighbor into the current spot
                    allNodes[j + 1] = temp;        // 3. Put the saved node into the neighbor's spot
                }
            }
        }

    // Step 3: Print them out
    Console.WriteLine("--- Hash Table Contents (Sorted) ---");
    foreach (Node node in allNodes)
    {
        // Assuming Point has a ToString() or we format it manually
        Console.WriteLine($"Key: {node.Key} | Value: {node.Value}");
    }
    Console.WriteLine("------------------------------------");
}
}

public class Program
{
    public static void Main()
    {
        // ---------------------------------------------------------
        // FINAL TEST: OUTPUT (GLOBAL SORT)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- Test 5: Output (Global Sort) ---");
        
        // Create a fresh table
        HashTable<Point, string> outputTable = new HashTable<Point, string>(5);
        
        // Insert points in RANDOM order (not sorted)
        outputTable.Insert(new Point(10, 10), "Medium");
        outputTable.Insert(new Point(100, 100), "Huge");
        outputTable.Insert(new Point(1, 1), "Tiny");
        outputTable.Insert(new Point(50, 50), "Large");
        outputTable.Insert(new Point(5, 5), "Small");

        Console.WriteLine("Calling Output()... (Visually check that order is Tiny -> Small -> Medium -> Large -> Huge)");
        outputTable.Output();
    }
}