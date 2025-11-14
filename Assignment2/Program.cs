using System;
using PartA;

class Program
{
    static void Main()
    {
        int rows, cols;
        Console.WriteLine("How many rows?");
        rows = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("How many columns?");
        cols = Convert.ToInt32(Console.ReadLine());

        // Declare pq as nullable and initialize to null
        PriorityQueue pq = new PriorityQueue(rows, cols);

        while (true)
        {
            Console.WriteLine("\n1. Insert");
            Console.WriteLine("2. Remove");
            Console.WriteLine("3. Print");
            Console.WriteLine("4. Front");
            Console.WriteLine("5. Found");
            Console.WriteLine("6. Size");
            Console.WriteLine("7. Exit");
            Console.WriteLine("Enter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    int item ;
                    Console.WriteLine("Enter the item to insert: ");
                    item = Convert.ToInt32(Console.ReadLine());
                    // int item = Convert.ToInt32(Console.ReadLine());
                    // pq.Insert(item);
                    pq.Insert(item);
                    break;
                case 2:
                    pq.Remove();
                    break;
                case 3:
                    pq.Print();
                    break;
                case 4:
                    Console.WriteLine($"Front item: {pq.Front()}");
                    break;
                case 5: 
                    int itemToFind;
                    Console.WriteLine("Enter the item to find: ");
                    itemToFind = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine($"Item found: {pq.Found(itemToFind)}");
                    break;
                case 6:
                    Console.WriteLine($"Size: {pq.Size()}");
                    break;
                case 7:
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}