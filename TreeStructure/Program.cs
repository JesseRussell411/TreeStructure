using System;
using System.Linq;
using System.Threading;
using Tree;

namespace TreeStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            Branch<int> b = new Branch<int>();
            Tree<string> tree = new Tree<string>(new Branch<string>("this is the root"));

            tree.Root.Add("This is a sub-Branch - apple");
            tree.Root.Add("This is a sub-Branch - banana");
            tree.Root.Add("This is a sub-Branch - coconut");
            tree.Root.Add("This is a sub-Branch - onion");
            tree.Root.Get(0).Add("This is a sub-sub-Branch - strawberry");
            tree.Root.Get(0).Add("This is a sub-sub-Branch - kiwi");
            tree.Root.Get(0).Add("This is a sub-sub-Branch - grape");
            tree.Root.Get(0).Get(0).Add("This is a sub-sub-sub-Branch - orange");
            tree.Root.Get(0).Get(0).Add("This is a sub-sub-sub-Branch - pinapple");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            tree.Add(new int[] { 0, 0, 1 }, "Hello World!");
            Console.WriteLine(tree);

            //foreach(var ba in bob)
            //{
            //    Console.WriteLine($"{ba.Address.ToDelimString(", ")}:  {ba.Branch.Item}");
            //}

            var flatTree = TreeUtils.Flatten(tree);

            Console.WriteLine(flatTree.Item1.Select(i => i.ToString()).Aggregate((a, c) => $"{a},{c}"));

            Tree<string> newBob = TreeUtils.UnFlatten(TreeUtils.ToRootDown(flatTree.Item1), flatTree.Item2);
            Console.WriteLine(newBob);
        }
    }
}
