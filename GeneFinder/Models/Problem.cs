using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Problem
    {
        public enum Subject { H, C, R, U };

        string name;
        Sequence human;
        Sequence chimp;
        Sequence rhesus;
        static Object lockObj = new Object();

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Sequence Human
        {
            get
            {
                return this.human;
            }
        }

        public Sequence Chimp
        {
            get
            {
                return this.chimp;
            }
        }

        public Sequence Rhesus
        {
            get
            {
                return this.rhesus;
            }
        }

        public int Length
        {
            get
            {
                return this.human.Length;
            }
        }

        public Problem(string name, string human, string chimp, string rhesus)
        {
            //Console.WriteLine ("name={0}\nhuman={1}\nchimp={2}\nrhesus={3}", name, human, chimp, rhesus);
            this.name = name;
            this.human = new Sequence(human);
            this.chimp = new Sequence(chimp);
            this.rhesus = new Sequence(rhesus);
        }

        public static Problem ReadProblem(StreamReader fastaStream)
        {
            string[] sequences = new string[3];
            string name;
            int iSequence = 0;
            string temp;
            int peek;
            lock (lockObj)
            {
                if (fastaStream.Peek() == -1)
                {
                    return null;
                }
                name = fastaStream.ReadLine().TrimStart('>');
                while (iSequence < 3 && (peek = fastaStream.Peek()) > -1)
                {
                    if (peek != '>')
                    {
                        temp = fastaStream.ReadLine();
                        sequences[iSequence] += temp.TrimEnd('\n');
                    }
                    else
                    {
                        iSequence++;
                        if (iSequence < 3)
                            temp = fastaStream.ReadLine();
                    }
                }
            }
            return new Problem(name, sequences[0], sequences[1], sequences[2]);
        }

        internal static Problem ReadProblem(PossibleSmorf pSmorf)
        {
            //string[] sequences = new string[3];
            string name;
            //int iSequence = 0;
            //string temp;
            //int peek;
            if (pSmorf == null)
            {
                return null;
            } 
            name = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", pSmorf.id, pSmorf.chromosome, pSmorf.strand, pSmorf.startPosition, pSmorf.endPosition, pSmorf.sequenceLength, pSmorf.similarity, pSmorf.startCodon, pSmorf.stopCodon, pSmorf.sourceFile);
            //lock (lockObj)
            //{
                
            //    //while (iSequence < 3 && (peek = fastaStream.Peek()) > -1)
            //    //{
            //    //    if (peek != '>')
            //    //    {
            //    //        temp = fastaStream.ReadLine();
            //    //        sequences[iSequence] += temp.TrimEnd('\n');
            //    //    }
            //    //    else
            //    //    {
            //    //        iSequence++;
            //    //        if (iSequence < 3)
            //    //            temp = fastaStream.ReadLine();
            //    //    }
            //    //}
            //}
            return new Problem(name, pSmorf.specie1Content, pSmorf.specie2Content, pSmorf.specie3Content);
        }
    }
}
