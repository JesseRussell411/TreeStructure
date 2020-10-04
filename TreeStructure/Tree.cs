using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Tree
{
    public class UsedBranchException<T> : Exception
    {
        public Branch<T> Branch { get; }
        public UsedBranchException(Branch<T> branch)
            : base($"The branch already belongs somewhere else.")
        {
            Branch = branch;
        }
    }
    public class Tree<T> : IEnumerable<(int[] Address, Branch<T> Branch)>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<(int[] Address, Branch<T> Branch)> GetEnumerator()
        {
            return GetEnumerator(new int[0], Root);
        }
        private IEnumerator<(int[] Address, Branch<T> Branch)> GetEnumerator(int[] address, Branch<T> branch)
        {
            yield return (address, branch);
            foreach(var bi in branch)
            {
                var enumer = GetEnumerator(address.Append(bi.Index).ToArray(), bi.Branch);
                while(enumer.MoveNext())
                {
                    var ba = enumer.Current;
                    yield return ba;
                }
            }
        }
        public List<(int[] Address, Branch<T> Branch)> GetBranchList()
        {
            return GetBranchList(new int[0], Root);
        }
        public List<(int[] Address, Branch<T> Branch)> GetBranchList(int[] address)
        {
            return GetBranchList(address, Get(address));
        }
        private List<(int[] Address, Branch<T> Branch)> GetBranchList(int[] address, Branch<T> branch)
        {
            var list = new List<(int[] Address, Branch<T> Branch)>();
            list.Append((address, branch));
            
            foreach (var bi in branch)
            {
                list.Concat(GetBranchList(address.Append(bi.Index).ToArray(), bi.Branch));
            }
            return list;
        }
        public (int[] address, Branch<T> branch)[] AllBranches
        {
            get
            {
                return GetBranchList().ToArray();
            }
        }
        public Branch<T> Root
        {
            get
            {
                return root;
            }
            set
            {
                root?.RemoveParent();
                root = value;
                root.RemoveParent();
                root.Tree = this;
            }
        }
        private Branch<T> root = null;
        public Tree(Branch<T> root)
        {
            Root = root;
        }
        public Tree(T rootItem)
        {
            Root = new Branch<T>(rootItem);
        }
        public Tree()
        {
            Root = new Branch<T>();
        }
        public Branch<T> Get()
        {
            return Root;
        }
        public T GetItem()
        {
            return Root.Item;
        }
        public Branch<T> Get(int[] address)
        {
            Branch<T> branch = Root;
            
            foreach (int index in address)
            {
                branch = branch.Get(index);
            }
            return branch;
        }
        public T GetItem(int[] address)
        {
            Branch<T> branch = Root;
            
            foreach (int index in address)
            {
                branch = branch.Get(index);
            }
            return branch.Item;
        }
        public override string ToString()
        {
            return ToString(0, Root);
        }
        public string ToString(int[] address)
        {
            return ToString(address.Length, Get(address));
        }
        private string ToString(int depth, Branch<T> branch)
        {
            StringBuilder sb = new StringBuilder();
            void padding(int d)
            {
                for (int i = 0; i < d; i++)
                {
                    sb.Append("  |");
                }
            }

            padding(depth - 1);

            if (depth > 0) { sb.Append($"  |--{(branch.Any() ? '+' : '-')}"); }

            sb.Append(branch.Item.ToString());

            if (branch.Any())
            {
                sb.Append('\n');
                padding(depth + 1);
            }

            foreach (Branch<T> subBranch in branch.Select(ib => ib.Branch))
            {
                sb.Append($"\n{ToString(depth + 1, subBranch)}");
            }
            if (branch.Any())
            {
                //sb.Append('\n');
                //padding(depth + 1);
                sb.Append('\n');
                padding(depth);
                sb.Append("  +------");
                sb.Append('\n');
                padding(depth);
            }

            return sb.ToString();
        }
        public int[] Add(T item)
        {
            return Add(new Branch<T>(item));
        }
        public int[] Add(Branch<T> branch)
        {
            return Add(new int[0], branch);
        }
        public int[] Add(int[] address, T item)
        {
            return Add(address, new Branch<T>(item));
        }
        public int[] Add(int[] address, Branch<T> branch)
        {
            int? index = Get(address).Add(branch);
            if (index == null) { return null; }
            return address.Append((int)index).ToArray();
        }
    }
    public class Branch<T> : IEnumerable<(int Index, Branch<T> Branch)>
    {
        public Branch<T> Parent { get; internal set; } = null;
        public Tree<T> Tree
        {
            get
            {
                return tree;
            }
            set
            {
                tree = value;
                IsRoot = tree.Root == this;
            }
        }
        private Tree<T> tree = null;
        public bool IsRoot { get; private set; } = false;
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<(int Index, Branch<T> Branch)> GetEnumerator()
        {
            foreach (var bPair in branches)
            {
                yield return (Index: bPair.Key, Branch: bPair.Value); // Can I just say that C# continues to amaze me with features like this that just seem to provide the perfect solution to whatever problem you may have.
            }
        }
        public T Item { get; set; } = default(T);
        internal Branch(T item) : this()
        {
            Item = item;
        }
        internal Branch()
        {

        }
        public int Count
        {
            get
            {
                return branches.Count;
            }
        }
        public Branch<T> Get(int i)
        {
            return branches[i];
        }
        public bool ContainsIndex(int index)
        {
            return branches.ContainsKey(index);
        }
        public bool ContainsBranch(Branch<T> branch)
        {
            return branches.ContainsValue(branch);
        }
        public bool Remove(int index)
        {
            Branch<T> branch;
            return Remove(index, out branch);
        }
        public bool Remove(int index, out Branch<T> branch)
        {
            if (branches.Remove(index, out branch))
            {
                NextIndex = index;
                branch.Parent = null;
                branch.Tree = null;
                return true;
            }
            else { return false; }
        }
        public bool Remove(Branch<T> branch)
        {
            return branch.Remove(branch);
        }
        public int Add(T branchItem)
        {
            return (int)Add(new Branch<T>(branchItem));
        }
        public int? Add(Branch<T> branch)
        {
            if (ContainsBranch(branch)) { return null; }
            int i = NextIndex;
            branches.Add(i, branch);
            branch.RemoveParent();
            branch.Parent = this;
            branch.Tree = Tree;
            return i;
        }

        private Dictionary<int, Branch<T>> branches = new Dictionary<int, Branch<T>>();
        private Queue<int> availableIndexes = new Queue<int>();
        private int highestIndex = -1;

        private int NextIndex
        {
            get
            {
                return availableIndexes.Any() ? availableIndexes.Dequeue() : ++highestIndex;
            }
            set
            {
                availableIndexes.Enqueue(value);
            }
        }
        private int NextIndex_Peek()
        {
            return availableIndexes.Any() ? availableIndexes.Peek() : highestIndex + 1;
        }

        public void RemoveParent()
        {
            if (tree != null)
            {
                if (Tree.Root == this)
                {
                    Tree.Root = null;
                }
                tree = null;
            }

            if (Parent != null)
            {
                Parent.Remove(this);
                Parent = null;
            }

            IsRoot = false;
        }
    }
}













//public struct Enumerator<B> : IEnumerator<B> where B : BranchIndexPair<T>
//{
//    public B Current
//    {
//        get
//        {
//            return (B)new BranchIndexPair<T>(branchesEnumerator.Current.Key, branchesEnumerator.Current.Value);
//        }
//    }
//    public bool MoveNext()
//    {
//        return branchesEnumerator.MoveNext();
//    }
//    public void Dispose()
//    {
//        branchesEnumerator.Dispose();
//    }
//    public void Reset()
//    {
//        branchesEnumerator = branchDictionary.GetEnumerator();
//    }
//    public Enumerator(Dictionary<int, Branch<T>> branchDictionary)
//    {
//        this.branchDictionary = branchDictionary;
//        branchesEnumerator = branchDictionary.GetEnumerator();
//    }

//    private Dictionary<int, Branch<T>>.Enumerator branchesEnumerator;
//    private Dictionary<int, Branch<T>> branchDictionary;
//}