using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RangeTree;

namespace GeneFinder.Models
{
    //The way in which intervals are compared
    public class ItemRangeComparer : IComparer<ItemRange>
    {
        public int Compare(ItemRange x, ItemRange y)
        {
            return x.Range.CompareTo(y.Range);
        }
    }

    //An interval-like class, with an identificator (key)
    public class ItemRange : IRangeProvider<int>
    {
        public Range<int> Range { get; set; }
        public int Key { get; set; }
        //Class constructor in case that we pass an interval on the form [a,b]
        public ItemRange(int a, int b)
        {
            Range = new Range<int>(a, b);
            Key = (a + b) / 2;
        }
        //Class constructor in for the case, in which we pass an interval on the form -- key [a,b] ---
        //This will be relevant in future uses for the class
        public ItemRange(int a, int b, int key)
        {
            Range = new Range<int>(a, b);
            Key = key;
        }
        //print the interval
        public override string ToString()
        {
            return string.Format("{0}  ({1}  -  {2})", Key, Range.From, Range.To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;

            return Equals((ItemRange)obj);
        }

        public static bool operator ==(ItemRange left, ItemRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ItemRange left, ItemRange right)
        {
            return !Equals(left, right);
        }
    }

    //Public implementation of an interval tree for our purposes
    public class IntervalTree
    {
        public RangeTree<int, ItemRange> tree { get; set; }

        public void make(List<ItemRange> input)
        {
            tree = new RangeTree<int, ItemRange>(new ItemRangeComparer());
            foreach (ItemRange i in input)
                tree.Add(i);
            tree.Rebuild();
        }

        //find all intervals that overlaps with [a,b]
        public List<int> findOverlap(int a, int b)
        {
            IEnumerable<ItemRange> result = tree.Query(new Range<int>(a, b));
            List<int> output = new List<int>();
            foreach (ItemRange i in result)
                output.Add(i.Key);
            return output;
        }

        //Finds all the intervals which properly contain [a,b]
        public List<int> findContained(int a, int b)
        {
            IEnumerable<ItemRange> result = tree.Query(new Range<int>(a, b));
            List<int> output = new List<int>();
            foreach (ItemRange i in result)
                if (i.Range.From <= a && b <= i.Range.To)
                    output.Add(i.Key);
            return output;
        }


    }
}
