using System;
using System.Collections.Generic;

class Node : IComparable
{
    public char Character { get; set; }
    public int Frequency { get; set; }
    public Node? Left { get; set; }
    public Node? Right { get; set; }

    public Node(char character, int frequency, Node? left, Node? right)
    {
        Character = character;
        Frequency = frequency;
        Left = left;
        Right = right;
    }

    // Task 1 (Part of implementation): CompareTo for priority queue ordering
    // Lower frequency = higher priority (lower CompareTo result)
    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;
        
        Node? other = obj as Node;
        if (other == null)
            throw new ArgumentException("Object is not a Node");

        return this.Frequency.CompareTo(other.Frequency);
    }
}

class Huffman
{
    private Node? HT; // Huffman tree to create codes and decode text
    private Dictionary<char, string> D; // Dictionary to store the codes for each character

    // Constructor
    // Invokes AnalyzeText, Build and CreateCodes
    public Huffman(string S)
    {
        int[] frequencies = AnalyzeText(S);
        Build(frequencies);
        D = new Dictionary<char, string>();
        CreateCodes();
    }

    // Task 1: Return the frequency of each character in the given text
    // Printable ASCII characters are codes 32 to 126 (95 total characters)
    private int[] AnalyzeText(string S)
    {
        int[] frequencies = new int[95]; // ASCII 32-126
        
        foreach (char c in S)
        {
            if (c >= 32 && c <= 126)
            {
                frequencies[c - 32]++;
            }
        }
        
        return frequencies;
    }

    // Task 2: Build a Huffman tree based on the character frequencies greater than 0
    private void Build(int[] F)
    {
        PriorityQueue<Node, Node> PQ = new PriorityQueue<Node, Node>();

        // Populate the priority queue with individual nodes for characters with frequency > 0
        for (int i = 0; i < F.Length; i++)
        {
            if (F[i] > 0)
            {
                char character = (char)(i + 32); // Convert index back to ASCII character
                Node node = new Node(character, F[i], null, null);
                PQ.Enqueue(node, node); // Use node as both element and priority
            }
        }

        // Build the Huffman tree by combining nodes with lowest frequencies
        while (PQ.Count > 1)
        {
            Node left = PQ.Dequeue();
            Node right = PQ.Dequeue();

            // Create parent node with combined frequency
            int combinedFrequency = left.Frequency + right.Frequency;
            Node parent = new Node('\0', combinedFrequency, left, right);

            PQ.Enqueue(parent, parent);
        }

        // The last node in the queue is the root of the Huffman tree
        HT = PQ.Dequeue();
    }

    // Task 3: Create the code of 0s and 1s for each character by traversing the Huffman tree
    // Store the codes in Dictionary D using the char as the key
    private void CreateCodes()
    {
        D = new Dictionary<char, string>();
        if (HT != null)
        {
            CreateCodesHelper(HT, "");
        }
    }

    // Helper method for CreateCodes - recursive prefix traversal
    private void CreateCodesHelper(Node? node, string code)
    {
        if (node == null)
            return;

        // If it's a leaf node, store the code
        if (node.Left == null && node.Right == null)
        {
            D[node.Character] = code.Length > 0 ? code : "0"; // Handle single character case
            return;
        }

        // Traverse left (add 0)
        CreateCodesHelper(node.Left, code + "0");

        // Traverse right (add 1)
        CreateCodesHelper(node.Right, code + "1");
    }

    // Task 4: Encode the given text and return a string of 0s and 1s
    public string Encode(string S)
    {
        string encoded = "";
        
        foreach (char c in S)
        {
            if (D.ContainsKey(c))
            {
                encoded += D[c];
            }
        }
        
        return encoded;
    }

    // Task 5: Decode the given string of 0s and 1s and return the original text
    public string Decode(string S)
    {
        string decoded = "";
        Node? current = HT;

        if (current == null)
            return "";

        // Handle single character case
        if (current.Left == null && current.Right == null)
        {
            // The entire encoded string represents repetitions of the same character
            for (int i = 0; i < S.Length; i++)
            {
                decoded += current.Character;
            }
            return decoded;
        }

        // Traverse the tree following the path of 0s and 1s
        foreach (char bit in S)
        {
            if (current == null)
                break;

            if (bit == '0')
            {
                current = current.Left;
            }
            else if (bit == '1')
            {
                current = current.Right;
            }

            // When we reach a leaf node, we've found a character
            if (current != null && current.Left == null && current.Right == null)
            {
                decoded += current.Character;
                current = HT; // Reset to root for next character
            }
        }

        return decoded;
    }
}

// Main program for testing
class Program
{
    static void Main()
    {
        Console.Clear();
        // Get user input
        Console.Write("Enter a string to encode: ");
        string? text = Console.ReadLine();
        
        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine("Error: Input cannot be empty.");
            return;
        }
        
        Huffman huffman = new Huffman(text);
        
        string encoded = huffman.Encode(text);
        string decoded = huffman.Decode(encoded);
        
        Console.WriteLine($"\nOriginal: {text}");
        Console.WriteLine($"Encoded: {encoded}");
        Console.WriteLine($"Decoded: {decoded}");
        Console.WriteLine($"Match: {text == decoded}");
    }
}