using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tree;
namespace TreeStructure
{
    public static class ToStringUtils
    {
        public static string ToDelimString<T>(this T enumerable, string delim) where T : IEnumerable
        {
            StringBuilder builder = new StringBuilder();

            var enu = enumerable.GetEnumerator();
            if (enu.MoveNext()) { builder.Append(enu.Current?.ToString()); } // "bob
            while (enu.MoveNext())
            {
                builder.Append(delim); // "bob, 
                builder.Append(enu.Current.ToString()); // "bob, bob
            }
            // "bob, bob, bob, bob... bob"

            return builder.ToString();
        }
        public static string NullableToString(this object self)
        {
            return self.NullableToString("null");
        }
        public static string NullableToString(this object self, string nullReplacementString)
        {
            if (self == null)
            {
                return nullReplacementString;
            }
            else
            {
                string self_ToString = self.ToString();
                return self_ToString.Length == 0 ? nullReplacementString : self_ToString;
            }
        }
    }
    public static class TreeUtils
    {
        public static (int[], T[]) Flatten<T>(Tree<T> tree)
        {
            var flat = FlattenHelper(tree.Root, new Dictionary<int, int>(), new Dictionary<int, T>(), -1);

            return (
                flat.Item1.OrderBy(p => p.Key).Select(p => p.Value).ToArray(),
                flat.Item2.OrderBy(p => p.Key).Select(p => p.Value).ToArray()
                );
        }
        private static (Dictionary<int, int>, Dictionary<int, T>) FlattenHelper<T>(Branch<T> branch, Dictionary<int, int> skeleton, Dictionary<int, T> meat, int parent)
        {
            int i = skeleton.Count == 0 ? 0 : skeleton.Keys.Max() + 1;
            skeleton.Add(i, parent);
            meat.Add(i, branch.Item);
            foreach(var child in branch)
            {
                FlattenHelper(child.Branch, skeleton, meat, i);
            }

            return (skeleton, meat);
        }

        public static Dictionary<int, List<int>> ToRootDown(int[] skeleton)
        {
            Dictionary<int, List<int>> rootDownSkeleton = new Dictionary<int, List<int>>();

            for (int i = 0; i < skeleton.Length; i++)
            {
                if (!rootDownSkeleton.TryAdd(skeleton[i], new List<int> { i }))
                {
                    rootDownSkeleton[skeleton[i]].Add(i);
                }
            }
            return rootDownSkeleton;
        }

        public static Tree<T> UnFlatten<T>(Dictionary<int, List<int>> skeleton, T[] meat)
        {
            int root = skeleton[-1][0];
            Tree<T> tree = new Tree<T>(meat[root]);

            UnFlattenHelper(skeleton, meat, root, tree.Root);

            return tree;
        }

        private static void UnFlattenHelper<T>(Dictionary<int, List<int>> skeleton, T[] meat, int parentIndex, Branch<T> parent)
        {
            if (skeleton.ContainsKey(parentIndex))
            {
                List<int> parentList = skeleton[parentIndex];
                for (int i = 0; i < parentList.Count; i++)
                {
                    int j = parent.Add(meat[parentList[i]]);
                    UnFlattenHelper(skeleton, meat, parentList[i], parent.Get(j));
                }
            }
            else
            {
                //pass, this node has no children and will die alone.
            }
        }
    }
}
